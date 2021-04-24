Public Enum DisplayMode
    [On]
    Off
    Force
End Enum
Public Class Context
    Private m_Engine As DisplayEngine
    Public Modes As New Stack(Of DisplayMode)

    Public Property Mode As DisplayMode
        Get
            Return Modes.Peek()
        End Get
        Set(value As DisplayMode)
            Modes.Pop()
            Modes.Push(value)
        End Set
    End Property


    Public Sub New(engine As DisplayEngine)
        m_Engine = engine
        Modes.Push(DisplayMode.On)
    End Sub

    Public ReadOnly Property Engine As DisplayEngine
        Get
            Return m_Engine
        End Get
    End Property
End Class
