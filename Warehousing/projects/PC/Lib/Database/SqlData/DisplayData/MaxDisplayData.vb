Imports Database.My.Resources
Imports Database.SqlData.Condition.Member

Namespace SqlData.DisplayData
    Public Class MaxDisplayData
        Implements IDisplayData

        Private ReadOnly _field As IMember

        Private ReadOnly _displayName As String

        Public Sub New(ByVal fieldName As String, ByVal displayName As String)
            _field = New FieldMember(fieldName)
            _displayName = displayName
        End Sub

        Public Sub New(ByVal field As String)
            Me.New(field, String.Empty)
        End Sub

        Public Sub New(ByVal displayData As IDisplayData, ByVal displayName As String)
            _field = New DisplayDataMember(displayData)
            _displayName = displayName
        End Sub

        Public Sub New(ByVal displayData As IDisplayData)
            Me.New(displayData, String.Empty)
        End Sub

        Friend Function ToSqlAndParameters() As SqlAndParameterList Implements IDisplayData.ToSqlAndParameters
            Dim sqlAndParameterList As SqlAndParameterList = _field.ToSqlAndParameter()
            Dim sqlString As String = AddDisplayName(String.Format(MaxSql, sqlAndParameterList.SqlString), _displayName)
            Return New SqlAndParameterList(sqlString, sqlAndParameterList.ParameterList)
        End Function

        Public Function ToSqlAndParametersWithoutDisplayName() As SqlAndParameterList Implements IDisplayData.ToSqlAndParametersWithoutDisplayName
            Dim sqlAndParameterList As SqlAndParameterList = _field.ToSqlAndParameter()
            Dim sqlString As String = String.Format(MaxSql, sqlAndParameterList.SqlString)
            Return New SqlAndParameterList(sqlString, sqlAndParameterList.ParameterList)
        End Function
    End Class
End Namespace
