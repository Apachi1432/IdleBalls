
Public Class Ball
    Private _Upgrades As Integer = 1
    Private _Weight As Integer = 1

    Public ReadOnly Property Radius As Integer = 15
    Public ReadOnly Property CenterLocation As New Vector2D(10, 10)
    Public ReadOnly Property Velocity As New Vector2D(4.5, 4.5)

    Public ReadOnly Property Color As Color = Color.White
    Public Const DefaultRadius As Integer = 15

    Public Sub New(ByVal Weight As Integer, ByVal Location As Vector2D, ByVal Optional IsMouse As Boolean = False)
        _Weight = Weight
        _CenterLocation = Location
        _Upgrades = 1

        If IsMouse Then _Radius = 1 Else _Radius = DefaultRadius
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
        _CenterLocation = CenterLocation.Add(Velocity)

        If CenterLocation.X >= s.Width - Radius Then
            CenterLocation.X = s.Width - Radius
            _Velocity = New Vector2D(-Velocity.X, Velocity.Y)
        ElseIf CenterLocation.X <= Radius Then
            CenterLocation.X = Radius
            _Velocity = New Vector2D(-Velocity.X, Velocity.Y)
        End If

        If CenterLocation.Y >= s.Height - Radius Then
            CenterLocation.Y = s.Height - Radius
            _Velocity = New Vector2D(Velocity.X, -Velocity.Y)
        ElseIf CenterLocation.Y < Radius Then
            CenterLocation.Y = Radius
            _Velocity = New Vector2D(Velocity.X, -Velocity.Y)
        End If
    End Sub
    Public Function CheckCollisions(ByRef _orbs As List(Of Orb)) As Double
        Dim TotalPoints As Integer = 0

        For Each o As Orb In _orbs.ToList

            If {CircleIntersectionPossibilities.Intersections_CirclesHaveIntersections, CircleIntersectionPossibilities.NoIntersections_OneContainsTheOther}.Contains(FindCircleCircleIntersections(o.CenterLocation, o.Radius, CenterLocation, Radius)) Then
                Dim angle As Double = Math.Atan2(o.CenterLocation.Y - CenterLocation.Y, o.CenterLocation.X - CenterLocation.X)
                Dim spread As Integer = Radius + o.Radius - o.CenterLocation.Subtract(CenterLocation).Length

                'solve collision(separation)
                _CenterLocation = CenterLocation.Subtract(New Vector2D(spread * Math.Cos(angle), spread * Math.Sin(angle)))

                Dim normalized As Vector2D = o.CenterLocation.Subtract(CenterLocation).Normalize

                _Velocity = normalized.Multiply(Velocity.Dot(normalized)).Multiply(-2).Add(Velocity)

                TotalPoints += o.Hit(Weight)

                If o.Value <= 0 Then _orbs.Remove(o)
            End If
        Next

        Return TotalPoints
    End Function
End Class