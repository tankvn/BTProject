Imports System.Windows.Forms
Imports SettingLib.Part

Namespace TabPart
    Public Class BtButton
        Inherits Button
        Implements ISheetChild

        Public Property Setting() As BtButtonSetting

        Protected Overrides Sub OnCreateControl()
            MyBase.OnCreateControl()
        End Sub

        Public Sub LoadSetting() Implements ISheetChild.LoadSetting
            Setting = ViewUtility.GetPartSetting(Name, Parent, DesignMode)
        End Sub
    End Class
End Namespace
