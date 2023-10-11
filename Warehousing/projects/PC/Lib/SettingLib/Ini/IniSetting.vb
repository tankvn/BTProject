Namespace Ini
    Public Interface IIniSetting
        ReadOnly Property SettingName() As String

        Property SheetName() As String

        Property ControlName() As String

        ReadOnly Property Data() As String()
    End Interface
End Namespace
