Imports SettingLib.Control

Namespace Part

    Public Class FormPrintButtonSetting
        Implements IGuiPartSetting

        Public Property Name() As String Implements IGuiPartSetting.Name

        Public Property PrintFormat() As PrintFormat

        Public Property Title() As String

        Public Property SlipColumnName() As String

        Public ReadOnly Property SlipColumnIndex() As Integer
            Get
                For index As Integer = 0 To PrintColumnSettings.Count - 1
                    If PrintColumnSettings(index).Name = SlipColumnName Then Return index
                Next
                Return -1
            End Get
        End Property

        Public Property PrintColumnSettings() As New List(Of PrintColumnSetting)

        Public Property BarcodeKind() As BarcodeKind = BarcodeKind.C128

        Public Function GetBarcodeColumnNumbers() As List(Of Integer)
            Dim list As New List(Of Integer)()
            If IsNothing(PrintColumnSettings) Then Return list
            For number As Integer = 0 To PrintColumnSettings.Count - 1
                If PrintColumnSettings(number).AddBarcode Then
                    list.Add(number)
                End If
            Next
            Return list
        End Function

        Public Function Clone() As FormPrintButtonSetting
            Return New FormPrintButtonSetting() With {
                .Name = Name,
                .PrintFormat = PrintFormat,
                .Title = Title,
                .SlipColumnName = SlipColumnName,
                .PrintColumnSettings = GetCloneList(),
                .BarcodeKind = BarcodeKind
            }
        End Function

        Private Function GetCloneList() As List(Of PrintColumnSetting)
            Dim list As New List(Of PrintColumnSetting)
            For Each setting As PrintColumnSetting In PrintColumnSettings
                list.Add(setting.Clone())
            Next
            Return list
        End Function
    End Class
End Namespace
