Imports Database.SqlData.Condition
Imports Database
Imports System.IO
Imports System.Globalization
Imports System.Windows.Forms
Imports SettingLib
Imports SettingLib.Part
Imports Database.SqlData
Imports UtilityLib
Imports ProcessLib.My.Resources

Namespace PartProcess
    Public Class FileOutputButtonProcess

        Public Shared Function Execute(ByVal setting As FileOutputButtonSetting, ByVal sheetSetting As SheetSetting,
                                       ByVal conditionList As ConditionList, ByVal sortIndex As Integer, ByVal sortOrder As SortOrder) As ProcessResult
            Dim outputPath As String = OutputPreProcess(setting.OutputPath)
            If String.IsNullOrEmpty(outputPath) Then
                ErrorUtility.OutputErrorLog(ErrorLogOutputFileNameIsInvalid, String.Empty)
                Return ProcessResult.SettingsError
            End If
            Dim data As DataTable
            Try
                Dim accessor As SQLiteAccessor = DbAccessManager.GetSQLiteAccessor(FileUtility.DbFilePath)
                data = accessor.CreateBindingSource(GetSelectCommand(sheetSetting, conditionList)).DataSource
                data = TableDisplayGridProcess.SortDataTable(data, sortIndex, sortOrder)
            Catch ex As Exception
                Return ProcessResult.DbAccessError
            End Try
            Return FileUtility.ExecuteOutputFile(outputPath, ConvertUtility.ToStringArray(data), AppSetting.AddDoubleQuotes, sheetSetting.ShowRowNumber, sheetSetting.ShowColumnHeader, GetColumnNames(sheetSetting).ToArray())
        End Function

        Public Shared Function Execute(ByVal data As String()(), ByVal setting As FileOutputButtonSetting, ByVal sheetSetting As SheetSetting) As ProcessResult
            Dim outputPath As String = OutputPreProcess(setting.OutputPath)
            If String.IsNullOrEmpty(outputPath) Then
                ErrorUtility.OutputErrorLog(ErrorLogOutputFileNameIsInvalid, String.Empty)
                Return ProcessResult.SettingsError
            End If
            Return FileUtility.ExecuteOutputFile(outputPath, data, AppSetting.AddDoubleQuotes, sheetSetting.ShowRowNumber, sheetSetting.ShowColumnHeader, GetColumnNames(sheetSetting).ToArray())
        End Function

        Private Shared Iterator Function GetColumnNames(ByVal sheetSetting As SheetSetting) As IEnumerable(Of String)
            For Each columnSetting As DisplayColumnSetting In sheetSetting.DisplayColumnList
                Yield columnSetting.DisplayName
            Next
            If IsNothing(sheetSetting.HistoryColumnList) Then Return
            Dim accessor As DbAccessor = DbAccessManager.GetSQLiteAccessor(FileUtility.DbFilePath)
            Dim historyColumnNames As String() = DestockingButtonProcess.LoadHistoryColumnNames(accessor, sheetSetting.TableName)
            For index As Integer = 0 To sheetSetting.HistoryColumnList.Count - 1
                If index Mod 2 = 0 AndAlso index / 2 < historyColumnNames.Length Then
                    Yield historyColumnNames(index / 2)
                Else
                    Yield sheetSetting.HistoryColumnList(index).DisplayName
                End If
            Next
        End Function

        Private Shared Function OutputPreProcess(ByVal fileName As String) As String
            Dim outputFileName As String
            Try
                outputFileName = IIf(fileName.EndsWith(FileUtility.CSV_EXTENSION, StringComparison.OrdinalIgnoreCase),
                                     Path.GetFileNameWithoutExtension(fileName),
                                     Path.GetFileName(fileName))
            Catch ex As Exception
                Return Nothing
            End Try

            Dim outputFileDirectory = String.Empty
            Try
                outputFileDirectory = Path.GetDirectoryName(fileName)
            Catch ex As Exception
            End Try

            Dim fullFileName = outputFileName + "_" + DateTime.Now.ToString(FileUtility.DATE_TIME_FORMAT, CultureInfo.InvariantCulture) + FileUtility.CSV_EXTENSION
            Dim outputDirectory As String = Nothing
            Try
                outputDirectory = If(Path.IsPathRooted(fileName),
                                       Path.GetDirectoryName(fileName),
                                       Path.Combine(FileUtility.GetDllDirectory(), FileUtility.OUTPUT_FOLDER))
            Catch ex As Exception
            End Try
            If Not String.IsNullOrEmpty(outputFileDirectory) Then
                outputDirectory = Path.Combine(outputDirectory, outputFileDirectory)
            End If
            Try
                If IsNothing(outputDirectory) Then
                    outputDirectory = Path.GetPathRoot(fileName)
                End If
            Catch ex As Exception
                Return Nothing
            End Try
            Dim outputPath = Path.Combine(outputDirectory, fullFileName)
            Try
                Directory.CreateDirectory(outputDirectory)
            Catch ex As Exception
            End Try
            FileUtility.CheckQuantityOfFiles(outputDirectory, Path.GetFileNameWithoutExtension(outputFileName), FileUtility.CSV_EXTENSION, FileUtility.MAX_OUTPUT_FILE_NUM)
            Return outputPath
        End Function

        Private Shared Function GetSelectCommand(ByVal sheetSetting As SheetSetting, ByVal conditionList As ConditionList) As SqlSelectCommand

            Dim selectCommand As New SqlSelectCommand(sheetSetting.TableName)
            selectCommand.DisplayDataList.AddRange(ConvertUtility.ToDisplayDataList(sheetSetting.DisplayColumnList, sheetSetting.CalculationColumnList))
            If Not IsNothing(sheetSetting.HistoryColumnList) Then
                selectCommand.DisplayDataList.AddRange(ConvertUtility.ToDisplayDataList(sheetSetting.HistoryColumnList, sheetSetting.CalculationColumnList))
            End If
            If Not IsNothing(conditionList) Then
                selectCommand.ConditionList.AddRange(conditionList)
            End If
            Return selectCommand
        End Function

    End Class
End Namespace
