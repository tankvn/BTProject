
Namespace Ini
    Public Class SearchHistoryData
        Implements IIniSetting

        Public ReadOnly Property SettingName() As String Implements IIniSetting.SettingName
            Get
                Return Me.GetType().Name
            End Get
        End Property

        Public Property SheetName() As String Implements IIniSetting.SheetName

        Public Property ControlName() As String Implements IIniSetting.ControlName

        Public ReadOnly Property Data() As String() Implements IIniSetting.Data
            Get
                Return New String() {HistoryString}
            End Get
        End Property

        Public Property HistoryString() As String

        Public Sub New(ByVal inputJobName As String, ByVal inputControlName As String, ByVal inputHistoryString As String)
            SheetName = inputJobName
            ControlName = inputControlName
            HistoryString = inputHistoryString
        End Sub
    End Class
End NameSpace
