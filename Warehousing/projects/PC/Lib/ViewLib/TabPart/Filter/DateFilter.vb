Imports System.Windows.Forms
Imports InterfaceLib
Imports System.ComponentModel
Imports System.Drawing
Imports Database.SqlData.Condition
Imports Database.SqlData.DisplayData
Imports System.Globalization
Imports Database.SqlData
Imports ProcessLib.PartProcess
Imports SettingLib
Imports UtilityLib.Controls
Imports SettingLib.Part
Imports UtilityLib
Imports ViewLib.My.Resources

Namespace TabPart.Filter
    Public Class DateFilter
        Inherits Panel
        Implements IFilterBox, ISheetChild

        Private ReadOnly _suppressEventCounter As New SuppressEventCounter

        Private _toolTip As ToolTip

        Property Setting As FilterPartSetting

        <DefaultValue(GetType(Color), "185, 210, 240")>
        Public Overrides Property BackColor() As Color
            Get
                Return MyBase.BackColor
            End Get
            Set(ByVal value As Color)
                MyBase.BackColor = value
            End Set
        End Property

        Private WithEvents _conditionComboBox As ComboBox
        Public Property ConditionComboBox() As ComboBox Implements IFilterBox.ConditionComboBox
            Get
                Return _conditionComboBox
            End Get
            Set(value As ComboBox)
                _conditionComboBox = value
            End Set
        End Property

        Private WithEvents _columnComboBox As ComboBox
        Public Property ColumnComboBox() As ComboBox Implements IFilterBox.ColumnComboBox
            Get
                Return _columnComboBox
            End Get
            Set(value As ComboBox)
                _columnComboBox = value
            End Set
        End Property

        Private WithEvents _dateTimePickerFrom As DateTimePicker
        Public Property DateTimePickerFrom() As DateTimePicker
            Get
                Return _dateTimePickerFrom
            End Get
            Set(value As DateTimePicker)
                _dateTimePickerFrom = value
            End Set
        End Property


        Private WithEvents _dateTimePickerTo As DateTimePicker
        Public Property DateTimePickerTo() As DateTimePicker
            Get
                Return _dateTimePickerTo
            End Get
            Set(value As DateTimePicker)
                _dateTimePickerTo = value
            End Set
        End Property

        Private WithEvents _filterToggleButton As FilterToggleButton
        Public Property FilterToggleButton() As FilterToggleButton Implements IFilterBox.FilterToggleButton
            Get
                Return _filterToggleButton
            End Get
            Set(value As FilterToggleButton)
                _filterToggleButton = value
            End Set
        End Property

        Public Event FilterExecuted(sender As Object, e As EventArgs) Implements IFilterPart.FilterExecuted

        Public Sub Initialize()
            If FileUtility.IsDesignMode(DesignMode) Then Return

            Using New SuppressEvent(_suppressEventCounter)
                InitializeComboBox()
                InitializeToolTip()
                DateTimePickerFrom.Value = Today
                DateTimePickerTo.Value = Today
            End Using
        End Sub

        Private Sub InitializeComboBox()
            If Setting.ShowConditionSettingBox Then
                ConditionComboBox.DisplayMember = "Value"
                ConditionComboBox.ValueMember = "Key"
                ConditionComboBox.DataSource = FilterPartProcess.DateConditions
                ConditionComboBox.SelectedValue = Setting.SearchCondition
            Else
                SetDateTimePickerEnabled(Setting.SearchCondition)
            End If
            If Setting.ShowColumnSettingBox Then
                FilterPartProcess.InitializeComboBox(ColumnComboBox, Setting.DisplayColumnList, True)
                ColumnComboBox.SelectedValue = ConvertUtility.ToFieldString(Setting.SearchTarget)
            End If
        End Sub

        Public Sub LoadSetting() Implements ISheetChild.LoadSetting
            Dim pageSetting As SheetSetting = ViewUtility.GetPageSetting(Parent, DesignMode)
            If IsNothing(pageSetting) Then Return
            Setting = pageSetting.GuiPartSettingList.GetSetting(Name)
            Setting.DisplayColumnList = pageSetting.DisplayColumnList
        End Sub

        Private Sub _comboBox_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) _
            Handles _dateTimePickerFrom.KeyDown, _dateTimePickerTo.KeyDown
            If e.KeyData = Keys.Enter Then
                DoFilter(sender, e)
                _filterToggleButton.Checked = True
                e.Handled = True
            End If
        End Sub

        Private Sub SelectedChanged(ByVal sender As Object, ByVal e As EventArgs) _
            Handles _columnComboBox.SelectedIndexChanged, _conditionComboBox.SelectedIndexChanged,
            _dateTimePickerFrom.ValueChanged, _dateTimePickerTo.ValueChanged
            If _suppressEventCounter.IsSuppress Then Exit Sub
            FilterToggleButton.Checked = False
        End Sub

        Private Sub DoFilter(ByVal sender As Object, ByVal e As EventArgs) Handles _filterToggleButton.CheckedChanged
            If _suppressEventCounter.IsSuppress Then Exit Sub

            If _filterToggleButton.Checked AndAlso
                (If(Setting.ShowConditionSettingBox,
                    ConditionComboBox.SelectedValue,
                    Setting.SearchCondition) = SearchConditionType.FromTo) AndAlso
                (DateTimePickerTo.Value < DateTimePickerFrom.Value) Then
                MessageBox.Show(Me, WarnInvaridPeriod, My.Application.Info.AssemblyName, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            RaiseEvent FilterExecuted(Me, New EventArgs())
        End Sub

        Private Sub _conditionComboBox_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) _
            Handles _conditionComboBox.SelectedIndexChanged
            Dim selectedText As String = _conditionComboBox.Text
            If IsNothing(selectedText) Then Exit Sub
            SetDateTimePickerEnabled(_conditionComboBox.SelectedValue)
        End Sub

        Private Sub SetDateTimePickerEnabled(ByVal searchCondition As SearchConditionType)
            Select Case searchCondition
                Case SearchConditionType.FromTo
                    _dateTimePickerFrom.Enabled = True
                    _dateTimePickerTo.Enabled = True
                Case SearchConditionType.[To]
                    _dateTimePickerFrom.Enabled = False
                    _dateTimePickerTo.Enabled = True
                Case SearchConditionType.From
                    _dateTimePickerFrom.Enabled = True
                    _dateTimePickerTo.Enabled = False
            End Select
        End Sub

        Public Function CreateConditionData(ByVal calculationList As CalculationColumnDisplayDataList) As ConditionList _
            Implements IFilterPart.CreateConditionData
            Dim conditionList As New ConditionList
            If IsNothing(FilterToggleButton) OrElse Not FilterToggleButton.Checked Then Return conditionList
            Dim fieldName As String = If(Setting.ShowColumnSettingBox,
                                         ColumnComboBox.SelectedValue,
                                         ConvertUtility.ToFieldString(Setting.SearchTarget))
            Dim startString As String = DateTimePickerFrom.Value.ToString(DateTimeUtility.SQLITE_DATE_FORMAT,
                                                                          CultureInfo.InvariantCulture)
            Dim lastString As String = DateTimePickerTo.Value.ToString(DateTimeUtility.SQLITE_DATE_FORMAT,
                                                                       CultureInfo.InvariantCulture)
            If String.IsNullOrEmpty(fieldName) Then Return Nothing
            If fieldName = FilterPartSetting.FieldAllText Then
                Dim dateFields As String() = FilterPartProcess.GetDateFieldNames(Parent)
                If dateFields.Length = 0 Then Return conditionList
                conditionList.Add(New MultipleColumnDateTimeLiteralConditionData(FilterPartProcess.GetDateFieldNames(Parent),
                                                                                 If(DateTimePickerFrom.Enabled, startString, String.Empty),
                                                                                 If(DateTimePickerTo.Enabled, lastString, String.Empty)))
                Return conditionList
            End If
            If DateTimePickerFrom.Enabled Then
                conditionList.AddRange(FilterPartProcess.CreateSearchCondition(ConditionType.AndMore, fieldName,
                                                                               startString, DataType.DateText, Parent))
            End If
            If DateTimePickerTo.Enabled Then
                conditionList.AddRange(FilterPartProcess.CreateSearchCondition(ConditionType.AndLess, fieldName,
                                                                               lastString, DataType.DateText, Parent))
            End If
            Return conditionList
        End Function

        Private Sub InitializeToolTip()
            _toolTip = New ToolTip()
            _toolTip.InitialDelay = 1000
            _toolTip.ReshowDelay = 1000
            _toolTip.AutoPopDelay = 10000
            _toolTip.ShowAlways = False
            If Setting.ShowColumnSettingBox Then
                _toolTip.SetToolTip(ColumnComboBox, ColumnComboBox.Text)
            End If
        End Sub
    End Class
End Namespace
