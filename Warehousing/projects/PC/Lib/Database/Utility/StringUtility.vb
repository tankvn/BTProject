Imports Database.SqlData.Condition
Imports Database.SqlData

Namespace Utility
    Public Module StringUtility

        Public Function EncloseIdentifier(ByVal target As String) As String
            If String.IsNullOrEmpty(target) Then Return Nothing
            Dim result As String = target.Replace("""", """""")
            Return """" + result + """"
        End Function

        Public Function GetComparisonTypeString(ByVal type As ComparisonType) As String
            Select Case type
                Case ComparisonType.Text
                    Return "TEXT"
                Case ComparisonType.Numeric
                    Return "REAL"
            End Select
            Return String.Empty
        End Function

        Public Function GetResultString(ByVal result As DbUpdateResult) As String
            Select Case result
                Case DbUpdateResult.OK
                    Return "OK"
                Case DbUpdateResult.NG
                    Return "NG"
                Case DbUpdateResult.Skip
                    Return "Skip"
                Case DbUpdateResult.Incomplete
                    Return "Incomplete"
                Case DbUpdateResult.None
                    Return "None"
            End Select
            Return String.Empty
        End Function

        Public Function EscapeString(ByVal target As String) As String
            Return target.Replace("'", "''")
        End Function
    End Module
End Namespace
