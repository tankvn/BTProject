Imports Database.SqlData

Namespace Table
    Public Interface ITableColumnSetting
        Property Name() As String

        Property DataType() As DataType

        Property IsKey() As Boolean

        Property IsIndex() As Boolean

    End Interface
End Namespace
