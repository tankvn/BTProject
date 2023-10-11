Imports Database.SqlData.Condition.Member

Namespace SqlData.Condition
    Public Class UpdateDataList
        Private ReadOnly _updateDataList As New List(Of UpdateData)

        Private Sub AddLiteral(ByVal type As UpdateType, ByVal target As FieldMember, ByVal value As LiteralMember)
            _updateDataList.Add(New UpdateData(type, target, value))
        End Sub

        Private Sub AddField(ByVal type As UpdateType, ByVal target As FieldMember, ByVal value As FieldMember)
            _updateDataList.Add(New UpdateData(type, target, value))
        End Sub

        Public Sub AddLiteral(ByVal type As UpdateType, ByVal target As String, ByVal value As Integer)
            AddLiteral(type, New FieldMember(target), New LiteralMember(value))
        End Sub

        Public Sub AddRangeLiteral(ByVal type As UpdateType, ByVal targets As ICollection(Of String),
ByVal values As ICollection(Of Integer))
            If Not targets.Count = values.Count Then Throw New ArgumentException("targets count != values count")
            For index As Integer = 0 To targets.Count - 1
                AddLiteral(type, targets(index), values(index))
            Next
        End Sub

        Public Sub AddLiteral(ByVal type As UpdateType, ByVal target As String, ByVal value As String)
            AddLiteral(type, New FieldMember(target), New LiteralMember(value))
        End Sub

        Public Sub AddRangeLiteral(ByVal type As UpdateType, ByVal targets As ICollection(Of String),
ByVal values As ICollection(Of String))
            If Not targets.Count = values.Count Then Throw New ArgumentException("targets count != values count")
            For index As Integer = 0 To targets.Count - 1
                AddLiteral(type, targets(index), values(index))
            Next
        End Sub

        Public Sub AddLiteral(ByVal type As UpdateType, ByVal target As String, ByVal value As Double)
            AddLiteral(type, New FieldMember(target), New LiteralMember(value))
        End Sub

        Public Sub AddRangeLiteral(ByVal type As UpdateType, ByVal targets As ICollection(Of String),
ByVal values As ICollection(Of Double))
            If Not targets.Count = values.Count Then Throw New ArgumentException("targets count != values count")
            For index As Integer = 0 To targets.Count - 1
                AddLiteral(type, targets(index), values(index))
            Next
        End Sub

        Public Sub AddLiteral(ByVal type As UpdateType, ByVal target As String, ByVal value As Single)
            AddLiteral(type, New FieldMember(target), New LiteralMember(value))
        End Sub

        Public Sub AddRangeLiteral(ByVal type As UpdateType, ByVal targets As ICollection(Of String),
ByVal values As ICollection(Of Single))
            If Not targets.Count = values.Count Then Throw New ArgumentException("targets count != values count")
            For index As Integer = 0 To targets.Count - 1
                AddLiteral(type, targets(index), values(index))
            Next
        End Sub

        Public Sub AddLiteral(ByVal updateType As UpdateType, ByVal target As String, ByVal value As Object,
ByVal dataType As DataType)
            AddLiteral(updateType, New FieldMember(target), New LiteralMember(value, DataTypeToDbType(dataType)))
        End Sub

        Public Sub AddRangeLiteral(ByVal updateType As UpdateType, ByVal targets As ICollection(Of String),
ByVal values As ICollection(Of Object), ByVal dataType As DataType)
            If Not targets.Count = values.Count Then Throw New ArgumentException("targets count != values count")
            For index As Integer = 0 To targets.Count - 1
                AddLiteral(updateType, targets(index), values(index), dataType)
            Next
        End Sub

        Public Sub AddRange(ByVal updateDataList As UpdateDataList)
            _updateDataList.AddRange(updateDataList._updateDataList)
        End Sub

        Public Sub AddField(ByVal type As UpdateType, ByVal target As String, ByVal value As String)
            AddField(type, New FieldMember(target), New FieldMember(value))
        End Sub

        Public Sub AddRangeField(ByVal type As UpdateType, ByVal targets As ICollection(Of String),
ByVal values As ICollection(Of String))
            If Not targets.Count = values.Count Then Throw New ArgumentException("targets count != values count")
            For index As Integer = 0 To targets.Count - 1
                AddField(type, targets(index), values(index))
            Next
        End Sub

        Public Function ToSqlAndParameter() As SqlAndParameterList
            Dim sqlString As String = String.Empty
            Dim parameterList As New List(Of Parameter)()
            For Each updateData As UpdateData In _updateDataList

                If Not String.IsNullOrEmpty(sqlString) Then
                    sqlString += ","
                End If
                Dim sqlAndParameterList As SqlAndParameterList = updateData.ToSqlAndParameter()
                sqlString += sqlAndParameterList.SqlString
                parameterList.AddRange(sqlAndParameterList.ParameterList)
            Next
            Return New SqlAndParameterList(sqlString, parameterList)
        End Function
    End Class
End Namespace
