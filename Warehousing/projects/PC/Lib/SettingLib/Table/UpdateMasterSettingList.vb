Namespace Table
    Public Class UpdateMasterSettingList

        Private ReadOnly _settingList As List(Of UpdateMasterSetting) = New List(Of UpdateMasterSetting)()
        Public ReadOnly Property SettingList() As List(Of UpdateMasterSetting)
            Get
                Return _settingList
            End Get
        End Property

        Public Function GetUpdateMasterSetting(ByVal jobName As String) As UpdateMasterSetting
            For Each dbUpdateSetting As UpdateMasterSetting In _settingList
                If String.Equals(dbUpdateSetting.JobName, jobName, StringComparison.OrdinalIgnoreCase) Then Return dbUpdateSetting
            Next
            Return Nothing
        End Function

        Public Function GetDbUpdateSettingList(ByVal masterName As String) As List(Of UpdateMasterSetting)
            Dim list As New List(Of UpdateMasterSetting)()
            For Each dbUpdateSetting As UpdateMasterSetting In _settingList
                If String.Equals(dbUpdateSetting.MasterName, masterName, StringComparison.OrdinalIgnoreCase) Then
                    list.Add(dbUpdateSetting)
                End If
            Next
            Return list
        End Function
    End Class
End Namespace
