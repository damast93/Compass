Public Class GlobalScope
    Private m_BuiltinBindings As Dictionary(Of String, Value)
    Private m_TempVars As New Dictionary(Of String, Value)

    Public Sub New(builtins As Dictionary(Of String, Value), temps As Dictionary(Of String, Value))
        m_BuiltinBindings = builtins
        m_TempVars = temps
    End Sub

    Public Function IsBuiltin(var As String) As Boolean
        Return m_BuiltinBindings.ContainsKey(var)
    End Function

    Public Function IsTemp(var As String) As Boolean
        Return m_TempVars.ContainsKey(var)
    End Function

    Public ReadOnly Property Builtin(var As String) As Value
        Get
            Return m_BuiltinBindings(var)
        End Get
    End Property

    Public Property Temp(var As String) As Value
        Get
            Return m_TempVars(var)
        End Get
        Set(value As Value)
            m_TempVars(var) = value
        End Set
    End Property
End Class

Public Class Scope
    Private ReadOnly m_Varname As String
    Private ReadOnly m_Value As Value

    Private ReadOnly m_Global As GlobalScope
    Private ReadOnly m_Outer As Scope

    Private Sub New(varname As String, value As Value, outer As Scope)
        m_Varname = varname
        m_Value = value
        m_Outer = outer
        m_Global = outer.m_Global
    End Sub

    Public Sub New(globalScope As GlobalScope)
        m_Outer = Nothing
        m_Global = globalScope
    End Sub

    Public Function Add(varname As String, value As Value) As Scope
        Return New Scope(varname, value, Me)
    End Function

    Public ReadOnly Property VarName As String
        Get
            Return m_Varname
        End Get
    End Property

    Public ReadOnly Property Value As Value
        Get
            Return m_Value
        End Get
    End Property

    Default Public ReadOnly Property LookupValue(varname As String) As Value
        Get
            If m_Global.IsTemp(varname) Then
                Return m_Global.Temp(varname)
            End If

            If m_Outer IsNot Nothing Then
                If m_Varname = varname Then
                    Return m_Value
                Else
                    Return m_Outer.LookupValue(varname)
                End If
            Else
                If m_Global.IsBuiltin(varname) Then
                    Return m_Global.Builtin(varname)
                Else
                    Throw New VariableOutOfScopeException("Variable " & varname & " does not exist in current scope.")
                End If
            End If
        End Get
    End Property

    Public Function IsTemp(var As String) As Boolean
        Return m_Global.IsTemp(var)
    End Function

    Public Sub SetTemp(var As String, value As Value)
        m_Global.Temp(var) = value
    End Sub
End Class
