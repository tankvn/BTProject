Imports System.Windows.Forms
Imports System.Drawing

Public Class DropDownWidthCalculator
    Public Shared Sub AdjustDropDownWidth(ByVal sender As Object, ByVal e As EventArgs)
        Dim comboBox = TryCast(sender, ComboBox)
        comboBox.DropDownWidth = CalculateAdjustedDropDownWidth(comboBox)
    End Sub

    Public Shared Function CalculateAdjustedDropDownWidth(ByVal comboBox As ComboBox) As Integer
        Dim maximumTextWidth As Integer = CalculateMaximumTextWidth(comboBox)

        Dim menuWidth As Integer = maximumTextWidth + SystemInformation.VerticalScrollBarWidth

        Dim adjustedWidth As Integer = AdjustToScreen(comboBox, menuWidth)

        Return Math.Max(1, adjustedWidth)
    End Function

    Private Shared Function CalculateMaximumTextWidth(ByVal comboBox As ComboBox) As Integer
        If (comboBox.Items.Count = 0) Then Return 1

        Using graphics As Graphics = comboBox.CreateGraphics()
            Dim width = comboBox.Items.Cast(Of Object)().Select(Function(obj As Object) comboBox.GetItemText(obj)).Select(Function(text As String) graphics.MeasureString(text, comboBox.Font).Width).Max()

            Return Convert.ToInt32(Math.Ceiling(width))
        End Using
    End Function

    Private Shared Function AdjustToScreen(ByVal comboBox As ComboBox, ByVal dropDownWidth As Integer) As Integer
        Dim screenBounds As Rectangle = Screen.AllScreens.Aggregate(Rectangle.Empty, Function(rectangle As Rectangle, screen As Screen) Union(rectangle, screen))
        Dim screenPoint As Point = comboBox.PointToScreen(Point.Empty)
        Return If((screenBounds.Right < screenPoint.X + dropDownWidth), screenBounds.Right - screenPoint.X, dropDownWidth)
    End Function

    Private Shared Function Union(ByVal rectangle As Rectangle, ByVal screen As Screen) As Rectangle
        Return If(rectangle.IsEmpty, screen.Bounds, rectangle.Union(rectangle, screen.Bounds))
    End Function

End Class
