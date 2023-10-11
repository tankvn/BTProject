Imports SettingLib.Part

Namespace Control
    Public Class TableOutputControlSetting
        Implements IControlPartSetting

        Public Property Name() As String Implements IControlPartSetting.Name

        Public Property FileName() As String

        Public Property OutputType() As OutputType

        Public Property AddDoubleQuotes() As Boolean

        Public Property PrintPerSlip() As Boolean

        Public Property SlipColumnNumber() As Integer

        Public Property PrintColumnSettings() As New List(Of PrintColumnSetting)

        Public Function GetBarcodeColumnNumber() As List(Of Integer)
            Dim list As New List(Of Integer)()
            If IsNothing(PrintColumnSettings) Then Return list
            For number As Integer = 0 To PrintColumnSettings.Count - 1
                If PrintColumnSettings(number).AddBarCode Then
                    list.Add(number)
                End If
            Next
            Return list
        End Function
    End Class
End Namespace
