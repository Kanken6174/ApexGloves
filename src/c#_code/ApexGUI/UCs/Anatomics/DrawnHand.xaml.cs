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
using System.Runtime.CompilerServices;

namespace ApexGUI.UCs.Anatomics
{


    /// <summary>
    /// Interaction logic for Hand.xaml
    /// </summary>
    public partial class DrawnHand : UserControl
    {

        Master Master => (App.Current as App).Master;
        public Hand ThisHand;
        private bool drawn = false;
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

        

        public async void AutoPlace()
        {
            for(int i = joints -1; i>=0; i--)
            {
                for (int u = 0; u < ((i == 0) ? 2 : 3); u++)
                {
                    await Task.Delay(20);
                    PlaceJoint("Root" + i);
                }
            }
        }

        public void Draw()
        {
            if(!drawn)
                AutoPlace();

            drawn = true;
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

        public void UpdateAll()
        {
            for (int i = joints - 1; i >= 0; i--)
            {
                string RootName = $"Root{i}";
                FrameworkElement root = HandCanvas.FindName(RootName) as Ellipse;
                DrawnJoint rootChild = (DrawnJoint)HandCanvas.FindName(root.Tag.ToString());
                rootChild.UpdateLength();
                while (rootChild.Tag != null)
                {
                    rootChild = (DrawnJoint)HandCanvas.FindName(rootChild.Tag.ToString());
                    rootChild.UpdateLength();
                }
            }
        }

    }
}
