Public Class Context
    Private m_Engine As DisplayEngine

    Public Sub New(engine As DisplayEngine)
        m_Engine = engine
    End Sub

    Public ReadOnly Property Engine As DisplayEngine
        Get
            Return m_Engine
        End Get
    End Property
End Class
