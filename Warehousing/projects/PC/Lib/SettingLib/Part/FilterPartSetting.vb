Imports SettingLib.My.Resources

Namespace Part
    Public Class FilterPartSetting
        Implements IGuiPartSetting

        Public Shared ReadOnly Property FieldAllText() As String
            Get
                Return FieldAll
            End Get
        End Property

        Public Property Name As String Implements IGuiPartSetting.Name

        Public Property ShowColumnSettingBox() As Boolean

        Public Property ShowConditionSettingBox() As Boolean

        Public Property SearchTarget() As SearchTarget = SearchTarget.All

        Public Property SearchCondition() As SearchConditionType = SearchConditionType.Include

        Public Property DisplayColumnList() As List(Of DisplayColumnSetting)
    End Class
End Namespace
