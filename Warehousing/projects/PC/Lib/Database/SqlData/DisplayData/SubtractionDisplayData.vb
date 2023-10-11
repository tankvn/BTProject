Imports Database.Utility

Namespace SqlData.DisplayData
    Public Class SubtractionDisplayData
        Implements IDisplayData

        Private ReadOnly _leftFieldName As String

        Private ReadOnly _rightFieldName As String

        Private ReadOnly _displayName As String

        Sub New(ByVal leftFieldName As String, ByVal rightFieldName As String, ByVal displayName As String)
            _leftFieldName = leftFieldName
            _rightFieldName = rightFieldName
            _displayName = displayName
        End Sub

        Sub New(ByVal leftFieldName As String, ByVal rightFieldName As String)
            Me.New(leftFieldName, rightFieldName, String.Empty)
        End Sub

        Public Function ToSqlAndParameters() As SqlAndParameterList Implements IDisplayData.ToSqlAndParameters
            Return _
    New SqlAndParameterList(AddDisplayName(EncloseIdentifier(_leftFieldName) + " - " + EncloseIdentifier(_rightFieldName), _displayName), New Parameter() {})
        End Function

        Public Function ToSqlAndParametersWithoutDisplayName() As SqlAndParameterList Implements IDisplayData.ToSqlAndParametersWithoutDisplayName
            Return _
   New SqlAndParameterList(EncloseIdentifier(_leftFieldName) + " - " + EncloseIdentifier(_rightFieldName), New Parameter() {})
        End Function
    End Class
End Namespace
