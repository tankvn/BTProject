Namespace Controls
    Public Class BtTreeNodeComparer
        Inherits EqualityComparer(Of BtTreeNode)

        Public Overloads Overrides Function Equals(x As BtTreeNode, y As BtTreeNode) As Boolean
            If IsNothing(x) AndAlso IsNothing(y) Then Return True
            If IsNothing(x) OrElse IsNothing(y) Then Return False

            Return x.FieldName = y.FieldName AndAlso
                x.KeyWord = y.KeyWord AndAlso
                x.ConditionType = y.ConditionType
        End Function

        Public Overloads Overrides Function GetHashCode(obj As BtTreeNode) As Integer
            If IsNothing(obj) OrElse IsNothing(obj.KeyWord) Then Return 0
            Return obj.KeyWord.GetHashCode()
        End Function
    End Class
End Namespace
