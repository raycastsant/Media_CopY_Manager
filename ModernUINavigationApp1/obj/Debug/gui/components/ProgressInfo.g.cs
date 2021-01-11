﻿#pragma checksum "..\..\..\..\gui\components\ProgressInfo.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "A8EBCF8D805768FA46CDC70A5FC2D5DFDA9424AD69416BE75CB0859042D4DE7A"
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


namespace MCP.gui.components {
    
    
    /// <summary>
    /// ProgressInfo
    /// </summary>
    public partial class ProgressInfo : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 27 "..\..\..\..\gui\components\ProgressInfo.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ColumnDefinition _cCancelBtn;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\..\gui\components\ProgressInfo.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock _lPercentage;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\..\gui\components\ProgressInfo.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock _lProgressState;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\..\..\gui\components\ProgressInfo.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock _lFilePercent;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\..\..\gui\components\ProgressInfo.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal FirstFloor.ModernUI.Windows.Controls.ModernButton BtnPause;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\..\..\gui\components\ProgressInfo.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal FirstFloor.ModernUI.Windows.Controls.ModernButton BtnContinue;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\..\..\gui\components\ProgressInfo.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal FirstFloor.ModernUI.Windows.Controls.ModernButton BtnViewList;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\..\..\gui\components\ProgressInfo.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal FirstFloor.ModernUI.Windows.Controls.ModernButton BtnCancel;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\..\gui\components\ProgressInfo.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ProgressBar _ProgressBar;
        
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
            System.Uri resourceLocater = new System.Uri("/MCP;component/gui/components/progressinfo.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\gui\components\ProgressInfo.xaml"
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
            this._cCancelBtn = ((System.Windows.Controls.ColumnDefinition)(target));
            return;
            case 2:
            this._lPercentage = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this._lProgressState = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this._lFilePercent = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 5:
            this.BtnPause = ((FirstFloor.ModernUI.Windows.Controls.ModernButton)(target));
            
            #line 32 "..\..\..\..\gui\components\ProgressInfo.xaml"
            this.BtnPause.Click += new System.Windows.RoutedEventHandler(this.BtnPause_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.BtnContinue = ((FirstFloor.ModernUI.Windows.Controls.ModernButton)(target));
            
            #line 33 "..\..\..\..\gui\components\ProgressInfo.xaml"
            this.BtnContinue.Click += new System.Windows.RoutedEventHandler(this.BtnContinue_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.BtnViewList = ((FirstFloor.ModernUI.Windows.Controls.ModernButton)(target));
            
            #line 34 "..\..\..\..\gui\components\ProgressInfo.xaml"
            this.BtnViewList.Click += new System.Windows.RoutedEventHandler(this.BtnViewList_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.BtnCancel = ((FirstFloor.ModernUI.Windows.Controls.ModernButton)(target));
            
            #line 35 "..\..\..\..\gui\components\ProgressInfo.xaml"
            this.BtnCancel.Click += new System.Windows.RoutedEventHandler(this.BtnCancel_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this._ProgressBar = ((System.Windows.Controls.ProgressBar)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

