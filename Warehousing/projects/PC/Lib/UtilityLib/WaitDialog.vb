
Public Class WaitDialog
    Friend Sub SetText(ByVal displayText As String)
        If IsNothing(displayText) Then
            displayText = String.Empty
        End If
        _label.Text = displayText
    End Sub
End Class
