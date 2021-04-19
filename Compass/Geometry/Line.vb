Imports Compass.Maths

Namespace Geometry
    Public Class Line
        Inherits Geometry

        Private x0, y0 As Double
        Private x1, y1 As Double

        Public ReadOnly Property A As Point
            Get
                Return New Point(x0, y0)
            End Get
        End Property

        Public ReadOnly Property B As Point
            Get
                Return New Point(x1, y1)
            End Get
        End Property

        Public ReadOnly Property dx As Double
            Get
                Return x1 - x0
            End Get
        End Property

        Public ReadOnly Property dy As Double
            Get
                Return y1 - y0
            End Get
        End Property

        Public Sub New(x0 As Double, y0 As Double, x1 As Double, y1 As Double)
            Me.x0 = x0 : Me.x1 = x1 : Me.y0 = y0 : Me.y1 = y1
        End Sub

        Public Sub New(a As Point, b As Point)
            Me.New(a.x, a.y, b.x, b.y)
        End Sub

        Public Overrides Sub Accept(visitor As ValueVisitor)
            visitor.Visit(Me)
        End Sub

        Public Overrides Function Intersect(other As Geometry) As Geometry
            If TypeOf other Is Circle Then
                Return Filter(CircleLine(DirectCast(other, Circle), Me))
            ElseIf TypeOf other Is Point Then
                Return Filter(PointLine(DirectCast(other, Point), Me))
            ElseIf TypeOf other Is Line Then
                Dim otherRay = DirectCast(other, Line)
                Return otherRay.Filter(Filter(LineLine(DirectCast(other, Line), Me)))
            ElseIf TypeOf other Is PointSet Then
                Return Filter(DirectCast(other, PointSet).Intersect(Me))
            Else
                Throw New NotImplementedException
            End If
        End Function

        Public Overrides Function Pick(n As Integer) As IList(Of Point)
            Dim results As New List(Of Point)
            For k = 1 To n
                Dim t = CDbl(k) / CDbl(n + 1)
                results.Add(New Point(x0 + t * dx, y0 + t * dy))
            Next
            Return results
        End Function

        Protected Overridable Function ValidatePoint(pnt As Point) As Boolean
            Return True
        End Function

        Private Function Filter(p As Geometry) As Geometry
            If TypeOf p Is Point Then
                Dim pnt = DirectCast(p, Point)
                If ValidatePoint(pnt) Then
                    Return pnt.AsSet
                Else
                    Return New PointSet({})
                End If
            ElseIf TypeOf p Is PointSet Then
                Dim ps = DirectCast(p, PointSet)
                Return Helper.PointSet((From pnt In ps.Points Where ValidatePoint(pnt) Select pnt).ToList)
            Else
                Return p
            End If
        End Function
    End Class
End Namespace