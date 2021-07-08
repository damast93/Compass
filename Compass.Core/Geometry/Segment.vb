Imports Compass.Core.Maths

Namespace Geometry
    Public Class Segment
        Inherits Line

        Private ReadOnly m_Ray As Line

        Public Sub New(start As Point, dest As Point)
            MyBase.New(start, dest)
        End Sub

        Public Overrides Sub Accept(visitor As ValueVisitor)
            visitor.Visit(Me)
        End Sub

        Protected Overrides Function ValidatePoint(pnt As Point) As Boolean
            Return PointOnSegment(pnt, Me)
        End Function
    End Class
End Namespace