﻿'------------------------------------------------------------------------------
' <auto-generated>
'
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System

Namespace My.Resources
    
    '''<summary>
    '''</summary>
    <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute(),  _
     Global.Microsoft.VisualBasic.HideModuleNameAttribute()>  _
    Friend Module Resources
        
        Private resourceMan As Global.System.Resources.ResourceManager
        
        Private resourceCulture As Global.System.Globalization.CultureInfo
        
        '''<summary>
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend ReadOnly Property ResourceManager() As Global.System.Resources.ResourceManager
            Get
                If Object.ReferenceEquals(resourceMan, Nothing) Then
                    Dim temp As Global.System.Resources.ResourceManager = New Global.System.Resources.ResourceManager("DTC.Resources", GetType(Resources).Assembly)
                    resourceMan = temp
                End If
                Return resourceMan
            End Get
        End Property
        
        '''<summary>
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend Property Culture() As Global.System.Globalization.CultureInfo
            Get
                Return resourceCulture
            End Get
            Set
                resourceCulture = value
            End Set
        End Property
        
        '''<summary>
        '''</summary>
        Friend ReadOnly Property ErrorLogBackUpFailed() As String
            Get
                Return ResourceManager.GetString("ErrorLogBackUpFailed", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''</summary>
        Friend ReadOnly Property ErrorLogDataCannotConvert() As String
            Get
                Return ResourceManager.GetString("ErrorLogDataCannotConvert", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''</summary>
        Friend ReadOnly Property ErrorLogDataDuplicated() As String
            Get
                Return ResourceManager.GetString("ErrorLogDataDuplicated", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''</summary>
        Friend ReadOnly Property ErrorLogDataTruncated() As String
            Get
                Return ResourceManager.GetString("ErrorLogDataTruncated", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''</summary>
        Friend ReadOnly Property ErrorLogDateFormatError() As String
            Get
                Return ResourceManager.GetString("ErrorLogDateFormatError", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''</summary>
        Friend ReadOnly Property ErrorLogLoadMasterData() As String
            Get
                Return ResourceManager.GetString("ErrorLogLoadMasterData", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''</summary>
        Friend ReadOnly Property ErrorLogOutputMasterData() As String
            Get
                Return ResourceManager.GetString("ErrorLogOutputMasterData", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''</summary>
        Friend ReadOnly Property ErrorLogOverFlowError() As String
            Get
                Return ResourceManager.GetString("ErrorLogOverFlowError", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''</summary>
        Friend ReadOnly Property ErrorLogRenameInventoryResultError() As String
            Get
                Return ResourceManager.GetString("ErrorLogRenameInventoryResultError", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''</summary>
        Friend ReadOnly Property ErrorLogUpdateMasterFailed() As String
            Get
                Return ResourceManager.GetString("ErrorLogUpdateMasterFailed", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''</summary>
        Friend ReadOnly Property ErrorMessageImportAccessDenied() As String
            Get
                Return ResourceManager.GetString("ErrorMessageImportAccessDenied", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''</summary>
        Friend ReadOnly Property ErrorMessageImportFileNotFound() As String
            Get
                Return ResourceManager.GetString("ErrorMessageImportFileNotFound", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''</summary>
        Friend ReadOnly Property ExistUnprocessedData() As String
            Get
                Return ResourceManager.GetString("ExistUnprocessedData", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''</summary>
        Friend ReadOnly Property LogFileIsReadOnly() As String
            Get
                Return ResourceManager.GetString("LogFileIsReadOnly", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''</summary>
        Friend ReadOnly Property LogFormatSettingNotFound() As String
            Get
                Return ResourceManager.GetString("LogFormatSettingNotFound", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''</summary>
        Friend ReadOnly Property RegisterLogFailed() As String
            Get
                Return ResourceManager.GetString("RegisterLogFailed", resourceCulture)
            End Get
        End Property
    End Module
End Namespace
