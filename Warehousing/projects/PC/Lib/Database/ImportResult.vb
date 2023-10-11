<Flags()>
Public Enum ImportResult

    Success = &H0

    Failure = &H1

    Duplication = &H2

    ColumnExceedance = &H4

    FileNotFound = &H8

    AccessDenied = &H10

    Converted = &H20

    Truncated = &H40

    ExceedByteLength = &H80

    Overflow = &H100
End Enum
