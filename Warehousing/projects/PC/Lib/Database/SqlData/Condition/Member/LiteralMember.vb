Namespace SqlData.Condition.Member
    Friend NotInheritable Class LiteralMember
        Implements IMember
        Private ReadOnly _value As Object

        Public ReadOnly Property SqlString() As String Implements IMember.SqlString
            Get
                Return "?"
            End Get
        End Property

        Private ReadOnly _type As DbType
        Public ReadOnly Property Type() As DbType
            Get
                Return _type
            End Get
        End Property

        Public ReadOnly Property Parameter() As ICollection(Of Parameter) Implements IMember.Parameter
            Get
                Dim newParameter As New Parameter(_value, _type)
                Return New Parameter() {newParameter}
            End Get
        End Property

        Sub New(ByVal value As Object, ByVal type As DbType)
            _value = value
            _type = type
        End Sub

        Sub New(ByVal value As Integer)
            Me.new(value, DbType.Int32)
        End Sub

        Sub New(ByVal value As String)
            Me.new(value, DbType.String)
        End Sub

        Sub New(ByVal value As Double)
            Me.new(value, DbType.Double)
        End Sub

        Sub New(ByVal value As Single)
            Me.new(value, DbType.Single)
        End Sub

        Friend Function ToSqlAndParameter() As SqlAndParameterList Implements IMember.ToSqlAndParameter
            Return New SqlAndParameterList(SqlString, New Parameter() {New Parameter(_value, _type)})
        End Function
    End Class
End Namespace
