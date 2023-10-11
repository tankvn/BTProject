Namespace SqlData.Condition.Member
    Friend Interface IMember
        ReadOnly Property SqlString() As String

        ReadOnly Property Parameter() As ICollection(Of Parameter)

        Function ToSqlAndParameter() As SqlAndParameterList
    End Interface
End Namespace
