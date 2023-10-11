Namespace DbException
    Public Class ExecuteSqlite3Exception
        Inherits Exception

        Public Sub New(ByVal inner As Exception)
            MyBase.new("It failed to execute sqlite3.exe", inner)
        End Sub
    End Class
End NameSpace
