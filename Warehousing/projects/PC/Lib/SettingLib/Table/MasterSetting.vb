

Namespace Table
    Public Class MasterSetting
        Inherits TableSetting

        Public Property LoadFilePath() As String = String.Empty

        Public Property HasHistory() As Boolean = False

        Public Function GetColumnSetting(ByVal columnName As String) As ITableColumnSetting
            For Each columnSetting As ITableColumnSetting In ColumnList
                If (columnSetting.Name.Equals(columnName)) Then Return columnSetting
            Next
            Return Nothing
        End Function
    End Class
End Namespace
