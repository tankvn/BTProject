Imports Database.My.Resources
Imports Database.Utility

Namespace SqlData.DisplayData
    Public Module DisplayDataUtility
        Friend Function AddDisplayName(ByVal targetString As String, ByVal displayName As String) As String
            Return AddDisplayName(targetString, displayName, True)
        End Function

        Friend Function AddDisplayName(ByVal targetString As String, ByVal displayName As String, ByVal enclose As Boolean) As String
            If String.IsNullOrEmpty(displayName) Then Return targetString
            Dim convertedName As String
            If enclose Then
                convertedName = EncloseIdentifier(displayName)
            Else
                convertedName = displayName
            End If
            Return String.Format(AsSql, targetString, convertedName)
        End Function
    End Module
End Namespace
