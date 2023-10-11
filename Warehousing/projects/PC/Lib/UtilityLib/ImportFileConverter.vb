
Imports Database
Imports System.IO
Imports System.Globalization
Imports Microsoft.VisualBasic.FileIO
Imports SettingLib
Imports SettingLib.Table
Imports System.Text
Imports Database.SqlData

Public Class ImportFileConverter

    Private Const HISTORY_COLUMN_COUNT As Integer = 8

    Private Const MAX_RECORD_BYTE As Integer = 7680

    Private ReadOnly _importFilePath As String

    Private ReadOnly _masterSetting As MasterSetting

    Private _result As ImportResult

    Public Sub New(ByVal importFilePath As String, ByVal masterSetting As MasterSetting)
        _importFilePath = importFilePath
        _masterSetting = masterSetting
        _result = ImportResult.Success
    End Sub

    Public Function ConvertImportFile(ByVal outputFilePath As String) As ImportFileConvertResult
        Try
            Using parser As TextFieldParser = CreateTextFieldParser()
                Dim hashSet As New HashSet(Of KeyStrings)()
                Using writer As New StreamWriter(outputFilePath, False, New UTF8Encoding(False))
                    While Not parser.EndOfData
                        InsertBreak(writer)
                        Dim fields As String() = parser.ReadFields()
                        ConvertAndWriteLine(fields, writer, hashSet)
                    End While
                End Using
            End Using
            Return New ImportFileConvertResult(outputFilePath, _result)
        Catch ex As FileNotFoundException
            Return New ImportFileConvertResult(String.Empty, ImportResult.Failure Or ImportResult.FileNotFound)
        Catch ex As UnauthorizedAccessException
            Return New ImportFileConvertResult(String.Empty, ImportResult.Failure Or ImportResult.AccessDenied)
        Catch ex As ByteLengthException
            Return New ImportFileConvertResult(String.Empty, ImportResult.Failure Or ImportResult.ExceedByteLength)
        Catch ex As OverflowException
            Return New ImportFileConvertResult(String.Empty, ImportResult.Failure Or ImportResult.Overflow)
        Catch ex As Exception
            Return New ImportFileConvertResult(String.Empty, ImportResult.Failure)
        End Try
    End Function

    Private Sub InsertBreak(ByVal writer As StreamWriter)
        If (writer.BaseStream.Position = 0) Then
            writer.Flush()
            If (writer.BaseStream.Position = 0) Then
                Return
            End If
        End If
        writer.WriteLine()
    End Sub

    Private Sub ConvertAndWriteLine(ByVal fields As String(), ByVal writer As StreamWriter, ByVal hashSet As HashSet(Of KeyStrings))
        Dim keys As New KeyStrings()

        Dim normalColumnCount = If(_masterSetting.HasHistory,
                                   _masterSetting.ColumnList.Count - HISTORY_COLUMN_COUNT,
                                   _masterSetting.ColumnList.Count)
        Dim byteSum As Integer = 0
        For index As Integer = 0 To fields.Count - 1
            If normalColumnCount <= index Then
                _result = _result Or ImportResult.ColumnExceedance
                Exit For
            End If

            If (0 < index) Then
                writer.Write(AppSetting.Separator)
            End If

            Dim columnSetting As ITableColumnSetting = _masterSetting.ColumnList(index)
            Dim convertedValue As String = GetConvertedValue(fields(index), columnSetting)

            If (columnSetting.IsKey) Then
                keys.Add(convertedValue)
            End If

            writer.Write(BasicUtility.EncloseInQuotes(convertedValue))
            byteSum += AppSetting.Encoding.GetByteCount(convertedValue)
        Next

        If (MAX_RECORD_BYTE < byteSum) Then Throw New ByteLengthException()

        If keys.HasItem AndAlso Not hashSet.Add(keys) Then
            _result = _result Or ImportResult.Duplication
        End If
    End Sub

    Private Function GetConvertedValue(ByVal field As String, ByVal columnSetting As ITableColumnSetting) As String
        If columnSetting.DataType <> DataType.IntegerNumber AndAlso columnSetting.DataType <> DataType.FloatNumber Then
            Return field
        End If

        Dim floatValue As Double
        If (Not Double.TryParse(field, NumberStyles.Float, CultureInfo.CurrentCulture, floatValue)) Then
            _result = _result Or ImportResult.Converted
            Return "0"
        End If

        If columnSetting.DataType = DataType.IntegerNumber Then
            Dim integerValue As Integer = Convert.ToInt32(Fix(floatValue))
            If (integerValue <> floatValue) Then
                _result = _result Or ImportResult.Truncated
            End If
            Return integerValue.ToString(CultureInfo.CurrentCulture)
        End If

        Return floatValue.ToString(CultureInfo.CurrentCulture)
    End Function

    Private Function CreateTextFieldParser() As TextFieldParser
        Dim parser As New TextFieldParser(_importFilePath, AppSetting.Encoding)
        parser.SetDelimiters(AppSetting.Separator.ToString())
        parser.HasFieldsEnclosedInQuotes = True
        parser.TrimWhiteSpace = False
        Return parser
    End Function
End Class
