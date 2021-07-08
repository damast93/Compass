Imports Compass.Core.Geometry

Namespace Interpreter
    Public Class Scope
        Private ReadOnly m_Varname As String
        Private ReadOnly m_Value As Value

        Private ReadOnly m_Outer As Scope

        Private Sub New(varname As String, value As Value, outer As Scope)
            m_Varname = varname
            m_Value = value
            m_Outer = outer
        End Sub

        Public Sub New()
            m_Outer = Nothing
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

        Public Function Lookup(varname As String) As Scope
            If m_Outer Is Nothing Then
                Return Nothing
            ElseIf varname = m_Varname Then
                Return Me
            Else
                Return m_Outer.Lookup(varname)
            End If
        End Function

        Default Public ReadOnly Property LookupValue(varname As String) As Value
            Get
                Dim lookupScope = Lookup(varname)
                If lookupScope Is Nothing Then
                    Throw New VariableOutOfScopeException("Variable " & varname & " not in scope")
                Else
                    Return lookupScope.Value
                End If
            End Get
        End Property
    End Class
End Namespace