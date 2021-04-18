module internal Parser.ParserImpl

open FParsec    
open Parser

let ws = spaces >>. opt (pstring "//" >>. skipRestOfLine true) >>. spaces >>% ()

let strws st = pstring st .>> ws
let skip p = p >>. (preturn ())
let parsec = new ParserCombinator()

let full term = ws >>. term .>> eof 

let integer = (many1Satisfy isDigit .>> ws) |>> int

let varname = many1Satisfy2L isAsciiUpper (fun c -> isLetter c || isDigit c || c = '\'') "variable name" .>> ws

(* Fwd decl *)

let expression, expressionref = createParserForwardedToRef<Expression, unit>()
let intersections, intersectionsref = createParserForwardedToRef<Expression, unit>()

(* Statement parser *)

type DispType = Disp | Silenced

let stmsep = (newline >>. ws >>% Disp) <|> ((strws "," <|> strws ".") >>% Disp) <|> (eof >>% Disp) <|> (strws ";" >>% Silenced)

let pntdef =
    let pntdefint = parsec { 
        let! name = varname
        let! (x,y) = between (strws "(") (strws ")") ((integer .>> strws "|") .>>. integer)
        let! sep = stmsep
        return match sep with
               | Silenced -> [Assignment(name, NamePnt(name,CPoint(x,y)))]
               | Disp     -> [Assignment(name, NamePnt(name,CPoint(x,y))); Display(Var(name))]
    }
    pntdefint <?> "point definition"

let assignment =
    let assignmentint = parsec { 
        let! ((var, expn), sep) = (attempt (varname .>> strws "=")) .>>. expression .>>. stmsep
        return match sep with
               | Silenced -> [Assignment(var,expn)]
               | Disp     -> [Assignment(var,expn); Display(Var(var))]
    }
    assignmentint <?> "assignment"

let namedAssignment =
    let namedAssignmentInt = parsec { 
        let! ((var, expn), sep) = ((between (pstring "\"") (strws "\"") varname) .>> strws "=") .>>. expression .>>. stmsep
        return match sep with
               | Silenced -> [Assignment(var,NamePnt(var, expn))]
               | Disp     -> [Assignment(var,NamePnt(var, expn)); Display(Var(var))]
    }
    namedAssignmentInt <?> "named assignment"

type PickVar = Plain of string | Quoted of string
    with
        member this.VarName = 
            match this with
            | Plain(n) -> n
            | Quoted(n) -> n
let pickVarname = ((between (pstring "\"") (strws "\"") varname) |>> Quoted) <|> (varname |>> Plain)


let pick =
    let pickint = parsec {
       let! pickVars = between (strws "{") (strws "}") (sepBy1 pickVarname (strws ","))
       let vars = pickVars |> List.map (fun v -> v.VarName)

       let nameStms = [
           for var in pickVars do
               match var with
               | Quoted(n) -> yield Assignment(n, NamePnt(n, Var(n)))
               | _         -> ()
       ]

       do! skip (strws "<-")
       let! (expn, sep) = expression .>>. stmsep

       return match sep with
              | Silenced -> (Pick(vars, expn)) :: nameStms
              | Disp     -> (Pick(vars, expn)) :: nameStms @ [ for var in vars do yield Display(Var(var)) ]
    }
    pickint <?> "pick expression"

let blindExpn = 
    let blindint = parsec {
       let! expr = intersections
       let! sep = stmsep

       return match sep with
              | Silenced -> [Assignment("ans", expr)]
              | Disp     -> [Assignment("ans", expr); Display(Var("ans"))]
    }
    blindint <?> "raw expression"   

let statement = (namedAssignment <|> (attempt pick) <|> (attempt pntdef) <|> assignment <|> blindExpn) <?> "statement"
let statements = many1 statement |>> List.concat 

(* Expression parser *)

let var = varname |>> Var

let sub = parsec {
    let! pars  = (strws "proc" >>. between (strws "(") (strws ")") (sepBy varname (strws ",")))
    let! cont = statements
    do! skip (strws "end")
    return Sub(pars, cont)
}

let set = between (strws "{") (strws "}") (sepBy intersections (strws ",")) |>> Set
let atom = var <|> between (strws "(") (strws ")") expression <|> set

let ctor =
    parsec {
        let! exp1 = atom
        return! choice [
            strws "°"   >>. atom |>> (fun exp2 -> CCircle(exp1, exp2))
            attempt (strws "--") >>. atom |>> (fun exp2 -> CRay(exp1, exp2))
            attempt (strws "->") >>. atom |>> (fun exp2 -> CHalfRay(exp1, exp2))
            strws "-"   >>. atom |>> (fun exp2 -> CLine(exp1, exp2))
            between (strws "(") (strws ")") (sepBy expression (strws ",")) |>> (fun args -> Funcall(exp1,args))
            preturn exp1
        ]
    }

intersectionsref := parsec {
    let! exp1 = ctor
    let! exprs = many ((anyOf "nu\\" .>> ws) .>>. expression)
    return List.fold (fun exp (op, arg) -> 
                          match op with
                          | 'n' -> Cap(exp,arg)
                          | 'u' -> Cup(exp,arg)
                          | '\\' -> Minus(exp,arg)) exp1 exprs
} 

expressionref := sub <|> intersections

let rec repl() = 
    printf "> "
    let input = System.Console.ReadLine().Trim() 
    if input <> "" && input <> ":q" then
        match run (full statements) input with
            | Success(term, _, _)  -> 
                printfn "> %A" term
            | Failure(msg, _, _) -> printfn "Failure %A" msg
        repl()
    else
        ()
