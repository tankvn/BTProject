Imports Database.SqlData.Condition
Imports SettingLib
Imports SettingLib.Table
Imports Database.SqlData

Friend Class TableSettings
    Friend Shared Function InitializeMasterSettings() As MasterSettingList
%InitializeMasterSettingsCodeText%
    End Function

    Public Shared Function InitializeDataFormatSettings() As DataFormatSettingList
%InitializeDataFormatSettingsCodeText%
    End Function

    Public Shared Function InitializeUpdateMasterSettings() As UpdateMasterSettingList
%InitializeUpdateMasterSettingsCodeText%
    End Function
End Class
