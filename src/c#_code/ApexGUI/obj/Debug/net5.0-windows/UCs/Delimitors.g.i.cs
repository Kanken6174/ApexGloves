﻿#pragma checksum "..\..\..\..\UCs\Delimitors.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "9195DB269A3EE2EF91DEBE7666C8E814EBD93988"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using ApexGUI.UCs;
using ApexGUI.UCs.DelimitorModels;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
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


namespace ApexGUI.UCs {
    
    
    /// <summary>
    /// Delimitors
    /// </summary>
    public partial class Delimitors : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 14 "..\..\..\..\UCs\Delimitors.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.WrapPanel Wrappy;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\..\UCs\Delimitors.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox SourceCOMBOX;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\..\..\UCs\Delimitors.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox Inputs;
        
        #line default
        #line hidden
        
        
        #line 68 "..\..\..\..\UCs\Delimitors.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox DescritpionBox;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "5.0.6.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/ApexGUI;V1.0.0.0;component/ucs/delimitors.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\UCs\Delimitors.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "5.0.6.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.Wrappy = ((System.Windows.Controls.WrapPanel)(target));
            return;
            case 2:
            this.SourceCOMBOX = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 3:
            
            #line 30 "..\..\..\..\UCs\Delimitors.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.Inputs = ((System.Windows.Controls.ComboBox)(target));
            
            #line 48 "..\..\..\..\UCs\Delimitors.xaml"
            this.Inputs.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.Inputs_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 5:
            this.DescritpionBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 69 "..\..\..\..\UCs\Delimitors.xaml"
            this.DescritpionBox.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.DescritpionBox_TextChanged);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

