Imports System.Threading
Imports System.Runtime.InteropServices
Imports Settings
Imports UtilityLib

''' <summary>アプリケーションのエントリポイント</summary>
Public Class EntryPoint
    ''' <summary>多重起動禁止用Mutex</summary>
    Private Shared ReadOnly Mutex As Mutex = New Mutex(False, "{586D879B-9F1A-4a41-9E28-1D6D184E95C4}")

    ''' <summary>ゴーストウィンドウを無効化する</summary>
    <DllImport("user32.dll")>
    Private Shared Sub DisableProcessWindowsGhosting()
    End Sub

    ''' <summary>アプリケーションのエントリポイント</summary>
    <STAThread()>
    Shared Sub Main()
        Try
            If Mutex.WaitOne(0, False) = False Then
                MessageBox.Show(My.Resources.Resources.ErrorMultipleStartUp, My.Application.Info.AssemblyName, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If
            'If Not CanStartUp() Then
            '    Return
            'End If
        Catch ex As AbandonedMutexException
            ' この例外が発生した場合、既にMutexは放棄されているので、起動しても問題ない。
        End Try
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        'ゴーストウィンドウの無効化
        DisableProcessWindowsGhosting()
        AppSettings.InitializeAppSettings()
        Using window As New %MainFormName%()
            SettingsUtility.LoadSettingFile(window)
            Application.Run(window)
        End Using
    End Sub
End Class