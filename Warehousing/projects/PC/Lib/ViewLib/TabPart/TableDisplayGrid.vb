Imports System.Windows.Forms
Imports System.Drawing
Imports System.Globalization
Imports InterfaceLib
Imports System.ComponentModel
Imports Database.SqlData.Condition.Member
Imports SettingLib
Imports SettingLib.Table
Imports Database.SqlData
Imports ViewLib.Basic
Imports Database.SqlData.Condition
Imports Database
Imports Database.SqlData.DisplayData
Imports Database.DbException
Imports ProcessLib.PartProcess
Imports SettingLib.Part
Imports UtilityLib

Namespace TabPart
    Public Class TableDisplayGrid
        Inherits DataGridView
        Implements IUpdatablePart, IDataClearablePart, ISheetChild

        Private _initialized As Boolean = False

        Private _bindingSource As BindingSource

        Private ReadOnly _displayColumnList As New List(Of DisplayColumnSetting)()

        Private _numericColumns As List(Of String)

        Private _sortIndex As Integer = -1
        Public ReadOnly Property SortIndex() As Integer
            Get
                Return _sortIndex
            End Get
        End Property

        Private _sortOrder As SortOrder = SortOrder.None
        Public Shadows ReadOnly Property SortOrder() As SortOrder
            Get
                Return _sortOrder
            End Get
        End Property

        Private _columnWidths As Integer()

        Private _tableName As String

        Public Setting As TableDisplayGridSetting

        <Browsable(True)>
        <DefaultValue(GetType(GridViewCellBackColor), "Black")>
        Public Property GridViewCellBackColor() As GridViewCellBackColor = GridViewCellBackColor.DarkGray

        <Browsable(True)>
        <DefaultValue(True)>
        Public Property Stripe() As Boolean = True

        Private _rowHeadersFont As Font
        <Browsable(True)>
        Public Property RowHeadersFont() As Font
            Get
                If IsNothing(_rowHeadersFont) Then
                    Return DefaultCellStyle.Font
                End If
                Return _rowHeadersFont
            End Get
            Set(value As Font)
                _rowHeadersFont = value
            End Set
        End Property

        <Browsable(True)>
        Public Property RowHeaderForeColor() As Color = Color.Black

        <Browsable(True)>
        Public Property ColumnHeaderForeColor() As Color = Color.Black

        <Browsable(True)>
        Public Property ColumnHeadersAlignment() As HorizontalAlignment

        <Browsable(True)>
        Public Property ColumnHeadersFont() As Font

        Public ReadOnly Property DataTable() As DataTable
            Get
                If IsNothing(_bindingSource) Then Return Nothing
                Return _bindingSource.DataSource
            End Get
        End Property

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub InitializeComponent()
            CType(Me, ISupportInitialize).BeginInit()
            SuspendLayout()
            AllowUserToAddRows = False
            AllowUserToDeleteRows = False
            AllowUserToResizeRows = False
            Margin = New Padding(5)
            MultiSelect = False
            Me.ReadOnly = True
            RowHeadersWidth = 65
            RowTemplate.Height = 21
            SelectionMode = DataGridViewSelectionMode.FullRowSelect
            VirtualMode = True
            DoubleBuffered = True
            CType(Me, ISupportInitialize).EndInit()
            ResumeLayout(False)

        End Sub

        Public Sub LoadSetting() Implements ISheetChild.LoadSetting
            If FileUtility.IsDesignMode(DesignMode) Then Return

            Dim parentSheet As BtSheet = TryCast(BasicUtility.GetParentControl(Of BtSheet)(Parent), BtSheet)
            If IsNothing(parentSheet) Then Return

            Dim pageSetting = parentSheet.SheetSetting
            If IsNothing(pageSetting) Then Return
            Setting = pageSetting.GuiPartSettingList.GetSetting(Name)
            If Not IsNothing(pageSetting.DisplayColumnList) Then
                _displayColumnList.AddRange(pageSetting.DisplayColumnList)
            End If
            If Not IsNothing(pageSetting.HistoryColumnList) Then
                _displayColumnList.AddRange(pageSetting.HistoryColumnList)
            End If
            _numericColumns = pageSetting.NumericColumnNameList
            _columnWidths = SettingsUtility.GetColumnWidths(parentSheet.Name, Name)
            _tableName = pageSetting.TableName
            ColumnHeadersVisible = pageSetting.ShowColumnHeader
            RowHeadersVisible = pageSetting.ShowRowNumber

            InitializeView()
            CreateColumnList()
            UpdateHistoryColumnName()
            _initialized = True
        End Sub

        Public Sub InitializeView()
            EnableHeadersVisualStyles = False
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single
            DefaultCellStyle.BackColor = TableDisplayGridProcess.GetEvenNumberRowCellBackColor(GridViewCellBackColor)
            If Stripe Then
                AlternatingRowsDefaultCellStyle.BackColor = TableDisplayGridProcess.GetOddNumberRowCellBackColor(GridViewCellBackColor)
            End If
            ColumnHeadersDefaultCellStyle.Alignment = TableDisplayGridProcess.GetAlignment(ColumnHeadersAlignment)
            ColumnHeadersDefaultCellStyle.BackColor = TableDisplayGridProcess.GetHeaderBackColor(GridViewCellBackColor)
            ColumnHeadersDefaultCellStyle.Font = ColumnHeadersFont
            ColumnHeadersDefaultCellStyle.ForeColor = ColumnHeaderForeColor
            ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            ColumnHeadersHeight = ViewUtility.CalculateGridRowHeight(ColumnHeadersFont, 24)
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None
            RowTemplate.Height = ViewUtility.CalculateGridRowHeight(Font, 24)
        End Sub

        Public Sub ClearData() Implements IDataClearablePart.ClearData
            If Not IsNothing(DataTable) Then
                DataTable.Dispose()
            End If
            If Not IsNothing(_bindingSource) Then
                _bindingSource.Dispose()
                _bindingSource = Nothing
            End If
        End Sub

        Public Overridable Function UpdateView(ByVal dbAccessor As DbAccessor, ByVal tableName As String, ByVal conditionList As ConditionList,
                                               ByVal calculationColumnSettings As CalculationColumnDisplayDataList, ByVal autoUpdate As Boolean) As Boolean Implements IUpdatablePart.UpdateView
            If FileUtility.IsDesignMode(DesignMode) Then Return True

            If Not _initialized Then
                OnCreateControl()
            End If
            Try
                BasicUtility.SuspendUpdate(Me)
                ClearData()
                _bindingSource = dbAccessor.CreateBindingSource(tableName, conditionList, ConvertUtility.ToDisplayDataList(_displayColumnList, calculationColumnSettings))
                RowCount = DataTable.Rows.Count
                If autoUpdate Then
                    SortData(_sortIndex, False, True)
                Else
                    ClearSortInfo()
                End If
                Return True
            Catch ex As DbAccessException
                ErrorUtility.OutputErrorLog(ex.Message, String.Empty)
                MessageBox.Show(ex.Message, My.Application.Info.AssemblyName, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            Finally
                BasicUtility.ResumeUpdate(Me)
            End Try
        End Function

        Private Sub ClearSortInfo()
            If (0 <= _sortIndex AndAlso _sortIndex < Columns.Count) Then
                Columns(_sortIndex).HeaderCell.SortGlyphDirection = SortOrder.None
            End If
            _sortIndex = -1
            _sortOrder = SortOrder.None
        End Sub

        Public Sub DestockingExecuted(ByVal sender As Object, ByVal e As EventArgs)
            UpdateHistoryColumnName()
        End Sub

        Private Sub CreateColumnList()
            Dim columnList As New List(Of DataGridViewColumn)
            Dim columnNumber = 1
            For Each displayColumnSetting As DisplayColumnSetting In _displayColumnList
                Dim gridColumn As New DataGridViewTextBoxColumn()
                gridColumn.HeaderText = displayColumnSetting.DisplayName
                gridColumn.Name = displayColumnSetting.Name
                gridColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                gridColumn.Width = If(columnNumber - 1 < _columnWidths.Length,
                                      _columnWidths(columnNumber - 1),
                                      displayColumnSetting.Width)
                Dim calculationColumn As CalculationDisplayColumnSetting = TryCast(displayColumnSetting, CalculationDisplayColumnSetting)

                If IsNumericColumn(gridColumn.Name) OrElse Not IsNothing(calculationColumn) Then
                    gridColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                Else
                    gridColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                End If

                If Not IsNothing(calculationColumn) Then
                    If (calculationColumn.ShowDecimalPoint AndAlso (0 < calculationColumn.DecimalPlace)) Then
                        gridColumn.DefaultCellStyle.Format = "0."
                        For count = 0 To calculationColumn.DecimalPlace - 1
                            gridColumn.DefaultCellStyle.Format += "0"
                        Next
                    Else
                        gridColumn.DefaultCellStyle.Format = "0"
                    End If

                    If calculationColumn.ShowPercent Then
                        If String.IsNullOrEmpty(gridColumn.DefaultCellStyle.Format) Then
                            gridColumn.DefaultCellStyle.Format = "0"
                        End If
                        gridColumn.DefaultCellStyle.Format += "\%"
                    End If
                End If

                columnList.Add(gridColumn)
                columnNumber += 1
            Next
            Columns.Clear()
            Columns.AddRange(columnList.ToArray())
        End Sub

        Public Sub UpdateHistoryColumnName(ByVal tableName As String)
            If _tableName <> tableName Then Return
            UpdateHistoryColumnName()
        End Sub

        Private Sub UpdateHistoryColumnName()
            Try
                Dim accessor As DbAccessor = DbAccessManager.GetSQLiteAccessor(FileUtility.DbFilePath)
                Dim historyColumnNames As String() = DestockingButtonProcess.CreateHistoryColumnNames().ToArray()
                Dim historyColumnDisplayNames As String() = DestockingButtonProcess.LoadHistoryColumnNames(accessor, _tableName)
                If IsNothing(historyColumnDisplayNames) OrElse historyColumnDisplayNames.Length = 0 Then Return
                For index As Integer = 0 To historyColumnNames.Count - 1
                    For Each column As DataGridViewColumn In Columns
                        If column.Name = historyColumnNames(index) Then
                            column.HeaderText = historyColumnDisplayNames(index)
                        End If
                    Next
                Next
            Catch ex As Exception
                ErrorUtility.OutputErrorLog(ex.Message, String.Empty)
            End Try
        End Sub

        Private Function IsNumericColumn(ByVal columnNumber As String) As Boolean
            If IsNothing(_numericColumns) Then Return False
            Return _numericColumns.Contains(columnNumber)
        End Function

        Protected Overrides Sub OnCellValueNeeded(ByVal e As DataGridViewCellValueEventArgs)
            MyBase.OnCellValueNeeded(e)
            If IsNothing(DataTable) Then Return
            If (DataTable.Rows.Count <= e.RowIndex) Then Return
            If (DataTable.Columns.Count <= e.ColumnIndex) Then Return

            e.Value = DataTable.Rows(e.RowIndex).ItemArray(e.ColumnIndex)
        End Sub

        Protected Overrides Sub OnCellPainting(ByVal e As DataGridViewCellPaintingEventArgs)
            MyBase.OnCellPainting(e)
            If Not RowHeadersVisible Then Return
            If (0 <= e.ColumnIndex OrElse e.RowIndex < 0) Then Return

            e.Paint(e.ClipBounds, DataGridViewPaintParts.All)

            Dim rect As Rectangle = e.CellBounds
            rect.Inflate(-2, -2)
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(), RowHeadersFont, rect,
                                  RowHeaderForeColor, TextFormatFlags.Right Or TextFormatFlags.VerticalCenter)
            Rows(e.RowIndex).Height = Math.Max(RowHeadersFont.Height, Rows(e.RowIndex).Height)
            e.Handled = True
        End Sub

        Protected Overrides Sub OnCellFormatting(ByVal e As DataGridViewCellFormattingEventArgs)
            MyBase.OnCellFormatting(e)
            If _displayColumnList.Count <= 0 Then Return
            Dim appearanceSetting As ColumnAppearanceSetting = _displayColumnList(e.ColumnIndex).ColumnAppearanceSetting
            If IsNothing(appearanceSetting) Then Return
            If Not JudgeAppearanceCondition(appearanceSetting, e.RowIndex) Then Return

            SetCellAppearance(e, appearanceSetting)
        End Sub

        Private Function JudgeAppearanceCondition(ByVal appearanceSetting As ColumnAppearanceSetting,
                                                  ByVal rowIndex As Integer) As Boolean
            If appearanceSetting.Condition.Count = 0 Then Return False

            If IsNothing(DataTable) Then Return False

            For Each condition As DisplayConditionSetting In appearanceSetting.Condition
                Dim compareResult As Boolean
                Dim comparisonValue As Object = GetComparisonValue(condition, rowIndex)
                Dim inputValue As Object = GetInputValue(condition, rowIndex)

                compareResult = Not IsNothing(comparisonValue) AndAlso Not IsNothing(inputValue) AndAlso Compare(condition, comparisonValue, inputValue)

                Select Case appearanceSetting.CombinationMethod
                    Case DeterminationMethod.AndCombining
                        If Not compareResult Then
                            Return False
                        End If
                    Case DeterminationMethod.OrCombining
                        If compareResult Then
                            Return True
                        End If
                End Select
            Next

            Select Case appearanceSetting.CombinationMethod
                Case DeterminationMethod.AndCombining
                    Return True
                Case DeterminationMethod.OrCombining
                    Return False
            End Select

            Return False
        End Function

        Private Function GetComparisonValue(ByVal condition As DisplayConditionSetting, ByVal rowIndex As Integer) As Object
            If condition.ComparisonMemberType = MemberType.CurrentDate Then Return DateTime.Now.ToString(DateTimeUtility.SQLITE_DATE_FORMAT)
            If DataTable.Rows.Count <= rowIndex Then Return Nothing
            If DataTable.Columns.Contains(condition.ComparisonData) Then
                Return DataTable.Rows(rowIndex).Item(condition.ComparisonData)
            End If
            Return Nothing
        End Function

        Private Function GetInputValue(ByVal condition As DisplayConditionSetting, ByVal rowIndex As Integer) As Object
            If DataTable.Rows.Count <= rowIndex Then Return Nothing
            Dim inputValue As Object = Nothing
            Select Case condition.InputMemberType
                Case MemberType.Field
                    If Not DataTable.Columns.Contains(condition.InputData) Then Return Nothing
                    inputValue = DataTable.Rows(rowIndex).Item(condition.InputData).ToString()
                Case MemberType.FixedValue
                    inputValue = condition.InputData
                Case MemberType.CurrentDate
                    inputValue = DateTime.Now.ToString(DateTimeUtility.SQLITE_DATE_FORMAT)
                Case MemberType.CurrentTime
                    Throw New ArgumentException
            End Select
            Return inputValue
        End Function

        Private Function Compare(ByVal condition As DisplayConditionSetting, ByVal comparisonValue As Object, ByVal inputValue As Object) As Boolean

            Select Case condition.ValueType
                Case DataType.Text
                    Return CompareString(condition, comparisonValue.ToString(), inputValue.ToString())
                Case DataType.IntegerNumber, DataType.FloatNumber
                    Return CompareNumeric(condition, comparisonValue, inputValue)
                Case DataType.DateText
                    Return CompareDate(condition, comparisonValue.ToString(), inputValue.ToString())
            End Select

            Return False
        End Function

        Private Function CompareDate(ByVal condition As DisplayConditionSetting, ByVal comparisonValue As String,
                                     ByVal inputValue As String) As Boolean
            Try
                Dim comparisonDate As DateTime = DateTime.ParseExact(comparisonValue, DateTimeUtility.SQLITE_DATE_FORMAT,
                                                                     CultureInfo.InvariantCulture)
                Dim inputDate As DateTime = DateTime.ParseExact(inputValue, DateTimeUtility.SQLITE_DATE_FORMAT,
                                                                CultureInfo.InvariantCulture)
                Dim calculationValue As Integer = condition.CalculationValue
                If condition.CalculationType = CalculationType.Subtraction Then
                    calculationValue = calculationValue * -1
                End If
                Select Case condition.CalculationUnit
                    Case DateUnit.Year
                        comparisonDate = comparisonDate.AddYears(calculationValue)
                    Case DateUnit.Month
                        comparisonDate = comparisonDate.AddMonths(calculationValue)
                    Case DateUnit.Day
                        comparisonDate = comparisonDate.AddDays(calculationValue)
                End Select

                Select Case condition.ConditionType
                    Case ConditionType.Equal
                        Return comparisonDate.Equals(inputDate)
                    Case ConditionType.NotEqual
                        Return Not comparisonDate.Equals(inputDate)
                    Case ConditionType.LessThan
                        Return comparisonDate < inputDate
                    Case ConditionType.MoreThan
                        Return inputDate < comparisonDate
                    Case ConditionType.AndLess
                        Return comparisonDate <= inputDate
                    Case ConditionType.AndMore
                        Return inputDate <= comparisonDate
                End Select

                Return False
            Catch ex As Exception
                Return False
            End Try
        End Function

        Private Function CompareNumeric(ByVal condition As DisplayConditionSetting, ByVal comparisonValue As Object,
                                        ByVal inputValue As Object) As Boolean
            Try
                If (TypeOf comparisonValue Is DBNull) OrElse (TypeOf inputValue Is DBNull) Then Return False
                Dim comparisonNumber As Double = CDbl(comparisonValue)
                Dim inputNumber As Double = CDbl(inputValue)

                Select Case condition.CalculationType
                    Case CalculationType.Addition
                        comparisonNumber += condition.CalculationValue
                    Case CalculationType.Subtraction
                        comparisonNumber -= condition.CalculationValue
                    Case CalculationType.Multiplication
                        comparisonNumber *= condition.CalculationValue
                    Case CalculationType.Division
                        comparisonNumber /= condition.CalculationValue
                    Case CalculationType.None
                End Select

                Select Case condition.ConditionType
                    Case ConditionType.Equal
                        Return comparisonNumber.Equals(inputNumber)
                    Case ConditionType.NotEqual
                        Return Not comparisonNumber.Equals(inputNumber)
                    Case ConditionType.LessThan
                        Return comparisonNumber < inputNumber
                    Case ConditionType.MoreThan
                        Return inputNumber < comparisonNumber
                    Case ConditionType.AndLess
                        Return comparisonNumber <= inputNumber
                    Case ConditionType.AndMore
                        Return inputNumber <= comparisonNumber
                End Select

                Return False
            Catch ex As Exception
                Return False
            End Try
        End Function

        Private Function CompareString(ByVal condition As DisplayConditionSetting, ByVal comparisonValue As String,
                                       ByVal inputValue As String) As Boolean
            Select Case condition.ConditionType
                Case ConditionType.ExactMatch
                    Return comparisonValue.Equals(inputValue)
                Case ConditionType.Include
                    Return comparisonValue.Contains(inputValue)
                Case ConditionType.NotInclude
                    Return Not comparisonValue.Contains(inputValue)
                Case ConditionType.ForwardMatch
                    Return comparisonValue.StartsWith(inputValue)
                Case ConditionType.BackwardMatch
                    Return comparisonValue.EndsWith(inputValue)
                Case ConditionType.NotEqual
                    Return Not comparisonValue.Equals(inputValue)
            End Select

            Return False
        End Function

        Private Sub SetCellAppearance(ByVal e As DataGridViewCellFormattingEventArgs,
                                      ByVal appearanceSetting As ColumnAppearanceSetting)
            e.CellStyle.BackColor = appearanceSetting.BackColor
            e.CellStyle.ForeColor = appearanceSetting.ForeColor
            e.CellStyle.Font = appearanceSetting.FontSetting
        End Sub

        Protected Overrides Sub OnColumnHeaderMouseClick(ByVal e As DataGridViewCellMouseEventArgs)
            MyBase.OnColumnHeaderMouseClick(e)
            If Not e.Button = MouseButtons.Left Then Return
            Cursor.Current = Cursors.WaitCursor
            SortData(e.ColumnIndex, _sortIndex = e.ColumnIndex, False)
            Cursor.Current = Cursors.Default
        End Sub

        Private Sub SortData(ByVal columnIndex As Integer, ByVal changeOrder As Boolean, ByVal isAutoUpdate As Boolean)
            If (columnIndex < 0) Then Return
            EndEdit()
            ClearSelection()
            SuspendLayout()
            If changeOrder Then
                _sortOrder = If(_sortOrder = SortOrder.Ascending, SortOrder.Descending, SortOrder.Ascending)
            Else
                If Not isAutoUpdate Then
                    _sortOrder = SortOrder.Ascending
                End If
                If 0 <= _sortIndex Then
                    Columns(_sortIndex).HeaderCell.SortGlyphDirection = SortOrder.None
                End If
            End If
            Columns(columnIndex).HeaderCell.SortGlyphDirection = _sortOrder
            _sortIndex = columnIndex
            Dim sortedTable As DataTable = TableDisplayGridProcess.SortDataTable(DataTable, columnIndex, _sortOrder)
            If Not IsNothing(_bindingSource) Then
                _bindingSource.DataSource = sortedTable
            End If
            ResumeLayout(True)
            Invalidate()
        End Sub

        Public Function GetColumnWidths() As Integer()
            If Not _initialized Then Return New Integer() {}
            Dim list As New List(Of Integer)
            For Each column As DataGridViewColumn In Columns
                list.Add(column.Width)
            Next
            Return list.ToArray()
        End Function
    End Class
End Namespace
