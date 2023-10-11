Imports Database.SqlData.Condition
Imports Database
Imports Database.SqlData.Column
Imports Database.SqlData.DisplayData
Imports System.IO
Imports Database.SqlData
Imports Database.DbException
Imports SettingLib.Table
Imports UtilityLib.My.Resources

Public Class DbAccessUtility

    Public Const MAX_FIELD_COUNT As Integer = 32

    Public Shared Sub CreateTable(ByVal dataBasePath As String, ByVal tableName As String, ByVal columnList As ColumnDataList,
                                  ByVal journalMode As JournalMode, ByVal columnCount As Integer, ByVal terminalName As String)
        Try
            Dim dbAccessor As SQLiteAccessor = DbAccessManager.GetSQLiteAccessor(dataBasePath, journalMode)
            Dim dbDirectory As String = Path.GetDirectoryName(dataBasePath)
            If Not Directory.Exists(dbDirectory) Then
                Directory.CreateDirectory(dbDirectory)
            End If
            CreateTable(dbAccessor, tableName, columnList, columnCount)
        Catch ex As Exception
            ErrorUtility.OutputErrorLog(ex.Message, terminalName)
        End Try
    End Sub

    Public Shared Sub CreateTable(ByVal dataBasePath As String, ByVal tableName As String,
                                  ByVal columnList As ColumnDataList, ByVal columnCount As Integer, ByVal terminalName As String)
        CreateTable(dataBasePath, tableName, columnList, JournalMode.Wal, columnCount, terminalName)
    End Sub

    Public Shared Sub CreateTable(ByVal dbAccessor As DbAccessor, ByVal tableName As String,
                                  ByVal columnList As ColumnDataList, ByVal columnCount As Integer)
        Try
            If dbAccessor.ExistTable(tableName) Then Return
            Dim createTableCommand As New SqlCreateTableCommand(tableName)
            Dim columnDataList As ColumnDataList = CreateColumnDataList(columnList, columnCount)
            createTableCommand.ColumnData.AddRange(columnDataList)
            dbAccessor.ExecuteCommand(createTableCommand)

            For i As Integer = 0 To columnList.Count - 1
                If columnList.Item(i).IsIndex Then
                    Dim createIndexCommand As SqlCreateIndexCommand = New SqlCreateIndexCommand(tableName, columnList.Item(i).Name)
                    dbAccessor.ExecuteCommand(createIndexCommand)
                End If
            Next
        Catch ex As DbAccessException
            Throw New Exception(String.Format(ErrorMessageFileAccessError, Path.GetFileName(dbAccessor.DbFileName)))
        End Try
    End Sub

    Public Shared Function GetDisplayData(ByVal fieldName As String,
                                          ByVal calculationColumnList As CalculationColumnDisplayDataList) As IDisplayData
        If IsNothing(calculationColumnList) Then Return New ColumnDisplayData(fieldName)

        Dim calculationColumnDisplayData As ICalculationColumnDisplayData = calculationColumnList.GetData(fieldName)
        If IsNothing(calculationColumnDisplayData) Then Return New ColumnDisplayData(fieldName)
        Return calculationColumnDisplayData
    End Function


    Private Shared Function CreateColumnDataList(ByVal columnList As ColumnDataList, ByVal columnCount As Integer) As ColumnDataList
        Dim columnDataList As New ColumnDataList
        columnDataList.AddRange(columnList)
        For count As Integer = columnDataList.Count() + 1 To columnCount
            columnDataList.Add(New ColumnData(String.Format(TableSetting.FIELD_NAME_FORMAT, count)))
        Next
        Return columnDataList
    End Function

    Public Shared Sub UpdateDatabase(ByVal dbAccessor As DbAccessor, ByVal tableName As String, _
    ByVal conditionList As ConditionList, ByVal updataList As UpdateDataList)
        If IsNothing(dbAccessor) Then Exit Sub
        Try
            Dim sqlData As New SqlUpdateCommand(tableName)
            If Not IsNothing(conditionList) Then
                sqlData.ConditionList.AddRange(conditionList)
            End If
            sqlData.UpdateDataList.AddRange(updataList)
            dbAccessor.ExecuteCommand(sqlData)
        Catch ex As DbAccessException
            Throw New Exception(ex.Message)
        End Try
    End Sub

    Public Shared Function GetDataStrings(ByVal dbAccessor As DbAccessor, ByVal tableName As String, _
    ByVal conditionList As ConditionList, ByVal displayDataList As DisplayDataList) _
    As String()()
        If IsNothing(dbAccessor) Then Return New String()() {}
        Try
            Dim command As New SqlSelectCommand(tableName)
            If Not IsNothing(conditionList) Then
                command.ConditionList.AddRange(conditionList)
            End If
            If Not IsNothing(displayDataList) Then
                command.DisplayDataList.AddRange(displayDataList)
            End If
            Return dbAccessor.ExecuteCommandAndRead(command)
        Catch ex As DbAccessException
            Throw New Exception(ex.Message)
        End Try
    End Function

    Public Shared Function HasTable(ByVal accessor As DbAccessor, ByVal tableName As String) As Boolean
        Try
            Dim tableNames As String() = accessor.GetTables()
            If IsNothing(tableNames) Then Return False
            Dim dbNameList As New List(Of String)(tableNames)
            Return dbNameList.Contains(tableName)
        Catch ex As DbAccessException
            Throw New Exception(String.Format(ErrorMessageFileAccessError, Path.GetFileName(accessor.DbFileName)))
        End Try
    End Function

    Public Shared Function GetLabelString(ByVal dbAccessor As DbAccessor, ByVal tableName As String,
                                          ByVal conditionList As ConditionList, ByVal displayDataList As DisplayDataList,
                                          ByVal labelDisplayData As DisplayDataList) As String
        If IsNothing(dbAccessor) Then Return Nothing
        Try
            Dim selectCommand As New SqlSelectCommand(tableName)
            If Not IsNothing(conditionList) Then
                selectCommand.ConditionList.AddRange(conditionList)
            End If
            If Not IsNothing(displayDataList) Then
                selectCommand.DisplayDataList.AddRange(displayDataList)
            End If

            Dim command As New SqlSelectCommandForCalculationLabel(labelDisplayData, selectCommand.ToSqlAndParametersForCalculationLabel())
            Return dbAccessor.ExecuteCommandAndRead(command)(0)(0)
        Catch ex As DbAccessException
            ErrorUtility.OutputErrorLog(ex.Message, String.Empty)
            Throw
        End Try
    End Function

    Public Shared Sub DeleteAllRecord(ByVal dbAccessor As DbAccessor, ByVal tableName As String)
        If IsNothing(dbAccessor) Then Exit Sub
        Try
            dbAccessor.ExecuteCommand(New SqlDeleteCommand(tableName))
        Catch ex As DbAccessException
            Throw New Exception(ex.Message)
        End Try
    End Sub

    Public Shared Function GetAllColumnStringData(ByVal dbAccessor As DbAccessor, ByVal tableName As String,
                                                  ByVal conditionList As ConditionList) As String()()
        Dim selectCommand As New SqlSelectCommand(tableName)
        If Not IsNothing(conditionList) Then
            selectCommand.ConditionList.AddRange(conditionList)
        End If
        Return dbAccessor.ExecuteCommandAndRead(selectCommand)
    End Function

    Public Shared Sub Insert(ByVal dbAccessor As DbAccessor, ByVal tableName As String, ByVal insertData As ICollection(Of String),
                             ByVal insertField As ICollection(Of String))
        If IsNothing(dbAccessor) Then Exit Sub
        Try
            Dim command As New SqlInsertCommand(tableName)
            command.InsertDataList.AddRange(insertData)
            command.InsertFieldList.AddRange(insertField)
            dbAccessor.ExecuteCommand(command)
        Catch ex As DbAccessException
            Throw New Exception(ex.Message)
        End Try
    End Sub

End Class
