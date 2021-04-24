module internal Parser.ParserImpl

open FParsec    
open Parser

let ws = skipMany (anyOf " \t") 
let commentLine = pstring "//" >>. skipRestOfLine true

let strws st = pstring st .>> ws
let wstrws st = ws >>. pstring st .>> ws
let skip p = p >>. (preturn ())
let parsec = new ParserCombinator()

let integer = (many1Satisfy isDigit) |>> int

let varname = many1Satisfy2L isAsciiUpper (fun c -> isLetter c || isDigit c || c = '\'') "variable name"
let quotedVarname = between (pstring "\"") (strws "\"") varname

let expression, expressionref = createParserForwardedToRef<Expression, unit>()
let intersections, intersectionsref = createParserForwardedToRef<Expression, unit>()

(* Statement parser *)

let pntdef = 
    parsec { 
        let! name = varname .>> ws
        let! (x,y) = between (strws "(") (pstring ")") ((integer .>> strws "|") .>>. integer)
        return [ Assignment(name, NamePnt(name, CPoint(x,y))) ]
    } <?> "point definition"

type VarType = Plain | Quoted
let quotedOrUnquotedVarname = ((between (pstring "\"") (strws "\"") varname) |>> fun n -> (n,Quoted)) <|> (varname |>> fun n -> (n,Plain))

let assignment =
    parsec { 
        let! ((name, vartype), rhs) = attempt (quotedOrUnquotedVarname .>> wstrws "=") .>>. expression
        match vartype with
        | Plain -> return [ Assignment(name, rhs) ]
        | Quoted -> return [ Assignment(name, NamePnt(name, rhs)) ]
    } <?> "assignment"

let pick =
    parsec {
       let! vars = attempt (between (strws "{") (pstring "}") (sepBy1 quotedOrUnquotedVarname (wstrws ",")) .>> wstrws "<-")
       let! rhs = expression
       let allnames = vars |> List.map fst

       return Pick(allnames, rhs) :: [
           for (name,vartype) in vars do
               match vartype with
               | Quoted -> yield Assignment(name, NamePnt(name, Var(name)))
               | _         -> ()
       ]
    } <?> "pick statement"

let mereExpression = 
    parsec {
       let! expr = intersections
       return [ Assignment("ans", expr) ]
    } <?> "expression"   

let statement = (pick <|> assignment <|> (attempt pntdef) <|> mereExpression) <?> "statement"

let stmsep = 
  ws >>.
  (   (anyOf ",.;!") 
  <|> (commentLine >>% '.')
  <|> (newline >>% '.')
  <|> (eof >>% '.')
  ) <?> "statement separator"

let skipCommentLines = skipMany (attempt (ws >>. (commentLine <|> (newline >>% ()))))

let statementAndSeparator = attempt (ws >>. statement .>>. stmsep .>> skipCommentLines)

let statements = 
  parsec { 
    do! skipCommentLines
    let! statementsWithSeparators = many statementAndSeparator 
    return [
        for (stm, sep) in statementsWithSeparators do
          match sep with
          | '.' | ',' -> yield! stm // Improve `,`
          | ';' -> 
            yield Display(DisplayCommand.Push); yield Display(DisplayCommand.Off)
            yield! stm
            yield Display(DisplayCommand.Pop)
          | '!' -> 
            yield Display(DisplayCommand.Push); yield Display(DisplayCommand.Force)
            yield! stm
            yield Display(DisplayCommand.Pop)
          | c -> failwith (sprintf "Internal Parser error, unknown statement separator `%c`" c)
    ]
  }

(* Expression parser *)

let var = varname |>> Var

let proc = parsec {
    let! parameters  = (strws "proc" >>. between (strws "(") (pstring ")") (sepBy varname (wstrws ",")))
    let! body = statements
    do! skip (ws >>. pstring "end")
    return Proc(parameters, body)
}

let set = between (strws "{") (pstring "}") (sepBy intersections (strws ",")) |>> Set
let atom = var <|> between (strws "(") (pstring ")") expression <|> set

let ctor =
    parsec {
        let! exp1 = atom .>> ws
        return! choice [
            (anyOf "o°" .>> ws) >>. atom |>> (fun exp2 -> CCircle(exp1, exp2))
            attempt (strws "--") >>. atom |>> (fun exp2 -> CRay(exp1, exp2))
            attempt (strws "->") >>. atom |>> (fun exp2 -> CHalfRay(exp1, exp2))
            strws "-"  >>. atom |>> (fun exp2 -> CLine(exp1, exp2))
            between (strws "(") (pstring ")") (sepBy expression (wstrws ",")) |>> (fun args -> Funcall(exp1,args))
            preturn exp1
        ]
    }

let translateOperator exp (op, arg) = 
    match op with
    | 'n' -> Cap(exp,arg)
    | 'u' -> Cup(exp,arg)
    | '\\' -> Minus(exp,arg)
    | c  -> failwith (sprintf "Internal parser error, unknown operator symbol `%c`" c)
                     
intersectionsref := parsec {
    let! exp1 = ctor
    let! exprs = many (attempt (ws >>. anyOf "nu\\" .>> ws) .>>. ctor)
    return List.fold translateOperator exp1 exprs
} 

expressionref := proc <|> intersections