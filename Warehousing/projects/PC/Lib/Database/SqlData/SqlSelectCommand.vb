Imports Database.SqlData.Condition
Imports Database.SqlData.DisplayData
Imports Database.My.Resources
Imports Database.Utility
Imports System.Data.SqlClient
Imports System.Globalization

Namespace SqlData
    Public NotInheritable Class SqlSelectCommand
        Implements ISqlCommand

        Private ReadOnly _displayDataList As DisplayDataList
        Public ReadOnly Property DisplayDataList() As DisplayDataList
            Get
                Return _displayDataList
            End Get
        End Property

        Private ReadOnly _tableName As String
        Public ReadOnly Property TableName() As String Implements ISqlCommand.TableName
            Get
                Return _tableName
            End Get
        End Property

        Private ReadOnly _conditionList As ConditionList
        Public ReadOnly Property ConditionList() As ConditionList Implements ISqlCommand.ConditionList
            Get
                Return _conditionList
            End Get
        End Property

        Public Property IsDistinct() As Boolean = False

        Public Property SortColumn() As String = String.Empty

        Public Property SortOrder() As SortOrder = SortOrder.Ascending

        Public Property Limit() As Integer = 0

        Public Property Offset() As Integer = 0

        Public Sub New(ByVal tableName As String)
            _tableName = EncloseIdentifier(tableName)
            _displayDataList = New DisplayDataList()
            _conditionList = New ConditionList()
        End Sub

        Public Sub New(ByVal schemaName As String, ByVal tableName As String)
            Me.New(String.Empty)
            _tableName = EncloseIdentifier(schemaName) + "." + EncloseIdentifier(tableName)
        End Sub

        Private Function GetSortOrderString(ByVal order As SortOrder) As String
            Select Case order
                Case SortOrder.Ascending
                    Return " ASC"
                Case SortOrder.Descending
                    Return " DESC"
            End Select
            Return String.Empty
        End Function

        Friend Function ToSqlCommand(ByVal connection As DbConnection) As DbCommand Implements ISqlCommand.ToSqlCommand
            Dim sqlString As String = _displayDataList.ToSqlAndParameter().SqlString
            Dim conditionSqlAndParameterList As SqlAndParameterList = _conditionList.ToWhereSql()
            Dim formatString = If(IsDistinct, SelectDistinctSql, SelectSql)
            Dim commandText As String = String.Format(formatString, sqlString, _tableName, conditionSqlAndParameterList.SqlString)
            commandText = AddOrder(commandText)
            commandText = AddLimit(commandText)
            Return CreateDbCommand(connection, _displayDataList, conditionSqlAndParameterList, commandText)
        End Function

        Private Function AddOrder(ByVal commandText As String) As String
            If Not String.IsNullOrEmpty(SortColumn) Then
                commandText = commandText + " ORDER BY " + SortColumn + GetSortOrderString(SortOrder)
            End If
            Return commandText
        End Function

        Private Function AddLimit(ByVal commandText As String) As String
            If 0 < Limit Then
                commandText = commandText + " LIMIT " + Limit.ToString(CultureInfo.InvariantCulture)
                If 0 < Offset Then
                    commandText = commandText + " OFFSET " + Offset.ToString(CultureInfo.InvariantCulture)
                End If
            End If
            Return commandText
        End Function

        Public Function ToSqlAndParametersForCalculationLabel() As SqlAndParameterList
            Dim displayDataSqlAndParameters As SqlAndParameterList = _displayDataList.ToSqlAndParameterForCalculationLabel()
            Dim parameterList As New List(Of Parameter)
            parameterList.AddRange(displayDataSqlAndParameters.ParameterList)
            Dim sqlString As String = displayDataSqlAndParameters.SqlString
            Dim conditionSqlAndParameterList As SqlAndParameterList = _conditionList.ToWhereSql()
            Dim formatString = If(IsDistinct, SelectDistinctSql, SelectSql)
            sqlString = String.Format(formatString, sqlString, _tableName, conditionSqlAndParameterList.SqlString)
            sqlString = AddOrder(sqlString)
            sqlString = AddLimit(sqlString)
            parameterList.AddRange(conditionSqlAndParameterList.ParameterList)
            Return New SqlAndParameterList(sqlString, parameterList)
        End Function
    End Class
End Namespace
