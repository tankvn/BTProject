Imports System.IO

Namespace Utility
    Public Module FileUtility
        Private Const TEMP_FILE_EXTENSION = ".tmp"

        Public Function GetTempFilePath(ByVal directory As String) As String
            Dim fileName As String
            Dim filePath As String
            Do
                fileName = Guid.NewGuid().ToString() + TEMP_FILE_EXTENSION
                filePath = Path.Combine(directory, fileName)
            Loop While File.Exists(filePath)
            Return filePath
        End Function

        Public Sub DeleteFile(ByVal filePath As String)
            If String.IsNullOrEmpty(filePath) Then Return
            Try
                File.Delete(filePath)
            Catch ex As Exception
            End Try
        End Sub

        Public Sub DeleteDirectory(ByVal directoryPath As String)
            If String.IsNullOrEmpty(directoryPath) Then Return
            Try
                Directory.Delete(directoryPath, True)
            Catch ex As Exception
            End Try
        End Sub
    End Module
End Namespace

