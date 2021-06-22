using System;
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
using ApexLogic.Anatomics;
using ApexLogic;

namespace ApexGUI.UCs.Anatomics
{
    /// <summary>
    /// Interaction logic for Hand.xaml
    /// </summary>
    public partial class Hand : UserControl
    {
        Master Master => (App.Current as App).Master;
        ApexLogic.Anatomics.Hand ThisHand;
        public Hand()
        {
            InitializeComponent();
        }

        public void SetHand(ApexLogic.Anatomics.Hand hand)
        {
            ThisHand = hand;
            DrawHand();
        }

        public void AutoPlace()
        {

        }

        public void DrawHand()
        {
            HandCanvas.Children.Clear();
            foreach (KeyValuePair<int, ApexLogic.Anatomics.Finger> aFinger in ThisHand.Fingers)
            {
                foreach(KeyValuePair<int, ApexLogic.Anatomics.Joint> aJoint in aFinger.Value.FingerJoints)
                {

                }
            }
        }
    }
}
