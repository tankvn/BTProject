Imports System.Globalization
Imports System.Windows.Forms
Imports SettingLib
Imports SettingLib.Part
Imports Database.SqlData
Imports Database.SqlData.Condition
Imports Database
Imports Database.SqlData.DisplayData
Imports InterfaceLib
Imports UtilityLib
Imports ViewLib.My.Resources

Namespace TabPart
    Public Class ProgressDisplayGraph
        Implements IUpdatablePart, ISheetChild

        Public Property ProgressItem As ProgressItem

        Public Property ShowRest() As Boolean

        Public Property ShowProgressionRate() As Boolean

        Public Property ProgressBar() As ProgressBar

        Public Property ProgressionRateLabel() As Label

        Public Property RestLabel() As Label

        Public Property Setting() As ProgressDisplaySetting

        Public Sub LoadSetting() Implements ISheetChild.LoadSetting
            If FileUtility.IsDesignMode(DesignMode) Then Return

            Setting = ViewUtility.GetPartSetting(Name, Parent, DesignMode)
            UpdateView("0", 0)
        End Sub

        Public Function UpdateView(ByVal dbAccessor As DbAccessor, ByVal tableName As String, ByVal conditionList As ConditionList,
                                   ByVal calculationColumnSettings As CalculationColumnDisplayDataList, ByVal autoUpdate As Boolean) As Boolean Implements IUpdatablePart.UpdateView
            If String.IsNullOrEmpty(Setting.OrderCountColumnName) OrElse
                String.IsNullOrEmpty(Setting.ProcessedCountColumnName) OrElse
                String.IsNullOrEmpty(Setting.WorkNoColumnName) Then Return True
            Dim fields As String() = dbAccessor.GetFields(tableName)
            Dim orderResult As String()() = GetOrderCount(tableName, dbAccessor)
            Dim remainResult As String()() = GetRemainingCount(tableName, dbAccessor)
            If Not fields.Contains(Setting.ProcessedCountColumnName) Then
                remainResult = orderResult
            End If
            If IsNothing(orderResult) OrElse IsNothing(remainResult) Then Return False

            Dim rate As Integer
            Dim rest As String = String.Empty
            CalculateRestAndProgressRate(orderResult, remainResult, rest, rate)
            UpdateView(rest, rate)
            Return True
        End Function

        Private Sub UpdateView(ByVal rest As String, ByVal rate As Integer)
            Dim rateValue As Integer = Math.Min(Math.Max(rate, ProgressBar.Minimum), ProgressBar.Maximum)
            ProgressBar.Value = rateValue

            If ShowProgressionRate AndAlso Not IsNothing(ProgressionRateLabel) Then
                ProgressionRateLabel.Text = rateValue.ToString("0\%", CultureInfo.InvariantCulture)
            End If
            If ShowRest AndAlso Not IsNothing(RestLabel) Then
                RestLabel.Text = String.Format(ProgressGraphRestTextFormat, rest)
            End If
        End Sub

        Private Sub CalculateRestAndProgressRate(ByVal orderResult As String()(), ByVal remainResult As String()(), ByRef rest As String, ByRef rate As Integer)
            Select Case ProgressItem
                Case ProgressItem.Work
                    Dim slipOrder As String = orderResult(0)(0)
                    rest = remainResult(0)(0)
                    Dim slipCompletion As String = GetCompletionCount(slipOrder, rest)
                    rate = GetRate(slipOrder, slipCompletion)
                Case ProgressItem.Item
                    Dim itemOrder As String = orderResult(0)(0)
                    rest = remainResult(0)(0)
                    Dim itemCompletion As String = GetCompletionCount(itemOrder, rest)
                    rate = GetRate(itemOrder, itemCompletion)
                Case ProgressItem.Figure
                    Dim particularOrder As String = orderResult(0)(0)
                    particularOrder = If(String.IsNullOrEmpty(particularOrder), "0", particularOrder)
                    rest = remainResult(0)(0)
                    rest = If(String.IsNullOrEmpty(rest), "0", rest)
                    Dim particularCompletion As String = GetCompletionCount(particularOrder, rest)
                    rate = GetRate(particularOrder, particularCompletion)
            End Select
        End Sub

        Private Function GetRate(ByVal order As String, ByVal completion As String) As Integer
            Try
                Dim orderNum = Convert.ToDouble(order)
                If orderNum = 0 Then Return 0
                Dim rate = Convert.ToDouble(completion) / orderNum * 100
                Return Convert.ToInt32(Math.Floor(rate))
            Catch ex As Exception
                Return 0
            End Try
        End Function

        Private Function GetCompletionCount(ByVal order As String, ByVal remaining As String) As String
            Try
                Return (Convert.ToInt64(order) - Convert.ToInt64(remaining)).ToString(CultureInfo.InvariantCulture)
            Catch ex As Exception
                Return "0"
            End Try
        End Function

        Private Function GetRemainingCount(ByVal tableName As String, ByVal dbAccessor As DbAccessor) As String()()
            Dim command As New SqlSelectCommand(tableName)
            Select Case ProgressItem
                Case ProgressItem.Work
                    command.DisplayDataList.Add(New CountFieldDisplayData(Setting.WorkNoColumnName, True))
                Case ProgressItem.Item
                    command.DisplayDataList.Add(New CountConditionDisplayData())
                Case ProgressItem.Figure
                    command.DisplayDataList.Add(New SumDisplayData(New SubtractionDisplayData(Setting.OrderCountColumnName, Setting.ProcessedCountColumnName)))
            End Select
            command.ConditionList.Add(New FieldConditionData(ConditionType.LessThan, Setting.ProcessedCountColumnName, Setting.OrderCountColumnName, ComparisonType.Numeric))
            Try
                Return dbAccessor.ExecuteCommandAndRead(command)
            Catch ex As Exception
                Return New String()() {New String() {"0"}}
            End Try
        End Function

        Private Function GetOrderCount(ByVal tableName As String, ByVal dbAccessor As DbAccessor) As String()()
            Dim command As New SqlSelectCommand(tableName)
            Select Case ProgressItem
                Case ProgressItem.Work
                    command.DisplayDataList.Add(New CountFieldDisplayData(Setting.WorkNoColumnName, True))
                Case ProgressItem.Item
                    command.DisplayDataList.Add(New CountConditionDisplayData())
                Case ProgressItem.Figure
                    command.DisplayDataList.Add(New SumDisplayData(Setting.OrderCountColumnName))
            End Select
            Try
                Return dbAccessor.ExecuteCommandAndRead(command)
            Catch ex As Exception
                Return New String()() {New String() {"0"}}
            End Try
        End Function
    End Class
End Namespace
