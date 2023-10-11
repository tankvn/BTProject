Imports Database.SqlData.Condition
Imports System.Windows.Forms
Imports SettingLib.Part
Imports SettingLib.Table

Public Interface IBtSheet

    Property Name As String

    ReadOnly Property SheetSetting() As SheetSetting

    ReadOnly Property SortIndex() As Integer

    ReadOnly Property SortOrder() As SortOrder

    Function GetColumnWidths() As Integer()

    Function GetAllFieldName() As String()

    Function GetTableSetting() As TableSetting

    Function GetConditionList() As ConditionList
End Interface
