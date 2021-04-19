Namespace Geometry
    Public Class Lambda : Inherits Value

        Private ReadOnly m_Op As Func(Of List(Of Value), Value)

        Public Sub New(op As Func(Of List(Of Value), Value))
            m_Op = op
        End Sub

        Default Public ReadOnly Property Eval(args As IEnumerable(Of Value)) As Value
            Get
                Return m_Op(args.ToList())
            End Get
        End Property

        Public Overrides Sub Accept(visitor As ValueVisitor)
            Return
        End Sub
    End Class
End Namespace