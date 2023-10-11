Imports System.Windows.Forms
Imports Database.SqlData.Column
Imports InterfaceLib
Imports Database.SqlData.Condition
Imports Database
Imports ViewLib.My.Resources
Imports ViewLib.TabPart.Filter
Imports ProcessLib.PartProcess
Imports SettingLib.Part
Imports SettingLib.Table
Imports ViewLib.TabPart
Imports UtilityLib

Namespace Basic
    Public Class BtSheet
        Inherits UserControl
        Implements IBtSheet

        Private _hasData = False

        Public Shadows Property Name() As String Implements IBtSheet.Name
            Get
                Return MyBase.Name
            End Get
            Set(value As String)
                MyBase.Name = value
            End Set
        End Property

        Public Overridable ReadOnly Property SheetSetting() As SheetSetting Implements IBtSheet.SheetSetting
            Get
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property SortIndex() As Integer Implements IBtSheet.SortIndex
            Get
                Dim gridList = BasicUtility.GetChildControls(Of TableDisplayGrid)(Controls)
                If gridList.Count <= 0 Then Return -1

                Return gridList(0).SortIndex
            End Get
        End Property

        Public ReadOnly Property SortOrder() As SortOrder Implements IBtSheet.SortOrder
            Get
                Dim gridList = BasicUtility.GetChildControls(Of TableDisplayGrid)(Controls)
                If gridList.Count <= 0 Then Return SortOrder.None

                Return gridList(0).SortOrder
            End Get
        End Property

        Public ReadOnly Property DataTable() As DataTable
            Get
                Dim gridList = BasicUtility.GetChildControls(Of TableDisplayGrid)(Controls)
                If gridList.Count <= 0 Then Return Nothing

                Return gridList(0).DataTable
            End Get
        End Property

        Protected Overrides Sub OnParentChanged(ByVal e As EventArgs)
            MyBase.OnParentChanged(e)

            Dim children As List(Of ISheetChild) = BasicUtility.GetChildControls(Of ISheetChild)(Controls)
            For Each child As ISheetChild In children
                child.LoadSetting()
            Next
        End Sub

        Public Sub UpdateView(ByVal onShown As Boolean, ByVal autoUpdate As Boolean)
            If FileUtility.IsDesignMode(DesignMode) Then Return

            If onShown AndAlso _hasData Then Return

            Cursor.Current = Cursors.WaitCursor
            Try
                Dim accessor As DbAccessor = DbAccessManager.GetSQLiteAccessor(FileUtility.DbFilePath)
                If IsNothing(accessor) Then
                    If IsNothing(accessor) Then
                        Return
                    End If
                End If
                If IsNothing(SheetSetting) OrElse String.IsNullOrEmpty(SheetSetting.TableName) Then Return
                If IsNothing(GetTableSetting()) Then Return
                If Not DbAccessUtility.HasTable(accessor, SheetSetting.TableName) Then
                    CreateTable()
                End If

                Dim hasFailed As Boolean

                For Each preUpdatePart As IPreUpdatePart In BasicUtility.GetChildControls(Of IPreUpdatePart)(Controls)
                    hasFailed = hasFailed Or Not preUpdatePart.UpdateView(accessor, SheetSetting.TableName)
                Next

                Dim searchConditionData As ConditionList = BtSheetProcess.CreateSearchCondition(SheetSetting, Controls)

                For Each updatablePart As IUpdatablePart In BasicUtility.GetChildControls(Of IUpdatablePart)(Controls)
                    hasFailed = hasFailed Or Not updatablePart.UpdateView(accessor, SheetSetting.TableName,
                                                                          searchConditionData, SheetSetting.CalculationColumnList, autoUpdate)
                Next

                If hasFailed Then
                    MessageBox.Show(ErrorMessageUpdateViewFailed, My.Application.Info.AssemblyName, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If

                _hasData = True

            Catch ex As Exception
                ErrorUtility.OutputErrorLog(ex.Message, String.Empty)
                MessageBox.Show(ErrorMessageUpdateViewFailed, My.Application.Info.AssemblyName, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                Cursor.Current = Cursors.Default
            End Try
        End Sub

        Public Sub NeedUpdateView(ByVal sender As Object, ByVal e As EventArgs)
            Dim window As BtWindow = BasicUtility.GetParentControl(Of BtWindow)(Parent)
            If IsNothing(window) Then Return
            window.UpdateViewData(False, True)
        End Sub

        Public Function GetColumnWidths() As Integer() Implements IBtSheet.GetColumnWidths
            Dim list As List(Of TableDisplayGrid) = BasicUtility.GetChildControls(Of TableDisplayGrid)(Controls)
            If (IsNothing(list) OrElse list.Count = 0) Then Return New Integer() {}
            Return list(0).GetColumnWidths()
        End Function

        Protected Overridable Sub CreateTable()
        End Sub

        Protected Overridable Function GetTableSetting() As TableSetting Implements IBtSheet.GetTableSetting
            Return Nothing
        End Function

        Protected Sub CreateTable(ByVal setting As TableSetting, ByVal isData As Boolean)
            If IsNothing(setting) Then Return

            Dim columnDataList As New ColumnDataList()
            For Each columnSetting As ITableColumnSetting In setting.ColumnList
                columnDataList.Add(columnSetting.Name, columnSetting.DataType, columnSetting.IsKey, columnSetting.IsIndex)
            Next

            If isData Then
                columnDataList.Add(DataColumnSetting.ResultColumn.Name)
            End If
            Try
                Dim accessor As DbAccessor = DbAccessManager.GetSQLiteAccessor(FileUtility.DbFilePath)
                DbAccessUtility.CreateTable(accessor, SheetSetting.TableName, columnDataList, columnDataList.Count())
            Catch ex As Exception
                MessageBox.Show(Me, ConnectionError, My.Application.Info.AssemblyName, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Friend Function GetAllFieldName() As String() Implements IBtSheet.GetAllFieldName
            If IsNothing(SheetSetting.DisplayColumnList) Then Return New String() {}

            Dim list As New List(Of String)
            For Each setting As DisplayColumnSetting In SheetSetting.DisplayColumnList
                list.Add(setting.Name)
            Next
            Return list.ToArray()
        End Function

        Public Sub ClearData()

            If Not SheetSetting.UpdateOnShown Then Return

            For Each control As Control In Controls
                Dim part = TryCast(control, IDataClearablePart)
                If IsNothing(part) Then Continue For

                part.ClearData()
            Next
            _hasData = False
        End Sub

        Public Sub CreateHistoryTable()
            Dim masterSetting As MasterSetting = TryCast(GetTableSetting(), MasterSetting)
            If IsNothing(masterSetting) OrElse Not masterSetting.HasHistory Then Return

            DestockingButtonProcess.AddHistoryColumns(SheetSetting.TableName)
            DestockingButtonProcess.CreateHistoryTable(SheetSetting.TableName)
        End Sub

        Public Sub UpdateToolSettings()
            For Each tableDisplayGrid As TableDisplayGrid In BasicUtility.GetChildControls(Of TableDisplayGrid)(Controls)
                SettingsUtility.SetColumnWidth(tableDisplayGrid.GetColumnWidths(), Name, tableDisplayGrid.Name)
                Continue For
            Next
            For Each stringFilter As StringFilter In BasicUtility.GetChildControls(Of StringFilter)(Controls)
                SettingsUtility.SetHistoryString(stringFilter.CreateHistoryString(), Name, stringFilter.Name)
                Continue For
            Next
            For Each filterTree As FilterTree In BasicUtility.GetChildControls(Of FilterTree)(Controls)
                If (Not filterTree.Setting.SettingSearchConditions) OrElse
                    (Not filterTree.Initialized) Then Continue For
                SettingsUtility.SetTreeSettings(filterTree.TreeNodes, Name, filterTree.Name)
            Next
        End Sub

        Public Function GetConditionList() As ConditionList Implements IBtSheet.GetConditionList
            Return BtSheetProcess.CreateSearchCondition(SheetSetting, Controls)
        End Function

        Public Sub UpdateHistoryColumnName(ByVal tableName As String)
            For Each tableDisplayGrid As TableDisplayGrid In BasicUtility.GetChildControls(Of TableDisplayGrid)(Controls)
                tableDisplayGrid.UpdateHistoryColumnName(tableName)
            Next
        End Sub
    End Class
End Namespace
