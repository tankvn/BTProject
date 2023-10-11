Public Enum CalculationLabelType
    NumericalNumber
    Sum
    Average
End Enum

Public Enum DeterminationMethod
    AndCombining
    OrCombining
End Enum

Public Enum DatePattern
    YYYYMMDD
    YYYYMM
    YYMMDD
    YYMM
    MMDDYYYY
    MMYYYY
    MMDDYY
    MMYY
    DDMMYYYY
    DDMMYY
End Enum

Public Enum DateSeparator
    None
    Slash
    Period
    Hyphen
    Space
End Enum

Public Enum GridViewCellBackColor
    LightGray
    DarkGray
    Red
    Blue
    Green
    Orange
End Enum

Public Enum RowPreviewSeparator
    Cologne
    None
End Enum

Public Enum BarcodeKind
    C39 = 1
    C128 = 4
    C93 = 15
End Enum

Public Enum ProcessResult
    Success
    Canceled
    DbAccessError
    None
    UnknownError
    FileAccessError
    PrinterError
    ExistUnprocessedData
    NoImportFile
    BackupFailed
    CreateDirectoryError
    SettingsError
    NoData
    PathIsTooLong
    RenameHistoryFailed
End Enum

Public Enum PrintFormat
    Table
    Slip
End Enum

Public Enum DateTimeFormatPattern
    YYYYMMDD_HHMM
    YYMMDD_HHMM
    MMDDYYYY_HHMM
    MMDDYY_HHMM
    DDMMYYYY_HHMM
    DDMMYY_HHMM
End Enum

Public Enum SearchConditionType
    Match
    FrontMatch
    BackMatch
    Include
    NotInclude
    Equal
    NotEqual
    Greater
    Less
    GreaterOrEqual
    LessOrEqual
    FromTo
    [To]
    From
End Enum

Public Enum SearchTarget
    All = 0

    DB1 = 1
    DB2 = 2
    DB3 = 3
    DB4 = 4
    DB5 = 5
    DB6 = 6
    DB7 = 7
    DB8 = 8
    DB9 = 9
    DB10 = 10
    DB11 = 11
    DB12 = 12
    DB13 = 13
    DB14 = 14
    DB15 = 15
    DB16 = 16
    DB17 = 17
    DB18 = 18
    DB19 = 19
    DB20 = 20
    DB21 = 21
    DB22 = 22
    DB23 = 23
    DB24 = 24
    DB25 = 25
    DB26 = 26
    DB27 = 27
    DB28 = 28
    DB29 = 29
    DB30 = 30
    DB31 = 31
    DB32 = 32

    Calculation1 = 33
    Calculation2
    Calculation3
    Calculation4
    Calculation5
    Calculation6
    Calculation7
    Calculation8
End Enum

Public Enum DisplayDatabaseType
    Managed
    Log
End Enum

Public Enum ProgressItem
    Work
    Item
    Figure
End Enum

Public Enum PrintLocation
    Header
    List
    HeaderAndList
    None
End Enum

Public Enum DuplicateDataCheckMode
    None
    PerRecord
    PerReceiveData
End Enum
