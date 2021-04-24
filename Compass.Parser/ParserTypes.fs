namespace Parser

open FParsec
open System.Collections.Generic

type DisplayCommand =
  | Off = 0
  | On = 1
  | Force = 2
  | Push = 3
  | Pop = 4

type Expression =
    | Proc    of string list * (Statement list) 
    | Var     of string
    | CPoint  of int * int
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
                | Proc(a,b)    -> visitor.Proc(a,b)
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
    | Assignment of string * Expression
    | Pick       of string list * Expression
    | Display    of DisplayCommand
        with
            member this.Accept(visitor : StatementVisitor<'T>) : 'T =
                match this with
                | Assignment(a,b) -> visitor.Assign(a,b)
                | Pick(a,b)       -> visitor.Pick(a,b)
                | Display(a)      -> visitor.Display(a)

and ExpressionVisitor<'T> = 
    abstract Proc      : seq<string> * seq<Statement> -> 'T
    abstract Var       : string -> 'T
    abstract Line      : Expression * Expression -> 'T
    abstract Ray       : Expression * Expression -> 'T
    abstract Circle    : Expression * Expression -> 'T
    abstract Intersect : Expression * Expression -> 'T
    abstract Funcall   : Expression * seq<Expression> -> 'T
    abstract NamePoint : string * Expression -> 'T
    abstract Point     : int * int -> 'T
    abstract Set       : seq<Expression> -> 'T
    abstract Union     : Expression * Expression -> 'T
    abstract SetMinus  : Expression * Expression -> 'T
    abstract HalfRay   : Expression * Expression -> 'T

and StatementVisitor<'T> = 
    abstract Assign  : string * Expression -> 'T
    abstract Pick    : seq<string> * Expression -> 'T
    abstract Display : DisplayCommand -> 'T 