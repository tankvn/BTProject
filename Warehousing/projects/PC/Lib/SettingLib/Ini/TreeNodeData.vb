Imports Database.SqlData.Condition

Namespace Ini
    Public Class TreeNodeData
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
                Return New String() {ColumnName, Keyword, ConditionType.ToString()}
            End Get
        End Property

        Public Property ColumnName() As String

        Public Property Keyword() As String

        Public Property ConditionType() As ConditionType

        Sub New(ByVal sheet As String, ByVal partName As String, ByVal fieldName As String,
                ByVal value As String, ByVal type As ConditionType)
            SheetName = sheet
            ControlName = partName
            ColumnName = fieldName
            Keyword = value
            ConditionType = type
        End Sub
    End Class
End Namespace
