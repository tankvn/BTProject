Imports Database.SqlData.Condition
Imports Database.SqlData
Imports Database.SqlData.DisplayData
Imports Database.SqlData.Condition.Member

Namespace Part
    Public Class FilterSetting
        Public Property ConditionType() As ConditionType

        Public Property SourceName() As String

        Public Property SourceMemberType() As MemberType

        Public Property ComparisonValue() As Object

        Public Property ComparisonValueType As DataType

        Public Property IsDate() As Boolean

        Public Property ComparisonMemberType() As MemberType

        Public Property CalculationType() As CalculationType

        Public Property CalculationValue() As Integer

        Public Property CalculationDateUnit() As DateUnit
    End Class
End Namespace
