Public Module Extensions
    Public Structure HSV
        Dim H As Double
        Dim S As Double
        Dim V As Double
    End Structure

    Public Function RGBToHSV(ByVal rgb As Color) As HSV
        Return RGBToHSV(rgb.R, rgb.G, rgb.B)
    End Function
    Public Function RGBToHSV(ByVal R As Double, ByVal G As Double, ByVal B As Double) As HSV
        Dim del_r As Double = R / 255
        Dim del_g As Double = G / 255
        Dim del_b As Double = B / 255

        Dim c_max As Double = Math.Max(del_r, Math.Max(del_g, del_b))
        Dim c_min As Double = Math.Min(del_r, Math.Min(del_g, del_b))

        Dim delta As Double = c_max - c_min

        Dim h As Double = 0
        Dim s As Double = 0
        Dim v As Double = 0

        If delta <> 0 Then
            Select Case c_max
                Case del_r
                    h = ((del_g - del_b) / delta) Mod 6
                Case del_g
                    h = ((del_b - del_r) / delta) + 2
                Case del_b
                    h = ((del_r - del_g) / delta) + 4
            End Select

            h *= 60
        End If

        If c_max <> 0 Then
            s = delta / c_max
        End If

        v = c_max

#Disable Warning IDE0037 ' Use inferred member name
        Return New HSV With {
            .H = h,
            .S = s,
            .V = v
        }
#Enable Warning IDE0037 ' Use inferred member name
    End Function

    Public Function HSVToRGB(ByVal hsv As HSV) As Color
        Return HSVToRGB(hsv.H, hsv.S, hsv.V)
    End Function
    Public Function HSVToRGB(ByVal H As Double, ByVal S As Double, ByVal V As Double) As Color
        Dim C As Double = V * S
        Dim X As Double = C * (1 - Math.Abs(H / 60 Mod 2 - 1))
        Dim m As Double = V - C

        Dim r As Double = 0
        Dim g As Double = 0
        Dim b As Double = 0

        Select Case Math.Floor((H / 60) Mod 6)
            Case 0D
                r = C : g = X
            Case 1D
                r = X : g = C
            Case 2D
                g = C : b = X
            Case 3D
                g = X : b = C
            Case 4D
                r = X : b = C
            Case 5D
                r = C : b = X
        End Select

        Return Color.FromArgb((r + m) * 255, (g + m) * 255, (b + m) * 255)
    End Function

    Public Function FindCircleCircleIntersections(ByVal Center0 As Point, ByVal Radius0 As Single, ByVal Center1 As Point, ByVal Radius1 As Single, ByRef Optional IntersectingPoints() As PointF = Nothing) As Integer
        ' Find the distance between the centers
        Dim dx As Single = Center0.X - Center1.X
        Dim dy As Single = Center0.Y - Center1.Y
        Dim dist As Double = Math.Sqrt(dx * dx + dy * dy)

        ' See how many solutions there are
        If dist > Radius0 + Radius1 Then
            ' No intersections, the circles are too far apart
            IntersectingPoints = {
                New PointF(Single.NaN, Single.NaN),
                New PointF(Single.NaN, Single.NaN)
            }
            Return 0
        ElseIf dist < Math.Abs(Radius0 - Radius1) Then
            ' No intersections, one circle contains the other
            IntersectingPoints = {
                New PointF(Single.NaN, Single.NaN),
                New PointF(Single.NaN, Single.NaN)
            }
            Return 0
        ElseIf (dist = 0) AndAlso (Radius0 = Radius1) Then
            ' No intersections, the circles are the same
            IntersectingPoints = {
                New PointF(Single.NaN, Single.NaN),
                New PointF(Single.NaN, Single.NaN)
            }
            Return 0
        Else
            ' Find a and h
            Dim a As Double = (Radius0 * Radius0 - Radius1 * Radius1 + dist * dist) / (2 * dist)
            Dim h As Double = Math.Sqrt(Radius0 * Radius0 - a * a)

            ' Find P2
            Dim cx2 As Double = Center0.X + a * (Center1.X - Center0.X) / dist
            Dim cy2 As Double = Center0.Y + a * (Center1.Y - Center0.Y) / dist

            ' Get the points P3
            IntersectingPoints = {
                New PointF(cx2 + h * (Center1.Y - Center0.Y) / dist, cy2 - h * (Center1.X - Center0.X) / dist),
                New PointF(cx2 - h * (Center1.Y - Center0.Y) / dist, cy2 + h * (Center1.X - Center0.X) / dist)
            }

            ' See if we have 1 or 2 solutions
            If dist = Radius0 + Radius1 Then Return 1 Else Return 2
        End If
    End Function

    Public Function Normalize(ByVal A As PointF) As PointF
        Dim length As Single = Math.Sqrt(A.X * A.X + A.Y * A.Y)
        Return New PointF(A.X / length, A.Y / length)
    End Function
End Module

Public Class Vector2D
    Public Property X As Single = Single.NaN
    Public Property Y As Single = Single.NaN

    Public Function Add(ByVal v As Vector2D) As Vector2D
        Return New Vector2D(X + v.X, Y + v.Y)
    End Function
    Public Function Subtract(ByVal v As Vector2D) As Vector2D
        Return New Vector2D(X - v.X, Y - v.Y)
    End Function
    Public Function Length() As Single
        Return Math.Sqrt(X * X + Y * Y)
    End Function
    Public Function Normalize() As Vector2D
        Return New Vector2D(X / Length(), Y / Length())
    End Function


    Public Sub New(ByVal X As Single, ByVal Y As Single)
        Me.X = X
        Me.Y = Y
    End Sub
End Class