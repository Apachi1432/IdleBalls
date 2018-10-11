Public Module Extensions
    Public Structure HSV
        Dim H As Double
        Dim S As Double
        Dim V As Double
    End Structure

    Public Enum CircleIntersectionPossibilities
        NoIntersections_TooFarApart = -1
        NoIntersections_OneContainsTheOther = -2
        NoIntersections_CirclesAreTheSame = -3
        Intersections_CirclesHaveIntersections = -4
    End Enum

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

    Public Function FindCircleCircleIntersections(ByVal Center0 As Vector2D, ByVal Radius0 As Double, ByVal Center1 As Vector2D, ByVal Radius1 As Double, ByRef Optional IntersectingPoints() As Vector2D = Nothing) As CircleIntersectionPossibilities
        ' Find the distance between the centers
        Dim dist As Double = Center0.Subtract(Center1).Length

        ' See how many solutions there are
        If dist > Radius0 + Radius1 Then
            ' No intersections, the circles are too far apart
            IntersectingPoints = {
                New Vector2D(Single.NaN, Single.NaN),
                New Vector2D(Single.NaN, Single.NaN)
            }
            Return CircleIntersectionPossibilities.NoIntersections_TooFarApart
        ElseIf dist < Math.Abs(Radius0 - Radius1) Then
            ' No intersections, one circle contains the other
            IntersectingPoints = {
                New Vector2D(Single.NaN, Single.NaN),
                New Vector2D(Single.NaN, Single.NaN)
            }
            Return CircleIntersectionPossibilities.NoIntersections_OneContainsTheOther
        ElseIf (dist = 0) AndAlso (Radius0 = Radius1) Then
            ' No intersections, the circles are the same
            IntersectingPoints = {
                New Vector2D(Double.NaN, Double.NaN),
                New Vector2D(Double.NaN, Double.NaN)
            }
            Return CircleIntersectionPossibilities.NoIntersections_CirclesAreTheSame
        Else
            ' Find a and h
            Dim a As Double = (Radius0 * Radius0 - Radius1 * Radius1 + dist * dist) / (2 * dist)
            Dim h As Double = Math.Sqrt(Radius0 * Radius0 - a * a)

            ' Find P2
            Dim c As Vector2D = Center1.Subtract(Center0).Multiply(a).Divide(dist).Add(Center0)

            ' Get the points P3
            IntersectingPoints = {
                New Vector2D(c.X + h * (Center1.Y - Center0.Y) / dist, c.Y - h * (Center1.X - Center0.X) / dist),
                New Vector2D(c.X - h * (Center1.Y - Center0.Y) / dist, c.Y + h * (Center1.X - Center0.X) / dist)
            }

            ' See if we have 1 or 2 solutions
            Return CircleIntersectionPossibilities.Intersections_CirclesHaveIntersections
        End If
    End Function

    Public Function RadsToDegs(ByVal Rads As Double) As Double
        Return Rads * 180 / Math.PI
    End Function

End Module

Public Class Vector2D
    Public Property X As Double = Double.NaN
    Public Property Y As Double = Double.NaN

    Public Function Add(ByVal v As Vector2D) As Vector2D
        Return New Vector2D(X + v.X, Y + v.Y)
    End Function
    Public Function Subtract(ByVal v As Vector2D) As Vector2D
        Return New Vector2D(X - v.X, Y - v.Y)
    End Function
    Public Function Multiply(ByVal scalar As Double) As Vector2D
        Return New Vector2D(X * scalar, Y * scalar)
    End Function
    Public Function Divide(ByVal scalar As Double) As Vector2D
        If scalar = 0D Then
            Return New Vector2D
        Else
            Return New Vector2D(X / scalar, Y / scalar)
        End If
    End Function
    Public Function Dot(ByVal v As Vector2D) As Double
        Return (X * v.X) + (Y * v.Y)
    End Function
    Public Function Angle(ByVal V As Vector2D) As Double
        Dim Len As Double = Length() * V.Length()

        If Len = 0D Then
            Return 0D
        Else
            Return Math.Acos(Dot(V) / Len)
        End If
    End Function
    Public Function Unit() As Double
        Dim Len As Double = Length()

        If Len = 0D Then
            Return 0D
        Else
            Return X / Length() + Y / Length()
        End If
    End Function
    Public Function Length() As Double
        ' Perform manual Square Root to keep as many decimal places as possible
        Return (X ^ 2 + Y ^ 2) ^ (0.5)
    End Function
    Public Function Magnitude() As Double
        Return Length()
    End Function
    Public Function Normalize() As Vector2D
        Return Divide(Length())
    End Function
    Public Function Projection(ByVal v As Vector2D) As Vector2D
        Dim Len As Double = Length()

        If Len = 0D Then
            Return New Vector2D
        Else
            Return Multiply(Dot(v) / Len ^ 2)
        End If
    End Function

    Public Sub New()
        X = 0
        Y = 0
    End Sub
    Public Sub New(ByVal X As Double, ByVal Y As Double)
        Me.X = X
        Me.Y = Y
    End Sub

    Public Shared Operator +(ByVal V1 As Vector2D, ByVal V2 As Vector2D) As Vector2D
        Return V1.Add(V2)
    End Operator
    Public Shared Operator -(ByVal V1 As Vector2D, ByVal V2 As Vector2D) As Vector2D
        Return V1.Subtract(V2)
    End Operator
    Public Shared Operator *(ByVal V1 As Vector2D, ByVal scalar As Double) As Vector2D
        Return V1.Multiply(scalar)
    End Operator
    Public Shared Operator /(ByVal V1 As Vector2D, ByVal scalar As Double) As Vector2D
        Return V1.Divide(scalar)
    End Operator

    Public Overloads Function ToString() As String
        Return $"X: {X}, Y: {Y}"
    End Function
End Class