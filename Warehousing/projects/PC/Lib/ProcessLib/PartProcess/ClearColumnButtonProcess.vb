Imports Database.SqlData.Condition
Imports Database
Imports Database.DbException
Imports SettingLib
Imports Database.SqlData
Imports UtilityLib

Namespace PartProcess
    Public Class ClearColumnButtonProcess
        Public Shared Function Execute(ByVal tableName As String, ByVal typeDictionary As Dictionary(Of String, DataType)) As ProcessResult
            If (typeDictionary.Count <= 0) Then Return ProcessResult.None
            Dim accessor As SQLiteAccessor = DbAccessManager.GetSQLiteAccessor(FileUtility.DbFilePath)
            Try
                Using manager As New TransactionManager(accessor)
                    Dim updateCommand As New SqlUpdateCommand(tableName)
                    For Each pair As KeyValuePair(Of String, DataType) In typeDictionary
                        Select Case pair.Value
                            Case DataType.Text
                                updateCommand.UpdateDataList.AddLiteral(UpdateType.Assignment, pair.Key, String.Empty)
                            Case DataType.DateText
                                updateCommand.UpdateDataList.AddLiteral(UpdateType.Assignment, pair.Key, "2000-01-01")
                            Case DataType.IntegerNumber, DataType.FloatNumber
                                updateCommand.UpdateDataList.AddLiteral(UpdateType.Assignment, pair.Key, 0)
                        End Select
                    Next
                    accessor.ExecuteCommand(updateCommand)
                    manager.Commit()
                End Using
                Return ProcessResult.Success
            Catch ex As DbAccessException
                Return ProcessResult.DbAccessError
            Catch ex As Exception
                Return ProcessResult.UnknownError
            End Try
        End Function
    End Class
End Namespace
