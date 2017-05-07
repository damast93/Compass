Public MustInherit Class DisplayEngine
    Implements ValueVisitor

    Public MustOverride Sub RenderPoint(point As Point)
    Public MustOverride Sub RenderNamedPoint(point As NamedPoint)
    Public MustOverride Sub RenderCircle(circle As Circle)
    Public MustOverride Sub RenderRay(ray As Line)
    Public MustOverride Sub RenderSegment(segment As Segment)
    Public MustOverride Sub RenderHalfRay(ray As Ray)

    Public Sub Visit(circle As Circle) Implements ValueVisitor.Visit
        RenderCircle(circle)
    End Sub

    Public Sub Visit(namedPoint As NamedPoint) Implements ValueVisitor.Visit
        RenderNamedPoint(namedPoint)
    End Sub

    Public Sub Visit(point As Point) Implements ValueVisitor.Visit
        RenderPoint(point)
    End Sub

    Public Sub Visit(ray As Line) Implements ValueVisitor.Visit
        RenderRay(ray)
    End Sub

    Public Sub Visit(segment As Segment) Implements ValueVisitor.Visit
        RenderSegment(segment)
    End Sub

    Public Sub Visit(lambda As Lambda) Implements ValueVisitor.Visit
        ' Do Nothing
    End Sub

    Public Sub Visit(halfray As Ray) Implements ValueVisitor.Visit
        RenderHalfRay(halfray)
    End Sub
End Class
