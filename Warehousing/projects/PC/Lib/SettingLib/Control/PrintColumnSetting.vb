Imports Database.SqlData

Namespace Control
    Public Class PrintColumnSetting

        Public Property Name() As String

        Public Property DisplayName() As String

        Public Property Type() As DataType

        Public Property PrintLocation() As PrintLocation

        Public Property AddBarcode() As Boolean

        Public Property WidthWeight As Integer

        Public Sub New()
            Name = String.Empty
            DisplayName = String.Empty
            Type = DataType.Text
            PrintLocation = PrintLocation.List
            AddBarcode = False
            WidthWeight = 0
        End Sub

        Public Function Clone() As PrintColumnSetting
            Return New PrintColumnSetting() With {
                .Name = Name,
                .DisplayName = DisplayName,
                .Type = Type,
                .PrintLocation = PrintLocation,
                .AddBarcode = AddBarcode,
                .WidthWeight = WidthWeight
            }
        End Function
    End Class
End Namespace
