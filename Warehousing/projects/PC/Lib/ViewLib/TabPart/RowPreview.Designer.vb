Namespace TabPart
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class RowPreview
        Inherits System.Windows.Forms.UserControl

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
            Me._groupBox = New System.Windows.Forms.GroupBox()
            Me._tableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
            Me._groupBox.SuspendLayout()
            Me.SuspendLayout()
            '
            '_groupBox
            '
            Me._groupBox.Controls.Add(Me._tableLayoutPanel)
            Me._groupBox.Dock = System.Windows.Forms.DockStyle.Fill
            Me._groupBox.Location = New System.Drawing.Point(0, 0)
            Me._groupBox.Margin = New System.Windows.Forms.Padding(0)
            Me._groupBox.Name = "_groupBox"
            Me._groupBox.Size = New System.Drawing.Size(544, 200)
            Me._groupBox.TabIndex = 0
            Me._groupBox.TabStop = False
            '
            '_tableLayoutPanel
            '
            Me._tableLayoutPanel.ColumnCount = 3
            Me._tableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
            Me._tableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
            Me._tableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
            Me._tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
            Me._tableLayoutPanel.Location = New System.Drawing.Point(3, 15)
            Me._tableLayoutPanel.Name = "_tableLayoutPanel"
            Me._tableLayoutPanel.RowCount = 1
            Me._tableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle())
            Me._tableLayoutPanel.Size = New System.Drawing.Size(538, 182)
            Me._tableLayoutPanel.TabIndex = 0
            '
            'RowPreview
            '
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
            Me.Controls.Add(Me._groupBox)
            Me.Name = "RowPreview"
            Me.Size = New System.Drawing.Size(544, 200)
            Me._groupBox.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents _groupBox As System.Windows.Forms.GroupBox
        Friend WithEvents _tableLayoutPanel As System.Windows.Forms.TableLayoutPanel

    End Class
End Namespace
