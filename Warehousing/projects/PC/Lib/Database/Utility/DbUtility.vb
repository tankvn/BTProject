Imports Database.SqlData.Condition
Imports Database.SqlData
Imports Database.My.Resources
Imports Database.SqlData.DisplayData

Namespace Utility
    Friend Module DbUtility
        Private Function CreateSQLiteConnectionString(ByVal path As String, ByVal journalMode As JournalMode) As String
            Return String.Format(ConnectionString, path.Replace("\\", "\\\"), [Enum].GetName(journalMode.GetType(), journalMode))
        End Function

        Friend Function CreateSQLiteConnection(ByVal dbProviderFactory As DbProviderFactory,
                                               ByVal path As String, ByVal journalMode As JournalMode) As DbConnection
            Dim dbConnection As DbConnection = dbProviderFactory.CreateConnection()
            dbConnection.ConnectionString = CreateSQLiteConnectionString(path, journalMode)
            Return dbConnection
        End Function

        Friend Function EncloseInParentheses(ByVal target As String)
            Return "(" + target + ")"
        End Function

        Friend Function ToDbParameterList(ByVal parameterList As ICollection(Of Parameter),
                                          ByVal command As DbCommand) As List(Of DbParameter)
            Dim dbParameter As New List(Of DbParameter)
            For Each parameter As Parameter In parameterList
                dbParameter.Add(parameter.ToDbParameter(command))
            Next
            Return dbParameter
        End Function

        Friend Function CreateDbCommand(ByVal connection As DbConnection, ByVal displayDataList As DisplayDataList,
                                        ByVal sqlPrameterList As SqlAndParameterList, ByVal commandText As String) As DbCommand
            Dim command = connection.CreateCommand()
            Dim displayDataSqlAndParameters As SqlAndParameterList = displayDataList.ToSqlAndParameter()
            command.Parameters.AddRange(ToDbParameterList(displayDataSqlAndParameters.ParameterList, command).ToArray())
            command.CommandText = commandText
            command.Parameters.AddRange(ToDbParameterList(sqlPrameterList.ParameterList, command).ToArray())
            command.Prepare()
            Return command
        End Function

        Friend Function DbTypeToComparisonType(ByVal type As DbType) As ComparisonType
            Select Case type
                Case DbType.Int32, DbType.Int64, DbType.Single, DbType.Double
                    Return ComparisonType.Numeric
                Case DbType.String, DbType.AnsiString
                    Return ComparisonType.Text
                Case DbType.Date, DbType.DateTime, DbType.DateTime2
                    Return ComparisonType.DateCompare
            End Select
            Return ComparisonType.Text
        End Function
    End Module
End Namespace
