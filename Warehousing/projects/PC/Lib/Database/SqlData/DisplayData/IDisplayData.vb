Namespace SqlData.DisplayData
    Public Interface IDisplayData
        Function ToSqlAndParameters() As SqlAndParameterList

        Function ToSqlAndParametersWithoutDisplayName() As SqlAndParameterList
    End Interface
End Namespace
