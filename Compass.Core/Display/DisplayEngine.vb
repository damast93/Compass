Imports Compass.Core.Geometry

Namespace Display
    Public MustInherit Class DisplayEngine
        Implements ValueVisitor

        Public MustOverride Sub Render(point As Point)
        Public MustOverride Sub Render(point As NamedPoint)
        Public MustOverride Sub Render(circle As Circle)
        Public MustOverride Sub Render(ray As Line)
        Public MustOverride Sub Render(segment As Segment)
        Public MustOverride Sub Render(ray As Ray)

        Public Sub Visit(circle As Circle) Implements ValueVisitor.Visit
            Render(circle)
        End Sub

        Public Sub Visit(namedPoint As NamedPoint) Implements ValueVisitor.Visit
            Render(namedPoint)
        End Sub

        Public Sub Visit(point As Point) Implements ValueVisitor.Visit
            Render(point)
        End Sub

        Public Sub Visit(ray As Line) Implements ValueVisitor.Visit
            Render(ray)
        End Sub

        Public Sub Visit(segment As Segment) Implements ValueVisitor.Visit
            Render(segment)
        End Sub

        Public Sub Visit(lambda As Lambda) Implements ValueVisitor.Visit
            ' Do Nothing
        End Sub

        Public Sub Visit(halfray As Ray) Implements ValueVisitor.Visit
            Render(halfray)
        End Sub
    End Class
End Namespace