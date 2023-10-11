Namespace DbException
    Public Class SqlCommandException
        Inherits DbAccessException

        Public Sub New(ByVal exception As Exception)
            MyBase.New(exception)
        End Sub
    End Class
End Namespace
