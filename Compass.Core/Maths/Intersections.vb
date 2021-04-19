Option Strict On

Imports System.Math
Imports Compass.Core.Geometry

Namespace Maths

    Module Intersections

        Public Function CirclePoint(a As Circle, p As Point) As Geometry.Geometry
            If Abs(a.Center.DistanceTo(p) - a.Radius) < EPS Then
                Return p.AsSet
            Else
                Return New PointSet({})
            End If
        End Function

        Public Function PointPoint(a As Point, b As Point) As Geometry.Geometry
            If Abs(a.DistanceTo(b)) < EPS Then
                Return a.AsSet
            Else
                Return New PointSet({})
            End If
        End Function

        Public Function CircleCircle(a As Circle, b As Circle) As Geometry.Geometry
            Dim r = a.Radius, s = b.Radius
            Dim d = a.Center.DistanceTo(b.Center)

            ' Are the circles identical?
            If (d < EPS) Then
                Return a
            End If

            If (d > r + s) OrElse (d < Abs(r - s)) Then
                Return New PointSet({})
            Else
                ' Solve circle equations for circles around (0,0) and (0,d)
                Dim x = (r ^ 2 + d ^ 2 - s ^ 2) / (2 * d)
                Dim y = Sqrt(r ^ 2 - x ^ 2)

                Dim tx = x / d
                Dim ty = y / d

                ' Compute midpoint
                Dim bx = a.Center.x + tx * (b.Center.x - a.Center.x)
                Dim by = a.Center.y + tx * (b.Center.y - a.Center.y)

                ' Compute orthogonal displacement from midpoint
                Dim dx = ty * (b.Center.y - a.Center.y)
                Dim dy = -ty * (b.Center.x - a.Center.x)

                If Abs(y) < EPS Then
                    Return New Point(bx, by).AsSet
                Else
                    Return New PointSet({New Point(bx + dx, by + dy), New Point(bx - dx, by - dy)})
                End If
            End If
        End Function

        Public Function CircleLine(c As Circle, line As Line) As Geometry.Geometry
            ' Shift to origin
            Dim x0 = line.A.x - c.Center.x, y0 = line.A.y - c.Center.y
            Dim x1 = line.B.x - c.Center.x, y1 = line.B.y - c.Center.y

            Dim r = c.Radius

            Dim dx = x1 - x0, dy = y1 - y0
            Dim l = dx ^ 2 + dy ^ 2
            Dim p = 2 * (x0 * dx + y0 * dy) / l
            Dim q = (x0 ^ 2 + y0 ^ 2 - r ^ 2) / l

            Dim disc = p ^ 2 - 4 * q
            If disc < 0 Then
                Return New PointSet({})
            ElseIf disc < EPS Then
                Dim sq = Sqrt(disc / 4)
                Dim t = -p / 2 + sq

                Return New Point(c.Center.x + x0 + t * dx, c.Center.y + y0 + dy * t).AsSet
            Else
                Dim sq = Sqrt(disc / 4)
                Dim tp = -p / 2 + sq
                Dim tm = -p / 2 - sq

                Dim pplus = New Point(c.Center.x + x0 + tp * dx, c.Center.y + y0 + tp * dy)
                Dim pminus = New Point(c.Center.x + x0 + tm * dx, c.Center.y + y0 + tm * dy)

                Return New PointSet({pplus, pminus})
            End If
        End Function

        Public Function PointLine(p As Point, line As Line) As Geometry.Geometry
            If PointOnLine(p, line) Then
                Return p.AsSet
            Else
                Return New PointSet({})
            End If
        End Function

        Public Function LineLine(a As Line, b As Line) As Geometry.Geometry
            Dim nx = -a.dy, ny = a.dx
            Dim d = nx * a.A.x + ny * a.A.y

            Dim x0 = b.A.x, y0 = b.A.y
            Dim dx = b.dx, dy = b.dy
            Dim o = dx * nx + dy * ny

            ' Rays are (almost) parallel
            If Abs(o) < EPS Then
                If Abs((nx * x0 + ny * y0 - d) / Sqrt(nx ^ 2 + ny ^ 2)) < EPS Then
                    Return a
                Else
                    Return New PointSet({})
                End If
            End If

            Dim t = (d - (nx * b.A.x + ny * b.A.y)) / o
            Dim p = New Point(b.A.x + t * dx, b.A.y + t * dy)
            Return p.AsSet
        End Function

        Friend Function PointOnLine(p As Point, ray As Line) As Boolean
            Dim nx = -ray.dy, ny = ray.dx
            Dim d = nx * ray.A.x + ny * ray.A.y

            Dim dst = Abs(nx * p.x + ny * p.y - d) / Sqrt(nx ^ 2 + ny ^ 2)
            Return (dst < EPS)
        End Function

        Friend Function PointOnSegment(a As Point, l As Segment) As Boolean
            If Not PointOnLine(a, l) Then Return False

            Dim x = a.x, y = a.y
            Dim nx = l.dx, ny = l.dy

            Dim s1 = nx * (x - l.A.x) + ny * (y - l.A.y)
            Dim s2 = -nx * (x - l.B.x) - ny * (y - l.B.y)

            Return (s1 > -EPS) AndAlso (s2 > -EPS)
        End Function

        Friend Function PointOnRay(a As Point, l As Ray) As Boolean
            If Not PointOnLine(a, l) Then Return False

            Dim x = a.x, y = a.y
            Dim nx = l.dx, ny = l.dy

            Dim s1 = nx * (x - l.A.x) + ny * (y - l.A.y)
            Return (s1 > -EPS)
        End Function

    End Module
End Namespace