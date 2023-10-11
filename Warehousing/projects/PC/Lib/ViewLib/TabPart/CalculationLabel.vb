Imports System.Windows.Forms
Imports InterfaceLib
Imports Database.SqlData.Condition
Imports Database
Imports Database.SqlData.DisplayData
Imports ProcessLib.PartProcess
Imports SettingLib.Part
Imports UtilityLib

Namespace TabPart
    Public Class CalculationLabel
        Inherits Label
        Implements IUpdatablePart, ISheetChild

        Private _labelDisplayData As DisplayDataList

        Private _allDisplayDataList As DisplayDataList

        Public Property Setting() As New CalculationLabelSetting

        Public Sub New()
            UseMnemonic = False
        End Sub

        Public Sub LoadSetting() Implements ISheetChild.LoadSetting
            Dim pageSetting As SheetSetting = ViewUtility.GetPageSetting(Parent, DesignMode)
            If IsNothing(pageSetting) Then Return
            Setting = pageSetting.GuiPartSettingList.GetSetting(Name)
            _allDisplayDataList = ConvertUtility.ToAllDisplayDataList(pageSetting.DisplayColumnList, pageSetting.CalculationColumnList)
            _labelDisplayData = CalculationLabelProcess.CreateDisplayData(Setting, pageSetting.CalculationColumnList)
        End Sub

        Public Function UpdateView(ByVal dbAccessor As DbAccessor, ByVal tableName As String, ByVal conditionList As ConditionList, ByVal calculationColumnSettings As CalculationColumnDisplayDataList, ByVal autoUpdate As Boolean) As Boolean Implements IUpdatablePart.UpdateView

            Try
                Dim labelString = CalculationLabelProcess.GetDisplayString(dbAccessor, tableName, conditionList, _allDisplayDataList,
                                                                           _labelDisplayData, Setting.UseNumberGroupSeparator)
                Text = Setting.Prefix + " " + labelString + " " + Setting.Suffix
                Return True
            Catch ex As Exception
                Text = Setting.Prefix + " 0 " + Setting.Suffix
                Return False
            End Try
        End Function
    End Class
End Namespace
