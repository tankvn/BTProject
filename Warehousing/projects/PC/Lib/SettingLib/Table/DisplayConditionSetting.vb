Imports Database.SqlData.Condition
Imports Database.SqlData.DisplayData
Imports Database.SqlData
Imports Database.SqlData.Condition.Member

Namespace Table
    Public Class DisplayConditionSetting

        Public Property ComparisonData As String = String.Empty

        Public Property ComparisonMemberType() As MemberType = MemberType.Field

        Public Property InputData() As Object = String.Empty

        Public Property ValueType() As DataType = DataType.IntegerNumber

        Public Property InputMemberType() As MemberType = MemberType.Field

        Public Property ConditionType() As ConditionType = ConditionType.Equal

        Public Property CalculationType() As CalculationType = CalculationType.None

        Public Property CalculationValue() As Double = 0

        Public Property CalculationUnit () As DateUnit = DateUnit.Day

    End Class
End Namespace
