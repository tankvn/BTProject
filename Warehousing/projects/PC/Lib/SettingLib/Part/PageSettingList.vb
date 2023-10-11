Namespace Part
    Public Class SheetSettingList
        Private ReadOnly _settingList As List(Of SheetSetting) = New List(Of SheetSetting)()
        Public ReadOnly Property SettingList() As List(Of SheetSetting)
            Get
                Return _settingList
            End Get
        End Property

        Public Function GetSetting(ByVal name As String) As SheetSetting
            For Each pageSetting As SheetSetting In _settingList
                If pageSetting.IdentityName.Equals(name) Then Return pageSetting
            Next
            Return Nothing
        End Function
    End Class
End Namespace
