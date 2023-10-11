
Namespace Basic
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class BtApplication
        Inherits System.Windows.Forms.Form

        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing Then
                    If _timer IsNot Nothing Then
                        _timer.Dispose()
                    End If
                    If components IsNot Nothing Then
                        components.Dispose()
                    End If
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        Private components As System.ComponentModel.IContainer

        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(BtApplication))
            Me.ButtonCommunicationSetting = New System.Windows.Forms.Button()
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
            Me.PanelRefresh = New System.Windows.Forms.Panel()
            Me.ButtonRefresh = New System.Windows.Forms.Button()
            Me.ButtonAutoUpdateSetting = New System.Windows.Forms.Button()
            Me.CheckBoxAutoUpdate = New System.Windows.Forms.CheckBox()
            Me.ButtonMasterFileCreation = New System.Windows.Forms.Button()
            Me.PanelWindowButtons = New System.Windows.Forms.Panel()
            Me.TableLayoutPanel1.SuspendLayout()
            Me.PanelRefresh.SuspendLayout()
            Me.SuspendLayout()
            '
            'ButtonCommunicationSetting
            '
            Me.ButtonCommunicationSetting.Location = New System.Drawing.Point(714, 3)
            Me.ButtonCommunicationSetting.Margin = New System.Windows.Forms.Padding(3, 3, 0, 3)
            Me.ButtonCommunicationSetting.Name = "ButtonCommunicationSetting"
            Me.ButtonCommunicationSetting.Size = New System.Drawing.Size(95, 48)
            Me.ButtonCommunicationSetting.TabIndex = 1
            Me.ButtonCommunicationSetting.Text = "Comm. settings"
            Me.ButtonCommunicationSetting.UseVisualStyleBackColor = True
            Me.ButtonCommunicationSetting.Visible = False
            '
            'TableLayoutPanel1
            '
            Me.TableLayoutPanel1.AutoSize = True
            Me.TableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.TableLayoutPanel1.ColumnCount = 4
            Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
            Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
            Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
            Me.TableLayoutPanel1.Controls.Add(Me.PanelRefresh, 3, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.ButtonMasterFileCreation, 2, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.PanelWindowButtons, 0, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.ButtonCommunicationSetting, 1, 0)
            Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top
            Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
            Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
            Me.TableLayoutPanel1.RowCount = 1
            Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
            Me.TableLayoutPanel1.Size = New System.Drawing.Size(1008, 54)
            Me.TableLayoutPanel1.TabIndex = 0
            '
            'PanelRefresh
            '
            Me.PanelRefresh.AutoSize = True
            Me.PanelRefresh.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.PanelRefresh.Controls.Add(Me.ButtonRefresh)
            Me.PanelRefresh.Controls.Add(Me.ButtonAutoUpdateSetting)
            Me.PanelRefresh.Controls.Add(Me.CheckBoxAutoUpdate)
            Me.PanelRefresh.Location = New System.Drawing.Point(910, 0)
            Me.PanelRefresh.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
            Me.PanelRefresh.Name = "PanelRefresh"
            Me.PanelRefresh.Size = New System.Drawing.Size(98, 54)
            Me.PanelRefresh.TabIndex = 3
            Me.PanelRefresh.Visible = False
            '
            'ButtonRefresh
            '
            Me.ButtonRefresh.Location = New System.Drawing.Point(0, 3)
            Me.ButtonRefresh.Name = "ButtonRefresh"
            Me.ButtonRefresh.Size = New System.Drawing.Size(95, 25)
            Me.ButtonRefresh.TabIndex = 0
            Me.ButtonRefresh.Text = "Refresh"
            Me.ButtonRefresh.UseVisualStyleBackColor = True
            '
            'ButtonAutoUpdateSetting
            '
            Me.ButtonAutoUpdateSetting.FlatAppearance.BorderSize = 0
            Me.ButtonAutoUpdateSetting.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            Me.ButtonAutoUpdateSetting.Image = Global.ViewLib.My.Resources.Resources.ButtonAutoUpdateSetting
            Me.ButtonAutoUpdateSetting.Location = New System.Drawing.Point(72, 28)
            Me.ButtonAutoUpdateSetting.Margin = New System.Windows.Forms.Padding(3, 0, 3, 3)
            Me.ButtonAutoUpdateSetting.Name = "ButtonAutoUpdateSetting"
            Me.ButtonAutoUpdateSetting.Size = New System.Drawing.Size(23, 23)
            Me.ButtonAutoUpdateSetting.TabIndex = 2
            Me.ButtonAutoUpdateSetting.UseVisualStyleBackColor = True
            '
            'CheckBoxAutoUpdate
            '
            Me.CheckBoxAutoUpdate.AutoSize = True
            Me.CheckBoxAutoUpdate.Location = New System.Drawing.Point(0, 26)
            Me.CheckBoxAutoUpdate.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
            Me.CheckBoxAutoUpdate.Name = "CheckBoxAutoUpdate"
            Me.CheckBoxAutoUpdate.Padding = New System.Windows.Forms.Padding(0, 2, 0, 0)
            Me.CheckBoxAutoUpdate.Size = New System.Drawing.Size(80, 26)
            Me.CheckBoxAutoUpdate.TabIndex = 1
            Me.CheckBoxAutoUpdate.Text = "Auto"
            Me.CheckBoxAutoUpdate.UseVisualStyleBackColor = True
            '
            'ButtonMasterFileCreation
            '
            Me.ButtonMasterFileCreation.Location = New System.Drawing.Point(812, 3)
            Me.ButtonMasterFileCreation.Margin = New System.Windows.Forms.Padding(3, 3, 0, 3)
            Me.ButtonMasterFileCreation.Name = "ButtonMasterFileCreation"
            Me.ButtonMasterFileCreation.Size = New System.Drawing.Size(95, 48)
            Me.ButtonMasterFileCreation.TabIndex = 2
            Me.ButtonMasterFileCreation.Text = "Master creation"
            Me.ButtonMasterFileCreation.UseVisualStyleBackColor = True
            Me.ButtonMasterFileCreation.Visible = False
            '
            'PanelWindowButtons
            '
            Me.PanelWindowButtons.Dock = System.Windows.Forms.DockStyle.Fill
            Me.PanelWindowButtons.Location = New System.Drawing.Point(0, 0)
            Me.PanelWindowButtons.Margin = New System.Windows.Forms.Padding(0)
            Me.PanelWindowButtons.Name = "PanelWindowButtons"
            Me.PanelWindowButtons.Size = New System.Drawing.Size(711, 54)
            Me.PanelWindowButtons.TabIndex = 0
            '
            'BtApplication
            '
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
            Me.ClientSize = New System.Drawing.Size(1008, 512)
            Me.Controls.Add(Me.TableLayoutPanel1)
            Me.Font = New System.Drawing.Font("Segoe UI", 9.75!)
            Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
            Me.MinimumSize = New System.Drawing.Size(1024, 550)
            Me.Name = "BtApplication"
            Me.Text = "PC Application"
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.TableLayoutPanel1.PerformLayout()
            Me.PanelRefresh.ResumeLayout(False)
            Me.PanelRefresh.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents ButtonCommunicationSetting As System.Windows.Forms.Button
        Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents PanelRefresh As System.Windows.Forms.Panel
        Friend WithEvents ButtonRefresh As System.Windows.Forms.Button
        Friend WithEvents ButtonAutoUpdateSetting As System.Windows.Forms.Button
        Friend WithEvents CheckBoxAutoUpdate As System.Windows.Forms.CheckBox
        Friend WithEvents ButtonMasterFileCreation As System.Windows.Forms.Button
        Friend WithEvents PanelWindowButtons As System.Windows.Forms.Panel
    End Class
End Namespace
