Imports Database.SqlData.Condition
Imports Database
Imports Database.DbException
Imports SettingLib.Part
Imports SettingLib
Imports Database.SqlData
Imports UtilityLib

Namespace PartProcess
    Public Class AdjustInventoryButtonProcess

        Public Shared Function AdjustInventory(ByVal tableName As String, ByVal setting As AdjustInventoryButtonSetting,
                                               ByVal historyColumnName As String, ByVal differenceColumnName As String) As ProcessResult
            Try
                Dim accessor As SQLiteAccessor = DbAccessManager.GetSQLiteAccessor(FileUtility.DbFilePath),
                    manager As New TransactionManager(accessor)

                Dim updateInventoryCommand As New UpdateHistoryCommand(tableName,
                                                                       setting.ActualStockField,
                                                                       setting.TheoreticalStockField,
                                                                       historyColumnName,
                                                                       differenceColumnName)
                accessor.ExecuteCommand(updateInventoryCommand)

                manager.Commit()
            Catch ex As DbAccessException
                Return ProcessResult.DbAccessError
            Catch ex As Exception
                Return ProcessResult.UnknownError
            End Try

            Return ProcessResult.Success
        End Function

        Public Shared Function ClearInventory(ByVal tableName As String, ByVal actualStockField As String) As ProcessResult
            Try
                Dim accessor As SQLiteAccessor = DbAccessManager.GetSQLiteAccessor(FileUtility.DbFilePath)
                Dim updateCommand As New SqlUpdateCommand(tableName)
                updateCommand.UpdateDataList.AddLiteral(UpdateType.Assignment, actualStockField, 0)
                accessor.ExecuteCommand(updateCommand)
            Catch ex As DbAccessException
                Return ProcessResult.DbAccessError
            Catch ex As Exception
                Return ProcessResult.UnknownError
            End Try
            Return ProcessResult.Success
        End Function

    End Class
End Namespace
