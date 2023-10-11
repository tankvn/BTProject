Namespace Table
    Public Class DataFormatSetting
        Inherits TableSetting

        Public Property JobName() As String

        Public Property LogFileName() As String


        Public Function GetColumnIndex(ByVal columnName As String) As Integer
            For index As Integer = 0 To ColumnList.Count - 1
                If ColumnList(index).Name = columnName Then Return index
            Next
            Return -1
        End Function

        Public Function GetColumnSetting(ByVal columnName As String) As DataColumnSetting
            For Each logColumnSetting As DataColumnSetting In ColumnList
                If logColumnSetting.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase) Then Return logColumnSetting
            Next
            Return Nothing
        End Function
    End Class
End Namespace
