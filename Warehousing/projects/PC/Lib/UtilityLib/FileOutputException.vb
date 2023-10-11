
Public Class FileOutputException
    Inherits Exception

    Public Sub New(ByVal message As String, ByVal exception As Exception)
        MyBase.New(message, exception)
    End Sub
End Class
