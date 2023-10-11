Imports Database.SqlData.Condition
Imports Database.Utility

Namespace SqlData
    Public Class SqlCommand
        Implements ISqlCommand

        Private ReadOnly _sqlString As String

        Private ReadOnly _parameters As List(Of Parameter)

        Public ReadOnly Property TableName() As String Implements ISqlCommand.TableName
            Get
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property ConditionList() As ConditionList Implements ISqlCommand.ConditionList
            Get
                Return Nothing
            End Get
        End Property

        Public Sub New(ByVal sqlString As String, ByVal parameters As List(Of Parameter))
            _sqlString = sqlString
            _parameters = parameters
        End Sub

        Public Function ToSqlCommand(ByVal connection As DbConnection) As DbCommand Implements ISqlCommand.ToSqlCommand
            Dim command = connection.CreateCommand()
            command.Parameters.AddRange(ToDbParameterList(_parameters, command).ToArray())
            command.CommandText = _sqlString
            Return command
        End Function
    End Class
End Namespace
