Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms
Imports UtilityLib.My.Resources

Namespace Controls
    Public Class FilterToggleButton
        Inherits CheckBox

        <Browsable(False)>
        Public Shadows Property Text() As String
            Get
                Return String.Empty
            End Get
            Set(value As String)
            End Set
        End Property

        Public Sub New()
            MyBase.New()
            Appearance = Appearance.Button
            Text = String.Empty
            Image = PC_FilterOn
            ImageAlign = ContentAlignment.MiddleCenter
        End Sub

        Protected Overrides Sub OnCheckedChanged(ByVal e As EventArgs)
            MyBase.OnCheckedChanged(e)

            Image = If(Checked, PC_FilterOff, PC_FilterOn)
        End Sub
    End Class
End Namespace
