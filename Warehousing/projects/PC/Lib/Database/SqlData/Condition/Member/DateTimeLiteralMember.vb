Imports Database.My.Resources
Imports System.Globalization

Namespace SqlData.Condition.Member
    Public Class DateTimeLiteralMember
        Inherits DateTimeMember

        Private ReadOnly _value As Object

        Public Overrides ReadOnly Property SqlString() As String
            Get
                Dim signString As String = GetOperatorString(CalculationType)
                Dim unitString = GetDateUnitString(CalculationUnit)
                Dim modifierString = signString + CalculationValue.ToString(CultureInfo.InvariantCulture) + " " + unitString
                Return String.Format(DateTimeCalculationSql, "?", modifierString)
            End Get
        End Property

        Private ReadOnly _type As DbType

        Public Overrides ReadOnly Property Parameter() As ICollection(Of Parameter)
            Get
                Dim newParameter As New Parameter(_value, _type)
                Return New Parameter() {newParameter}
            End Get
        End Property

        Sub New(ByVal value As String)
            Me.New(value, CalculationType.None, 0, DateUnit.Year)
        End Sub

        Sub New(ByVal value As String, ByVal calculationType As CalculationType, ByVal calculationValue As Integer, ByVal calculationUnit As DateUnit)
            MyBase.New(calculationType, calculationValue, calculationUnit)
            _value = value
            _type = DbType.DateTime
        End Sub

        Friend Overrides Function ToSqlAndParameter() As SqlAndParameterList
            Return New SqlAndParameterList(SqlString, Parameter)
        End Function
    End Class
End Namespace
