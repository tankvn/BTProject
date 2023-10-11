Imports Database.SqlData.Condition
Imports Database.My.Resources

Namespace SqlData
    Public Class SqlDeleteTableCommand
        Implements ISqlCommand

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

        Private ReadOnly _confirmExist As Boolean

        Sub New(ByVal tableName As String)
            Me.New(tableName, False)
        End Sub

        Sub New(ByVal tablename As String, ByVal confirmExist As Boolean)
            _tableName = tablename
            _confirmExist = confirmExist
        End Sub

        Public Function ToSqlCommand(ByVal connection As DbConnection) As DbCommand Implements ISqlCommand.ToSqlCommand
            Dim command = connection.CreateCommand()
            command.CommandText = If(_confirmExist,
                                     String.Format(DropTableIfExistSql, _tableName),
                                     String.Format(DropTableSql, _tableName))
            Return command
        End Function
    End Class
End Namespace
