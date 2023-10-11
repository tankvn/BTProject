Public Class TransactionManager
    Implements IDisposable

    Private ReadOnly _dbAccessor As DbAccessor

    Private ReadOnly _transactionName As String

    Private _isCommitted As Boolean

    Public Sub New(ByVal dbAccessor As DbAccessor, ByVal transactionName As String)
        _dbAccessor = dbAccessor
        _transactionName = transactionName
        _dbAccessor.BeginTransaction(_transactionName)
    End Sub

    Public Sub New(ByVal dbAccessor As DbAccessor)
        Me.new(dbAccessor, String.Empty)
    End Sub

    Public Sub Commit()
        _dbAccessor.CommitTransaction()
        _isCommitted = True
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        If Not _isCommitted Then
            _dbAccessor.RollbackTransaction(_transactionName)
        End If
    End Sub
End Class
