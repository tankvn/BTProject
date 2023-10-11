Imports Database.My.Resources
Imports System.Text

Namespace SqlData.DisplayData
    Public Class CountFieldDisplayData
        Implements IDisplayData

        Private ReadOnly _fieldName As String

        Private ReadOnly _displayName As String

        Private ReadOnly _isDistinct As Boolean

        Public Sub New(ByVal fieldName As String, ByVal displayName As String, ByVal isDistinct As Boolean)
            _fieldName = fieldName
            _displayName = displayName
            _isDistinct = isDistinct
        End Sub

        Public Sub New(ByVal fieldName As String)
            Me.New(fieldName, String.Empty, False)
        End Sub

        Public Sub New(ByVal fieldName As String, ByVal isDistinct As Boolean)
            Me.New(fieldName, String.Empty, isDistinct)
        End Sub

        Public Sub New()
            Me.New(Nothing, String.Empty, False)
        End Sub

        Friend Function ToSqlAndParameters() As SqlAndParameterList Implements IDisplayData.ToSqlAndParameters
            If IsNothing(_fieldName) Then
                Return New SqlAndParameterList(AddDisplayName(CountAllSql, _displayName), New Parameter() {})
            End If
            Dim sqlString As String = GetSqlString()
            Return New SqlAndParameterList(AddDisplayName(sqlString, _displayName), New Parameter() {})
        End Function

        Public Function ToSqlAndParametersWithoutDisplayName() As SqlAndParameterList _
            Implements IDisplayData.ToSqlAndParametersWithoutDisplayName
            If IsNothing(_fieldName) Then
                Return New SqlAndParameterList(CountAllSql, New Parameter() {})
            End If
            Dim sqlString As String = GetSqlString()
            Return New SqlAndParameterList(sqlString, New Parameter() {})
        End Function

        Private Function GetSqlString() As String
            Dim sqlString As New StringBuilder()
            sqlString.Append("COUNT(")
            If _isDistinct Then
                sqlString.Append("DISTINCT ")
            End If

            If String.IsNullOrEmpty(_fieldName) Then
                sqlString.Append("*")
            Else
                sqlString.Append(Utility.StringUtility.EncloseIdentifier(_fieldName))
            End If
            sqlString.Append(")")
            Return sqlString.ToString()
        End Function
    End Class
End Namespace
