Imports Database.SqlData.Condition

Namespace SqlData
    Public Module DataTypeConverter
        Public Function DataTypeToString(ByVal type As DataType) As String
            Select Case type
                Case DataType.Text
                    Return "TEXT"
                Case DataType.IntegerNumber
                    Return "INTEGER"
                Case DataType.FloatNumber
                    Return "REAL"
                Case DataType.DateText
                    Return "TEXT"
            End Select
            Return String.Empty
        End Function

        Public Function DataTypeToDbType(ByVal type As DataType) As DbType
            Select Case type
                Case DataType.Text
                    Return DbType.String
                Case DataType.IntegerNumber
                    Return DbType.Int32
                Case DataType.FloatNumber
                    Return DbType.Double
                Case DataType.DateText
                    Return DbType.String
            End Select
            Return String.Empty
        End Function

        Public Function DataTypeToComparisonType(ByVal type As DataType) As ComparisonType
            Select Case type
                Case DataType.Text
                    Return ComparisonType.Text
                Case DataType.IntegerNumber, DataType.FloatNumber
                    Return ComparisonType.Numeric
                Case DataType.DateText
                    Return ComparisonType.DateCompare
            End Select
        End Function
    End Module
End Namespace
