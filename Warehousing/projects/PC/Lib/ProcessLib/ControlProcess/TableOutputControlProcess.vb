Imports System.Drawing.Printing
Imports System.Globalization
Imports System.IO
Imports System.Windows.Forms
Imports Database
Imports Database.SqlData
Imports ProcessLib.PartProcess
Imports SettingLib
Imports SettingLib.Control
Imports SettingLib.Part
Imports SettingLib.Table
Imports UtilityLib

Namespace ControlProcess

    Public Class TableOutputControlProcess
        Public Shared Function Execute(ByVal setting As TableOutputControlSetting, ByVal sheetSetting As SheetSetting,
                                       ByVal columnWidths As Integer()) As ProcessResult
            If setting.OutputType = OutputType.File Then
                Return ExecuteOutputFile(setting, sheetSetting)
            End If
            Return ExecutePrint(setting, sheetSetting, columnWidths)
        End Function

        Public Shared Function Execute(ByVal setting As TableOutputControlSetting, ByVal sheetSetting As SheetSetting,
                                       ByVal data As DataTable, ByVal columnWidths As Integer()) As ProcessResult
            If IsNothing(data) Then
                Execute(setting, sheetSetting, columnWidths)
            End If

            If setting.OutputType = OutputType.File Then
                Return FileUtility.ExecuteOutputFile(OutputPreProcess(setting.FileName), ConvertUtility.ToStringArray(data), setting.AddDoubleQuotes)
            End If
            Return ExecutePrint(data, setting, columnWidths)
        End Function

        Private Shared Function ExecutePrint(ByVal data As DataTable, ByVal setting As TableOutputControlSetting, ByVal columnWidths As Integer()) As ProcessResult

            Using processor As New PrintProcessor(data, setting, columnWidths.ToList()), printDocument As New PrintDocument()
                AddHandler printDocument.PrintPage, AddressOf processor.PrintDocument_PrintPage
                Using printDialog As New PrintDialog
                    printDialog.Document = printDocument
                    If printDialog.ShowDialog() = DialogResult.OK Then
                        printDocument.Print()
                    End If
                End Using
            End Using
            Return ProcessResult.Success
        End Function

        Private Shared Function ExecutePrint(ByVal setting As TableOutputControlSetting, ByVal sheetSetting As SheetSetting, ByVal columnWidth As Integer()) As ProcessResult
            Dim data As DataTable
            Try
                Dim accessor As SQLiteAccessor = DbAccessManager.GetSQLiteAccessor(FileUtility.DbFilePath)
                data = accessor.CreateBindingSource(GetSelectCommand(sheetSetting)).DataSource
            Catch ex As Exception
                Return ProcessResult.DbAccessError
            End Try
            Return ExecutePrint(data, setting, columnWidth)
        End Function

        Private Shared Function GetSelectCommand(ByVal sheetSetting As SheetSetting) As SqlSelectCommand

            Dim selectCommand As New SqlSelectCommand(sheetSetting.TableName)
            selectCommand.DisplayDataList.AddRange(ConvertUtility.ToDisplayDataList(sheetSetting.DisplayColumnList, sheetSetting.CalculationColumnList))
            selectCommand.DisplayDataList.AddRange(sheetSetting.CalculationColumnList.GetVisibleData())
            selectCommand.ConditionList.AddRange(BtSheetProcess.CreateSearchCondition(sheetSetting))
            Return selectCommand
        End Function

        Private Shared Function ExecuteOutputFile(ByVal setting As TableOutputControlSetting, ByVal sheetSetting As SheetSetting) As ProcessResult
            Dim outputPath As String = OutputPreProcess(setting.FileName)
            Dim data As String()()
            Try
                Dim accessor As SQLiteAccessor = DbAccessManager.GetSQLiteAccessor(FileUtility.DbFilePath)
                data = accessor.ExecuteCommandAndRead(GetSelectCommand(sheetSetting))
            Catch ex As Exception
                Return ProcessResult.DbAccessError
            End Try
            Return FileUtility.ExecuteOutputFile(outputPath, data, setting.AddDoubleQuotes)
        End Function

        Private Shared Function OutputPreProcess(ByVal fileName As String) As String
            Dim outputFileName = Path.GetFileNameWithoutExtension(fileName)
            Dim fullFileName = outputFileName + DateTime.Now.ToString(FileUtility.DATE_TIME_FORMAT, CultureInfo.InvariantCulture) + FileUtility.CSV_EXTENSION
            Dim outputDirectory = If(Path.IsPathRooted(fileName),
                                   Path.GetDirectoryName(outputFileName),
                                   Path.Combine(FileUtility.GetDllDirectory(), FileUtility.OUTPUT_FOLDER))
            Dim outputPath = Path.Combine(outputDirectory, fullFileName)
            Directory.CreateDirectory(outputDirectory)
            FileUtility.CheckQuantityOfBackupFiles(outputDirectory, Path.GetFileNameWithoutExtension(outputFileName), FileUtility.CSV_EXTENSION, FileUtility.MAX_OUTPUT_FILE_NUM)
            Return outputPath
        End Function
    End Class
End Namespace
