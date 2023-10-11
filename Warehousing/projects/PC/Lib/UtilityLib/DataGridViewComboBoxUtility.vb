Imports System.Windows.Forms

Public Class DataGridViewComboBoxUtility
    Public Shared Sub ShowComboBoxDropDownListOneClick(ByVal dataGridView As DataGridView, ByVal e As DataGridViewCellMouseEventArgs)
        If (e.Button <> MouseButtons.Left) Then Return

        ShowDropDownList(dataGridView, e.ColumnIndex, e.RowIndex)
    End Sub


    Private Shared Sub ShowDropDownList(ByVal dataGridView As DataGridView, ByVal column As Integer, ByVal row As Integer)
        Dim comboBox As DataGridViewComboBoxEditingControl = GetComboBoxEditingControl(dataGridView, column, row)
        If IsNothing(comboBox) OrElse comboBox.DroppedDown Then Return
        comboBox.DroppedDown = True
    End Sub

    Private Shared Function GetComboBoxEditingControl(ByVal dataGridView As DataGridView, ByVal column As Integer, ByVal row As Integer) As DataGridViewComboBoxEditingControl
        If (row < 0 OrElse column < 0) Then Return Nothing
        If Not (TypeOf dataGridView(column, row) Is DataGridViewComboBoxCell) Then Return Nothing

        dataGridView.BeginEdit(False)
        Dim editingControl = TryCast(dataGridView.EditingControl, DataGridViewComboBoxEditingControl)
        If IsNothing(editingControl) Then Return Nothing

        Return editingControl
    End Function

    Public Shared Sub SetDataGridViewImmediateCommit(ByVal dataGridView As DataGridView)
        RemoveHandler dataGridView.EditingControlShowing, AddressOf dataGridView_EditingControlShowing
        AddHandler dataGridView.EditingControlShowing, AddressOf dataGridView_EditingControlShowing
    End Sub


    Private Shared Sub dataGridView_EditingControlShowing(ByVal sender As Object, ByVal e As DataGridViewEditingControlShowingEventArgs)

        Dim comboBox = TryCast(e.Control, DataGridViewComboBoxEditingControl)
        If IsNothing(comboBox) Then Return

        RemoveHandler comboBox.DropDownClosed, AddressOf dataGridViewComboBox_DropDownClosed
        AddHandler comboBox.DropDownClosed, AddressOf dataGridViewComboBox_DropDownClosed

        RemoveHandler comboBox.DropDown, AddressOf DropDownWidthCalculator.AdjustDropDownWidth
        AddHandler comboBox.DropDown, AddressOf DropDownWidthCalculator.AdjustDropDownWidth
    End Sub


    Private Shared Sub dataGridViewComboBox_DropDownClosed(ByVal sender As Object, ByVal e As EventArgs)
        Dim comboBox = TryCast(sender, DataGridViewComboBoxEditingControl)
        If IsNothing(comboBox) Then Return

        RemoveHandler comboBox.DropDownClosed, AddressOf dataGridViewComboBox_DropDownClosed
        RemoveHandler comboBox.DropDown, AddressOf DropDownWidthCalculator.AdjustDropDownWidth

        Dim dataGridView As DataGridView = comboBox.EditingControlDataGridView
        dataGridView.BeginInvoke(Sub() dataGridView.EndEdit())
    End Sub
End Class
