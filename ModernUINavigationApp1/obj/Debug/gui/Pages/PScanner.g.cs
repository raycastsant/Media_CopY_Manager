﻿#pragma checksum "..\..\..\..\gui\Pages\PScanner.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "78BA78236B390B1118A8EE7245B5F6E66863A2C6C712E139DD048CA6BE5F908A"
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
using MCP.gui.components;
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


namespace MCP.gui.Pages {
    
    
    /// <summary>
    /// PScanner
    /// </summary>
    public partial class PScanner : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\..\..\gui\Pages\PScanner.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid _grid;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\..\..\gui\Pages\PScanner.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RowDefinition _cProgress;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\..\gui\Pages\PScanner.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TabControl _tab;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\..\gui\Pages\PScanner.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ProgressBar _pBar;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\..\..\gui\Pages\PScanner.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock lProgress;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\..\..\gui\Pages\PScanner.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnScan;
        
        #line default
        #line hidden
        
        
        #line 50 "..\..\..\..\gui\Pages\PScanner.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnSaveMovies;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\..\..\gui\Pages\PScanner.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal FirstFloor.ModernUI.Windows.Controls.ModernProgressRing _LoaderGif;
        
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
            System.Uri resourceLocater = new System.Uri("/MCP;component/gui/pages/pscanner.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\gui\Pages\PScanner.xaml"
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
            this._grid = ((System.Windows.Controls.Grid)(target));
            return;
            case 2:
            this._cProgress = ((System.Windows.Controls.RowDefinition)(target));
            return;
            case 3:
            this._tab = ((System.Windows.Controls.TabControl)(target));
            return;
            case 4:
            this._pBar = ((System.Windows.Controls.ProgressBar)(target));
            return;
            case 5:
            this.lProgress = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.BtnScan = ((System.Windows.Controls.Button)(target));
            
            #line 43 "..\..\..\..\gui\Pages\PScanner.xaml"
            this.BtnScan.Click += new System.Windows.RoutedEventHandler(this.BtnScan_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.BtnSaveMovies = ((System.Windows.Controls.Button)(target));
            
            #line 50 "..\..\..\..\gui\Pages\PScanner.xaml"
            this.BtnSaveMovies.Click += new System.Windows.RoutedEventHandler(this.BtnSaveMovies_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this._LoaderGif = ((FirstFloor.ModernUI.Windows.Controls.ModernProgressRing)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

