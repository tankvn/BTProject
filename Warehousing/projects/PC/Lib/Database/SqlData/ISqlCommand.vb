Imports Database.SqlData.Condition

Namespace SqlData
    Public Interface ISqlCommand
        ReadOnly Property TableName() As String

        ReadOnly Property ConditionList() As ConditionList

        Function ToSqlCommand(ByVal connection As DbConnection) As DbCommand
    End Interface
End Namespace
