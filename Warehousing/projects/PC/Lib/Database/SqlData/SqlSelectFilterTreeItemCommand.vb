Imports Database.SqlData.Condition
Imports Database.My.Resources
Imports Database.Utility
Imports Database.SqlData.DisplayData

Namespace SqlData
    Public Class SqlSelectFilterTreeItemCommand
        Implements ISqlCommand

        Private ReadOnly _targetField As String = String.Empty

        Private ReadOnly _conditionList As New ConditionList()

        Private ReadOnly _displayDataList As DisplayDataList

        Public ReadOnly Property ConditionList As ConditionList Implements ISqlCommand.ConditionList
            Get
                Return _conditionList
            End Get
        End Property

        Private ReadOnly _tableName As String
        Public ReadOnly Property TableName As String Implements ISqlCommand.TableName
            Get
                Return _tableName
            End Get
        End Property

        Public Sub New(ByVal tableName As String, ByVal field As String, ByVal displayDataList As DisplayDataList, ByVal condition As IConditionData)
            _tableName = tableName
            _targetField = EncloseIdentifier(field)
            _displayDataList = displayDataList
            If Not IsNothing(condition) Then
                _conditionList.Add(condition)
            End If
        End Sub

        Public Function ToSqlCommand(connection As DbConnection) As DbCommand Implements ISqlCommand.ToSqlCommand
            Dim sqlSelectCommand As New SqlSelectCommand(_tableName)
            If Not IsNothing(_displayDataList) Then
                sqlSelectCommand.DisplayDataList.AddRange(_displayDataList)
            End If
            If Not IsNothing(_conditionList) Then
                sqlSelectCommand.ConditionList.AddRange(_conditionList)
            End If
            Dim selectCommandAndParameter = sqlSelectCommand.ToSqlAndParametersForCalculationLabel()

            Dim command As DbCommand = connection.CreateCommand()
            command.CommandText = String.Format(SelectFilterTreeItem, EncloseInParentheses(selectCommandAndParameter.SqlString), _targetField)
            command.Parameters.AddRange(ToDbParameterList(selectCommandAndParameter.ParameterList, command).ToArray())
            Return command
        End Function
    End Class
End Namespace
