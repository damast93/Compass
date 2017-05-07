module Compass.Parser.Parser

open FParsec
open Compass.Parser.ParserImpl

let Parse(source) : seq<Compass.Parser.Statement> = 
    match run (ws >>. full statements) source with
    | Success(res, _, _) -> res |> List.toSeq 
    | Failure(msg, _, _) -> raise <| new SyntaxError(msg)

let Repl() = repl()