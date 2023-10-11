Namespace SqlData.DisplayData
    Public Interface ICalculationColumnDisplayData
        Inherits IDisplayData
        Property Name() As String

        Property Visible() As Boolean

        Sub UpdateMemberData(ByVal dataList As CalculationColumnDisplayDataList)
    End Interface
End Namespace
