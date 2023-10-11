Imports System.Drawing

Namespace Table
    Public Class ColumnAppearanceSetting

        Public Property FontSetting() As Font

        Public Property ForeColor() As Color

        Public Property BackColor() As Color

        Public Property Condition() As New List(Of DisplayConditionSetting)()

        Public Property CombinationMethod() As DeterminationMethod = DeterminationMethod.AndCombining
    End Class
End Namespace
