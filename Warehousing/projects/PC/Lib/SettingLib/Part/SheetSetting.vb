Imports Database.SqlData.DisplayData

Namespace Part
    Public Class SheetSetting

        Public Property IdentityName() As String

        Public Property TableName() As String

        Public Property DisplayDatabaseType() As DisplayDatabaseType

        Public Property CalculationColumnList() As CalculationColumnDisplayDataList

        Public Property NumericColumnNameList() As List(Of String)

        Public Property DateColumnNameList() As List(Of String)

        Public Property DisplayColumnList() As List(Of DisplayColumnSetting)

        Public Property HistoryColumnList() As List(Of DisplayColumnSetting)

        Public Property FilterSettingList() As FilterSettingList

        Public Property GuiPartSettingList() As GuiPartSettingList

        Public Property ControlPartSettingList() As ControlPartSettingList

        Public Property UpdateOnShown() As Boolean

        Public Property ShowColumnHeader() As Boolean

        Public Property ShowRowNumber() As Boolean

        Public ReadOnly Property DisplayDateColumnList() As List(Of DisplayColumnSetting)
            Get
                Dim list As New List(Of DisplayColumnSetting)
                For Each displayColumnSetting As DisplayColumnSetting In DisplayColumnList
                    If Not IsNothing(DateColumnNameList) AndAlso
                        DateColumnNameList.Contains(displayColumnSetting.Name) Then
                        list.Add(displayColumnSetting)
                    End If
                Next
                Return list
            End Get
        End Property

        Public Sub New()
            IdentityName = String.Empty
            TableName = String.Empty
        End Sub
    End Class
End Namespace
