Imports Database.SqlData.Condition
Imports System.Windows.Forms
Imports InterfaceLib
Imports SettingLib.Part
Imports UtilityLib

Namespace PartProcess
    Public Class BtSheetProcess

        Public Shared Function CreateSearchCondition(ByVal sheetSetting As SheetSetting,
                                                     ByVal controls As Control.ControlCollection) As ConditionList
            Dim searchConditionData As ConditionList = New ConditionList()
            If Not IsNothing(sheetSetting) AndAlso
                Not IsNothing(sheetSetting.FilterSettingList) AndAlso
                (0 < sheetSetting.FilterSettingList.ConditionList.Count) Then
                searchConditionData.Add(ConvertUtility.ToConditionData(sheetSetting.FilterSettingList,
                                                                       sheetSetting.CalculationColumnList))
            End If
            For Each filterPart As IFilterPart In BasicUtility.GetChildControls(Of IFilterPart)(controls)
                Dim searchConditionList As ConditionList = filterPart.CreateConditionData(sheetSetting.CalculationColumnList)
                If IsNothing(searchConditionList) OrElse Not searchConditionList.HasCondition Then Continue For
                searchConditionData.AddRange(searchConditionList)
            Next
            Return searchConditionData
        End Function

        Public Shared Function CreateSearchCondition(ByVal sheetSetting As SheetSetting) As ConditionList
            Return CreateSearchCondition(sheetSetting, New Control.ControlCollection(Nothing))
        End Function
    End Class
End Namespace
