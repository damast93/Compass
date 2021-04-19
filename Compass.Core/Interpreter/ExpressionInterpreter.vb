Imports Compass.Core.Geometry
Imports Parser

Namespace Interpreter

    Public Class ExpressionInterpreter : Implements ExpressionVisitor(Of Value)
        Private m_Scope As Scope
        Private m_Context As Context

        Public Sub New(scope As Scope, context As Context)
            m_Scope = scope
            m_Context = context
        End Sub

        Public Function Circle(A As Expression, B As Expression) As Value Implements ExpressionVisitor(Of Value).Circle
            Dim _a = A.Accept(Me).AsOf(Of Point)()
            Dim _b = B.Accept(Me).AsOf(Of Point)()

            Return New Circle(_a, _b)
        End Function

        Public Function Funcall(func As Expression, args As IEnumerable(Of Expression)) As Value Implements ExpressionVisitor(Of Value).Funcall
            Dim f = func.Accept(Me).AsOf(Of Lambda)()
            Dim _args = args.Select(Function(arg) arg.Accept(Me)).ToList()
            Return f.Eval(_args)
        End Function

        Public Function Intersect(A As Expression, B As Expression) As Value Implements ExpressionVisitor(Of Value).Intersect
            Dim _a = A.Accept(Me).AsOf(Of Geometry.Geometry)()
            Dim _b = B.Accept(Me).AsOf(Of Geometry.Geometry)()

            Return _a.Intersect(_b)
        End Function

        Public Function Line(A As Expression, B As Expression) As Value Implements ExpressionVisitor(Of Value).Line
            Dim _a = A.Accept(Me).AsOf(Of Point)()
            Dim _b = B.Accept(Me).AsOf(Of Point)()

            Return New Segment(_a, _b)
        End Function

        Public Function Ray(A As Expression, B As Expression) As Value Implements ExpressionVisitor(Of Value).Ray
            Dim _a = A.Accept(Me).AsOf(Of Point)()
            Dim _b = B.Accept(Me).AsOf(Of Point)()

            Return New Line(_a, _b)
        End Function

        Public Function Var(varname As String) As Value Implements ExpressionVisitor(Of Value).Var
            Return m_Scope(varname)
        End Function

        Public Function Point(x As Integer, y As Integer) As Value Implements ExpressionVisitor(Of Value).Point
            Return New Point(x, y)
        End Function

        Public Function NamePoint(name As String, pnt As Expression) As Value Implements ExpressionVisitor(Of Value).NamePoint
            Dim point = pnt.Accept(Me).AsOf(Of Point)()
            Return New NamedPoint(name, point.x, point.y)
        End Function

        Public Function [Sub](pars As IEnumerable(Of String), body As IEnumerable(Of Statement)) As Value Implements ExpressionVisitor(Of Value).Sub
            Dim parlist = pars.ToList()

            Dim scope = m_Scope
            Dim context = m_Context

            Dim f = Function(args As IEnumerable(Of Value)) As Value
                        Dim arglist = args.ToList()

                        If arglist.Count <> parlist.Count Then Throw New ArgumentException(String.Format("Insufficient arguments for function call: {0} expected, {1} given", parlist.Count, arglist.Count))

                        Dim newScope = scope
                        For i = 0 To arglist.Count - 1
                            newScope = newScope.Add(parlist(i), arglist(i))
                        Next

                        Dim int = New StatementInterpreter(newScope, context)

                        For Each stm In body
                            stm.Accept(int)
                        Next

                        Return int.LastOperation
                    End Function

            Return New Lambda(f)
        End Function

        Public Function [Set](args As IEnumerable(Of Expression)) As Value Implements ExpressionVisitor(Of Value).Set
            Dim exprs = args.Select(Function(arg) arg.Accept(Me).AsOf(Of Point)()).ToList
            Return New PointSet(exprs)
        End Function

        Public Function SetMinus(a As Expression, b As Expression) As Value Implements ExpressionVisitor(Of Value).SetMinus
            Dim _a = a.Accept(Me).AsOf(Of PointSet)()
            Dim _b = b.Accept(Me).AsOf(Of PointSet)()

            Return _a.SetMinus(_b)
        End Function

        Public Function Union(a As Expression, b As Expression) As Value Implements ExpressionVisitor(Of Value).Union
            Dim _a = a.Accept(Me).AsOf(Of PointSet)()
            Dim _b = b.Accept(Me).AsOf(Of PointSet)()

            Return _a.SetUnion(_b)
        End Function

        Public Function HalfRay(a As Expression, b As Expression) As Value Implements ExpressionVisitor(Of Value).HalfRay
            Dim _a = a.Accept(Me).AsOf(Of Point)()
            Dim _b = b.Accept(Me).AsOf(Of Point)()

            Return New Ray(_a, _b)
        End Function
    End Class
End Namespace