

Namespace Part
    Public Class RowPreviewSetting
        Implements IGuiPartSetting
        Public Property Name() As String Implements IGuiPartSetting.Name

        Public Property ItemSettings() As List(Of RowPreviewItemSetting)

        Public Property RowPreviewSeparator As RowPreviewSeparator

        Public Sub New()
            Name = String.Empty
            ItemSettings = New List(Of RowPreviewItemSetting)()
            RowPreviewSeparator = RowPreviewSeparator.Cologne
        End Sub
    End Class

End Namespace
