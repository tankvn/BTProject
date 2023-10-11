Imports Database.SqlData.Condition
Imports Database.SqlData.Condition.Member

Namespace SqlData
    Friend Class CalculationData
        Private ReadOnly _type As CalculationType

        Private ReadOnly _left As IMember

        Private ReadOnly _right As IMember

        Public Sub New(ByVal type As CalculationType, ByVal left As IMember, ByVal right As IMember)
            _type = type
            _left = left
            _right = right
        End Sub

        Friend Function ToSqlAndParameters() As SqlAndParameterList
            If _type = CalculationType.None Then Return _left
            Dim parameterList As New List(Of Parameter)(_left.Parameter)
            parameterList.Add(_right.Parameter)
            Return New SqlAndParameterList(_left.SqlString + GetOperatorString(_type) + _right.SqlString, parameterList)
        End Function
    End Class
End Namespace
