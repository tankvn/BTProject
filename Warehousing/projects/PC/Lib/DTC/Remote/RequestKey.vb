Namespace Remote
    Public Class RequestKey
        Public Property Ip() As String

        Public Property SequenceNo() As UInteger

        Public Sub New(ByVal ip As String, ByVal seq As UInteger)
            _Ip = ip
            SequenceNo = seq
        End Sub

        Public Overrides Function GetHashCode() As Integer
            Dim tempHashCode As Long = CLng(Ip.GetHashCode()) + CLng(SequenceNo.GetHashCode())
            Return tempHashCode.GetHashCode()
        End Function

        Public Overrides Function Equals(obj As Object) As Boolean
            Dim key As RequestKey = TryCast(obj, RequestKey)
            If IsNothing(key) Then Return False
            If Not key.Ip.Equals(Ip) Then Return False
            If Not key.SequenceNo.Equals(SequenceNo) Then Return False

            Return True
        End Function

    End Class
End Namespace
