Namespace SqlData.Condition.Member
    Public MustInherit Class DateTimeMember
        Implements IMember


        Private ReadOnly _calculationType As CalculationType = CalculationType.None
        Public ReadOnly Property CalculationType() As CalculationType
            Get
                Return _calculationType
            End Get
        End Property


        Private ReadOnly _calculationValue As Integer
        Public ReadOnly Property CalculationValue() As Integer
            Get
                Return _calculationValue
            End Get
        End Property

        Private ReadOnly _calculationUnit As DateUnit
        Public ReadOnly Property CalculationUnit() As DateUnit
            Get
                Return _calculationUnit
            End Get
        End Property

        Sub New(ByVal calculationType As CalculationType, ByVal calculationValue As Integer, ByVal calculationUnit As DateUnit)
            _calculationType = calculationType
            _calculationValue = calculationValue
            _calculationUnit = calculationUnit
        End Sub


        Public MustOverride ReadOnly Property Parameter As ICollection(Of Parameter) Implements IMember.Parameter

        Public MustOverride ReadOnly Property SqlString As String Implements IMember.SqlString

        Friend MustOverride Function ToSqlAndParameter() As SqlAndParameterList Implements IMember.ToSqlAndParameter
    End Class
End Namespace
