﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ApexLogic;
using ApexLogic.Anatomics;

namespace ApexGUI.UCs.Anatomics
{
    /// <summary>
    /// Interaction logic for Joint.xaml
    /// </summary>
    public partial class DrawnJoint : UserControl
    {
        public Joint joint;
        public DrawnJoint()
        {
            InitializeComponent();
        }
        public DrawnJoint(Joint joint)
        {
            this.joint = joint;
            InitializeComponent();
        }
    }
}