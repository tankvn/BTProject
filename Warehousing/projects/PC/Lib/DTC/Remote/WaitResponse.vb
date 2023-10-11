Namespace Remote
    Public Class WaitResponse
        Public Property IsWait() As Boolean

        Public Property DbPath() As String

        Public Sub New()
            Me.New(False, String.Empty)
        End Sub

        Public Sub New(ByVal wait As Boolean, ByVal path As String)
            IsWait = wait
            DbPath = path
        End Sub
    End Class
End Namespace
