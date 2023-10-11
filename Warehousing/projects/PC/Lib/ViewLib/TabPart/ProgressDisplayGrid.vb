Imports System.Windows.Forms
Imports System.ComponentModel
Imports ViewLib.Basic
Imports Database.SqlData.Condition
Imports Database
Imports Database.SqlData.DisplayData
Imports System.Globalization
Imports InterfaceLib
Imports ProcessLib.PartProcess
Imports SettingLib
Imports SettingLib.Part
Imports Database.SqlData
Imports UtilityLib

Namespace TabPart
    Public Class ProgressDisplayGrid
        Inherits DataGridView
        Implements IUpdatablePart, ISheetChild

        <Browsable(True)>
        <DefaultValue(True)>
        Public Property ShowWorkListProgress() As Boolean = True

        <Browsable(True)>
        <DefaultValue(True)>
        Public Property ShowItemProgress() As Boolean = True

        <Browsable(True)>
        <DefaultValue(True)>
        Public Property ShowFigureProgress() As Boolean = True

        <Browsable(True)>
        <DefaultValue(True)>
        Public Property ShowOrderCount() As Boolean = True

        <Browsable(True)>
        <DefaultValue(True)>
        Public Property ShowProcessedCount() As Boolean = True

        <Browsable(True)>
        <DefaultValue(True)>
        Public Property ShowRest() As Boolean = True

        <Browsable(True)>
        <DefaultValue(True)>
        Public Property ShowProgressionRate() As Boolean = True

        <Browsable(True)>
        <DefaultValue(GetType(GridViewCellBackColor), "Black")>
        Public Property GridViewCellBackColor() As GridViewCellBackColor = GridViewCellBackColor.DarkGray

        <Browsable(True)>
        Public Property WorkListColumnText As String

        <Browsable(True)>
        Public Property ItemColumnText As String

        <Browsable(True)>
        Public Property FigureColumnText As String

        <Browsable(True)>
        Public Property OrderCountRowText As String

        <Browsable(True)>
        Public Property ProcessedCountRowText As String

        <Browsable(True)>
        Public Property RestRowText As String

        <Browsable(True)>
        Public Property ProgressionRateRowText As String

        <Browsable(True)>
        Public Property TextAlign() As DataGridViewContentAlignment = DataGridViewContentAlignment.MiddleLeft

        Public Property Setting() As ProgressDisplaySetting

        Public Sub New()
            RowHeadersVisible = False
            DoubleBuffered = True
            AllowUserToAddRows = False
            AllowUserToDeleteRows = False
            AllowUserToResizeRows = False
            [ReadOnly] = True
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells
            RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        End Sub

        Public Sub LoadSetting() Implements ISheetChild.LoadSetting
            If FileUtility.IsDesignMode(DesignMode) Then Return

            Dim parentSheet As BtSheet = TryCast(BasicUtility.GetParentControl(Of BtSheet)(Parent), BtSheet)
            If IsNothing(parentSheet) Then Return

            Dim pageSetting = parentSheet.SheetSetting
            If IsNothing(pageSetting) Then Return
            Setting = pageSetting.GuiPartSettingList.GetSetting(Name)

            CreateColumns()
            UpdateBackColor()
            ColumnHeadersHeight = ViewUtility.CalculateGridRowHeight(Font, ColumnHeadersHeight)
        End Sub

        Public Function UpdateView(ByVal dbAccessor As DbAccessor, ByVal tableName As String, ByVal conditionList As ConditionList,
                                   ByVal calculationColumnSettings As CalculationColumnDisplayDataList, ByVal autoUpdate As Boolean) As Boolean Implements IUpdatablePart.UpdateView
            If String.IsNullOrEmpty(Setting.OrderCountColumnName) OrElse
                String.IsNullOrEmpty(Setting.ProcessedCountColumnName) OrElse
                String.IsNullOrEmpty(Setting.WorkNoColumnName) Then Return True
            Dim fields As String() = dbAccessor.GetFields(tableName)
            Dim orderResult As String()() = GetOrderCount(tableName, dbAccessor)
            Dim remainResult As String()() = GetRemainingCount(tableName, dbAccessor)
            If Not fields.Contains(Setting.ProcessedCountColumnName) Then
                remainResult = orderResult
            End If
            If IsNothing(orderResult) OrElse IsNothing(remainResult) Then Return False

            Dim slipOrder As String = String.Empty
            Dim slipRemaining As String = String.Empty
            Dim slipCompletion As String = String.Empty
            Dim slipRate As String = String.Empty
            If ShowWorkListProgress Then
                slipOrder = orderResult(0)(2)
                slipRemaining = remainResult(0)(2)
                slipCompletion = GetCompletionCount(slipOrder, slipRemaining)
                slipRate = GetRateCount(slipOrder, slipCompletion)
            End If
            Dim itemOrder As String = orderResult(0)(1)
            Dim itemRemaining As String = remainResult(0)(1)
            Dim itemCompletion As String = GetCompletionCount(itemOrder, itemRemaining)
            Dim itemRate As String = GetRateCount(itemOrder, itemCompletion)
            Dim particularOrder As String = orderResult(0)(0)
            particularOrder = If(String.IsNullOrEmpty(particularOrder), "0", particularOrder)
            Dim particularRemaining As String = remainResult(0)(0)
            particularRemaining = If(String.IsNullOrEmpty(particularRemaining), "0", particularRemaining)
            Dim particularCompletion As String = GetCompletionCount(particularOrder, particularRemaining)
            Dim particularRate = GetRateCount(particularOrder, particularCompletion)

            Rows.Clear()
            Rows.Add(OrderCountRowText, slipOrder, itemOrder, particularOrder)
            Rows.Add(ProcessedCountRowText, slipCompletion, itemCompletion, particularCompletion)
            Rows.Add(RestRowText, slipRemaining, itemRemaining, particularRemaining)
            Rows.Add(ProgressionRateRowText, slipRate, itemRate, particularRate)
            UpdateRowsVisible()
            Return True
        End Function

        Private Sub UpdateRowsVisible()
            Rows(0).Visible = ShowOrderCount
            Rows(1).Visible = ShowProcessedCount
            Rows(2).Visible = ShowRest
            Rows(3).Visible = ShowProgressionRate
        End Sub

        Private Function GetRateCount(ByVal order As String, ByVal completion As String) As String
            Try
                Dim orderNum As Double = Convert.ToDouble(order)
                If orderNum = 0 Then Return "0%"
                Dim rate = Convert.ToDouble(completion) / orderNum * 100
                Return (Math.Floor(rate)).ToString("0\%", CultureInfo.InvariantCulture)
            Catch ex As Exception
                Return "0%"
            End Try
        End Function

        Private Function GetCompletionCount(ByVal order As String, ByVal remaining As String) As String
            Try
                Return (Convert.ToInt64(order) - Convert.ToInt64(remaining)).ToString(CultureInfo.InvariantCulture)
            Catch ex As Exception
                Return "0"
            End Try
        End Function

        Private Function GetRemainingCount(ByVal tableName As String, ByVal dbAccessor As DbAccessor) As String()()
            Dim command As New SqlSelectCommand(tableName)

            command.DisplayDataList.Add(New SumDisplayData(New SubtractionDisplayData(Setting.OrderCountColumnName, Setting.ProcessedCountColumnName)))
            command.DisplayDataList.Add(New CountConditionDisplayData())
            If ShowWorkListProgress Then
                command.DisplayDataList.Add(New CountFieldDisplayData(Setting.WorkNoColumnName, True))
            End If
            command.ConditionList.Add(New FieldConditionData(ConditionType.LessThan, Setting.ProcessedCountColumnName,
                                                             Setting.OrderCountColumnName, ComparisonType.Numeric))
            Try
                Return dbAccessor.ExecuteCommandAndRead(command)
            Catch ex As Exception
                Return New String()() {New String() {"0", "0", "0"}}
            End Try
        End Function

        Private Function GetOrderCount(ByVal tableName As String, ByVal dbAccessor As DbAccessor) As String()()
            Dim command As New SqlSelectCommand(tableName)
            command.DisplayDataList.Add(New SumDisplayData(Setting.OrderCountColumnName))
            command.DisplayDataList.Add(New CountConditionDisplayData())
            If ShowWorkListProgress Then
                command.DisplayDataList.Add(New CountFieldDisplayData(Setting.WorkNoColumnName, True))
            End If
            Try
                Return dbAccessor.ExecuteCommandAndRead(command)
            Catch ex As Exception
                Return New String()() {New String() {"0", "0", "0"}}
            End Try
        End Function

        Public Sub UpdateBackColor()
            EnableHeadersVisualStyles = False
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single
            DefaultCellStyle.BackColor = TableDisplayGridProcess.GetEvenNumberRowCellBackColor(GridViewCellBackColor)
            DefaultCellStyle.ForeColor = TableDisplayGridProcess.GetCellForeColor(GridViewCellBackColor)
            ColumnHeadersDefaultCellStyle.BackColor = TableDisplayGridProcess.GetHeaderBackColor(GridViewCellBackColor)
            ColumnHeadersDefaultCellStyle.ForeColor = TableDisplayGridProcess.GetHeaderForeColor(GridViewCellBackColor)
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        End Sub

        Private Sub CreateColumns()
            Columns.Add("RowHeaderColumn", "")
            Columns.Add("WorkListColumn", WorkListColumnText)
            Columns.Add("ItemColumn", ItemColumnText)
            Columns.Add("QuantityColumn", FigureColumnText)
            Columns(0).SortMode = DataGridViewColumnSortMode.NotSortable
            Columns(0).AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            Columns(0).DefaultCellStyle.BackColor = TableDisplayGridProcess.GetHeaderBackColor(GridViewCellBackColor)
            Columns(0).DefaultCellStyle.ForeColor = TableDisplayGridProcess.GetHeaderForeColor(GridViewCellBackColor)
            Columns(0).DefaultCellStyle.Alignment = TextAlign
            Columns(0).HeaderCell.Style.Alignment = TextAlign
            Columns(1).Visible = ShowWorkListProgress
            Columns(1).SortMode = DataGridViewColumnSortMode.NotSortable
            Columns(1).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            Columns(1).FillWeight = 100
            Columns(1).DefaultCellStyle.Alignment = TextAlign
            Columns(1).HeaderCell.Style.Alignment = TextAlign
            Columns(2).Visible = ShowItemProgress
            Columns(2).SortMode = DataGridViewColumnSortMode.NotSortable
            Columns(2).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            Columns(2).FillWeight = 100
            Columns(2).DefaultCellStyle.Alignment = TextAlign
            Columns(2).HeaderCell.Style.Alignment = TextAlign
            Columns(3).Visible = ShowFigureProgress
            Columns(3).SortMode = DataGridViewColumnSortMode.NotSortable
            Columns(3).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            Columns(3).FillWeight = 100
            Columns(3).DefaultCellStyle.Alignment = TextAlign
            Columns(3).HeaderCell.Style.Alignment = TextAlign
            RowTemplate.Height = 21
            RowHeadersWidth = 80
        End Sub
    End Class
End Namespace
