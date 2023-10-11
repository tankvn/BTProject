Imports Database.SqlData.Condition.Member
Imports Database.My.Resources

Namespace SqlData.DisplayData
    Public Class SumDisplayData
        Implements IDisplayData

        Private ReadOnly _field As IMember

        Private ReadOnly _displayName As String

        Public Sub New(ByVal fieldName As String, ByVal displayName As String)
            _field = New FieldMember(fieldName)
            _displayName = displayName
        End Sub

        Public Sub New(ByVal fieldName As String)
            Me.New(fieldName, String.Empty)
        End Sub

        Public Sub New(ByVal displayData As IDisplayData, ByVal displayName As String)
            _field = New DisplayDataMember(displayData)
            _displayName = displayName
        End Sub

        Public Sub New(ByVal displayData As IDisplayData)
            Me.New(displayData, String.Empty)
        End Sub

        Friend Function ToSqlAndParameters() As SqlAndParameterList _
            Implements IDisplayData.ToSqlAndParameters
            Dim sqlAndParameterList As SqlAndParameterList = _field.ToSqlAndParameter()
            Dim sqlString As String = String.Format(SumSql, sqlAndParameterList.SqlString)
            Return _
                New SqlAndParameterList(AddDisplayName(sqlString, _displayName), sqlAndParameterList.ParameterList)
        End Function

        Public Function ToSqlAndParametersWithoutDisplayName() As SqlAndParameterList _
            Implements IDisplayData.ToSqlAndParametersWithoutDisplayName
            Dim sqlAndParameterList As SqlAndParameterList = _field.ToSqlAndParameter()
            Dim sqlString As String = String.Format(SumSql, sqlAndParameterList.SqlString)
            Return _
                New SqlAndParameterList(sqlString, sqlAndParameterList.ParameterList)
        End Function
    End Class
End Namespace
