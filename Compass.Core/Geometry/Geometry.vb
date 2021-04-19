Namespace Geometry
    Public MustInherit Class Value
        Public MustOverride Sub Accept(visitor As ValueVisitor)
    End Class

    Public MustInherit Class Geometry
        Inherits Value

        Public MustOverride Function Intersect(other As Geometry) As Geometry
        Public MustOverride Function Pick(n As Integer) As IList(Of Point)
    End Class

    Public Interface ValueVisitor
        Sub Visit(point As Point)
        Sub Visit(circle As Circle)
        Sub Visit(namedPoint As NamedPoint)
        Sub Visit(ray As Line)
        Sub Visit(segment As Segment)
        Sub Visit(halfray As Ray)
        Sub Visit(lambda As Lambda)
    End Interface
End Namespace