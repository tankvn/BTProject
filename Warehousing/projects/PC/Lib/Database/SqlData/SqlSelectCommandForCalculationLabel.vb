Imports Database.SqlData.Condition
Imports Database.SqlData.DisplayData
Imports Database.My.Resources

Namespace SqlData
    Public Class SqlSelectCommandForCalculationLabel
        Implements ISqlCommand
        Private ReadOnly _displayDataList As DisplayDataList

        Public ReadOnly Property DisplayDataList() As DisplayDataList
            Get
                Return _displayDataList
            End Get
        End Property

        Private ReadOnly _selectTable As SqlAndParameterList

        Public Sub New(ByVal displayDataList As DisplayDataList, ByVal selectTable As SqlAndParameterList)
            _displayDataList = displayDataList
            _selectTable = selectTable
        End Sub

        Public Function ToSqlCommand(ByVal dbConnection As DbConnection) As DbCommand Implements ISqlCommand.ToSqlCommand
            Dim sqlString As String = _displayDataList.ToSqlAndParameter().SqlString
            Dim commandText = String.Format(SelectSqlForCalculationLabel, sqlString, _selectTable.SqlString)
            Dim command As DbCommand = Utility.CreateDbCommand(dbConnection, _displayDataList, _selectTable, commandText)
            Return command
        End Function

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
    End Class
End Namespace
