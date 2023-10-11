Public Class DbAccessManager

    Private Shared ReadOnly AccessorDictionary As New Dictionary(Of String, DbAccessor)

    Public Shared Function GetSQLiteAccessor(ByVal path As String) As SQLiteAccessor
        Return GetSQLiteAccessor(path, JournalMode.Wal)
    End Function

    Public Shared Function GetSQLiteAccessor(ByVal path As String, ByVal journalMode As JournalMode) As SQLiteAccessor
        Dim accessor As SQLiteAccessor = Nothing
        If AccessorDictionary.TryGetValue(path, accessor) Then
            Return accessor
        End If

        accessor = New SQLiteAccessor(path, journalMode)
        AccessorDictionary.Add(path, accessor)
        Return accessor
    End Function

    Public Shared Sub DeleteSQLiteAccessor(ByVal path As String)
        Dim accessor As SQLiteAccessor = Nothing
        If Not AccessorDictionary.TryGetValue(path, accessor) Then Return
        accessor.Dispose()
        AccessorDictionary.Remove(path)
    End Sub

    Public Shared Sub ClearAllAccessor()
        For Each pair As KeyValuePair(Of String, DbAccessor) In AccessorDictionary
            If IsNothing(pair.Value) Then
                pair.Value.Dispose()
            End If
        Next
        AccessorDictionary.Clear()
    End Sub
End Class
