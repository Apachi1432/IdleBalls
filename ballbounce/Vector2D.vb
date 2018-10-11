Public Class Vector2d

    Public Property X As Single = 0
    Public Property Y As Single = 0

    Public Sub New()

    End Sub
    Public Sub New(ByVal x As Single, ByVal y As Single)
        Me.X = x
        Me.Y = y
    End Sub

    Public Function Dot(ByVal v2 As Vector2d) As Single
        Return X * v2.X + Y * v2.Y
    End Function
    Public Function getLength() As Single
        Return Math.Sqrt(X ^ 2 + Y ^ 2)
    End Function
    Public Function getDistance(ByVal v2 As Vector2d) As Single
        Return Math.Sqrt((v2.X - X) ^ 2 + (v2.Y - Y) ^ 2)
    End Function
    Public Function Add(ByVal v2 As Vector2d) As Vector2d
        Return New Vector2d(X + v2.X, Y + v2.Y)
    End Function
    Public Function Subtract(ByVal v2 As Vector2d) As Vector2d
        Return New Vector2d(X - v2.X, Y - v2.Y)
    End Function
    Public Function Multiply(ByVal scaleFactor As Single) As Vector2d
        Return New Vector2d(X * scaleFactor, Y * scaleFactor)
    End Function
    Public Function Normalize() As Vector2d
        Dim len As Single = getLength()

        If len <> 0.0F Then
            X /= len
            Y /= len
        Else
            X = 0.0F
            Y = 0.0F
        End If

        Return Me
    End Function

    Public Overloads Function toString() As String
        Return "X: " + X + " Y: " + Y
    End Function
End Class