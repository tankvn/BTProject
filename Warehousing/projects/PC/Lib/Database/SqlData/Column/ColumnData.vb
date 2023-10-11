Imports Database.Utility

Namespace SqlData.Column
    Public Class ColumnData
        Private ReadOnly _name As String
        Public ReadOnly Property Name() As String
            Get
                Return _name
            End Get
        End Property

        Private ReadOnly _isPrimaryKey As Boolean
        Public ReadOnly Property IsPrimaryKey() As Boolean
            Get
                Return _isPrimaryKey
            End Get
        End Property

        Private ReadOnly _isIndex As Boolean
        Public ReadOnly Property IsIndex() As Boolean
            Get
                Return _isIndex
            End Get
        End Property

        Private ReadOnly _sqliteDataType As DataType

        Sub New(ByVal name As String, ByVal sqliteDataType As DataType, ByVal isPrimaryKey As Boolean, ByVal isIndex As Boolean)
            _name = name
            _sqliteDataType = sqliteDataType
            _isPrimaryKey = isPrimaryKey
            _isIndex = isIndex
        End Sub

        Sub New(ByVal name As String)
            Me.New(name, DataType.Text, False, False)
        End Sub

        Sub New(ByVal name As String, ByVal sqliteDataType As DataType)
            Me.New(name, sqliteDataType, False, False)
        End Sub

        Friend Function ToSqlString() As String
            Return EncloseIdentifier(_name) + " " + DataTypeToString(_sqliteDataType)
        End Function
    End Class
End Namespace
