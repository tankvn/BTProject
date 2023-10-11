Namespace DbException
    Public Class DbAccessException
        Inherits Exception

        Public Sub New(ByVal exception As Exception)
            MyBase.New(exception.Message, exception)
        End Sub
    End Class
End Namespace
