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
using ApexGUI.UCs.Anatomics;
using ApexLogic;
using ApexLogic.Anatomics;

namespace ApexGUI.UCs
{
    /// <summary>
    /// Interaction logic for InputDataViewer.xaml
    /// </summary>
    public partial class InputDataViewer : UserControl
    {
        Master Master => (App.Current as App).Master;
        public InputDataViewer()
        {
            InitializeComponent();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Hand ActiveHand;
            if (HandPicker.SelectedIndex == 0)
                ActiveHand = Master.RightHand;
            else
                ActiveHand = Master.LefttHand;

            RightHand = new DrawnHand(ActiveHand);
            RightHand.AddJoint();
        }
    }
}