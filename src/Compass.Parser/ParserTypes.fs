namespace Compass.Parser

open FParsec
open System.Collections.Generic

type Expression =
    | Sub     of string list * (Statement list)
    | Var     of string
    | CPoint  of float * float
    | CLine   of Expression * Expression
    | CRay    of Expression * Expression
    | CCircle of Expression * Expression
    | CHalfRay of Expression * Expression
    | Cap     of Expression * Expression
    | Cup     of Expression * Expression
    | Minus   of Expression * Expression
    | Funcall of Expression * Expression list
    | Set     of Expression list
    | NamePnt of string * Expression
        with
            member this.Accept(visitor : ExpressionVisitor<'T>) : 'T = 
                match this with
                | Sub(a,b)     -> visitor.Sub(a,b)
                | Var(n)       -> visitor.Var(n)
                | CLine(a,b)   -> visitor.Line(a,b)
                | CRay(a,b)    -> visitor.Ray(a,b)
                | CCircle(a,b) -> visitor.Circle(a,b)
                | Cap(a,b)     -> visitor.Intersect(a,b)
                | Funcall(f,x) -> visitor.Funcall(f,x)
                | NamePnt(s,x) -> visitor.NamePoint(s,x)
                | CPoint(x,y)  -> visitor.Point(x,y)
                | Set(s)       -> visitor.Set(s)
                | Cup(a,b)     -> visitor.Union(a,b)
                | Minus(a,b)   -> visitor.SetMinus(a,b)
                | CHalfRay(a,b) -> visitor.HalfRay(a,b)

and Statement = 
    | Assignment of string * Expression * int option
    | Display    of Expression * int option
    | Pick       of string list * Expression * int option
        with
            member this.Accept(visitor : StatementVisitor<'T>) : 'T =
                let op = function | Some(x) -> x | None -> -1
                match this with
                | Assignment(a,b, o) -> visitor.Assign(a,b, op o)
                | Pick(a,b, o)       -> visitor.Pick(a,b, op o)
                | Display(a, o)      -> visitor.Display(a, op o)

and ExpressionVisitor<'T> = 
    abstract Sub       : seq<string> * seq<Statement> -> 'T
    abstract Var       : string -> 'T
    abstract Line      : Expression * Expression -> 'T
    abstract Ray       : Expression * Expression -> 'T
    abstract Circle    : Expression * Expression -> 'T
    abstract Intersect : Expression * Expression -> 'T
    abstract Funcall   : Expression * seq<Expression> -> 'T
    abstract NamePoint : string * Expression -> 'T
    abstract Point     : float * float -> 'T
    abstract Set       : seq<Expression> -> 'T
    abstract Union     : Expression * Expression -> 'T
    abstract SetMinus  : Expression * Expression -> 'T
    abstract HalfRay   : Expression * Expression -> 'T

and StatementVisitor<'T> = 
    abstract Assign  : string * Expression * int -> 'T
    abstract Pick    : seq<string> * Expression * int -> 'T
    abstract Display : Expression * int -> 'T 

type SyntaxError(code : int, msg : string) =
    inherit System.Exception(msg)
    member e.Code = code
    new (msg : string) = SyntaxError(0, msg)