Imports Database.SqlData.Condition
Imports Database.My.Resources
Imports Database.Utility

Namespace SqlData
    Public Class SqlInsertCommand
        Implements ISqlCommand

        Private ReadOnly _insertDataList As InsertDataList
        Public ReadOnly Property InsertDataList() As InsertDataList
            Get
                Return _insertDataList
            End Get
        End Property

        Private ReadOnly _insertFieldList As List(Of String)
        Public ReadOnly Property InsertFieldList() As List(Of String)
            Get
                Return _insertFieldList
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

            Dim command = connection.CreateCommand()
            Dim sqlAndParameterList As SqlAndParameterList = _insertDataList.ToSqlAndParameter()
            command.Parameters.AddRange(ToDbParameterList(sqlAndParameterList.ParameterList, command).ToArray())
            command.CommandText = String.Format(InsertSql, _tableName, CreateInsertFieldString(),
                                                sqlAndParameterList.SqlString,
                                                _conditionList.ToWhereSql())
            command.Prepare()
            Return command
        End Function

        Private Function CreateInsertFieldString() As String
            If _insertFieldList.Count = 0 Then Return String.Empty

            Dim sqlString As String = "("
            For Each field As String In _insertFieldList
                If Not sqlString.Equals("(") Then
                    sqlString += ","
                End If
                sqlString += EncloseIdentifier(field)
            Next
            sqlString += ")"
            Return sqlString
        End Function

        Public Sub New(ByVal tableName As String)
            _tableName = EncloseIdentifier(tableName)
            _insertDataList = New InsertDataList()
            _insertFieldList = New List(Of String)()
            _conditionList = New ConditionList()
        End Sub
    End Class
End Namespace
