Imports System.Windows.Forms
Imports System.IO
Imports UtilityLib
Imports ViewLib.Basic
Imports ProcessLib.PartProcess
Imports SettingLib
Imports ViewLib.My.Resources
Imports SettingLib.Part

Namespace TabPart.ButtonTool
    Public Class FileOutputButton
        Inherits Button
        Implements ISheetChild

        Public Property Setting() As FileOutputButtonSetting

        Private _sheetSetting As SheetSetting

        Public Sub New()
            UseMnemonic = False
        End Sub

        Protected Overrides Sub OnCreateControl()
            MyBase.OnCreateControl()

        End Sub

        Protected Overrides Sub OnClick(ByVal e As EventArgs)
            MyBase.OnClick(e)

            Using New SuppressEvent(BtApplication.AutoUpdateSuppressCounter)

                Dim sheet As BtSheet = BasicUtility.GetParentControl(Of BtSheet)(Parent)
                If IsNothing(sheet) Then Return

                If MessageBox.Show(Me, MessageOutputCsv, My.Application.Info.AssemblyName, MessageBoxButtons.OKCancel) <> DialogResult.OK Then Return
                Dim result = FileOutputButtonProcess.Execute(Setting, _sheetSetting, sheet.GetConditionList(), sheet.SortIndex, sheet.SortOrder)
                Select Case result
                    Case ProcessResult.Success
                        MessageBox.Show(Me, FileOutputSuccess, My.Application.Info.AssemblyName, MessageBoxButtons.OK)
                    Case ProcessResult.CreateDirectoryError
                        MessageBox.Show(Me, String.Format(FolderCreateFailed, Path.GetDirectoryName(Setting.OutputPath)),
                                        My.Application.Info.AssemblyName, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Case Else
                        MessageBox.Show(Me, String.Format(FileOutputFailed, Setting.OutputPath),
                                        My.Application.Info.AssemblyName, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Select
            End Using
        End Sub

        Public Sub LoadSetting() Implements ISheetChild.LoadSetting
            _sheetSetting = ViewUtility.GetPageSetting(Parent, DesignMode)
            Setting = ViewUtility.GetPartSetting(Name, Parent, DesignMode)
        End Sub
    End Class
End Namespace
