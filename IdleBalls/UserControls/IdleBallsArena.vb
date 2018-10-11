Imports System.ComponentModel

Public Class IdleBallsArena
    Inherits UserControl

#Region "Private Variables, etc"
    Private WithEvents TRender As New Timer With {.Enabled = False, .Interval = 10}

    Private _orbs As New List(Of Orb)
    Private _balls As New List(Of Ball)

    Private _UnclaimedPoints As Double = 0
#End Region

#Region "Properties"
    <Localizable(False)>
    <Bindable(False)>
    <Browsable(True)>
    <DefaultValue(1)>
    <Category("IdleBallsArena")>
    <Description("Indicates the current level of the game in progress.")>
    Public Property Level As Integer = 1

    <Localizable(False)>
    <Bindable(False)>
    <Browsable(True)>
    <DefaultValue(14)>
    <Category("IdleBallsArena")>
    <Description("Indicates how many orbs should be shown at the start of the game.")>
    Public Property StartingOrbCount As Integer = 14

    <Localizable(False)>
    <Bindable(False)>
    <Browsable(True)>
    <DefaultValue(1)>
    <Category("IdleBallsArena")>
    <Description("Indicates how many orbs should be shown at the start of the game.")>
    Public Property StartingBallCount As Integer = 3

    <Localizable(False)>
    <Bindable(False)>
    <Browsable(True)>
    <DefaultValue(30)>
    <Category("IdleBallsArena")>
    <Description("Indicates how far from the sides of the game orbs should spawn.")>
    Public Property WallPadding As Integer = 30

    <DefaultValue(GetType(Size), "500, 500")>
    Public Overrides Property MinimumSize() As Size
        Get
            Return New Size(500, 500)
        End Get
        Set(value As Size)
            ' Do Nothing
        End Set
    End Property

    <DefaultValue(GetType(Color), "Black")>
    Public Overrides Property BackColor() As Color
        Get
            Return MyBase.BackColor
        End Get
        Set(value As Color)
            MyBase.BackColor = value
        End Set
    End Property
#End Region

#Region "Constructor"
    Public Sub New()
        Me.Name = "IdleBallsArena"
        Me.Size = New Size(500, 500)
        Me.SetStyle(ControlStyles.DoubleBuffer, True)
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        Me.SetStyle(ControlStyles.ResizeRedraw, True)
        Me.SetStyle(ControlStyles.UserPaint, True)
        Me.SetStyle(ControlStyles.SupportsTransparentBackColor, True)

        Me.BackColor = Color.Black

        Me.SuspendLayout()
        ' Add Other Controls?
        Me.ResumeLayout()
    End Sub
#End Region

#Region "Overridden Methods"
    Protected Overrides Sub OnPaintBackground(e As PaintEventArgs)
        MyBase.OnPaintBackground(e)
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        'MyBase.OnPaint(e)

        e.Graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        e.Graphics.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic

        If Not IsHandleCreated Then Return

        For Each o As Orb In _orbs
            Dim pn As New Pen(o.Color)

            Dim rect As New Rectangle(o.CenterLocation.X - o.Radius, o.CenterLocation.Y - o.Radius, o.Radius * 2, o.Radius * 2)
            e.Graphics.DrawEllipse(pn, rect)
            e.Graphics.FillEllipse(pn.Brush, rect)

            e.Graphics.DrawString(o.Value, Font, New Pen(Color.Black).Brush, rect, New StringFormat() With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center})
        Next

        For Each b As Ball In _balls
            Dim pn As New Pen(b.Color)

            Dim rect As New Rectangle(b.CenterLocation.X - b.Radius, b.CenterLocation.Y - b.Radius, b.Radius * 2, b.Radius * 2)
            e.Graphics.DrawEllipse(pn, rect)
            e.Graphics.FillEllipse(pn.Brush, rect)
        Next

    End Sub

    Public Overrides Function GetPreferredSize(proposedSize As Size) As Size
        If proposedSize.Width < 500 Then proposedSize.Width = 500
        If proposedSize.Height < 500 Then proposedSize.Height = 500

        Return proposedSize
    End Function
#End Region

