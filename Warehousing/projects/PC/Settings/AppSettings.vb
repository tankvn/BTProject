%ImportsCodeText%

Public Class AppSettings
    Private Shared _masterSettingList As MasterSettingList
    Public Shared ReadOnly Property MasterSettingList() As MasterSettingList
        Get
            If IsNothing(_masterSettingList) Then
                _masterSettingList = TableSettings.InitializeMasterSettings()
            End If
            Return _masterSettingList
        End Get
    End Property


    Private Shared _dataFormatSettingList As DataFormatSettingList
    Public Shared ReadOnly Property DataFormatSettingList() As DataFormatSettingList
        Get
            If IsNothing(_dataFormatSettingList) Then
                _dataFormatSettingList = TableSettings.InitializeDataFormatSettings()
            End If
            Return _dataFormatSettingList
        End Get
    End Property

    Public Shared ReadOnly Property TableSettingList As List(Of TableSetting)
        Get
            Dim list = New List(Of TableSetting)
            list.AddRange(MasterSettingList.SettingList)
            list.AddRange(DataFormatSettingList.SettingList)
            Return list
        End Get
    End Property


    Private Shared _updateMasterSettingList As UpdateMasterSettingList
    Public Shared ReadOnly Property UpdateMasterSettingList() As UpdateMasterSettingList
        Get
            If IsNothing(_updateMasterSettingList) Then
                _updateMasterSettingList = TableSettings.InitializeUpdateMasterSettings()
            End If
            Return _updateMasterSettingList
        End Get
    End Property

    Private Shared _sheetSettingList As SheetSettingList
    Public Shared ReadOnly Property SheetSettingList() As SheetSettingList
        Get
            If IsNothing(_sheetSettingList) Then
                _sheetSettingList = InitializePageSettings()
            End If
            Return _sheetSettingList
        End Get
    End Property

    Public Shared Sub InitializeAppSettings()
        If AppSetting.Initialized Then Return
        AppSetting.AppName = "%AppName%"
        AppSetting.Separator = "%Separator%"c
        AppSetting.Language = "%Language%"
        AppSetting.AddDoubleQuotes = %AddDoubleQuotes%
        AppSetting.DuplicateDataCheckMode = %DuplicateDataCheckMode%
        AppSetting.Initialized = True
    End Sub

    Private Shared Function InitializePageSettings() As SheetSettingList
%InitializePageSettingsCodeText%
    End Function

End Class
