Namespace Part
    Public Class ControlPartSettingList
        Private ReadOnly _settingList As List(Of IControlPartSetting) = New List(Of IControlPartSetting)()
        Public ReadOnly Property SettingList() As List(Of IControlPartSetting)
            Get
                Return _settingList
            End Get
        End Property

        Public Function GetSetting(ByVal name As String) As IControlPartSetting
            For Each controlPartSetting As IControlPartSetting In _settingList
                If controlPartSetting.Name.Equals(name) Then Return controlPartSetting
            Next
            Return Nothing
        End Function
    End Class
End Namespace
