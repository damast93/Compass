Public Class Point
    Inherits Geometry

    Private ReadOnly m_x, m_y As Double

    Public Sub New(x As Double, y As Double)
        m_x = x : m_y = y
    End Sub

    Public ReadOnly Property x As Double
        Get
            Return m_x
        End Get
    End Property

    Public ReadOnly Property y As Double
        Get
            Return m_y
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return String.Format("({0},{1})", x, y)
    End Function

    Public Function DistanceTo(other As Point) As Double
        Return Math.Sqrt((x - other.x) ^ 2 + (y - other.y) ^ 2)
    End Function

    Public Overrides Sub Accept(visitor As ValueVisitor)
        visitor.Visit(Me)
    End Sub

    Public Overrides Function Intersect(other As Geometry) As Geometry
        If TypeOf other Is Point Then
            Return Intersections.PointPoint(Me, DirectCast(other, Point))
        ElseIf TypeOf other Is PointSet Then
            Return DirectCast(other, PointSet).Intersect(Me)
        ElseIf TypeOf other Is Circle Then
            Return Intersections.CirclePoint(DirectCast(other, Circle), Me)
        ElseIf TypeOf other Is Line Then
            Return DirectCast(other, Line).Intersect(Me)
        Else
            Throw New NotImplementedException()
        End If
    End Function

    Public Overrides Function Pick(n As Integer) As System.Collections.Generic.IList(Of Point)
        If n = 1 Then
            Return New Point() {Me}
        Else
            Return New Point() {}
        End If
    End Function
End Class

Public Class NamedPoint
    Inherits Point

    Private ReadOnly m_Name As String

    Public Sub New(name As String, x As Double, y As Double)
        MyBase.new(x, y)
        m_Name = name
    End Sub

    Public ReadOnly Property Name As String
        Get
            Return m_Name
        End Get
    End Property

    Public Overrides Sub Accept(visitor As ValueVisitor)
        visitor.Visit(Me)
    End Sub
End Class