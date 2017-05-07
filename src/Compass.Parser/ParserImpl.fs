module internal Compass.Parser.ParserImpl

open FParsec    
open Compass.Parser

let ws = many (anyOf " \t") >>% ()

let strws st = pstring st .>> ws
let skip p = p >>. (preturn ())
let parsec = new ParserCombinator()

let full term = ws >>. term .>> eof 

let integer = (many1Satisfy isDigit .>> ws) |>> int

let floating = (many1Satisfy isDigit) .>>. (((pchar '.') >>. many1Satisfy isDigit) <|> (preturn "0")) .>> ws |>> (fun (a,b) -> float (a + "." + b))

let varname = many1Satisfy2L isAsciiUpper (fun c -> isLetter c || isDigit c || c = '\'') "variable name" .>> ws

(* Fwd decl *)

let expression, expressionref = createParserForwardedToRef<Expression, unit>()
let intersections, intersectionsref = createParserForwardedToRef<Expression, unit>()

(* Statement parser *)

type DispType = Disp | Silenced

let pos : Position -> int option = fun ln -> Some (int (ln.Line))

let commentEol = pstring "//" >>. skipRestOfLine false
let eofOrNewline = (eof) <|> (skip newline)

let stmsep = (opt commentEol >>. eofOrNewline >>. ws >>% Disp) <|> ((strws "," <|> strws ".") >>% Disp) <|> (strws ";" >>% Silenced)

let emptyStm : Parser<Statement list,unit> = notEmpty (ws >>. (opt commentEol) >>. eofOrNewline >>. ws >>% [])

let pntdef =
    let pntdefint = parsec { 
        let! name = varname
        let! (x,y) = between (strws "(") (strws ")") ((floating .>> strws "|") .>>. floating)
        let! ln = getPosition
        let! sep = stmsep
        return match sep with
               | Silenced -> [Assignment(name, NamePnt(name,CPoint(x,y)), pos ln)]
               | Disp     -> [Assignment(name, NamePnt(name,CPoint(x,y)), pos ln); Display(Var(name), pos ln)]
    }
    pntdefint <?> "point definition"

let assignment =
    let assignmentint = parsec {
        let! ln = getPosition 
        let! ((var, expn), sep) = (attempt (varname .>> strws "=")) .>>. expression .>>. stmsep
        return match sep with
               | Silenced -> [Assignment(var,expn, pos ln)]
               | Disp     -> [Assignment(var,expn, pos ln); Display(Var(var), pos ln)]
    }
    assignmentint <?> "assignment"

let namedAssignment =
    let namedAssignmentInt = parsec { 
        let! ln = getPosition 
        let! ((var, expn), sep) = ((between (pstring "\"") (strws "\"") varname) .>> strws "=") .>>. expression .>>. stmsep
        return match sep with
               | Silenced -> [Assignment(var,NamePnt(var, expn), pos ln)]
               | Disp     -> [Assignment(var,NamePnt(var, expn), pos ln); Display(Var(var), pos ln)]
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
       let! ln = getPosition
       let! pickVars = between (strws "{") (strws "}") (sepBy1 pickVarname (strws ","))
       let vars = pickVars |> List.map (fun v -> v.VarName)

       let nameStms = [
           for var in pickVars do
               match var with
               | Quoted(n) -> yield Assignment(n, NamePnt(n, Var(n)), pos ln)
               | _         -> ()
       ]

       do! skip (strws "<-")
       let! (expn, sep) = expression .>>. stmsep

       return match sep with
              | Silenced -> (Pick(vars, expn, pos ln)) :: nameStms
              | Disp     -> (Pick(vars, expn, pos ln)) :: nameStms @ [ for var in vars do yield Display(Var(var), pos ln) ]
    }
    pickint <?> "pick expression"

let blindExpn = 
    let blindint = parsec {
       let! ln   = getPosition
       let! expr = intersections
       let! sep  = stmsep

       return match sep with
              | Silenced -> [Assignment("ans", expr, pos ln)]
              | Disp     -> [Assignment("ans", expr, pos ln); Display(Var("ans"), pos ln)]
    }
    blindint <?> "raw expression"   

let statement = (namedAssignment <|> (attempt pick) <|> (attempt pntdef) <|> assignment <|> blindExpn) <?> "statement"
let statements = (many (statement <|> emptyStm)) |>> List.concat

(* Expression parser *)

let var = varname |>> Var

let sub = parsec {
    let! pars  = (strws "proc" >>. between (strws "(") (strws ")") (sepBy varname (strws ",")))
    let! cont = statements
    do! skip (strws "end")
    return! (
        if cont.Length = 0 then
            fail("Proc body may not be empty")
        else
            preturn (Sub(pars, cont)))
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
