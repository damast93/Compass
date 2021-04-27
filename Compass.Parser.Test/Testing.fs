module Testing

open NUnit.Framework
open FsUnit

open FParsec
open Parser
open Parser.ParserImpl

let trim parser = ws >>. parser
let exactly parser = ws >>. parser >>. ws .>> eof

let accepts input parser = 
   match run parser input with
   | Success(_) -> ()
   | Failure(msg, _, _) -> Assert.Fail(msg)

let runOn input parser = 
   match run parser input with
   | Success(res, _, _) -> res
   | Failure(msg, _, _) -> failwith msg