Imports Application.Data
Imports Application.Tool
Imports Application.Utility
Imports Database
Imports Database.SqlData
Imports Database.SqlData.Condition
Imports Database.SqlData.DisplayData

Namespace Data
    Public Class ServerSettings
        Private Shared _dbUpdateSettingList As DbUpdateSettingList
        Public Shared ReadOnly Property DbUpdateSettingList() As DbUpdateSettingList
            Get
                If IsNothing(_dbUpdateSettingList) Then
                    InitializeDbUpdateSettings()
                End If
                Return _dbUpdateSettingList
            End Get
        End Property

        Private Shared _logFormatSettingList As LogFormatSettingList
        Public Shared ReadOnly Property LogFormatSettingList() As LogFormatSettingList
            Get
                If IsNothing(_logFormatSettingList) Then
                    InitializeLogFormatSettings()
                End If
                Return _logFormatSettingList
            End Get
        End Property

        Private Shared _masterSettingList As MasterSettingList
        Public Shared ReadOnly Property MasterSettingList() As MasterSettingList
            Get
                If IsNothing(_masterSettingList) Then
                    InitializeMasterSettings()
                End If
                Return _masterSettingList
            End Get
        End Property

        Private Shared _tabPageSettingList As TabPageSettingList
        Public Shared ReadOnly Property TabpageSettingList() As TabPageSettingList
            Get
                If IsNothing(_tabPageSettingList) Then
                    InitializeTabPageSettings()
                End If
                Return _tabPageSettingList
            End Get
        End Property

        Private Shared _terminalMasterSettingList As TerminalMasterSettingList
        Public Shared ReadOnly Property TerminalMasterSettingList() As TerminalMasterSettingList
            Get
                If IsNothing(_terminalMasterSettingList) Then
                    InitializeTerminalMasterSetting()
                End If
                Return _terminalMasterSettingList
            End Get
        End Property

        Private Shared _pickingMasterSettingList As PickingMasterSettingList
        Public Shared ReadOnly Property PickingMasterSettingList() As PickingMasterSettingList
            Get
                If IsNothing(_pickingMasterSettingList) Then
                    InitializePickingMasterSetting()
                End If
                Return _pickingMasterSettingList
            End Get
        End Property
        Private Shared Sub InitializeTerminalMasterSetting()
%InitializeTerminalMasterSettingCodeText%
        End Sub

        Private Shared Sub InitializeTabPageSettings()
%InitializeTabPageSettingsCodeText%
        End Sub

        Private Shared Sub InitializePickingMasterSetting()
%InitializePickingMasterSettingsCodeText%
        End Sub

        Private Shared Function GetAdditionalColumnDisplayDataForInventory(ByVal masterName As String) As DisplayDataList
            Dim masterSetting As MasterSetting = MasterSettingList.GetMasterSetting(masterName)
            Dim additionalTableName As String = TableViewTool.GetAdditonalTableName(masterSetting.TableName)
            Dim displayDataList As New DisplayDataList()
            Dim additionalColumnNames As String() = TableViewTool.GetAdditionalColumnNames(InventoryTabPageTool.HISTORY_COUNT)
            Try
                Using accessor As New SQLiteAccessor(masterSetting.FilePath)
                    Dim tables As String() = accessor.GetTables()
                    If Not tables.Contains(additionalTableName) Then Return displayDataList

                    Dim command As New SqlSelectCommand(additionalTableName)
                    Dim additionalColumnDisplayNames As String()() = accessor.ExecuteCommandAndRead(command)
                    If IsNothing(additionalColumnDisplayNames) OrElse additionalColumnDisplayNames.Count <= 0 Then Return displayDataList
                    For i As Integer = 0 To additionalColumnDisplayNames(0).Length - 1
                        If String.IsNullOrEmpty(additionalColumnDisplayNames(0)(i)) Then Continue For
                        displayDataList.AddColumn(additionalColumnNames(i), additionalColumnDisplayNames(0)(i))
                    Next
                End Using
            Catch ex As Exception
                displayDataList = New DisplayDataList()
            End Try
            Return displayDataList
        End Function

        Public Shared Sub InitializeAppSettings()
%InitializeAppSettingsCodeText%
        End Sub

        Private Shared Sub InitializeDbUpdateSettings()
%InitializeDbUpdateSettingsCodeText%
        End Sub

        Private Shared Sub InitializeLogFormatSettings()
%InitializeLogFormatSettingsCodeText%
        End Sub

        Private Shared Sub InitializeMasterSettings()
%InitializeMasterSettingsCodeText%
        End Sub
    End Class
End Namespace
