Imports Database.SqlData.Condition.Member
Imports UtilityLib.Controls
Imports Database.SqlData.Condition
Imports Database.SqlData.DisplayData
Imports System.Globalization
Imports SettingLib
Imports SettingLib.Part
Imports Database.SqlData
Imports SettingLib.Table

Public Class ConvertUtility

    Public Shared Function ToDisplayColumnSettingList(ByVal fieldNames As String(), ByVal displayNames As String(),
                                                      ByVal widths As Integer(), ByVal columnAppearanceSettings As ColumnAppearanceSetting()) As List(Of DisplayColumnSetting)
        If IsNothing(fieldNames) OrElse IsNothing(displayNames) Then Throw New ArgumentException()
        If fieldNames.Length <> displayNames.Length Then Throw New ArgumentException()
        Dim list = New List(Of DisplayColumnSetting)
        For index As Integer = 0 To fieldNames.Length - 1
            list.Add(New DisplayColumnSetting(fieldNames(index), displayNames(index), widths(index), columnAppearanceSettings(index)))
        Next
        Return list
    End Function

    Public Shared Function ToCalculationDisplayColumnSettingList(ByVal fieldNames As String(),
                                                                 ByVal displayNames As String(),
                                                                 ByVal widths As Integer(),
                                                                 ByVal columnAppearanceSettings As ColumnAppearanceSetting(),
                                                                 ByVal showDecimalPoints As Boolean(),
                                                                 ByVal decimalPlaces As Integer(),
                                                                 ByVal showPercent As Boolean()) As List(Of CalculationDisplayColumnSetting)
        If IsNothing(fieldNames) OrElse IsNothing(displayNames) Then Throw New ArgumentException()
        If fieldNames.Length <> displayNames.Length Then Throw New ArgumentException()
        Dim list = New List(Of CalculationDisplayColumnSetting)
        For index As Integer = 0 To fieldNames.Length - 1
            list.Add(New CalculationDisplayColumnSetting(fieldNames(index), displayNames(index), widths(index), columnAppearanceSettings(index),
                                                         showDecimalPoints(index), decimalPlaces(index), showPercent(index)))
        Next
        Return list
    End Function

    Public Shared Function ToDisplayColumnSettingList(ByVal fieldNames As String(), ByVal displayNames As String()) As List(Of DisplayColumnSetting)
        If IsNothing(fieldNames) OrElse IsNothing(displayNames) Then Throw New ArgumentException()
        If fieldNames.Length <> displayNames.Length Then Throw New ArgumentException()
        Dim list = New List(Of DisplayColumnSetting)
        For index As Integer = 0 To fieldNames.Length - 1
            list.Add(New DisplayColumnSetting(fieldNames(index), displayNames(index)))
        Next
        Return list
    End Function

    Public Shared Function ToDisplayDataList(ByVal list As List(Of DisplayColumnSetting), ByVal calculationColumnList As CalculationColumnDisplayDataList) As DisplayDataList
        Dim displayDataList = New DisplayDataList()
        If IsNothing(list) Then Return displayDataList
        For Each displayColumnSetting As DisplayColumnSetting In list
            Dim calculationColumnData As ICalculationColumnDisplayData = GetCalculationColumnSetting(displayColumnSetting.Name, calculationColumnList)

            If IsNothing(calculationColumnData) Then
                displayDataList.Add(New ColumnDisplayData(displayColumnSetting.Name))
                Continue For
            End If

            displayDataList.Add(calculationColumnData)
        Next
        Return displayDataList
    End Function

    Public Shared Function ToAllDisplayDataList(ByVal list As List(Of DisplayColumnSetting), ByVal calculationColumnList As CalculationColumnDisplayDataList) As DisplayDataList
        Dim displayDataList = New DisplayDataList()
        If IsNothing(list) Then Return displayDataList
        displayDataList.Add(New AllDisplayData())
        For Each displayColumnSetting As DisplayColumnSetting In list
            Dim calculationColumnData As ICalculationColumnDisplayData = GetCalculationColumnSetting(displayColumnSetting.Name, calculationColumnList)
            If IsNothing(calculationColumnData) Then Continue For
            displayDataList.Add(calculationColumnData)
        Next
        Return displayDataList
    End Function

    Private Shared Function GetCalculationColumnSetting(ByVal name As String,
                                                        ByVal calculationColumnList As CalculationColumnDisplayDataList) _
        As ICalculationColumnDisplayData
        If IsNothing(calculationColumnList) Then Return Nothing

        For Each calculationColumnSetting As ICalculationColumnDisplayData In calculationColumnList.DataList
            If (name.Equals(calculationColumnSetting.Name)) Then Return calculationColumnSetting
        Next
        Return Nothing
    End Function

    Public Shared Function ToStringArray(ByVal dataTable As DataTable) As String()()
        Dim arrayList As New List(Of String())
        For Each row As DataRow In dataTable.Rows
            Dim list As New List(Of String)
            For Each item As Object In row.ItemArray
                list.Add(item.ToString())
            Next
            arrayList.Add(list.ToArray())
        Next
        Return arrayList.ToArray()
    End Function

    Public Shared Function ToSeparatorString(ByVal separator As RowPreviewSeparator) As String
        Select Case separator
            Case RowPreviewSeparator.Cologne
                Return ":"
            Case RowPreviewSeparator.None
                Return String.Empty
        End Select
        Return ":"
    End Function

    Public Shared Function ConvertConditionData(ByVal filterSetting As FilterSetting,
                                                ByVal calculationDisplayDataList As CalculationColumnDisplayDataList) As IConditionData
        Return ToConditionData(filterSetting, calculationDisplayDataList)
    End Function

    Private Shared Function ToConditionData(ByVal filterSetting As FilterSetting,
                                            ByVal calculationDisplayDataList As CalculationColumnDisplayDataList) As IConditionData
        Dim condition As IConditionData
        condition = Nothing
        If filterSetting.ComparisonValueType = DataType.DateText Then
            Dim sourceValue As DateTimeMember = GetDateTimeSourceValue(filterSetting)
            Dim comparisonValue As DateTimeMember = Nothing
            comparisonValue = GetDateTimeComparisonValue(filterSetting, comparisonValue)

            condition = New DateTimeConditionData(filterSetting.ConditionType, sourceValue, comparisonValue)
        Else
            Select Case filterSetting.SourceMemberType
                Case MemberType.FixedValue
                    Throw New ArgumentException()
                Case MemberType.Field
                    Dim leftDisplayData As IDisplayData = DbAccessUtility.GetDisplayData(filterSetting.SourceName, calculationDisplayDataList)
                    If filterSetting.ComparisonMemberType = MemberType.FixedValue Then
                        Dim comparisonValue As Object = filterSetting.ComparisonValue
                        If filterSetting.ComparisonValueType = DataType.Text Then
                            comparisonValue = EscapeSpecialCharacters(filterSetting.ConditionType, comparisonValue)
                        End If
                        condition = New LiteralConditionData(filterSetting.ConditionType,
                                                             leftDisplayData,
                                                             filterSetting.CalculationType,
                                                             comparisonValue,
                                                             DataTypeToDbType(filterSetting.ComparisonValueType),
                                                             filterSetting.CalculationValue)
                    ElseIf filterSetting.ComparisonMemberType = MemberType.Field Then
                        Dim rightDisplayData As IDisplayData = DbAccessUtility.GetDisplayData(filterSetting.ComparisonValue.ToString(),
                                                                                              calculationDisplayDataList)
                        If filterSetting.ComparisonValueType = DataType.Text Then
                            condition = New FieldConditionData(filterSetting.ConditionType, leftDisplayData, rightDisplayData,
                                                               DataTypeToComparisonType(filterSetting.ComparisonValueType))
                        Else
                            condition = New FieldConditionData(filterSetting.ConditionType, leftDisplayData, filterSetting.CalculationType,
                                                               rightDisplayData, filterSetting.CalculationValue)
                        End If
                    ElseIf filterSetting.ComparisonMemberType = MemberType.CurrentDate Then
                        Dim comparisonValue As Object = Nothing
                        Select Case filterSetting.ComparisonValueType
                            Case DataType.Text
                                comparisonValue = DateTime.Now.ToString(DateTimeUtility.SQLITE_DATE_FORMAT)
                            Case DataType.FloatNumber, DataType.IntegerNumber
                                comparisonValue = DateTime.Now.Year
                        End Select
                        condition = New LiteralConditionData(filterSetting.ConditionType,
                                                             leftDisplayData,
                                                             filterSetting.CalculationType,
                                                             comparisonValue,
                                                             DataTypeToDbType(filterSetting.ComparisonValueType),
                                                             filterSetting.CalculationValue)
                    End If
                Case MemberType.CurrentDate
                    Dim left As Object = Nothing
                    Select Case filterSetting.ComparisonValueType
                        Case DataType.Text
                            left = DateTime.Now.ToString(DateTimeUtility.SQLITE_DATE_FORMAT)
                        Case DataType.FloatNumber, DataType.IntegerNumber
                            left = DateTime.Now.Year
                    End Select
                    If filterSetting.ComparisonMemberType = MemberType.FixedValue Then
                        condition = New LiteralLiteralConditionData(filterSetting.ConditionType,
                                                             left,
                                                             filterSetting.ComparisonValue,
                                                             DataTypeToDbType(filterSetting.ComparisonValueType),
                                                             filterSetting.CalculationType,
                                                             filterSetting.CalculationValue)
                    ElseIf filterSetting.ComparisonMemberType = MemberType.Field Then
                        Dim rightDisplayData As IDisplayData = DbAccessUtility.GetDisplayData(filterSetting.ComparisonValue.ToString(),
                                                                                              calculationDisplayDataList)
                        condition = New LiteralFieldConditionData(filterSetting.ConditionType, left, filterSetting.CalculationType,
                                                                  rightDisplayData, filterSetting.CalculationValue, DataTypeToDbType(filterSetting.ComparisonValueType))
                    End If
                Case MemberType.CurrentTime
                    Throw New ArgumentException()
            End Select

        End If
        Return condition
    End Function

    Private Shared Function GetDateTimeComparisonValue(ByVal filterSetting As FilterSetting, ByVal comparisonValue As DateTimeMember) As DateTimeMember

        Select Case filterSetting.ComparisonMemberType
            Case MemberType.FixedValue
                comparisonValue = New DateTimeLiteralMember(filterSetting.ComparisonValue.ToString())
            Case MemberType.Field
                comparisonValue = New DateTimeFieldMember(filterSetting.ComparisonValue.ToString())
            Case MemberType.CurrentDate
                comparisonValue = New DateTimeLiteralMember(DateTime.Now.ToString(DateTimeUtility.SQLITE_DATE_FORMAT))
            Case MemberType.CurrentTime
                Throw New ArgumentException()
        End Select
        Return comparisonValue
    End Function

    Private Shared Function GetDateTimeSourceValue(ByVal filterSetting As FilterSetting) As DateTimeMember

        Dim sourceValue As DateTimeMember = Nothing
        Select Case filterSetting.SourceMemberType
            Case MemberType.FixedValue
                sourceValue = New DateTimeLiteralMember(filterSetting.SourceName.ToString(),
                                                        filterSetting.CalculationType, filterSetting.CalculationValue,
                                                        filterSetting.CalculationDateUnit)
            Case MemberType.Field
                sourceValue = New DateTimeFieldMember(filterSetting.SourceName.ToString(),
                                                      filterSetting.CalculationType, filterSetting.CalculationValue,
                                                      filterSetting.CalculationDateUnit)
            Case MemberType.CurrentDate
                sourceValue = New DateTimeLiteralMember(DateTime.Now.ToString(DateTimeUtility.SQLITE_DATE_FORMAT),
                                                        filterSetting.CalculationType, filterSetting.CalculationValue,
                                                        filterSetting.CalculationDateUnit)
            Case MemberType.CurrentTime
                Throw New ArgumentException()
        End Select
        Return sourceValue
    End Function

    Public Shared Function ToConditionData(ByVal filterSettingList As FilterSettingList, ByVal calculationDisplayDataList As CalculationColumnDisplayDataList) As IConditionData
        If filterSettingList.ConditionList.Count <= 0 Then Return Nothing
        Dim condition As New CombineConditionData()
        condition.OrCombining = (filterSettingList.DeterminationMethod = DeterminationMethod.OrCombining)
        For Each filterSetting As FilterSetting In filterSettingList.ConditionList
            condition.ConditionList.Add(ConvertConditionData(filterSetting, calculationDisplayDataList))
        Next
        Return condition
    End Function

    Public Shared Function ToConditionType(ByVal searchCondition As SearchConditionType) As ConditionType
        Select Case searchCondition
            Case SearchConditionType.Match
                Return ConditionType.ExactMatch
            Case SearchConditionType.FrontMatch
                Return ConditionType.ForwardMatch
            Case SearchConditionType.BackMatch
                Return ConditionType.BackwardMatch
            Case SearchConditionType.Include
                Return ConditionType.Include
            Case SearchConditionType.NotInclude
                Return ConditionType.NotInclude
            Case SearchConditionType.Equal
                Return ConditionType.Equal
            Case SearchConditionType.NotEqual
                Return ConditionType.NotEqual
            Case SearchConditionType.Greater
                Return ConditionType.MoreThan
            Case SearchConditionType.Less
                Return ConditionType.LessThan
            Case SearchConditionType.GreaterOrEqual
                Return ConditionType.AndMore
            Case SearchConditionType.LessOrEqual
                Return ConditionType.AndLess
            Case SearchConditionType.FromTo
            Case SearchConditionType.[To]
            Case SearchConditionType.From
        End Select
        Return ConditionType.Equal
    End Function

    Public Shared Function ToFieldString(ByVal searchTarget As SearchTarget) As String
        If searchTarget = searchTarget.All Then
            Return FilterPartSetting.FieldAllText
        End If

        If searchTarget.DB1 <= searchTarget AndAlso searchTarget <= searchTarget.DB32 Then
            Return String.Format(TableSetting.FIELD_NAME_FORMAT, Convert.ToInt32(searchTarget).ToString(CultureInfo.InvariantCulture))
        End If

        Return String.Format(TableSetting.CALCULATION_COLUMN_NAME_FORMAT,
                             (Convert.ToInt32(searchTarget) - searchTarget.DB32).ToString(CultureInfo.InvariantCulture))
    End Function

    Public Shared Function ToBtTreeNode(ByVal setting As TreeNodeSetting) As BtTreeNode
        Return New BtTreeNode(If(IsNothing(setting.Keyword), String.Empty, setting.Keyword), setting.Field, ToConditionType(setting.ConditionType))
    End Function

    Public Shared Function ToSearchConditionType(ByVal condition As ConditionType) As SearchConditionType
        Select Case condition
            Case ConditionType.ExactMatch
                Return SearchConditionType.Match
            Case ConditionType.ForwardMatch
                Return SearchConditionType.FrontMatch
            Case ConditionType.BackwardMatch
                Return SearchConditionType.BackMatch
            Case ConditionType.Include
                Return SearchConditionType.Include
            Case ConditionType.NotInclude
                Return SearchConditionType.NotInclude
            Case ConditionType.Equal
                Return SearchConditionType.Equal
            Case ConditionType.NotEqual
                Return SearchConditionType.NotEqual
            Case ConditionType.MoreThan
                Return SearchConditionType.Greater
            Case ConditionType.LessThan
                Return SearchConditionType.Less
            Case ConditionType.AndMore
                Return SearchConditionType.GreaterOrEqual
            Case ConditionType.AndLess
                Return SearchConditionType.LessOrEqual
        End Select
        Return SearchConditionType.Equal
    End Function
End Class
