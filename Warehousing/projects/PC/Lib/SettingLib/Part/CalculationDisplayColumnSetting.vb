Imports SettingLib.Table

Namespace Part
    Public Class CalculationDisplayColumnSetting
        Inherits DisplayColumnSetting
        Public Property ShowDecimalPoint As Boolean

        Public Property DecimalPlace As Integer

        Public Property ShowPercent As Boolean

        Public Sub New(ByVal name As String, ByVal displayName As String, ByVal width As Integer,
               ByVal columnAppearanceSettings As ColumnAppearanceSetting, ByVal showPoint As Boolean, ByVal decimalPlace As Integer,
               ByVal percent As Boolean)
            MyBase.New(name, displayName, width, columnAppearanceSettings)
            ShowDecimalPoint = showPoint
            _DecimalPlace = decimalPlace
            ShowPercent = percent
        End Sub
    End Class
End Namespace
