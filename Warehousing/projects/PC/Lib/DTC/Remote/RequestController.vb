Imports btnaviReqHandler
Imports System.Threading
Imports UtilityLib

Namespace Remote
    Public Class RequestController

        Private Const RESULT_WAIT As Integer = 100
        Private Const RESULT_WAIT_COUNT As Integer = 30

        Private _requestTask As Task

        Private ReadOnly _results As New Dictionary(Of RequestKey, String)

        Private ReadOnly _resultResetEvents As New Dictionary(Of RequestKey, ManualResetEvent)

        Private ReadOnly _lockObject As New Object()

        Public Sub Start()
            _requestTask = Task.Factory.StartNew(Sub()
                                                 End Sub, TaskCreationOptions.LongRunning)
        End Sub

        Public Sub Wait()
            _requestTask.Wait()
        End Sub

        Public Sub AddRequest(ByVal ip As String, ByVal sequenceNo As UInteger, ByRef handy As BTHandy2)

            Dim request As New Request(ip, sequenceNo, handy)

            SyncLock _lockObject
                _results(request.Key) = String.Empty
                _resultResetEvents(request.Key) = New ManualResetEvent(False)
            End SyncLock

            _requestTask = _requestTask.ContinueWith(Sub(t)
                                                         ExecuteRequest(request)
                                                     End Sub)
        End Sub

        Public Sub AddResult(ByVal ip As String, ByVal sequenceNo As UInteger, ByVal result As String)
            Dim key As New RequestKey(ip, sequenceNo)

            SyncLock _lockObject
                If Not _results.ContainsKey(key) Then
                    Return
                End If
                _results(key) = result

                If Not _resultResetEvents.ContainsKey(key) Then
                    Return
                End If
                _resultResetEvents(key).Set()
            End SyncLock
        End Sub

        Public Sub Cancel(ByVal ip As String, ByVal sequenceNo As UInt32)
            Dim key As New RequestKey(ip, sequenceNo)

            SyncLock _lockObject
                If Not _resultResetEvents.ContainsKey(key) Then
                    Return
                End If
                _results(key) = "Cancel"

                If Not _resultResetEvents.ContainsKey(key) Then
                    Return
                End If
                _resultResetEvents(key).Set()
            End SyncLock
        End Sub

        Private Sub ExecuteRequest(ByVal request As Request)
            Try
                Dim nowResult As String = Nothing
                SyncLock _lockObject
                    If Not _results.ContainsKey(request.Key) Then
                        ErrorUtility.OutputErrorLog("Failed to get current result.", request.Key.Ip)
                        Return
                    End If
                    nowResult = _results(request.Key)
                End SyncLock
                If Not String.IsNullOrEmpty(nowResult) Then
                    Return
                End If

                Dim waitResult As WaitResult = request.DoAction()

                Dim resetEvent As ManualResetEvent = Nothing
                SyncLock _lockObject
                    If Not _resultResetEvents.ContainsKey(request.Key) Then
                        ErrorUtility.OutputErrorLog("Failed to get reset-event.", request.Key.Ip)
                        RemoteAccess.ResultAction(waitResult, "Error")
                        Return
                    End If
                    resetEvent = _resultResetEvents(request.Key)
                End SyncLock

                For i As Integer = 0 To RESULT_WAIT_COUNT
                    If resetEvent.WaitOne(RESULT_WAIT) Then
                        Exit For
                    End If

                    If i = RESULT_WAIT_COUNT Then
                        ErrorUtility.OutputErrorLog("Result did not reach.", request.Key.Ip)
                        RemoteAccess.ResultAction(waitResult, "Error")
                        Return
                    End If
                Next

                Dim result As String = Nothing
                SyncLock _lockObject
                    If Not _results.ContainsKey(request.Key) Then
                        ErrorUtility.OutputErrorLog("Failed to get result.", request.Key.Ip)
                        RemoteAccess.ResultAction(waitResult, "Error")
                        Return
                    End If
                    result = _results(request.Key)
                End SyncLock

                RemoteAccess.ResultAction(waitResult, result)

            Finally
                SyncLock _lockObject
                    _results.Remove(request.Key)
                    _resultResetEvents.Remove(request.Key)
                End SyncLock
            End Try
        End Sub

    End Class
End Namespace
