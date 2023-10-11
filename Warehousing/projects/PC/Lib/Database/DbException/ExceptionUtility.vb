Imports System.Data.SQLite
Imports System.IO

Namespace DbException
    Public Module ExceptionUtility
        Friend Function ConvertException(ByVal exception As Exception) As Exception
            If TypeOf (exception) Is SQLiteException Then
                Dim sqliteException As SQLiteException = TryCast(exception, SQLiteException)
                Select Case sqliteException.ErrorCode
                    Case SQLiteErrorCode.Error
                        Return New SqlCommandException(exception)
                    Case SQLiteErrorCode.Auth
                        Return New DbConnectionException(exception)
                    Case SQLiteErrorCode.CantOpen
                        Return New DbConnectionException(exception)
                    Case SQLiteErrorCode.Format
                        Return New DbConnectionException(exception)
                    Case SQLiteErrorCode.IoErr
                        Return New DbConnectionException(exception)
                    Case SQLiteErrorCode.NotADb
                        Return New DbConnectionException(exception)
                    Case SQLiteErrorCode.NotFound
                        Return New DbConnectionException(exception)
                End Select
                Return New DbAccessException(exception)
            End If
            If TypeOf (exception) Is FileNotFoundException Then Return New DbConnectionException(exception)

            Return New DbAccessException(exception)
        End Function
    End Module
End Namespace
