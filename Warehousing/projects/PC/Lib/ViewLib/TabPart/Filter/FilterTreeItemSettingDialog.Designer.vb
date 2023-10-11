Namespace TabPart.Filter
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class FilterTreeItemSettingDialog
        Inherits System.Windows.Forms.Form

        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        Private components As System.ComponentModel.IContainer

        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FilterTreeItemSettingDialog))
            Me._textBoxKeyword = New System.Windows.Forms.TextBox()
            Me._labelFilterKeyword = New System.Windows.Forms.Label()
            Me._comboBoxCondition = New System.Windows.Forms.ComboBox()
            Me._buttonDelete = New System.Windows.Forms.Button()
            Me._buttonAdd = New System.Windows.Forms.Button()
            Me._dataGridView = New System.Windows.Forms.DataGridView()
            Me._columnDisplayText = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me._columnKeyword = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me._columnField = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me._columnConditionType = New System.Windows.Forms.DataGridViewComboBoxColumn()
            Me._buttonUp = New System.Windows.Forms.Button()
            Me._buttonDown = New System.Windows.Forms.Button()
            Me._buttonOK = New System.Windows.Forms.Button()
            Me._buttonCancel = New System.Windows.Forms.Button()
            Me._targetColumnList = New System.Windows.Forms.ListBox()
            Me.Label1 = New System.Windows.Forms.Label()
            CType(Me._dataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            '_textBoxKeyword
            '
            Me._textBoxKeyword.Location = New System.Drawing.Point(18, 131)
            Me._textBoxKeyword.Name = "_textBoxKeyword"
            Me._textBoxKeyword.Size = New System.Drawing.Size(304, 27)
            Me._textBoxKeyword.TabIndex = 3
            '
            '_labelFilterKeyword
            '
            Me._labelFilterKeyword.AutoSize = True
            Me._labelFilterKeyword.Location = New System.Drawing.Point(14, 108)
            Me._labelFilterKeyword.Name = "_labelFilterKeyword"
            Me._labelFilterKeyword.Size = New System.Drawing.Size(113, 20)
            Me._labelFilterKeyword.TabIndex = 2
            Me._labelFilterKeyword.Text = "Character string to search for:"
            '
            '_comboBoxCondition
            '
            Me._comboBoxCondition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me._comboBoxCondition.FormattingEnabled = True
            Me._comboBoxCondition.Location = New System.Drawing.Point(195, 164)
            Me._comboBoxCondition.Name = "_comboBoxCondition"
            Me._comboBoxCondition.Size = New System.Drawing.Size(127, 28)
            Me._comboBoxCondition.TabIndex = 4
            '
            '_buttonDelete
            '
            Me._buttonDelete.Image = Global.ViewLib.My.Resources.Resources.UpAllow
            Me._buttonDelete.Location = New System.Drawing.Point(180, 199)
            Me._buttonDelete.Name = "_buttonDelete"
            Me._buttonDelete.Size = New System.Drawing.Size(40, 35)
            Me._buttonDelete.TabIndex = 6
            Me._buttonDelete.UseVisualStyleBackColor = True
            '
            '_buttonAdd
            '
            Me._buttonAdd.Image = Global.ViewLib.My.Resources.Resources.DownAllow
            Me._buttonAdd.Location = New System.Drawing.Point(95, 199)
            Me._buttonAdd.Name = "_buttonAdd"
            Me._buttonAdd.Size = New System.Drawing.Size(40, 35)
            Me._buttonAdd.TabIndex = 5
            Me._buttonAdd.UseVisualStyleBackColor = True
            '
            '_dataGridView
            '
            Me._dataGridView.AllowUserToAddRows = False
            Me._dataGridView.AllowUserToDeleteRows = False
            Me._dataGridView.AllowUserToResizeColumns = False
            Me._dataGridView.AllowUserToResizeRows = False
            Me._dataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
            Me._dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me._dataGridView.ColumnHeadersVisible = False
            Me._dataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me._columnDisplayText, Me._columnKeyword, Me._columnField, Me._columnConditionType})
            Me._dataGridView.Location = New System.Drawing.Point(18, 243)
            Me._dataGridView.Name = "_dataGridView"
            Me._dataGridView.RowHeadersVisible = False
            Me._dataGridView.RowTemplate.Height = 28
            Me._dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
            Me._dataGridView.Size = New System.Drawing.Size(257, 153)
            Me._dataGridView.TabIndex = 7
            '
            '_columnDisplayText
            '
            Me._columnDisplayText.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
            Me._columnDisplayText.DataPropertyName = "DisplayText"
            Me._columnDisplayText.HeaderText = ""
            Me._columnDisplayText.MinimumWidth = 100
            Me._columnDisplayText.Name = "_columnDisplayText"
            '
            '_columnKeyword
            '
            Me._columnKeyword.DataPropertyName = "Keyword"
            Me._columnKeyword.HeaderText = ""
            Me._columnKeyword.Name = "_columnKeyword"
            Me._columnKeyword.ReadOnly = True
            Me._columnKeyword.Visible = False
            '
            '_columnField
            '
            Me._columnField.DataPropertyName = "Field"
            Me._columnField.HeaderText = ""
            Me._columnField.Name = "_columnField"
            Me._columnField.ReadOnly = True
            Me._columnField.Visible = False
            '
            '_columnConditionType
            '
            Me._columnConditionType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
            Me._columnConditionType.DataPropertyName = "ConditionType"
            Me._columnConditionType.HeaderText = ""
            Me._columnConditionType.MinimumWidth = 75
            Me._columnConditionType.Name = "_columnConditionType"
            '
            '_buttonUp
            '
            Me._buttonUp.Image = Global.ViewLib.My.Resources.Resources.UpAllow
            Me._buttonUp.Location = New System.Drawing.Point(282, 277)
            Me._buttonUp.Name = "_buttonUp"
            Me._buttonUp.Size = New System.Drawing.Size(40, 35)
            Me._buttonUp.TabIndex = 8
            Me._buttonUp.UseVisualStyleBackColor = True
            '
            '_buttonDown
            '
            Me._buttonDown.Image = Global.ViewLib.My.Resources.Resources.DownAllow
            Me._buttonDown.Location = New System.Drawing.Point(282, 326)
            Me._buttonDown.Name = "_buttonDown"
            Me._buttonDown.Size = New System.Drawing.Size(40, 35)
            Me._buttonDown.TabIndex = 9
            Me._buttonDown.UseVisualStyleBackColor = True
            '
            '_buttonOK
            '
            Me._buttonOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me._buttonOK.Location = New System.Drawing.Point(136, 408)
            Me._buttonOK.Name = "_buttonOK"
            Me._buttonOK.Size = New System.Drawing.Size(90, 30)
            Me._buttonOK.TabIndex = 10
            Me._buttonOK.Text = "OK"
            Me._buttonOK.UseVisualStyleBackColor = True
            '
            '_buttonCancel
            '
            Me._buttonCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me._buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me._buttonCancel.Location = New System.Drawing.Point(232, 408)
            Me._buttonCancel.Name = "_buttonCancel"
            Me._buttonCancel.Size = New System.Drawing.Size(90, 30)
            Me._buttonCancel.TabIndex = 11
            Me._buttonCancel.Text = "Cancel"
            Me._buttonCancel.UseVisualStyleBackColor = True
            '
            '_targetColumnList
            '
            Me._targetColumnList.FormattingEnabled = True
            Me._targetColumnList.ItemHeight = 20
            Me._targetColumnList.Location = New System.Drawing.Point(18, 32)
            Me._targetColumnList.Name = "_targetColumnList"
            Me._targetColumnList.Size = New System.Drawing.Size(304, 64)
            Me._targetColumnList.TabIndex = 1
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Location = New System.Drawing.Point(14, 9)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(152, 20)
            Me.Label1.TabIndex = 0
            Me.Label1.Text = "Search target (column) list"
            '
            'FilterTreeItemSettingDialog
            '
            Me.AcceptButton = Me._buttonOK
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
            Me.CancelButton = Me._buttonCancel
            Me.ClientSize = New System.Drawing.Size(334, 450)
            Me.Controls.Add(Me.Label1)
            Me.Controls.Add(Me._targetColumnList)
            Me.Controls.Add(Me._buttonCancel)
            Me.Controls.Add(Me._buttonOK)
            Me.Controls.Add(Me._buttonDown)
            Me.Controls.Add(Me._buttonUp)
            Me.Controls.Add(Me._dataGridView)
            Me.Controls.Add(Me._buttonAdd)
            Me.Controls.Add(Me._buttonDelete)
            Me.Controls.Add(Me._comboBoxCondition)
            Me.Controls.Add(Me._labelFilterKeyword)
            Me.Controls.Add(Me._textBoxKeyword)
            Me.Font = New System.Drawing.Font("Segoe UI", 9.75!)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
            Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "FilterTreeItemSettingDialog"
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Search condition settings"
            CType(Me._dataGridView, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents _textBoxKeyword As System.Windows.Forms.TextBox
        Friend WithEvents _labelFilterKeyword As System.Windows.Forms.Label
        Friend WithEvents _comboBoxCondition As System.Windows.Forms.ComboBox
        Friend WithEvents _buttonDelete As System.Windows.Forms.Button
        Friend WithEvents _buttonAdd As System.Windows.Forms.Button
        Friend WithEvents _buttonUp As System.Windows.Forms.Button
        Friend WithEvents _buttonDown As System.Windows.Forms.Button
        Friend WithEvents _buttonOK As System.Windows.Forms.Button
        Friend WithEvents _buttonCancel As System.Windows.Forms.Button
        Friend WithEvents _targetColumnList As System.Windows.Forms.ListBox
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents _columnDisplayText As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents _columnKeyword As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents _columnField As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents _columnConditionType As System.Windows.Forms.DataGridViewComboBoxColumn
        Private WithEvents _dataGridView As System.Windows.Forms.DataGridView
    End Class
End Namespace
