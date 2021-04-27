module ExpressionTests

open NUnit.Framework
open FsUnit

open FParsec

open Parser
open Parser.ParserImpl

open Testing

[<Test>]
let ``Valid Var`` () =
    expression |> runOn "Xy42'" |> should equal (Var "Xy42'")
    
[<Test>]
let ``Invalid Var`` () =
    shouldFail (fun () -> expression |> accepts "yX")

[<Test>]
let ``Proc expression`` () =
    expression
    |> exactly
    |> accepts "  (  proc ( X,  Y ) X; Y. end ) ( B)  "

[<Test>]
let ``Valid Parens`` () =
    expression |> exactly |> accepts "(  ((A) ))"

[<Test>]
let ``Invalid Parens`` () =
     shouldFail (fun() -> expression |> exactly |> accepts "(  (A) ))")

[<Test>]
let ``Funcall Parens`` () =
     expression |> exactly |> accepts "  F( A, B  , C,D, (E)  , (proc (F) G. end), K( I,  J) ) "

[<Test>]
let ``Funcall`` () =
     expression |> exactly |> accepts "F(A,B,C,D,(E),(proc (F) G. end),K(I,J))"
         