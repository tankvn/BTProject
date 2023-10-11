Imports System.Globalization
Imports System.Text

Public Class AppSetting

    Public Shared Property Initialized() As Boolean

    Public Shared Property AppName() As String = String.Empty

    Public Shared Property Language() As String = "ja-JP"

    Public Shared Property DuplicateDataCheckMode() As DuplicateDataCheckMode = DuplicateDataCheckMode.None

    Public Shared ReadOnly Property Culture() As CultureInfo
        Get
            Return CultureInfo.GetCultureInfo(_Language)
        End Get
    End Property

    Public Shared ReadOnly Property Encoding() As Encoding
        Get
            If Language.Equals("ja-JP", StringComparison.OrdinalIgnoreCase) Then Return Encoding.GetEncoding("shift_jis")
            If Language.Equals("en", StringComparison.OrdinalIgnoreCase) Then Return Encoding.GetEncoding("ascii")
            If Language.Equals("zh-Hans", StringComparison.OrdinalIgnoreCase) Then Return Encoding.GetEncoding("GB2312")

            Return Encoding.GetEncoding("shift_jis")
        End Get
    End Property

    Public Shared Property Separator() As Char = ","c

    Public Shared Property ContinueUpdateDb() As Boolean = True

    Public Shared Property AddDoubleQuotes() As Boolean = False

End Class
