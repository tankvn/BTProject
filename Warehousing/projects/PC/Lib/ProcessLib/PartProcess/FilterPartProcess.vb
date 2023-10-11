Imports Database.SqlData.Condition
Imports System.Windows.Forms
Imports System.Drawing
Imports InterfaceLib
Imports Database.SqlData.Condition.Member
Imports SettingLib
Imports SettingLib.Part
Imports Database.SqlData
Imports UtilityLib
Imports ProcessLib.My.Resources

Namespace PartProcess

    Public Class FilterPartProcess
        Private Const VALUE_MEMBER_NAME As String = "FieldName"

        Private Const DISPLAY_MEMBER_NAME As String = "Name"

        Friend Const DROP_DOWN_WIDTH_MARGIN As Integer = 25


        Public Shared ReadOnly Property NumericConditions As KeyValuePair(Of SearchConditionType, String)()
            Get
                Return New KeyValuePair(Of SearchConditionType, String)() {
                                                                        New KeyValuePair(Of SearchConditionType, String)(SearchConditionType.Equal, FilterConditionEqual),
                                                                        New KeyValuePair(Of SearchConditionType, String)(SearchConditionType.NotEqual, FilterConditionNotEqual),
                                                                        New KeyValuePair(Of SearchConditionType, String)(SearchConditionType.Greater, FilterConditionMoreThan),
                                                                        New KeyValuePair(Of SearchConditionType, String)(SearchConditionType.Less, FilterConditionLessThan),
                                                                        New KeyValuePair(Of SearchConditionType, String)(SearchConditionType.GreaterOrEqual, FilterConditionAndMore),
                                                                        New KeyValuePair(Of SearchConditionType, String)(SearchConditionType.LessOrEqual, FilterConditionAndLess)}
            End Get
        End Property

        Public Shared ReadOnly Property StringConditions As KeyValuePair(Of SearchConditionType, String)()
            Get
                Return New KeyValuePair(Of SearchConditionType, String)() {
                                                                        New KeyValuePair(Of SearchConditionType, String)(SearchConditionType.Match, FilterConditionExactMatch),
                                                                        New KeyValuePair(Of SearchConditionType, String)(SearchConditionType.FrontMatch, FilterConditionForwardMatch),
                                                                        New KeyValuePair(Of SearchConditionType, String)(SearchConditionType.BackMatch, FilterConditionBackwardMatch),
                                                                        New KeyValuePair(Of SearchConditionType, String)(SearchConditionType.Include, FilterConditionInclude),
                                                                        New KeyValuePair(Of SearchConditionType, String)(SearchConditionType.NotInclude, FilterConditionNotInclude)}
            End Get
        End Property

        Public Shared ReadOnly Property DateConditions() As KeyValuePair(Of SearchConditionType, String)()
            Get
                Return New KeyValuePair(Of SearchConditionType, String)() {New KeyValuePair(Of SearchConditionType, String)(SearchConditionType.FromTo, FilterConditionPeriod),
                                                                       New KeyValuePair(Of SearchConditionType, String)(SearchConditionType.[To], FilterConditionUntil),
                                                                       New KeyValuePair(Of SearchConditionType, String)(SearchConditionType.From, FilterConditionFrom)}
            End Get
        End Property

        Private Shared ReadOnly ConditionDictionary As New Dictionary(Of String, ConditionType)

        Private Shared Sub InitializeDictionary()
            ConditionDictionary.Add(FilterConditionEqual, ConditionType.Equal)
            ConditionDictionary.Add(FilterConditionNotEqual, ConditionType.NotEqual)
            ConditionDictionary.Add(FilterConditionMoreThan, ConditionType.MoreThan)
            ConditionDictionary.Add(FilterConditionLessThan, ConditionType.LessThan)
            ConditionDictionary.Add(FilterConditionAndMore, ConditionType.AndMore)
            ConditionDictionary.Add(FilterConditionAndLess, ConditionType.AndLess)
            ConditionDictionary.Add(FilterConditionInclude, ConditionType.Include)
            ConditionDictionary.Add(FilterConditionNotInclude, ConditionType.NotInclude)
            ConditionDictionary.Add(FilterConditionExactMatch, ConditionType.ExactMatch)
            ConditionDictionary.Add(FilterConditionNotMatch, ConditionType.NotEqual)
        End Sub

        Public Shared Function ConvertConditionType(ByVal key As String) As ConditionType
            If ConditionDictionary.Count = 0 Then
                InitializeDictionary()
            End If
            If ConditionDictionary.ContainsKey(key) Then
                Return ConditionDictionary(key)
            End If
            Return Nothing
        End Function

        Public Shared Sub InitializeComboBox(ByVal comboBox As ComboBox, ByVal columnSettingList As List(Of DisplayColumnSetting), ByVal needAllItem As Boolean)
            Dim fieldTable As New DataTable()
            fieldTable.Columns.Add(VALUE_MEMBER_NAME, GetType(String))
            fieldTable.Columns.Add(DISPLAY_MEMBER_NAME, GetType(String))
            If needAllItem Then
                Dim rowAll As DataRow = fieldTable.NewRow()
                rowAll(VALUE_MEMBER_NAME) = FilterPartSetting.FieldAllText
                rowAll(DISPLAY_MEMBER_NAME) = FilterPartSetting.FieldAllText
                fieldTable.Rows.Add(rowAll)
            End If
            For Each columnSetting As DisplayColumnSetting In columnSettingList
                Dim row As DataRow = fieldTable.NewRow()
                row(VALUE_MEMBER_NAME) = columnSetting.Name
                row(DISPLAY_MEMBER_NAME) = columnSetting.DisplayName
                fieldTable.Rows.Add(row)
            Next
            fieldTable.AcceptChanges()
            comboBox.DataSource = fieldTable
            comboBox.ValueMember = VALUE_MEMBER_NAME
            comboBox.DisplayMember = DISPLAY_MEMBER_NAME
            Dim max As Integer = comboBox.Width
            Dim graphics As Graphics = comboBox.CreateGraphics()
            For Each row As DataRow In fieldTable.Rows
                max = Math.Max(max, graphics.MeasureString(TryCast(row.Item(DISPLAY_MEMBER_NAME), String),
                                                           comboBox.Font).Width + DROP_DOWN_WIDTH_MARGIN)
            Next
            comboBox.DropDownWidth = max
        End Sub

        Public Shared Function CreateSearchCondition(ByVal conditionType As ConditionType, ByVal fieldName As String,
                                                     ByVal searchString As String,
                                                     ByVal type As DataType, ByVal parentTool As Control) As ConditionList
            If String.IsNullOrEmpty(fieldName) Then Return Nothing
            If searchString Is Nothing Then Return Nothing

            Select Case type
                Case DataType.Text
                    Return CreateConditionList(fieldName, conditionType, searchString,
                                               parentTool, DbType.String)
                Case DataType.IntegerNumber
                    Return CreateConditionList(fieldName, conditionType, searchString,
                                               parentTool, DbType.Int32)
                Case DataType.FloatNumber
                    Return CreateConditionList(fieldName, conditionType, searchString,
                                               parentTool, DbType.Double)
                Case DataType.DateText
                    Dim conditionList As New ConditionList
                    conditionList.Add(New DateTimeConditionData(conditionType,
                                                                New DateTimeFieldMember(fieldName),
                                                                New DateTimeLiteralMember(searchString)))
                    Return conditionList
            End Select
            Return Nothing
        End Function

        Private Shared Function CreateConditionList(ByVal fieldName As String, ByVal conditionType As ConditionType,
                                                    ByVal searchString As String, ByVal parentTool As Control,
                                                    ByVal dbType As DbType) As ConditionList

            Dim conditionList As New ConditionList
            If fieldName = FilterPartSetting.FieldAllText Then
                Dim allFieldName As String() = GetAllFieldNames(parentTool)
                If Not allFieldName.Any() Then Return Nothing
                Dim combine As New CombineConditionData()
                combine.ConditionList.Add(New MultipleColumnLiteralConditionData(conditionType, allFieldName, searchString, dbType,
                                                                                   (conditionType = conditionType.NotEqual OrElse conditionType = conditionType.NotInclude)))
                If searchString = String.Empty And Not (conditionType = conditionType.NotInclude) Then
                    combine.ConditionList.Add(New MultipleColumnIsNullConditionData(allFieldName, (conditionType = conditionType.NotEqual OrElse conditionType = conditionType.NotInclude)))
                End If
                conditionList.Add(combine)

                Return conditionList
            End If
            If conditionType <> conditionType.NotEqual Then
                Dim combine As New CombineConditionData()
                combine.ConditionList.Add(New LiteralConditionData(conditionType, fieldName, searchString, dbType))

                If searchString = String.Empty And Not (conditionType = conditionType.NotInclude) Then
                    combine.AddIsNull(fieldName, (conditionType = conditionType.NotEqual OrElse conditionType = conditionType.NotInclude))
                End If

                conditionList.Add(combine)
            Else
                Dim combine As New CombineConditionData()
                combine.AddIsNull(fieldName)
                combine.ConditionList.Add(New LiteralConditionData(conditionType, fieldName, searchString, dbType))
                conditionList.Add(combine)
            End If
            Return conditionList
        End Function

        Private Shared Function GetAllFieldNames(ByVal parentTool As Control) As String()
            Dim parentSheet As IBtSheet = TryCast(BasicUtility.GetParentControl(Of IBtSheet)(parentTool), IBtSheet)
            If IsNothing(parentSheet) Then Return New String() {}
            Return parentSheet.GetAllFieldName()
        End Function

        Public Shared Function GetDateFieldNames(ByVal parentTool As Control) As String()
            Dim parentSheet As IBtSheet = TryCast(BasicUtility.GetParentControl(Of IBtSheet)(parentTool), IBtSheet)
            If IsNothing(parentSheet) Then Return New String() {}
            Return parentSheet.SheetSetting.DateColumnNameList.ToArray()
        End Function

        Public Shared Sub SetDropDownWidth(ByVal comboBox As ComboBox)
            Dim max As Integer = comboBox.Width
            Dim graphics As Graphics = comboBox.CreateGraphics()
            For Each keyword As String In comboBox.Items
                max = Math.Max(max,
                               graphics.MeasureString(keyword, comboBox.Font).Width + DROP_DOWN_WIDTH_MARGIN)
            Next
            comboBox.DropDownWidth = max
        End Sub
    End Class
End Namespace
