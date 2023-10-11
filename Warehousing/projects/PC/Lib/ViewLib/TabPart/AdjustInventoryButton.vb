Imports System.Windows.Forms
Imports System.ComponentModel
Imports ProcessLib.PartProcess
Imports SettingLib.Part

Namespace TabPart
    Public Class AdjustInventoryButton
        Inherits Panel

        Private _tableName As String

        Private WithEvents _executeButton As Button
        <Browsable(True)>
        Public Property ExecuteButton() As Button
            Get
                Return _executeButton
            End Get
            Set(ByVal value As Button)
                _executeButton = value
            End Set
        End Property

        Private WithEvents _clearButton As Button
        <Browsable(True)>
        Public Property ClearButton() As Button
            Get
                Return _clearButton
            End Get
            Set(ByVal value As Button)
                _clearButton = value
            End Set
        End Property

        Public Property Setting() As AdjustInventoryButtonSetting

        Protected Overrides Sub OnCreateControl()
            MyBase.OnCreateControl()
            Dim pageSetting As SheetSetting = ViewUtility.GetPageSetting(Parent, DesignMode)
            If IsNothing(pageSetting) Then Return
            Setting = pageSetting.GuiPartSettingList.GetSetting(Name)
            _tableName = pageSetting.TableName
        End Sub

        Private Sub ExecuteButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles _executeButton.Click

        End Sub

        Private Sub ClearButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles _clearButton.Click
            AdjustInventoryButtonProcess.ClearInventory(_tableName, Setting.ActualStockField)
        End Sub

    End Class
End Namespace
