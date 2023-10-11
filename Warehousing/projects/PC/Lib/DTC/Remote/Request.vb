Imports btnaviReqHandler

Namespace Remote
    Public Class Request
        Public Property Key() As RequestKey

        Private _handy As BTHandy2

        Public Sub New(ByVal ip As String, ByVal seq As UInteger, ByRef handy As BTHandy2)
            Key = New RequestKey(ip, seq)
            _handy = handy
        End Sub

        Public Function DoAction() As WaitResult
            Return RemoteAccess.RequestArrival(_handy)
        End Function

    End Class
End Namespace
