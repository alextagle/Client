Public Class loadingScreen

    Dim loadTime As Integer = 0
    Private Sub loadingScreen_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.CenterToScreen()
        Timer1.Start()

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

    End Sub

    Private Sub Timer1_Tick_1(sender As Object, e As EventArgs) Handles Timer1.Tick
        loadTime += 1
        If loadTime = 7 Then
            PictureBox2.Visible = True
        ElseIf loadTime = 10 Then
            Me.Dispose()
            Form1.Visible = True
        End If
    End Sub
End Class