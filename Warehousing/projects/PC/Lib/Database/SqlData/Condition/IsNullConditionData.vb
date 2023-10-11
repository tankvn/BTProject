Imports Database.SqlData.Condition.Member
Imports Database.My.Resources

Namespace SqlData.Condition
    Public Class IsNullConditionData
        Implements IConditionData

        Private ReadOnly _field As FieldMember

        Private Sub New(ByVal field As FieldMember)
            _field = field
        End Sub

        Friend Sub New(ByVal fieldName As String)
            Me.New(New FieldMember(fieldName))
        End Sub

        Public Function ToSqlAndParameter() As SqlAndParameterList Implements IConditionData.ToSqlAndParameter
            Return New SqlAndParameterList(String.Format(IsNullSql, _field.SqlString), New Parameter() {})
        End Function
    End Class
End Namespace
