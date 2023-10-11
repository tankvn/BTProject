Imports Database.SqlData.Condition
Imports Database
Imports Database.SqlData.DisplayData

Public Interface IUpdatablePart
    Function UpdateView(ByVal dbAccessor As DbAccessor, ByVal tableName As String, ByVal conditionList As ConditionList,
                        ByVal calculationColumnSettings As CalculationColumnDisplayDataList, ByVal autoUpdate As Boolean) As Boolean
End Interface
