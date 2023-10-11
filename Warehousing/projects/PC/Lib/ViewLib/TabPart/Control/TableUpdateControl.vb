Imports InterfaceLib

Namespace TabPart.Control
    Public Class TableUpdateControl
        Implements IBtControl

        Public Property Setting() As TableUpdateControlSetting

        Public Shadows Property Name() As String Implements IBtControl.Name

        Public Sub Execute(btSheet As IBtSheet) Implements IBtControl.Execute

        End Sub
    End Class

    Public Class TableUpdateControlSetting
    End Class
End Namespace
