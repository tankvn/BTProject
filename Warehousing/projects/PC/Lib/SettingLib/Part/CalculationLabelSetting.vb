Namespace Part
    Public Class CalculationLabelSetting
        Implements IGuiPartSetting

        Public Property Name As String Implements IGuiPartSetting.Name

        Public Property CalculationLabelType() As CalculationLabelType

        Public Property TargetColumn() As String

        Public Property Prefix() As String

        Public Property Suffix() As String

        Public Property UseNumberGroupSeparator() As Boolean
    End Class
End Namespace
