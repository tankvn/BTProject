Namespace SqlData
    Public Class Parameter
        Private ReadOnly _type As DbType

        Private ReadOnly _value As Object

        Public Sub New(ByVal value As Object, ByVal type As DbType)
            _value = value
            _type = type
        End Sub

        Public Function ToDbParameter(ByVal command As DbCommand) As DbParameter
            Dim parameter As DbParameter = command.CreateParameter()
            parameter.DbType = _type
            parameter.Value = _value
            If String.Empty.Equals(_value) Then
                parameter.DbType = DbType.String
            End If
            Return parameter
        End Function
    End Class
End Namespace
