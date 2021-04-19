Imports System.IO
Imports Compass.Display
Imports Compass.Interpreter

Class MainWindow
    Dim engine As DisplayEngine

    Private Sub Background_Loaded(sender As Object, e As RoutedEventArgs) Handles BackgroundCanvas.Loaded
        engine = New CanvasDisplay(BackgroundCanvas)

        Me.Samples.ItemsSource = New DirectoryInfo("Samples").EnumerateFiles()

        If Environment.GetCommandLineArgs().Length >= 2 Then
            txtSource.Text = File.ReadAllText(Environment.GetCommandLineArgs(1))
            Render()
        End If
    End Sub

    Private Sub CommandBinding_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        Render()
    End Sub

    Sub Render()
        Dim scope = New Scope

        BackgroundCanvas.Children.Clear()

        Try
            Dim expressions = Parser.Parser.Parse(txtSource.Text)
            Dim context = New Context(engine)
            Dim statementInterpreter = New StatementInterpreter(scope, context)
            For Each statement In expressions
                statement.Accept(statementInterpreter)
            Next
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub Samples_OnSelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim fileInfo = CType(Me.Samples.SelectedItem, FileInfo)
        Me.txtSource.Text = File.ReadAllText(fileInfo.FullName)

        Render()
    End Sub
End Class
