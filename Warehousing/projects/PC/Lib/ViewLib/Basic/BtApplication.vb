Imports System.ComponentModel
Imports System.Threading
Imports System.Windows.Forms
Imports System.Drawing
Imports ViewLib.TabPart.ButtonTool
Imports Database
Imports Database.DbException
Imports Service.ControllerAccess
Imports SettingLib.Table
Imports Database.SqlData
Imports UtilityLib
Imports ViewLib.My.Resources

Namespace Basic

    Partial Public Class BtApplication

        Private _timer As Threading.Timer

        Private ReadOnly _windowDictionary As New Dictionary(Of RadioButton, BtWindow)

        Public Property IsProcessing As Boolean

        <Browsable(True)>
        Public Property ShowCommunicationSettingsButton() As Boolean
            Get
                Return ButtonCommunicationSetting.Visible
            End Get
            Set(value As Boolean)
                ButtonCommunicationSetting.Visible = value
            End Set
        End Property

        <Browsable(True)>
        Public Property ShowRefreshButton() As Boolean
            Get
                Return PanelRefresh.Visible
            End Get
            Set(value As Boolean)
                PanelRefresh.Visible = value
            End Set
        End Property

        <Browsable(True)>
        Public Property ShowMasterFileCreationButton() As Boolean
            Get
                Return ButtonMasterFileCreation.Visible
            End Get
            Set(value As Boolean)
                ButtonMasterFileCreation.Visible = value
            End Set
        End Property

        Private _updateFrequency As Integer = 60000

        Private Shared ReadOnly _autoUpdateSuppressCounter As New SuppressEventCounter()
        Public Shared ReadOnly Property AutoUpdateSuppressCounter() As SuppressEventCounter
            Get
                Return _autoUpdateSuppressCounter
            End Get
        End Property

        Public Sub New()
            InitializeComponent()

            AddHandler Application.Idle, AddressOf Application_Idle
        End Sub

        Private Sub Application_Idle(ByVal sender As Object, ByVal e As EventArgs)
            IsProcessing = False
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As EventArgs)
            MyBase.OnLoad(e)

            If FileUtility.IsDesignMode(DesignMode) Then Return

            Try
                If (Not ControllerAccessClient.LaunchController()) Then
                    MessageBox.Show(Me, ErrorMessageFailedStartTransferApp,
                                    My.Application.Info.AssemblyName, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Catch ex As Exception
            End Try

            For Each destockingButton As DestockingButton In BasicUtility.GetChildControls(Of DestockingButton)(Controls)
                AddHandler destockingButton.DestockingExecuted, AddressOf DestockingExecuted
            Next
        End Sub

        Protected Overrides Sub OnShown(ByVal e As EventArgs)
            MyBase.OnShown(e)
            If FileUtility.IsDesignMode(DesignMode) Then Return

            BasicUtility.SuspendUpdate(Me)

            If SettingsUtility.BtApplicationData.WindowBounds <> Rectangle.Empty Then
                Bounds = BoundsAdjuster.AdjustToScreen(SettingsUtility.BtApplicationData.WindowBounds)
            End If
            If SettingsUtility.BtApplicationData.AppMaximized Then
                WindowState = FormWindowState.Maximized
            End If
            _updateFrequency = SettingsUtility.BtApplicationData.UpdateFrequency
            CheckBoxAutoUpdate.Checked = SettingsUtility.BtApplicationData.AutoUpdateEnabled

            For Each splitContainer As SplitContainer In BasicUtility.GetChildControls(Of SplitContainer)(Controls)
                Dim distance = SettingsUtility.GetSplitContainerData(splitContainer.Parent.Name, splitContainer.Name)
                If distance = -1 Then Continue For
                splitContainer.SplitterDistance = distance
            Next

            For Each tab As BtTabControl In BasicUtility.GetChildControls(Of BtTabControl)(Controls)
                tab.InitializePage()
            Next

            SelectRadioButton()

            BasicUtility.ResumeUpdate(Me)
        End Sub

        Protected Overrides Sub OnClosing(ByVal e As CancelEventArgs)
            If FileUtility.IsDesignMode(DesignMode) Then Return

            MyBase.OnClosing(e)
            If e.Cancel Then Return
            Try
                WaitDialogController.ShowWaitDialog(Me, Resources.Closing)
                For Each window As BtWindow In BasicUtility.GetChildControls(Of BtWindow)(Controls)
                    window.UpdateToolSettings()
                Next

                Dim selectedWindow As String = String.Empty
                For Each pair As KeyValuePair(Of RadioButton, BtWindow) In _windowDictionary
                    If pair.Key.Checked Then
                        selectedWindow = pair.Value.Name
                        Exit For
                    End If
                Next

                SettingsUtility.SetBtApplicationData(Me, _updateFrequency, selectedWindow, CheckBoxAutoUpdate.Checked)
                SettingsUtility.SaveSettingFile()
                If (Not ControllerAccessClient.RequestClose()) Then
                    MessageBox.Show(Me, ErrorMessageFailedCloseTransferApp,
                                    My.Application.Info.AssemblyName, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
                WaitDialogController.CloseWaitDialog(Me)
            Catch ex As Exception
            End Try
        End Sub

        Protected Sub AddWindow(ByVal displayName As String, ByVal window As BtWindow)
            Controls.Add(window)
            window.Dock = DockStyle.Fill
            window.BringToFront()
            Dim radioButton = New RadioButton With {
                .Text = displayName,
                .FlatStyle = FlatStyle.Standard,
                .Appearance = Appearance.Button,
                .Size = New Size(95, 48),
                .Location = New Point(PanelWindowButtons.Controls.Count * 98 + 3, 3),
                .TextAlign = ContentAlignment.MiddleCenter,
                .UseMnemonic = False
            }
            PanelWindowButtons.Controls.Add(radioButton)
            AddHandler radioButton.CheckedChanged, AddressOf RadioButton_CheckedChanged
            _windowDictionary.Add(radioButton, window)
        End Sub

        Protected Sub SelectRadioButton()
            If _windowDictionary.Count <= 0 Then Return
            For Each pair As KeyValuePair(Of RadioButton, BtWindow) In _windowDictionary
                If pair.Value.Name = SettingsUtility.BtApplicationData.SelectedWindow Then
                    pair.Key.Checked = True
                    Exit Sub
                End If
            Next
            _windowDictionary.First().Key.Checked = True
        End Sub

        Protected Sub DeleteAllData(ByVal tableSettingList As List(Of TableSetting))
            If MessageBox.Show(Me, ConfirmationMessageDeleteAllData, My.Application.Info.AssemblyName,
                               MessageBoxButtons.OKCancel, MessageBoxIcon.None) <> DialogResult.OK Then Return

            Try
                Dim accessor As SQLiteAccessor = DbAccessManager.GetSQLiteAccessor(FileUtility.DbFilePath)
                accessor.RollbackTransaction()
                Using manager As New TransactionManager(accessor)
                    Dim tables As String() = accessor.GetTables()
                    For Each tableSetting As TableSetting In tableSettingList
                        If Not tables.Contains(tableSetting.TableName) Then Continue For
                        Dim command As New SqlDeleteCommand(tableSetting.TableName)
                        accessor.ExecuteCommand(command)
                    Next
                    manager.Commit()
                End Using

                Try
                    accessor.ExecuteVacuum()
                Catch ex As Exception
                End Try

            Catch ex As DbAccessException
                MessageBox.Show(Me, ErrorMessageDeleteFailed, My.Application.Info.AssemblyName,
                                MessageBoxButtons.OK, MessageBoxIcon.Error)
            Catch ex As Exception
                MessageBox.Show(Me, ErrorMessageDeleteFailed, My.Application.Info.AssemblyName,
                    MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub RadioButton_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
            Dim radioButton As RadioButton = TryCast(sender, RadioButton)
            If IsNothing(radioButton) OrElse Not radioButton.Checked Then Return

            WindowSelectionChanged()
        End Sub

        Private Sub WindowSelectionChanged()
            BasicUtility.SuspendUpdate(Me)
            For Each pair As KeyValuePair(Of RadioButton, BtWindow) In _windowDictionary
                pair.Value.Visible = pair.Key.Checked
            Next
            BasicUtility.ResumeUpdate(Me)
        End Sub

        Private Sub ButtonCommunicationSetting_Click(sender As Object, e As EventArgs) Handles ButtonCommunicationSetting.Click
            ControllerAccessClient.RequestShowSettingForm()
        End Sub

        Protected Overridable Sub ButtonUpdate_Click(sender As Object, e As EventArgs) Handles ButtonRefresh.Click
        End Sub

        Private Sub DestockingExecuted(ByVal sender As Object, ByVal e As DestockingExecutedEventArgs)
            UpdateHistoryColumnNames(e.TableName)
        End Sub

        Public Sub UpdateHistoryColumnNames(ByVal tableName As String)
            For Each sheet As BtSheet In BasicUtility.GetChildControls(Of BtSheet)(Controls)
                sheet.UpdateHistoryColumnName(tableName)
                If sheet.Visible Then
                    sheet.UpdateView(False, True)
                End If
            Next
        End Sub

        Private Sub CheckBoxAutoUpdate_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxAutoUpdate.CheckedChanged
            If CheckBoxAutoUpdate.Checked Then
                StartAutoUpdate()
                Return
            End If
            StopAutoUpdate()
        End Sub

        Private Sub StartAutoUpdate()
            _timer = New Threading.Timer(New TimerCallback(AddressOf AutoUpdate), Nothing, 0, _updateFrequency)
        End Sub

        Private Sub StopAutoUpdate()
            If IsNothing(_timer) Then Return

            _timer.Dispose()
            _timer = Nothing
        End Sub

        Private Sub AutoUpdate()
            If 0 < OwnedForms.Length Then Return
            If _autoUpdateSuppressCounter.IsSuppress Then Return
            BeginInvoke(Sub() UpdateView())
        End Sub

        Private Sub UpdateView()
            If IsProcessing Then Return
            IsProcessing = True

            Using New SuppressEvent(_autoUpdateSuppressCounter)
                Dim comboBoxs As List(Of ComboBox) = BasicUtility.GetChildControls(Of ComboBox)(Controls)
                If comboBoxs.Any(Function(comboBox) comboBox.DroppedDown) Then Return
                For Each control As Control In Controls
                    Dim window As BtWindow = TryCast(control, BtWindow)
                    If IsNothing(window) OrElse Not window.Visible Then Continue For
                    window.UpdateViewData(False, True)
                Next
            End Using
        End Sub

        Private Sub ButtonAutoUpdateSetting_Click(sender As Object, e As EventArgs) Handles ButtonAutoUpdateSetting.Click
            Using form As New AutoUpdateSettingForm
                form.UpdateFrequency = _updateFrequency
                If form.ShowDialog(Me) <> DialogResult.OK Then Return
                _updateFrequency = form.UpdateFrequency * 1000
                If CheckBoxAutoUpdate.Checked Then
                    StopAutoUpdate()
                    StartAutoUpdate()
                End If
            End Using
        End Sub

        Private Sub ButtonMasterFileCreation_Click(sender As Object, e As EventArgs) Handles ButtonMasterFileCreation.Click
            WaitDialogController.ShowWaitDialog(Me, Nothing)
            Try
                If (Not ControllerAccessClient.LaunchController(String.Empty, False)) Then
                    MessageBox.Show(ErrorMessageFailedStartTransferApp,
                                    My.Application.Info.AssemblyName, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If

                If (Not ControllerAccessClient.RequestCreateTerminalMasterFile()) Then
                    MessageBox.Show(ErrorMessageFailedCallTransferApp,
                                    My.Application.Info.AssemblyName, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Finally
                WaitDialogController.CloseWaitDialog(Me)
            End Try
        End Sub
    End Class
End Namespace
