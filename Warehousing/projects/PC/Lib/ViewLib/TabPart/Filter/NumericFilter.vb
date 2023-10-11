Imports System.Windows.Forms
Imports InterfaceLib
Imports System.ComponentModel
Imports System.Drawing
Imports Database.SqlData.Condition
Imports Database.SqlData.DisplayData
Imports Database.SqlData
Imports ProcessLib.PartProcess
Imports UtilityLib.Controls
Imports SettingLib.Part
Imports UtilityLib

Namespace TabPart.Filter

    Public Class NumericFilter
        Inherits Panel
        Implements IFilterBox, ISheetChild

        Property Setting As FilterPartSetting

        Private ReadOnly _suppressEventCounter As New SuppressEventCounter

        Private _toolTip As ToolTip

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

        Private WithEvents _keywordNumericUpDown As NumericUpDown
        Public Property KeywordNumericUpDown() As NumericUpDown
            Get
                Return _keywordNumericUpDown
            End Get
            Set(value As NumericUpDown)
                _keywordNumericUpDown = value
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

        <DefaultValue(GetType(Color), "185, 210, 240")>
        Public Overrides Property BackColor() As Color
            Get
                Return MyBase.BackColor
            End Get
            Set(ByVal value As Color)
                MyBase.BackColor = value
            End Set
        End Property

        Public Event FilterExecuted(ByVal sender As Object, ByVal e As EventArgs) Implements IFilterPart.FilterExecuted

        Public Sub Initialize()
            If FileUtility.IsDesignMode(DesignMode) Then Return

            Using New SuppressEvent(_suppressEventCounter)
                If Setting.ShowColumnSettingBox Then
                    FilterPartProcess.InitializeComboBox(ColumnComboBox, Setting.DisplayColumnList, True)
                    ColumnComboBox.SelectedValue = ConvertUtility.ToFieldString(Setting.SearchTarget)
                End If
                If Setting.ShowConditionSettingBox Then
                    ConditionComboBox.DisplayMember = "Value"
                    ConditionComboBox.ValueMember = "Key"
                    ConditionComboBox.DataSource = FilterPartProcess.NumericConditions
                    ConditionComboBox.SelectedValue = Setting.SearchCondition
                End If

                KeywordNumericUpDown.Minimum = Integer.MinValue
                KeywordNumericUpDown.Maximum = Integer.MaxValue
                InitializeToolTip()
            End Using
        End Sub

        Public Sub LoadSetting() Implements ISheetChild.LoadSetting
            Dim pageSetting As SheetSetting = ViewUtility.GetPageSetting(Parent, DesignMode)
            If IsNothing(pageSetting) Then Return
            Setting = pageSetting.GuiPartSettingList.GetSetting(Name)
            Setting.DisplayColumnList = pageSetting.DisplayColumnList
        End Sub

        Public Function CreateConditionData(ByVal calculationList As CalculationColumnDisplayDataList) As ConditionList _
            Implements IFilterPart.CreateConditionData
            If IsNothing(FilterToggleButton) OrElse Not FilterToggleButton.Checked Then Return Nothing
            Dim conditionType = ConvertUtility.ToConditionType(If(Setting.ShowConditionSettingBox,
                                                                  ConditionComboBox.SelectedValue,
                                                                  Setting.SearchCondition))
            Dim fieldName As String = If(Setting.ShowColumnSettingBox,
                                         ColumnComboBox.SelectedValue,
                                         ConvertUtility.ToFieldString(Setting.SearchTarget))
            Return FilterPartProcess.CreateSearchCondition(conditionType, fieldName, KeywordNumericUpDown.Text, DataType.FloatNumber, Parent)
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

        Private Sub _keywordNumericUpDown_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles _keywordNumericUpDown.KeyPress
            If e.KeyChar = ChrW(Keys.Enter) Then
                DoFilter(sender, e)
                FilterToggleButton.Checked = True
                e.Handled = True
            End If
        End Sub

        Private Sub DoFilter(ByVal sender As Object, ByVal e As EventArgs) Handles _filterToggleButton.CheckedChanged
            If _suppressEventCounter.IsSuppress Then Exit Sub
            RaiseEvent FilterExecuted(Me, e)
        End Sub

        Private Sub _comboBoxField_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles _columnComboBox.SelectedIndexChanged
            If IsNothing(_toolTip) Then Return
            _toolTip.Active = False
            _toolTip.SetToolTip(ColumnComboBox, ColumnComboBox.Text)
            _toolTip.Active = True
        End Sub

        Private Sub ConditionChanged(ByVal sender As Object, ByVal e As EventArgs) _
            Handles _columnComboBox.SelectedIndexChanged, _conditionComboBox.SelectedIndexChanged, _keywordNumericUpDown.TextChanged
            If _suppressEventCounter.IsSuppress Then Exit Sub
            FilterToggleButton.Checked = False
        End Sub
    End Class
End Namespace
