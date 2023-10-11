Imports Database.Utility

Namespace SqlData.DisplayData
    Public Class ColumnDisplayData
        Implements IDisplayData

        Private ReadOnly _fieldName As String
        Friend ReadOnly Property FieldName() As String
            Get
                Return _fieldName
            End Get
        End Property

        Private ReadOnly _displayName As String

        Sub New(ByVal fieldName As String)
            Me.New(fieldName, String.Empty)
        End Sub

        Sub New(ByVal fieldName As String, ByVal displayName As String)
            _fieldName = fieldName
            _displayName = displayName
        End Sub

        Friend Function ToSqlAndParameters() As SqlAndParameterList Implements IDisplayData.ToSqlAndParameters
            Return New SqlAndParameterList(AddDisplayName(EncloseIdentifier(_fieldName), _displayName), New Parameter() {})
        End Function

        Public Function ToSqlAndParametersWithoutDisplayName() As SqlAndParameterList Implements IDisplayData.ToSqlAndParametersWithoutDisplayName
            Return New SqlAndParameterList(EncloseIdentifier(_fieldName), New Parameter() {})
        End Function
    End Class
End Namespace
