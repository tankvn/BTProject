Imports Database.Utility

Namespace SqlData.Column
    Public Class ColumnDataList
        Private ReadOnly _columnDataList As List(Of ColumnData)

        Public Sub New()
            _columnDataList = New List(Of ColumnData)()
        End Sub

        Public Sub Add(ByVal columnData As ColumnData)
            _columnDataList.Add(columnData)
        End Sub

        Public Sub Add(ByVal columnName As String, ByVal dataType As DataType, ByVal isPrimaryKey As Boolean, ByVal isIndex As Boolean)
            Add(New ColumnData(columnName, dataType, isPrimaryKey, isIndex))
        End Sub

        Public Sub Add(ByVal columnName As String, ByVal sqliteDataType As DataType)
            Add(New ColumnData(columnName, sqliteDataType))
        End Sub

        Public Sub Add(ByVal columnName As String)
            Add(New ColumnData(columnName))
        End Sub

        Public Sub AddRange(ByVal columns As ICollection(Of ColumnData))
            For Each columnData As ColumnData In columns
                Add(columnData)
            Next
        End Sub

        Public Sub AddRange(ByVal columnNames As ICollection(Of String))
            For Each columnName As String In columnNames
                Add(columnName)
            Next
        End Sub

        Public Sub AddRange(ByVal columnDataList As ColumnDataList)
            _columnDataList.AddRange(columnDataList._columnDataList)
        End Sub

        Public Function Count() As Integer
            Return _columnDataList.Count
        End Function

        Public Function Item(ByVal index As Integer) As ColumnData
            Return _columnDataList(index)
        End Function

        Public Function ToSqlString() As String
            Dim keyList As New List(Of ColumnData)
            Dim sqlString As String = "("
            For Each columnData As ColumnData In _columnDataList
                If columnData.IsPrimaryKey Then
                    keyList.Add(columnData)
                End If
                sqlString += columnData.ToSqlString() + ","
            Next
            If keyList.Count <= 0 Then Return sqlString.TrimEnd(",") + ")"

            sqlString += "PRIMARY KEY("
            For Each columnData As ColumnData In keyList
                sqlString += EncloseIdentifier(columnData.Name()) + ","
            Next
            Return sqlString.TrimEnd(",") + "))"
        End Function
    End Class
End Namespace
