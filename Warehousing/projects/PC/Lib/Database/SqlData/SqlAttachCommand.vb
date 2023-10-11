Imports Database.SqlData.Condition
Imports Database.My.Resources
Imports Database.Utility

Namespace SqlData
    Public Class SqlAttachCommand
        Implements ISqlCommand

        Private ReadOnly _filePath As String

        Private ReadOnly _name As String

        Public ReadOnly Property ConditionList() As ConditionList Implements ISqlCommand.ConditionList
            Get
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property TableName() As String Implements ISqlCommand.TableName
            Get
                Return Nothing
            End Get
        End Property

        Public Sub New(ByVal filePath As String, ByVal name As String)
            _filePath = filePath
            _name = EncloseIdentifier(name)
        End Sub

        Public Function ToSqlCommand(ByVal connection As DbConnection) As DbCommand Implements ISqlCommand.ToSqlCommand
            Dim command As DbCommand = connection.CreateCommand()
            command.CommandText = String.Format(AttachSql, _filePath, _name)
            Return command
        End Function
    End Class
End Namespace
