Imports System.IO
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Text
Imports System.Globalization
Imports SettingLib.Ini
Imports UtilityLib.Controls
Imports Database.SqlData.Condition
Imports SettingLib.Part

Public Class OperationalSettingUtility
    Private Const FILE_PATH As String = "Setting\LibrarySettings.ini"

    Private Const SEPARATOR As String = vbTab

    Private Const FORM_POSITION_DATA_LENGTH As Integer = 4

    Private Const FORM_SIZE_DATA_LENGTH As Integer = 4

    Private Const AUTO_UPDATE_DATA_LENGTH As Integer = 3

    Private Const SEARCH_HISTORY_DATA_LENGTH As Integer = 4

    Private Const TREE_SETTING_DATA_LENGTH As Integer = 5

    Private Const COLUMN_WIDTH_DATA_LENGTH As Integer = 4

    Private Const SPLIT_CONTAINER_DATA_LENGTH As Integer = 4

    Private Const LOCATION_PROPERTY_NAME As String = "Location"

    Private Const AUTO_UPDATE_PROPERTY_NAME As String = "AutoUpdate"

    Private Shared ReadOnly Property FilePath() As String
        Get
            Return Path.Combine(FileUtility.GetDllDirectory, FILE_PATH)
        End Get
    End Property

    Private Shared ReadOnly _searchHistoryList As New List(Of SearchHistoryData)

    Private Shared ReadOnly _treeSettingList As New List(Of TreeNodeData)

    Private Shared ReadOnly _columnWidthDataList As New List(Of ColumnWidthData)

    Private Shared ReadOnly _splitContainerDataList As New List(Of SplitContainerData)

    Private Shared _windowLocation As Point
    Public Shared ReadOnly Property WindowLocation As Point
        Get
            Return _windowLocation
        End Get
    End Property

    Private Shared _windowSize As Size
    Public Shared ReadOnly Property WindowSize As Size
        Get
            Return _windowSize
        End Get
    End Property

    Private Shared _updateFrequency As Integer = 60000
    Public Shared ReadOnly Property UpdateFrequency() As Integer
        Get
            Return _updateFrequency
        End Get
    End Property


    Public Shared Sub LoadSettingFile(ByRef topMenuForm As Form)
        If Not File.Exists(FilePath) Then Exit Sub
        Try
            Dim settingStringList As String() = File.ReadAllLines(FilePath, Encoding.GetEncoding("shift_jis"))
            AnalyzeSettingsFile(topMenuForm, settingStringList)
        Catch ex As Exception
            MessageBox.Show(ex.Message, My.Application.Info.AssemblyName, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Shared Sub AnalyzeSettingsFile(ByVal topMenuForm As Form, ByVal settingStringList As String())
        Dim splitContainerDataList As New List(Of SplitContainerData)
        Dim columnWidthDataList As New List(Of ColumnWidthData)
        Dim historyList As New List(Of SearchHistoryData)
        Dim treeNodeList As New List(Of TreeNodeData)
        For Each settingStringLine As String In settingStringList
            Dim settingString As String() = settingStringLine.Split(SEPARATOR.ToCharArray())
            If String.Equals(topMenuForm.GetType().Name, settingString(0)) Then
                If (String.Equals(settingString(1), LOCATION_PROPERTY_NAME)) AndAlso
                    (FORM_POSITION_DATA_LENGTH <= settingString.Length) Then
                    _windowLocation = New Point(Integer.Parse(settingString(2)), Integer.Parse(settingString(3)))
                    Continue For
                End If
            End If
            If String.Equals(topMenuForm.GetType().Name, settingString(0)) Then
                If (String.Equals(settingString(1), GetType(Size).Name)) AndAlso
                    (FORM_SIZE_DATA_LENGTH <= settingString.Length) Then
                    _windowSize = New Size(Integer.Parse(settingString(2)), Integer.Parse(settingString(3)))
                    Continue For
                End If
            End If
            If String.Equals(topMenuForm.GetType().Name, settingString(0)) Then
                If (String.Equals(settingString(1), AUTO_UPDATE_PROPERTY_NAME)) AndAlso
                    (AUTO_UPDATE_DATA_LENGTH <= settingString.Length) Then
                    _updateFrequency = Integer.Parse(settingString(2))
                    Continue For
                End If
            End If
            If String.Equals(GetType(SplitContainerData).Name, settingString(0)) Then
                If settingString.Length < SPLIT_CONTAINER_DATA_LENGTH Then Continue For
                Dim distance As Integer
                If Not Integer.TryParse(settingString(3), distance) Then Continue For
                splitContainerDataList.Add(New SplitContainerData(settingString(1), settingString(2), distance))
            End If
            If String.Equals(GetType(ColumnWidthData).Name, settingString(0)) Then
                If settingString.Length < COLUMN_WIDTH_DATA_LENGTH Then Continue For
                Dim widthList As New List(Of Integer)
                For index As Integer = 3 To settingString.Length - 1
                    Dim number As Integer
                    If Not Integer.TryParse(settingString(index), number) Then
                        widthList.Add(DisplayColumnSetting.DEFAULT_WIDTH)
                        Continue For
                    End If
                    widthList.Add(number)
                Next
                columnWidthDataList.Add(New ColumnWidthData(settingString(1), settingString(2), widthList))
            End If
            If String.Equals(GetType(SearchHistoryData).Name, settingString(0)) Then
                If settingString.Length < SEARCH_HISTORY_DATA_LENGTH Then Continue For
                historyList.Add(New SearchHistoryData(settingString(1), settingString(2), settingString(3)))
            End If
            If String.Equals(GetType(TreeNodeData).Name, settingString(0)) Then
                If settingString.Length < TREE_SETTING_DATA_LENGTH Then Continue For
                Dim conditionType As ConditionType
                If Not [Enum].TryParse(Of ConditionType)(settingString(5), conditionType) Then Continue For
                Dim treeSetting As New TreeNodeData(settingString(1), settingString(2), settingString(3),
                                                       settingString(4), conditionType)
                treeNodeList.Add(treeSetting)
            End If
        Next
        _splitContainerDataList.AddRange(splitContainerDataList)
        _columnWidthDataList.AddRange(columnWidthDataList)
        _searchHistoryList.AddRange(historyList)
        _treeSettingList.AddRange(treeNodeList)
    End Sub

    Public Shared Sub SaveSettingFile(ByVal topMenuForm As Form, ByVal frequency As Integer)
        Try
            Dim directoryName As String = Path.GetDirectoryName(FilePath)
            If Not Directory.Exists(directoryName) Then
                Directory.CreateDirectory(directoryName)
            End If
            Using streamWriter As StreamWriter = New StreamWriter(FilePath, False, Encoding.GetEncoding("shift_jis"))
                Dim topFormSettings As String = WriteTopFormSetting(topMenuForm, frequency)
                If Not String.IsNullOrEmpty(topFormSettings) Then
                    streamWriter.Write(topFormSettings)
                End If
                Dim librarySettings As String = WriteLibrarySetting()
                If Not String.IsNullOrEmpty(librarySettings) Then
                    streamWriter.Write(librarySettings)
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show(ex.Message, My.Application.Info.AssemblyName, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Shared Function WriteTopFormSetting(ByVal topMenuForm As Form, ByVal frequency As Integer) As String
        Dim stringBuilder As New StringBuilder
        Dim position = If((topMenuForm.WindowState = FormWindowState.Minimized),
                           topMenuForm.RestoreBounds.Location, topMenuForm.Location)
        Dim size As Size = If((topMenuForm.WindowState = FormWindowState.Minimized),
                        topMenuForm.RestoreBounds.Size, topMenuForm.Size)
        Dim writeString As String = WriteWindowPosition(topMenuForm.GetType().Name, position, size, frequency)
        If String.IsNullOrEmpty(writeString) Then Return String.Empty
        stringBuilder.Append(writeString)
        Return stringBuilder.ToString()
    End Function

    Private Shared Function WriteWindowPosition(ByVal topMenuFormName As String, ByVal position As Point, ByVal size As Size, ByVal frequency As Integer) As String
        If (String.IsNullOrEmpty(topMenuFormName) OrElse IsNothing(position)) Then
            Return String.Empty
        End If
        Dim stringBuilder As New StringBuilder
        stringBuilder.Append(topMenuFormName)
        stringBuilder.Append(SEPARATOR)
        stringBuilder.Append(LOCATION_PROPERTY_NAME)
        stringBuilder.Append(SEPARATOR)
        stringBuilder.Append(position.X.ToString(CultureInfo.InvariantCulture))
        stringBuilder.Append(SEPARATOR)
        stringBuilder.AppendLine(position.Y.ToString(CultureInfo.InvariantCulture))
        stringBuilder.Append(topMenuFormName)
        stringBuilder.Append(SEPARATOR)
        stringBuilder.Append(size.GetType().Name)
        stringBuilder.Append(SEPARATOR)
        stringBuilder.Append(size.Width.ToString(CultureInfo.InvariantCulture))
        stringBuilder.Append(SEPARATOR)
        stringBuilder.AppendLine(size.Height.ToString(CultureInfo.InvariantCulture))
        stringBuilder.Append(topMenuFormName)
        stringBuilder.Append(SEPARATOR)
        stringBuilder.Append(AUTO_UPDATE_PROPERTY_NAME)
        stringBuilder.Append(SEPARATOR)
        stringBuilder.AppendLine(frequency.ToString(CultureInfo.InvariantCulture))
        Return stringBuilder.ToString()
    End Function

    Private Shared Function WriteLibrarySetting() As String
        Dim stringBuilder As New StringBuilder
        Dim splitContainerDataString As String = WriteIniSetting(_splitContainerDataList)
        If Not String.IsNullOrEmpty(splitContainerDataString) Then
            stringBuilder.Append(splitContainerDataString)
        End If
        Dim columnWidthDataString As String = WriteIniSetting(_columnWidthDataList)
        If Not String.IsNullOrEmpty(columnWidthDataString) Then
            stringBuilder.Append(columnWidthDataString)
        End If
        Dim searchHistoryString As String = WriteIniSetting(_searchHistoryList)
        If Not String.IsNullOrEmpty(searchHistoryString) Then
            stringBuilder.Append(searchHistoryString)
        End If
        Dim treeSettingString As String = WriteIniSetting(_treeSettingList)
        If Not String.IsNullOrEmpty(treeSettingString) Then
            stringBuilder.Append(treeSettingString)
        End If
        Return stringBuilder.ToString()
    End Function

    Public Shared Sub SetHistoryString(ByVal historyList As List(Of SearchHistoryData),
                                       ByVal sheetName As String, ByVal partName As String)
        If IsNothing(historyList) Then Exit Sub
        For index = _searchHistoryList.Count - 1 To 0 Step -1
            If (_searchHistoryList(index).SheetName = sheetName) AndAlso
                (_searchHistoryList(index).ControlName = partName) Then
                _searchHistoryList.RemoveAt(index)
            End If
        Next
        _searchHistoryList.AddRange(historyList)
    End Sub

    Public Shared Sub SetSplitContainerData(ByVal distance As Integer,
                                            ByVal parentName As String, ByVal partName As String)
        For Each data As SplitContainerData In _splitContainerDataList
            If (data.ParentName = parentName) AndAlso (data.ControlName = partName) Then
                data.Distance = distance
                Return
            End If
        Next
        _splitContainerDataList.Add(New SplitContainerData(parentName, partName, distance))
    End Sub

    Public Shared Function GetSplitContainerData(ByVal parentName As String, ByVal partName As String) As Integer
        If String.IsNullOrEmpty(parentName) Then Return -1
        If String.IsNullOrEmpty(partName) Then Return -1

        For Each data As SplitContainerData In _splitContainerDataList
            If (data.ParentName = parentName) AndAlso (data.ControlName = partName) Then
                Return data.Distance
            End If
        Next
        Return -1
    End Function

    Public Shared Sub SetColumnWidth(ByVal widths As Integer(),
                                     ByVal sheetName As String, ByVal partName As String)
        If IsNothing(widths) Or widths.Length = 0 Then Return
        For Each columnWidthData As ColumnWidthData In _columnWidthDataList
            If Not columnWidthData.SheetName.Equals(sheetName) OrElse
                Not columnWidthData.ControlName.Equals(partName) Then Continue For

            columnWidthData.WidthList.Clear()
            columnWidthData.WidthList.AddRange(widths)
            Return
        Next
        _columnWidthDataList.Add(New ColumnWidthData(sheetName, partName, widths))
    End Sub

    Public Shared Function GetColumnWidths(ByVal sheetName As String, ByVal partName As String) As Integer()
        If String.IsNullOrEmpty(sheetName) Then Return New Integer() {}
        If String.IsNullOrEmpty(partName) Then Return New Integer() {}

        For Each columnWidthData As ColumnWidthData In _columnWidthDataList
            If (columnWidthData.SheetName = sheetName) AndAlso (columnWidthData.ControlName = partName) Then
                Return columnWidthData.WidthList.ToArray()
            End If
        Next
        Return New Integer() {}
    End Function

    Public Shared Function GetHistoryString(ByVal sheetName As String, ByVal partName As String) As List(Of String)
        If String.IsNullOrEmpty(sheetName) Then Return New List(Of String)
        If String.IsNullOrEmpty(partName) Then Return New List(Of String)
        Dim historyString As New List(Of String)
        For Each searchHistory As SearchHistoryData In _searchHistoryList
            If (searchHistory.SheetName = sheetName) AndAlso (searchHistory.ControlName = partName) Then
                historyString.Add(searchHistory.Data(0))
            End If
        Next
        Return historyString
    End Function

    Private Shared Function WriteIniSetting(ByVal settings As IEnumerable(Of IIniSetting)) As String
        Dim stringBuilder As New StringBuilder()
        For Each iniSetting As IIniSetting In settings
            stringBuilder.Append(iniSetting.SettingName)
            stringBuilder.Append(SEPARATOR)
            stringBuilder.Append(iniSetting.SheetName)
            stringBuilder.Append(SEPARATOR)
            stringBuilder.Append(iniSetting.ControlName)
            stringBuilder.Append(SEPARATOR)
            For Each data As String In iniSetting.Data
                stringBuilder.Append(data)
                stringBuilder.Append(SEPARATOR)
            Next
            stringBuilder.Remove(stringBuilder.Length - SEPARATOR.Length, SEPARATOR.Length)
            stringBuilder.AppendLine()
        Next
        Return stringBuilder.ToString()
    End Function

    Public Shared Sub SetTreeSettings(ByVal treeNodeList As List(Of BtTreeNode),
                                      ByVal sheetName As String, ByVal partName As String)
        If IsNothing(treeNodeList) Then Exit Sub
        For index = _treeSettingList.Count - 1 To 0 Step -1
            If (_treeSettingList(index).SheetName = sheetName) AndAlso
                (_treeSettingList(index).ControlName = partName) Then
                _treeSettingList.RemoveAt(index)
            End If
        Next
        Dim treeSettingsList As New List(Of TreeNodeData)
        For Each treeNodes As BtTreeNode In treeNodeList
            treeSettingsList.Add(New TreeNodeData(sheetName, partName, treeNodes.FieldName,
                                                     treeNodes.Text, treeNodes.ConditionType))
        Next
        _treeSettingList.AddRange(treeSettingsList)
    End Sub

    Public Shared Function GetTreeNode(ByVal sheetName As String, ByVal partName As String) As List(Of TreeNodeData)
        If String.IsNullOrEmpty(sheetName) Then Return New List(Of TreeNodeData)
        If String.IsNullOrEmpty(partName) Then Return New List(Of TreeNodeData)
        Dim treeSettingsList As New List(Of TreeNodeData)
        For Each treeSetting As TreeNodeData In _treeSettingList
            If (treeSetting.SheetName = sheetName) AndAlso (treeSetting.ControlName = partName) Then
                treeSettingsList.Add(treeSetting)
            End If
        Next
        Return treeSettingsList
    End Function
End Class
