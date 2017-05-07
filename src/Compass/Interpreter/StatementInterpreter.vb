Imports Compass.Parser

Public NotInheritable Class Void
    Protected Sub New()
    End Sub
End Class

Public Class StatementInterpreter : Implements StatementVisitor(Of Void)
    Private m_Scope As Scope
    Private m_Context As Context
    Private m_LastExpn As Value

    Public Sub New(scope As Scope, context As Context)
        m_Scope = scope
        m_Context = context
    End Sub

    Private Sub DoAssign(varname As String, lhs As Value)
        If m_Scope.IsTemp(varname) Then
            m_Scope.SetTemp(varname, lhs)
        Else
            m_Scope = m_Scope.Add(varname, lhs)
        End If
        m_LastExpn = lhs
    End Sub

    Public Function Assign(varname As String, expr As Expression, lineNumber As Integer) As Void Implements StatementVisitor(Of Void).Assign
        Dim lhs = expr.Accept(New ExpressionInterpreter(m_Scope, m_Context))
        DoAssign(varname, lhs)
        Return Nothing
    End Function

    Public Function Pick(vars As IEnumerable(Of String), expr As Expression, lineNumber As Integer) As Void Implements Parser.StatementVisitor(Of Void).Pick
        Dim int = New ExpressionInterpreter(m_Scope, m_Context)
        Dim lhs = expr.Accept(int)
        Dim varList = vars.ToList()
        Dim n = varList.Count

        Dim pnts = lhs.AsOf(Of Geometry)().Pick(n)
        If pnts.Count <> n Then
            Throw New InsufficientPickException("Cannot pick " & n.ToString() & " points from given set")
        End If

        For i = 0 To n - 1
            DoAssign(varList(i), pnts(i))
        Next
        Return Nothing
    End Function

    Public Function Display(arg As Expression, lineNumber As Integer) As Void Implements Parser.StatementVisitor(Of Void).Display
        Dim x = arg.Accept(New ExpressionInterpreter(m_Scope, m_Context)).AsOf(Of Geometry)()
        x.Accept(m_Context.Engine)
        Return Nothing
    End Function

    Friend ReadOnly Property LastOperation As Value
        Get
            Return m_LastExpn
        End Get
    End Property

End Class

Public NotInheritable Class LineNumberVisitor : Implements StatementVisitor(Of Integer)
    Private Sub New()

    End Sub

    Private Shared ReadOnly inst As LineNumberVisitor = New LineNumberVisitor()

    Public Shared ReadOnly Property Instance As LineNumberVisitor
        Get
            Return inst
        End Get
    End Property

    Public Function Assign(Param As String, Param1 As Parser.Expression, ln As Integer) As Integer Implements Parser.StatementVisitor(Of Integer).Assign
        Return ln
    End Function

    Public Function Display(Param As Parser.Expression, ln As Integer) As Integer Implements Parser.StatementVisitor(Of Integer).Display
        Return ln
    End Function

    Public Function Pick(Param As System.Collections.Generic.IEnumerable(Of String), Param1 As Parser.Expression, ln As Integer) As Integer Implements Parser.StatementVisitor(Of Integer).Pick
        Return ln
    End Function
End Class