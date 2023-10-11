
Imports Database.SqlData.Condition
Imports Database.SqlData.DisplayData

Public Interface IFilterPart

    Event FilterExecuted(ByVal sender As Object, ByVal e As EventArgs)

    Function CreateConditionData(ByVal calculationList As CalculationColumnDisplayDataList) As ConditionList
End Interface
