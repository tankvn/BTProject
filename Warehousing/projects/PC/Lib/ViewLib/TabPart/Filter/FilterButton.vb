Imports System.Windows.Forms
Imports System.Drawing
Imports InterfaceLib
Imports Database.SqlData.Condition
Imports Database.SqlData.DisplayData
Imports SettingLib.Part
Imports UtilityLib
Imports ViewLib.My.Resources

Namespace TabPart.Filter

    Public Class FilterButton
        Inherits CheckBox
        Implements IFilterPart, ISheetChild

        Public Property Setting() As FilterButtonSetting

        Public Event FilterExecuted(ByVal sender As Object, ByVal e As EventArgs) Implements IFilterPart.FilterExecuted

        Public Sub New()
            MyBase.New()
            Appearance = Appearance.Button
            Image = PC_FilterOn
            ImageAlign = ContentAlignment.MiddleLeft
            UseMnemonic = False
            UseVisualStyleBackColor = True
        End Sub

        Public Sub LoadSetting() Implements ISheetChild.LoadSetting
            Setting = ViewUtility.GetPartSetting(Name, Parent, DesignMode)
        End Sub

        Public Function CreateConditionData(ByVal calculationList As CalculationColumnDisplayDataList) As ConditionList Implements IFilterPart.CreateConditionData
            Dim conditionList As New ConditionList()
            If Not Checked Then Return conditionList
            Dim conditionData = ConvertUtility.ToConditionData(Setting.FilterSettingList, calculationList)
            If IsNothing(conditionData) Then Return conditionList
            conditionList.Add(conditionData)
            Return conditionList
        End Function

        Protected Overrides Sub OnCheckedChanged(ByVal e As EventArgs)
            MyBase.OnCheckedChanged(e)

            Image = If(Checked, PC_FilterOff, PC_FilterOn)
            RaiseEvent FilterExecuted(Me, EventArgs.Empty)
        End Sub
    End Class
End Namespace
