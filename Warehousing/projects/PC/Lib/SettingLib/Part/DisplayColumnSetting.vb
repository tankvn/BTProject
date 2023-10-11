Imports SettingLib.My.Resources
Imports SettingLib.Table

Namespace Part
    Public Class DisplayColumnSetting

        Public Const DEFAULT_WIDTH = 100

        Public Property Name As String

        Public Property DisplayName() As String

        Public Property Width() As Integer = DEFAULT_WIDTH

        Public Property ColumnAppearanceSetting() As ColumnAppearanceSetting

        Public Shared ReadOnly Property StatusColumnSetting() As DisplayColumnSetting
            Get
                Return New DisplayColumnSetting(TableDisplayGridSetting.STATUS_COLUMN_NAME, StatusColumnDisplayName, DEFAULT_WIDTH, New ColumnAppearanceSetting())
            End Get
        End Property

        Public Sub New(ByVal name As String, ByVal displayName As String, ByVal width As Integer,
                       ByVal columnAppearanceSettings As ColumnAppearanceSetting)
            Me.Name = name
            Me.DisplayName = displayName
            Me.Width = width
            ColumnAppearanceSetting = columnAppearanceSettings
        End Sub

        Public Sub New(ByVal name As String, ByVal displayName As String)
            Me.Name = name
            Me.DisplayName = displayName
            ColumnAppearanceSetting = New ColumnAppearanceSetting()
        End Sub
    End Class
End Namespace
