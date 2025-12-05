Option Strict On

Imports Compass.Core.Display
Imports Compass.Core.Geometry
Imports Compass.Core.Maths
Imports WPFLine = System.Windows.Shapes.Line

Namespace Display

    Public Class CanvasDisplay
        Inherits DisplayEngine

        Private m_Canvas As Canvas

        Private Const PointThickness As Double = 4

        Public Sub New(canvas As Canvas)
            m_Canvas = canvas
        End Sub

        Public Overrides Sub Render(circle As Circle)
            Dim ellipse = New Ellipse()
            Dim r = circle.Radius
            With ellipse
                .Width = 2 * r
                .Height = 2 * r
                .Stroke = Brushes.Black
            End With
            m_Canvas.Children.Add(ellipse)
            Canvas.SetLeft(ellipse, circle.Center.x - circle.Radius)
            Canvas.SetTop(ellipse, circle.Center.y - circle.Radius)

        End Sub

        Public Overrides Sub Render(point As Point)
            Dim pnt = New Ellipse()
            With pnt
                .Width = 2 * PointThickness
                .Height = 2 * PointThickness
                .Fill = Brushes.Navy
            End With
            m_Canvas.Children.Add(pnt)
            Canvas.SetLeft(pnt, point.x - PointThickness)
            Canvas.SetTop(pnt, point.y - PointThickness)
        End Sub

        Public Overrides Sub Render(point As NamedPoint)
            Render(CType(point, Point))
            Dim lbl = New Label()
            With lbl
                .Content = point.Name
                .Foreground = Brushes.Black
            End With
            m_Canvas.Children.Add(lbl)
            Canvas.SetLeft(lbl, point.x)
            Canvas.SetTop(lbl, point.y)
        End Sub

        Public Overrides Sub Render(ray As Line)
            Dim line As New WPFLine

            Dim updater = New RayUpdater() With {.Line = ray, .P1Fixed = False, .GUILine = line, .display = Me}
            line.Stroke = Brushes.Gray

            m_Canvas.Children.Add(line)
            updater.Update()
            AddHandler m_Canvas.SizeChanged, AddressOf updater.Canvas_SizeChanged
        End Sub

        Private Structure RayUpdater
            Public Line As Line
            Public GUILine As WPFLine
            Public display As CanvasDisplay
            Public P1Fixed As Boolean

            Private ReadOnly Property canvas As Canvas
                Get
                    Return display.m_Canvas
                End Get
            End Property

            Public Sub Canvas_SizeChanged(sender As Object, e As SizeChangedEventArgs)
                Update()
            End Sub

            Public Sub Update()
                Dim pts As New List(Of Point)
                For Each seg In CanvasBounds
                    Dim ints = seg.Intersect(Line).Pick(1)
                    For Each p In ints
                        If pts.All(Function(x) p.DistanceTo(x) > MathGlobals.EPS) Then
                            pts.Add(p)
                        End If
                    Next
                Next

                If P1Fixed Then
                    If pts.Count = 1 Then
                        GUILine.X2 = pts(0).x
                        GUILine.Y2 = pts(0).y
                    End If
                Else
                    If pts.Count = 2 Then
                        GUILine.X1 = pts(0).x
                        GUILine.Y1 = pts(0).y
                        GUILine.X2 = pts(1).x
                        GUILine.Y2 = pts(1).y
                    End If
                End If
            End Sub

            Public ReadOnly Property CanvasBounds As IEnumerable(Of Segment)
                Get
                    Dim NW = New Point(0, 0)
                    Dim NE = New Point(canvas.ActualWidth, 0)
                    Dim SW = New Point(0, canvas.ActualHeight)
                    Dim SE = New Point(canvas.ActualWidth, canvas.ActualHeight)

                    Dim bounds As New List(Of Segment) From {
                            New Segment(NW, NE),
                            New Segment(NE, SE),
                            New Segment(SE, SW),
                            New Segment(SW, NW)
                            }

                    Return bounds
                End Get
            End Property
        End Structure

        Public Overrides Sub Render(r As Segment)
            Dim line As New WPFLine
            line.X1 = (r.A.x)
            line.Y1 = (r.A.y)
            line.X2 = (r.B.x)
            line.Y2 = (r.B.y)
            line.Stroke = Brushes.Red

            m_Canvas.Children.Add(line)
        End Sub

        Public Overrides Sub Render(ray As Ray)
            Dim line As New WPFLine

            Dim updater = New RayUpdater() With {.Line = ray, .P1Fixed = True, .GUILine = line, .display = Me}
            line.Stroke = Brushes.Gray

            m_Canvas.Children.Add(line)
            line.X1 = ray.A.x
            line.Y1 = ray.A.y
            updater.Update()
            AddHandler m_Canvas.SizeChanged, AddressOf updater.Canvas_SizeChanged
        End Sub
    End Class
End Namespace