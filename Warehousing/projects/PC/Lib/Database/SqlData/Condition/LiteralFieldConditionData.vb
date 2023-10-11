Imports Database.SqlData.Condition.Member
Imports Database.SqlData.DisplayData
Imports Database.Utility

Namespace SqlData.Condition
    Public Class LiteralFieldConditionData
        Implements IConditionData

        Private ReadOnly _conditionType As ConditionType
        Protected ReadOnly Property ConditionType() As ConditionType
            Get
                Return _conditionType
            End Get
        End Property

        Private ReadOnly _left As IMember

        Private ReadOnly _right As IMember

        Private ReadOnly _comparisonType As ComparisonType = ComparisonType.Text

        Private Sub New(ByVal conditionType As ConditionType, ByVal left As IMember,
                        ByVal right As IMember, ByVal comparisonType As ComparisonType)
            _conditionType = conditionType
            _left = left

            Debug.Assert(TypeOf right Is FieldMember, "LiteralFieldConditionData: Type of RightMember is Wrong")
            _right = right
            _comparisonType = comparisonType
        End Sub

        Public Sub New(ByVal conditionType As ConditionType, ByVal left As Object, ByVal leftType As DbType,
                       right As Object, rightType As DbType, ByVal comparisonType As ComparisonType)
            Me.New(conditionType, New LiteralMember(left, leftType), New LiteralMember(right, rightType), comparisonType)
        End Sub

        Public Sub New(ByVal conditionType As ConditionType, ByVal left As Object, ByVal leftType As DbType,
                       rightDisplayData As IDisplayData, ByVal comparisonType As ComparisonType)
            Me.New(conditionType, New LiteralMember(left, leftType), New DisplayDataMember(rightDisplayData), comparisonType)
        End Sub

        Public Sub New(ByVal type As ConditionType, ByVal left As Object, ByVal calculationType As CalculationType,
               ByVal right As IDisplayData, ByVal rightCalculationValue As Integer, ByVal dbType As DbType)
            Me.New(type, New CalculationLiteralMember(calculationType, left, rightCalculationValue, dbType), New DisplayDataMember(right), DbTypeToComparisonType(dbType))
        End Sub

        Friend Overridable Function ToSqlAndParameter() As SqlAndParameterList Implements IConditionData.ToSqlAndParameter
            Return GetConditionSqlAndParameterList(_conditionType, _left, _right, _comparisonType)
        End Function
    End Class
End Namespace
