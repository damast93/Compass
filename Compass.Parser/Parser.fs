module Parser.Parser

open FParsec
open Parser.ParserImpl

let Parse(source) : seq<Parser.Statement> = 
    match run (ws >>. statements .>> eof) source with
    | Success(res, _, _) -> res |> List.toSeq 
    | Failure(msg, _, _) -> failwith msg

let display (input : string) =
    match run intersections input with
        | Success(term, u, pos)  -> 
            printfn "%A\n> %A" pos term 
        | Failure(msg, _, _) -> printfn "Failure %A" msg

let rec repl() = 
    printf "> "
    let input = System.Console.ReadLine().Trim() 
    if input <> "" && input <> ":q" then
        display input
        repl()
    else
        ()

repl()
