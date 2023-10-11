Imports System.Windows.Forms
Imports System.ComponentModel
Imports UtilityLib
Imports UtilityLib.Controls
Imports ProcessLib.PartProcess
Imports SettingLib.Part

Namespace TabPart.Filter
    Public Class FilterTreeItemSettingDialog

        Private Const MAX_CONDITION_COUNT As Integer = 63

        Private Const CONDITION_INDEX As Integer = 3

        Private ReadOnly _settingDictionary As Dictionary(Of String, BindingList(Of TreeNodeSetting))

        Public Sub New(ByVal displayColumnSettings As List(Of DisplayColumnSetting), ByVal nodes As List(Of BtTreeNode))

            InitializeComponent()

            _settingDictionary = CreateSettings(nodes)

            _targetColumnList.DisplayMember = "Key"
            _targetColumnList.ValueMember = "Value"

            Dim list As New List(Of KeyValuePair(Of String, String))()
            For Each displayColumnSetting As DisplayColumnSetting In displayColumnSettings
                list.Add(New KeyValuePair(Of String, String)(displayColumnSetting.DisplayName, displayColumnSetting.Name))
            Next
            If list.Count = 0 Then
                _buttonAdd.Enabled = False
                _buttonDelete.Enabled = False
                _textBoxKeyword.Enabled = False
                _comboBoxCondition.Enabled = False
                _dataGridView.Enabled = False
            End If
            _targetColumnList.DataSource = list

            _comboBoxCondition.DisplayMember = "Value"
            _comboBoxCondition.ValueMember = "Key"
            _comboBoxCondition.DataSource = FilterPartProcess.StringConditions
            _comboBoxCondition.SelectedIndex = 0

            Dim dataGridViewColumnComboBox As DataGridViewComboBoxColumn = TryCast(_dataGridView.Columns(3), DataGridViewComboBoxColumn)
            dataGridViewColumnComboBox.DisplayMember = "Value"
            dataGridViewColumnComboBox.ValueMember = "key"
            dataGridViewColumnComboBox.DataSource = FilterPartProcess.StringConditions

            DataGridViewComboBoxUtility.SetDataGridViewImmediateCommit(_dataGridView)
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As EventArgs)
            MyBase.OnLoad(e)
            UpdateAddDeleteButtonState()
            UpdateMoveButtonEnabled()
        End Sub

        Private Function CreateSettings(ByVal nodes As List(Of BtTreeNode)) As Dictionary(Of String, BindingList(Of TreeNodeSetting))
            Dim dictionary As New Dictionary(Of String, BindingList(Of TreeNodeSetting))()
            Dim list As BindingList(Of TreeNodeSetting) = Nothing
            For Each node As BtTreeNode In nodes
                If Not dictionary.TryGetValue(node.FieldName, list) Then
                    list = New BindingList(Of TreeNodeSetting)()
                    dictionary.Add(node.FieldName, list)
                End If
                list.Add(New TreeNodeSetting(node.KeyWord, node.FieldName, ConvertUtility.ToSearchConditionType(node.ConditionType)))
            Next
            Return dictionary
        End Function

        Private Sub _buttonAdd_Click(sender As Object, e As EventArgs) Handles _buttonAdd.Click
            Dim list As BindingList(Of TreeNodeSetting) = GetNodeSettingList()
            list.Add(New TreeNodeSetting(_textBoxKeyword.Text, _targetColumnList.SelectedValue, _comboBoxCondition.SelectedValue))
            _dataGridView.CurrentCell = _dataGridView.Rows(_dataGridView.Rows.Count - 1).Cells(2)
            _textBoxKeyword.Clear()
            UpdateAddDeleteButtonState()
            UpdateMoveButtonEnabled()
        End Sub

        Private Function GetNodeSettingList() As BindingList(Of TreeNodeSetting)
            Dim list As BindingList(Of TreeNodeSetting) = Nothing
            If Not _settingDictionary.TryGetValue(_targetColumnList.SelectedValue, list) Then
                list = New BindingList(Of TreeNodeSetting)()
                _settingDictionary.Add(_targetColumnList.SelectedValue, list)
            End If
            Return list
        End Function

        Private Sub _targetColumnList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles _targetColumnList.SelectedIndexChanged
            If IsNothing(_settingDictionary) Then Return
            Dim list As BindingList(Of TreeNodeSetting) = GetNodeSettingList()
            _dataGridView.DataSource = list
            UpdateAddDeleteButtonState()
            UpdateMoveButtonEnabled()
        End Sub

        Private Sub _buttonDelete_Click(sender As Object, e As EventArgs) Handles _buttonDelete.Click
            If _dataGridView.SelectedRows.Count <= 0 Then Return

            Dim list As BindingList(Of TreeNodeSetting) = TryCast(_dataGridView.DataSource, BindingList(Of TreeNodeSetting))
            Dim maxIndex As Integer = -1
            For Each row As DataGridViewRow In _dataGridView.SelectedRows
                maxIndex = Math.Max(maxIndex, row.Index)
            Next
            Dim nextRow As DataGridViewRow = Nothing
            If maxIndex + 1 < _dataGridView.Rows.Count Then
                nextRow = _dataGridView.Rows(maxIndex + 1)
            End If
            For Each row As DataGridViewRow In _dataGridView.SelectedRows
                list.RemoveAt(row.Index)
            Next
            _dataGridView.ClearSelection()
            If 0 < _dataGridView.Rows.Count Then
                nextRow = If(IsNothing(nextRow), _dataGridView.Rows(_dataGridView.Rows.Count - 1), nextRow)
                nextRow.Selected = True
            End If
            UpdateAddDeleteButtonState()
            UpdateMoveButtonEnabled()
        End Sub

        Public Function GetBtTreeNodeList() As List(Of BtTreeNode)
            Dim hashSet As New HashSet(Of BtTreeNode)(New BtTreeNodeComparer())
            For Each pair As KeyValuePair(Of String, BindingList(Of TreeNodeSetting)) In _settingDictionary
                For Each nodeSetting As TreeNodeSetting In pair.Value
                    Dim node As BtTreeNode = ConvertUtility.ToBtTreeNode(nodeSetting)
                    If hashSet.Contains(node) Then Continue For
                    hashSet.Add(node)
                Next
            Next
            Return hashSet.ToList()
        End Function

        Private Sub _buttonOK_Click(sender As Object, e As EventArgs) Handles _buttonOK.Click
            DialogResult = DialogResult.OK
        End Sub

        Private Sub _buttonCancel_Click(sender As Object, e As EventArgs) Handles _buttonCancel.Click
            DialogResult = DialogResult.Cancel
        End Sub

        Private Sub _dataGridView_CellMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles _dataGridView.CellMouseClick
            DataGridViewComboBoxUtility.ShowComboBoxDropDownListOneClick(_dataGridView, e)
        End Sub

        Private Sub _dataGridView_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles _dataGridView.CurrentCellDirtyStateChanged
            If _dataGridView.CurrentCell.ColumnIndex <> CONDITION_INDEX Then Return

            If _dataGridView.IsCurrentCellDirty Then
                _dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit)

                BasicUtility.SuspendUpdate(_dataGridView)
                Dim currentLocation As Integer = _dataGridView.FirstDisplayedScrollingRowIndex
                Dim bindingList As BindingList(Of TreeNodeSetting) = GetNodeSettingList()
                If IsNothing(bindingList) Then Return
                bindingList.ResetBindings()
                _dataGridView.FirstDisplayedScrollingRowIndex = currentLocation
                UpdateAddDeleteButtonState()
                UpdateMoveButtonEnabled()
                BasicUtility.ResumeUpdate(_dataGridView)
            End If
        End Sub

        Private Sub UpdateAddDeleteButtonState()
            _buttonDelete.Enabled = (0 < _dataGridView.SelectedRows.Count) AndAlso (0 < _dataGridView.Rows.Count)
            _buttonAdd.Enabled = (0 <= _targetColumnList.SelectedIndex) AndAlso (_dataGridView.Rows.Count < MAX_CONDITION_COUNT)
        End Sub

        Private Sub UpdateMoveButtonEnabled()
            Dim allRows As DataGridViewRow() = AsEnumerable(_dataGridView.Rows).ToArray()
            Dim selectedRows = GetSelectedDataGridViewRows().ToArray()
            _buttonUp.Enabled = CanUpItem(allRows, selectedRows)
            _buttonDown.Enabled = CanDownItem(allRows, selectedRows)
        End Sub

        Private Function AsEnumerable(ByVal rows As DataGridViewRowCollection) As IEnumerable(Of DataGridViewRow)
            Return rows.Cast(Of DataGridViewRow)()
        End Function

        Private Function CanUpItem(ByVal allRows As DataGridViewRow(), ByVal selectedRows As DataGridViewRow()) As Boolean
            If (Not selectedRows.Any()) Then Return False
            If (selectedRows.Contains(allRows.FirstOrDefault())) Then Return False

            Return True
        End Function

        Private Function CanDownItem(ByVal allRows As DataGridViewRow(), ByVal selectedRows As DataGridViewRow()) As Boolean
            If (Not selectedRows.Any()) Then Return False
            If (selectedRows.Contains(allRows.LastOrDefault())) Then Return False

            Return True
        End Function

        Private Sub _buttonUp_Click(sender As Object, e As EventArgs) Handles _buttonUp.Click
            Dim rows As IOrderedEnumerable(Of DataGridViewRow) =
                GetSelectedDataGridViewRows().OrderBy(Function(x) x.Index)
            MoveNextDataGridViewItemsRow(rows, False)
            UpdateMoveButtonEnabled()
        End Sub

        Private Function GetSelectedDataGridViewRows() As IEnumerable(Of DataGridViewRow)
            Return _dataGridView.SelectedCells.Cast(Of DataGridViewCell)().GroupBy(Function(x) x.OwningRow).Select(Function(x) x.Key)
        End Function

        Private Sub _buttonDown_Click(sender As Object, e As EventArgs) Handles _buttonDown.Click
            Dim rows As IOrderedEnumerable(Of DataGridViewRow) =
                GetSelectedDataGridViewRows().OrderByDescending(Function(x) x.Index)
            MoveNextDataGridViewItemsRow(rows, True)
            UpdateMoveButtonEnabled()
        End Sub

        Private Sub MoveNextDataGridViewItemsRow(ByVal rows As IEnumerable(Of DataGridViewRow), ByVal forward As Boolean)
            Dim oldIndexList As List(Of Integer) = rows.Select(Function(row) row.Index).ToList()
            Dim newIndexList As New List(Of Integer)()
            Dim currentCellRowIndex As Integer = _dataGridView.CurrentCell.RowIndex

            For index As Integer = 0 To oldIndexList.Count() - 1
                Dim newIndex As Integer = oldIndexList(index) + If(forward, 1, -1)
                If ((newIndex < 0) OrElse (_dataGridView.Rows.Count <= newIndex)) Then Continue For

                MoveIndex(oldIndexList(index), newIndex)
                newIndexList.Add(newIndex)
            Next

            _dataGridView.ClearSelection()

            Dim rowIndex = If(forward, currentCellRowIndex + 1, currentCellRowIndex - 1)

            _dataGridView.CurrentCell = _dataGridView.Rows(rowIndex).Cells(2)
            For Each index As Integer In newIndexList
                _dataGridView.Rows(index).Cells(0).Selected = True
            Next
        End Sub

        Private Sub MoveIndex(ByVal oldIndex As Integer, ByVal newIndex As Integer)
            Dim list As BindingList(Of TreeNodeSetting) = GetNodeSettingList()

            Dim nodeSetting As New TreeNodeSetting(list(oldIndex))
            list.RemoveAt(oldIndex)
            list.Insert(newIndex, nodeSetting)
        End Sub

        Private Sub _dataGridView_SelectionChanged(sender As Object, e As EventArgs) Handles _dataGridView.SelectionChanged
            UpdateAddDeleteButtonState()
            UpdateMoveButtonEnabled()
        End Sub

        Private Sub _dataGridView_EditingControlShowing(ByVal sender As Object, ByVal e As DataGridViewEditingControlShowingEventArgs) Handles _dataGridView.EditingControlShowing
            Dim textBox = TryCast(e.Control, DataGridViewTextBoxEditingControl)
            If IsNothing(textBox) Then Return

            Dim list As BindingList(Of TreeNodeSetting) = GetNodeSettingList()
            Dim currentRowIndex = _dataGridView.CurrentRow.Index
            textBox.Text = list(currentRowIndex).Keyword
        End Sub

        Private Sub _textBoxKeyword_PreviewKeyDown(ByVal sender As Object, ByVal e As PreviewKeyDownEventArgs) Handles _textBoxKeyword.PreviewKeyDown
            If (e.KeyCode <> Keys.Enter) Then Return
            _buttonAdd.PerformClick()
            e.IsInputKey = True
        End Sub
    End Class
End Namespace
