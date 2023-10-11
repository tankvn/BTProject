Namespace Part
    Public Class ClearButtonSetting
        Implements IGuiPartSetting

        Public Property Name As String Implements IGuiPartSetting.Name

        Public Property TargetColumns() As String()

        Public Property DeleteAllRows() As Boolean
    End Class
End Namespace
