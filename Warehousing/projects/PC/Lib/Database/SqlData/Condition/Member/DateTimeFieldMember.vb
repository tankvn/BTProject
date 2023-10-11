Imports Database.My.Resources
Imports Database.Utility
Imports System.Globalization

Namespace SqlData.Condition.Member
    Public Class DateTimeFieldMember
        Inherits DateTimeMember

        Private ReadOnly _fieldName As String

        Public Overrides ReadOnly Property SqlString() As String
            Get
                Dim signString As String = GetOperatorString(CalculationType)
                Dim unitString = GetDateUnitString(CalculationUnit)
                Dim modifierString = signString + CalculationValue.ToString(CultureInfo.InvariantCulture) + " " + unitString
                Return String.Format(DateTimeCalculationSql, _fieldName, modifierString)
            End Get
        End Property

        Public Overrides ReadOnly Property Parameter() As ICollection(Of Parameter)
            Get
                Return New Parameter() {}
            End Get
        End Property

        Sub New(ByVal fieldName As String, ByVal calculationType As CalculationType, ByVal calculationValue As Integer, ByVal calculationUnit As DateUnit)
            MyBase.New(calculationType, calculationValue, calculationUnit)
            _fieldName = EncloseIdentifier(fieldName)
        End Sub

        Sub New(ByVal fieldName As String)
            Me.New(fieldName, CalculationType.None, 0, DateUnit.Year)
        End Sub

        Friend Overrides Function ToSqlAndParameter() As SqlAndParameterList
            Return New SqlAndParameterList(SqlString, New Parameter() {})
        End Function

    End Class
End Namespace
