Imports System.Windows.Forms
Imports UtilityLib

Namespace Basic
    Public Class BtTabControl
        Inherits TabControl

        Protected Overrides Sub OnCreateControl()
            MyBase.OnCreateControl()
        End Sub

        Public Sub InitializePage()
            Dim index = SettingsUtility.GetTabSelectionData(BasicUtility.GetParentControl(Of BtWindow)(Parent).Name, Name)
            If (0 < index) Then
                SelectedIndex = index
            End If
        End Sub

        Public Sub UpdateDisplayedPage(ByVal onShown As Boolean, ByVal autoUpdate As Boolean)
            For Each control As Control In SelectedTab.Controls
                Dim sheet As BtSheet = GetBtSheet(control)
                If IsNothing(sheet) OrElse
                    IsNothing(sheet.SheetSetting) Then Continue For
                sheet.UpdateView(onShown, autoUpdate)
            Next
        End Sub

        Public Sub ClearDisplayedPage()
            For Each control As Control In SelectedTab.Controls
                Dim sheet As BtSheet = GetBtSheet(control)
                If IsNothing(sheet) OrElse
                    IsNothing(sheet.SheetSetting) OrElse
                    (Not sheet.SheetSetting.UpdateOnShown) Then Continue For
                sheet.ClearData()
            Next
        End Sub

        Protected Overrides Sub OnSelectedIndexChanged(ByVal e As EventArgs)
            MyBase.OnSelectedIndexChanged(e)

            If FileUtility.IsDesignMode(DesignMode) Then Return

            For Each tabPage As TabPage In TabPages
                UpdateSheetData(tabPage, GetUpdateSheet(tabPage))
            Next
        End Sub

        Public Function GetUpdateSheet(ByVal tabPage As TabPage) As BtSheet

            For Each control As Control In tabPage.Controls
                Dim sheet As BtSheet = GetBtSheet(control)
                If IsNothing(sheet) Then Continue For
                Return sheet
            Next
            Return Nothing
        End Function

        Private Sub UpdateSheetData(ByVal tabPage As TabPage, ByVal sheet As BtSheet)
            If IsNothing(tabPage) OrElse IsNothing(sheet) Then Return
            If TabPages.IndexOf(tabPage) <> SelectedIndex Then
                sheet.ClearData()
                Return
            End If
            sheet.UpdateView(True, True)
        End Sub

        Public Shared Function GetBtSheet(ByVal parent As Control) As BtSheet
            If TypeOf (parent) Is BtSheet Then Return parent
            For Each child As Control In parent.Controls
                If TypeOf (child) Is BtSheet Then
                    Return child
                End If
                Return GetBtSheet(child)
            Next
            Return Nothing
        End Function
    End Class
End Namespace
