Imports Database.SqlData.Condition
Imports Database.SqlData.Condition.Member
Imports Database.Utility
Imports Database.My.Resources

Namespace SqlData.DisplayData
    Public Class CalculationDisplayData
        Implements ICalculationColumnDisplayData

        Private _leftMember As IDisplayData

        Private _rightMember As IDisplayData

        Private _leftFixedValueData As LiteralMember

        Private _rightFixedValueData As LiteralMember

        Private _name As String
        Public Property Name() As String Implements ICalculationColumnDisplayData.Name
            Get
                Return _name
            End Get
            Set(ByVal value As String)
                _name = value
            End Set
        End Property

        Public Property CalculationType() As CalculationType

        Public Property LeftMemberType() As MemberType

        Public Property LeftFixedValue() As Object

        Public Property LeftMemberName() As String

        Public Property RightMemberType() As MemberType

        Public Property RightMemberName() As String

        Public Property RightFixedValue() As Object

        Public Property ShowDecimalPoint() As Boolean

        Public Property DecimalPoint() As Integer

        Public Property ShowPercentage() As Boolean

        Public Property Visible() As Boolean Implements ICalculationColumnDisplayData.Visible

        Friend Function ToSqlAndParameters() As SqlAndParameterList Implements IDisplayData.ToSqlAndParameters
            Return ToSqlAndParameters(True)
        End Function

        Public Function ToSqlAndParametersWithoutDisplayName() As SqlAndParameterList _
            Implements IDisplayData.ToSqlAndParametersWithoutDisplayName
            Return ToSqlAndParameters(False)
        End Function

        Private Function ToSqlAndParameters(ByVal withDisplayName As Boolean) As SqlAndParameterList
            Dim operatorString As String = GetOperatorString(_CalculationType)

            Dim leftSqlAndParameterList As SqlAndParameterList
            If LeftMemberType = MemberType.Field Then
                leftSqlAndParameterList = _leftMember.ToSqlAndParametersWithoutDisplayName()
            Else
                leftSqlAndParameterList = _leftFixedValueData.ToSqlAndParameter()
            End If

            Dim sqlString As String
            Dim parameterList As New List(Of Parameter)(leftSqlAndParameterList.ParameterList)
            If _RightMemberType = MemberType.Field Then
                sqlString = CreateSqlStringAndParametersForField(leftSqlAndParameterList, operatorString, parameterList)
            Else
                sqlString = CreateSqlStringAndParametersForLiteral(leftSqlAndParameterList, operatorString, parameterList)
            End If
            If withDisplayName Then
                sqlString = AddDisplayName(sqlString, _name)
            End If
            Return New SqlAndParameterList(sqlString, parameterList)
        End Function

        Private Function CreateSqlStringAndParametersForLiteral(ByVal leftSqlAndParameterList As SqlAndParameterList, ByVal operatorString As String, ByVal parameterList As List(Of Parameter)) As String
            Dim sqlString As String

            If _CalculationType = CalculationType.Division Then
                sqlString = EncloseInParentheses(leftSqlAndParameterList.SqlString) + operatorString +
                            String.Format(CastToRealSql, _rightFixedValueData.SqlString)
            Else
                sqlString = EncloseInParentheses(leftSqlAndParameterList.SqlString) + operatorString +
                            _rightFixedValueData.SqlString
            End If
            If ShowPercentage Then
                sqlString = EncloseInParentheses(sqlString) + " * 100"
            End If
            If Not ShowDecimalPoint Then
                sqlString = String.Format(CastString, sqlString, "INTEGER")
            Else
                sqlString = String.Format(RoundString, sqlString, DecimalPoint)
            End If

            parameterList.AddRange(_rightFixedValueData.Parameter)
            Return sqlString
        End Function

        Private Function CreateSqlStringAndParametersForField(ByVal leftSqlAndParameterList As SqlAndParameterList, ByVal operatorString As String, ByVal parameterList As List(Of Parameter)) As String
            Dim sqlString As String

            Dim rightSqlAndParameterList = _rightMember.ToSqlAndParametersWithoutDisplayName()
            If _CalculationType = CalculationType.Division Then
                sqlString = EncloseInParentheses(leftSqlAndParameterList.SqlString) + operatorString +
                            String.Format(CastToRealSql, EncloseInParentheses(rightSqlAndParameterList.SqlString))
            Else
                sqlString = EncloseInParentheses(leftSqlAndParameterList.SqlString) + operatorString +
                            EncloseInParentheses(rightSqlAndParameterList.SqlString)
            End If
            If ShowPercentage Then
                sqlString = EncloseInParentheses(sqlString) + " * 100"
            End If
            If Not ShowDecimalPoint Then
                sqlString = String.Format(CastString, sqlString, "INTEGER")
            Else
                sqlString = String.Format(RoundString, sqlString, DecimalPoint)
            End If
            parameterList.AddRange(rightSqlAndParameterList.ParameterList)
            Return sqlString
        End Function

        Friend Sub UpdateMemberData(ByVal dataList As CalculationColumnDisplayDataList) Implements ICalculationColumnDisplayData.UpdateMemberData
            UpdateLeftMemberData(dataList)
            UpdateRightMemberData(dataList)
        End Sub

        Private Sub UpdateRightMemberData(ByVal dataList As CalculationColumnDisplayDataList)

            If RightMemberType = MemberType.FixedValue Then
                _rightFixedValueData = New LiteralMember(RightFixedValue, DataTypeToDbType(DataType.FloatNumber))
                Return
            End If
            Dim rightDisplayData As CalculationDisplayData = dataList.GetData(RightMemberName)
            If IsNothing(rightDisplayData) Then
                _rightMember = New ColumnDisplayData(RightMemberName)
            Else
                _rightMember = rightDisplayData
            End If
        End Sub

        Private Sub UpdateLeftMemberData(ByVal dataList As CalculationColumnDisplayDataList)
            If LeftMemberType = MemberType.FixedValue Then
                _leftFixedValueData = New LiteralMember(LeftFixedValue, DataTypeToDbType(DataType.FloatNumber))
                Return
            End If

            Dim leftDisplayData As CalculationDisplayData = dataList.GetData(LeftMemberName)
            If IsNothing(leftDisplayData) Then
                _leftMember = New ColumnDisplayData(LeftMemberName)
            Else
                _leftMember = leftDisplayData
            End If
        End Sub
    End Class
End Namespace
