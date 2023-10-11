Namespace SqlData.Condition
    Public Module CalculationTypeConverter
        Friend Function GetOperatorString(ByVal type As CalculationType) As String
            Select Case type
                Case CalculationType.Addition
                    Return "+"
                Case CalculationType.Subtraction
                    Return "-"
                Case CalculationType.Multiplication
                    Return "*"
                Case CalculationType.Division
                    Return "/"
            End Select
            Return String.Empty
        End Function
    End Module
End Namespace
