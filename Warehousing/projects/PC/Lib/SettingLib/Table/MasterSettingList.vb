Namespace Table

    Public Class MasterSettingList
        Private ReadOnly _settingList As List(Of MasterSetting)

        Public ReadOnly Property SettingList() As List(Of MasterSetting)
            Get
                Return _settingList
            End Get
        End Property

        Public Sub New()
            _settingList = New List(Of MasterSetting)()
        End Sub

        Public Function GetMasterSetting(ByVal name As String) As MasterSetting
            For Each masterDataSetting As MasterSetting In _settingList
                If String.Equals(masterDataSetting.Name, name, StringComparison.OrdinalIgnoreCase) Then Return masterDataSetting
            Next
            Return Nothing
        End Function

        Public Function GetMasterSettingByTableName(ByVal tableName As String) As MasterSetting
            For Each masterDataSetting As MasterSetting In _settingList
                If String.Equals(masterDataSetting.TableName, tableName, StringComparison.OrdinalIgnoreCase) Then Return masterDataSetting
            Next
            Return Nothing
        End Function

        Public Function GetNames() As String()
            Dim nameList As New List(Of String)()
            For Each masterSetting As MasterSetting In _settingList
                nameList.Add(masterSetting.Name)
            Next
            Return nameList.ToArray()
        End Function
    End Class
End Namespace
