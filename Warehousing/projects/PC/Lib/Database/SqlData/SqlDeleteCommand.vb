Imports Database.SqlData.Condition
Imports Database.My.Resources
Imports Database.Utility

Namespace SqlData
    Public Class SqlDeleteCommand
        Implements ISqlCommand

        Private ReadOnly _tableName As String
        Public ReadOnly Property TableName() As String Implements ISqlCommand.TableName
            Get
                Return _tableName
            End Get
        End Property

        Private ReadOnly _conditionList As ConditionList
        Public ReadOnly Property ConditionList() As ConditionList Implements ISqlCommand.ConditionList
            Get
                Return _conditionList
            End Get
        End Property

        Sub New(ByVal tableName As String)
            _tableName = EncloseIdentifier(tableName)
            _conditionList = New ConditionList()
        End Sub

        Public Function ToSqlCommand(ByVal connection As DbConnection) As DbCommand Implements ISqlCommand.ToSqlCommand
            Dim command = connection.CreateCommand()

            Dim sqlAndParameterList As SqlAndParameterList = _conditionList.ToWhereSql()
            command.CommandText = String.Format(DeleteSql, _tableName, sqlAndParameterList.SqlString)
            command.Parameters.AddRange(ToDbParameterList(sqlAndParameterList.ParameterList, command).ToArray())
            command.Prepare()
            Return command
        End Function
    End Class
End Namespace
