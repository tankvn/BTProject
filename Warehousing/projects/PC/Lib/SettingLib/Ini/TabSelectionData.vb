Imports System.Globalization

Namespace Ini
    Public Class TabSelectionData
        Implements IIniSetting
        Public ReadOnly Property SettingName() As String Implements IIniSetting.SettingName
            Get
                Return Me.GetType().Name
            End Get
        End Property

        Public Property ParentName() As String Implements IIniSetting.SheetName

        Public Property ControlName() As String Implements IIniSetting.ControlName

        Public ReadOnly Property Data() As String() Implements IIniSetting.Data
            Get
                Return New String() {TabIndex.ToString(CultureInfo.InvariantCulture)}
            End Get
        End Property

        Public Property TabIndex() As Integer

        Public Sub New(ByVal parent As String, ByVal control As String, ByVal index As Integer)
            ParentName = parent
            ControlName = control
            TabIndex = index
        End Sub
    End Class
End Namespace
