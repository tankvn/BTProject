Imports Database.SqlData.Condition
Imports Database.SqlData.Condition.Member
Imports Database.My.Resources

Namespace SqlData.DisplayData
    Public Class BranchDisplayData
        Implements IDisplayData

        Private ReadOnly _conditionData As IConditionData

        Private ReadOnly _trueValue As LiteralMember

        Private ReadOnly _falseValue As LiteralMember

        Private ReadOnly _displayName As String

        Sub New(ByVal conditionData As IConditionData, ByVal trueValue As String, ByVal falseValue As String,
                ByVal displayName As String)
            _conditionData = conditionData
            _trueValue = New LiteralMember(trueValue)
            _falseValue = New LiteralMember(falseValue)
            _displayName = displayName
        End Sub

        Sub New(ByVal conditionData As IConditionData, ByVal trueValue As String, ByVal falseValue As String)
            Me.New(conditionData, trueValue, falseValue, String.Empty)
        End Sub

        Public Function ToSqlAndParameters() As SqlAndParameterList Implements IDisplayData.ToSqlAndParameters
            Return ToSqlAndParameters(True)
        End Function

        Public Function ToSqlAndParametersWithoutDisplayName() As SqlAndParameterList _
            Implements IDisplayData.ToSqlAndParametersWithoutDisplayName
            Return ToSqlAndParameters(False)
        End Function

        Private Function ToSqlAndParameters(ByVal withDisplayName As Boolean) As SqlAndParameterList
            Dim sqlAndParameterList As SqlAndParameterList = _conditionData.ToSqlAndParameter()
            Dim parameterList As New List(Of Parameter)(sqlAndParameterList.ParameterList)
            parameterList.AddRange(_trueValue.Parameter)
            parameterList.AddRange(_falseValue.Parameter)
            Dim sqlString As String = String.Format(CaseSql, sqlAndParameterList.SqlString, _trueValue.SqlString,
                                                    _falseValue.SqlString)
            If withDisplayName Then
                sqlString = AddDisplayName(sqlString, _displayName)
            End If
            Return New SqlAndParameterList(sqlString, parameterList)
        End Function
    End Class
End Namespace
