﻿Namespace Part
    Public Class AdjustInventoryButtonSetting
        Implements IGuiPartSetting

        Public Property Name As String Implements IGuiPartSetting.Name

        Public Property ActualStockField() As String

        Public Property TheoreticalStockField() As String
    End Class
End Namespace
