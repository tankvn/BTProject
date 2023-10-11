Namespace SqlData.DisplayData
    Public Class AllDisplayData
        Implements IDisplayData

        Public Function ToSqlAndParameters() As SqlAndParameterList Implements IDisplayData.ToSqlAndParameters
            Return New SqlAndParameterList("*", New Parameter() {})
        End Function

        Public Function ToSqlAndParametersWithoutDisplayName() As SqlAndParameterList Implements IDisplayData.ToSqlAndParametersWithoutDisplayName
            Return New SqlAndParameterList("*", New Parameter() {})
        End Function
    End Class
End Namespace
