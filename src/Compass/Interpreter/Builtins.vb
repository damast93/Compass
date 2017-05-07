Imports System.Reflection

Public Module Builtins
    Private Class BuiltinsImpl
        Public Shared Function Move(_a As Value, _b As Value, _c As Value) As Value
            Dim a = _a.AsOf(Of Point)(), b = _b.AsOf(Of Point)(), c = _c.AsOf(Of Point)()

            Return New Circle(a, b.DistanceTo(c))
        End Function

        Public Shared Function Mid(_a As Value, _b As Value) As Value
            Dim a = _a.AsOf(Of Point)(), b = _b.AsOf(Of Point)()

            Return New Point((a.x + b.x) / 2, (a.y + b.y) / 2)
        End Function

        Public Shared Function Center(_c As Value) As Value
            Return _c.AsOf(Of Circle).Center
        End Function

        Public Shared Function Proj(_a As Value, _b As Value, _c As Value) As Value
            Dim a = _a.AsOf(Of Point)(), b = _b.AsOf(Of Point)(), c = _c.AsOf(Of Point)()

            Dim dx = c.x - b.x, dy = c.y - b.y
            Dim vx = a.x - b.x, vy = a.y - b.y

            Dim dsq = dx ^ 2 + dy ^ 2
            Dim l = Math.Sqrt(vx ^ 2 + vy ^ 2)
            Dim dot = dx * vx + dy * vy
            Dim px = (dx / dsq) * dot, py = (dy / dsq) * dot

            Return New Point(b.x + px, b.y + py)
        End Function

        Public Shared Function Rect(_a As Value, _b As Value) As Value
            Dim a = _a.AsOf(Of Point)(), b = _b.AsOf(Of Point)()
            Return New Point(a.x - (b.y - a.y), a.y + (b.x - a.x))
        End Function

        Public Shared Function U(_a As Value) As Value
            Dim a = _a.AsOf(Of Point)()
            Return New Circle(a, Unit)
        End Function

        Public Shared Function D(_a As Value, _b As Value) As Value
            Dim a = _a.AsOf(Of Point)(), b = _b.AsOf(Of Point)()
            Dim dst = a.DistanceTo(b) / Unit

            MessageBox.Show(dst.ToString(), "Measurement", MessageBoxButton.OK, MessageBoxImage.Information)

            Return New PointSet(New Point() {})
        End Function
    End Class

    '' ............................................
    Public Function GetBuiltins() As Dictionary(Of String, Value)
        Dim ty = GetType(BuiltinsImpl)
        Dim methods = ty.GetMethods(BindingFlags.Static Or BindingFlags.DeclaredOnly Or BindingFlags.Public)

        Dim dict = New Dictionary(Of String, Value)
        For Each _method In methods
            Dim method = _method
            Dim f = Function(args As IEnumerable(Of Value)) As Value
                        Dim parcount = method.GetParameters().Count()
                        Dim arglist = args.Cast(Of Object).ToArray

                        If arglist.Count <> parcount Then Throw New ArgumentException(String.Format("Insufficient arguments for function call: {0} expected, {1} given", parcount, arglist.Count))

                        Return DirectCast(method.Invoke(Nothing, arglist), Value)
                    End Function
            dict.Add(method.Name, New Lambda(f))
        Next

        Return dict
    End Function
End Module

