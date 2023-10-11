Imports System.Globalization

Namespace Ini
    Public Class ColumnWidthData
        Implements IIniSetting

        Public ReadOnly Property SettingName() As String Implements IIniSetting.SettingName
            Get
                Return Me.GetType().Name
            End Get
        End Property

        Public Property SheetName() As String Implements IIniSetting.SheetName

        Public Property ControlName() As String Implements IIniSetting.ControlName

        Public ReadOnly Property Data() As String() Implements IIniSetting.Data
            Get
                Dim list As New List(Of String)
                For Each width As Integer In WidthList
                    list.Add(width.ToString(CultureInfo.InvariantCulture))
                Next
                Return list.ToArray()
            End Get
        End Property

        Public Property WidthList() As New List(Of Integer)

        Public Sub New(ByVal sheet As String, ByVal control As String, ByVal widths As IEnumerable(Of Integer))
            SheetName = sheet
            ControlName = control
            WidthList.AddRange(widths)
        End Sub
    End Class
End Namespace