#Region "Private Methods"
    Private Sub GenerateOrbs()
        _orbs = New List(Of Orb)

        Dim rand As New Random()

        For I = 0 To StartingOrbCount - 1
            ' Flag which holds true whenever a new circle was found
            Dim newCircleFound As Boolean = False
            Dim TryCount As Integer = 0

            ' Loop iteration which runs until we find a circle that doesn't intersect with the others
            While Not newCircleFound
                Dim NewCenter As New Vector2D(
                    rand.Next(WallPadding + Orb.DefaultRequiredPadding, Width - WallPadding * 2 - Orb.DefaultRequiredPadding * 2),
                    rand.Next(WallPadding + Orb.DefaultRequiredPadding, Height - WallPadding * 2 - Orb.DefaultRequiredPadding * 2)
                )

                If TryCount >= 100 Then
                    I = 0
                    _orbs = New List(Of Orb)
                Else
                    TryCount += 1
                End If

                For Each o As Orb In _orbs
                    If {CircleIntersectionPossibilities.Intersections_CirclesHaveIntersections, CircleIntersectionPossibilities.NoIntersections_CirclesAreTheSame}.Contains(FindCircleCircleIntersections(o.CenterLocation, o.Radius + o.RequiredPadding * 2, NewCenter, o.Radius + o.RequiredPadding * 2)) Then
                        ' Overlaps
                        Continue While
                    Else
                        ' Doesn't Overlap
                        Continue For
                    End If
                Next

                _orbs.Add(New Orb(100 ^ Level, NewCenter))
                newCircleFound = True
            End While
        Next
    End Sub
    Private Sub GenerateBalls()
        _balls = New List(Of Ball)

        Dim rand As New Random()

        For I = 0 To StartingBallCount - 1
            ' Flag which holds true whenever a new circle was found
            Dim newCircleFound As Boolean = False

            ' Loop iteration which runs until we find a circle that doesn't intersect with the others
            While Not newCircleFound
                Dim NewCenter As New Vector2D(
                    rand.Next(Ball.DefaultRadius, Width - Ball.DefaultRadius),
                    rand.Next(Ball.DefaultRadius, Height - Ball.DefaultRadius)
                )

                For Each o As Orb In _orbs
                    If FindCircleCircleIntersections(o.CenterLocation, o.Radius, NewCenter, Ball.DefaultRadius) = CircleIntersectionPossibilities.Intersections_CirclesHaveIntersections Then
                        ' Overlaps
                        Continue While
                    Else
                        ' Doesn't Overlap
                        Continue For
                    End If
                Next

                _balls.Add(New Ball(5, NewCenter))
                newCircleFound = True
            End While
        Next
    End Sub
    Private Sub DrawPoint(e As PaintEventArgs, pn As Pen, p As PointF)
        Const RADIUS As Integer = 3
        e.Graphics.DrawEllipse(pn, p.X - RADIUS, p.Y - RADIUS, 2 * RADIUS, 2 * RADIUS)
        e.Graphics.FillEllipse(pn.Brush, p.X - RADIUS, p.Y - RADIUS, 2 * RADIUS, 2 * RADIUS)
    End Sub

#End Region

#Region "Public Methods"
    Public Sub StartNewGame()
        GenerateOrbs()
        GenerateBalls()
        TRender.Start()
        Invalidate()
    End Sub
#End Region

    Private Sub IdleBallsArena_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
    Private Sub InitializeComponent()
        Me.SuspendLayout()
        '
        'IdleBallsArena
        '
        Me.Name = "IdleBallsArena"
        Me.ResumeLayout(False)
    End Sub

    Private Sub TRender_Tick(sender As Object, e As EventArgs) Handles TRender.Tick
        TRender.Enabled = False

        For Each b As Ball In _balls
            b.Move(Size)
            _UnclaimedPoints += b.CheckCollisions(_orbs)
        Next

        If _orbs.Count = 0 Then
            'MessageBox.Show(_UnclaimedPoints)

            Level += 1
            _balls.ForEach(Sub(b) b.Upgrade())
            GenerateOrbs()
        End If

        Invalidate()

        TRender.Enabled = True
    End Sub

    Private Sub IdleBallsArena_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        _UnclaimedPoints += New Ball(100 * Level, New Vector2D(e.Location.X, e.Location.Y), True).CheckCollisions(_orbs)
    End Sub
End Class
