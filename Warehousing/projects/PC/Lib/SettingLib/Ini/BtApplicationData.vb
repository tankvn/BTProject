Imports System.Drawing

Namespace Ini
    Public Class BtApplicationData

        Public Property TypeName() As String

        Public Property WindowBounds() As Rectangle

        Public Property AppMaximized() As Boolean

        Public Property AutoUpdateEnabled() As Boolean

        Public Property UpdateFrequency() As Integer = 60000

        Public Property SelectedWindow() As String

    End Class
End Namespace
