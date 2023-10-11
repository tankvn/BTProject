Imports System.Windows.Forms
Imports ProcessLib.PartProcess
Imports SettingLib
Imports SettingLib.Part
Imports SettingLib.Table
Imports Database.SqlData
Imports UtilityLib
Imports ViewLib.My.Resources

Namespace TabPart.ButtonTool
    Public Class ClearColumnButton
        Inherits Button
        Implements ISheetChild

        Private _tableName As String

        Private _hasErrorColumn As Boolean

        Private ReadOnly _typeDictionary As New Dictionary(Of String, DataType)()

        Public Event ClearColumnExecuted As EventHandler

        Public Property Setting() As ClearColumnButtonSetting

        Public Sub New()
            UseMnemonic = False
        End Sub

        Public Sub LoadSetting() Implements ISheetChild.LoadSetting
            Dim pageSetting As SheetSetting = ViewUtility.GetPageSetting(Parent, DesignMode)
            If IsNothing(pageSetting) Then Return
            Setting = pageSetting.GuiPartSettingList.GetSetting(Name)
            _tableName = pageSetting.TableName
        End Sub

        Public Sub Initialize(ByVal tableSetting As TableSetting)
            If FileUtility.IsDesignMode(DesignMode) Then Return
            If (TypeOf (tableSetting) Is DataFormatSetting) Then Return

            For Each targetColumn As String In Setting.TargetColumns
                If _typeDictionary.ContainsKey(targetColumn) Then Continue For
                For Each columnSetting As ITableColumnSetting In tableSetting.ColumnList
                    If targetColumn.Equals(columnSetting.Name, StringComparison.OrdinalIgnoreCase) Then
                        If columnSetting.IsKey Then
                            _hasErrorColumn = True
                            Continue For
                        End If
                        _typeDictionary.Add(targetColumn, columnSetting.DataType)
                        Exit For
                    End If
                Next
            Next

            If Setting.TargetColumns.Length = 0 Then
                For Each columnSetting As ITableColumnSetting In tableSetting.ColumnList
                    If columnSetting.IsKey OrElse TypeOf (columnSetting) Is MasterHistoryColumnSetting Then Continue For
                    _typeDictionary.Add(columnSetting.Name, columnSetting.DataType)
                Next
            End If
        End Sub

        Protected Overrides Sub OnClick(ByVal e As EventArgs)
            MyBase.OnClick(e)
            If _hasErrorColumn Then
                MessageBox.Show(Me, WarningCannotClearKeyColumn, My.Application.Info.AssemblyName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End If
            If _typeDictionary.Count <= 0 Then Return

            If MessageBox.Show(Me, ConfirmationMessageClearColumn, My.Application.Info.AssemblyName, MessageBoxButtons.OKCancel) <> DialogResult.OK Then Return

            Dim result As ProcessResult = ClearColumnButtonProcess.Execute(_tableName, _typeDictionary)
            Select Case result
                Case ProcessResult.Success
                    MessageBox.Show(Me, ClearColumnSuccess, My.Application.Info.AssemblyName, MessageBoxButtons.OK)
                    RaiseEvent ClearColumnExecuted(Me, e)
                Case ProcessResult.DbAccessError
                    MessageBox.Show(Me, WriteDatabaseFailed, My.Application.Info.AssemblyName, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Case ProcessResult.UnknownError
                    MessageBox.Show(Me, ClearColumnFailed, My.Application.Info.AssemblyName, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Case ProcessResult.None
            End Select
        End Sub
    End Class
End Namespace
