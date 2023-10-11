Imports Database

Public Interface IPreUpdatePart
    Function UpdateView(ByVal dbAccessor As DbAccessor, ByVal tableName As String) As Boolean
End Interface
