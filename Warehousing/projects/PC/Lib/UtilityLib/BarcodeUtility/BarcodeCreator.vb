Imports System.Runtime.InteropServices
Imports System.IO
Imports System.Drawing
Imports System.Drawing.Imaging
Imports SettingLib
Imports System.Text

Namespace BarcodeUtility

    Public Class BarcodeCreator

        Private Const MAX_ASCII_CODE As Integer = 127

        Private Const QUESTION_CODE_NUMBER As Integer = 63

        <DllImport("BarcodeUtility\EncodeLibrary.dll", CallingConvention:=CallingConvention.Cdecl, CharSet:=CharSet.Ansi)> _
        Private Shared Function CreateBarcode(ByVal kind As Integer, ByVal quiet As Integer, ByVal checkDigit As Short, _
                                              ByVal barcodeString As Byte(), ByVal outputPath As Byte(), _
                                              ByVal key As Byte()) As Integer
        End Function

        Public Sub MakeBarcodeImages(ByVal kind As BarcodeKind, ByVal quiet As Integer, ByVal checkDigit As Short, _
                          ByVal inputString As String, ByVal filePath As String)
            Try
                Dim bytes As Byte() = Encoding.ASCII.GetBytes(inputString)
                If Not InputCheck(inputString, bytes, kind) Then Return
                Dim tempFolderPath As String = Path.GetTempPath()
                Dim outputFileName As String = Path.Combine(tempFolderPath, filePath)
                Dim outputPath As Byte() = Encoding.ASCII.GetBytes(outputFileName)
                Dim key As Byte() = Encoding.ASCII.GetBytes("Disabled other than BT")
                Dim barcodeSize As Integer = CreateBarcode(kind, quiet, checkDigit, bytes, outputPath, key)
                CutMargin(barcodeSize, filePath)
            Catch ex As Exception
                Throw
            End Try
        End Sub

        Private Function InputCheck(ByVal inputText As String, ByVal byteText As Byte(), ByVal barcodeKind As BarcodeKind) As Boolean
            If inputText.Length <= 0 Then Return False
            Select Case barcodeKind
                Case barcodeKind.C128, barcodeKind.C93
                    If Not CheckAscii(byteText) Then Return False
                Case barcodeKind.C39
                    If Not CheckCode39Character(byteText) Then Return False
            End Select

            Return QuestionCountCheck(inputText, byteText)
        End Function

        Private Function CheckCode39Character(ByVal byteText As Byte()) As Boolean
            For Each character As Byte In byteText
                If IsUpperAlphabet(character) Then Continue For
                If IsNumber(character) Then Continue For
                If IsCode39Symbol(character) Then Continue For
                Return False
            Next
            Return True
        End Function

        Private Function IsCode39Symbol(ByVal character As Byte) As Boolean
            Dim characterInteger As Integer = Convert.ToInt32(character)
            Return characterInteger = Asc("-"c) OrElse
                characterInteger = Asc("."c) OrElse
                characterInteger = Asc(" "c) OrElse
                characterInteger = Asc("$"c) OrElse
                characterInteger = Asc("/"c) OrElse
                characterInteger = Asc("+"c) OrElse
                characterInteger = Asc("%"c)
        End Function

        Private Function IsUpperAlphabet(ByVal character As Byte) As Boolean
            Dim characterInteger As Integer = Convert.ToInt32(character)
            Return Asc("A"c) <= characterInteger AndAlso characterInteger <= Asc("Z"c)
        End Function

        Private Function IsNumber(ByVal character As Byte) As Boolean
            Dim characterInteger As Integer = Convert.ToInt32(character)
            Return Asc("0"c) <= characterInteger AndAlso characterInteger <= Asc("9"c)
        End Function

        Private Function CheckAscii(ByVal byteText As Byte()) As Boolean

            For Each character As Byte In byteText
                If MAX_ASCII_CODE < character Then Return False
            Next
            Return True
        End Function

        Private Function QuestionCountCheck(ByVal inputText As String, ByVal byteText As Byte()) As Boolean
            Dim questionCount As Integer = 0
            For Each inputString As Char In inputText
                If inputString = "?"c Then
                    questionCount += 1
                End If
            Next
            Dim byteQuestionCount As Integer = 0
            For Each inputByte As Byte In byteText
                If inputByte = CByte(QUESTION_CODE_NUMBER) Then
                    byteQuestionCount += 1
                End If
            Next
            Return (questionCount = byteQuestionCount)
        End Function

        Private Sub CutMargin(ByVal barcodeSize As Integer, ByVal barcodeFileName As String)
            Dim barcodeImage As Bitmap = New Bitmap(barcodeFileName)
            Dim rect As Rectangle = New Rectangle(0, 30, barcodeSize, barcodeImage.Height - 60)
            Dim newBitmap As Bitmap = barcodeImage.Clone(rect, barcodeImage.PixelFormat)

            barcodeImage.Dispose()
            If File.Exists(barcodeFileName) Then
                File.Delete(barcodeFileName)
            End If

            newBitmap.Save(barcodeFileName, ImageFormat.Bmp)
            newBitmap.Dispose()
        End Sub
    End Class
End Namespace
