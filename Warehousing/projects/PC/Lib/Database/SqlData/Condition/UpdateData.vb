
Imports Database.SqlData.Condition.Member

Namespace SqlData.Condition
    Friend Class UpdateData
        Private ReadOnly _target As FieldMember

        Private ReadOnly _value As IMember

        Private ReadOnly _type As UpdateType

        Public Sub New(ByVal type As UpdateType, ByVal target As FieldMember, ByVal value As IMember)
            _target = target
            _value = value
            _type = type
        End Sub

        Public Function ToSqlAndParameter() As SqlAndParameterList
            Dim sqlString As String = String.Empty
            Dim parameterList As New List(Of Parameter)()
            Select Case _type
                Case UpdateType.Assignment
                    sqlString = _target.SqlString + "=" + _value.SqlString
                Case UpdateType.Addition
                    sqlString = _target.SqlString + "=" + _target.SqlString + "+" + _value.SqlString
                Case UpdateType.Subtraction
                    sqlString = _target.SqlString + "=" + _target.SqlString + "-" + _value.SqlString
            End Select
            parameterList.AddRange(_value.Parameter)
            Return New SqlAndParameterList(sqlString, parameterList)
        End Function
    End Class
End Namespace
