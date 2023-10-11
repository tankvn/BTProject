Imports Database.SqlData.Condition.Member
Imports Database.Utility

Namespace SqlData.Condition
    Public Class MultipleColumnLiteralConditionData
        Inherits LiteralConditionData

        Private ReadOnly _targets As String()

        Private ReadOnly _isNegative As Boolean

        Sub New(ByVal conditionType As ConditionType, ByVal targets As String(), ByVal right As String)
            MyBase.New(conditionType, String.Empty, right)
            _targets = targets
        End Sub

        Sub New(ByVal conditionType As ConditionType, ByVal targets As String(), ByVal right As Integer)
            MyBase.New(conditionType, String.Empty, right)
            _targets = targets
        End Sub

        Sub New(ByVal conditionType As ConditionType, ByVal targets As String(), ByVal right As Double)
            MyBase.New(conditionType, String.Empty, right)
            _targets = targets
        End Sub

        Sub New(ByVal conditionType As ConditionType, ByVal targets As String(), ByVal right As Single)
            MyBase.New(conditionType, String.Empty, right)
            _targets = targets
        End Sub

        Sub New(ByVal conditionType As ConditionType, ByVal targets As String(), ByVal right As Object, ByVal type As DbType)
            MyBase.New(conditionType, String.Empty, right, type)
            _targets = targets
        End Sub

        Sub New(ByVal conditionType As ConditionType, ByVal targets As String(), ByVal right As String, ByVal isNegative As Boolean)
            Me.New(conditionType, targets, right)
            _isNegative = isNegative
        End Sub

        Sub New(ByVal conditionType As ConditionType, ByVal targets As String(), ByVal right As Integer, ByVal isNegative As Boolean)
            Me.New(conditionType, targets, right)
            _isNegative = isNegative
        End Sub

        Sub New(ByVal conditionType As ConditionType, ByVal targets As String(), ByVal right As Double, ByVal isNegative As Boolean)
            Me.New(conditionType, targets, right)
            _isNegative = isNegative
        End Sub

        Sub New(ByVal conditionType As ConditionType, ByVal targets As String(), ByVal right As Single, ByVal isNegative As Boolean)
            Me.New(conditionType, targets, right)
            _isNegative = isNegative
        End Sub

        Sub New(ByVal conditionType As ConditionType, ByVal targets As String(), ByVal right As Object, ByVal type As DbType, ByVal isNegative As Boolean)
            Me.New(conditionType, targets, right, type)
            _isNegative = isNegative
        End Sub

        Friend Overrides Function ToSqlAndParameter() As SqlAndParameterList
            Dim parameterList = New List(Of Parameter)()
            Dim sqlString As String = "("
            For Each field As String In _targets
                If Not sqlString = "(" Then
                    If _isNegative Then
                        sqlString += " AND "
                    Else
                        sqlString += " OR "
                    End If
                End If
                parameterList.AddRange(Right.Parameter)
                Dim rightMember As LiteralMember = TryCast(Right, LiteralMember)
                If IsNothing(rightMember) Then Continue For
                Dim typeString = GetComparisonTypeString(DbTypeToComparisonType(rightMember.Type))
                Dim fieldString = String.Format(My.Resources.CastString, EncloseIdentifier(field), typeString)
                Select Case ConditionType
                    Case ConditionType.Equal
                        sqlString += fieldString + "=" + Right.SqlString
                    Case ConditionType.NotEqual
                        sqlString += fieldString + "!=" + Right.SqlString
                    Case ConditionType.MoreThan
                        sqlString += fieldString + ">" + Right.SqlString
                    Case ConditionType.LessThan
                        sqlString += fieldString + "<" + Right.SqlString
                    Case ConditionType.AndMore
                        sqlString += fieldString + ">=" + Right.SqlString
                    Case ConditionType.AndLess
                        sqlString += fieldString + "<=" + Right.SqlString
                    Case ConditionType.Include
                        sqlString += fieldString + " LIKE '%' || " + Right.SqlString + " || '%' ESCAPE '" + ESCAPE_CHARACTER + "'"
                    Case ConditionType.NotInclude
                        sqlString += fieldString + " NOT LIKE '%' || " + Right.SqlString + " || '%' ESCAPE '" + ESCAPE_CHARACTER + "'"
                    Case ConditionType.ForwardMatch
                        sqlString += fieldString + " LIKE " + Right.SqlString + " || '%' ESCAPE '" + ESCAPE_CHARACTER + "'"
                    Case ConditionType.BackwardMatch
                        sqlString += fieldString + " LIKE '%' || " + Right.SqlString + " ESCAPE '" + ESCAPE_CHARACTER + "'"
                    Case ConditionType.ExactMatch
                        sqlString += fieldString + "=" + Right.SqlString
                End Select
            Next
            sqlString += ")"
            Return New SqlAndParameterList(sqlString, parameterList)
        End Function
    End Class
End Namespace
