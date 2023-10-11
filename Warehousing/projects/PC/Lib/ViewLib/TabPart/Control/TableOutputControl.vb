Imports InterfaceLib
Imports ProcessLib.ControlProcess
Imports SettingLib.Control

Namespace TabPart.Control
    Public Class TableOutputControl
        Implements IBtControl

        Public Property Setting() As TableOutputControlSetting

        Public Property Name() As String Implements IBtControl.Name

        Public Sub Execute(ByVal btSheet As IBtSheet) Implements IBtControl.Execute
            If IsNothing(Setting) Then
                Setting = TryCast(btSheet.SheetSetting.ControlPartSettingList.GetSetting(Name), TableOutputControlSetting)
                If IsNothing(Setting) Then Throw New Exception(String.Format("TableOutputControl failed Get Setting : {0}", Name))
            End If

            TableOutputControlProcess.Execute(Setting, btSheet.SheetSetting, btSheet.DataTable, btSheet.GetColumnWidths())
        End Sub
    End Class
End Namespace
