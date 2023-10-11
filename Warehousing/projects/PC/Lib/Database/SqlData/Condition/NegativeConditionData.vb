Imports Database.My.Resources

Namespace SqlData.Condition
    Public Class NegativeConditionData
        Implements IConditionData

        Private ReadOnly _conditionData As IConditionData

        Sub New(ByVal conditionData As IConditionData)
            _conditionData = conditionData
        End Sub

        Public Function ToSqlAndParameter() As SqlAndParameterList Implements IConditionData.ToSqlAndParameter
            Dim sqlAndParameters As SqlAndParameterList = _conditionData.ToSqlAndParameter()
            Return _
                New SqlAndParameterList(String.Format(NotSql, sqlAndParameters.SqlString),
                                        sqlAndParameters.ParameterList)
        End Function
    End Class
End Namespace
