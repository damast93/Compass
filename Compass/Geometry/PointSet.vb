Imports System.Runtime.CompilerServices

Public Class PointSet
    Inherits Geometry

    Private ReadOnly m_Points As IList(Of Point)

    Public Sub New(points As IEnumerable(Of Point))
        Dim results As New List(Of Point)
        For Each pnt In points
            Dim p = pnt
            If results.TrueForAll(Function(q) q.DistanceTo(p) >= EPS) Then results.Add(pnt)
        Next
        m_Points = results
    End Sub

    Public ReadOnly Property Points As IList(Of Point)
        Get
            Return m_Points
        End Get
    End Property

    Public Overrides Sub Accept(visitor As ValueVisitor)
        For Each point In m_Points
            point.Accept(visitor)
        Next
    End Sub

    Public Overrides Function Intersect(other As Geometry) As Geometry
        Dim resultingPoints As New List(Of Point)

        For Each point In m_Points
            Dim intersection = point.Intersect(other)
            If TypeOf intersection Is Point Then
                resultingPoints.Add(DirectCast(intersection, Point))
            ElseIf TypeOf intersection Is PointSet Then
                resultingPoints.AddRange(DirectCast(intersection, PointSet).m_Points)
            End If
        Next

        Return Helper.PointSet(resultingPoints)
    End Function

    Public Function SetMinus(other As PointSet) As PointSet
        Dim results = From p In m_Points Where other.m_Points.All(Function(q) q.DistanceTo(p) >= EPS) Select p

        Return New PointSet(results)
    End Function

    Public Function SetUnion(other As PointSet) As PointSet
        Dim results = m_Points.Concat(other.m_Points)

        Return New PointSet(results)
    End Function

    Public Overrides Function Pick(n As Integer) As IList(Of Point)
        If m_Points.Count > 0 AndAlso n <= m_Points.Count Then
            Return m_Points.Take(n).ToList
        Else
            Return New Point() {}
        End If
    End Function
End Class

Public Module Helper
    Public Function PointSet(points As IEnumerable(Of Point)) As Geometry
        'Dim ptList = points.ToList()
        'If ptList.Count = 1 Then
        '    Return ptList(0)
        'Else
        '    Return New PointSet(ptList)
        'End If
        Return New PointSet(points)
    End Function

    <Extension()>
    Public Function AsSet(pnt As Point) As PointSet
        Return New PointSet({pnt})
    End Function

End Module
