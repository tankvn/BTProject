Imports System.IO
Imports System.Windows.Forms
Imports Database
Imports Database.SqlData
Imports Database.SqlData.Condition
Imports Database.SqlData.DisplayData
Imports Database.Utility
Imports ProcessLib.PartProcess
Imports SettingLib
Imports SettingLib.Table
Imports UtilityLib
Imports ProcessLib.My.Resources

Namespace ControlProcess

    Public Class TableUpdateProcess
        Public Const BACKUP_FOLDER_NAME = "Backup\"

        Public Shared Function Import(ByVal dbAccessor As DbAccessor, ByVal masterSetting As MasterSetting,
                                      ByVal dataformatSettingList As DataFormatSettingList, ByVal updateMasterSettingList As UpdateMasterSettingList,
                                      ByVal showMessage As Boolean) As ProcessResult

            If HasUnprocessedData(masterSetting.Name, dataformatSettingList, updateMasterSettingList) Then
                If showMessage Then
                    ErrorUtility.OutputErrorLog(ExistUnprocessedData, String.Empty)
                End If
                Return ProcessResult.ExistUnprocessedData
            End If

            If Not File.Exists(FileUtility.ToImportFullPath(masterSetting.LoadFilePath)) Then Return ProcessResult.NoImportFile

            If FileUtility.Backup(masterSetting, dbAccessor) <> ProcessResult.Success Then Return ProcessResult.BackupFailed

            Dim result As ProcessResult = ImportFile(dbAccessor, masterSetting, showMessage)
            If result <> ProcessResult.Success Then
                Return ProcessResult.UnknownError
            End If
            Return DestockingButtonProcess.ResetHistoryColumnNames(dbAccessor, masterSetting.TableName)
        End Function

        Private Shared Function ImportFile(ByVal dbAccessor As DbAccessor, ByVal masterSetting As MasterSetting, ByVal showMessage As Boolean) As ProcessResult
            Dim workingDirectory = Path.GetDirectoryName(FileUtility.DbFilePath)
            Dim tempFilePath = FileUtility.GetTempFilePath(workingDirectory)
            Try
                dbAccessor.BeginTransaction()
                DbAccessUtility.DeleteAllRecord(dbAccessor, masterSetting.TableName)
                Dim converter As New ImportFileConverter(FileUtility.ToImportFullPath(masterSetting.LoadFilePath), masterSetting)
                Dim convertResult As ImportFileConvertResult = converter.ConvertImportFile(tempFilePath)
                Dim result As ImportResult = convertResult.ImportResult
                If (result And ImportResult.Failure) <> ImportResult.Failure Then
                    result = result Or dbAccessor.ImportCsv(dbAccessor, FileUtility.DbFilePath, convertResult.FilePath, masterSetting.TableName, AppSetting.Separator)
                End If
                Dim errorMessage As String = ErrorUtility.GetErrorMessage(result)
                If Not String.IsNullOrEmpty(errorMessage) Then
                    If ((result And ImportResult.Failure) = ImportResult.Failure) Then
                        If Not showMessage Then
                            Throw New Exception(errorMessage)
                        End If
                        MessageBox.Show(errorMessage, My.Application.Info.AssemblyName, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        ErrorUtility.OutputErrorLog(errorMessage, String.Empty)
                        dbAccessor.RollbackTransaction()
                        Return ProcessResult.UnknownError
                    End If

                    If showMessage Then
                        MessageBox.Show(errorMessage, My.Application.Info.AssemblyName, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                    ErrorUtility.OutputErrorLog(errorMessage, String.Empty)
                End If
                dbAccessor.CommitTransaction()
                BackupSourceFile(FileUtility.ToImportFullPath(masterSetting.LoadFilePath))
                Return ProcessResult.Success
            Catch ex As Exception
                dbAccessor.RollbackTransaction()
                Throw New Exception(ex.Message)
            Finally
                FileUtility.DeleteFile(tempFilePath, False)
            End Try
        End Function

        Private Shared Sub BackupSourceFile(ByVal importFilePath As String)

            Dim folder As String = Path.GetDirectoryName(importFilePath)
            Dim backupFolder As String = Path.Combine(folder, BACKUP_FOLDER_NAME)
            Dim fileName As String = Path.GetFileName(importFilePath)
            Dim backupFilePath = Path.Combine(backupFolder, fileName)
            Try
                Directory.CreateDirectory(backupFolder)
                My.Computer.FileSystem.MoveFile(importFilePath, backupFilePath, True)
            Catch ex As Exception
                ErrorUtility.OutputErrorLog(String.Format(ErrorLogBackupImportFileFailed, Path.GetFileName(importFilePath)), String.Empty)
                FileUtility.DeleteFile(importFilePath, True)
            End Try
        End Sub

        Public Shared Function HasUnprocessedData(ByVal masterName As String, ByVal logFormatSettingList As DataFormatSettingList, ByVal dbUpdateSettingList As UpdateMasterSettingList) As Boolean
            Dim list As List(Of UpdateMasterSetting) = dbUpdateSettingList.GetDbUpdateSettingList(masterName)
            For Each dbUpdateSetting As UpdateMasterSetting In list
                Dim logFormatSetting As DataFormatSetting = logFormatSettingList.GetByJobName(dbUpdateSetting.JobName)

                If IsNothing(logFormatSetting) Then Return False

                If Not File.Exists(FileUtility.DbFilePath) Then Return False

                Try
                    Dim accessor As SQLiteAccessor = DbAccessManager.GetSQLiteAccessor(FileUtility.DbFilePath)
                    Dim tables As String() = accessor.GetTables()
                    If Not tables.Contains(logFormatSetting.TableName) Then Return False

                    Dim command As New SqlSelectCommand(logFormatSetting.TableName)
                    command.DisplayDataList.Add(New ColumnDisplayData(LogUpdateCommand.LOG_STATUS_COLUMN_NAME))
                    Dim combineCondition As New CombineConditionData()
                    combineCondition.OrCombining = True
                    combineCondition.ConditionList.Add(New LiteralConditionData(ConditionType.ExactMatch, LogUpdateCommand.LOG_STATUS_COLUMN_NAME, GetResultString(DbUpdateResult.NG)))
                    combineCondition.ConditionList.Add(New LiteralConditionData(ConditionType.ExactMatch, LogUpdateCommand.LOG_STATUS_COLUMN_NAME, GetResultString(DbUpdateResult.Skip)))
                    combineCondition.ConditionList.Add(New LiteralConditionData(ConditionType.ExactMatch, LogUpdateCommand.LOG_STATUS_COLUMN_NAME, GetResultString(DbUpdateResult.Incomplete)))
                    command.ConditionList.Add(combineCondition)
                    Dim result = accessor.ExecuteCommandAndRead(command)
                    If (0 < result.Length) Then
                        Return True
                    End If
                Catch ex As Exception
                    Throw New Exception(String.Format(ErrorMessageFileAccessError, Path.GetFileName(FileUtility.DbFilePath)))
                End Try
            Next
            Return False
        End Function
    End Class
End Namespace
