Imports Compass.Parser

Imports System.IO

Imports WPFLine = System.Windows.Shapes.Line

Class MainWindow
    Dim engine As DisplayEngine

    Private Function NewScope() As Scope
        Dim builtinMethods = Builtins.GetBuiltins()

        Dim glob = New GlobalScope(builtinMethods, New Dictionary(Of String, Value) From {{"ans", Nothing}})
        Dim sc = New Scope(glob)
        Return sc
    End Function

    Private Sub Notify(str As String)
        txtNotify.Text = str
    End Sub

    Sub RunSource(source As String)
        Dim sc = NewScope()
        Dim exps As List(Of Parser.Statement)

        Try
            exps = Parser.Parser.Parse(source).ToList()
        Catch ex As Parser.SyntaxError
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Syntax error")
            Notify("Terminated with syntax error")
            Exit Sub
        End Try

        Background.Children.Clear()

        Dim cont = New Context(engine)
        Dim int = New StatementInterpreter(sc, cont)

        Notify("Running ...")

        For Each stm In exps
            Try
                stm.Accept(int)
            Catch ex As RuntimeException
                Dim errLn = stm.Accept(LineNumberVisitor.Instance)

                MsgBox(String.Format("Error in Line {0}:{3}{1}{3}{3}{2}", errLn, txtSource.GetLineText(errLn - 1), ex.Message, Environment.NewLine), MsgBoxStyle.Critical, ex.Description)
                Notify("Terminated with runtime error")
                Exit Sub
            Catch ex As Exception
                Dim errLn = stm.Accept(LineNumberVisitor.Instance)

                MsgBox("Internal error" & Environment.NewLine & ex.Message, MsgBoxStyle.Critical, ex.GetType().Name)
                Notify("Terminated with internal error")
                Exit Sub
            End Try
        Next

        Notify("Ready")
    End Sub

    Private Sub Background_Loaded(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles Background.Loaded
        AddHandler txtSource.SelectionChanged, AddressOf txtSource_TextChanged
        engine = New CanvasDisplay(Background)

        Dim sc = NewScope()

        If Environment.GetCommandLineArgs().Length >= 2 Then
            Dim src = My.Computer.FileSystem.ReadAllText(Environment.GetCommandLineArgs(1))
            txtSource.Text = src
            RunSource(src)
        End If
    End Sub

    Private Sub CommandBinding_Executed(sender As System.Object, e As System.Windows.Input.ExecutedRoutedEventArgs)
        Render()
    End Sub

    Private Sub SaveCanvas(sender As System.Object, e As System.Windows.Input.ExecutedRoutedEventArgs)
        Dim cnt = 1
        Dim fn As String

        Do
            fn = "Save" & CStr(cnt) & ".png"
            If Not IO.File.Exists(fn) Then
                Exit Do
            Else
                cnt += 1
            End If
        Loop

        Dim size = New Size(Background.ActualWidth, Background.ActualHeight)
        Background.Measure(size)
        Background.Arrange(New Rect(size))

        Dim renderBitmap = New RenderTargetBitmap(
                           CInt(size.Width),
                           CInt(size.Height),
                           96,
                           96,
                           PixelFormats.Pbgra32)
        renderBitmap.Render(Background)

        Using fstream = New FileStream(fn, FileMode.Create)
            Dim encoder = New PngBitmapEncoder
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap))
            encoder.Save(fstream)
        End Using

        Notify("Saved as " & fn)
    End Sub

    Sub Render()
        RunSource(txtSource.Text)
    End Sub

    Private Sub txtSource_TextChanged(sender As System.Object, e As System.Windows.RoutedEventArgs)
        txtPosInfo.Text = String.Format("Line {0}/{1}",
                                        txtSource.GetLineIndexFromCharacterIndex(txtSource.CaretIndex) + 1,
                                        txtSource.LineCount)
    End Sub
End Class
