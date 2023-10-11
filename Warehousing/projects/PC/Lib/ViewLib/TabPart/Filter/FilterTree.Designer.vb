Imports System.Windows.Forms
Imports UtilityLib.Controls

Namespace TabPart.Filter
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class FilterTree
        Inherits Panel

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
            Me.SuspendLayout()

            '
            'FilterTree
            '
            Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(185, Byte), Integer), CType(CType(210, Byte), Integer), CType(CType(240, Byte), Integer))
            Me.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
            Me.Name = "FilterTree"
            Me.Size = New System.Drawing.Size(158, 274)
            Me.ResumeLayout(False)
        End Sub

    End Class
End Namespace
