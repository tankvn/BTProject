Namespace Part
    Public Class BtButtonSetting
        Implements IGuiPartSetting

        Public Property Name() As String Implements IGuiPartSetting.Name

        Public Property ShowConfirmMessage() As Boolean

        Public Property Message() As String

        Sub New()
            Name = String.Empty
            ShowConfirmMessage = True
            Message = String.Empty
        End Sub

    End Class
End Namespace
