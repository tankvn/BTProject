Imports Database.SqlData.Condition

Namespace SqlData
    Friend Class LogUpdateTarget
        Private ReadOnly _masterName As String
        Public ReadOnly Property MasterName() As String
            Get
                Return _masterName
            End Get
        End Property

        Private ReadOnly _logName As String
        Public ReadOnly Property LogName() As String
            Get
                Return _logName
            End Get
        End Property

        Private ReadOnly _type As UpdateType
        Public ReadOnly Property Type() As UpdateType
            Get
                Return _type
            End Get
        End Property

        Sub New(ByVal logName As String, ByVal masterName As String, ByVal type As UpdateType)
            _logName = logName
            _masterName = masterName
            _type = type
        End Sub
    End Class
End Namespace
