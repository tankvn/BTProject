Imports System.Diagnostics.CodeAnalysis

Public Class SuppressEvent
    Implements IDisposable

    Private _counter As SuppressEventCounter

    Public Sub New(ByVal counter As SuppressEventCounter)
        _counter = counter
        _counter.Increment()
    End Sub

    <SuppressMessage("Microsoft.Performance", "CA1821:RemoveEmptyFinalizers")>
    Protected Overrides Sub Finalize()
        MyBase.Finalize()
        If Not IsNothing(_counter) Then
            Trace.WriteLine("Did not disposed!")
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        If Not IsNothing(_counter) Then
            _counter.Decrement()
            _counter = Nothing
        End If
    End Sub
End Class
