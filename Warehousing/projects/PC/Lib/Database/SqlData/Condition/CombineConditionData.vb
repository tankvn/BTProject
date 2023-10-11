Namespace SqlData.Condition
    Public Class CombineConditionData
        Implements IConditionData
        Private ReadOnly _conditionList As New List(Of IConditionData)
        Public ReadOnly Property ConditionList() As List(Of IConditionData)
            Get
                Return _conditionList
            End Get
        End Property

        Private _orCombining As Boolean = True
        Public Property OrCombining() As Boolean
            Get
                Return _orCombining
            End Get
            Set(ByVal value As Boolean)
                _orCombining = value
            End Set
        End Property

        Public Sub AddIsNull(ByVal fieldName As String, ByVal isNegative As Boolean)
            If isNegative Then
                _conditionList.Add(New NegativeConditionData(New IsNullConditionData(fieldName)))
                Return
            End If
            _conditionList.Add(New IsNullConditionData(fieldName))
        End Sub

        Public Sub AddIsNull(ByVal fieldName As String)
            AddIsNull(fieldName, False)
        End Sub

        Public Function ToSqlAndParameter() As SqlAndParameterList Implements IConditionData.ToSqlAndParameter
            If _conditionList.Count = 0 Then Return New SqlAndParameterList()
            Dim whereString = String.Empty
            Dim parameterList As New List(Of Parameter)()

            For Each condition As IConditionData In _conditionList
                If Not whereString = String.Empty Then
                    whereString += If(_orCombining, " OR ", " AND ")
                End If
                Dim sqlAndParameterList As SqlAndParameterList = condition.ToSqlAndParameter()
                whereString += sqlAndParameterList.SqlString
                parameterList.AddRange(sqlAndParameterList.ParameterList)
            Next
            If Not String.IsNullOrEmpty(whereString) Then
                whereString = Utility.EncloseInParentheses(whereString)
            End If
            Return New SqlAndParameterList(whereString, parameterList)
        End Function
    End Class
End Namespace
