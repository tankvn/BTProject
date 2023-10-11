Imports Database.SqlData.Condition.Member
Imports Database.Utility

Namespace SqlData.Condition
    Public Module ConditionUtility
        Public Const ESCAPE_CHARACTER As String = "\"c

        Public ReadOnly SpecialCharacter As String() = New String() {ESCAPE_CHARACTER, "%", "_"}

        Friend Function GetConditionSqlAndParameterList(ByVal type As ConditionType, ByVal left As IMember, ByVal right As IMember, ByVal comparisonType As ComparisonType) As SqlAndParameterList
            Dim parameterList = New List(Of Parameter)(left.Parameter)
            parameterList.AddRange(right.Parameter)
            Dim leftString As String = left.SqlString
            Dim rightString As String = right.SqlString
            If comparisonType <> comparisonType.DateCompare Then
                Dim typeString = GetComparisonTypeString(comparisonType)
                leftString = String.Format(My.Resources.CastString, leftString, typeString)
                rightString = String.Format(My.Resources.CastString, rightString, typeString)
            End If

            Select Case type
                Case ConditionType.Equal
                    Return New SqlAndParameterList(EncloseInParentheses(leftString + "=" + rightString), parameterList)
                Case ConditionType.NotEqual
                    Return New SqlAndParameterList(EncloseInParentheses(leftString + "!=" + rightString), parameterList)
                Case ConditionType.MoreThan
                    Return New SqlAndParameterList(EncloseInParentheses(leftString + ">" + rightString), parameterList)
                Case ConditionType.LessThan
                    Return New SqlAndParameterList(EncloseInParentheses(leftString + "<" + rightString), parameterList)
                Case ConditionType.AndMore
                    Return New SqlAndParameterList(EncloseInParentheses(leftString + ">=" + rightString), parameterList)
                Case ConditionType.AndLess
                    Return New SqlAndParameterList(EncloseInParentheses(leftString + "<=" + rightString), parameterList)
                Case ConditionType.Include
                    Return _
                        New SqlAndParameterList(EncloseInParentheses(leftString + " LIKE '%' || " + rightString + " || '%' ESCAPE '" + ESCAPE_CHARACTER + "'"), parameterList)
                Case ConditionType.NotInclude
                    Return _
                        New SqlAndParameterList(EncloseInParentheses(leftString + " NOT LIKE '%' || " + rightString + " || '%' ESCAPE '" + ESCAPE_CHARACTER + "'"), parameterList)
                Case ConditionType.ForwardMatch
                    Return _
    New SqlAndParameterList(EncloseInParentheses(leftString + " LIKE " + rightString + " || '%' ESCAPE '" + ESCAPE_CHARACTER + "'"), parameterList)
                Case ConditionType.BackwardMatch
                    Return _
    New SqlAndParameterList(EncloseInParentheses(leftString + " LIKE '%' || " + rightString + " ESCAPE '" + ESCAPE_CHARACTER + "'"), parameterList)
                Case ConditionType.ExactMatch
                    Return New SqlAndParameterList(EncloseInParentheses(leftString + "=" + rightString), parameterList)
            End Select
            Return Nothing
        End Function

        Public Function EscapeSpecialCharacters(ByVal conditionType As ConditionType, ByVal searchText As String) As String
            If conditionType = conditionType.Include OrElse
               conditionType = conditionType.NotInclude OrElse
               conditionType = conditionType.ForwardMatch OrElse
               conditionType = conditionType.BackwardMatch Then
                For Each specialChar As String In SpecialCharacter
                    searchText = searchText.Replace(specialChar, ESCAPE_CHARACTER + specialChar)
                Next
            End If
            Return searchText
        End Function
    End Module
End Namespace
