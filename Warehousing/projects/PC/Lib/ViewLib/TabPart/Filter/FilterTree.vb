Imports Database
Imports SettingLib.Ini
Imports InterfaceLib
Imports System.ComponentModel
Imports System.Drawing
Imports Database.SqlData.Condition
Imports Database.SqlData.DisplayData
Imports System.Windows.Forms
Imports ProcessLib.PartProcess
Imports SettingLib.Part
Imports Database.SqlData
Imports UtilityLib
Imports UtilityLib.Controls

Namespace TabPart.Filter

    Public Class FilterTree
        Implements IFilterPart, IPreUpdatePart, ISheetChild

        Private Const MAX_ITEM_COUNT = 63

        Private ReadOnly _nodeAll As New BtTreeNode(FilterPartSetting.FieldAllText, String.Empty, ConditionType.ExactMatch)

        Private ReadOnly _displayNode As New List(Of BtTreeNode)

        Private ReadOnly _suppressEventCounter As New SuppressEventCounter()

        Public Property Setting() As FilterTreeSetting

        Private ReadOnly _treeNodes As New List(Of BtTreeNode)
        Public ReadOnly Property TreeNodes() As List(Of BtTreeNode)
            Get
                Return _treeNodes
            End Get
        End Property

        Private WithEvents _columnSelectComboBox As ComboBox
        Public Property ColumnSelectComboBox() As ComboBox
            Get
                Return _columnSelectComboBox
            End Get
            Set(ByVal value As ComboBox)
                _columnSelectComboBox = value
            End Set
        End Property

        Private WithEvents _editButton As Button
        Public Property EditButton() As Button
            Get
                Return _editButton
            End Get
            Set(ByVal value As Button)
                _editButton = value
            End Set
        End Property

        Private WithEvents _treeView As BtTreeView
        Public Property TreeView() As BtTreeView
            Get
                Return _treeView
            End Get
            Set(ByVal value As BtTreeView)
                _treeView = value
            End Set
        End Property

        Public Property Initialized() As Boolean

        Private _sheetName As String

        Private _sheetSetting As SheetSetting

        Private _tableName As String

        <DefaultValue(GetType(Color), "185, 210, 240")>
        Public Overrides Property BackColor() As Color
            Get
                Return MyBase.BackColor
            End Get
            Set(ByVal value As Color)
                MyBase.BackColor = value
            End Set
        End Property

        Public Event FilterExecuted(sender As Object, e As EventArgs) Implements IFilterPart.FilterExecuted

        Public Sub LoadSetting() Implements ISheetChild.LoadSetting
            If FileUtility.IsDesignMode(DesignMode) Then Return
            If IsNothing(Parent) Then Return

            Dim parentSheet As IBtSheet = TryCast(BasicUtility.GetParentControl(Of IBtSheet)(Parent), IBtSheet)
            If IsNothing(parentSheet) Then Return
            Setting = ViewUtility.GetPartSetting(Name, Parent, DesignMode)
            If IsNothing(Setting) Then Return

            _sheetName = parentSheet.Name
            _sheetSetting = parentSheet.SheetSetting
            _tableName = parentSheet.SheetSetting.TableName
        End Sub

        Public Sub Initialize()
            If FileUtility.IsDesignMode(DesignMode) Then Return

            DoubleBuffered = True
            Using New SuppressEvent(_suppressEventCounter)
                FilterPartProcess.InitializeComboBox(_columnSelectComboBox, Setting.DisplayColumns, False)
                _treeView.BeginUpdate()
                InitializeTreeNode()
                UpdateTreeView()
                _treeView.Nodes.Add(_nodeAll)
                _treeView.Nodes.Item(0).Expand()
                _treeView.EndUpdate()
                Initialized = True
            End Using
        End Sub

        Private Sub InitializeTreeNode()
            Dim treeSettingsList As List(Of TreeNodeData) = SettingsUtility.GetTreeNode(_sheetName, Name)
            If IsNothing(treeSettingsList) Then Exit Sub
            If treeSettingsList.Count = 0 Then
                For Each nodeSetting As TreeNodeSetting In Setting.DefaultConditionSettings
                    _treeNodes.Add(ConvertUtility.ToBtTreeNode(nodeSetting))
                Next
            End If
            For Each treeSettings As TreeNodeData In treeSettingsList
                _treeNodes.Add(New BtTreeNode(treeSettings.Keyword, treeSettings.ColumnName, treeSettings.ConditionType))
            Next
        End Sub

        Private Sub UpdateTreeView()
            If IsNothing(_treeNodes) Then Exit Sub
            If Not Setting.SettingSearchConditions Then Return

            _treeView.SelectedNode = Nothing
            _nodeAll.Nodes.Clear()
            _displayNode.Clear()
            For Each node As BtTreeNode In _treeNodes
                If Equals(node.FieldName, _columnSelectComboBox.SelectedValue) Then
                    _nodeAll.Nodes.Add(node)
                    _displayNode.Add(node)
                End If
            Next
            If 0 < _nodeAll.Nodes.Count Then
                _nodeAll.Nodes.Add(New BtTreeNode(My.Resources.SearchStringOther, String.Empty, ConditionType.ExactMatch))
            End If
            _treeView.SelectedNode = _nodeAll
        End Sub

        Public Function UpdateView(ByVal dbAccessor As DbAccessor, ByVal tableName As String) As Boolean Implements IPreUpdatePart.UpdateView
            If IsNothing(Setting) OrElse Setting.SettingSearchConditions Then Return True
            If IsNothing(_treeView) Then Return True

            Dim lastSelectedNodeText As String = Nothing
            Dim hasLastSelectedNode As Boolean = False
            If Not IsNothing(_treeView.SelectedNode) Then
                hasLastSelectedNode = True
                lastSelectedNodeText = _treeView.SelectedNode.Text
            End If
            _treeView.SelectedNode = Nothing

            If Not TypeOf (_columnSelectComboBox.SelectedValue) Is String Then Return True

            _nodeAll.Nodes.Clear()
            _displayNode.Clear()
            Try
                Dim displayDataList As New DisplayDataList()
                displayDataList.AddRange(ConvertUtility.ToDisplayDataList(_sheetSetting.DisplayColumnList, _sheetSetting.CalculationColumnList))
                Dim command = New SqlSelectFilterTreeItemCommand(tableName, _columnSelectComboBox.SelectedValue,
                                                                   ConvertUtility.ToDisplayDataList(_sheetSetting.DisplayColumnList, _sheetSetting.CalculationColumnList),
                                                                   ConvertUtility.ToConditionData(_sheetSetting.FilterSettingList, _sheetSetting.CalculationColumnList))
                Dim result As String() = dbAccessor.ExecuteCommandAndRead(command)
                Dim count = Math.Min(result.Length, MAX_ITEM_COUNT)
                For index As Integer = 0 To count - 1
                    _nodeAll.Nodes.Add(New BtTreeNode(result(index), _columnSelectComboBox.SelectedValue, ConditionType.ExactMatch))
                    _displayNode.Add(New BtTreeNode(result(index), _columnSelectComboBox.SelectedValue, ConditionType.ExactMatch))
                Next
                If MAX_ITEM_COUNT < result.Length Then
                    _nodeAll.Nodes.Add(New BtTreeNode(My.Resources.SearchStringOther, String.Empty, ConditionType.ExactMatch))
                End If
                If hasLastSelectedNode Then
                    For Each node As BtTreeNode In _nodeAll.Nodes
                        If node.KeyWord.Equals(lastSelectedNodeText) Then
                            _treeView.SelectedNode = node
                            _treeView.Refresh()
                            Return True
                        End If
                    Next
                End If
                _treeView.SelectedNode = _nodeAll
                _treeView.Refresh()
                Return True
            Catch ex As Exception
                ErrorUtility.OutputErrorLog(ex.Message, String.Empty)
                Return False
            End Try
        End Function

        Public Function CreateConditionData(calculationList As CalculationColumnDisplayDataList) As ConditionList Implements IFilterPart.CreateConditionData
            If IsNothing(_treeView) Then Return Nothing
            Dim selectedNode As BtTreeNode = _treeView.SelectedNode
            If IsNothing(selectedNode) Then Return Nothing
            If selectedNode.KeyWord = FilterPartSetting.FieldAllText Then Return Nothing

            If (selectedNode.KeyWord <> My.Resources.SearchStringOther) OrElse
                (_treeView.SelectedNode.Index < (_nodeAll.Nodes.Count - 1)) Then
                Dim searchText = EscapeSpecialCharacters(selectedNode.ConditionType, selectedNode.KeyWord)
                Return FilterPartProcess.CreateSearchCondition(selectedNode.ConditionType, _columnSelectComboBox.SelectedValue,
                                                               searchText, DataType.Text, Parent)
            Else
                Dim otherConditionList As New ConditionList
                For Each displayNode As BtTreeNode In _displayNode
                    Dim searchText As String = EscapeSpecialCharacters(selectedNode.ConditionType, displayNode.KeyWord)
                    otherConditionList.AddLiteralNegative(displayNode.ConditionType, _columnSelectComboBox.SelectedValue,
                                                          searchText)
                Next
                Return otherConditionList
            End If
        End Function

        Private Sub _buttonToolSetting_Click(ByVal sender As Object, ByVal e As EventArgs) Handles _editButton.Click
            If IsNothing(Setting) Then Return
            Using treeSettingForm As New FilterTreeItemSettingDialog(Setting.DisplayColumns, _treeNodes)
                If DialogResult.OK = treeSettingForm.ShowDialog(FindForm()) Then
                    _treeNodes.Clear()
                    _treeNodes.AddRange(treeSettingForm.GetBtTreeNodeList())
                    UpdateTreeView()
                    TreeView.ExpandAll()
                End If
            End Using
        End Sub

        Private Sub _treeView_AfterSelect(ByVal sender As Object, ByVal e As TreeViewEventArgs) Handles _treeView.AfterSelect
            If _suppressEventCounter.IsSuppress Then Exit Sub
            Using New SuppressEvent(_suppressEventCounter)
                RaiseEvent FilterExecuted(sender, New EventArgs())
            End Using
        End Sub

        Private Sub _comboBox_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles _columnSelectComboBox.SelectedIndexChanged
            If IsNothing(Setting) Then Return
            _treeView.BeginUpdate()
            If Setting.SettingSearchConditions Then
                UpdateTreeView()
            Else
                Dim accessor As DbAccessor = DbAccessManager.GetSQLiteAccessor(FileUtility.DbFilePath)
                If DbAccessUtility.HasTable(accessor, _tableName) Then
                    UpdateView(accessor, _tableName)
                End If
            End If
            _treeView.EndUpdate()
        End Sub
    End Class
End Namespace
