Imports Database.SqlData.Condition

Namespace Table
    Public Class UpdateMasterColumnSetting
        Public Property MasterName() As String

        Public Property DataFormatName() As String

        Public Property UpdateType() As UpdateType

        Public Property IsKey() As Boolean

        Sub New()
            MasterName = String.Empty
            DataFormatName = String.Empty
            UpdateType = UpdateType.Assignment
            IsKey = False
        End Sub
    End Class
End Namespace
