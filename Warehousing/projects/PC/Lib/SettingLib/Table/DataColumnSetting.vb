Imports Database.SqlData

Namespace Table
    Public Class DataColumnSetting
        Implements ITableColumnSetting

        Private Const RESULT_COLUMN_NAME = "Result"

        Public Property Name() As String Implements ITableColumnSetting.Name

        Public Property DataType() As DataType Implements ITableColumnSetting.DataType

        Public Property DatePattern() As DatePattern

        Public Property DateSeparator() As DateSeparator

        Public Property Iskey() As Boolean Implements ITableColumnSetting.IsKey
            Get
                Return False
            End Get
            Set(value As Boolean)
            End Set
        End Property

        Public Property IsIndex() As Boolean Implements ITableColumnSetting.IsIndex

        Public Shared ReadOnly Property ResultColumn() As DataColumnSetting
            Get
                Return New DataColumnSetting() With {
                    .Name = RESULT_COLUMN_NAME
                    }
            End Get
        End Property

        Sub New()
            Name = String.Empty
            DataType = DataType.Text
            DatePattern = DatePattern.YYYYMMDD
            DateSeparator = DateSeparator.Slash
            IsIndex = False
        End Sub
    End Class
End Namespace
