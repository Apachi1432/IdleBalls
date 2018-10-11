Public Class Ball
    Public velocity As Vector2d
    Public position As Vector2d

    Public Property mass As Single
    Public Property radius As Single

    Private angularVel As Single
    Private orientation As Single = 45.0F

    Public Sub New(ByVal x As Single, ByVal y As Single, ByVal radius As Single)
        Me.New(x, y, radius, 1)
    End Sub
    Public Sub New(ByVal x As Single, ByVal y As Single, ByVal radius As Single, ByVal mass As Single)
        velocity = New Vector2d(0, 0)
        position = New Vector2d(x, y)
        Me.mass = mass
        Me.radius = radius
    End Sub

    Public Function getBallColor(ByVal magnitude As Single) As Color
        Dim maxMagnitude As Single = 1000 ' tweak this To Get the right color range

        magnitude = If(magnitude < maxMagnitude, magnitude, maxMagnitude)

        Dim H As Single = (magnitude / maxMagnitude) * 0.38F ' 0.4F = green
        Dim S As Single = 0.98F
        Dim B As Single = 0.95F

        Return Color.White 'Color.getHSBColor(H, S, B)
    End Function


    Public Sub draw(ByVal g2 As Graphics)
        Dim pn As New Pen(getBallColor(velocity.getLength()))
        Dim rect As New Rectangle(position.X - radius, position.Y - radius, 2 * radius, 2 * radius)

        g2.DrawEllipse(pn, rect)
        g2.FillEllipse(pn.Brush, rect)
    End Sub

    ' Deprecated, rolled this into checkCollision for effeciency.
    Public Function colliding(ByVal ball As Ball) As Boolean
        Dim D As Vector2d = position.Subtract(ball.position)

        Dim sqrRadius As Single = (radius + ball.radius) ^ 2
        Dim distSqr As Single = D.X ^ 2 + D.Y ^ 2

        If distSqr <= sqrRadius Then Return True

        Return False
    End Function
    Public Sub resolveCollision(ByVal ball As Ball)
        ' get the mtd
        Dim delta As Vector2d = position.Subtract(ball.position)
        Dim r As Single = radius + ball.radius
        Dim dist2 As Single = delta.dot(delta)

        If dist2 > r * r Then Return ' they aren't colliding

        Dim d As Single = delta.getLength()

        If d = 0.0F Then ' Special Case. Balls are exactly On top Of eachother.  Don't want to divide by zero.
            d = ball.radius + radius - 1.0F
            delta = New Vector2d(ball.radius + radius, 0.0F)
        End If

        Dim mtd As Vector2d = delta.Multiply((r - d) / d) ' minimum translation distance To push balls apart after intersecting

        ' resolve intersection
        Dim im1 As Single = 1 / mass ' inverse mass quantities
        Dim im2 As Single = 1 / ball.mass

        ' push-pull them apart
        position = position.add(mtd.multiply(im1 / (im1 + im2)))
        ball.position = ball.position.subtract(mtd.multiply(im2 / (im1 + im2)))

        ' impact speed
        Dim v As Vector2d = velocity.subtract(ball.velocity)
        Dim vn As Single = v.dot(mtd.normalize())

        ' sphere intersecting but moving away from each other already
        If vn > 0.0F Then Return

        ' collision impulse
        Dim i As Single = (-(1.0F + restitution) * vn) / (im1 + im2)
        Dim impulse As Vector2d = mtd.multiply(i)

        ' change in momentum
        velocity = velocity.add(impulse.multiply(im1))
        ball.velocity = ball.velocity.subtract(impulse.multiply(im2))
    End Sub

    Public Function compareTo(ByVal ball As Ball) As Integer
        If position.X - radius > ball.position.X - ball.radius Then
            Return 1
        ElseIf position.X - radius < ball.position.X - ball.radius Then
            Return -1
        Else
            Return 0
        End If
    End Function
End Class