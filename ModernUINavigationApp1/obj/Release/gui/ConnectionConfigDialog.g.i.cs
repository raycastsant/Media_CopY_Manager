﻿#pragma checksum "..\..\..\gui\ConnectionConfigDialog.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "3933B4BE8EE01BFF6F75F113E38E8E0F5D8C3979"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows;
using FirstFloor.ModernUI.Windows.Controls;
using FirstFloor.ModernUI.Windows.Converters;
using FirstFloor.ModernUI.Windows.Navigation;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace MCP.gui {
    
    
    /// <summary>
    /// ConnectionConfigDialog
    /// </summary>
    public partial class ConnectionConfigDialog : FirstFloor.ModernUI.Windows.Controls.ModernDialog, System.Windows.Markup.IComponentConnector {
        
        
        #line 23 "..\..\..\gui\ConnectionConfigDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock lUser;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\..\gui\ConnectionConfigDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tbxUser;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\gui\ConnectionConfigDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock lPass;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\..\gui\ConnectionConfigDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox tbxPass;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\..\gui\ConnectionConfigDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock lServer;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\..\gui\ConnectionConfigDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tbxServer;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\gui\ConnectionConfigDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock lDB;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\gui\ConnectionConfigDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tbxDB;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\..\gui\ConnectionConfigDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnSaveConnection;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/MCP;component/gui/connectionconfigdialog.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\gui\ConnectionConfigDialog.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.lUser = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 2:
            this.tbxUser = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.lPass = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.tbxPass = ((System.Windows.Controls.PasswordBox)(target));
            return;
            case 5:
            this.lServer = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.tbxServer = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.lDB = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 8:
            this.tbxDB = ((System.Windows.Controls.TextBox)(target));
            return;
            case 9:
            this.BtnSaveConnection = ((System.Windows.Controls.Button)(target));
            
            #line 31 "..\..\..\gui\ConnectionConfigDialog.xaml"
            this.BtnSaveConnection.Click += new System.Windows.RoutedEventHandler(this.BtnSaveConnection_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

