Public Class Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        IdleBallsGame1.StartNewGame()
    End Sub

    Private Sub IdleBallsGame1_HandleCreated(sender As Object, e As EventArgs) Handles IdleBallsGame1.HandleCreated
        IdleBallsGame1.StartNewGame()
    End Sub
End Class
