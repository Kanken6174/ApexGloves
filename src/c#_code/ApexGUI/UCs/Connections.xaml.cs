using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

namespace ApexGUI.UCs
{
    /// <summary>
    /// Interaction logic for Connections.xaml
    /// </summary>
    public partial class Connections : UserControl
    {
        public MainTabViewer MyParent;
        Master Master => (App.Current as App).Master;
        public Connections()
        {
            InitializeComponent();
            FindGloves();
        }

        private void FindGloves()
        {
            bool foundR = false;
            bool foundL = false;
            CBOX_RIGHTCOM.Items.Clear();
            CBOX_RIGHTCOM.Items.Add("None");
            CBOX_LEFTCOM.Items.Clear();
            CBOX_LEFTCOM.Items.Add("None");
            foreach (string str in Master.ValidPorts)
            {
                switch (str[0])
                {
                    case 'R':
                        CBOX_RIGHTCOM.Items.Add(str[1..]);
                        foundR = true;
                        break;
                    case 'L':
                        CBOX_LEFTCOM.Items.Add(str[1..]);
                        foundL = true;
                        break;
                    default:
                        break;
                }
            }
            if (foundR)
            {
                CBOX_RIGHTCOM.BorderBrush = Brushes.Green;
                CBOX_RIGHTCOM.SelectedIndex = 1;
            }
            else
                CBOX_RIGHTCOM.BorderBrush = Brushes.Transparent;
            if (foundL)
                CBOX_LEFTCOM.BorderBrush = Brushes.Green;
            else
                CBOX_LEFTCOM.BorderBrush = Brushes.Transparent;

            BCR.IsEnabled = foundR;
            BCL.IsEnabled = foundL;
            BCALL.IsEnabled = (foundL || foundR);
        }

        private void BCR_Click(object sender, RoutedEventArgs e)
        {
            if (CBOX_RIGHTCOM.SelectedItem != null || CBOX_RIGHTCOM.SelectedItem.ToString() != "None")
                Master.ToConnectR = CBOX_RIGHTCOM.SelectedItem.ToString();
            else
                Master.ToConnectR = null;

            ToDelimitors();
        }

        private void BCL_Click(object sender, RoutedEventArgs e)
        {
            if (CBOX_LEFTCOM.SelectedItem != null || CBOX_LEFTCOM.SelectedItem.ToString() != "None")
                Master.ToConnectL = CBOX_LEFTCOM.SelectedItem.ToString();
            else
                Master.ToConnectL = null;

            ToDelimitors();
        }

        private void BCALL_Click(object sender, RoutedEventArgs e)
        {
            BCR_Click(sender, e);
            BCL_Click(sender, e);
            ToDelimitors();
        }

        private void ToDelimitors()
        {
            if(MyParent is not null)
                MyParent.Tabber.SelectedIndex = 1;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FindGloves();
        }
    }
}
