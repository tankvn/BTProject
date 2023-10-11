Imports Database.SqlData.DisplayData
Imports Database.Utility

Namespace SqlData.Condition.Member
    Public Class DisplayDataMember
        Implements IMember

        Private ReadOnly _displayData As IDisplayData

        Public ReadOnly Property Parameter() As ICollection(Of Parameter) Implements IMember.Parameter
            Get
                Return _displayData.ToSqlAndParametersWithoutDisplayName().ParameterList
            End Get
        End Property

        Public ReadOnly Property SqlString() As String Implements IMember.SqlString
            Get
                Return _displayData.ToSqlAndParametersWithoutDisplayName().SqlString
            End Get
        End Property

        Sub New(ByVal displayData As IDisplayData)
            _displayData = displayData
        End Sub

        Public Function ToSqlAndParameter() As SqlAndParameterList Implements IMember.ToSqlAndParameter
            Dim sqlAndParameterList As SqlAndParameterList = _displayData.ToSqlAndParametersWithoutDisplayName()
            Return _
                New SqlAndParameterList(EncloseInParentheses(sqlAndParameterList.SqlString),
                                        sqlAndParameterList.ParameterList)
        End Function
    End Class
End Namespace
