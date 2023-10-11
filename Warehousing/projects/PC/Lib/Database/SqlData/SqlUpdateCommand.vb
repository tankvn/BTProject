Imports Database.SqlData.Condition
Imports Database.My.Resources
Imports Database.Utility

Namespace SqlData
    Public NotInheritable Class SqlUpdateCommand
        Implements ISqlCommand

        Private ReadOnly _updateDataList As UpdateDataList
        Public ReadOnly Property UpdateDataList() As UpdateDataList
            Get
                Return _updateDataList
            End Get
        End Property

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

        Friend Function ToSqlCommand(ByVal connection As DbConnection) As DbCommand _
            Implements ISqlCommand.ToSqlCommand
            Dim command As DbCommand = connection.CreateCommand()

            Dim updateDataSqlAndParameters = _updateDataList.ToSqlAndParameter()
            command.Parameters.AddRange(ToDbParameterList(updateDataSqlAndParameters.ParameterList, command).ToArray())

            Dim conditionSqlAndParameters = _conditionList.ToWhereSql()
            command.Parameters.AddRange(ToDbParameterList(conditionSqlAndParameters.ParameterList, command).ToArray())

            command.CommandText = String.Format(UpdateSql, _tableName, updateDataSqlAndParameters.SqlString,
                                                conditionSqlAndParameters.SqlString)
            command.Prepare()
            Return command
        End Function

        Sub New(ByVal tableName As String)
            _tableName = EncloseIdentifier(tableName)
            _conditionList = New ConditionList()
            _updateDataList = New UpdateDataList()
        End Sub

        Sub New(ByVal tableName As String, ByVal dbName As String)
            _tableName = EncloseIdentifier(dbName) + "." + EncloseIdentifier(tableName)
            _conditionList = New ConditionList()
            _updateDataList = New UpdateDataList()
        End Sub
    End Class
End Namespace
