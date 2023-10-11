Imports Database.SqlData.Condition.Member
Imports Database.SqlData.DisplayData

Namespace SqlData.Condition
    Public Class FieldConditionData
        Implements IConditionData

        Private ReadOnly _type As ConditionType

        Private ReadOnly _left As IMember

        Private ReadOnly _right As IMember

        Private ReadOnly _comparisonType As ComparisonType = ComparisonType.Text

        Public Sub New(ByVal type As ConditionType, ByVal left As String, ByVal right As String, ByVal comparisonType As ComparisonType)
            _type = type
            _left = New FieldMember(left)
            _right = New FieldMember(right)
            _comparisonType = comparisonType
        End Sub

        Public Sub New(ByVal type As ConditionType, ByVal left As IDisplayData, ByVal right As String, ByVal comparisonType As ComparisonType)
            _type = type
            _left = New DisplayDataMember(left)
            _right = New FieldMember(right)
            _comparisonType = comparisonType
        End Sub

        Public Sub New(ByVal type As ConditionType, ByVal left As String, ByVal right As IDisplayData, ByVal comparisonType As ComparisonType)
            _type = type
            _left = New FieldMember(left)
            _right = New DisplayDataMember(right)
            _comparisonType = comparisonType
        End Sub

        Public Sub New(ByVal type As ConditionType, ByVal left As IDisplayData, ByVal right As IDisplayData, ByVal comparisonType As ComparisonType)
            _type = type
            _left = New DisplayDataMember(left)
            _right = New DisplayDataMember(right)
            _comparisonType = comparisonType
        End Sub

        Public Sub New(ByVal type As ConditionType, ByVal left As String, ByVal calculationType As CalculationType,
                       ByVal right As String, ByVal calculationValue As Integer)
            _type = type
            _left = New CalculationFieldMember(calculationType, left, calculationValue)
            _right = New FieldMember(right)
            _comparisonType = ComparisonType.Numeric
        End Sub


        Public Sub New(ByVal type As ConditionType, ByVal left As String,
                       ByVal calculationType As CalculationType, ByVal right As String,
                       ByVal calculationValue As Single)
            _type = type
            _left = New CalculationFieldMember(calculationType, left, calculationValue)
            _right = New FieldMember(right)
            _comparisonType = ComparisonType.Numeric
        End Sub


        Public Sub New(ByVal type As ConditionType, ByVal left As String,
                       ByVal calculationType As CalculationType, ByVal right As String,
                       ByVal calculationValue As Double)
            _type = type
            _left = New CalculationFieldMember(calculationType, left, calculationValue)
            _right = New FieldMember(right)
            _comparisonType = ComparisonType.Numeric
        End Sub

        Public Sub New(ByVal type As ConditionType, ByVal left As IDisplayData,
                       ByVal calculationType As CalculationType, ByVal right As IDisplayData,
                       ByVal calculationValue As Integer)
            _type = type
            _left = New CalculationFieldMember(calculationType, left, calculationValue)
            _right = New DisplayDataMember(right)
            _comparisonType = ComparisonType.Numeric
        End Sub


        Public Sub New(ByVal type As ConditionType, ByVal left As IDisplayData,
                       ByVal calculationType As CalculationType, ByVal right As IDisplayData,
                       ByVal calculationValue As Single)
            _type = type
            _left = New CalculationFieldMember(calculationType, left, calculationValue)
            _right = New DisplayDataMember(right)
            _comparisonType = ComparisonType.Numeric
        End Sub


        Public Sub New(ByVal type As ConditionType, ByVal left As IDisplayData,
                       ByVal calculationType As CalculationType, ByVal right As IDisplayData,
                       ByVal calculationValue As Double)
            _type = type
            _left = New CalculationFieldMember(calculationType, left, calculationValue)
            _right = New DisplayDataMember(right)
            _comparisonType = ComparisonType.Numeric
        End Sub

        Friend Overridable Function ToSqlAndParameter() As SqlAndParameterList Implements IConditionData.ToSqlAndParameter
            Return GetConditionSqlAndParameterList(_type, _left, _right, _comparisonType)
        End Function
    End Class
End Namespace
