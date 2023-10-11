
Public Class SearchHistoryData
    Public Property JobName() As String

    Public Property ControlName() As String

    Public Property HistoryString() As String

    Public Sub AddData(ByVal inputJobName As String, ByVal inputControlName As String, ByVal inputHistoryString As String)
        JobName = inputJobName
        ControlName = inputControlName
        HistoryString = inputHistoryString
    End Sub
End Class

