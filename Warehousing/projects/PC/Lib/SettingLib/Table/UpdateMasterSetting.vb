Namespace Table
    Public Class UpdateMasterSetting
        Public Property MasterName() As String

        Public Property JobName() As String

        Private ReadOnly _columnSettingList As List(Of UpdateMasterColumnSetting) = New List(Of UpdateMasterColumnSetting)()
        Public ReadOnly Property ColumnSettingList() As List(Of UpdateMasterColumnSetting)
            Get
                Return _columnSettingList
            End Get
        End Property

        Public Function GetSetting(ByVal logName As String) As UpdateMasterColumnSetting
            For Each columnSetting As UpdateMasterColumnSetting In _columnSettingList
                If columnSetting.DataFormatName.Equals(logName) Then
                    Return columnSetting
                End If
            Next
            Return Nothing
        End Function
    End Class
End Namespace
