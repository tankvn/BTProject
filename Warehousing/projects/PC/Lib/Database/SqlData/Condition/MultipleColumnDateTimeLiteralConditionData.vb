Imports Database.SqlData.Condition.Member

Namespace SqlData.Condition
    Public Class MultipleColumnDateTimeLiteralConditionData
        Implements IConditionData

        Private ReadOnly _targetList As List(Of DateTimeFieldMember)

        Private ReadOnly _fromMember As DateTimeMember

        Private ReadOnly _toMember As DateTimeMember

        Sub New(ByVal targets As String(), ByVal fromDateString As String, ByVal toDateString As String)
            If Not String.IsNullOrEmpty(fromDateString) Then
                _fromMember = New DateTimeLiteralMember(fromDateString)
            End If
            If Not String.IsNullOrEmpty(toDateString) Then
                _toMember = New DateTimeLiteralMember(toDateString)
            End If
            _targetList = New List(Of DateTimeFieldMember)()
            For Each target As String In targets
                _targetList.Add(New DateTimeFieldMember(target))
            Next
        End Sub

        Friend Function ToSqlAndParameter() As SqlAndParameterList Implements IConditionData.ToSqlAndParameter
            Dim parameterList = New List(Of Parameter)()
            Dim sqlString As String = "("
            For Each field As DateTimeFieldMember In _targetList
                If Not sqlString = "(" Then
                    sqlString += " OR "
                End If

                Dim existLeft As Boolean = False
                If Not IsNothing(_fromMember) Then
                    sqlString += "(" + field.SqlString + ">=" + _fromMember.SqlString + ")"
                    parameterList.AddRange(_fromMember.Parameter)
                    existLeft = True
                End If

                If Not IsNothing(_toMember) Then
                    If existLeft Then
                        sqlString += " AND "
                    End If
                    parameterList.AddRange(_toMember.Parameter)
                    sqlString += "(" + field.SqlString + "<=" + _toMember.SqlString + ")"
                End If
            Next
            sqlString += ")"
            Return New SqlAndParameterList(sqlString, parameterList)
        End Function
    End Class
End Namespace
