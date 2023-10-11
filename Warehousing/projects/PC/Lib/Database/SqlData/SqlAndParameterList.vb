Namespace SqlData
    Public Class SqlAndParameterList
        Private ReadOnly _parameterList As List(Of Parameter)
        Public ReadOnly Property ParameterList() As List(Of Parameter)
            Get
                Return _parameterList
            End Get
        End Property

        Private ReadOnly _sqlString As String
        Public ReadOnly Property SqlString() As String
            Get
                Return _sqlString
            End Get
        End Property

        Sub New(ByVal sqlString As String, ByVal parameters As ICollection(Of Parameter))
            _sqlString = sqlString
            _parameterList = New List(Of Parameter)(parameters)
        End Sub

        Sub New()
            _sqlString = String.Empty
            _parameterList = New List(Of Parameter)()
        End Sub
    End Class
End Namespace
