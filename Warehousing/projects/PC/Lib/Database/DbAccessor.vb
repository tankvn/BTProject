Imports Database.SqlData.Condition
Imports Database.SqlData
Imports System.Windows.Forms
Imports Database.SqlData.DisplayData
Imports Database.Utility
Imports Database.DbException
Imports Database.My.Resources
Imports System.IO
Imports System.Globalization
Imports System.Text

Public MustInherit Class DbAccessor
    Implements IDisposable

    Private Const SQLITE_FILE_PATH = "sqlite3.exe"

    Private Const LOG_FILE_PATH = "SQLLog.log"

    Private Const WAIT_TIME = 600000

    Private ReadOnly _connection As DbConnection

    Private _isDisposed As Boolean

    Private _transactionState As TransactionState

    Public Shared Property DebugMode() As Boolean = False

    Public ReadOnly Property TransactionState() As TransactionState
        Get
            Return _transactionState
        End Get
    End Property

    Private ReadOnly Property IsConnectionOpen() As Boolean
        Get
            Return Not _connection.State = ConnectionState.Closed
        End Get
    End Property

    Private ReadOnly _dbFileName As String
    Public ReadOnly Property DbFileName() As String
        Get
            Return _dbFileName
        End Get
    End Property

    Private ReadOnly _factory As DbProviderFactory

    Friend Sub New(ByVal connection As DbConnection, ByVal factory As DbProviderFactory, ByVal dbFileName As String)
        Try
            _factory = factory
            _connection = connection
            _dbFileName = dbFileName
            _transactionState = TransactionState.NonExecution

            ConnectionOpen()
        Catch ex As Exception
            Throw ConvertException(ex)
        End Try
    End Sub

    Friend Sub Dispose() Implements IDisposable.Dispose
        If _isDisposed Then Return
        If Not IsNothing(_connection) Then
            _connection.Close()
            _connection.Dispose()
            GC.Collect()
            GC.WaitForPendingFinalizers()
            _isDisposed = True
        End If
    End Sub

    Friend Sub ConnectionOpen()
        Try
            If Not IsConnectionOpen Then
                _connection.Open()
            End If
        Catch ex As Exception
            Throw ConvertException(ex)
        End Try
    End Sub

    Public Sub BeginTransaction(ByVal transactionName As String)
        If _transactionState = TransactionState.Execution Then Return
        ConnectionOpen()
        Transaction(transactionName, BeginTransactionSql)
        _transactionState = TransactionState.Execution
    End Sub

    Public Sub BeginTransaction()
        BeginTransaction(String.Empty)
    End Sub

    Public Sub CommitTransaction(ByVal transactionName As String)
        If _transactionState = TransactionState.NonExecution Then Return
        Transaction(transactionName, CommitTransactionSql)
        _transactionState = TransactionState.NonExecution
    End Sub

    Public Sub CommitTransaction()
        CommitTransaction(String.Empty)
    End Sub

    Public Sub RollbackTransaction(ByVal transactionName As String)
        If _transactionState = TransactionState.NonExecution Then Return
        Transaction(transactionName, RollbackTransactionSql)
        _transactionState = TransactionState.NonExecution
    End Sub

    Public Sub RollbackTransaction()
        RollbackTransaction(String.Empty)
    End Sub

    Private Sub Transaction(ByVal transactionName As String, ByVal transactionSql As String)
        Dim name = transactionName
        If name = Nothing Then
            name = String.Empty
        End If
        name = EncloseIdentifier(name)
        Try
            Using command As DbCommand = _connection.CreateCommand()
                command.CommandText = String.Format(transactionSql, name)
                ExecuteNonQuery(command)
            End Using
        Catch ex As Exception
            Throw ConvertException(ex)
        End Try
    End Sub

    Public Function ExecuteCommandAndRead(ByVal sqlCommand As SqlSelectCommand) As String()()
        Return ExecuteCommandAndReadImplement(sqlCommand)
    End Function

    Public Function ExecuteCommandAndRead(ByVal sqlCommand As SqlSelectCommandForCalculationLabel) As String()()
        Return ExecuteCommandAndReadImplement(sqlCommand)
    End Function

    Public Function ExecuteCommandAndRead(ByVal sqlCommand As SqlSelectFilterTreeItemCommand) As String()
        Dim result As String()() = ExecuteCommandAndReadImplement(sqlCommand)
        Dim resultList = New List(Of String)
        For Each strings As String() In result
            resultList.Add(strings(0))
        Next
        Return resultList.ToArray()
    End Function

    Private Function ExecuteCommandAndReadImplement(ByVal sqlCommand As ISqlCommand) As String()()
        Try
            ConnectionOpen()
            Using dbCommand As DbCommand = sqlCommand.ToSqlCommand(_connection)
                Return GetValues(dbCommand)
            End Using
        Catch ex As Exception
            Throw ConvertException(ex)
        End Try
    End Function

    Private Function GetValues(ByVal command As DbCommand) As String()()
        If Not DebugMode Then
            Using reader As DbDataReader = command.ExecuteReader()
                Return GetValues(reader)
            End Using
        End If
        Try
            Dim values As String()()
            Dim start = DateTime.Now
            OutputLog(command)
            Using reader As DbDataReader = command.ExecuteReader()
                values = GetValues(reader)
            End Using
            Dim elapsedTime = DateTime.Now - start
            OutputLog(vbTab + String.Format(LogElapsedTime, elapsedTime.TotalMilliseconds.ToString(CultureInfo.InvariantCulture)))
            Return values
        Catch ex As Exception
            OutputLog(vbTab + ex.Message)
            Throw
        End Try
    End Function

    Public Function ExecuteCommandAndRead(ByVal sqlCommand As SqlSelectCommand, ByVal retryNum As Integer) As String()()
        Try
            Return ExecuteCommandAndRead(sqlCommand)
        Catch ex As Exception
            If retryNum <= 0 Then
                Throw
            End If
            Return ExecuteCommandAndRead(sqlCommand, retryNum - 1)
        End Try
    End Function

    Public Sub ExecuteCommand(ByVal data As ISqlCommand)
        Dim needTransaction = _transactionState = TransactionState.NonExecution
        Try
            ConnectionOpen()
            If needTransaction Then
                Using transactionManager As New TransactionManager(Me)
                    Using command As DbCommand = data.ToSqlCommand(_connection)
                        ExecuteNonQuery(command)
                    End Using
                    transactionManager.Commit()
                End Using
            Else
                Using command As DbCommand = data.ToSqlCommand(_connection)
                    ExecuteNonQuery(command)
                End Using
            End If
        Catch ex As Exception
            Throw ConvertException(ex)
        End Try
    End Sub

    Public Sub ExecuteCommand(ByVal data As SqlAttachCommand)
        Try
            ConnectionOpen()
            Using dbCommand As DbCommand = data.ToSqlCommand(_connection)
                ExecuteNonQuery(dbCommand)
            End Using
        Catch ex As Exception
            Throw ConvertException(ex)
        End Try
    End Sub

    Public Sub ExecuteCommandWithRetry(ByVal data As ISqlCommand, ByVal retryNum As Integer)
        Try
            ExecuteCommand(data)
        Catch ex As Exception
            If retryNum = 0 Then
                Throw
            End If
            ExecuteCommandWithRetry(data, retryNum - 1)
        End Try
    End Sub

    Private Function GetValues(ByVal reader As DbDataReader) As String()()
        Dim strListList As List(Of String()) = New List(Of String())()
        Dim strList As List(Of String) = New List(Of String)()
        While reader.Read()
            Dim values(reader.FieldCount - 1) As Object
            reader.GetValues(values)
            For Each value As Object In values
                If IsNothing(value) Then
                    strList.Add(String.Empty)
                    Continue For
                End If
                strList.Add(value.ToString())
            Next
            strListList.Add(strList.ToArray())
            strList.Clear()
        End While
        Return strListList.ToArray()
    End Function

    Public MustOverride Function GetTables() As String()

    Public MustOverride Function GetViews() As String()

    Public MustOverride Function GetTablesAndViews() As String()

    Public MustOverride Function GetFields(ByVal tableName As String) As String()

    Public MustOverride Function GetIndexes() As String()

    Public MustOverride Sub AddColumn(ByVal tableName As String, ByVal columnName As String, ByVal type As DataType)

    Public MustOverride Function GetTableSchema(ByVal tableName As String) As String

    Public MustOverride Function ExistTable(ByVal tableName As String) As Boolean

    Protected Function GetStringArrayFromDb(ByVal sqlString As String, ByVal index As Integer)
        Try
            ConnectionOpen()
            Using command As DbCommand = _connection.CreateCommand()
                command.CommandText = sqlString
                Return GetStringArray(command, index)
            End Using
        Catch ex As Exception
            Throw ConvertException(ex)
        End Try
    End Function

    Private Function GetStringArray(ByVal command As DbCommand, ByVal index As Integer) As String()
        If Not DebugMode Then
            Return ReadData(command, index)
        End If

        Try
            OutputLog(command)
            Dim start = DateTime.Now
            Dim array As String() = ReadData(command, index)
            Dim elapsedTime = DateTime.Now - start
            OutputLog(vbTab + String.Format(LogElapsedTime, elapsedTime.TotalMilliseconds.ToString(CultureInfo.InvariantCulture)))
            Return array
        Catch ex As Exception
            OutputLog(vbTab + ex.Message)
            Throw
        End Try
    End Function

    Private Function ReadData(ByVal command As DbCommand, ByVal index As Integer) As String()
        Using reader As DbDataReader = command.ExecuteReader()
            Dim resultList As List(Of String) = New List(Of String)()
            While (reader.Read())
                resultList.Add(reader.GetValue(index).ToString())
            End While
            Return resultList.ToArray()
        End Using
    End Function

    Public Function CreateBindingSource(ByVal sqlSelectData As SqlSelectCommand) As BindingSource
        Try
            Dim bindingSource As BindingSource = New BindingSource()
            bindingSource.DataSource = CreateDataTable(GetDataAdaptor(sqlSelectData.ToSqlCommand(_connection)))
            Return bindingSource
        Catch ex As Exception
            Throw ConvertException(ex)
        End Try
    End Function

    Public Function CreateBindingSource(ByVal tableName As String, ByVal conditionList As ConditionList, ByRef displayDataList As DisplayDataList) As BindingSource
        If IsNothing(Me) Then Return Nothing
        Dim sqlData As New SqlSelectCommand(tableName)
        If Not IsNothing(conditionList) Then
            sqlData.ConditionList.AddRange(conditionList)
        End If
        If Not IsNothing(displayDataList) Then
            sqlData.DisplayDataList.AddRange(displayDataList)
        End If
        Return CreateBindingSource(sqlData)
    End Function

    Private Function CreateDataTable(ByVal dbDataAdapter As DbDataAdapter) As DataTable
        If Not DebugMode Then
            Dim dataTable As DataTable = New DataTable()
            dbDataAdapter.Fill(dataTable)
            Return dataTable
        End If
        Try
            Dim start = DateTime.Now
            OutputLog(dbDataAdapter.SelectCommand)
            Dim dataTable As DataTable = New DataTable()
            dbDataAdapter.Fill(dataTable)
            Dim elapsedTime = DateTime.Now - start
            OutputLog(vbTab + String.Format(LogElapsedTime, elapsedTime.TotalMilliseconds.ToString(CultureInfo.InvariantCulture)))
            Return dataTable
        Catch ex As Exception
            OutputLog(vbTab + ex.Message)
            Throw
        End Try

    End Function

    Private Function GetDataAdaptor(ByVal command As DbCommand) As DbDataAdapter
        Dim dbDataAdapter As DbDataAdapter = _factory.CreateDataAdapter()
        dbDataAdapter.SelectCommand = command
        Return dbDataAdapter
    End Function

    Friend Sub ExecuteCommand(ByVal sqlCommand As String)
        Dim needTransaction = _transactionState = TransactionState.NonExecution
        Try
            If needTransaction Then
                Using transactionManager As New TransactionManager(Me)
                    Dim command As New SqlCommand(sqlCommand, New List(Of Parameter))
                    ExecuteCommand(command)
                    transactionManager.Commit()
                End Using
            Else
                Dim command As New SqlCommand(sqlCommand, New List(Of Parameter))
                ExecuteCommand(command)
            End If
        Catch ex As Exception
            Throw ConvertException(ex)
        End Try
    End Sub


    Friend Sub ExecuteCommandWithoutTransaction(ByVal sqlCommand As String)
        Try
            Dim command As DbCommand = _connection.CreateCommand()
            command.CommandText = sqlCommand
            ExecuteNonQuery(command)
        Catch ex As Exception
            Throw ConvertException(ex)
        End Try
    End Sub

    Private Sub ExecuteNonQuery(ByVal command As DbCommand)
        If Not DebugMode Then
            command.ExecuteNonQuery()
            Return
        End If
        Try
            OutputLog(command)
            Dim start = DateTime.Now
            command.ExecuteNonQuery()
            Dim elapsedTime = DateTime.Now - start
            OutputLog(vbTab + String.Format(LogElapsedTime, elapsedTime.TotalMilliseconds.ToString(CultureInfo.InvariantCulture)))
        Catch ex As Exception
            OutputLog(vbTab + ex.Message)
            Throw
        End Try
    End Sub

    Private Sub OutputLog(ByVal text As String)
        Try
            Using writer As New StreamWriter(LOG_FILE_PATH, True, Encoding.UTF8)
                writer.WriteLine(text)
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Private Sub OutputLog(ByVal command As DbCommand)
        If (IsNothing(command.Parameters) OrElse command.Parameters.Count = 0) Then
            OutputLog(command.CommandText)
            Return
        End If

        Dim parameterString As New StringBuilder()
        Dim parameterNumber = 1
        For Each parameter As DbParameter In command.Parameters
            parameterString.AppendLine(vbTab + String.Format(LogPlaceHolder,
                                                             parameterNumber,
                                                             parameter.Value.ToString(),
                                                             parameter.DbType.ToString()))
            parameterNumber += 1
        Next
        Dim logString As String = command.CommandText + vbCrLf + parameterString.ToString().TrimEnd(vbCrLf).TrimEnd(vbCr).TrimEnd(vbLf)
        OutputLog(logString)
    End Sub

    Public Sub ExecuteCommand(ByVal data As UpdateHistoryCommand)
        Dim needTransaction = _transactionState = TransactionState.NonExecution
        Try
            If needTransaction Then
                Using transactionManager As New TransactionManager(Me)
                    ExecuteUpdateInventoryCommand(data)
                    transactionManager.Commit()
                End Using
            Else
                ExecuteUpdateInventoryCommand(data)
            End If
        Catch ex As Exception
            Throw ConvertException(ex)
        End Try
    End Sub

    Private Sub ExecuteUpdateInventoryCommand(ByVal data As UpdateHistoryCommand)
        Using command = data.CreateHistoryCommand(_connection)
            ExecuteNonQuery(command)
        End Using
        Using command = data.CreateNullCommand(_connection)
            ExecuteNonQuery(command)
        End Using
    End Sub

    Public Sub ExecuteCommand(ByVal logUpdateCommand As LogUpdateCommand)
        Try
            If logUpdateCommand.LogUpdateTargetList.HasAssignmentTarget Then
                Dim deleteCommand As New SqlDeleteTableCommand(logUpdateCommand.TEMP_NEW_TABLE_NAME, True)
                ExecuteCommand(deleteCommand)
                ExecuteCommand(logUpdateCommand.GetNewDataCommand())
            End If
            If logUpdateCommand.LogUpdateTargetList.HasSumTarget Then
                Dim deleteCommand As New SqlDeleteTableCommand(logUpdateCommand.TEMP_SUM_TABLE_NAME, True)
                ExecuteCommand(deleteCommand)
                ExecuteCommand(logUpdateCommand.GetGroupSumCommand())
            End If

            ExecuteCommand(logUpdateCommand.GetUpdateCommand())
            ExecuteCommand(logUpdateCommand.GetInsertCommand())
        Catch ex As Exception
            Throw ConvertException(ex)
        End Try
    End Sub

    Public Shared Function ImportCsv(ByVal accessor As DbAccessor, ByVal dbFilePath As String, ByVal importFilePath As String, ByVal tableName As String, ByVal separator As Char) As ImportResult
        Dim workingDirectory = Path.GetDirectoryName(dbFilePath)
        accessor.CommitTransaction()
        Return ExecuteImport(Path.GetFileName(importFilePath), Path.GetFileName(dbFilePath), workingDirectory, tableName, separator)
    End Function

    Private Shared Function ExecuteImport(ByVal importFile As String, ByVal dbFilePath As String, ByVal workingDirectory As String, ByVal tableName As String, ByVal separator As Char) As ImportResult
        Try
            Dim result As ImportResult = ImportResult.Success
            Dim importFileName = Path.GetFileName(importFile)
            Dim dbFileName = Path.GetFileName(dbFilePath)
            Using process = CreateProcess(workingDirectory)
                process.StartInfo.RedirectStandardInput = True
                process.StartInfo.Arguments = """" + dbFileName + """"

                process.Start()
                InputImportCommand(process, importFileName, tableName, separator)
                If Not process.WaitForExit(WAIT_TIME) Then
                    process.Kill()
                    Throw New TimeoutException()
                End If
            End Using
            Return result
        Catch ex As Exception
            Throw New ExecuteSqlite3Exception(ex)
        End Try
    End Function

    Private Shared Function CreateProcess(ByVal workingDirectory As String) As Process
        Dim process = New Process()
        process.StartInfo.FileName = SQLITE_FILE_PATH
        process.StartInfo.CreateNoWindow = True
        process.StartInfo.UseShellExecute = False
        process.StartInfo.WorkingDirectory = workingDirectory
        Return process
    End Function

    Private Shared Sub InputImportCommand(ByVal process As Process, ByVal csvFileName As String, ByVal tableName As String, ByVal separator As String)
        Dim tempPath = csvFileName.Replace("\"c, "/"c)
        process.StandardInput.WriteLine(SQLite3SeparatorCommand, separator)
        process.StandardInput.WriteLine(SQLite3ImportCommand, tempPath, tableName)
        process.StandardInput.WriteLine(SQLite3ExitCommand)
    End Sub
End Class
