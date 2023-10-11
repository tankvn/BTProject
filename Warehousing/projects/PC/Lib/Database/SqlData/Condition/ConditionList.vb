Imports Database.My.Resources
Imports Database.SqlData.Condition.Member

Namespace SqlData.Condition
    Public Class ConditionList
        Private ReadOnly _conditionList As List(Of IConditionData) = New List(Of IConditionData)()

        Public ReadOnly Property HasCondition() As Boolean
            Get
                Return 0 < _conditionList.Count
            End Get
        End Property


        Public Sub AddLiteral(ByVal type As ConditionType, ByVal left As String, ByVal right As String)
            _conditionList.Add(New LiteralConditionData(type, left, right))
        End Sub

        Public Sub AddLiteral(ByVal type As ConditionType, ByVal left As String, ByVal right As Integer)
            _conditionList.Add(New LiteralConditionData(type, left, right))
        End Sub

        Public Sub AddLiteral(ByVal type As ConditionType, ByVal left As String, ByVal right As Double)
            _conditionList.Add(New LiteralConditionData(type, left, right))
        End Sub

        Public Sub AddLiteral(ByVal type As ConditionType, ByVal left As String, ByVal right As Single)
            _conditionList.Add(New LiteralConditionData(type, left, right))
        End Sub

        Public Sub AddLiteral(ByVal type As ConditionType, ByVal left As String, ByVal right As Object, ByVal dataType As DataType)
            _conditionList.Add(New LiteralConditionData(type, left, right, DataTypeToDbType(dataType)))
        End Sub

        Public Sub AddLiteralNegative(ByVal type As ConditionType, ByVal left As String, ByVal right As String)
            _conditionList.Add(New NegativeConditionData(New LiteralConditionData(type, left, right)))
        End Sub

        Public Sub AddLiteralNegative(ByVal type As ConditionType, ByVal left As String, ByVal right As Integer)
            _conditionList.Add(New NegativeConditionData(New LiteralConditionData(type, left, right)))
        End Sub

        Public Sub AddLiteralNegative(ByVal type As ConditionType, ByVal left As String, ByVal right As Double)
            _conditionList.Add(New NegativeConditionData(New LiteralConditionData(type, left, right)))
        End Sub

        Public Sub AddLiteralNegative(ByVal type As ConditionType, ByVal left As String, ByVal right As Single)
            _conditionList.Add(New NegativeConditionData(New LiteralConditionData(type, left, right)))
        End Sub

        Public Sub AddLiteralNegative(ByVal type As ConditionType, ByVal left As String,
                                      ByVal right As Object, ByVal dataType As DataType)
            _conditionList.Add(New NegativeConditionData(New LiteralConditionData(type, left, right, DataTypeToDbType(dataType))))
        End Sub

        Public Sub AddDateTimeLiteral(ByVal type As ConditionType, ByVal left As String, ByVal right As String)
            _conditionList.Add(New DateTimeConditionData(type, New DateTimeFieldMember(left), New DateTimeLiteralMember(right)))
        End Sub

        Public Sub AddField(ByVal type As ConditionType, ByVal left As String, ByVal right As String, ByVal comparisonType As ComparisonType)
            _conditionList.Add(New FieldConditionData(type, left, right, comparisonType))
        End Sub

        Public Sub AddFieldNegative(ByVal type As ConditionType, ByVal left As String, ByVal right As String, ByVal comparisonType As ComparisonType)
            _conditionList.Add(New NegativeConditionData(New FieldConditionData(type, left, right, comparisonType)))
        End Sub

        Public Sub AddDateTimeField(ByVal type As ConditionType, ByVal left As String, ByVal right As String)
            _conditionList.Add(New DateTimeConditionData(type, New DateTimeFieldMember(left), New DateTimeFieldMember(right)))
        End Sub

        Public Sub Add(ByVal condition As IConditionData)
            _conditionList.Add(condition)
        End Sub

        Public Sub AddRange(ByVal conditionList As ConditionList)
            _conditionList.AddRange(conditionList._conditionList)
        End Sub

        Public Sub AddRange(ByVal conditionList As ICollection(Of IConditionData))
            _conditionList.AddRange(conditionList)
        End Sub

        Public Sub AddLiteral(ByVal type As ConditionType, ByVal left As String(), ByVal right As String)
            _conditionList.Add(New MultipleColumnLiteralConditionData(type, left, right))
        End Sub

        Public Sub AddLiteral(ByVal type As ConditionType, ByVal left As String(), ByVal right As Integer)
            _conditionList.Add(New MultipleColumnLiteralConditionData(type, left, right))
        End Sub

        Public Sub AddLiteral(ByVal type As ConditionType, ByVal left As String(), ByVal right As Double)
            _conditionList.Add(New MultipleColumnLiteralConditionData(type, left, right))
        End Sub

        Public Sub AddLiteral(ByVal type As ConditionType, ByVal left As String(), ByVal right As Single)
            _conditionList.Add(New MultipleColumnLiteralConditionData(type, left, right))
        End Sub

        Public Sub AddIsNull(ByVal fieldName As String)
            _conditionList.Add(New IsNullConditionData(fieldName))
        End Sub

        Public Sub AddCalculationFieldCondition(ByVal conditionType As ConditionType, ByVal left As String,
                                                ByVal calculationType As CalculationType, ByVal rightSource As String,
                                                ByVal rightCalculationValue As Integer)
            _conditionList.Add(New FieldConditionData(conditionType, left, calculationType, rightSource, rightCalculationValue))
        End Sub

        Public Sub AddCalculationFieldCondition(ByVal conditionType As ConditionType, ByVal left As String,
                                                ByVal calculationType As CalculationType, ByVal rightSource As String,
                                                ByVal rightCalculationValue As Single)
            _conditionList.Add(New FieldConditionData(conditionType, left, calculationType, rightSource, rightCalculationValue))
        End Sub

        Public Sub AddCalculationFieldCondition(ByVal conditionType As ConditionType, ByVal left As String,
                                                ByVal calculationType As CalculationType, ByVal rightSource As String,
                                                ByVal rightCalculationValue As Double)
            _conditionList.Add(New FieldConditionData(conditionType, left, calculationType, rightSource, rightCalculationValue))
        End Sub

        Public Sub AddCalculationLiteralCondition(ByVal conditionType As ConditionType, ByVal left As String,
                                                  ByVal calculationType As CalculationType, ByVal rightSource As Integer,
                                                  ByVal rightCalculationValue As Integer)
            _conditionList.Add(New LiteralConditionData(conditionType, left, calculationType, rightSource, rightCalculationValue))
        End Sub

        Public Sub AddCalculationLiteralCondition(ByVal conditionType As ConditionType, ByVal left As String,
                                                  ByVal calculationType As CalculationType, ByVal rightSource As Integer,
                                                  ByVal rightCalculationValue As Single)
            _conditionList.Add(New LiteralConditionData(conditionType, left, calculationType, rightSource, rightCalculationValue))
        End Sub

        Public Sub AddCalculationLiteralCondition(ByVal conditionType As ConditionType, ByVal left As String,
                                                  ByVal calculationType As CalculationType, ByVal rightSource As Integer,
                                                  ByVal rightCalculationValue As Double)
            _conditionList.Add(New LiteralConditionData(conditionType, left, calculationType, rightSource, rightCalculationValue))
        End Sub

        Public Sub AddCalculationLiteralCondition(ByVal conditionType As ConditionType, ByVal left As String,
                                                  ByVal calculationType As CalculationType, ByVal rightSource As Single,
                                                  ByVal rightCalculationValue As Integer)
            _conditionList.Add(New LiteralConditionData(conditionType, left, calculationType, rightSource, rightCalculationValue))
        End Sub

        Public Sub AddCalculationLiteralCondition(ByVal conditionType As ConditionType, ByVal left As String,
                                                  ByVal calculationType As CalculationType, ByVal rightSource As Single,
                                                  ByVal rightCalculationValue As Single)
            _conditionList.Add(New LiteralConditionData(conditionType, left, calculationType, rightSource, rightCalculationValue))
        End Sub

        Public Sub AddCalculationLiteralCondition(ByVal conditionType As ConditionType, ByVal left As String,
                                                  ByVal calculationType As CalculationType, ByVal rightSource As Single,
                                                  ByVal rightCalculationValue As Double)
            _conditionList.Add(New LiteralConditionData(conditionType, left, calculationType, rightSource, rightCalculationValue))
        End Sub

        Public Sub AddCalculationLiteralCondition(ByVal conditionType As ConditionType, ByVal left As String,
                                                  ByVal calculationType As CalculationType, ByVal rightSource As Double,
                                                  ByVal rightCalculationValue As Integer)
            _conditionList.Add(New LiteralConditionData(conditionType, left, calculationType, rightSource, rightCalculationValue))
        End Sub

        Public Sub AddCalculationLiteralCondition(ByVal conditionType As ConditionType, ByVal left As String,
                                                  ByVal calculationType As CalculationType, ByVal rightSource As Double,
                                                  ByVal rightCalculationValue As Single)
            _conditionList.Add(New LiteralConditionData(conditionType, left, calculationType, rightSource, rightCalculationValue))
        End Sub

        Public Sub AddCalculationLiteralCondition(ByVal conditionType As ConditionType, ByVal left As String,
                                                  ByVal calculationType As CalculationType, ByVal rightSource As Double,
                                                  ByVal rightCalculationValue As Double)
            _conditionList.Add(New LiteralConditionData(conditionType, left, calculationType, rightSource, rightCalculationValue))
        End Sub

        Friend Function ToWhereSql() As SqlAndParameterList
            If _conditionList.Count = 0 Then Return New SqlAndParameterList()
            Dim whereString = String.Empty
            Dim parameterList As New List(Of Parameter)()

            For Each condition As IConditionData In _conditionList
                Dim sqlAndParameterList As SqlAndParameterList = condition.ToSqlAndParameter()
                If sqlAndParameterList.SqlString = String.Empty Then Continue For
                If Not whereString = String.Empty Then
                    whereString += " AND "
                End If
                whereString += sqlAndParameterList.SqlString
                parameterList.AddRange(sqlAndParameterList.ParameterList)
            Next
            If whereString = String.Empty Then Return New SqlAndParameterList()
            Return New SqlAndParameterList(String.Format(WhereSql, whereString), parameterList)
        End Function
    End Class
End Namespace
