Imports Database.SqlData.Condition.Member
Imports Database.SqlData.DisplayData
Imports Database.Utility

Namespace SqlData.Condition
    Public Class LiteralConditionData
        Implements IConditionData

        Private ReadOnly _conditionType As ConditionType
        Protected ReadOnly Property ConditionType() As ConditionType
            Get
                Return _conditionType
            End Get
        End Property

        Private ReadOnly _left As IMember

        Private ReadOnly _right As IMember
        Friend ReadOnly Property Right() As IMember
            Get
                Return _right
            End Get
        End Property

        Private ReadOnly _comparisonType As ComparisonType = ComparisonType.Text

        Private Sub New(ByVal conditionType As ConditionType, ByVal left As IMember, ByVal right As IMember, ByVal comparisonType As ComparisonType)
            _conditionType = conditionType
            _left = left
            _comparisonType = comparisonType
            Debug.Assert(TypeOf right Is LiteralMember, "LiteralConditionData: Type of RightMember is Wrong")
            _right = right
        End Sub

        Public Sub New(ByVal conditionType As ConditionType, ByVal left As String, ByVal right As String)
            Me.New(conditionType, New FieldMember(left), New LiteralMember(right), ComparisonType.Text)
        End Sub

        Public Sub New(ByVal conditionType As ConditionType, ByVal left As String, ByVal right As Integer)
            Me.New(conditionType, New FieldMember(left), New LiteralMember(right), ComparisonType.Numeric)
        End Sub

        Public Sub New(ByVal conditionType As ConditionType, ByVal left As String, ByVal calculationType As CalculationType,
                       ByVal right As Integer, ByVal calculationValue As Integer)
            Me.New(conditionType, New CalculationFieldMember(calculationType, left, calculationValue),
                   New LiteralMember(right), ComparisonType.Numeric)
        End Sub

        Public Sub New(ByVal conditionType As ConditionType, ByVal left As String, ByVal calculationType As CalculationType,
                       ByVal right As Integer, ByVal calculationValue As Double)
            Me.New(conditionType, New CalculationFieldMember(calculationType, left, calculationValue),
                   New LiteralMember(right), ComparisonType.Numeric)
        End Sub

        Public Sub New(ByVal conditionType As ConditionType, ByVal left As String, ByVal right As Double)
            Me.New(conditionType, New FieldMember(left), New LiteralMember(right), ComparisonType.Numeric)
        End Sub

        Public Sub New(ByVal conditionType As ConditionType, ByVal left As String, ByVal calculationType As CalculationType,
                       ByVal right As Double, ByVal calculationValue As Integer)
            Me.New(conditionType, New CalculationFieldMember(calculationType, left, calculationValue),
                   New LiteralMember(right), ComparisonType.Numeric)
        End Sub

        Public Sub New(ByVal conditionType As ConditionType, ByVal left As String, ByVal calculationType As CalculationType,
                       ByVal right As Double, ByVal calculationValue As Double)
            Me.New(conditionType, New CalculationFieldMember(calculationType, left, calculationValue),
                   New LiteralMember(right), ComparisonType.Numeric)
        End Sub

        Public Sub New(ByVal conditionType As ConditionType, ByVal left As String, ByVal right As Object, ByVal dbType As DbType)
            Me.New(conditionType, New FieldMember(left), New LiteralMember(right, dbType), DbTypeToComparisonType(dbType))
        End Sub

        Public Sub New(ByVal conditionType As ConditionType, ByVal left As IDisplayData, ByVal right As String)
            Me.New(conditionType, New DisplayDataMember(left), New LiteralMember(right), ComparisonType.Text)
        End Sub

        Public Sub New(ByVal conditionType As ConditionType, ByVal left As IDisplayData, ByVal right As Integer)
            Me.New(conditionType, New DisplayDataMember(left), New LiteralMember(right), ComparisonType.Numeric)
        End Sub

        Public Sub New(ByVal conditionType As ConditionType, ByVal left As IDisplayData, ByVal calculationType As CalculationType,
                       ByVal right As Integer, ByVal calculationValue As Integer)
            Me.New(conditionType, New CalculationFieldMember(calculationType, left, calculationValue),
                   New LiteralMember(right), ComparisonType.Numeric)
        End Sub

        Public Sub New(ByVal conditionType As ConditionType, ByVal left As IDisplayData, ByVal calculationType As CalculationType,
                       ByVal right As Object, ByVal type As DbType, ByVal calculationValue As Integer)
            Me.New(conditionType, New CalculationFieldMember(calculationType, left, calculationValue),
                   New LiteralMember(right, type), DbTypeToComparisonType(type))
        End Sub

        Public Sub New(ByVal conditionType As ConditionType, ByVal left As IDisplayData, ByVal calculationType As CalculationType,
                       ByVal right As Integer, ByVal calculationValue As Double)
            Me.New(conditionType, New CalculationFieldMember(calculationType, left, calculationValue),
                   New LiteralMember(right), ComparisonType.Numeric)
        End Sub

        Public Sub New(ByVal conditionType As ConditionType, ByVal left As IDisplayData, ByVal right As Double)
            Me.New(conditionType, New DisplayDataMember(left), New LiteralMember(right), ComparisonType.Numeric)
        End Sub

        Public Sub New(ByVal conditionType As ConditionType, ByVal left As IDisplayData, ByVal calculationType As CalculationType,
                       ByVal right As Double, ByVal calculationValue As Integer)
            Me.New(conditionType, New CalculationFieldMember(calculationType, left, calculationValue),
                   New LiteralMember(right), ComparisonType.Numeric)
        End Sub

        Public Sub New(ByVal conditionType As ConditionType, ByVal left As IDisplayData, ByVal calculationType As CalculationType,
                       ByVal right As Double, ByVal calculationValue As Double)
            Me.New(conditionType, New CalculationFieldMember(calculationType, left, calculationValue),
                   New LiteralMember(right), ComparisonType.Numeric)
        End Sub

        Public Sub New(ByVal conditionType As ConditionType, ByVal left As IDisplayData, ByVal right As Object,
                       ByVal dbType As DbType)
            Me.New(conditionType, New DisplayDataMember(left), New LiteralMember(right, dbType), DbTypeToComparisonType(dbType))
        End Sub

        Friend Overridable Function ToSqlAndParameter() As SqlAndParameterList Implements IConditionData.ToSqlAndParameter
            Return GetConditionSqlAndParameterList(_conditionType, _left, _right, _comparisonType)
        End Function
    End Class
End Namespace
