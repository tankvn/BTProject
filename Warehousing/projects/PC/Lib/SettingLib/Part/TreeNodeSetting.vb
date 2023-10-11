Namespace Part
    Public Class TreeNodeSetting
        Public Property Field() As String

        Public Property Keyword() As String

        Public Property DisplayText() As String
            Get
                Select Case ConditionType
                    Case SearchConditionType.Match
                        Return Keyword
                    Case SearchConditionType.FrontMatch
                        Return Keyword + " *"
                    Case SearchConditionType.BackMatch
                        Return "* " + Keyword
                    Case SearchConditionType.Include
                        Return "* " + Keyword + " *"
                    Case SearchConditionType.NotInclude
                        Return "(NOT) " + Keyword
                End Select
                Return Keyword
            End Get
            Set(value As String)
                Keyword = value
            End Set
        End Property

        Public Property ConditionType() As SearchConditionType

        Public Sub New(ByVal searchKeyword As String, ByVal fieldName As String, ByVal condition As SearchConditionType)
            Keyword = searchKeyword
            Field = fieldName
            ConditionType = condition
        End Sub

        Public Sub New(ByVal nodeSetting As TreeNodeSetting)
            Keyword = nodeSetting.Keyword
            Field = nodeSetting.Field
            ConditionType = nodeSetting.ConditionType
        End Sub
    End Class
End Namespace
