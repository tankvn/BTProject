Namespace SqlData.Condition
    Public Class SqlConditionData
        Implements IConditionData

        Private ReadOnly _sqlString As String

        Sub New(ByVal sqlString As String)
            _sqlString = sqlString
        End Sub

        Public Function ToSqlAndParameter() As SqlAndParameterList Implements IConditionData.ToSqlAndParameter
            Return New SqlAndParameterList("(" + _sqlString + ")", New Parameter() {})
        End Function
    End Class
End Namespace
