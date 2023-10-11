Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms
Imports SettingLib.Part
Imports UtilityLib

Namespace TabPart
    Public Class RowPreview
        Private Const TITLE_LABEL_NAME As String = "TitleLabel{0}"

        Private Const COLOGNE_LABEL_NAME As String = "CologneLabel{0}"

        Private Const DATA_LABEL_NAME As String = "DataLabel{0}"

        <Browsable(True)>
        Public Property TitleFont As Font

        <Browsable(True)>
        Public Property TitleForeColor() As Color

        <Browsable(True)>
        Public Property TitleBackColor() As Color

        <Browsable(True)>
        Public Property DataFont As Font

        <Browsable(True)>
        Public Property DataForeColor() As Color

        <Browsable(True)>
        Public Property DataBackColor() As Color

        <Browsable(False)>
        Public Property Setting() As RowPreviewSetting

        Private ReadOnly _dataLabelList As New List(Of Label)

        Protected Overrides Sub OnLoad(ByVal e As EventArgs)
            MyBase.OnLoad(e)
            Setting = ViewUtility.GetPartSetting(Name, Parent, DesignMode)

            _tableLayoutPanel.SuspendLayout()
            _tableLayoutPanel.RowCount = Setting.ItemSettings.Count + 1
            For count = 1 To Setting.ItemSettings.Count
                _tableLayoutPanel.RowStyles.Add(New RowStyle())
            Next
            CreateLabels()
            _tableLayoutPanel.ResumeLayout()
            _tableLayoutPanel.PerformLayout()
        End Sub

        Public Sub UpdateView(ByVal row As DataGridViewRow)
            For count As Integer = 1 To Setting.ItemSettings.Count
                If row.Cells.Count <= count Then Return
                Dim itemSetting As RowPreviewItemSetting = Setting.ItemSettings(count - 1)
                _dataLabelList(count - 1).Text = row.Cells(itemSetting.ColumnNumber - 1).Value.ToString()
            Next
        End Sub

        Private Sub CreateLabels()
            Dim count As Integer = 1
            For Each itemSetting As RowPreviewItemSetting In Setting.ItemSettings
                CreateRow(itemSetting, count)
                count += 1
            Next
        End Sub

        Private Sub CreateRow(ByVal itemSetting As RowPreviewItemSetting, ByVal count As Integer)
            Dim titleLabel As New Label()
            With titleLabel
                .AutoSize = True
                .Name = String.Format(TITLE_LABEL_NAME, count)
                .Text = itemSetting.Title
                .Font = TitleFont
                .ForeColor = TitleForeColor
                .BackColor = TitleBackColor
                .Location = New Point(3, 0)
                .Size = New Size(38, 12)
            End With
            _tableLayoutPanel.Controls.Add(titleLabel, 0, count - 1)

            Dim cologneLabel As New Label()
            With cologneLabel
                .Name = String.Format(COLOGNE_LABEL_NAME, count)
                .Text = ConvertUtility.ToSeparatorString(Setting.RowPreviewSeparator)
                .AutoSize = True
            End With
            _tableLayoutPanel.Controls.Add(cologneLabel, 1, count - 1)

            Dim dataLabel As New Label()
            With dataLabel
                .Name = String.Format(DATA_LABEL_NAME, count)
                .Font = DataFont
                .Size = New Size(10, CreateGraphics().MeasureString(" ", DataFont).Height * itemSetting.RowCount)
                .ForeColor = DataForeColor
                .BackColor = DataBackColor
                .Anchor = AnchorStyles.Top & AnchorStyles.Bottom
            End With
            _tableLayoutPanel.Controls.Add(dataLabel, 2, count - 1)
            _dataLabelList.Add(dataLabel)
        End Sub
    End Class

    Public Class RowChangedEventArgs
        Inherits EventArgs

        Public Sub New(ByVal dataGridViewRow As DataGridViewRow)
            Row = dataGridViewRow
        End Sub

        Public Property Row As DataGridViewRow
    End Class
End Namespace
