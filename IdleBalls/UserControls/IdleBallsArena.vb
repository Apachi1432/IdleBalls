Imports System.ComponentModel

Public Class IdleBallsArena
    Inherits UserControl

#Region "Private Variables, etc"
    Private WithEvents TRender As New Timer With {.Enabled = True, .Interval = 10}

    Private Class Orb
        Public ReadOnly Property MaxValue As Integer = 100
        Public ReadOnly Property HitValue As Integer = 0

        Public Shared ReadOnly Property Radius As Integer = 32
        Public Shared ReadOnly Property RequiredPadding As Integer = 10
        Public ReadOnly Property CenterLocation As New Point(0, 0)

        Public Sub New(ByVal Value As Integer, ByVal Location As Point)
            _MaxValue = Value
            _CenterLocation = Location
        End Sub

        Public ReadOnly Property Value As Integer
            Get
                If HitValue >= MaxValue Then Return 0
                If HitValue <= 0 Then Return MaxValue

                Return MaxValue - HitValue
            End Get
        End Property
        Public ReadOnly Property Color As Color
            Get
                Dim Percentage As Double = HitValue / MaxValue

                If Percentage >= 1 Then Percentage = 1
                If Percentage <= 0 Then Percentage = 0

                Return HSVToRGB(200 * Percentage, 1, 1)
            End Get
        End Property
        Public Sub Hit(ByVal Value As Integer)
            _HitValue += Value
        End Sub
    End Class
    Private Class Ball
        Private _Upgrades As Integer = 1
        Private _Weight As Integer = 1

        Public Shared ReadOnly Property Radius As Integer = 10
        Public ReadOnly Property CenterLocation As New Point(10, 10)
        Public ReadOnly Property Velocity As New PointF(4, 4)

        Public ReadOnly Property Color As Color = Color.White

        Public Sub New(ByVal Weight As Integer, ByVal Location As Point)
            _Weight = Weight
            _CenterLocation = Location
            _Upgrades = 1
        End Sub

        Public ReadOnly Property Weight As Integer
            Get
                Return _Weight ^ _Upgrades
            End Get
        End Property
        Public Sub Upgrade()
            _Upgrades += 1
        End Sub
        Public Sub Move(ByVal s As Size)
            _CenterLocation = New Point(CenterLocation.X + Velocity.X, CenterLocation.Y + Velocity.Y)

            If CenterLocation.X + Velocity.X > s.Width - Radius OrElse CenterLocation.X + Velocity.X < Radius Then
                _Velocity = New Point(-Velocity.X, Velocity.Y)
            End If

            If CenterLocation.Y + Velocity.Y > s.Height - Radius OrElse CenterLocation.Y + Velocity.Y < Radius Then
                _Velocity = New Point(Velocity.X, -Velocity.Y)
            End If
        End Sub
        Public Sub CheckCollisions(ByRef _orbs As List(Of Orb))
            Dim MinDistance As Integer = Radius + Orb.Radius

            For Each o As Orb In _orbs.ToList
                If FindCircleCircleIntersections(o.CenterLocation, Orb.Radius, CenterLocation, Radius) Then
                    Dim distance As Integer = Math.Sqrt((Math.Abs(o.CenterLocation.X - CenterLocation.X) ^ 2) + (Math.Abs(o.CenterLocation.Y - CenterLocation.Y) ^ 2))
                    Dim angle As Double = Math.Atan2(o.CenterLocation.Y - CenterLocation.Y, o.CenterLocation.X - CenterLocation.X)
                    Dim spread As Integer = MinDistance - distance

                    Dim ax As Double = spread * Math.Cos(angle)
                    Dim ay As Double = spread * Math.Sin(angle)

                    ' solve collision (separation)
                    '_CenterLocation = New Point(CenterLocation.X - ax, CenterLocation.Y - ay)

                    Dim dx As Single = o.CenterLocation.X - CenterLocation.X
                    Dim dy As Single = o.CenterLocation.Y - CenterLocation.Y

                    Dim length As Single = Math.Sqrt(dx * dx + dy * dy)

                    dx /= length
                    dy /= length

                    dx *= Velocity.X
                    dy *= Velocity.Y

                    _CenterLocation = New Point(CenterLocation.X + dx, CenterLocation.Y + dy)

                    'Dim normalized As PointF = Normalize(New PointF(o.CenterLocation.X - CenterLocation.X, o.CenterLocation.Y - CenterLocation.Y))
                    '_Velocity = New PointF(
                    '    (Velocity.X - (2 * (normalized.X * Velocity.X + normalized.Y * Velocity.Y)) * normalized.X),
                    '    (Velocity.Y - (2 * (normalized.X * Velocity.X + normalized.Y * Velocity.Y)) * normalized.Y)
                    ')

                    o.Hit(Weight)
                    If o.Value = 0 Then
                        _orbs.Remove(o)
                    End If
                End If
            Next
        End Sub
    End Class

    Private _orbs As New List(Of Orb)
    Private _balls As New List(Of Ball)
    Private _backcolor As Color = Color.Black
#End Region

#Region "Properties"
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
    Public Property StartingBallCount As Integer = 2

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

            Dim rect As New Rectangle(o.CenterLocation.X - Orb.Radius, o.CenterLocation.Y - Orb.Radius, Orb.Radius * 2, Orb.Radius * 2)
            e.Graphics.DrawEllipse(pn, rect)
            e.Graphics.FillEllipse(pn.Brush, rect)

            e.Graphics.DrawString(o.Value, Font, New Pen(Color.Black).Brush, rect, New StringFormat() With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center})
        Next

        For Each b As Ball In _balls
            Dim pn As New Pen(b.Color)

            Dim rect As New Rectangle(b.CenterLocation.X - Ball.Radius, b.CenterLocation.Y - Ball.Radius, Ball.Radius * 2, Ball.Radius * 2)
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

            ' Loop iteration which runs until we find a circle that doesn't intersect with the others
            While Not newCircleFound
                Dim NewCenter As New Point(
                    rand.Next(WallPadding + Orb.RequiredPadding, Width - WallPadding * 2 - Orb.RequiredPadding * 2),
                    rand.Next(WallPadding + Orb.RequiredPadding, Height - WallPadding * 2 - Orb.RequiredPadding * 2)
                )

                Dim p() As PointF = {
                    New PointF(Single.NaN, Single.NaN),
                    New PointF(Single.NaN, Single.NaN)
                }

                For Each o As Orb In _orbs

                    If FindCircleCircleIntersections(o.CenterLocation, Orb.Radius + Orb.RequiredPadding * 2, NewCenter, Orb.Radius + Orb.RequiredPadding * 2, p) <> 0 Then
                        ' Overlaps
                        Continue While
                    Else
                        ' Doesn't Overlap
                        Continue For
                    End If
                Next

                _orbs.Add(New Orb(Integer.MaxValue, NewCenter))
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
                Dim NewCenter As New Point(
                    rand.Next(Ball.Radius, Width - Ball.Radius),
                    rand.Next(Ball.Radius, Height - Ball.Radius)
                )

                For Each o As Orb In _orbs
                    If FindCircleCircleIntersections(o.CenterLocation, Orb.Radius, NewCenter, Ball.Radius) <> 0 Then
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
        For Each b As Ball In _balls
            b.Move(Size)
            b.CheckCollisions(_orbs)
        Next

        If _orbs.Count = 0 Then
            GenerateOrbs()
        End If

        Invalidate()
    End Sub
End Class
