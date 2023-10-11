Imports Database

Public Class ImportFileConvertResult
    Private ReadOnly _filePath As String
    Public ReadOnly Property FilePath() As String
        Get
            Return _filePath
        End Get
    End Property

    Private ReadOnly _importResult As ImportResult
    Public ReadOnly Property ImportResult() As ImportResult
        Get
            Return _importResult
        End Get
    End Property

    Sub New(ByVal filePath As String, ByVal importResult As ImportResult)
        _filePath = filePath
        _importResult = ImportResult
    End Sub
End Class
