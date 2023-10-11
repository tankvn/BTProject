Imports Database.SqlData.Column
Imports Database.SqlData.Condition
Imports Database
Imports Database.DbException
Imports System.Globalization
Imports SettingLib.Part
Imports SettingLib
Imports Database.SqlData
Imports SettingLib.Table
Imports UtilityLib
Imports ProcessLib.My.Resources

Namespace PartProcess
    Public Class DestockingButtonProcess

        Private Const HISTORY_COLUMN_COUNT As Integer = 4

        Private Const HISTORY_TABLE_STRING As String = "_history"

        Private Const HISTORY_COLUMN_NAME_FORMAT As String = "history{0}"

        Private Const DIFFERENCE_COLUMN_NAME_FORMAT As String = "difference{0}"

        Public Shared Function AdjustInventory(ByVal tableName As String, ByVal setting As DestockingButtonSetting, ByVal masterSetting As MasterSetting) As ProcessResult
            Try
                Dim accessor As SQLiteAccessor = DbAccessManager.GetSQLiteAccessor(FileUtility.DbFilePath)
                Using manager As New TransactionManager(accessor)

                    Dim fieldNames As String() = accessor.GetFields(tableName)
                    If Not fieldNames.Contains(setting.ActualStockField) OrElse Not fieldNames.Contains(setting.TheoreticalStockField) Then Return ProcessResult.None

                    If FileUtility.Backup(masterSetting, accessor) <> ProcessResult.Success Then
                        Return ProcessResult.BackupFailed
                    End If

                    Dim historyColumnNames As String() = CreateHistoryColumnNames().ToArray()
                    Dim differenceColumnNames As String() = CreateDifferenceColumnNames().ToArray()
                    Dim historyTableName = tableName + HISTORY_TABLE_STRING
                    Dim historyData As String()() = DbAccessUtility.GetDataStrings(accessor, historyTableName, Nothing, Nothing)

                    MoveHistoryDisplayName(accessor, historyTableName, historyData, historyColumnNames)
                    MoveHistoryData(accessor, tableName, historyColumnNames, differenceColumnNames)
                    CreateHistory(accessor, tableName, historyColumnNames, differenceColumnNames, setting)
                    UpdateTheoreticalStock(accessor, tableName, setting)
                    SetNewName(accessor, historyTableName, historyData, historyColumnNames)

                    manager.Commit()
                End Using
            Catch ex As DbAccessException
                Return ProcessResult.DbAccessError
            Catch ex As Exception
                Return ProcessResult.UnknownError
            End Try

            Return ProcessResult.Success
        End Function

        Private Shared Sub UpdateTheoreticalStock(ByVal accessor As SQLiteAccessor, ByVal tableName As String, ByVal setting As DestockingButtonSetting)

            Dim adjustUpdateData As New UpdateDataList()
            adjustUpdateData.AddField(UpdateType.Assignment, setting.TheoreticalStockField, setting.ActualStockField)
            adjustUpdateData.AddLiteral(UpdateType.Assignment, setting.ActualStockField, 0)
            DbAccessUtility.UpdateDatabase(accessor, tableName, Nothing, adjustUpdateData)
        End Sub

        Private Shared Sub SetNewName(ByVal accessor As SQLiteAccessor, ByVal historyTableName As String, ByVal historyData As String()(), ByVal historyColumnNames As String())

            Dim newName As String = Date.Today.ToShortDateString()
            If Not IsNothing(historyData) AndAlso (0 < historyData.Length) Then
                newName = CheckOverlap(historyData(0), newName)
            End If
            Dim columnNameUpdateData As New UpdateDataList()
            columnNameUpdateData.AddLiteral(UpdateType.Assignment, historyColumnNames(0), newName)
            DbAccessUtility.UpdateDatabase(accessor, historyTableName, Nothing, columnNameUpdateData)
        End Sub

        Private Shared Sub CreateHistory(ByVal accessor As SQLiteAccessor, ByVal tableName As String, ByVal historyColumnNames As String(), ByVal differenceColumnNames As String(), ByVal setting As DestockingButtonSetting)

            Dim updateInventoryCommand As New UpdateHistoryCommand(tableName,
                                                                   setting.ActualStockField,
                                                                   setting.TheoreticalStockField,
                                                                   historyColumnNames(0),
                                                                   differenceColumnNames(0))
            accessor.ExecuteCommand(updateInventoryCommand)
        End Sub

        Private Shared Sub MoveHistoryData(ByVal accessor As SQLiteAccessor, ByVal tableName As String,
                                           ByVal historyColumnNames As String(), ByVal differenceColumnNames As String())

            Dim slideUpdateData As New UpdateDataList
            For index As Integer = 0 To HISTORY_COLUMN_COUNT - 2
                slideUpdateData.AddField(UpdateType.Assignment, historyColumnNames(index + 1), historyColumnNames(index))
                slideUpdateData.AddField(UpdateType.Assignment, differenceColumnNames(index + 1), differenceColumnNames(index))
            Next
            DbAccessUtility.UpdateDatabase(accessor, tableName, Nothing, slideUpdateData)
        End Sub

        Private Shared Sub MoveHistoryDisplayName(ByVal accessor As SQLiteAccessor, ByVal historyTableName As String, ByVal historyData As String()(), ByVal historyColumnNames As String())

            Dim updateHeader As New UpdateDataList
            Dim hasHistory As Boolean = False
            For index As Integer = 0 To HISTORY_COLUMN_COUNT - 2
                If Not historyData(0)(index).Contains(HistoryColumnDisplayNameFormat) Then
                    updateHeader.AddField(UpdateType.Assignment, historyColumnNames(index + 1), historyColumnNames(index))
                    hasHistory = True
                End If
            Next
            If hasHistory Then
                DbAccessUtility.UpdateDatabase(accessor, historyTableName, Nothing, updateHeader)
            End If
        End Sub

        Private Shared Function CheckOverlap(ByVal targetList As String(), ByVal checkString As String) As String
            Dim newName As String = checkString
            Dim nameIndex As Integer = 1
            If targetList(0).Contains(newName) Then
                Dim index As String = targetList(0).Replace(newName & "_", "")
                Dim parseResult As Integer
                If Integer.TryParse(index, parseResult) Then
                    nameIndex = parseResult + 1
                    newName = checkString + "_" + nameIndex.ToString(CultureInfo.InvariantCulture)
                End If
            End If

            Do While targetList.Contains(newName)
                newName = checkString + "_" + nameIndex.ToString(CultureInfo.InvariantCulture)
                nameIndex += 1
            Loop
            Return newName
        End Function


        Public Shared Iterator Function CreateHistoryColumnNames() As IEnumerable(Of String)
            For number As Integer = 1 To HISTORY_COLUMN_COUNT
                Yield String.Format(HISTORY_COLUMN_NAME_FORMAT, number.ToString(CultureInfo.InvariantCulture))
            Next
        End Function

        Private Shared Iterator Function CreateDifferenceColumnNames() As IEnumerable(Of String)
            For number As Integer = 1 To HISTORY_COLUMN_COUNT
                Yield String.Format(DIFFERENCE_COLUMN_NAME_FORMAT, number.ToString(CultureInfo.InvariantCulture))
            Next
        End Function

        Private Shared Iterator Function CreateDefaultHistoryDisplayNames() As IEnumerable(Of String)
            For number As Integer = 1 To HISTORY_COLUMN_COUNT
                Yield HistoryColumnDisplayNameFormat + number.ToString(CultureInfo.InvariantCulture)
            Next
        End Function

        Public Shared Function CreateHistoryTable(ByVal tableName As String) As ProcessResult
            Dim historyTableName = tableName + HISTORY_TABLE_STRING
            Dim accessor As SQLiteAccessor = DbAccessManager.GetSQLiteAccessor(FileUtility.DbFilePath)
            If DbAccessUtility.HasTable(accessor, historyTableName) Then Return ProcessResult.Success
            Dim columnHistoryDataList As ColumnDataList = New ColumnDataList
            Dim historyColumnNames = CreateHistoryColumnNames().ToArray()
            For Each historyColumnName As String In historyColumnNames
                columnHistoryDataList.Add(historyColumnName, DataType.Text)
            Next
            DbAccessUtility.CreateTable(FileUtility.DbFilePath, historyTableName, columnHistoryDataList, HISTORY_COLUMN_COUNT, String.Empty)
            DbAccessUtility.Insert(accessor, historyTableName, CreateDefaultHistoryDisplayNames().ToArray(), historyColumnNames)

            Return ProcessResult.Success
        End Function

        Public Shared Function LoadHistoryColumnNames(ByVal accessor As DbAccessor, ByVal tableName As String) As String()
            Dim historyTableName = tableName + HISTORY_TABLE_STRING
            If Not DbAccessUtility.HasTable(accessor, historyTableName) Then Return New String() {}
            Dim historyData As String()() = DbAccessUtility.GetDataStrings(accessor, historyTableName, Nothing, Nothing)
            If historyData.Length = 0 Then Return New String() {}
            Return historyData(0)
        End Function

        Public Shared Sub AddHistoryColumns(ByVal tableName As String)
            Dim accessor As SQLiteAccessor = DbAccessManager.GetSQLiteAccessor(FileUtility.DbFilePath)
            If Not DbAccessUtility.HasTable(accessor, tableName) Then Return

            Dim fieldNames = accessor.GetFields(tableName)

            Dim historyColumnNames As String() = CreateHistoryColumnNames().ToArray()
            Dim differenceColumnNames As String() = CreateDifferenceColumnNames().ToArray()

            For index As Integer = 0 To HISTORY_COLUMN_COUNT - 1
                If Not fieldNames.Contains(historyColumnNames(index)) Then
                    accessor.AddColumn(tableName, historyColumnNames(index), DataType.IntegerNumber)
                End If
                If Not fieldNames.Contains(differenceColumnNames(index)) Then
                    accessor.AddColumn(tableName, differenceColumnNames(index), DataType.IntegerNumber)
                End If
            Next
        End Sub

        Public Shared Function ResetHistoryColumnNames(ByVal accessor As DbAccessor, ByVal tableName As String) As ProcessResult
            Try
                Dim historyTableName = tableName + HISTORY_TABLE_STRING
                If Not DbAccessUtility.HasTable(accessor, historyTableName) Then Return ProcessResult.Success
                Dim updateCommand As New SqlUpdateCommand(historyTableName)
                Dim defaultDisplayNames = CreateDefaultHistoryDisplayNames().ToArray()
                Dim historyColumnNames = CreateHistoryColumnNames().ToArray()
                For index As Integer = 0 To historyColumnNames.Length - 1
                    updateCommand.UpdateDataList.AddLiteral(UpdateType.Assignment, historyColumnNames(index), defaultDisplayNames(index))
                Next
                accessor.ExecuteCommand(updateCommand)
                Return ProcessResult.Success
            Catch ex As Exception
                Return ProcessResult.RenameHistoryFailed
            End Try
        End Function
    End Class
End Namespace
