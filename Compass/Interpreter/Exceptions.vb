Imports System.Runtime.Serialization

<Serializable()>
Public Class TypeMismatchException
    Inherits Exception

    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub

    Public Sub New(ByVal message As String, ByVal inner As Exception)
        MyBase.New(message, inner)
    End Sub

    Public Sub New(
        ByVal info As SerializationInfo,
        ByVal context As StreamingContext)
        MyBase.New(info, context)
    End Sub
End Class

<Serializable()>
Public Class ArgumentException
    Inherits Exception

    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub

    Public Sub New(ByVal message As String, ByVal inner As Exception)
        MyBase.New(message, inner)
    End Sub

    Public Sub New(
        ByVal info As SerializationInfo,
        ByVal context As StreamingContext)
        MyBase.New(info, context)
    End Sub
End Class

<Serializable()>
Public Class VariableOutOfScopeException
    Inherits Exception

    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub

    Public Sub New(ByVal message As String, ByVal inner As Exception)
        MyBase.New(message, inner)
    End Sub

    Public Sub New(
        ByVal info As SerializationInfo,
        ByVal context As StreamingContext)
        MyBase.New(info, context)
    End Sub
End Class


<Serializable()>
Public Class InsufficientPickException
    Inherits Exception

    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub

    Public Sub New(ByVal message As String, ByVal inner As Exception)
        MyBase.New(message, inner)
    End Sub

    Public Sub New(
        ByVal info As SerializationInfo,
        ByVal context As StreamingContext)
        MyBase.New(info, context)
    End Sub
End Class
