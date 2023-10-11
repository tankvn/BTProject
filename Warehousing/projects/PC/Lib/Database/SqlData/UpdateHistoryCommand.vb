Imports Database.My.Resources
Imports Database.Utility

Namespace SqlData
    Public Class UpdateHistoryCommand
        Private ReadOnly _tableName As String

        Private ReadOnly _historyFieldName As String

        Private ReadOnly _differenceFieldName As String

        Private ReadOnly _actualStockFieldName As String

        Private ReadOnly _theoreticalStockFieldName As String

        Public Sub New(ByVal tableName As String, ByVal actualStockFieldName As String,
                       ByVal theoreticalStockFieldName As String, ByVal historyFieldName As String,
                       ByVal differenceFieldName As String)
            _tableName = EncloseIdentifier(tableName)
            _actualStockFieldName = actualStockFieldName
            _theoreticalStockFieldName = theoreticalStockFieldName
            _historyFieldName = historyFieldName
            _differenceFieldName = differenceFieldName
        End Sub

        Public Function CreateHistoryCommand(ByVal connection As DbConnection) As DbCommand
            Dim command As DbCommand = connection.CreateCommand()
            command.CommandText = String.Format(UpdateSqlForUpdateInventory, _tableName, _historyFieldName,
                                                _differenceFieldName, _actualStockFieldName, _theoreticalStockFieldName)
            Return command
        End Function

        Public Function CreateNullCommand(ByRef connection As DbConnection) As DbCommand
            Dim command As DbCommand = connection.CreateCommand()
            command.CommandText = String.Format(UpdateSqlForUpdateInventoryNull, _tableName, _historyFieldName,
                                                _differenceFieldName, _actualStockFieldName, _theoreticalStockFieldName)
            Return command
        End Function
    End Class
End Namespace
