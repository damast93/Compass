Option Strict On

Imports System.Math

Public Class Circle
    Inherits Geometry

    Private ReadOnly m_Center As Point
    Private ReadOnly m_Radius As Double

    Public Sub New(center As Point, r As Double)
        m_Center = center : m_Radius = r
    End Sub

    Public Sub New(a As Point, b As Point)
        m_Center = a
        m_Radius = a.DistanceTo(b)
    End Sub

    Public ReadOnly Property Center As Point
        Get
            Return m_Center
        End Get
    End Property

    Public ReadOnly Property Radius As Double
        Get
            Return m_Radius
        End Get
    End Property


    Public Overloads Overrides Function Intersect(other As Geometry) As Geometry
        If TypeOf other Is Circle Then
            Return CircleCircle(Me, DirectCast(other, Circle))
        ElseIf TypeOf other Is Point Then
            Return CirclePoint(Me, DirectCast(other, Point))
        ElseIf TypeOf other Is Line Then
            Return DirectCast(other, Line).Intersect(Me)
        ElseIf TypeOf other Is PointSet Then
            Return DirectCast(other, PointSet).Intersect(Me)
        Else
            Throw New NotImplementedException()
        End If
    End Function

    Public Overrides Function Pick(n As Integer) As IList(Of Point)
        ' TODO: Should be random
        Dim results As New List(Of Point)
        For i = 0 To n - 1
            Dim phi = 2.0 * PI * i / CDbl(n)
            Dim px = m_Center.x + m_Radius * Cos(phi)
            Dim py = m_Center.y + m_Radius * Sin(phi)
            results.Add(New Point(px, py))
        Next
        Return results
    End Function

    Public Overrides Sub Accept(visitor As ValueVisitor)
        visitor.Visit(Me)
    End Sub
End Class
