Imports System.Windows.Forms
Imports System.IO
Imports SettingLib
Imports ViewLib.Basic
Imports SettingLib.Table
Imports InterfaceLib
Imports ProcessLib.PartProcess
Imports SettingLib.Part
Imports UtilityLib
Imports ViewLib.My.Resources

Namespace TabPart.ButtonTool
    Public Class DestockingButton
        Inherits Button
        Implements ISheetChild

        Private _tableName As String

        Private _isInvalid As Boolean

        Private _masterSetting As MasterSetting

        Public Property Setting() As DestockingButtonSetting

        Public Event DestockingExecuted As EventHandler(Of DestockingExecutedEventArgs)

        Public Sub New()
            UseMnemonic = False
        End Sub

        Public Sub LoadSetting() Implements ISheetChild.LoadSetting
            Dim parentSheet As IBtSheet = BasicUtility.GetParentControl(Of IBtSheet)(Parent)
            If IsNothing(parentSheet) Then Return
            Dim tableSetting As TableSetting = parentSheet.GetTableSetting()
            _isInvalid = TypeOf tableSetting Is DataFormatSetting
            _masterSetting = TryCast(tableSetting, MasterSetting)
            If IsNothing(_masterSetting) Then Return
            Dim pageSetting As SheetSetting = ViewUtility.GetPageSetting(Parent, DesignMode)
            If IsNothing(pageSetting) Then Return
            Setting = pageSetting.GuiPartSettingList.GetSetting(Name)
            _tableName = pageSetting.TableName
        End Sub

        Protected Overrides Sub OnClick(ByVal e As EventArgs)
            Using New SuppressEvent(BtApplication.AutoUpdateSuppressCounter)
                If _isInvalid Then
                    MessageBox.Show(Me, ErrorInvalidTarget, My.Application.Info.AssemblyName, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If

                If String.IsNullOrEmpty(_tableName) Then Return

                If (MessageBox.Show(Me, ConfirmationMessageUpdateInventory, My.Application.Info.AssemblyName, MessageBoxButtons.OKCancel) <> DialogResult.OK) Then Return

                Dim result As ProcessResult = DestockingButtonProcess.AdjustInventory(_tableName, Setting, _masterSetting)
                Select Case result
                    Case ProcessResult.Success
                        MessageBox.Show(Me, DestockingSuccess, My.Application.Info.AssemblyName, MessageBoxButtons.OK)
                    Case ProcessResult.DbAccessError
                        MessageBox.Show(Me, String.Format(WriteDatabaseFailed, Path.GetFileName(FileUtility.DbFilePath)),
                                        My.Application.Info.AssemblyName, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Case ProcessResult.UnknownError, ProcessResult.None
                        MessageBox.Show(Me, DestockingFailed, My.Application.Info.AssemblyName, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Case ProcessResult.BackupFailed
                        MessageBox.Show(Me, ErrorMessageBackUpFailed, My.Application.Info.AssemblyName, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Select

                RaiseEvent DestockingExecuted(Me, New DestockingExecutedEventArgs(_tableName))
            End Using
        End Sub
    End Class
End Namespace
