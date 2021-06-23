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
    public partial class DrawnHand : UserControl
    {

        Master Master => (App.Current as App).Master;
        Hand ThisHand;
        int joints = 5;

        public DrawnHand()
        {
            InitializeComponent();
        }

        public DrawnHand(Hand hand)
        {
            ThisHand = hand;
            InitializeComponent();
        }

        public void SetHand(ApexLogic.Anatomics.Hand hand)
        {
            ThisHand = hand;
        }



        public async void AutoPlace()
        {
            for(int i = joints -1; i>=0; i--)
            {
                for (int u = 0; u < ((i == 0) ? 2 : 3); u++)
                {
                    await Task.Delay(200);
                    PlaceJoint("Root" + i);
                }
            }
        }

        public void AddJoint()
        {
            AutoPlace();
        }

        private void PlaceJoint(string RootName)
        {
            DrawnJoint DJ = new();
            DJ.Tag = null;
            FrameworkElement root = HandCanvas.FindName(RootName) as Ellipse;
            root = TravelRootToEnd(root, out int i);
            root.Tag = RootName+i;
            double X = Canvas.GetLeft(root) - (i!=0 ? 0 : root.ActualWidth/4);
            double Y = Canvas.GetTop(root) - (i!=0 ? root.ActualHeight/1.3 : (root.ActualHeight*2.5));
            Canvas.SetLeft(DJ, X);
            Canvas.SetTop(DJ, Y);
            HandCanvas.Children.Add(DJ);
            HandCanvas.RegisterName(root.Tag.ToString(), DJ);
        }

        private FrameworkElement TravelRootToEnd(FrameworkElement root, out int index)
        {
            index = 0;
            while (root.Tag != null)
            {
                index++;
                root = (FrameworkElement)HandCanvas.FindName(root.Tag.ToString());
            }
            return root;
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
