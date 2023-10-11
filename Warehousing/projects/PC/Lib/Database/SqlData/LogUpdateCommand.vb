Imports Database.SqlData.Condition
Imports Database.My.Resources
Imports Database.Utility

Namespace SqlData
    Public Class LogUpdateCommand
        Public Const TEMP_SUM_TABLE_NAME = "TempSum"

        Public Const TEMP_NEW_TABLE_NAME = "TempNew"

        Public Const LOG_STATUS_COLUMN_NAME = "Result"

        Private Const MAIN_DB_NAME = "main"

        Private ReadOnly _keyColumnList As List(Of String)

        Public ReadOnly Property KeyColumnList() As List(Of String)
            Get
                Return _keyColumnList
            End Get
        End Property

        Private ReadOnly _logKeyColumnList As List(Of String)
        Public ReadOnly Property LogKeyColumnList() As List(Of String)
            Get
                Return _logKeyColumnList
            End Get
        End Property

        Private ReadOnly _logUpdateTargetList As LogUpdateTargetList
        Public ReadOnly Property LogUpdateTargetList() As LogUpdateTargetList
            Get
                Return _logUpdateTargetList
            End Get
        End Property

        Private ReadOnly _integerColumnName As List(Of String)
        Public ReadOnly Property IntegerColumnName() As List(Of String)
            Get
                Return _integerColumnName
            End Get
        End Property


        Private ReadOnly _logTableName As String

        Private ReadOnly _masterTableName As String

        Private ReadOnly _isRetransmission As Boolean

        Private Sub New(ByVal masterTableName As String, ByVal logTableName As String)
            _masterTableName = masterTableName
            _logTableName = logTableName
            _keyColumnList = New List(Of String)()
            _logKeyColumnList = New List(Of String)()
            _logUpdateTargetList = New LogUpdateTargetList()
            _integerColumnName = New List(Of String)()
            _isRetransmission = False
        End Sub

        Sub New(ByVal masterTableName As String, ByVal logTableName As String, ByVal isRetransmission As Boolean)
            Me.New(masterTableName, logTableName)
            _isRetransmission = isRetransmission
        End Sub

        Private Function GetMasterKeyListString() As String
            Dim listString As String = String.Empty
            For Each name As String In _keyColumnList
                If Not listString = String.Empty Then
                    listString += ","
                End If
                listString += name
            Next
            Return listString
        End Function

        Private Function GetLogKeyListString() As String
            Dim listString As String = String.Empty
            For Each name As String In _logKeyColumnList
                If Not listString = String.Empty Then
                    listString += ","
                End If
                listString += name
            Next
            Return listString
        End Function

        Friend Function GetGroupSumCommand() As SqlCommand
            Dim conditionSqlAndParamters As SqlAndParameterList = GetConditionSql()
            Return New SqlCommand(String.Format(CreateGroupSumTempTable, EncloseIdentifier(TEMP_SUM_TABLE_NAME), GetLogKeyListString(),
                          _logUpdateTargetList.GetSumTargetListString(_integerColumnName), EncloseIdentifier(_logTableName), conditionSqlAndParamters.SqlString),
                          conditionSqlAndParamters.ParameterList)
        End Function

        Friend Function GetNewDataCommand() As SqlCommand
            Dim conditionSqlAndParamters As SqlAndParameterList = GetConditionSql()
            Return New SqlCommand(String.Format(CreateNewDataTempTable, EncloseIdentifier(TEMP_NEW_TABLE_NAME),
                                                GetLogKeyListString(), _logUpdateTargetList.GetNewTargetListString(_integerColumnName),
                                                EncloseIdentifier(_logTableName), conditionSqlAndParamters.SqlString),
                                  conditionSqlAndParamters.ParameterList)
        End Function

        Private Function GetConditionSql() As SqlAndParameterList
            If Not _isRetransmission Then
                Dim combineConditionData As New CombineConditionData()
                combineConditionData.OrCombining = False
                combineConditionData.ConditionList.Add(New LiteralConditionData(ConditionType.ExactMatch, LOG_STATUS_COLUMN_NAME,
                                                                                GetResultString(DbUpdateResult.Incomplete)))
                For Each target As LogUpdateTarget In LogUpdateTargetList
                    If (target.Type = UpdateType.Assignment) Then Continue For
                    combineConditionData.ConditionList.Add(New NegativeConditionData(New LiteralConditionData(ConditionType.ExactMatch, target.LogName, String.Empty)))
                Next
                Return combineConditionData.ToSqlAndParameter()
            Else
                Dim conditionString As String = String.Format(LogRetransmissionCondition,
                                                              LOG_STATUS_COLUMN_NAME,
                                                              GetResultString(DbUpdateResult.NG),
                                                              GetResultString(DbUpdateResult.Skip),
                                                              GetResultString(DbUpdateResult.Incomplete))
                Return New SqlAndParameterList(conditionString, New List(Of Parameter))
            End If
        End Function

        Private Function CreateSetStrings() As String()
            Dim stringList As New List(Of String)()
            For Each updateTarget As LogUpdateTarget In _logUpdateTargetList
                Dim setString As String = String.Empty
                Dim enclosedMasterName As String = EncloseIdentifier(updateTarget.MasterName)
                Select Case updateTarget.Type
                    Case UpdateType.Assignment
                        setString = String.Format(SetSqlForLogUpdateAssignment, enclosedMasterName,
                                                  CreateSelectString(TEMP_NEW_TABLE_NAME, updateTarget))
                    Case UpdateType.Addition
                        Dim updateTypeString = enclosedMasterName + " +"
                        setString = String.Format(SetSqlForLogUpdateCalculate, enclosedMasterName, String.Empty,
                                                  updateTypeString, CreateSelectString(TEMP_SUM_TABLE_NAME, updateTarget))
                    Case UpdateType.Subtraction
                        Dim updateTypeString = enclosedMasterName + " -"
                        setString = String.Format(SetSqlForLogUpdateCalculate, enclosedMasterName, "-",
                                                  updateTypeString, CreateSelectString(TEMP_SUM_TABLE_NAME, updateTarget))
                End Select
                stringList.Add(setString)
            Next
            Return stringList.ToArray()
        End Function


        Private Function CreateWhereStrings() As String()
            Dim stringList As New List(Of String)()
            For Each updateTarget As LogUpdateTarget In _logUpdateTargetList
                Dim tempTableName As String = String.Empty
                Select Case updateTarget.Type
                    Case UpdateType.Assignment
                        tempTableName = TEMP_NEW_TABLE_NAME
                    Case UpdateType.Addition
                        tempTableName = TEMP_SUM_TABLE_NAME
                    Case UpdateType.Subtraction
                        tempTableName = TEMP_SUM_TABLE_NAME
                End Select
                Dim existsString = String.Format(ExistsSql, CreateSelectString(tempTableName, updateTarget))
                stringList.Add(existsString)
            Next
            Return stringList.ToArray()
        End Function

        Private Function CreateSelectString(ByVal tableName As String, ByVal updateTarget As LogUpdateTarget) As Object
            Dim tempTableName = EncloseIdentifier(tableName)
            Dim whereString As String = GetWhereString(tempTableName)
            Dim selectString As String = String.Format(SelectSqlForLogUpdate,
                                                       tempTableName + "." + EncloseIdentifier(updateTarget.LogName),
                                                       tempTableName,
                                                       whereString)
            Return selectString
        End Function

        Private Function GetWhereString(ByVal tempTableName As String) As String
            Dim whereString As String = String.Empty
            For index As Integer = 0 To _keyColumnList.Count - 1
                If Not whereString = String.Empty Then
                    whereString += " AND "
                End If
                whereString += EncloseIdentifier(_masterTableName) + "." +
                    EncloseIdentifier(_keyColumnList(index)) + "=" + tempTableName + "." +
                    EncloseIdentifier(_logKeyColumnList(index))
            Next
            Return whereString
        End Function

        Friend Function GetUpdateCommand() As String
            Dim setCommand As String = String.Join(", ", CreateSetStrings())

            Dim whereCommand As String = "WHERE "
            For Each whereString As String In CreateWhereStrings()
                If Not whereCommand = "WHERE " Then
                    whereCommand += " AND "
                End If
                whereCommand += whereString
            Next
            Return String.Format(UpdateSql, EncloseIdentifier(_masterTableName), setCommand, whereCommand)
        End Function

        Friend Function GetInsertCommand() As String
            Dim insertTarget As String = GetMasterKeyListString()
            For Each logUpdateTarget As LogUpdateTarget In _logUpdateTargetList
                insertTarget += ","
                insertTarget += EncloseIdentifier(logUpdateTarget.MasterName)
            Next

            Dim mainTempTable As String
            Dim subTempTable As String = Nothing
            If _logUpdateTargetList.HasSumTarget And _logUpdateTargetList.HasAssignmentTarget Then
                mainTempTable = TEMP_SUM_TABLE_NAME
                subTempTable = TEMP_NEW_TABLE_NAME
            ElseIf _logUpdateTargetList.HasSumTarget Then
                mainTempTable = TEMP_SUM_TABLE_NAME
            ElseIf _logUpdateTargetList.HasAssignmentTarget Then
                mainTempTable = TEMP_NEW_TABLE_NAME
            Else
                Return Nothing
            End If
            mainTempTable = EncloseIdentifier(mainTempTable)
            subTempTable = EncloseIdentifier(subTempTable)

            Dim selectSql As String = GetSelectSql(mainTempTable)
            Dim fromSql As String = GetFromSql(mainTempTable, subTempTable)

            Dim whereSql As String = String.Empty
            For Each keyColumn As String In KeyColumnList
                If Not whereSql = String.Empty Then
                    whereSql += " OR "
                End If
                whereSql += String.Format(IsNullSql, EncloseIdentifier(_masterTableName) + "." + keyColumn)
            Next
            selectSql = String.Format(SelectSqlForLogUpdate, selectSql, fromSql, whereSql)

            Return String.Format(InsertSelectSql, EncloseIdentifier(MAIN_DB_NAME) + "." + EncloseIdentifier(_masterTableName), insertTarget, selectSql)
        End Function

        Private Function GetSelectSql(ByVal mainTempTable As String) As String

            Dim selectSql As String = String.Empty
            For Each logKeyColumn As String In _logKeyColumnList
                If Not selectSql = String.Empty Then
                    selectSql += ","
                End If
                selectSql += mainTempTable + "." + EncloseIdentifier(logKeyColumn)
            Next
            For Each logUpdateTarget As LogUpdateTarget In _logUpdateTargetList
                selectSql += ","
                If logUpdateTarget.Type = UpdateType.Assignment Then
                    selectSql += EncloseIdentifier(TEMP_NEW_TABLE_NAME) + "."
                ElseIf logUpdateTarget.Type = UpdateType.Addition Then
                    selectSql += EncloseIdentifier(TEMP_SUM_TABLE_NAME) + "."
                ElseIf logUpdateTarget.Type = UpdateType.Subtraction Then
                    selectSql += "-" + EncloseIdentifier(TEMP_SUM_TABLE_NAME) + "."
                End If
                selectSql += EncloseIdentifier(logUpdateTarget.LogName)
            Next
            Return selectSql
        End Function

        Private Function GetFromSql(ByVal mainTempTable As String, ByVal subTempTable As String) As String

            Dim fromSql As String = mainTempTable
            If Not IsNothing(subTempTable) Then
                fromSql += " INNER JOIN " + subTempTable + " ON"
                For index As Integer = 0 To _logKeyColumnList.Count - 1
                    If Not index = 0 Then
                        fromSql += " AND "
                    End If
                    fromSql += " " + mainTempTable + "." + _logKeyColumnList(index) + "=" + subTempTable + "." +
                               _logKeyColumnList(index)
                Next
            End If
            fromSql += " LEFT OUTER JOIN " + EncloseIdentifier(_masterTableName) + " ON"
            For index As Integer = 0 To _keyColumnList.Count - 1
                If Not index = 0 Then
                    fromSql += " AND "
                End If
                fromSql += " " + mainTempTable + "." + _logKeyColumnList(index) + "=" +
                           EncloseIdentifier(_masterTableName) + "." + _keyColumnList(index)
            Next
            Return fromSql
        End Function
    End Class
End Namespace
