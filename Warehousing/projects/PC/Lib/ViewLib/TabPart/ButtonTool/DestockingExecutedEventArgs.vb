Namespace TabPart.ButtonTool
    Public Class DestockingExecutedEventArgs
        Inherits EventArgs

        Public Property TableName As String

        Public Sub New(ByVal tableName As String)
            MyBase.New()
            _TableName = tableName
        End Sub
    End Class
End Namespace
