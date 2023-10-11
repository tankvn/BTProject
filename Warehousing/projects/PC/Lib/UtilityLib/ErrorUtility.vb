Imports Database
Imports System.IO
Imports System.Text
Imports System.Globalization
Imports Microsoft.VisualBasic.FileIO
Imports System.Text.RegularExpressions
Imports UtilityLib.My.Resources

Public Class ErrorUtility
    Private Const LOG_FILE_NAME As String = "ErrorLog.txt"

    Private Const ERROR_FILE_FOLDER_NAME As String = "ErrorFiles"

    Private Const ERROR_FILE_SEPARATOR As String = ","

    Public Shared Sub OutputErrorLog(ByVal errorText As String, ByVal terminalName As String)
        Dim lineString As String = CreateLogLineString(errorText, terminalName)
        Dim logFilePath As String = Path.Combine(FileUtility.GetDllDirectory(), LOG_FILE_NAME)
        Try
            Using writer As New StreamWriter(logFilePath, True, Encoding.UTF8)
                writer.WriteLine(lineString)
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Public Shared Sub OutputErrorLogForDataImport(ByVal errorText As String, ByVal terminalName As String, ByVal filePath As String)
        Dim errorPathText = Path.Combine(ERROR_FILE_FOLDER_NAME, Path.GetFileName(filePath))
        Dim logText As String = If(String.IsNullOrEmpty(filePath),
                                   errorText,
                                   String.Format(ErrorLogCaptureFailed, errorPathText, errorText)).ToString()
        OutputErrorLog(logText, terminalName)
    End Sub

    Private Shared Function CreateLogLineString(ByVal errorText As String, ByVal terminalName As String) As String
        Dim writeString As New StringBuilder()
        Dim now As DateTime = DateTime.Now
        writeString.Append("S" + vbTab)
        writeString.Append(now.ToShortDateString())
        writeString.Append(vbTab)
        writeString.Append(now.ToLongTimeString())
        writeString.Append(vbTab)
        writeString.Append(terminalName)
        writeString.Append(vbTab)
        writeString.Append(errorText.Replace(vbCrLf, vbLf).Replace(vbCr, vbLf).Replace(vbLf, " "))
        Return writeString.ToString()
    End Function

    Public Shared Sub CreateErrorFile(ByVal logFileName As String, ByVal logData As String()(),
                                      ByVal isRetransmission As Boolean)
        If isRetransmission Then Return

        Dim errorFileFolder = GetErrorFileFolder()
        If Not Directory.Exists(errorFileFolder) Then
            Directory.CreateDirectory(errorFileFolder)
        End If
        Try
            CreateErrorFile(logData, GetErrorFilePath(logFileName))
        Catch ex As Exception
            OutputErrorLog(ErrorMessageCreateErrorFile + " : " + ex.Message, String.Empty)
        End Try
    End Sub

    Private Shared Sub CreateErrorFile(ByVal logData As String()(), ByVal filePath As String)
        Using stream As New FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read)
            Using writer As New StreamWriter(stream, Encoding.GetEncoding("shift_jis"))
                For Each fields As String() In logData
                    Dim lineString As New StringBuilder()
                    For Each field As String In fields
                        lineString.Append(BasicUtility.EncloseInQuotes(field))
                        lineString.Append(ERROR_FILE_SEPARATOR)
                    Next
                    lineString.Remove(lineString.Length - 1, 1)
                    writer.WriteLine(lineString.ToString())
                Next
            End Using
        End Using
    End Sub

    Public Shared Function ReadErrorFile(ByVal filePath As String) As String()()
        Try
            Using stream As New FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                Dim fieldsList As New List(Of String())
                Using parser As New TextFieldParser(stream, Encoding.GetEncoding("shift_jis"))
                    parser.HasFieldsEnclosedInQuotes = True
                    parser.TrimWhiteSpace = False
                    parser.SetDelimiters(ERROR_FILE_SEPARATOR)
                    While (Not parser.EndOfData)
                        fieldsList.Add(parser.ReadFields())
                    End While
                End Using
                Return fieldsList.ToArray()
            End Using
        Catch ex As UnauthorizedAccessException
            Throw New Exception(ErrorMessageImportAccessDenied)
        Catch ex As MalformedLineException
            Throw New Exception(ErrorLogFormatError)
        Catch ex As Exception
            Throw New Exception(ErrorMessageImportFailure)
        End Try
    End Function

    Private Shared Function GetErrorFilePath(ByVal logFileName As String) As String
        Dim fileName As String = Path.GetFileNameWithoutExtension(logFileName)
        Dim fileExtension As String = Path.GetExtension(logFileName)
        fileName = fileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture)
        Dim fileFullName = fileName + fileExtension
        Dim filePath As String = Path.Combine(GetErrorFileFolder(), fileFullName)
        Dim index = 1
        While File.Exists(filePath)
            Dim fileNameWithIndex = fileName + "_" + index.ToString()
            fileFullName = fileNameWithIndex + fileExtension
            filePath = Path.Combine(GetErrorFileFolder(), fileFullName)
            index += 1
        End While
        Return filePath
    End Function

    Public Shared Function GetErrorFileFolder() As String
        Return Path.Combine(FileUtility.GetDllDirectory(), ERROR_FILE_FOLDER_NAME)
    End Function

    Public Shared Function GetLogFileName(ByVal errorFileName As String) As String
        Dim fileName As String = Path.GetFileNameWithoutExtension(errorFileName)
        Dim fileExtension As String = Path.GetExtension(errorFileName)
        Dim match As Match = Regex.Match(fileName, "(?<FileName>.+)_\d{14}_\d+$")
        If match.Success Then
            Return match.Groups("FileName").Value + fileExtension
        End If
        Dim logFileName As String = Regex.Replace(fileName, "(?<FileName>.+)_\d{14}$", "${FileName}")
        Return logFileName + fileExtension
    End Function

    Public Shared Function GetErrorMessage(ByVal result As ImportResult) As String
        Dim errorMessage As New StringBuilder()
        If (result And ImportResult.Failure) = ImportResult.Failure Then
            errorMessage.AppendLine(ErrorMessageImportFailure)
        End If
        If (result And ImportResult.FileNotFound) = ImportResult.FileNotFound Then
            errorMessage.AppendLine(ErrorMessageImportFileNotFound)
        End If
        If (result And ImportResult.Duplication) = ImportResult.Duplication Then
            errorMessage.AppendLine(ErrorMessageImportDuplication)
        End If
        If (result And ImportResult.ColumnExceedance) = ImportResult.ColumnExceedance Then
            errorMessage.AppendLine(ErrorMessageImportColumnExceedance)
        End If
        If (result And ImportResult.Converted) = ImportResult.Converted Then
            errorMessage.AppendLine(ErrorMessageImportConverted)
        End If
        If (result And ImportResult.Truncated) = ImportResult.Truncated Then
            errorMessage.AppendLine(ErrorMessageImportTruncated)
        End If
        If (result And ImportResult.AccessDenied) = ImportResult.AccessDenied Then
            errorMessage.AppendLine(ErrorMessageImportAccessDenied)
        End If
        If (result And ImportResult.ExceedByteLength) = ImportResult.ExceedByteLength Then
            errorMessage.AppendLine(ErrorMessageImportExceedByteLength)
        End If
        If (result And ImportResult.Overflow) = ImportResult.Overflow Then
            errorMessage.AppendLine(ErrorMessageImportOverflow)
        End If
        Return errorMessage.ToString()
    End Function
End Class
