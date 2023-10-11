Imports System.Windows.Forms
Imports System.Drawing
Imports InterfaceLib
Imports SettingLib.Part
Imports UtilityLib

Public Class ViewUtility
    Private Const COLUMN_HEADERS_HEIGHT_MARGIN As Integer = 9

    Public Shared Function GetPageSetting(ByVal parentControl As Control,
                                ByVal designMode As Boolean) As SheetSetting

        If FileUtility.IsDesignMode(designMode) Then Return Nothing
        If IsNothing(parentControl) Then Return Nothing

        Dim parentSheet As IBtSheet = TryCast(BasicUtility.GetParentControl(Of IBtSheet)(parentControl), IBtSheet)
        If IsNothing(parentSheet) Then Return Nothing

        Return parentSheet.SheetSetting
    End Function

    Public Shared Function GetPartSetting(ByVal name As String, ByVal parent As Control, ByVal designMode As Boolean)
        Dim pageSetting As SheetSetting = GetPageSetting(parent, designMode)
        If IsNothing(pageSetting) Then Return Nothing
        Return pageSetting.GuiPartSettingList.GetSetting(name)
    End Function

    Public Shared Function CalculateGridRowHeight(ByVal font As Font, ByVal defaultHeight As Integer)
        Return If(IsNothing(font), defaultHeight, font.Height + COLUMN_HEADERS_HEIGHT_MARGIN)
    End Function
End Class
