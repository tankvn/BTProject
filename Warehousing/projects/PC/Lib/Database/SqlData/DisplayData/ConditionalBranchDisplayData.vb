Namespace SqlData.DisplayData
    Public Class ConditionalBranchDisplayData
        Implements ICalculationColumnDisplayData

        Public Property Name() As String Implements ICalculationColumnDisplayData.Name

        Public Property Visible() As Boolean Implements ICalculationColumnDisplayData.Visible

        Public Sub UpdateMemberData(ByVal dataList As CalculationColumnDisplayDataList) _
            Implements ICalculationColumnDisplayData.UpdateMemberData
            Throw New NotImplementedException()
        End Sub

        Public Function ToSqlAndParameters() As SqlAndParameterList Implements IDisplayData.ToSqlAndParameters
            Throw New NotImplementedException()
        End Function

        Public Function ToSqlAndParametersWithoutDisplayName() As SqlAndParameterList Implements IDisplayData.ToSqlAndParametersWithoutDisplayName
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace
