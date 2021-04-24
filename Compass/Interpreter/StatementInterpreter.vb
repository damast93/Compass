﻿Imports Parser

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
        m_Scope = m_Scope.Add(varname, lhs)
        m_LastExpn = lhs
    End Sub

    Public Function Assign(varname As String, expr As Expression) As Void Implements StatementVisitor(Of Void).Assign
        Dim lhs = expr.Accept(New ExpressionInterpreter(m_Scope, m_Context))
        DoAssign(varname, lhs)
        Render(lhs)
        Return Nothing
    End Function

    Private Sub Render(value As Value)
        Dim mode = m_Context.Mode

        If mode <> DisplayMode.Off Then
            Dim geom = TryCast(value, Geometry)
            If geom IsNot Nothing Then
                geom.Accept(m_Context.Engine)
            End If
        End If
    End Sub

    Public Function Pick(vars As IEnumerable(Of String), expr As Expression) As Void Implements StatementVisitor(Of Void).Pick
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

        Render(pnts(n - 1))
        Return Nothing
    End Function

    Public Function Display(command As Parser.DisplayCommand) As Void Implements StatementVisitor(Of Void).Display
        Dim mode = m_Context.Mode

        Select Case command
            Case DisplayCommand.On
                If mode <> DisplayMode.Force Then m_Context.Mode = DisplayMode.On
            Case DisplayCommand.Off
                If mode <> DisplayMode.Force Then m_Context.Mode = DisplayMode.Off
            Case DisplayCommand.Force
                m_Context.Mode = DisplayMode.Force
            Case DisplayCommand.Push
                m_Context.Modes.Push(m_Context.Mode)
            Case DisplayCommand.Pop
                m_Context.Modes.Pop()
        End Select

        Return Nothing
    End Function

    Friend ReadOnly Property LastOperation As Value
        Get
            Return m_LastExpn
        End Get
    End Property

End Class
