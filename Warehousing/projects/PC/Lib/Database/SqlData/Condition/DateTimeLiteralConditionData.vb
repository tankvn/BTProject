Imports Database.SqlData.Condition.Member

Namespace SqlData.Condition
    Public Class DateTimeLiteralConditionData
        Implements IConditionData

        Private ReadOnly _conditionType As ConditionType
        Protected ReadOnly Property ConditionType() As ConditionType
            Get
                Return _conditionType
            End Get
        End Property

        Private ReadOnly _left As DateTimeFieldMember

        Private ReadOnly _right As DateTimeLiteralMember
        Friend ReadOnly Property Right() As IMember
            Get
                Return _right
            End Get
        End Property

        Private Sub New(ByVal conditionType As ConditionType, ByVal left As DateTimeFieldMember,
                        ByVal right As DateTimeLiteralMember)
            _conditionType = conditionType
            _left = left
            _right = right
        End Sub

        Public Sub New(ByVal conditionType As ConditionType, ByVal left As String, ByVal right As String)
            Me.new(conditionType, New DateTimeFieldMember(left), New DateTimeLiteralMember(right))
        End Sub

        Public Sub New(ByVal conditionType As ConditionType, ByVal left As String, ByVal right As String,
                       ByVal calculationType As CalculationType, ByVal calculationValue As Integer, ByVal dateUnit As DateUnit)
            Me.New(conditionType, New DateTimeFieldMember(left),
                   New DateTimeLiteralMember(right, calculationType, calculationValue, dateUnit))
        End Sub

        Friend Overridable Function ToSqlAndParameter() As SqlAndParameterList _
            Implements IConditionData.ToSqlAndParameter
            Return GetConditionSqlAndParameterList(_conditionType, _left, _right)
        End Function
    End Class
End Namespace
