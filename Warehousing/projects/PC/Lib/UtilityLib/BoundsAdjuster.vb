Imports System.Drawing
Imports System.Windows.Forms

Public Class BoundsAdjuster

    Public Shared Function AdjustToScreen(ByVal bounds As Rectangle) As Rectangle
        Dim screenArea As Rectangle = Screen.FromRectangle(bounds).WorkingArea
        Return If(screenArea.Contains(bounds), bounds, AdjustToInside(bounds, screenArea))
    End Function

    Private Shared Function AdjustToInside(ByVal targetBounds As Rectangle, ByVal containerBounds As Rectangle) As Rectangle
        Dim width As Integer = Math.Min(targetBounds.Width, containerBounds.Width)
        Dim height As Integer = Math.Min(targetBounds.Height, containerBounds.Height)
        Dim x As Integer = Round(targetBounds.X, containerBounds.Left, containerBounds.Right - width)
        Dim y As Integer = Round(targetBounds.Y, containerBounds.Top, containerBounds.Bottom - height)
        Return New Rectangle(x, y, width, height)
    End Function

    Private Shared Function Round(ByVal value As Integer, ByVal minimun As Integer, ByVal maximun As Integer)
        Return Math.Min(Math.Max(value, minimun), maximun)
    End Function
End Class
