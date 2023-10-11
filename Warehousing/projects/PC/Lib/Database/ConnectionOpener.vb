Friend Class ConnectionOpener
    Implements IDisposable

    Private ReadOnly _connection As DbConnection

    Private ReadOnly _transactionState As TransactionState

    Public Sub New(ByRef connection As DbConnection, ByVal state As TransactionState)
        _connection = connection
        _transactionState = state
        If _transactionState = TransactionState.Execution Then Return
        _connection.Open()
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        If _transactionState = TransactionState.Execution Then Return
        _connection.Close()
    End Sub
End Class
