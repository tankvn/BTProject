Namespace SqlData.Condition.Member
    Public Class CalculationLiteralMember
        Implements IMember

        Private ReadOnly _value As Object

        Private ReadOnly _calculationValue As Double

        Private ReadOnly _calculationType As CalculationType

        Public ReadOnly Property SqlString() As String Implements IMember.SqlString
            Get
                Dim operatorString As String = GetOperatorString(_calculationType)

                If String.IsNullOrEmpty(operatorString) Then Return "?"

                Return "?" + operatorString + "?"
            End Get
        End Property

        Private ReadOnly _type As DbType
        Public ReadOnly Property Type() As DbType
            Get
                Return _type
            End Get
        End Property

        Public ReadOnly Property Parameter() As ICollection(Of Parameter) Implements IMember.Parameter
            Get
                Dim list As New List(Of Parameter)()
                list.Add(New Parameter(_value, _type))
                If _calculationType <> CalculationType.None Then
                    list.Add(New Parameter(_calculationValue, DbType.Double))
                End If
                Return list.ToArray()
            End Get
        End Property

        Sub New(ByVal calculationType As CalculationType, ByVal value As Object, ByVal calculationValue As Object, ByVal type As DbType)
            _calculationType = calculationType
            _value = value
            _type = type
            _calculationValue = Convert.ToDouble(calculationValue)
        End Sub

        Sub New(ByVal calculationType As CalculationType, ByVal value As Integer, ByVal calculationValue As Object)
            Me.New(calculationType, value, calculationValue, DbType.Int32)
        End Sub

        Sub New(ByVal calculationType As CalculationType, ByVal value As Double, ByVal calculationValue As Object)
            Me.New(calculationType, value, calculationValue, DbType.Double)
        End Sub

        Sub New(ByVal calculationType As CalculationType, ByVal value As Single, ByVal calculationValue As Object)
            Me.New(calculationType, value, calculationValue, DbType.Single)
        End Sub

        Friend Function ToSqlAndParameter() As SqlAndParameterList Implements IMember.ToSqlAndParameter
            Return New SqlAndParameterList(SqlString, Parameter)
        End Function
    End Class
End Namespace
