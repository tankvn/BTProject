Imports System.IO
Imports System.Drawing
Imports System.Drawing.Printing
Imports Database.SqlData
Imports System.Globalization
Imports SettingLib.Part
Imports UtilityLib.BarcodeUtility
Imports SettingLib
Imports ProcessLib.My.Resources

Namespace PartProcess

    Public Class PrintProcessor
        Implements IDisposable

        Private Const TITLE_TEXT_SIZE As Integer = 20

        Private Const TEXT_SIZE As Integer = 9

        Private Const PAGE_NUMBER_HEIGHT As Integer = 30 * 3

        Private Const DRAW_STRING_MARGIN As Integer = 2 * 3

        Private Const DRAW_BARCODE_MARGIN As Integer = 10

        Private Const HEADER_BARCODE_MARGIN As Integer = 20

        Private Const ROW_NUMBER_COLUMN_WIDTH As Integer = 150

        Private Const HEADER_MARGIN As Integer = 6 * 3

        Private Const HEADER_LINE_WIDTH = 800

        Private Const BARCODE_IMAGE_EXTENSION As String = ".bmp"

        Private _currentPage As Integer = 1

        Private _currentDataListRowIndex As Integer = 0

        Private _currentRowNumber As Integer = 1

        Private _currentDrawPointY As Integer = 0

        Private _currentSlipNumber As String = Nothing

        Private ReadOnly _setting As FormPrintButtonSetting

        Private ReadOnly _orderColumnList As String()

        Private ReadOnly _columnWidths As List(Of Integer)

        Private ReadOnly _dataTable As DataTable

        Private ReadOnly _showRowNumber As Boolean

        Private ReadOnly _showColumnHeader As Boolean

        Private ReadOnly _barcodeDictionary As Dictionary(Of String, String) = New Dictionary(Of String, String)()

        Public Sub New(ByVal dataTable As DataTable, ByVal setting As FormPrintButtonSetting,
                       ByVal showRowNumber As Boolean, ByVal showColumnHeader As Boolean)
            _setting = setting
            _dataTable = dataTable
            _columnWidths = setting.PrintColumnSettings.Select(Function(columnSetting) columnSetting.WidthWeight).ToList()
            _showRowNumber = showRowNumber
            _showColumnHeader = showColumnHeader
            If setting.PrintFormat <> PrintFormat.Slip Then Return

            _dataTable = TableDisplayGridProcess.SortDataTable(dataTable, setting.SlipColumnIndex, SqlClient.SortOrder.Ascending)
            _orderColumnList = CreateColumnArray(_dataTable, _setting.SlipColumnIndex)

            Dim barcodeColumnList As List(Of String()) = New List(Of String())()
            For Each columnNumber As Integer In _setting.GetBarcodeColumnNumbers()
                barcodeColumnList.Add(CreateColumnArray(_dataTable, columnNumber))
            Next

            MakeBarcodeImages(barcodeColumnList, setting.BarcodeKind)
        End Sub

        Private Sub CorrectWidths(ByVal width As Integer)
            Dim sum As Integer = _columnWidths.Sum()
            If sum <= 0 Then Return
            Dim ratio As Double = width / sum
            For index As Integer = 0 To _columnWidths.Count - 1
                _columnWidths(index) = Convert.ToInt32(Fix(_columnWidths(index) * ratio))
            Next
        End Sub

        Public Overloads Sub Dispose() Implements IDisposable.Dispose
            For Each imagePath In _barcodeDictionary
                Try
                    Dim tempFolderPath As String = Path.GetTempPath()
                    Dim fileInfo As FileInfo = New FileInfo(Path.Combine(tempFolderPath, imagePath.Value))
                    fileInfo.Delete()
                Catch ex As Exception
                End Try
            Next
            _barcodeDictionary.Clear()
        End Sub

        Private Function CreateColumnArray(ByVal dataTable As DataTable, ByVal columnNumber As Integer) As String()
            Dim columnData As List(Of String) = New List(Of String)()
            For Each row As DataRow In dataTable.Rows
                columnData.Add(row(columnNumber).ToString())
            Next
            Return columnData.ToArray()
        End Function

        Private Sub MakeBarcodeImages(ByVal inputDataList As IEnumerable(Of String()), ByVal barcodeKind As BarcodeKind)
            For Each inputData As String() In inputDataList
                Dim dataList As String() = inputData.Distinct().ToArray()
                For Each barcodeData As String In dataList
                    If _barcodeDictionary.ContainsKey(barcodeData) Then Continue For
                    Dim fileInfo As FileInfo
                    Do
                        Dim tempFolderPath As String = Path.GetTempPath()
                        Dim tempFileName As String = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + BARCODE_IMAGE_EXTENSION
                        fileInfo = New FileInfo(Path.Combine(tempFolderPath, tempFileName))
                    Loop While File.Exists(fileInfo.FullName)

                    Dim creator As BarcodeCreator = New BarcodeCreator()

                    If 100 < barcodeData.Length Then Continue For

                    Try
                        creator.MakeBarcodeImages(barcodeKind, 0, 0, barcodeData.Replace("%", "%%"), fileInfo.FullName)
                        If Not File.Exists(fileInfo.FullName) Then
                            Dim image As New Bitmap(1, 1)
                            image.Save(fileInfo.FullName)
                        End If

                        _barcodeDictionary.Add(barcodeData, fileInfo.FullName)
                    Catch ex As Exception
                    End Try
                Next
            Next
        End Sub

        Public Sub PrintDocument_PrintPage(ByVal sender As Object, ByVal e As PrintPageEventArgs)
            If _dataTable.Rows.Count < 1 Then Return

            e.Graphics.PageUnit = GraphicsUnit.Document

            Dim listWidth = e.MarginBounds.Width * 3
            If _showRowNumber Then
                listWidth -= ROW_NUMBER_COLUMN_WIDTH
            End If
            CorrectWidths(listWidth)
            _currentDrawPointY = e.MarginBounds.Top * 3 + PAGE_NUMBER_HEIGHT

            If _setting.PrintFormat = PrintFormat.Slip Then
                Dim previousSlipNumber As String = _currentSlipNumber
                _currentSlipNumber = _dataTable.Rows(_currentDataListRowIndex).Item(_setting.SlipColumnIndex).ToString()
                Dim needTitleAndHeader As Boolean = previousSlipNumber Is Nothing OrElse Not (_currentSlipNumber = previousSlipNumber)
                If needTitleAndHeader Then
                    DrawTitle(e)
                    DrawHeader(e)
                End If
                DrawSlipForm(e, _setting, _currentSlipNumber, needTitleAndHeader)
            Else
                DrawNormalForm(e, _setting, _currentSlipNumber)
            End If
            If (_dataTable.Rows.Count() <= _currentDataListRowIndex) Then
                e.HasMorePages = False
            Else
                e.HasMorePages = True
                _currentPage += 1
            End If
        End Sub

        Private Sub DrawTitle(ByVal e As PrintPageEventArgs)
            Dim drawPoint As New PointF(e.MarginBounds.Left * 3, _currentDrawPointY)
            Dim font As New Font(FontName, TITLE_TEXT_SIZE, FontStyle.Underline)
            Dim drawHeight As Double = e.Graphics.MeasureString(_setting.Title, font, e.MarginBounds.Width * 3).Height
            Dim stringFormat As StringFormat = New StringFormat()
            stringFormat.Alignment = StringAlignment.Center
            e.Graphics.DrawString(_setting.Title, font, Brushes.Black, New RectangleF(drawPoint, New SizeF(e.MarginBounds.Width * 3, drawHeight)), stringFormat)
            _currentDrawPointY += drawHeight + DRAW_STRING_MARGIN

            font.Dispose()
        End Sub

        Private Sub DrawHeader(ByVal e As PrintPageEventArgs)
            Dim drawPoint = New PointF(e.MarginBounds.Left * 3, _currentDrawPointY)
            Dim font As New Font(FontName, TEXT_SIZE)
            Dim pen As New Pen(Brushes.Black, 2)
            Dim stringFormat As New StringFormat With
                {
                    .Alignment = StringAlignment.Near,
                    .LineAlignment = StringAlignment.Far
                }
            For index As Integer = 0 To _setting.PrintColumnSettings.Count - 1
                If _setting.PrintColumnSettings(index).PrintLocation = PrintLocation.List OrElse
                    _setting.PrintColumnSettings(index).PrintLocation = PrintLocation.None Then Continue For

                Dim headerText = String.Format("{0} :  {1}", _setting.PrintColumnSettings(index).DisplayName,
                                               _dataTable.Rows(_currentDataListRowIndex).Item(index))

                Dim drawsize As SizeF = e.Graphics.MeasureString(headerText, font, HEADER_LINE_WIDTH)
                Dim currentDrawTop As Single = drawPoint.Y
                Dim barcodeImage As Image = Nothing
                Try
                    If _setting.PrintColumnSettings(index).AddBarcode Then
                        Dim barcodePath As String = String.Empty
                        If (_barcodeDictionary.TryGetValue(_dataTable.Rows(_currentDataListRowIndex).Item(index), barcodePath)) Then
                            barcodeImage = New Bitmap(barcodePath)
                            drawPoint.Y += HEADER_BARCODE_MARGIN
                            drawsize.Height = Math.Max(drawsize.Height, barcodeImage.Height * 3)
                        End If
                    End If
                Catch ex As Exception
                End Try
                e.Graphics.DrawString(headerText, font, Brushes.Black, New RectangleF(drawPoint, drawsize), stringFormat)
                drawPoint.Y += drawsize.Height
                If _setting.PrintColumnSettings(index).AddBarcode AndAlso Not IsNothing(barcodeImage) Then
                    Dim x = drawPoint.X + HEADER_LINE_WIDTH + DRAW_BARCODE_MARGIN
                    e.Graphics.DrawImage(barcodeImage, New Point(x, currentDrawTop))
                    drawPoint.Y = Math.Max(drawPoint.Y, currentDrawTop + barcodeImage.Height * 3 + DRAW_BARCODE_MARGIN)
                End If
                e.Graphics.DrawLine(pen, drawPoint, New PointF(drawPoint.X + HEADER_LINE_WIDTH, drawPoint.Y))
                drawPoint.Y += HEADER_MARGIN
            Next

            font.Dispose()
            pen.Dispose()
            _currentDrawPointY = drawPoint.Y
        End Sub

        Private Sub DrawSlipForm(ByVal e As PrintPageEventArgs, ByVal printData As FormPrintButtonSetting,
                                 ByVal currentSlipNumber As String, ByVal hasTitleAndHeader As Boolean)
            Dim slipFromData As New FormPrintButtonSetting
            slipFromData.PrintColumnSettings = printData.PrintColumnSettings
            slipFromData.PrintFormat = printData.PrintFormat
            slipFromData.SlipColumnName = printData.SlipColumnName
            DrawingDataList(e, slipFromData, currentSlipNumber, hasTitleAndHeader)
        End Sub

        Private Sub DrawNormalForm(ByVal e As PrintPageEventArgs, ByVal printData As FormPrintButtonSetting, ByVal currentSlipNumber As String)
            DrawingDataList(e, printData, currentSlipNumber, False)
        End Sub

        Private Function ConvertStringArray(ByVal dataTable As DataTable) As String()()
            Dim dataList As New List(Of String())
            For Each row As DataRow In dataTable.Rows
                dataList.Add(Array.ConvertAll(row.ItemArray, Function(input As Object) input.ToString()))
            Next
            Return dataList.ToArray()
        End Function

        Private Sub DrawingDataList(ByVal e As PrintPageEventArgs, ByVal setting As FormPrintButtonSetting,
                                    ByVal currentSlipNumber As String, ByVal hasTitleAndHeader As Boolean)
            Dim font As New Font(FontName, TEXT_SIZE)
            Dim isSlipForm As Boolean = setting.PrintFormat = PrintFormat.Slip
            Dim currentLocation As Point = New Point(e.MarginBounds.Left * 3, _currentDrawPointY)

            Dim pageText As String = String.Format(PrintPageFormat, _currentPage)
            Dim textSize As SizeF = e.Graphics.MeasureString(pageText, font)
            e.Graphics.DrawString(pageText, font, Brushes.Black, e.MarginBounds.Right * 3 - textSize.Width, e.MarginBounds.Top * 3)

            Dim dataList As String()() = ConvertStringArray(_dataTable)
            Dim columnHeaderHeight = GetColumnHeaderHeight(setting, e, font)

            Dim startRowIndex As Integer = _currentDataListRowIndex
            For rowIndex As Integer = _currentDataListRowIndex To dataList.Count() - 1
                If ((isSlipForm) AndAlso (Not _orderColumnList(rowIndex).TrimEnd() = currentSlipNumber.TrimEnd())) Then
                    _currentRowNumber = 1
                    Exit For
                End If
                Dim isFirstRow As Boolean = (startRowIndex = rowIndex)

                Dim rowHeight As Integer = CalculateHeight(e.Graphics, dataList(rowIndex), _columnWidths.ToArray(),
                                                font, setting.GetBarcodeColumnNumbers().ToArray(), setting)
                Dim newHeight As Integer = rowHeight
                If isFirstRow Then
                    newHeight += columnHeaderHeight
                End If

                If ((e.MarginBounds.Bottom * 3 - currentLocation.Y) < newHeight) Then
                    If Not hasTitleAndHeader AndAlso (rowIndex = startRowIndex) Then
                        DrawColumnHeader(setting, e, font, currentLocation, isSlipForm)
                        currentLocation.Y += columnHeaderHeight
                        DrawLineMethod(e.Graphics, dataList(rowIndex), setting,
                                       e.MarginBounds.Height * 3 - currentLocation.Y, currentLocation, _
                                       Brushes.White, font, isSlipForm, False, setting.GetBarcodeColumnNumbers().ToArray())
                        _currentDataListRowIndex += 1
                    End If
                    Exit For
                End If

                If isFirstRow Then
                    DrawColumnHeader(setting, e, font, currentLocation, isSlipForm)
                    currentLocation.Y += columnHeaderHeight
                End If
                DrawLineMethod(e.Graphics, dataList(rowIndex), setting, rowHeight, currentLocation,
                               Brushes.White, font, isSlipForm, False, setting.GetBarcodeColumnNumbers().ToArray())
                currentLocation.Y += rowHeight
                _currentDataListRowIndex += 1
            Next
        End Sub

        Private Sub DrawColumnHeader(ByVal setting As FormPrintButtonSetting, ByVal e As PrintPageEventArgs,
                                     ByVal font As Font, ByVal currentLocation As Point, ByVal isSlipForm As Boolean)
            If Not _showColumnHeader Then Return

            Dim headList As List(Of String) = New List(Of String)()
            For index As Integer = 0 To setting.PrintColumnSettings.Count - 1
                headList.Add(setting.PrintColumnSettings(index).DisplayName)
            Next
            Dim headHeight As Integer = CalculateHeight(e.Graphics, headList.ToArray(), _columnWidths.ToArray(), font, New Integer() {}, setting)
            DrawLineMethod(e.Graphics, headList.ToArray(), setting, headHeight, currentLocation,
                           Brushes.WhiteSmoke, font, isSlipForm, True, New Integer() {})
        End Sub

        Private Function GetColumnHeaderHeight(ByVal setting As FormPrintButtonSetting, ByVal e As PrintPageEventArgs,
                             ByVal font As Font) As Integer
            If Not _showColumnHeader Then Return 0

            Dim headList As List(Of String) = New List(Of String)()
            For index As Integer = 0 To setting.PrintColumnSettings.Count - 1
                headList.Add(setting.PrintColumnSettings(index).DisplayName)
            Next
            Return CalculateHeight(e.Graphics, headList.ToArray(), _columnWidths.ToArray(), font, New Integer() {}, setting)
        End Function

        Private Sub DrawLineMethod(ByVal graphics As Graphics, ByVal dataList As String(), ByVal printData As FormPrintButtonSetting,
                                   ByVal columnHeight As Integer, ByVal location As Point, ByVal brush As Brush,
                                   ByVal font As Font, ByVal isSlipForm As Boolean, ByVal isHeadRow As Boolean,
                                   ByVal needBarcodeColumnNumberList As Integer())
            Dim widthList As Integer() = _columnWidths.ToArray()
            Dim format As New StringFormat
            If _showRowNumber AndAlso 0 < widthList.Sum() Then
                format.Alignment = StringAlignment.Far
                DrawCellMethod(graphics,
                               If(isHeadRow,
                                  String.Empty,
                                  _currentRowNumber.ToString(CultureInfo.InvariantCulture)).ToString(),
                               New Rectangle(location.X, location.Y, ROW_NUMBER_COLUMN_WIDTH, columnHeight),
                               brush, font, format, False)
                location.X += ROW_NUMBER_COLUMN_WIDTH
                If Not isHeadRow Then
                    _currentRowNumber += 1
                End If
            End If

            For column As Integer = 0 To dataList.Length - 1
                If isSlipForm AndAlso
                    ((printData.PrintColumnSettings(column).PrintLocation = PrintLocation.Header) OrElse
                    (printData.PrintColumnSettings(column).PrintLocation = PrintLocation.None)) Then Continue For
                format.Alignment = If(isHeadRow, StringAlignment.Near, GetAlignmentSetting(printData.PrintColumnSettings(column).Type))
                DrawCellMethod(graphics, dataList(column), New Rectangle(location.X, location.Y, widthList(column), columnHeight),
                               brush, font, format, needBarcodeColumnNumberList.Contains(column))
                location.X += widthList(column)
            Next
        End Sub

        Private Sub DrawCellMethod(ByVal graphics As Graphics, ByVal drawData As String, ByVal rectangle As Rectangle,
                                   ByVal brush As Brush, ByVal font As Font, ByVal format As StringFormat, ByVal needBarcode As Boolean)
            graphics.FillRectangle(brush, rectangle)
            graphics.DrawRectangle(Pens.Black, rectangle)
            rectangle.Y += DRAW_STRING_MARGIN
            If needBarcode Then
                Dim barcodePath As String = String.Empty
                If (_barcodeDictionary.TryGetValue(drawData, barcodePath)) Then
                    Using barcodeImage As Image = New Bitmap(barcodePath)
                        graphics.DrawImage(barcodeImage, New Point(rectangle.X + DRAW_BARCODE_MARGIN, rectangle.Y))
                        rectangle.X += DRAW_BARCODE_MARGIN
                        rectangle.Width -= DRAW_BARCODE_MARGIN
                        rectangle.Y += barcodeImage.Size.Height * 3
                    End Using
                End If
            End If
            graphics.DrawString(drawData, font, Brushes.Black, rectangle, format)
        End Sub

        Private Function CalculateHeight(ByVal graphics As Graphics, ByVal printingDataList As String(),
                                         ByVal widthList As Integer(), ByVal font As Font,
                                         ByVal needBarcodeColumnNumberList As Integer(),
                                         ByVal setting As FormPrintButtonSetting) As Integer
            Dim height As Integer
            For columnIndex As Integer = 0 To printingDataList.Length - 1
                If setting.PrintFormat = PrintFormat.Slip AndAlso
                    (setting.PrintColumnSettings(columnIndex).PrintLocation = PrintLocation.Header OrElse
                    setting.PrintColumnSettings(columnIndex).PrintLocation = PrintLocation.None) Then Continue For
                Dim drawSize As SizeF = graphics.MeasureString(printingDataList(columnIndex), font, widthList(columnIndex))
                If needBarcodeColumnNumberList.Contains(columnIndex) Then
                    Dim barcodePath As String = String.Empty
                    If Not _barcodeDictionary.TryGetValue(printingDataList(columnIndex), barcodePath) Then
                        height = Math.Max(height, drawSize.Height)
                        Continue For
                    End If
                    Using barcodeImage As Image = New Bitmap(barcodePath)
                        drawSize.Height += barcodeImage.Height * 3 + DRAW_BARCODE_MARGIN
                    End Using
                End If
                height = Math.Max(height, drawSize.Height)
            Next
            Return height + DRAW_STRING_MARGIN
        End Function

        Private Function GetAlignmentSetting(ByVal dataType As DataType) As StringAlignment
            Select Case dataType
                Case dataType.FloatNumber, dataType.IntegerNumber
                    Return StringAlignment.Far
                Case dataType.DateText, dataType.Text
                    Return StringAlignment.Near
            End Select
            Return StringAlignment.Near
        End Function

    End Class
End Namespace
