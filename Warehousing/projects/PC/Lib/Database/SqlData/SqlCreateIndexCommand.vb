Imports Database.SqlData.Condition
Imports Database.My.Resources
Imports Database.Utility

Namespace SqlData
    Public Class SqlCreateIndexCommand
        Implements ISqlCommand

        Private ReadOnly _tableName As String = String.Empty
        Public ReadOnly Property TableName() As String Implements ISqlCommand.TableName
            Get
                Return _tableName
            End Get
        End Property

        Private ReadOnly _fieldName As String = String.Empty

        Private ReadOnly _indexName As String = String.Empty

        Public ReadOnly Property ConditionList() As ConditionList Implements ISqlCommand.ConditionList
            Get
                Return Nothing
            End Get
        End Property

        Public Sub New(ByVal tableName As String, ByVal fieldName As String)
            _tableName = tableName
            _fieldName = fieldName
            _indexName = tableName & "_" & fieldName
        End Sub

        Public Function ToSqlCommand(ByVal connection As DbConnection) As DbCommand Implements ISqlCommand.ToSqlCommand
            Dim command = connection.CreateCommand()
            command.CommandText = String.Format(CreateIndexSql, EncloseIdentifier(_indexName), EncloseIdentifier(_tableName), EncloseIdentifier(_fieldName))
            Return command
        End Function
    End Class
End Namespace
