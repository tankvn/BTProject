Imports Database.My.Resources

Namespace SqlData.Condition
    Public Class MultipleColumnIsNullConditionData
        Implements IConditionData

        Private ReadOnly _targets As String()


        Private ReadOnly _isNegative As Boolean

        Public Sub New(ByVal allFieldName As String(), ByVal isNegative As Boolean)
            _targets = allFieldName
            _isNegative = isNegative
        End Sub

        Public Function ToSqlAndParameter() As SqlAndParameterList Implements IConditionData.ToSqlAndParameter
            Dim sqlString As String = "("
            For Each field As String In _targets
                If Not sqlString = "(" Then
                    If _isNegative Then
                        sqlString += " AND "
                    Else
                        sqlString += " OR "
                    End If
                End If
                If _isNegative Then
                    sqlString += "NOT "
                End If
                sqlString += String.Format(IsNullSql, field)
            Next
            sqlString += ")"
            Return New SqlAndParameterList(sqlString, New List(Of Parameter)())
        End Function
    End Class
End Namespace
