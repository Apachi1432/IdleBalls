
Public Class Orb
    Private OriginalValue As Double

    Public ReadOnly Property Radius As Integer
    Public ReadOnly Property RequiredPadding As Integer
    Public ReadOnly Property CenterLocation As New Vector2D(0, 0)

    Public Const DefaultOriginalValue As Integer = 100
    Public Const DefaultRadius As Integer = 32
    Public Const DefaultRequiredPadding As Integer = 10

    Public Sub New(ByVal Value As Integer, ByVal Location As Vector2D)
        OriginalValue = Value
        _Value = Value
        _CenterLocation = Location

        Radius = DefaultRadius
        RequiredPadding = DefaultRequiredPadding
    End Sub

    Public ReadOnly Property Value As Integer

    Public ReadOnly Property Color As Color
        Get
            Dim Percentage As Double = (OriginalValue - Value) / OriginalValue

            If Percentage >= 1 Then Percentage = 1
            If Percentage <= 0 Then Percentage = 0

            Return HSVToRGB(200 * Percentage, 1, 1)
        End Get
    End Property
    Public Function Hit(ByVal Value As Integer) As Integer
        If Value >= Me.Value Then
            Hit = Me.Value
            _Value = 0
        Else
            Hit = Value
            _Value -= Value
        End If
    End Function

    Public Overloads Function ToString() As String
        Return $"X: {CenterLocation.X}, Y: {CenterLocation.Y}"
    End Function
End Class