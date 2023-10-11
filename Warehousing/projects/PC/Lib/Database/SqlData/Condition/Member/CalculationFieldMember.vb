Imports Database.SqlData.DisplayData
Imports Database.Utility

Namespace SqlData.Condition.Member
    Public Class CalculationFieldMember
        Implements IMember

        Private ReadOnly _field As IMember

        Private ReadOnly _calculationValue As Double

        Private ReadOnly _calculationType As CalculationType

        Public ReadOnly Property SqlString() As String Implements IMember.SqlString
            Get
                Dim operatorString As String = GetOperatorString(_calculationType)

                If String.IsNullOrEmpty(operatorString) Then Return _field.SqlString

                Return _field.SqlString + operatorString + "?"
            End Get
        End Property

        Public ReadOnly Property Parameter() As ICollection(Of Parameter) Implements IMember.Parameter
            Get
                Dim valueParameter As New Parameter(_calculationValue, DbType.Double)
                Dim list As New List(Of Parameter)(_field.Parameter)
                If _calculationType <> CalculationType.None Then
                    list.Add(valueParameter)
                End If
                Return list
            End Get
        End Property

        Sub New(ByVal calculationType As CalculationType, ByVal fieldName As String, ByVal calculationValue As Object)
            _calculationType = calculationType
            _field = New FieldMember(EncloseIdentifier(fieldName))
            _calculationValue = Convert.ToDouble(calculationValue)
        End Sub

        Sub New(ByVal calculationType As CalculationType, ByVal field As IDisplayData, ByVal calculationValue As Object)
            _calculationType = calculationType
            _field = New DisplayDataMember(field)
            _calculationValue = Convert.ToDouble(calculationValue)
        End Sub

        Friend Function ToSqlAndParameter() As SqlAndParameterList Implements IMember.ToSqlAndParameter
            Return New SqlAndParameterList(SqlString, Parameter)
        End Function
    End Class
End Namespace
