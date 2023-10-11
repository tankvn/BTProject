Imports System.Windows.Forms
Imports ProcessLib.ControlProcess
Imports Database
Imports DTC.Log
Imports SettingLib.Table
Imports UtilityLib

Namespace Basic
    Public Class BtWindow
        Inherits UserControl

        Protected Overrides Sub OnVisibleChanged(ByVal e As EventArgs)
            MyBase.OnVisibleChanged(e)
            UpdateViewData(True, True)
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As EventArgs)
            MyBase.OnLoad(e)
            CreateHistoryTables()
            UpdateViewData(False, False)
        End Sub

        Private Sub CreateHistoryTables()
            For Each sheet As BtSheet In BasicUtility.GetChildControls(Of BtSheet)(Controls)
                sheet.CreateHistoryTable()
            Next
        End Sub

        Public Sub UpdateDatabase(ByVal dataFormatSettingList As DataFormatSettingList, ByVal updateMasterSettingList As UpdateMasterSettingList, ByVal masterSettingList As MasterSettingList)
            Dim updateTableNames As String() = GetUpdateTableNames()

            UpdateTables(updateTableNames, dataFormatSettingList, updateMasterSettingList, masterSettingList)

            UpdateViewData(False, False)
        End Sub

        Private Sub UpdateTables(ByVal updateTableNames As String(), ByVal dataFormatSettingList As DataFormatSettingList,
                                   ByVal updateMasterSettingList As UpdateMasterSettingList, ByVal masterSettingList As MasterSettingList)
            For Each tableName As String In updateTableNames
                If String.IsNullOrEmpty(tableName) Then Continue For

                Dim masterSetting As MasterSetting = masterSettingList.GetMasterSettingByTableName(tableName)
                Dim dataFormatSetting As DataFormatSetting = dataFormatSettingList.GetByTableName(tableName)

                If Not IsNothing(masterSetting) Then
                    Dim accessor As SQLiteAccessor = DbAccessManager.GetSQLiteAccessor(FileUtility.DbFilePath)
                    TableUpdateProcess.Import(accessor, masterSetting, dataFormatSettingList, updateMasterSettingList, True)
                    Dim btApplication As BtApplication = BasicUtility.GetParentControl(Of BtApplication)(Parent)
                    If Not IsNothing(btApplication) Then
                        btApplication.UpdateHistoryColumnNames(tableName)
                    End If
                End If

                If Not IsNothing(dataFormatSetting) Then
                    LogProcessor.ProcessErrorLogFile((dataFormatSetting.LogFileName))
                End If
            Next
        End Sub

        Public Sub UpdateViewData(ByVal onShown As Boolean, ByVal autoUpdate As Boolean)
            If FileUtility.IsDesignMode(DesignMode) Then Return

            SuspendLayout()
            For Each control As Control In Controls
                Dim btTabControls = New List(Of BtTabControl)
                SearchBtTabControl(control, btTabControls)
                For Each btTabControl As BtTabControl In btTabControls
                    If Visible Then
                        btTabControl.UpdateDisplayedPage(onShown, autoUpdate)
                        Continue For
                    End If
                    If onShown Then
                        btTabControl.ClearDisplayedPage()
                    End If
                Next
            Next
            ResumeLayout()
        End Sub

        Private Function GetUpdateTableNames() As String()
            Dim list = New List(Of String)
            For Each control As Control In Controls
                Dim btTabControls = New List(Of BtTabControl)
                SearchBtTabControl(control, btTabControls)
                For Each btTabControl As BtTabControl In btTabControls
                    For Each tabPage As TabPage In btTabControl.TabPages
                        Dim sheet As BtSheet = btTabControl.GetUpdateSheet(tabPage)
                        If IsNothing(sheet) OrElse Not sheet.Visible Then Continue For
                        list.Add(sheet.SheetSetting.TableName)
                    Next
                Next
            Next
            Return list.Distinct().ToArray()
        End Function

        Private Shared Sub SearchBtTabControl(ByVal parent As Control, ByVal list As List(Of BtTabControl))
            If TypeOf (parent) Is BtTabControl Then
                list.Add(parent)
            End If
            For Each child As Control In parent.Controls
                SearchBtTabControl(child, list)
            Next
        End Sub

        Public Sub UpdateToolSettings()
            For Each container As SplitContainer In BasicUtility.GetChildControls(Of SplitContainer)(Controls)
                SettingsUtility.SetSplitContainerData(container.SplitterDistance, container.Parent.Name, container.Name)
            Next

            For Each tabControl As BtTabControl In BasicUtility.GetChildControls(Of BtTabControl)(Controls)
                SettingsUtility.SetTabSelectionSetting(tabControl.SelectedIndex, BasicUtility.GetParentControl(Of BtWindow)(tabControl.Parent).Name, tabControl.Name)
            Next

            For Each sheet As BtSheet In BasicUtility.GetChildControls(Of BtSheet)(Controls)
                sheet.UpdateToolSettings()
            Next
        End Sub
    End Class
End Namespace
