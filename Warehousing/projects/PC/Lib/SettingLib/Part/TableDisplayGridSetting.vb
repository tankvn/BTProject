Namespace Part
    Public Class TableDisplayGridSetting
        Implements IGuiPartSetting

        Public Const STATUS_COLUMN_NAME = "Result"

        Public Property Name() As String Implements IGuiPartSetting.Name

        Public Sub New()
            Name = String.Empty
        End Sub
    End Class
End Namespace
