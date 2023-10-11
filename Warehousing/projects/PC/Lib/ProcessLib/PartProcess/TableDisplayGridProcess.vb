Imports System.Windows.Forms
Imports System.Drawing
Imports SettingLib

Namespace PartProcess

    Public Class TableDisplayGridProcess
        Public Shared Function SortDataTable(ByVal table As DataTable, ByVal index As Integer, ByVal order As SortOrder) As DataTable
            If index < 0 OrElse IsNothing(table) Then Return table
            table.CaseSensitive = True
            Dim dataView As DataView = New DataView(table)
            Dim sourceName As String = table.Columns(index).ColumnName
            Dim tempColumnName As String = GetTempColumnName(table)
            table.Columns(index).ColumnName = tempColumnName
            dataView.Sort = tempColumnName + " " + GetSortOrderString(order)
            table.Columns(index).ColumnName = sourceName
            Dim sortedTable As DataTable = table.Clone
            For Each row As DataRowView In dataView
                sortedTable.ImportRow(row.Row)
            Next
            Return sortedTable
        End Function

        Private Shared Function GetTempColumnName(ByVal table As DataTable) As String
            Dim tempName As String
            Do
                tempName = Guid.NewGuid().ToString()
            Loop While table.Columns.Contains(tempName)
            Return tempName
        End Function

        Private Shared Function GetSortOrderString(ByVal order As SortOrder) As String
            Select Case order
                Case SortOrder.Ascending
                    Return "ASC"
                Case SortOrder.Descending
                    Return "DESC"
            End Select
            Return Nothing
        End Function

        Public Shared Function GetHeaderBackColor(ByVal color As GridViewCellBackColor) As Color
            Select Case color
                Case GridViewCellBackColor.LightGray
                    Return Drawing.Color.FromArgb(234, 240, 245)
                Case GridViewCellBackColor.DarkGray
                    Return Drawing.Color.FromArgb(154, 154, 154)
                Case GridViewCellBackColor.Blue
                    Return Drawing.Color.FromArgb(71, 123, 186)
                Case GridViewCellBackColor.Green
                    Return Drawing.Color.FromArgb(150, 184, 81)
                Case GridViewCellBackColor.Red
                    Return Drawing.Color.FromArgb(189, 72, 69)
                Case GridViewCellBackColor.Orange
                    Return Drawing.Color.FromArgb(247, 144, 59)
                Case Else
                    Return Drawing.Color.Black
            End Select
        End Function


        Public Shared Function GetHeaderForeColor(ByVal color As GridViewCellBackColor) As Color
            Select Case color
                Case GridViewCellBackColor.LightGray
                    Return Drawing.Color.FromArgb(39, 65, 62)
                Case GridViewCellBackColor.DarkGray
                    Return Drawing.Color.FromArgb(240, 240, 240)
                Case GridViewCellBackColor.Blue
                    Return Drawing.Color.FromArgb(230, 237, 246)
                Case GridViewCellBackColor.Green
                    Return Drawing.Color.FromArgb(241, 245, 232)
                Case GridViewCellBackColor.Red
                    Return Drawing.Color.FromArgb(246, 230, 230)
                Case GridViewCellBackColor.Orange
                    Return Drawing.Color.FromArgb(253, 229, 209)
                Case Else
                    Return Drawing.Color.FromArgb(39, 65, 62)
            End Select
        End Function

        Public Shared Function GetCellForeColor(ByVal color As GridViewCellBackColor) As Color
            Select Case color
                Case GridViewCellBackColor.LightGray
                    Return Drawing.Color.FromArgb(0, 0, 0)
                Case GridViewCellBackColor.DarkGray
                    Return Drawing.Color.FromArgb(0, 0, 0)
                Case GridViewCellBackColor.Blue
                    Return Drawing.Color.FromArgb(37, 38, 39)
                Case GridViewCellBackColor.Green
                    Return Drawing.Color.FromArgb(38, 39, 37)
                Case GridViewCellBackColor.Red
                    Return Drawing.Color.FromArgb(39, 37, 37)
                Case GridViewCellBackColor.Orange
                    Return Drawing.Color.FromArgb(43, 41, 40)
                Case Else
                    Return Drawing.Color.Gainsboro
            End Select
        End Function

        Public Shared Function GetOddNumberRowCellBackColor(ByVal color As GridViewCellBackColor) As Color
            Return Drawing.Color.FromArgb(248, 248, 248)
        End Function

        Public Shared Function GetEvenNumberRowCellBackColor(ByVal color As GridViewCellBackColor) As Color
            Select Case color
                Case GridViewCellBackColor.LightGray
                    Return Drawing.Color.FromArgb(248, 248, 248)
                Case GridViewCellBackColor.DarkGray
                    Return Drawing.Color.FromArgb(242, 242, 242)
                Case GridViewCellBackColor.Blue
                    Return Drawing.Color.FromArgb(243, 247, 254)
                Case GridViewCellBackColor.Green
                    Return Drawing.Color.FromArgb(249, 253, 244)
                Case GridViewCellBackColor.Red
                    Return Drawing.Color.FromArgb(254, 243, 243)
                Case GridViewCellBackColor.Orange
                    Return Drawing.Color.FromArgb(255, 249, 243)
                Case Else
                    Return Drawing.Color.FromArgb(248, 248, 248)
            End Select
        End Function

        Public Shared Function GetAlignment(ByVal alignment As HorizontalAlignment) As DataGridViewContentAlignment
            Select Case alignment
                Case HorizontalAlignment.Left
                    Return DataGridViewContentAlignment.MiddleLeft
                Case HorizontalAlignment.Center
                    Return DataGridViewContentAlignment.MiddleCenter
                Case HorizontalAlignment.Right
                    Return DataGridViewContentAlignment.MiddleRight
            End Select
            Return DataGridViewContentAlignment.MiddleCenter
        End Function
    End Class
End Namespace
