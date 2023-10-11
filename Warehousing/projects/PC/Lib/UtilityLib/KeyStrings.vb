
Public Class KeyStrings
    Implements IEquatable(Of KeyStrings)

    Private ReadOnly _list As New List(Of String)()

    Public ReadOnly Property HasItem() As Boolean
        Get
            Return 0 < _list.Count
        End Get
    End Property

    Public Sub Add(ByVal key As String)
        _list.Add(key)
    End Sub

    Public Overloads Function Equals(ByVal other As KeyStrings) As Boolean Implements IEquatable(Of KeyStrings).Equals
        Return _list.SequenceEqual(other._list)
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return String.Join(String.Empty, _list.ToArray()).GetHashCode()
    End Function
End Class
