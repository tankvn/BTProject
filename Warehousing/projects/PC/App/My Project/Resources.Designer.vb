

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
                    Dim temp As Global.System.Resources.ResourceManager = New Global.System.Resources.ResourceManager("App.Resources", GetType(Resources).Assembly)
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
        Friend ReadOnly Property Closing() As String
            Get
                Return ResourceManager.GetString("Closing", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''</summary>
        Friend ReadOnly Property ErrorMessageFailedCloseTransferApp() As String
            Get
                Return ResourceManager.GetString("ErrorMessageFailedCloseTransferApp", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''</summary>
        Friend ReadOnly Property ErrorMessageFailedStartTransferApp() As String
            Get
                Return ResourceManager.GetString("ErrorMessageFailedStartTransferApp", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''</summary>
        Friend ReadOnly Property ErrorMultipleStartUp() As String
            Get
                Return ResourceManager.GetString("ErrorMultipleStartUp", resourceCulture)
            End Get
        End Property
        
        Friend ReadOnly Property PCApplication() As System.Drawing.Icon
            Get
                Dim obj As Object = ResourceManager.GetObject("PCApplication", resourceCulture)
                Return CType(obj,System.Drawing.Icon)
            End Get
        End Property
    End Module
End Namespace
