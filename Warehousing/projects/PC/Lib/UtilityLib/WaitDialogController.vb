Imports System.Windows.Forms

Public Class WaitDialogController
    Private Shared _waitDialog As WaitDialog

    Private Shared _owner As IWin32Window

    Public Shared Sub ShowWaitDialog(ByVal owner As Form)
        ShowWaitDialog(owner, Nothing)
    End Sub

    Public Shared Sub ShowWaitDialog(ByVal owner As Form, ByVal text As String)
        If IsNothing(_waitDialog) Then
            Cursor.Current = Cursors.WaitCursor
            _waitDialog = New WaitDialog
            If Not IsNothing(text) Then
                _waitDialog.SetText(text)
            End If
            _waitDialog.Text = My.Application.Info.AssemblyName
            If Not IsNothing(owner) Then
                _waitDialog.StartPosition = FormStartPosition.Manual
                _waitDialog.Left = owner.Left + (owner.Width - _waitDialog.Width) / 2
                _waitDialog.Top = owner.Top + (owner.Height - _waitDialog.Height) / 2
            End If
            _waitDialog.Show(owner)
            _owner = owner
            _waitDialog.Update()
        End If
    End Sub

    Public Shared Sub CloseWaitDialog(ByVal owner As Form)
        If Not IsNothing(_waitDialog) AndAlso _owner.Equals(owner) Then
            _waitDialog.Close()
            _waitDialog.Dispose()
            _waitDialog = Nothing
        End If
        Cursor.Current = Cursors.Default
    End Sub
End Class
