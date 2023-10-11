

Namespace Part
    Public Class FilterSettingList
        Private ReadOnly _conditionList As New List(Of FilterSetting)()
        Public ReadOnly Property ConditionList() As List(Of FilterSetting)
            Get
                Return _conditionList
            End Get
        End Property

        Public Property DeterminationMethod() As DeterminationMethod


    End Class
End Namespace
