Namespace SqlData.DisplayData
    Public Class CalculationColumnDisplayDataList
        Implements IEnumerable(Of ICalculationColumnDisplayData)

        Private ReadOnly _dataList As List(Of ICalculationColumnDisplayData)
        Public ReadOnly Property DataList() As List(Of ICalculationColumnDisplayData)
            Get
                Return _dataList
            End Get
        End Property

        Public Sub New()
            _dataList = New List(Of ICalculationColumnDisplayData)()
        End Sub

        Public Sub Add(ByVal calculationDisplayData As ICalculationColumnDisplayData)
            _dataList.Add(calculationDisplayData)
        End Sub

        Public Sub AddRange(ByVal columnDisplayDataList As CalculationColumnDisplayDataList)
            _dataList.AddRange(columnDisplayDataList)
        End Sub

        Public Function IsCalculationColumn(ByVal columnName As String) As Boolean
            For Each calculationDisplayData As ICalculationColumnDisplayData In _dataList
                If calculationDisplayData.Name = columnName Then
                    Return True
                End If
            Next
            Return False
        End Function

        Public Function GetData(ByVal columnName As String) As ICalculationColumnDisplayData
            For Each calculationColumnDisplayData As ICalculationColumnDisplayData In _dataList
                If calculationColumnDisplayData.Name = columnName Then
                    Return calculationColumnDisplayData
                End If
            Next
            Return Nothing
        End Function

        Public Function GetVisibleData() As CalculationColumnDisplayDataList
            Dim columnDisplayDataList As New CalculationColumnDisplayDataList()
            For Each calculationColumnDisplayData As ICalculationColumnDisplayData In _dataList
                If Not calculationColumnDisplayData.Visible Then Continue For
                columnDisplayDataList.Add(calculationColumnDisplayData)
            Next
            Return columnDisplayDataList
        End Function

        Public Sub UpdateMemberData()
            For Each calculationColumnDisplayData As ICalculationColumnDisplayData In _dataList
                calculationColumnDisplayData.UpdateMemberData(Me)
            Next
        End Sub

        Public Function IEnumerable_GetEnumerator() As IEnumerator(Of ICalculationColumnDisplayData) _
            Implements IEnumerable(Of ICalculationColumnDisplayData).GetEnumerator
            Return _dataList.GetEnumerator()
        End Function

        Public Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return _dataList.GetEnumerator()
        End Function
    End Class
End Namespace
