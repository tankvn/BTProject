Imports System.Windows.Forms
Imports Database.SqlData.Condition
Imports Database
Imports Database.SqlData.DisplayData
Imports SettingLib.Part
Imports Database.SqlData
Imports Database.Utility
Imports UtilityLib

Namespace TabPart
    Public Class LogDisplayGrid
        Inherits TableDisplayGrid

        Private WithEvents _contextMenuStrip As New ContextMenuStrip

        Public Event Retransmission(ByVal sender As Object, ByVal fields As List(Of String))

        Public Event RetransmissionAll(ByVal sender As Object, ByVal e As EventArgs)

        Public Sub New()
            _contextMenuStrip.Items.Add(My.Resources.ResendSelectRecord)
            _contextMenuStrip.Items.Add(My.Resources.ResendAllRecord)
        End Sub

        Protected Overrides Sub OnCellContextMenuStripNeeded(ByVal e As DataGridViewCellContextMenuStripNeededEventArgs)
            MyBase.OnCellContextMenuStripNeeded(e)
            If (0 <= e.ColumnIndex) AndAlso (0 <= e.RowIndex) Then
                ClearSelection()
                Me(e.ColumnIndex, e.RowIndex).Selected = True
                If String.IsNullOrEmpty(TableDisplayGridSetting.STATUS_COLUMN_NAME) OrElse
                    Not Columns.Contains(TableDisplayGridSetting.STATUS_COLUMN_NAME) Then Exit Sub
                If TypeOf (Me(TableDisplayGridSetting.STATUS_COLUMN_NAME, e.RowIndex).Value) Is DBNull Then Exit Sub
                _contextMenuStrip.Items(0).Enabled = CanResendData(e.RowIndex)
                e.ContextMenuStrip = _contextMenuStrip
            End If
        End Sub

        Private Function CanResendData(ByVal rowIndex As Integer) As Boolean
            Return (Me(TableDisplayGridSetting.STATUS_COLUMN_NAME, rowIndex).Value.Equals(GetResultString(DbUpdateResult.NG))) OrElse
                   (Me(TableDisplayGridSetting.STATUS_COLUMN_NAME, rowIndex).Value.Equals(GetResultString(DbUpdateResult.Skip))) OrElse
                   (Me(TableDisplayGridSetting.STATUS_COLUMN_NAME, rowIndex).Value.Equals(GetResultString(DbUpdateResult.Incomplete)))
        End Function

        Public Overrides Function UpdateView(ByVal dbAccessor As DbAccessor, ByVal tableName As String,
                                             ByVal conditionList As ConditionList, ByVal calculationColumnSettings As CalculationColumnDisplayDataList,
                                             ByVal autoUpdate As Boolean) As Boolean
            Dim result As Boolean = MyBase.UpdateView(dbAccessor, tableName, conditionList, calculationColumnSettings, autoUpdate)
            Try
                If 0 < Rows.Count Then
                    FirstDisplayedScrollingRowIndex = Rows.Count - 1
                End If
            Catch ex As Exception
            End Try
            Return result
        End Function

        Private Sub _contextMenuStrip_Clicked(ByVal sender As Object, ByVal e As ToolStripItemClickedEventArgs) _
            Handles _contextMenuStrip.ItemClicked
            Try
                WaitDialogController.ShowWaitDialog(FindForm())
                If e.ClickedItem.Text = My.Resources.ResendSelectRecord Then
                    Dim selectedRowIndexList As New List(Of Integer)
                    For Each cell As DataGridViewCell In SelectedCells
                        If selectedRowIndexList.Contains(cell.RowIndex) Then Continue For
                        RaiseEvent Retransmission(Me, GetRowValues(cell.RowIndex))
                        selectedRowIndexList.Add(cell.RowIndex)
                    Next
                End If
                If e.ClickedItem.Text = My.Resources.ResendAllRecord Then
                    RaiseEvent RetransmissionAll(Me, New EventArgs())
                End If
            Catch ex As Exception
                MessageBox.Show(FindForm(), ex.Message, My.Application.Info.AssemblyName, MessageBoxButtons.OK,
                                MessageBoxIcon.Error)
            Finally
                WaitDialogController.CloseWaitDialog(FindForm())
            End Try
        End Sub

        Private Function GetRowValues(ByVal rowIndex As Integer) As List(Of String)
            Dim rowStringList As New List(Of String)
            For Each cell As DataGridViewCell In Rows(rowIndex).Cells
                If (TypeOf (cell.Value) Is DBNull) Then
                    rowStringList.Add(String.Empty)
                    Continue For
                End If
                rowStringList.Add(cell.Value)
            Next
            Return rowStringList
        End Function
    End Class
End Namespace
