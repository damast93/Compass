module Main

open FParsec
open Parser.ParserImpl

let display (input : string) =
    match run statements input with
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

[<EntryPoint>]
let main arguments =
    repl()
    0