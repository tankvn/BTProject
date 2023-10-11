Imports btnaviReqHandler
Imports System.IO
Imports Database.SqlData.Condition
Imports Database.SqlData.DisplayData
Imports System.Globalization
Imports Microsoft.VisualBasic.FileIO
Imports Service.CommSettings
Imports Database
Imports Database.SqlData
Imports Database.DbException
Imports DTC.Log
Imports SettingLib.Table
Imports System.Text
Imports UtilityLib

Namespace Remote
    Public Class RemoteAccess

        Private Const OPEN_SUCCESS_EVENT_ID = 1003

        Private Const OPEN_FAILED_EVENT_ID = 1004

        Private Const CLOSE_EVENT_ID = 71

        Private Const ERR_DB_ACCESS As Integer = -1100

        Private Const ERR_HRC_REQUEST As Integer = -2017

        Private Const REMOTE_ACCESS_LOG_FILENAME As String = "CommunicationLog\RemoteAccessLog.txt"

        Private Const ROWID_FIELD_NAME As String = "ROWID"

        Private Const REQUEST_OPT_INSERT_DATA As String = "INSERT_DATA"

        Private Const REQUEST_OPT_SELECT_COUNT As String = "SELECT_COUNT"

        Private Const REQUEST_OPT_SELECT_ONE As String = "SELECT_ONE"

        Private Const REQUEST_OPT_SELECT_MULTIPLE As String = "SELECT_MULTIPLE"

        Private Const OPERATION_EXACT_MATCH As Byte = 0

        Private Const OPERATION_FORWARD_MATCH As Byte = 20

        Private Const OPERATION_BACKWARD_MATCH As Byte = 21

        Private Const OPERATION_PARTICAL_MATCH As Byte = 22

        Private Const OPERATION_OR_GROUP_START As Byte = 90

        Private Const OPERATION_OR_GROUP_END As Byte = 91

        Private Const MAX_RECORD_COUNT As Integer = 36

        Private Const RESULT_COLUMN_NAME = "Result"

        Private Shared WithEvents _btRemoteAccess As BTRemoteAccess2

        Private Shared ReadOnly RequestController As New RequestController()

        Private Shared _selectMultipleResults As New Dictionary(Of String, List(Of String()))

        Public Shared Event OpenSucceeded As EventHandler

        Public Shared Event OpenFailed As EventHandler

        Public Shared Event CloseCompleted As EventHandler

        Private Shared _logFileStream As FileStream
        Private Shared _logWriter As StreamWriter
        Private Shared _syncLogWriter As TextWriter

        Public Shared Function GetIsOpen() As Boolean
            If IsNothing(_btRemoteAccess) Then Return False
            Return _btRemoteAccess.Open
        End Function

        Public Shared Sub Open(ByVal dataPort As Integer)
            Try
                _btRemoteAccess = New BTRemoteAccess2
                _btRemoteAccess.DataPort = dataPort
                _btRemoteAccess.Open = True
                RequestController.Start()
            Catch ex As Exception
                ErrorUtility.OutputErrorLog("remote open failed", String.Empty)
                RaiseEvent OpenFailed(Nothing, EventArgs.Empty)
            End Try

            Try
                Dim logFilePath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, REMOTE_ACCESS_LOG_FILENAME)
                _logFileStream = File.Open(logFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)
                _logWriter = New StreamWriter(_logFileStream)
                _logWriter.AutoFlush = True
                _syncLogWriter = TextWriter.Synchronized(_logWriter)
            Catch ex As Exception
            End Try
        End Sub

        Public Shared Sub Close()
            Try
                _btRemoteAccess.Open = False
                RequestController.Wait()
            Catch ex As Exception
                ErrorUtility.OutputErrorLog("remote close failed", String.Empty)
            End Try

            Try
                If Not IsNothing(_syncLogWriter) Then
                    _syncLogWriter.Dispose()
                    _syncLogWriter = Nothing
                End If
                If Not IsNothing(_logWriter) Then
                    _logWriter.Dispose()
                    _logWriter = Nothing
                End If
                If Not IsNothing(_logFileStream) Then
                    _logFileStream.Dispose()
                    _logFileStream = Nothing
                End If
            Catch ex As Exception
            End Try
        End Sub

        Private Shared Sub WriteLog(ByVal isDetail As Boolean, ByVal ip As String, ByVal seqNo As UInteger, ByVal eventName As String, ByVal text As String)
            If IsNothing(_syncLogWriter) Then Return

            Dim writeString As New StringBuilder()
            Dim now As DateTime = DateTime.Now
            If isDetail Then
                writeString.Append("D" + vbTab)

            Else
                writeString.Append("S" + vbTab)
            End If
            writeString.Append(now.ToShortDateString())
            writeString.Append(vbTab)
            writeString.Append(now.ToLongTimeString())
            writeString.Append(now.ToString(".fff"))
            writeString.Append(vbTab + vbTab)
            writeString.Append("[" & ip & "] SEQ:" & seqNo.ToString(CultureInfo.InvariantCulture) & " " & eventName & " " & text)

            _syncLogWriter.WriteLine(writeString.ToString())
        End Sub

        Private Shared Sub BTRemoteAccess_RequestArrival(ByVal sender As Object, ByVal e As BTRemoteAccessRequestEventArgs) Handles _btRemoteAccess.RequestArrival

            If e.Handy Is Nothing Then
                WriteLog(False, e.Ip, e.SeqNo, "RequestArrival", "Canceled")
                Exit Sub
            End If

            WriteLog(False, e.Ip, e.SeqNo, "RequestArrival", e.Handy.requestID.ToString() & " " & e.Handy.Request.Option(1))

            RequestController.AddRequest(e.Ip, e.SeqNo, e.Handy)
        End Sub

        Private Shared Sub BTRemoteAccess_SystemEvent(ByVal sender As Object, ByVal e As BTRemoteAccessSystemEventArgs) Handles _btRemoteAccess.SystemEvent
            Select Case e.EventId
                Case OPEN_SUCCESS_EVENT_ID
                    RaiseEvent OpenSucceeded(sender, EventArgs.Empty)
                Case OPEN_FAILED_EVENT_ID
                    RaiseEvent OpenFailed(sender, EventArgs.Empty)
                Case CLOSE_EVENT_ID
                    RaiseEvent CloseCompleted(sender, EventArgs.Empty)
            End Select
        End Sub

        Private Shared Sub BTRemoteAccess_Result(ByVal sender As Object, ByVal e As BTRemoteAccessResultEventArgs) Handles _btRemoteAccess.Result
            WriteLog(False, e.Ip, e.SeqNo, "Result", e.Result)

            RequestController.AddResult(e.Ip, e.SeqNo, e.Result)
        End Sub

        Public Shared Sub ResultAction(ByVal waitResult As WaitResult, ByVal result As String)
            If waitResult.IsWait Then
                Dim accessor As DbAccessor = DbAccessManager.GetSQLiteAccessor(waitResult.DbPath)
                If result = "response reached" Then
                    accessor.CommitTransaction()
                Else
                    accessor.RollbackTransaction()
                End If

                If waitResult.NeedClose Then
                    DbAccessManager.DeleteSQLiteAccessor(waitResult.DbPath)
                End If
            End If
        End Sub

        Private Shared Sub BTRemoteAccess_Cancel(ByVal sender As Object, ByVal e As BTRemoteAccessCancelEventArgs) Handles _btRemoteAccess.Cancel
            WriteLog(False, e.Ip, e.SeqNo, "Cancel", String.Empty)

            RequestController.Cancel(e.Ip, e.SeqNo)
        End Sub

        Public Shared Function RequestArrival(ByRef handy As BTHandy2) As WaitResult

            If handy.requestID <> RequestID.REQID_DB_FETCH_F Then
                _selectMultipleResults.Remove(handy.Ip)
            End If

            Select Case handy.requestID
                Case RequestID.REQID_DB_INSERT
                    Return ExecuteDbInsert(handy)
                Case RequestID.REQID_DB_SELECT
                    Return ExecuteDbSelect(handy)
                Case RequestID.REQID_DB_SELECT_INTO_FILE
                Case RequestID.REQID_DB_SQLDIRECT
                Case RequestID.REQID_DB_DELETE
                Case RequestID.REQID_DB_UPDATE
                Case RequestID.REQID_DB_COUNT
                Case RequestID.REQID_F_READ
                Case RequestID.REQID_F_WRITE
                Case RequestID.REQID_DB_FIN
                Case RequestID.REQID_DB_FETCH_F
                    Return ExecuteDbSelectFetch(handy)
                Case RequestID.REQID_DB_FETCH_B
                Case RequestID.REQID_F_SEND
                    handy.Response.Result = 0

                Case RequestID.REQID_F_RECV
                    handy.Response.Result = 0
                Case RequestID.REQID_DB_INIT
                Case RequestID.REQID_USER
            End Select

            If handy.IsValid Then
                WriteLog(False, handy.Ip, handy.SeqNo, "SendResponse", String.Empty)
                handy.SendResponse()
            Else
                ErrorUtility.OutputErrorLog("Request Error!", String.Empty)
            End If

            Return New WaitResult()
        End Function

        Private Shared Function ExecuteDbSelect(ByVal handy As BTHandy2) As WaitResult
            Dim dtcSettingsLoadResult As LoadResult = SettingsReaderWriter.Load()
            Dim request As BTRequestDB2 = handy.Request
            Dim response As BTResponseDB2 = handy.Response

            Dim isSendFolderFile As Boolean = False

            Dim dbPath As String
            If String.IsNullOrEmpty(request.DatabaseName) Then
                dbPath = FileUtility.DbFilePath
            Else
                Dim sendFolder = dtcSettingsLoadResult.Settings.Common.Folders.SendFolder
                dbPath = Path.Combine(sendFolder, request.DatabaseName)
                isSendFolderFile = True
            End If

            Dim accessor As SQLiteAccessor = DbAccessManager.GetSQLiteAccessor(dbPath)
            Try
                accessor.BeginTransaction()

                Select Case request.Option(1)
                    Case REQUEST_OPT_SELECT_COUNT

                        If Not ExecuteDbSelectCount(request, accessor, response) Then
                            SendResponse(handy, ERR_DB_ACCESS)
                            If isSendFolderFile Then
                                DbAccessManager.DeleteSQLiteAccessor(dbPath)
                            End If
                            Return New WaitResult()
                        End If

                        SendResponse(handy, 1)

                    Case REQUEST_OPT_SELECT_ONE

                        Dim recordCount As Integer = ExecuteDbSelectOne(request, accessor, response)
                        SendResponse(handy, recordCount)

                    Case REQUEST_OPT_SELECT_MULTIPLE

                        Dim recordCount As Integer = ExecuteDbSelectMultiple(handy.Ip, request, accessor, response)
                        SendResponse(handy, recordCount)

                    Case Else
                        SendResponse(handy, ERR_HRC_REQUEST)
                        If isSendFolderFile Then
                            DbAccessManager.DeleteSQLiteAccessor(dbPath)
                        End If
                        Return New WaitResult()
                End Select

            Catch ex As DbAccessException
                ErrorUtility.OutputErrorLog(My.Resources.RegisterLogFailed, handy.Ip)

                SendResponse(handy, ERR_DB_ACCESS)
                If isSendFolderFile Then
                    DbAccessManager.DeleteSQLiteAccessor(dbPath)
                End If
                Return New WaitResult()
            End Try

            Return New WaitResult(True, dbPath, isSendFolderFile)

        End Function

        Private Shared Function ExecuteDbSelectCount(ByVal request As BTRequestDB2, ByVal accessor As SQLiteAccessor, ByVal response As BTResponseDB2) As Boolean

            Dim command As SqlSelectCommand = GetSelectCountCommand(request)

            Dim data As String()() = accessor.ExecuteCommandAndRead(command)

            If data.Length = 0 Then
                Return False
            End If

            If data(0).Length < 2 Then
                Return False
            End If

            response.ResultValues.Count = 1
            response.ResultValues(0).Index = 0
            response.ResultValues(0).Value = data(0)(0)
            response.ResultValues(0).Operation = 0
            response.Option(1, data(0)(1))

            Return True

        End Function

        Private Shared Function GetSelectCountCommand(ByVal request As BTRequestDB2) As SqlSelectCommand

            Dim command As New SqlSelectCommand(request.TableName)
            command.DisplayDataList.Add(New CountFieldDisplayData(String.Empty))
            command.DisplayDataList.Add(New MaxDisplayData(ROWID_FIELD_NAME))

            For index As Integer = 0 To request.KeyValues.Count - 1
                Dim condition As New LiteralConditionData(ConditionType.Equal,
                                                          String.Format(TableSetting.FIELD_NAME_FORMAT, request.KeyValues(index).Index + 1),
                                                          request.KeyValues(index).Value)
                command.ConditionList.Add(condition)
            Next
            Return command
        End Function

        Private Shared Function ExecuteDbSelectOne(ByVal request As BTRequestDB2, ByVal accessor As SQLiteAccessor, ByVal response As BTResponseDB2) As Integer

            Dim maxRecordId As Integer
            Dim offset As Integer
            Dim option2 As String() = request.Option(2).Split(","c)
            If (option2.Length < 2) OrElse (Not Integer.TryParse(option2(0), maxRecordId)) OrElse (Not Integer.TryParse(option2(1), offset)) Then
                maxRecordId = -1
                offset = 0
            End If

            Dim command As SqlSelectCommand = GetSelectOneCommand(request, maxRecordId)
            command.Limit = 1

            If 0 <= maxRecordId Then
                command.SortColumn = ROWID_FIELD_NAME
                command.SortOrder = SqlClient.SortOrder.Descending

                If 0 < offset Then
                    command.Offset = offset
                End If
            End If

            If request.NewValues.Count = 0 Then
                Dim fieldCount As Integer = accessor.GetFields(request.TableName).Length
                response.Option(1, fieldCount.ToString(CultureInfo.InvariantCulture))
            Else
                response.Option(1, request.NewValues.Count.ToString(CultureInfo.InvariantCulture))
            End If

            Dim data As String()() = accessor.ExecuteCommandAndRead(command)

            If data.Length = 0 Then
                response.ResultValues.Count = 0
                Return 0
            End If

            response.ResultValues.Count = 1
            response.ResultValues(0).Index = 0

            Dim value As StringBuilder = New StringBuilder()
            For index As Integer = 0 To data(0).Length - 1
                If index <> 0 Then
                    value.Append(",")
                End If

                value.Append(EncloseDoubleQuotations(data(0)(index)))
            Next
            response.ResultValues(0).Value = value.ToString()
            response.ResultValues(0).Operation = 0

            Return 1

        End Function

        Private Shared Function GetSelectOneCommand(ByVal request As BTRequestDB2, ByVal maxRecordId As Integer) As SqlSelectCommand

            Dim command As SqlSelectCommand = GetSelectCommand(request)

            If 0 <= maxRecordId Then
                Dim condition As New LiteralConditionData(ConditionType.AndLess,
                                                          ROWID_FIELD_NAME,
                                                          maxRecordId)
                command.ConditionList.Add(condition)
            End If

            Return command
        End Function

        Private Shared Function ExecuteDbSelectMultiple(ByVal ip As String, ByVal request As BTRequestDB2, ByVal accessor As SQLiteAccessor, ByVal response As BTResponseDB2) As Integer

            Dim command As SqlSelectCommand = GetSelectMultipleCommand(request)
            Dim limit As Integer
            Dim option2 As String = request.Option(2)
            If Integer.TryParse(option2, limit) Then
                command.Limit = limit
            End If

            Dim data As String()() = accessor.ExecuteCommandAndRead(command)

            If request.NewValues.Count = 0 Then
                Dim fieldCount As Integer = accessor.GetFields(request.TableName).Length
                response.Option(1, fieldCount.ToString(CultureInfo.InvariantCulture))
            Else
                response.Option(1, request.NewValues.Count.ToString(CultureInfo.InvariantCulture))
            End If

            If data.Length = 0 Then
                response.ResultValues.Count = 0
                Return 0
            End If


            Dim setDataCount As Integer = Math.Min(MAX_RECORD_COUNT, data.Length)
            response.ResultValues.Count = setDataCount

            For recordIndex As Integer = 0 To setDataCount - 1
                response.ResultValues(recordIndex).Index = 0

                Dim value As StringBuilder = New StringBuilder()
                For fieldIndex As Integer = 0 To data(recordIndex).Length - 1
                    If fieldIndex <> 0 Then
                        value.Append(",")
                    End If

                    value.Append(EncloseDoubleQuotations(data(recordIndex)(fieldIndex)))
                Next
                response.ResultValues(recordIndex).Value = value.ToString()
                response.ResultValues(recordIndex).Operation = 0
            Next

            If 0 < (data.Length - setDataCount) Then
                Dim restResult As List(Of String()) = New List(Of String())(data)
                restResult.RemoveRange(0, setDataCount)
                _selectMultipleResults(ip) = restResult
            End If

            Return data.Length

        End Function

        Private Shared Function EncloseDoubleQuotations(ByVal target As String) As String
            Dim result As String = target.Replace("""", """""")
            Return """" + result + """"
        End Function

        Private Shared Function GetSelectMultipleCommand(ByVal request As BTRequestDB2) As SqlSelectCommand

            Return GetSelectCommand(request)

        End Function

        Private Shared Function GetSelectCommand(ByVal request As BTRequestDB2) As SqlSelectCommand

            Dim command As New SqlSelectCommand(request.TableName)
            For index As Integer = 0 To request.NewValues.Count - 1
                command.DisplayDataList.Add(New ColumnDisplayData(String.Format(TableSetting.FIELD_NAME_FORMAT, request.NewValues(index).Index + 1)))
            Next

            Dim combineCondition As CombineConditionData = New CombineConditionData()
            combineCondition.OrCombining = False

            For index As Integer = 0 To request.KeyValues.Count - 1

                If request.KeyValues(index).Operation = OPERATION_OR_GROUP_START Then
                    If 0 < combineCondition.ConditionList.Count Then
                        command.ConditionList.Add(combineCondition)
                    End If
                    combineCondition = New CombineConditionData()
                    combineCondition.OrCombining = True
                    Continue For
                End If
                If request.KeyValues(index).Operation = OPERATION_OR_GROUP_END Then
                    If 0 < combineCondition.ConditionList.Count Then
                        command.ConditionList.Add(combineCondition)
                    End If
                    combineCondition = New CombineConditionData()
                    combineCondition.OrCombining = False
                    Continue For
                End If

                Dim conditionType As ConditionType = conditionType.Equal
                If request.KeyValues(index).Operation = OPERATION_FORWARD_MATCH Then
                    conditionType = SqlData.Condition.ConditionType.ForwardMatch
                ElseIf request.KeyValues(index).Operation = OPERATION_BACKWARD_MATCH Then
                    conditionType = SqlData.Condition.ConditionType.BackwardMatch
                ElseIf request.KeyValues(index).Operation = OPERATION_PARTICAL_MATCH Then
                    conditionType = SqlData.Condition.ConditionType.Include
                End If

                Dim condition As New LiteralConditionData(conditionType,
                                                          String.Format(TableSetting.FIELD_NAME_FORMAT, request.KeyValues(index).Index + 1),
                                                          request.KeyValues(index).Value)
                combineCondition.ConditionList.Add(condition)
            Next

            If 0 < combineCondition.ConditionList.Count Then
                command.ConditionList.Add(combineCondition)
            End If

            Return command
        End Function

        Private Shared Function ExecuteDbSelectFetch(ByVal handy As BTHandy2) As WaitResult

            Dim response As BTResponseDB2 = handy.Response

            Dim restResult As List(Of String()) = Nothing
            If Not _selectMultipleResults.TryGetValue(handy.Ip, restResult) Then
                SendResponse(handy, ERR_HRC_REQUEST)
                Return New WaitResult()
            End If

            Dim restCount = restResult.Count

            Dim setDataCount As Integer = Math.Min(MAX_RECORD_COUNT, restCount)
            response.ResultValues.Count = setDataCount

            For recordIndex As Integer = 0 To setDataCount - 1
                response.ResultValues(recordIndex).Index = 0

                Dim value As StringBuilder = New StringBuilder()
                For fieldIndex As Integer = 0 To restResult(recordIndex).Length - 1
                    If fieldIndex <> 0 Then
                        value.Append(",")
                    End If

                    value.Append(EncloseDoubleQuotations(restResult(recordIndex)(fieldIndex)))
                Next
                response.ResultValues(recordIndex).Value = value.ToString()
                response.ResultValues(recordIndex).Operation = 0
            Next

            If 0 < (restCount - setDataCount) Then
                restResult.RemoveRange(0, setDataCount)
                _selectMultipleResults(handy.Ip) = restResult
            Else
                _selectMultipleResults.Remove(handy.Ip)
            End If

            SendResponse(handy, restCount)
            Return New WaitResult()

        End Function


        Private Shared Function ExecuteDbInsert(ByVal handy As BTHandy2) As WaitResult

            Dim request As BTRequestDB2 = handy.Request

            Dim accessor As SQLiteAccessor = DbAccessManager.GetSQLiteAccessor(FileUtility.DbFilePath)
            Try
                accessor.BeginTransaction()

                Select Case request.Option(1)
                    Case REQUEST_OPT_INSERT_DATA

                        If Not ExecuteDbInsertData(request, handy.Ip) Then
                            accessor.RollbackTransaction()
                            SendResponse(handy, ERR_DB_ACCESS)
                            Return New WaitResult()
                        End If

                        SendResponse(handy, 0)

                    Case Else
                        accessor.RollbackTransaction()
                        SendResponse(handy, ERR_HRC_REQUEST)
                        Return New WaitResult()
                End Select

            Catch ex As DbAccessException
                ErrorUtility.OutputErrorLog(My.Resources.RegisterLogFailed, handy.Ip)
                accessor.RollbackTransaction()

                SendResponse(handy, ERR_DB_ACCESS)
                Return New WaitResult()
            End Try

            Return New WaitResult(True, FileUtility.DbFilePath, False)

        End Function

        Private Shared Function ExecuteDbInsertData(ByVal request As BTRequestDB2, ip As String) As Boolean

            Dim isExistSuccessItem As Boolean = False

            For i As Integer = 0 To request.NewValues.Count - 1

                Dim newValues As String() = Nothing
                Using reader As StringReader = New StringReader(request.NewValues(i).Value)
                    Using fieldParser As TextFieldParser = New TextFieldParser(reader)
                        fieldParser.Delimiters = New String() {","}
                        fieldParser.TrimWhiteSpace = False
                        If Not fieldParser.EndOfData Then
                            newValues = fieldParser.ReadFields()
                        End If
                    End Using
                End Using

                If IsNothing(newValues) Then
                    Return False
                End If

                Dim logData(0)() As String
                logData(0) = newValues

                Dim registResult As Result = LogProcessor.RegisterLog(logData, request.TableName, ip)

                If (registResult <> Result.Failure) Then
                    isExistSuccessItem = True
                End If
            Next

            Return isExistSuccessItem

        End Function

        Private Shared Sub SendResponse(ByVal handy As BTHandy2, ByVal resultValue As Integer)

            WriteLog(False, handy.Ip, handy.SeqNo, "SendResponse", resultValue.ToString(CultureInfo.InvariantCulture))

            handy.Response.Result = resultValue
            If handy.IsValid Then
                handy.SendResponse()
            End If
        End Sub

    End Class
End Namespace
