Imports System.Windows.Forms
Imports System.Runtime.InteropServices

Namespace Controls
    Public Class BtTreeView
        Inherits TreeView
        Private Const TVS_EX_DOUBLEBUFFER As Integer = &H4

        Private Const TVM_SETEXTENDEDSTYLE As Integer = &H1100 + 44

        <DllImport("user32.dll")>
        Private Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal msg As Integer, ByVal wp As IntPtr, ByVal lp As IntPtr) As IntPtr
        End Function

        Public Sub New()
            DoubleBuffered = True
            HideSelection = False
        End Sub

        Protected Overrides Sub OnHandleCreated(ByVal e As EventArgs)
            MyBase.OnHandleCreated(e)
            SendMessage(Handle, TVM_SETEXTENDEDSTYLE, CType(TVS_EX_DOUBLEBUFFER, IntPtr), CType(TVS_EX_DOUBLEBUFFER, IntPtr))
        End Sub
    End Class
End Namespace
