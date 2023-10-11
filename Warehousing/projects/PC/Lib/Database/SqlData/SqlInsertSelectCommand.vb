Imports Database.My.Resources
Imports Database.Utility
Imports System.Text

Namespace SqlData
    Public Class SqlInsertSelectCommand
        Implements ISqlCommand

        Private ReadOnly _insertFieldList As List(Of String)
        Public ReadOnly Property InsertFieldList() As List(Of String)
            Get
                Return _insertFieldList
            End Get
        End Property

        Private ReadOnly _tableName As String
        Public ReadOnly Property TableName() As String Implements ISqlCommand.TableName
            Get
                Return _tableName
            End Get
        End Property

        Private ReadOnly _selectCommand As SqlSelectCommand

        Private Sub New(ByVal selectCommand As SqlSelectCommand)
            _selectCommand = selectCommand
            _insertFieldList = New List(Of String)()
        End Sub

        Sub New(ByVal tableName As String, ByVal selectCommand As SqlSelectCommand)
            Me.new(selectCommand)
            _tableName = EncloseIdentifier(tableName)
        End Sub

        Sub New(ByVal schemaName As String, ByVal tableName As String, ByVal selectCommand As SqlSelectCommand)
            Me.new(selectCommand)
            _tableName = EncloseIdentifier(schemaName) + "." + EncloseIdentifier(tableName)
        End Sub

        Public ReadOnly Property ConditionList() As Condition.ConditionList Implements ISqlCommand.ConditionList
            Get
                Return Nothing
            End Get
        End Property

        Public Function ToSqlCommand(ByVal connection As DbConnection) As DbCommand Implements ISqlCommand.ToSqlCommand

            Dim command = connection.CreateCommand()
            Dim sqlAndParameters As SqlAndParameterList = _selectCommand.ToSqlAndParametersForCalculationLabel()
            command.CommandText = String.Format(InsertSelectSql, _tableName, CreateInsertFieldString(), sqlAndParameters.SqlString)
            command.Parameters.AddRange(ToDbParameterList(sqlAndParameters.ParameterList, command).ToArray())
            command.Prepare()
            Return command
        End Function

        Private Function CreateInsertFieldString() As String
            If _insertFieldList.Count = 0 Then Return String.Empty

            Dim sqlString As New StringBuilder()
            For Each field As String In _insertFieldList
                If Not sqlString.ToString().Equals(String.Empty) Then
                    sqlString.Append(",")
                End If
                sqlString.Append(EncloseIdentifier(field))
            Next
            Return sqlString.ToString()
        End Function
    End Class
End Namespace

