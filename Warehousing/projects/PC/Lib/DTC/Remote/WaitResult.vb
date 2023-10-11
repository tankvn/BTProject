Namespace Remote
    Public Class WaitResult
        Public Property IsWait() As Boolean

        Public Property DbPath() As String

        Public Property NeedClose() As Boolean

        Public Sub New()
            Me.New(False, String.Empty, False)
        End Sub

        Public Sub New(ByVal wait As Boolean, ByVal path As String, ByVal close As Boolean)
            IsWait = wait
            DbPath = path
            NeedClose = close
        End Sub
    End Class
End Namespace
