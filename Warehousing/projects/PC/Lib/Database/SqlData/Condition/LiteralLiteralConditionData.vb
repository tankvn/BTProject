Imports Database.SqlData.Condition.Member
Imports Database.Utility

Namespace SqlData.Condition

    Public Class LiteralLiteralConditionData
        Implements IConditionData

        Private ReadOnly _conditionType As ConditionType

        Private ReadOnly _left As IMember

        Private ReadOnly _right As IMember

        Private ReadOnly _comparisonType As ComparisonType = ComparisonType.Text

        Private Sub New(ByVal conditionType As ConditionType, ByVal left As IMember, ByVal right As IMember, ByVal comparisonType As ComparisonType)
            _conditionType = conditionType
            _left = left

            Debug.Assert(TypeOf right Is LiteralMember, "LiteralConditionData: Type of RightMember is Wrong")
            _right = right
            _comparisonType = comparisonType
        End Sub

        Public Sub New(ByVal conditionType As ConditionType, ByVal left As Object, ByVal right As Object, ByVal dataType As DbType)
            Me.New(conditionType, New LiteralMember(left, dataType), New LiteralMember(right, dataType), DbTypeToComparisonType(dataType))
        End Sub

        Public Sub New(ByVal conditionType As ConditionType, ByVal left As Object, ByVal right As Object,
                       ByVal dataType As DbType, ByVal calculationType As CalculationType, ByVal calculationValue As Integer)
            Me.New(conditionType, New CalculationLiteralMember(calculationType, left, calculationValue, dataType),
                   New LiteralMember(right, dataType), DbTypeToComparisonType(dataType))
        End Sub

        Friend Overridable Function ToSqlAndParameter() As SqlAndParameterList Implements IConditionData.ToSqlAndParameter
            Return GetConditionSqlAndParameterList(_conditionType, _left, _right, _comparisonType)
        End Function
    End Class
End Namespace
