Imports System.Windows.Forms
Imports InterfaceLib
Imports Database.SqlData.Condition
Imports Database
Imports Database.SqlData.DisplayData
Imports SettingLib
Imports UtilityLib

Namespace TabPart
    Public Class UpdateTimeLabel
        Inherits Label
        Implements IUpdatablePart

        Public Property DateTimeFormatPattern As DateTimeFormatPattern = DateTimeFormatPattern.YYYYMMDD_HHMM

        Public Function UpdateView(ByVal dbAccessor As DbAccessor, ByVal tableName As String, ByVal conditionList As ConditionList,
                                   ByVal calculationColumnSettings As CalculationColumnDisplayDataList, ByVal autoUpdate As Boolean) As Boolean Implements IUpdatablePart.UpdateView
            Text = DateTime.Now.ToString(DateTimeUtility.GetFormatString(DateTimeFormatPattern))
            Return True
        End Function
    End Class
End Namespace
