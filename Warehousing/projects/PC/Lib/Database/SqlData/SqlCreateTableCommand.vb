Imports Database.SqlData.Condition
Imports Database.SqlData.Column
Imports Database.My.Resources
Imports Database.Utility

Namespace SqlData
    Public Class SqlCreateTableCommand
        Implements ISqlCommand
        Private ReadOnly _columnData As ColumnDataList
        Public ReadOnly Property ColumnData() As ColumnDataList
            Get
                Return _columnData
            End Get
        End Property

        Private ReadOnly _tableName As String
        Public ReadOnly Property TableName() As String Implements ISqlCommand.TableName
            Get
                Return _tableName
            End Get
        End Property

        Public ReadOnly Property ConditionList() As ConditionList Implements ISqlCommand.ConditionList
            Get
                Return Nothing
            End Get
        End Property

        Sub New(ByVal tableName As String)
            _tableName = EncloseIdentifier(tableName)
            _columnData = New ColumnDataList()
        End Sub

        Public Function ToSqlCommand(ByVal connection As DbConnection) As DbCommand Implements ISqlCommand.ToSqlCommand
            Dim command = connection.CreateCommand()
            command.CommandText = String.Format(CreateTableSql, _tableName, _columnData.ToSqlString())
            Return command
        End Function
    End Class
End Namespace
