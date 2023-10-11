Imports System.Windows.Forms
Imports System.Runtime.InteropServices
Imports SettingLib

Public Class BasicUtility
    Private Const WM_SETREDRAW As Integer = &HB

    <DllImport("user32")>
    Private Shared Sub SendMessage(ByVal hwnd As IntPtr, ByVal wMsg As Integer, ByVal wParam As Integer, ByVal lParam As Integer)
    End Sub

    Public Shared Sub SuspendUpdate(ByVal control As Control)
        SendMessage(control.Handle, WM_SETREDRAW, 0, 0)
    End Sub

    Public Shared Sub ResumeUpdate(ByVal control As Control)
        SendMessage(control.Handle, WM_SETREDRAW, 1, 0)
        control.Refresh()
    End Sub

    Public Shared Function GetParentControl(Of T)(ByVal parentControl As Control) As Control
        If IsNothing(parentControl) Then Return Nothing
        If TypeOf parentControl Is T Then
            Return parentControl
        End If
        Return GetParentControl(Of T)(parentControl.Parent)
    End Function

    Public Shared Function GetChildControls(Of T)(ByVal controls As Control.ControlCollection) As List(Of T)
        Dim list = New List(Of T)()
        SearchChildControls(controls, GetType(T), list)
        Return list
    End Function

    Private Shared Sub SearchChildControls(ByVal controls As Control.ControlCollection, ByVal type As Type, ByVal list As IList)
        For Each control As Control In controls
            SearchChildControls(control.Controls, type, list)
            If (type.IsInstanceOfType(control)) Then
                list.Add(control)
            End If
        Next
    End Sub

    Public Shared Function DecodePassword(ByVal encryptedPassword As Byte()) As String
        Dim decodedPassword As String = String.Empty
        Dim byteList As List(Of Byte) = New List(Of Byte)()

        For index = 0 To encryptedPassword.Length - 1
            Dim byteCode() As Byte = BitConverter.GetBytes(encryptedPassword(index) Xor &HFF)
            byteList.Add(byteCode(0))
        Next

        Return _
            decodedPassword &
            AppSetting.Encoding.GetString(byteList.ToArray(), 0, byteList.Count)
    End Function

    Public Shared Function EncloseInQuotes(ByVal source As String) As String
        Return """" & source.Replace("""", """""") & """"
    End Function
End Class
