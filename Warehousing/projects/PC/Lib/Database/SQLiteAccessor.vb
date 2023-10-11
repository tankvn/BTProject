Imports Database.SqlData
Imports System.Data.SQLite
Imports Database.My.Resources
Imports Database.Utility
Imports System.IO

Public Class SQLiteAccessor
    Inherits DbAccessor

    Friend Sub New(ByVal dbPath As String, ByVal journalMode As JournalMode)
        MyBase.New(CreateSQLiteConnection(SQLiteFactory.Instance, dbPath, journalMode), SQLiteFactory.Instance, Path.GetFileName(dbPath))
    End Sub

    Public Overrides Function GetTables() As String()
        Dim sqlString As String = SelectTableSql
        Return GetStringArrayFromDb(sqlString, 0)
    End Function

    Public Overrides Function GetViews() As String()
        Dim sqlString As String = SelectViewSql
        Return GetStringArrayFromDb(sqlString, 0)
    End Function

    Public Overrides Function GetTablesAndViews() As String()
        Dim sqlString As String = SelectTableAndViewSql
        Return GetStringArrayFromDb(sqlString, 0)
    End Function

    Public Overrides Function GetFields(ByVal tableName As String) As String()
        Dim sqlString As String = String.Format(GetFieldsSql, EncloseIdentifier(tableName))
        Return GetStringArrayFromDb(sqlString, 1)
    End Function

    Public Overrides Function GetIndexes() As String()
        Dim sqlString As String = String.Format(SelectIndexSql)
        Return GetStringArrayFromDb(sqlString, 0)
    End Function

    Public Sub ExecuteVacuum()
        ExecuteCommandWithoutTransaction("VACUUM")
    End Sub

    Public Overrides Sub AddColumn(ByVal tableName As String, ByVal columnName As String, ByVal type As DataType)
        Dim sqlString As String = String.Format(AddColumnSql, EncloseIdentifier(tableName), columnName, DataTypeToString(type))
        ExecuteCommand(sqlString)
    End Sub

    Public Overrides Function GetTableSchema(ByVal tableName As String) As String
        Dim sqlString As String = String.Format(GetTableSchemaSql, tableName)
        Return GetStringArrayFromDb(sqlString, 0)(0)
    End Function

    Public Overrides Function ExistTable(ByVal tableName As String) As Boolean
        Dim sqlString As String = String.Format(ExistTableSql, EscapeString(tableName))
        Dim count As Integer
        If Integer.TryParse(GetStringArrayFromDb(sqlString, 0)(0), count) Then
            Return (0 < count)
        End If
        Return False
    End Function
End Class
