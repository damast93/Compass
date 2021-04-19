Imports Compass.Maths

Namespace Geometry
    Public Class Ray
        Inherits Line

        Public Sub New(a As Point, b As Point)
            MyBase.New(a, b)
        End Sub

        Public Overrides Sub Accept(visitor As ValueVisitor)
            visitor.Visit(Me)
        End Sub

        Protected Overrides Function ValidatePoint(pnt As Point) As Boolean
            Return PointOnRay(pnt, Me)
        End Function

    End Class
End Namespace