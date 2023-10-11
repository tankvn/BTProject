Imports System.Windows.Forms
Imports SettingLib.Table
Imports ViewLib.Basic
Imports InterfaceLib
Imports ProcessLib.PartProcess
Imports SettingLib
Imports SettingLib.Part
Imports UtilityLib
Imports ViewLib.My.Resources

Namespace TabPart.ButtonTool
    Public Class FormPrintButton
        Inherits Button
        Implements ISheetChild

        Public Property Setting() As FormPrintButtonSetting

        Private _parentSheet As IBtSheet

        Public Sub New()
            UseMnemonic = False
        End Sub

        Public Sub LoadSetting() Implements ISheetChild.LoadSetting
            If FileUtility.IsDesignMode(DesignMode) Then Return
            If IsNothing(Parent) Then Return

            Dim parentSheet As IBtSheet = TryCast(BasicUtility.GetParentControl(Of IBtSheet)(Parent), IBtSheet)
            If IsNothing(parentSheet) Then Return
            _parentSheet = parentSheet

            Dim sheetSetting As SheetSetting = ViewUtility.GetPageSetting(Parent, DesignMode)
            If IsNothing(sheetSetting) Then Return

            Setting = sheetSetting.GuiPartSettingList.GetSetting(Name)
        End Sub

        Protected Overrides Sub OnClick(ByVal e As EventArgs)
            MyBase.OnClick(e)
            Using New SuppressEvent(BtApplication.AutoUpdateSuppressCounter)
                ExecutePrint()
            End Using
        End Sub

        Public Sub ExecutePrint()

            If Setting.PrintFormat = PrintFormat.Slip AndAlso
                TypeOf (_parentSheet.GetTableSetting()) Is DataFormatSetting Then
                MessageBox.Show(Me, ErrorInvalidTarget, My.Application.Info.AssemblyName, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim result = FormPrintButtonProcess.Execute(Setting.Clone(), _parentSheet.SheetSetting, _parentSheet.GetTableSetting(),
                                                        _parentSheet.GetColumnWidths(), _parentSheet.GetConditionList(),
                                                        _parentSheet.SortIndex, _parentSheet.SortOrder)
            Select Case result
                Case ProcessResult.Success
                    MessageBox.Show(Me, PrintSuccess, My.Application.Info.AssemblyName, MessageBoxButtons.OK)
                Case ProcessResult.Canceled
                Case ProcessResult.SettingsError
                    MessageBox.Show(Me, PrintSettingsError, My.Application.Info.AssemblyName, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Case ProcessResult.NoData
                    MessageBox.Show(Me, PrintNoData, My.Application.Info.AssemblyName, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Case Else
                    MessageBox.Show(Me, PrintFailed, My.Application.Info.AssemblyName, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Select
        End Sub
    End Class
End Namespace
