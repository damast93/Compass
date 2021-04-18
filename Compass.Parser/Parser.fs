module Parser.Parser

open FParsec
open Parser.ParserImpl

let Parse(source) : seq<Parser.Statement> = 
    match run (ws >>. full statements) source with
    | Success(res, _, _) -> res |> List.toSeq 
    | Failure(msg, _, _) -> failwith msg

let Repl() = repl()