Imports Database.SqlData.Condition.Member

Namespace SqlData.Condition
    Public Class InsertDataList
        Private ReadOnly _insertDataList As List(Of LiteralMember)

        Public Sub New()
            _insertDataList = New List(Of LiteralMember)()
        End Sub

        Public Sub Add(ByVal value As Integer)
            _insertDataList.Add(New LiteralMember(value))
        End Sub

        Public Sub AddRange(ByVal values As ICollection(Of Integer))
            For Each value As Integer In values
                Add(value)
            Next
        End Sub

        Public Sub Add(ByVal value As String)
            _insertDataList.Add(New LiteralMember(value))
        End Sub

        Public Sub AddRange(ByVal values As ICollection(Of String))
            For Each value As String In values
                Add(value)
            Next
        End Sub

        Public Sub Add(ByVal value As Double)
            _insertDataList.Add(New LiteralMember(value))
        End Sub

        Public Sub AddRange(ByVal values As ICollection(Of Double))
            For Each value As Double In values
                Add(value)
            Next
        End Sub

        Public Sub Add(ByVal value As Single)
            _insertDataList.Add(New LiteralMember(value))
        End Sub

        Public Sub AddRange(ByVal values As ICollection(Of Single))
            For Each value As Single In values
                Add(value)
            Next
        End Sub

        Public Sub Add(ByVal value As Object, ByVal type As DataType)
            _insertDataList.Add(New LiteralMember(value, DataTypeToDbType(type)))
        End Sub

        Public Sub AddRange(ByVal values As ICollection(Of Object), ByVal type As DataType)
            For Each value As Single In values
                Add(value, type)
            Next
        End Sub

        Public Sub AddRange(ByVal insertDataList As InsertDataList)
            _insertDataList.AddRange(insertDataList._insertDataList)
        End Sub

        Public Function ToSqlAndParameter() As SqlAndParameterList
            Dim sqlString As String = String.Empty
            Dim parameterList As New List(Of Parameter)()
            For Each value As IMember In _insertDataList
                If Not sqlString = String.Empty Then
                    sqlString += ","
                End If
                sqlString += value.SqlString
                parameterList.AddRange(value.Parameter)
            Next
            Return New SqlAndParameterList(sqlString, parameterList)
        End Function
    End Class
End Namespace
