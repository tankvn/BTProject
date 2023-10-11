Imports System.Windows.Forms
Imports SettingLib.Ini
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
    Public Class StringFilter
        Inherits Panel
        Implements IFilterBox, ISheetChild

        Private Const INPUT_HISTORY_COUNT As Integer = 5

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

        Private WithEvents _keywordComboBox As ComboBox
        Public Property KeywordComboBox() As ComboBox
            Get
                Return _keywordComboBox
            End Get
            Set(value As ComboBox)
                _keywordComboBox = value
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

        Public Sub LoadSetting() Implements ISheetChild.LoadSetting
            Dim pageSetting As SheetSetting = ViewUtility.GetPageSetting(Parent, DesignMode)
            If IsNothing(pageSetting) Then Return
            Setting = pageSetting.GuiPartSettingList.GetSetting(Name)
            Setting.DisplayColumnList = pageSetting.DisplayColumnList
        End Sub

        Public Sub Initialize()
            If FileUtility.IsDesignMode(DesignMode) Then Return

            Using New SuppressEvent(_suppressEventCounter)
                If Setting.ShowConditionSettingBox Then
                    ConditionComboBox.DisplayMember = "Value"
                    ConditionComboBox.ValueMember = "Key"
                    ConditionComboBox.DataSource = FilterPartProcess.StringConditions
                    ConditionComboBox.SelectedValue = Setting.SearchCondition
                End If

                InitializeKeywordComboBox()
                If Setting.ShowColumnSettingBox Then
                    FilterPartProcess.InitializeComboBox(ColumnComboBox, Setting.DisplayColumnList, True)
                    ColumnComboBox.SelectedValue = ConvertUtility.ToFieldString(Setting.SearchTarget)
                End If
                InitializeToolTip()
            End Using
        End Sub

        Private Sub InitializeKeywordComboBox()
            Dim historyStringList As List(Of String) = SettingsUtility.GetHistoryString(Parent.Name, Name)
            If IsNothing(historyStringList) Then Exit Sub
            For Each historyString As String In historyStringList
                KeywordComboBox.Items.Add(historyString)
            Next
            FilterPartProcess.SetDropDownWidth(KeywordComboBox)
        End Sub

        Private Sub InitializeToolTip()
            _toolTip = New ToolTip()
            _toolTip.InitialDelay = 1000
            _toolTip.ReshowDelay = 1000
            _toolTip.AutoPopDelay = 10000
            _toolTip.ShowAlways = False
            If Setting.ShowColumnSettingBox Then
                _toolTip.SetToolTip(ColumnComboBox, ColumnComboBox.Text)
            End If
            _toolTip.SetToolTip(KeywordComboBox, KeywordComboBox.Text)
        End Sub

        Private Sub _comboBox_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles _keywordComboBox.KeyDown
            If e.KeyData = Keys.Enter Then
                Using New SuppressEvent(_suppressEventCounter)
                    FilterToggleButton.Checked = True
                End Using
                DoFilter(sender, e)
                e.Handled = True
            End If
        End Sub

        Private Sub DoFilter(ByVal sender As Object, ByVal e As EventArgs) Handles _filterToggleButton.CheckedChanged
            If _suppressEventCounter.IsSuppress Then Exit Sub
            RaiseEvent FilterExecuted(Me, New EventArgs())
            If FilterToggleButton.Checked Then
                AddSearchHistory()
            End If
        End Sub

        Public Function CreateConditionData(ByVal calculationList As CalculationColumnDisplayDataList) As ConditionList _
            Implements IFilterPart.CreateConditionData
            If IsNothing(FilterToggleButton) OrElse Not FilterToggleButton.Checked Then Return Nothing
            Dim conditionType As ConditionType = ConvertUtility.ToConditionType(If(Setting.ShowConditionSettingBox,
                                                                                   ConditionComboBox.SelectedValue,
                                                                                   Setting.SearchCondition))
            Dim fieldName As String = If(Setting.ShowColumnSettingBox,
                                         ColumnComboBox.SelectedValue,
                                         ConvertUtility.ToFieldString(Setting.SearchTarget))
            Dim searchText = KeywordComboBox.Text
            searchText = EscapeSpecialCharacters(conditionType, searchText)
            Return FilterPartProcess.CreateSearchCondition(conditionType, fieldName, searchText, DataType.Text, Parent)
        End Function

        Private Sub AddSearchHistory()
            Using New SuppressEvent(_suppressEventCounter)
                Dim inputText As String = KeywordComboBox.Text
                Dim selectPoint As Integer = KeywordComboBox.SelectionStart
                For index As Integer = 0 To KeywordComboBox.Items.Count - 1
                    Dim value As String = KeywordComboBox.Items(index).ToString()
                    If value.Equals(inputText, StringComparison.InvariantCultureIgnoreCase) Then
                        KeywordComboBox.Items.RemoveAt(index)
                        Exit For
                    End If
                Next
                KeywordComboBox.Items.Insert(0, inputText)
                If INPUT_HISTORY_COUNT < KeywordComboBox.Items.Count Then
                    KeywordComboBox.Items.RemoveAt(INPUT_HISTORY_COUNT)
                End If
                KeywordComboBox.Text = inputText
                KeywordComboBox.SelectionStart = selectPoint
                FilterPartProcess.SetDropDownWidth(KeywordComboBox)
            End Using
        End Sub

        Friend Function CreateHistoryString() As List(Of SearchHistoryData)
            If String.IsNullOrEmpty(Parent.Name) Then Return New List(Of SearchHistoryData)
            Dim searchHistoryList As New List(Of SearchHistoryData)

            If KeywordComboBox.Items.Count = 0 Then
                Dim historyStringList As List(Of String) = SettingsUtility.GetHistoryString(Parent.Name, Name)
                If IsNothing(historyStringList) Then Return searchHistoryList

                For Each historyString As String In historyStringList
                    searchHistoryList.Add(New SearchHistoryData(Parent.Name, Name, historyString))
                Next
                Return searchHistoryList
            End If

            For Each historyString As String In KeywordComboBox.Items
                searchHistoryList.Add(New SearchHistoryData(Parent.Name, Name, historyString))
            Next
            Return searchHistoryList
        End Function

        Private Sub SelectedChanged(ByVal sender As Object, ByVal e As EventArgs) _
            Handles _columnComboBox.SelectedIndexChanged, _conditionComboBox.SelectedIndexChanged, _keywordComboBox.TextChanged
            If _suppressEventCounter.IsSuppress Then Exit Sub
            FilterToggleButton.Checked = False
        End Sub

        Private Sub FieldComboBox_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles _columnComboBox.SelectedIndexChanged
            If IsNothing(_toolTip) Then Return
            _toolTip.Active = False
            _toolTip.SetToolTip(ColumnComboBox, ColumnComboBox.Text)
            _toolTip.Active = True
        End Sub

        Private Sub KeywordComboBox_TextChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles _keywordComboBox.TextChanged
            If IsNothing(_toolTip) Then Return
            _toolTip.Active = False
            _toolTip.SetToolTip(KeywordComboBox, KeywordComboBox.Text)
            _toolTip.Active = True
        End Sub
    End Class
End Namespace
