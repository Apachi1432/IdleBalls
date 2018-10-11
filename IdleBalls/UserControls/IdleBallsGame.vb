Public Class IdleBallsGame
    Inherits UserControl

    Private WithEvents IdleBallsArena1 As IdleBallsArena

    Public Sub StartNewGame()
        IdleBallsArena1.StartNewGame()
    End Sub

    Public Sub New()
        Me.IdleBallsArena1 = New IdleBalls.IdleBallsArena()
        Me.SuspendLayout()
        '
        'IdleBallsArena2
        '
        Me.IdleBallsArena1.Level = 1
        Me.IdleBallsArena1.Location = New System.Drawing.Point(0, 100)
        Me.IdleBallsArena1.Name = "IdleBallsArena2"
        Me.IdleBallsArena1.Size = New System.Drawing.Size(500, 500)
        Me.IdleBallsArena1.StartingBallCount = 20
        Me.IdleBallsArena1.TabIndex = 0
        '
        'IdleBallsGame
        '
        Me.Controls.Add(Me.IdleBallsArena1)
        Me.Name = "IdleBallsGame"
        Me.ResumeLayout(False)

    End Sub

    Private Sub InitializeComponent()
        Me.SuspendLayout()
        '
        'IdleBallsArena
        '
        Me.Name = "IdleBallsArena"
        Me.ResumeLayout(False)
    End Sub

    Private Sub IdleBallsGame_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class
