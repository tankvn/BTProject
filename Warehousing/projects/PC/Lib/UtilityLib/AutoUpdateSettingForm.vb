Imports System.Windows.Forms

Public Class AutoUpdateSettingForm
    Public Property UpdateFrequency() As Integer
        Get
            Return Convert.ToInt32(NumericUpDown1.Value)
        End Get
        Set(value As Integer)
            Dim decimalValue = Convert.ToDecimal(value / 1000)
            NumericUpDown1.Value = Math.Max(Math.Min(NumericUpDown1.Maximum, decimalValue), NumericUpDown1.Minimum)
        End Set
    End Property

    Private Sub ButtonOK_Click(sender As Object, e As EventArgs) Handles ButtonOK.Click
        DialogResult = DialogResult.OK
    End Sub

    Private Sub ButtonCancel_Click(sender As Object, e As EventArgs) Handles ButtonCancel.Click
        DialogResult = DialogResult.Cancel
    End Sub
End Class
