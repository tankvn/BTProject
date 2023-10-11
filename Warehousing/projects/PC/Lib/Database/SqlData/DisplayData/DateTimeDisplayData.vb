Imports Database.SqlData.Condition.Member
Imports Database.My.Resources

Namespace SqlData.DisplayData
    Public Class DateTimeDisplayData
        Implements IDisplayData

        Private ReadOnly _fieldName As String

        Private ReadOnly _displayName As String

        Private ReadOnly _format As LiteralMember

        Private ReadOnly _modifierList As New List(Of LiteralMember)

        Sub New(ByVal fieldName As String, ByVal format As String, ByVal displayName As String)
            _fieldName = fieldName
            _displayName = displayName
            _format = New LiteralMember(format)
        End Sub

        Sub New(ByVal fieldName As String, ByVal format As String)
            Me.new(fieldName, format, String.Empty)
        End Sub

        Sub New(ByVal fieldName As String, ByVal format As String, ByVal displayName As String, ByVal modifier As String)
            Me.new(fieldName, format, displayName)
            AddModifier(modifier)
        End Sub

        Sub New(ByVal fieldName As String, ByVal format As String, ByVal displayName As String, _
                ByVal modifiers As ICollection(Of String))
            Me.new(fieldName, format, displayName)
            AddRangeModifier(modifiers)
        End Sub

        Public Sub AddModifier(ByVal modifier As String)
            _modifierList.Add(New LiteralMember(modifier))
        End Sub

        Public Sub AddRangeModifier(ByVal modifiers As ICollection(Of String))
            For Each modifier As String In modifiers
                AddModifier(modifier)
            Next
        End Sub

        Public Function ToSqlAndParameters() As SqlAndParameterList Implements IDisplayData.ToSqlAndParameters
            Return ToSqlAndParameters(True)
        End Function

        Public Function ToSqlAndParametersWithoutDisplayName() As SqlAndParameterList _
            Implements IDisplayData.ToSqlAndParametersWithoutDisplayName
            Return ToSqlAndParameters(False)
        End Function

        Private Function ToSqlAndParameters(ByVal withDisplayName As Boolean) As SqlAndParameterList
            Dim parameterList As New List(Of Parameter)
            parameterList.AddRange(_format.Parameter)
            Dim modifierString As String = String.Empty
            For Each modifier As LiteralMember In _modifierList
                modifierString += "," + modifier.SqlString
                parameterList.Add(modifier.Parameter)
            Next
            Dim sqlString As String = String.Format(strftimeSql, _format.SqlString, _fieldName, modifierString)
            If withDisplayName Then
                sqlString = AddDisplayName(sqlString, _displayName)
            End If
            Return New SqlAndParameterList(sqlString, parameterList)
        End Function
    End Class
End Namespace
