Namespace SqlData.Condition.Member
    Public Module DateUnitConverter
        Public Function GetDateUnitString(ByVal dateUnit As DateUnit)
            Dim unitString As String = Nothing
            Select Case dateUnit
                Case dateUnit.Year
                    unitString = "years"
                Case dateUnit.Month
                    unitString = "months"
                Case dateUnit.Day
                    unitString = "days"
            End Select
            Return unitString
        End Function
    End Module
End Namespace
