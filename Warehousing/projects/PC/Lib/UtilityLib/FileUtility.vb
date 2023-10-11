Imports Database.SqlData.Condition
Imports Database
Imports System.IO
Imports System.Reflection
Imports System.Text.RegularExpressions
Imports System.Windows.Forms
Imports Database.DbException
Imports System.Globalization
Imports SettingLib.Table
Imports UtilityLib.My.Resources
Imports SettingLib

Public Class FileUtility
    Private Const TEMP_FILE_EXTENSION = ".tmp"

    Public Const IMPORT_FOLDER_PATH = "Import\"

    Public Const OUTPUT_FOLDER As String = "Export\"

    Public Const BACKUP_FOLDER_NAME = "Backup\"

    Private Const DB_FILE_PATH = "Dat\DataFile.dat"

    Public Shared ReadOnly Property DbFilePath() As String
        Get
            Return DB_FILE_PATH
        End Get
    End Property

    Public Const DATE_TIME_FORMAT As String = "yyyyMMddHHmmss"

    Public Const CSV_EXTENSION As String = ".csv"

    Private Const DOUBLE_QUOTE = """"

    Public Const MAX_BACKUP_FILE_NUM As Integer = 100

    Public Const MAX_OUTPUT_FILE_NUM As Integer = 100

    Public Shared Function GetDllDirectory() As String
        Dim uriString = Assembly.GetCallingAssembly().CodeBase
        Dim uri As New Uri(uriString)
        Return Path.GetDirectoryName(uri.LocalPath + uri.UnescapeDataString(uri.Fragment))
    End Function

    Public Shared Function IsDesignMode(ByVal baseDesignMode As Boolean) As Boolean
        Dim fileName = Path.GetFileName(Application.ExecutablePath)
        Return baseDesignMode OrElse
               fileName.Equals("devenv.exe", StringComparison.OrdinalIgnoreCase) OrElse
               fileName.Equals("vbexpress.exe", StringComparison.OrdinalIgnoreCase)
    End Function

    Public Shared Function Backup(ByVal masterSetting As MasterSetting, ByVal dbAccessor As DbAccessor) As ProcessResult
        Return Backup(masterSetting, dbAccessor, BACKUP_FOLDER_NAME)
    End Function

    Public Shared Function Backup(ByVal masterSetting As MasterSetting, ByVal dbAccessor As DbAccessor, ByVal folder As String) As ProcessResult
        Dim backUpFolder As String = Path.Combine(GetDllDirectory(), folder)
        Dim backUpFileName As String = backUpFolder + masterSetting.Name + "_" +
                                       DateTime.Now.ToString(DATE_TIME_FORMAT, CultureInfo.InvariantCulture) + CSV_EXTENSION
        If Not Directory.Exists(backUpFolder) Then
            Try
                Directory.CreateDirectory(backUpFolder)
            Catch ex As Exception
            End Try
        End If
        CheckQuantityOfFiles(Path.Combine(GetDllDirectory(), folder), masterSetting.Name, CSV_EXTENSION, MAX_BACKUP_FILE_NUM)
        Try
            Return SaveBackUp(backUpFileName, dbAccessor, masterSetting.TableName, New ConditionList())
        Catch ex As Exception
            ErrorUtility.OutputErrorLog(String.Format(ErrorLogBackUpFailed, Path.GetFileName(backUpFileName), ex.Message), String.Empty)
            Return ProcessResult.UnknownError
        End Try
    End Function

    Public Shared Sub CheckQuantityOfFiles(ByVal outputFolderName As String, ByVal fileName As String,
                                                 ByVal extension As String, ByVal maxFileCount As Integer)
        If Not Directory.Exists(outputFolderName) Then Return
        Dim files As String()
        Try
            files = Directory.GetFiles(outputFolderName, "*", SearchOption.TopDirectoryOnly)
        Catch ex As Exception
            Return
        End Try
        Dim escapedFolderName = Regex.Escape(outputFolderName)
        Dim escapedFileName = Regex.Escape(fileName)
        Dim escapedExtension = Regex.Escape(extension)
        Dim regex1 As New Regex(escapedFolderName + escapedFileName + "_\d{14}" + escapedExtension, RegexOptions.IgnoreCase)
        Dim sortList As New List(Of String)()
        For Each file As String In files
            If Not regex1.IsMatch(file) Then Continue For
            sortList.Add(file)
        Next
        sortList.Sort()
        While (maxFileCount <= sortList.Count)
            File.Delete(sortList(0))
            sortList.RemoveAt(0)
        End While
    End Sub

    Public Shared Sub DeleteFile(ByVal filePath As String, ByVal outputLog As Boolean)
        If String.IsNullOrEmpty(filePath) Then Return
        Try
            File.Delete(filePath)
        Catch ex As Exception
            If outputLog Then
                ErrorUtility.OutputErrorLog(String.Format(ErrorLogDeleteFileFailed, Path.GetFileName(filePath)), String.Empty)
            End If
        End Try
    End Sub

    Public Shared Function ToImportFullPath(ByVal fileName As String) As String
        Dim directory As String = Path.Combine(GetDllDirectory(), IMPORT_FOLDER_PATH)
        Return Path.Combine(directory, fileName.TrimStart("."c, "\"c))
    End Function

    Public Shared Sub DeleteDirectory(ByVal directoryPath As String)
        If String.IsNullOrEmpty(directoryPath) Then Return
        Try
            Directory.Delete(directoryPath, True)
        Catch ex As Exception
        End Try
    End Sub

    Public Shared Function GetTempFilePath(ByVal directory As String) As String
        Dim fileName As String
        Dim filePath As String
        Do
            fileName = Guid.NewGuid().ToString() + TEMP_FILE_EXTENSION
            filePath = Path.Combine(directory, fileName)
        Loop While File.Exists(filePath)
        Return filePath
    End Function

    Public Shared Function SaveBackUp(ByVal filePath As String, ByVal dbAccessor As DbAccessor,
                                      ByVal tableName As String, ByVal conditionList As ConditionList) As ProcessResult
        Try
            Dim dataList As String()() = DbAccessUtility.GetAllColumnStringData(dbAccessor, tableName, conditionList)
            Return ExecuteOutputFile(filePath, dataList, AppSetting.AddDoubleQuotes, False, False, New String() {})
        Catch ex As DbAccessException
            Throw New FileOutputException(String.Format(ErrorMessageFileAccessError, dbAccessor.DbFileName), ex)
        Catch ex As Exception
            Throw New FileOutputException(ex.Message, ex)
        End Try
    End Function

    Public Shared Function ExecuteOutputFile(ByVal filePath As String, ByVal data As String()(),
                                             ByVal addDoubleQuotes As Boolean, ByVal addRowNumber As Boolean,
                                             ByVal addColumnName As Boolean, ByVal columnNames As String()) As ProcessResult
        If IsNothing(data) OrElse String.IsNullOrEmpty(filePath) Then Return ProcessResult.UnknownError
        Try
            Dim directoryName As String = Path.GetDirectoryName(filePath)
            If String.IsNullOrEmpty(directoryName) Then
                directoryName = GetDllDirectory()
            End If

            If Not Directory.Exists(directoryName) Then
                Try
                    Directory.CreateDirectory(directoryName)
                Catch ex As DirectoryNotFoundException
                    Return ProcessResult.CreateDirectoryError
                End Try
            End If
        Catch ex As PathTooLongException
            Return ProcessResult.PathIsTooLong
        End Try
        Try
            Using streamWriter As StreamWriter = New StreamWriter(filePath, False, AppSetting.Encoding)
                If addColumnName Then
                    WriteColumnNames(columnNames, streamWriter, addDoubleQuotes, addRowNumber)
                End If
                WriteCsvString(data, streamWriter, addDoubleQuotes, addRowNumber)
            End Using
        Catch ex As Exception
            Return ProcessResult.FileAccessError
        End Try
        Return ProcessResult.Success
    End Function

    Private Shared Sub WriteColumnNames(ByVal columnNames As String(), ByVal streamWriter As StreamWriter, ByVal addDoubleQuotes As Boolean, ByVal addRowNumber As Boolean)
        If addRowNumber And 0 < columnNames.Length Then
            streamWriter.Write(AppSetting.Separator)
        End If
        For index As Integer = 0 To columnNames.Length - 1
            Dim nameString = columnNames(index)
            If addDoubleQuotes Then
                nameString = EncloseDoubleQuotation(nameString)
            End If
            streamWriter.Write(nameString)
            streamWriter.Write(If((index < (columnNames.Length - 1)), AppSetting.Separator, String.Empty))
        Next
        streamWriter.WriteLine()
    End Sub

    Private Shared Sub WriteCsvString(ByVal dataList As String()(), ByVal streamWriter As StreamWriter, ByVal addDoubleQuotes As Boolean, ByVal addRowNumber As Boolean)
        For rowIndex = 0 To dataList.Count - 1
            If addRowNumber Then
                Dim rowNumberString = (rowIndex + 1).ToString(CultureInfo.InvariantCulture)
                If addDoubleQuotes Then
                    rowNumberString = EncloseDoubleQuotation(rowNumberString)
                End If
                streamWriter.Write(rowNumberString)
                If (0 < dataList(rowIndex).Count) Then
                    streamWriter.Write(AppSetting.Separator)
                End If
            End If
            For itemIndex = 0 To dataList(rowIndex).Count - 1
                Dim itemString = dataList(rowIndex)(itemIndex).ToString()
                If addDoubleQuotes Then
                    itemString = EncloseDoubleQuotation(itemString)
                End If
                streamWriter.Write(itemString)
                streamWriter.Write(If((itemIndex < (dataList(rowIndex).Count - 1)), AppSetting.Separator, String.Empty))
            Next
            If Not rowIndex = dataList.Count - 1 Then
                streamWriter.WriteLine()
            End If
        Next
    End Sub

    Private Shared Function EncloseDoubleQuotation(ByVal inputString As String) As String
        If String.IsNullOrEmpty(inputString) Then
            Return String.Empty
        End If
        inputString = inputString.Replace(DOUBLE_QUOTE, DOUBLE_QUOTE + DOUBLE_QUOTE)
        Return (DOUBLE_QUOTE + inputString + DOUBLE_QUOTE)
    End Function
End Class
