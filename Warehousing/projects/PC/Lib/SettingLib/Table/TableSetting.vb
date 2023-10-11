Imports Database.SqlData.Column
Imports Database.SqlData
Imports System.Globalization

Namespace Table
    Public MustInherit Class TableSetting
        Private Const MAX_COLUMN_COUNT As Integer = 32

        Public Const FIELD_NAME_FORMAT As String = "field{0}"

        Public Const CALCULATION_COLUMN_NAME_FORMAT As String = "Calculation{0}"

        Public Property Name As String

        Public TableName As String

        Private ReadOnly _columnList As List(Of ITableColumnSetting) = New List(Of ITableColumnSetting)()

        Public ReadOnly Property ColumnList() As List(Of ITableColumnSetting)
            Get
                Return _columnList
            End Get
        End Property

        Public Function GetColumnDataList() As ColumnDataList
            Dim columnDataList As New ColumnDataList
            For Each tableColumnSetting As ITableColumnSetting In ColumnList
                If tableColumnSetting.Name.Equals(LogUpdateCommand.LOG_STATUS_COLUMN_NAME) Then Continue For
                columnDataList.Add(tableColumnSetting.Name, tableColumnSetting.DataType)
            Next
            For columnNumber As Integer = columnDataList.Count() + 1 To MAX_COLUMN_COUNT
                columnDataList.Add(String.Format(FIELD_NAME_FORMAT, columnNumber.ToString(CultureInfo.InvariantCulture)), DataType.Text)
            Next
            columnDataList.Add(LogUpdateCommand.LOG_STATUS_COLUMN_NAME, DataType.Text)
            Return columnDataList
        End Function

        Public Function GetNumericColumnNumbers() As List(Of String)
            Dim list = New List(Of String)()
            For Each columnSetting As ITableColumnSetting In _columnList
                If columnSetting.DataType = DataType.IntegerNumber OrElse
                    columnSetting.DataType = DataType.FloatNumber Then
                    list.Add(columnSetting.Name)
                End If
            Next
            Return list
        End Function

        Public Function GetDateColumnNumbers() As List(Of String)
            Dim list = New List(Of String)()
            For Each columnSetting As ITableColumnSetting In _columnList
                If columnSetting.DataType = DataType.DateText Then
                    list.Add(columnSetting.Name)
                End If
            Next
            Return list
        End Function
    End Class
End Namespace
