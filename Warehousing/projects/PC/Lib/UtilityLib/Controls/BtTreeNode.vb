Imports System.Windows.Forms
Imports Database.SqlData.Condition

Namespace Controls

    Public Class BtTreeNode
        Inherits TreeNode
        Implements IEquatable(Of BtTreeNode)

        Public Property FieldName() As String = String.Empty

        Private _keyword As String
        Public Property KeyWord() As String
            Get
                Return _keyword
            End Get
            Set(value As String)
                _keyword = value
                UpdateText()
            End Set
        End Property

        Private _conditionType As ConditionType
        Public Property ConditionType() As ConditionType
            Get
                Return _conditionType
            End Get
            Set(value As ConditionType)
                _conditionType = value
                UpdateText()
            End Set
        End Property

        Public Sub New(ByVal keywordString As String, ByVal field As String, ByVal condition As ConditionType)
            FieldName = field
            ConditionType = condition
            KeyWord = keywordString
        End Sub

        Public Sub New()
        End Sub

        Private Sub UpdateText()
            Select Case ConditionType
                Case ConditionType.ExactMatch
                    Text = KeyWord
                Case ConditionType.ForwardMatch
                    Text = KeyWord + " *"
                Case ConditionType.BackwardMatch
                    Text = "* " + KeyWord
                Case ConditionType.Include
                    Text = "* " + KeyWord + " *"
                Case ConditionType.NotInclude
                    Text = "(NOT) " + KeyWord
            End Select
        End Sub

        Public Overloads Function Equals(ByVal other As BtTreeNode) As Boolean Implements IEquatable(Of BtTreeNode).Equals
            Return FieldName = other.FieldName AndAlso
                KeyWord = other.KeyWord AndAlso
                ConditionType = other.ConditionType
        End Function

        Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
            If (obj Is Nothing) OrElse Not (Me.GetType() Is obj.GetType()) Then
                Return False
            End If

            Return Equals(CType(obj, BtTreeNode))
        End Function
    End Class
End Namespace
