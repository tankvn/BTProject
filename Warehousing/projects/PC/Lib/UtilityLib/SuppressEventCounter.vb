
Public Class SuppressEventCounter

    Private _counter As Integer

    Public ReadOnly Property IsSuppress() As Boolean
        Get
            Return (0 < _counter)
        End Get
    End Property

    Public Sub Increment()
        _counter += 1
    End Sub

    Public Sub Decrement()
        If (0 < _counter) Then
            _counter -= 1
        End If
    End Sub
End Class
