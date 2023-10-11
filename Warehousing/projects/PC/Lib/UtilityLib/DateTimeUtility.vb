Imports System.Globalization
Imports SettingLib
Imports SettingLib.Table

Public Class DateTimeUtility
    Public Const SQLITE_DATE_FORMAT As String = "yyyy-MM-dd"

    Private Const SHORT_YEAR_MAX As Integer = 2079

    Public Shared Function ConvertDateString(ByVal dateString As String, ByVal dataColumnSetting As DataColumnSetting) As String
        Dim dateFormatInfo As DateTimeFormatInfo = CType(CultureInfo.InvariantCulture.DateTimeFormat.Clone(), DateTimeFormatInfo)
        dateFormatInfo.Calendar.TwoDigitYearMax = SHORT_YEAR_MAX

        Dim dateValue As DateTime = DateTime.ParseExact(dateString, GetFormatString(dataColumnSetting), dateFormatInfo)
        Return dateValue.ToString(SQLITE_DATE_FORMAT, CultureInfo.InvariantCulture)
    End Function

    Private Shared Function GetFormatString(ByVal dataColumnSetting As DataColumnSetting) As String
        Dim separator As String = String.Empty
        Select Case dataColumnSetting.DateSeparator
            Case DateSeparator.None
                separator = String.Empty
            Case DateSeparator.Slash
                separator = "/"
            Case DateSeparator.Period
                separator = "."
            Case DateSeparator.Hyphen
                separator = "-"
            Case DateSeparator.Space
                separator = " "
        End Select
        Select Case dataColumnSetting.DatePattern
            Case DatePattern.YYYYMMDD
                Return String.Format("yyyy{0}MM{0}dd", separator)
            Case DatePattern.YYYYMM
                Return String.Format("yyyy{0}MM", separator)
            Case DatePattern.YYMMDD
                Return String.Format("yy{0}MM{0}dd", separator)
            Case DatePattern.YYMM
                Return String.Format("yy{0}MM", separator)
            Case DatePattern.MMDDYYYY
                Return String.Format("MM{0}dd{0}yyyy", separator)
            Case DatePattern.MMYYYY
                Return String.Format("MM{0}yyyy", separator)
            Case DatePattern.MMDDYY
                Return String.Format("MM{0}dd{0}yy", separator)
            Case DatePattern.MMYY
                Return String.Format("MM{0}yy", separator)
            Case DatePattern.DDMMYYYY
                Return String.Format("dd{0}MM{0}yyyy", separator)
            Case DatePattern.DDMMYY
                Return String.Format("dd{0}MM{0}yy", separator)
        End Select
        Return String.Format("yyyy{0}MM{0}dd", separator)
    End Function

    Public Shared Function GetFormatString(ByVal dateTimeFormatPattern As DateTimeFormatPattern) As String
        Select Case dateTimeFormatPattern
            Case dateTimeFormatPattern.YYYYMMDD_HHMM
                Return "yyyy/MM/dd HH:mm"
            Case dateTimeFormatPattern.YYMMDD_HHMM
                Return "yy/MM/dd HH:mm"
            Case dateTimeFormatPattern.MMDDYYYY_HHMM
                Return "MM/dd/yyyy HH:mm"
            Case dateTimeFormatPattern.MMDDYY_HHMM
                Return "MM/dd/yy HH:mm"
            Case dateTimeFormatPattern.DDMMYYYY_HHMM
                Return "dd/MM/yyyy HH:mm"
            Case dateTimeFormatPattern.DDMMYY_HHMM
                Return "dd/MM/yy HH:mm"
        End Select
        Return "yyyy/MM/dd HH:mm"
    End Function
End Class
