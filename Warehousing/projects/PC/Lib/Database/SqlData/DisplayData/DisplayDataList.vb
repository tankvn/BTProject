
Namespace SqlData.DisplayData
    Public Class DisplayDataList
        Private ReadOnly _displayDataList As New List(Of IDisplayData)

        Public Sub New(ByVal displayDatas As ICollection(Of IDisplayData))
            _displayDataList.AddRange(displayDatas)
        End Sub

        Public Sub New(ByVal displayDatas As ICollection(Of String))
            For Each displayData As String In displayDatas
                _displayDataList.Add(New ColumnDisplayData(displayData))
            Next
        End Sub

        Public Sub New()
        End Sub

        Public ReadOnly Property Count() As Integer
            Get
                Return _displayDataList.Count
            End Get
        End Property

        Public Sub Add(ByVal displayData As IDisplayData)
            _displayDataList.Add(displayData)
        End Sub

        Public Sub AddRange(ByVal displayData As ICollection(Of IDisplayData))
            _displayDataList.AddRange(displayData)
        End Sub

        Public Sub AddRange(ByVal displayData As ICollection(Of ICalculationColumnDisplayData))
            _displayDataList.AddRange(displayData)
        End Sub

        Public Sub AddColumn(ByVal columnName As String)
            _displayDataList.Add(New ColumnDisplayData(columnName))
        End Sub

        Public Sub AddColumn(ByVal columnName As String, ByVal displayName As String)
            _displayDataList.Add(New ColumnDisplayData(columnName, displayName))
        End Sub

        Public Sub AddColumnRange(ByVal columnNames As ICollection(Of String))
            For Each columnName As String In columnNames
                _displayDataList.Add(New ColumnDisplayData(columnName))
            Next
        End Sub

        Public Sub AddColumnRange(ByVal columnNames As ICollection(Of String), ByVal displayNames As ICollection(Of String))
            If Not columnNames.Count = displayNames.Count Then Throw New Exception()

            For index As Integer = 0 To columnNames.Count - 1
                AddColumn(columnNames(index), displayNames(index))
            Next
        End Sub

        Public Sub AddRange(ByVal displayDataList As DisplayDataList)
            If IsNothing(displayDataList) Then Return
            _displayDataList.AddRange(displayDataList._displayDataList)
        End Sub

        Public Sub AddRange(ByVal dataList As CalculationColumnDisplayDataList)
            For Each calculationColumnDisplayData As ICalculationColumnDisplayData In dataList.DataList
                If TypeOf (calculationColumnDisplayData) Is IDisplayData Then
                    _displayDataList.Add(calculationColumnDisplayData)
                End If
            Next
        End Sub

        Public Function GetColumnDisplayDataFieldName() As String()
            Dim list As New List(Of String)()
            For Each displayData As IDisplayData In _displayDataList
                Dim columnDisplayData As ColumnDisplayData = TryCast(displayData, ColumnDisplayData)
                If Not IsNothing(columnDisplayData) Then
                    list.Add(columnDisplayData.FieldName)
                End If
                Dim calcDisplayData As CalculationDisplayData = TryCast(displayData, CalculationDisplayData)
                If Not IsNothing(calcDisplayData) Then
                    list.Add(calcDisplayData.Name)
                End If
            Next
            Return list.ToArray()
        End Function

        Friend Function ToSqlAndParameter() As SqlAndParameterList
            Dim sqlString As String = String.Empty
            Dim parameterList As New List(Of Parameter)
            For Each column As IDisplayData In _displayDataList
                Dim sqlAndParameters As SqlAndParameterList = column.ToSqlAndParameters()
                sqlString += sqlAndParameters.SqlString + ","
                parameterList.AddRange(sqlAndParameters.ParameterList)
            Next
            sqlString = sqlString.TrimEnd(",")
            If _displayDataList.Count = 0 Then
                sqlString = "*"
            End If
            Return New SqlAndParameterList(sqlString, parameterList)
        End Function

        Friend Function ToSqlAndParameterForCalculationLabel() As SqlAndParameterList
            Dim sqlString As String = String.Empty
            Dim parameterList As New List(Of Parameter)
            For Each column As IDisplayData In _displayDataList
                Dim sqlAndParameters As SqlAndParameterList
                If TypeOf column Is ICalculationColumnDisplayData Then
                    sqlAndParameters = column.ToSqlAndParameters()
                Else
                    sqlAndParameters = column.ToSqlAndParametersWithoutDisplayName()
                End If
                sqlString += sqlAndParameters.SqlString + ","
                parameterList.AddRange(sqlAndParameters.ParameterList)
            Next
            sqlString = sqlString.TrimEnd(",")
            If _displayDataList.Count = 0 Then
                sqlString = "*"
            End If
            Return New SqlAndParameterList(sqlString, parameterList)
        End Function
    End Class
End Namespace
