Imports Database.Utility

Namespace SqlData.Condition.Member
    Friend Class FieldMember
        Implements IMember

        Private ReadOnly _fieldName As String

        Public ReadOnly Property SqlString() As String Implements IMember.SqlString
            Get
                Return _fieldName
            End Get
        End Property

        Public ReadOnly Property Parameter() As ICollection(Of Parameter) Implements IMember.Parameter
            Get
                Return New Parameter() {}
            End Get
        End Property

        Sub New(ByVal fieldName As String)
            _fieldName = EncloseIdentifier(fieldName)
        End Sub

        Friend Function ToSqlAndParameter() As SqlAndParameterList Implements IMember.ToSqlAndParameter
            Return New SqlAndParameterList(_fieldName, New Parameter() {})
        End Function
    End Class
End Namespace
