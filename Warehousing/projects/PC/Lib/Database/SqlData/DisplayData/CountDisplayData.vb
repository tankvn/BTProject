Imports Database.SqlData.Condition
Imports Database.My.Resources

Namespace SqlData.DisplayData
    Public Class CountDisplayData
        Implements IDisplayData

        Private ReadOnly _conditionData As IConditionData

        Private ReadOnly _displayName As String

        Public Sub New(ByVal conditionData As IConditionData, ByVal displayName As String)
            _conditionData = conditionData
            _displayName = displayName
        End Sub

        Public Sub New(ByVal conditionData As IConditionData)
            Me.New(conditionData, String.Empty)
        End Sub

        Public Sub New()
            Me.New(Nothing, String.Empty)
        End Sub

        Friend Function ToSqlAndParameters() As SqlAndParameterList Implements IDisplayData.ToSqlAndParameters
            If IsNothing(_conditionData) Then
                Return New SqlAndParameterList(AddDisplayName(CountAllSql, _displayName), New Parameter() {})
            End If
            Dim sqlAndParameterList As SqlAndParameterList = _conditionData.ToSqlAndParameter()
            Dim sqlString = String.Format(CountSql, sqlAndParameterList.SqlString)
            Return New SqlAndParameterList(AddDisplayName(sqlString, _displayName), sqlAndParameterList.ParameterList)
        End Function

        Public Function ToSqlAndParametersWithoutDisplayName() As SqlAndParameterList _
            Implements IDisplayData.ToSqlAndParametersWithoutDisplayName
            If IsNothing(_conditionData) Then
                Return New SqlAndParameterList(CountAllSql, New Parameter() {})
            End If
            Dim sqlAndParameterList As SqlAndParameterList = _conditionData.ToSqlAndParameter()
            Dim sqlString = String.Format(CountSql, sqlAndParameterList.SqlString)
            Return New SqlAndParameterList(sqlString, sqlAndParameterList.ParameterList)
        End Function
    End Class
End Namespace

