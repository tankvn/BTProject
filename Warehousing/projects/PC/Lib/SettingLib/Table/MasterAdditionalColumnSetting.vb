Imports Database.SqlData

Namespace Table
    Public Class MasterHistoryColumnSetting
        Implements ITableColumnSetting
        Public Property Name As String Implements ITableColumnSetting.Name

        Public Property DataType As Database.SqlData.DataType Implements ITableColumnSetting.DataType

        Public Property IsKey As Boolean Implements ITableColumnSetting.IsKey

        Public Property IsIndex() As Boolean Implements ITableColumnSetting.IsIndex

        Sub New()
            Name = String.Empty
            DataType = DataType.IntegerNumber
            IsKey = False
            IsIndex = False
        End Sub
    End Class
End Namespace
