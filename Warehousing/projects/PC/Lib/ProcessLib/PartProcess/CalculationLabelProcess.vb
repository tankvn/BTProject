Imports Database.SqlData.Condition
Imports Database
Imports Database.SqlData.DisplayData
Imports System.Globalization
Imports SettingLib
Imports SettingLib.Part
Imports UtilityLib

Namespace PartProcess

    Public Class CalculationLabelProcess

        Public Shared Function GetDisplayString(ByVal dbAccessor As DbAccessor, ByVal tableName As String, ByVal conditionList As ConditionList,
                                                ByVal allDisplayDataList As DisplayDataList, ByVal labelDisplayData As DisplayDataList,
                                                ByVal useNumberGroupSeparator As Boolean) As String
            Dim data As String = DbAccessUtility.GetLabelString(dbAccessor, tableName, conditionList, allDisplayDataList, labelDisplayData)
            If String.IsNullOrEmpty(data) Then
                data = "0"
            End If
            Dim labelString As String
            If data.Contains(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator) Then
                If useNumberGroupSeparator Then
                    labelString = Convert.ToDouble(data).ToString("#,0.##")
                Else
                    labelString = Convert.ToDouble(data).ToString("0.##")
                End If

            Else
                If useNumberGroupSeparator Then
                    labelString = Convert.ToInt64(data).ToString("#,0")
                Else
                    labelString = Convert.ToInt64(data).ToString("0")
                End If
            End If
            Return labelString
        End Function

        Public Shared Function CreateDisplayData(ByVal setting As CalculationLabelSetting,
                                                 ByVal calculationColumnList As CalculationColumnDisplayDataList) As DisplayDataList
            Dim displayDataList As DisplayDataList = New DisplayDataList()
            Dim leftDisplayData As IDisplayData = DbAccessUtility.GetDisplayData(setting.TargetColumn, calculationColumnList)
            Select Case setting.CalculationLabelType
                Case CalculationLabelType.NumericalNumber
                    Dim condition = New LiteralConditionData(ConditionType.NotEqual, setting.TargetColumn, String.Empty)
                    displayDataList.Add(New CountConditionDisplayData(condition))
                Case CalculationLabelType.Sum
                    displayDataList.Add(New SumDisplayData(leftDisplayData))
                Case CalculationLabelType.Average
                    displayDataList.Add(New AverageDisplayData(leftDisplayData))
            End Select
            Return displayDataList
        End Function
    End Class
End Namespace
