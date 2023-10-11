Imports System.Windows.Forms
Imports UtilityLib.My.Resources

Public Class PasswordForm

    Private ReadOnly _planeText As String

    Public Sub New(ByVal password() As Byte)
        InitializeComponent()
        _planeText = BasicUtility.DecodePassword(password)
    End Sub

    Private Function AuthenticatePassword() As Boolean
        Return _planeText = TextBoxPassword.Text
    End Function

    Private Sub PasswordForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If (DialogResult <> DialogResult.OK) Then Return

        If Not AuthenticatePassword() Then
            MessageBox.Show(Me, ErrorMessagePasswordMismatch, My.Application.Info.AssemblyName, MessageBoxButtons.OK, MessageBoxIcon.Error)
            e.Cancel = True
        End If
    End Sub

    Private Sub ButtonOK_Click(sender As Object, e As EventArgs) Handles ButtonOK.Click
        DialogResult = DialogResult.OK
    End Sub
End Class
