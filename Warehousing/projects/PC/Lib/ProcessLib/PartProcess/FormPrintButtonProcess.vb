Imports System.Drawing.Printing
Imports System.Windows.Forms
Imports Database.SqlData.Condition
Imports Database.SqlData.DisplayData
Imports SettingLib.Control
Imports Database
Imports SettingLib.Part
Imports SettingLib
Imports Database.SqlData
Imports SettingLib.Table
Imports UtilityLib
Imports ProcessLib.My.Resources

Namespace PartProcess
    Public Class FormPrintButtonProcess

        Private Const DEFAULT_COLUMN_WIDTH = 100

        Public Shared Function Execute(ByVal data As DataTable, ByVal setting As FormPrintButtonSetting, ByVal showRowNumber As Boolean,
                                       ByVal showColumnHeader As Boolean) As ProcessResult

            Using processor As New PrintProcessor(data, setting, showRowNumber, showColumnHeader), printDocument As New PrintDocument()
                AddHandler printDocument.PrintPage, AddressOf processor.PrintDocument_PrintPage
                Using printDialog As New PrintDialog
                    printDialog.Document = printDocument
                    Try
                        If printDialog.ShowDialog() = DialogResult.OK Then
                            printDocument.Print()
                            Return ProcessResult.Success
                        End If
                        Return ProcessResult.Canceled
                    Catch ex As Exception
                        Return ProcessResult.PrinterError
                    End Try
                End Using
            End Using
        End Function

        Public Shared Function Execute(ByVal setting As FormPrintButtonSetting, ByVal sheetSetting As SheetSetting,
                                       ByVal tableSetting As TableSetting, ByVal columnWidths As Integer(),
                                       ByVal conditionList As ConditionList, ByVal sortIndex As Integer, ByVal sortOrder As SortOrder) As ProcessResult
            Dim data As DataTable
            Try
                Dim correctedSortIndex As Integer = CorrectSortIndex(setting, sheetSetting, sortIndex)
                If correctedSortIndex = setting.PrintColumnSettings.Count Then
                    AddSortColumnData(setting, sheetSetting, sortIndex)
                End If
                Dim accessor As SQLiteAccessor = DbAccessManager.GetSQLiteAccessor(FileUtility.DbFilePath)
                Select Case setting.PrintFormat
                    Case PrintFormat.Table
                        data = accessor.CreateBindingSource(GetSelectCommand(sheetSetting, conditionList)).DataSource
                        setting.PrintColumnSettings = CreatePrintColumnSettings(sheetSetting, tableSetting, columnWidths)
                    Case PrintFormat.Slip
                        If (Not accessor.GetFields(sheetSetting.TableName).Contains(setting.SlipColumnName)) AndAlso
                            (Not sheetSetting.CalculationColumnList.Any(Function(calc) calc.Name = setting.SlipColumnName)) Then
                            ErrorUtility.OutputErrorLog(PrintSettingsError, String.Empty)
                            Return ProcessResult.SettingsError
                        End If
                        data = accessor.CreateBindingSource(GetSelectCommand(setting, sheetSetting, conditionList)).DataSource
                    Case Else
                        Throw New InvalidProgramException
                End Select

                data = TableDisplayGridProcess.SortDataTable(data, correctedSortIndex, sortOrder)
            Catch ex As Exception
                Return ProcessResult.DbAccessError
            End Try

            If data.Rows.Count = 0 Then Return ProcessResult.NoData
            Return Execute(data, setting, sheetSetting.ShowRowNumber, sheetSetting.ShowColumnHeader)
        End Function

        Private Shared Sub AddSortColumnData(ByVal setting As FormPrintButtonSetting, ByVal sheetSetting As SheetSetting, ByVal sortIndex As Integer)

            setting.PrintColumnSettings.Add(New PrintColumnSetting() With {
                                               .Name = GetSortTargetName(sheetSetting, sortIndex),
                                               .PrintLocation = PrintLocation.None})
        End Sub

        Private Shared Function CorrectSortIndex(ByVal setting As FormPrintButtonSetting, ByVal sheetSetting As SheetSetting, ByVal sortIndex As Integer) As Integer
            If (sortIndex = -1) OrElse (setting.PrintFormat = PrintFormat.Table) Then Return sortIndex
            Dim sortTargetName As String = GetSortTargetName(sheetSetting, sortIndex)
            For index As Integer = 0 To setting.PrintColumnSettings.Count - 1
                If setting.PrintColumnSettings(index).Name = sortTargetName Then Return index
            Next
            Return setting.PrintColumnSettings.Count
        End Function

        Private Shared Function CreateColumnWidths(ByVal columnWidths As Integer(), ByVal setting As FormPrintButtonSetting, ByVal sheetSetting As SheetSetting) As Integer()
            If setting.PrintFormat = PrintFormat.Table Then Return columnWidths
            Dim list As New List(Of Integer)()
            Dim currentWidthIndex = 0
            Dim printColumnNames As List(Of String) = setting.PrintColumnSettings.Select(Function(columnSetting) columnSetting.Name).ToList()
            For Each printColumnSetting As PrintColumnSetting In setting.PrintColumnSettings
                If printColumnSetting.PrintLocation = PrintLocation.Header OrElse
                    printColumnSetting.PrintLocation = PrintLocation.None Then
                    currentWidthIndex += 1
                    list.Add(0)
                    Continue For
                End If
                Do While (currentWidthIndex < sheetSetting.DisplayColumnList.Count) AndAlso
                    Not printColumnNames.Contains(sheetSetting.DisplayColumnList(currentWidthIndex).Name)
                    currentWidthIndex += 1
                Loop

                Dim hasDisplayColumnSetting = sheetSetting.DisplayColumnList.Any(Function(column) column.Name.Equals(printColumnSetting.Name))
                Dim hasHistoryColumnSetting = Not IsNothing(sheetSetting.HistoryColumnList) AndAlso sheetSetting.HistoryColumnList.Any(Function(column) column.Name.Equals(printColumnSetting.Name))
                If Not ((hasDisplayColumnSetting) OrElse (hasHistoryColumnSetting)) OrElse
                    (columnWidths.Length <= currentWidthIndex) Then
                    list.Add(DEFAULT_COLUMN_WIDTH)
                    Continue For
                End If
                list.Add(columnWidths(currentWidthIndex))
                currentWidthIndex += 1
            Next
            Return list.ToArray()
        End Function

        Private Shared Function CreatePrintColumnSettings(ByVal sheetSetting As SheetSetting, ByVal tableSetting As TableSetting, ByVal columnWidths As Integer()) As List(Of PrintColumnSetting)
            Dim list As New List(Of PrintColumnSetting)
            For index As Integer = 0 To sheetSetting.DisplayColumnList.Count - 1
                Dim printSetting As New PrintColumnSetting()
                printSetting.Name = sheetSetting.DisplayColumnList(index).Name
                printSetting.DisplayName = sheetSetting.DisplayColumnList(index).DisplayName
                printSetting.Type = GetDataType(printSetting, tableSetting)
                printSetting.PrintLocation = PrintLocation.List
                printSetting.AddBarcode = False
                printSetting.WidthWeight = columnWidths(index)
                list.Add(printSetting)
            Next
            Dim displayColumnListCount As Integer = sheetSetting.DisplayColumnList.Count

            If Not IsNothing(sheetSetting.HistoryColumnList) Then
                Dim accessor As SQLiteAccessor = DbAccessManager.GetSQLiteAccessor(FileUtility.DbFilePath)
                Dim names As String() = DestockingButtonProcess.LoadHistoryColumnNames(accessor, sheetSetting.TableName)
                For index As Integer = 0 To sheetSetting.HistoryColumnList.Count - 1
                    Dim printSetting As New PrintColumnSetting
                    printSetting.Name = sheetSetting.HistoryColumnList(index).Name
                    printSetting.DisplayName = If((index Mod 2 = 0) AndAlso (index / 2 < names.Count),
                                                  names(index / 2),
                                                  sheetSetting.HistoryColumnList(index).DisplayName)
                    printSetting.Type = DataType.IntegerNumber
                    printSetting.PrintLocation = PrintLocation.List
                    printSetting.AddBarcode = False
                    printSetting.WidthWeight = columnWidths(index + displayColumnListCount)
                    list.Add(printSetting)
                Next
            End If
            Return list
        End Function

        Private Shared Function GetDataType(ByVal printSetting As PrintColumnSetting, ByVal tableSetting As TableSetting) As DataType
            For Each tableColumnSetting As ITableColumnSetting In tableSetting.ColumnList
                If tableColumnSetting.Name = printSetting.Name Then Return tableColumnSetting.DataType
            Next
            Return DataType.FloatNumber
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

        Private Shared Function GetSelectCommand(ByVal setting As FormPrintButtonSetting, ByVal sheetSetting As SheetSetting,
                                                 ByVal conditionList As ConditionList) As SqlSelectCommand

            Dim selectCommand As New SqlSelectCommand(sheetSetting.TableName)
            For Each columnSetting As PrintColumnSetting In setting.PrintColumnSettings
                Dim calculationColumn = GetCalculationColumn(sheetSetting, columnSetting)
                If Not IsNothing(calculationColumn) Then
                    selectCommand.DisplayDataList.Add(calculationColumn)
                    Continue For
                End If
                selectCommand.DisplayDataList.AddColumn(columnSetting.Name)
            Next
            If Not IsNothing(conditionList) Then
                selectCommand.ConditionList.AddRange(conditionList)
            End If
            Return selectCommand
        End Function

        Private Shared Function GetSortTargetName(ByVal sheetSetting As SheetSetting, ByVal sortIndex As Integer) As String
            If sortIndex < sheetSetting.DisplayColumnList.Count Then
                Return sheetSetting.DisplayColumnList(sortIndex).Name
            End If
            If (sortIndex - sheetSetting.DisplayColumnList.Count) < sheetSetting.HistoryColumnList.Count Then
                Return sheetSetting.HistoryColumnList(sortIndex - sheetSetting.DisplayColumnList.Count).Name
            End If
            Return -1
        End Function

        Private Shared Function GetCalculationColumn(ByVal sheetSetting As SheetSetting, ByVal columnSetting As PrintColumnSetting) As ICalculationColumnDisplayData
            For Each calculationColumn As ICalculationColumnDisplayData In sheetSetting.CalculationColumnList.DataList
                If calculationColumn.Name = columnSetting.Name Then
                    Return calculationColumn
                End If
            Next
            Return Nothing
        End Function
    End Class
End Namespace
