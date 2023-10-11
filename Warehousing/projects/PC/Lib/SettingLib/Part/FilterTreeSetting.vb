Namespace Part
    Public Class FilterTreeSetting
        Implements IGuiPartSetting

        Public Property Name As String Implements IGuiPartSetting.Name

        Public Property DisplayColumns() As List(Of DisplayColumnSetting)

        Public Property ShowSettingButton() As Boolean

        Public Property DefaultConditionSettings() As New List(Of TreeNodeSetting)

        Public Property SettingSearchConditions() As Boolean
    End Class
End Namespace
