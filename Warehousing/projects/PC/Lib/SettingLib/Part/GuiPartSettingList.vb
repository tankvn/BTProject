Namespace Part
    Public Class GuiPartSettingList
        Private ReadOnly _settingList As List(Of IGuiPartSetting) = New List(Of IGuiPartSetting)()
        Public ReadOnly Property SettingList() As List(Of IGuiPartSetting)
            Get
                Return _settingList
            End Get
        End Property

        Public Function GetSetting(ByVal name As String) As IGuiPartSetting
            For Each guiPartSetting As IGuiPartSetting In _settingList
                If guiPartSetting.Name.Equals(name) Then Return guiPartSetting
            Next
            Return Nothing
        End Function
    End Class
End Namespace
