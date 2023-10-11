Namespace Table
    Public Class DataFormatSettingList
        Private ReadOnly _settingList As List(Of DataFormatSetting) = New List(Of DataFormatSetting)()
        Public ReadOnly Property SettingList() As List(Of DataFormatSetting)
            Get
                Return _settingList
            End Get
        End Property

        Public Function GetByJobName(ByVal jobName As String) As DataFormatSetting
            For Each dataFormatSetting As DataFormatSetting In _settingList
                If String.Equals(dataFormatSetting.JobName, jobName, StringComparison.OrdinalIgnoreCase) Then Return dataFormatSetting
            Next
            Return Nothing
        End Function

        Public Function GetByLogFileName(ByVal logFileName As String) As DataFormatSetting
            For Each dataFormatSetting As DataFormatSetting In _settingList
                If String.Equals(dataFormatSetting.LogFileName, logFileName, StringComparison.OrdinalIgnoreCase) Then Return dataFormatSetting
            Next
            Return Nothing
        End Function


        Public Function GetByTableName(ByVal tableName As String) As DataFormatSetting
            For Each dataFormatSetting As DataFormatSetting In _settingList
                If String.Equals(dataFormatSetting.TableName, tableName, StringComparison.OrdinalIgnoreCase) Then Return dataFormatSetting
            Next
            Return Nothing
        End Function
    End Class
End Namespace
