Imports Database.SqlData.Condition
Imports Database.My.Resources
Imports Database.SqlData.DisplayData

Namespace SqlData
    Public Class LogUpdateTargetList
        Implements IEnumerable(Of LogUpdateTarget)

        Private ReadOnly _updateTargetList As List(Of LogUpdateTarget)

        Sub New()
            _updateTargetList = New List(Of LogUpdateTarget)()
        End Sub

        Friend ReadOnly Property HasSumTarget() As Boolean
            Get
                For Each updateTarget As LogUpdateTarget In _updateTargetList
                    If (updateTarget.Type = UpdateType.Addition) Or (updateTarget.Type = UpdateType.Subtraction) Then Return True
                Next
                Return False
            End Get
        End Property

        Friend ReadOnly Property HasAssignmentTarget() As Boolean
            Get
                For Each updateTarget As LogUpdateTarget In _updateTargetList
                    If (updateTarget.Type = UpdateType.Assignment) Then Return True
                Next
                Return False
            End Get
        End Property

        Friend Sub Add(ByVal logUpdateTarget As LogUpdateTarget)
            _updateTargetList.Add(logUpdateTarget)
        End Sub

        Public Sub Add(ByVal logName As String, ByVal masterName As String, ByVal type As UpdateType)
            Add(New LogUpdateTarget(logName, masterName, type))
        End Sub

        Public Sub Add(ByVal logNames As ICollection(Of String), ByRef masterNames As ICollection(Of String),
                       ByVal type As UpdateType)
            If Not logNames.Count = masterNames.Count Then
                Throw New ArgumentException("logNames Count not equals masterNames")
            End If
            For index As Integer = 0 To logNames.Count - 1
                Add(logNames(index), masterNames(index), type)
            Next
        End Sub

        Friend Function GetSumTargetListString(ByVal integerColumnName As List(Of String)) As String
            Dim sumTargetListString As String = String.Empty
            For Each updateTarget As LogUpdateTarget In _updateTargetList
                If (Not updateTarget.Type = UpdateType.Addition) And (Not updateTarget.Type = UpdateType.Subtraction) Then
                    Continue For
                End If
                If Not sumTargetListString = String.Empty Then
                    sumTargetListString += ","
                End If
                Dim sumTargetString = If(integerColumnName.Contains(updateTarget.MasterName),
                                         String.Format(CastIntegerString, updateTarget.LogName),
                                         updateTarget.LogName)
                sumTargetListString += AddDisplayName(String.Format(SumSql, sumTargetString),
                                                      updateTarget.LogName, False)
            Next
            Return sumTargetListString
        End Function

        Friend Function GetNewTargetListString(ByVal integerColumnName As List(Of String)) As String
            Dim newTargetListString As String = String.Empty
            For Each updateTarget As LogUpdateTarget In _updateTargetList
                If Not updateTarget.Type = UpdateType.Assignment Then Continue For

                If Not newTargetListString = String.Empty Then
                    newTargetListString += ","
                End If
                Dim newTargetString = If(integerColumnName.Contains(updateTarget.MasterName),
                                         String.Format(CastIntegerString, updateTarget.LogName),
                                         updateTarget.LogName)
                newTargetListString += AddDisplayName(newTargetString, updateTarget.LogName)
            Next
            Return newTargetListString
        End Function

        Friend Function IEnumerable_GetEnumerator() As IEnumerator(Of LogUpdateTarget) Implements IEnumerable(Of LogUpdateTarget).GetEnumerator
            Return _updateTargetList.GetEnumerator()
        End Function

        Friend Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return _updateTargetList.GetEnumerator()
        End Function
    End Class
End Namespace
