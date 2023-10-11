Imports Database.My.Resources
Imports Database.Utility

Namespace SqlData
    Public Class UpdateInventoryCommand
        Private ReadOnly _tableName As String

        Private ReadOnly _historyColumnName As String

        Private ReadOnly _trueQuantityColumnName As String

        Private ReadOnly _assumedQuantityColumnName As String

        Public Sub New(ByVal tableName As String, ByVal trueQuantityColumnName As String, _
                       ByVal assumedQuantityColumnName As String, ByVal historyColumnName As String)
            _tableName = EncloseIdentifier(tableName)
            _trueQuantityColumnName = trueQuantityColumnName
            _assumedQuantityColumnName = assumedQuantityColumnName
            _historyColumnName = historyColumnName
        End Sub

        Public Function CreateNotEqualCommand(ByRef connection As DbConnection) As DbCommand
            Dim command As DbCommand = connection.CreateCommand()
            command.CommandText = String.Format(UpdateSqlForUpdateInventoryNotEqual, _tableName, _historyColumnName, _
                                                _trueQuantityColumnName, _assumedQuantityColumnName)
            Return command
        End Function

        Public Function CreateEqualCommand(ByRef connection As DbConnection) As DbCommand
            Dim command As DbCommand = connection.CreateCommand()
            command.CommandText = String.Format(UpdateSqlForUpdateInventoryEqual, _tableName, _historyColumnName, _
                                                _trueQuantityColumnName, _assumedQuantityColumnName)
            Return command
        End Function

        Public Function CreateNullCommand(ByRef connection As DbConnection) As DbCommand
            Dim command As DbCommand = connection.CreateCommand()
            command.CommandText = String.Format(UpdateSqlForUpdateInventoryNull, _tableName, _historyColumnName, _
                                                _trueQuantityColumnName, _assumedQuantityColumnName)
            Return command
        End Function
    End Class
End Namespace

