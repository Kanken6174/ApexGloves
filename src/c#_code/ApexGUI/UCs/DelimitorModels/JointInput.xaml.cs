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
using ApexLogic;
using ApexLogic.DataFormats;
using ApexLogic.Anatomics;
using ApexLogic.Delimiters;

namespace ApexGUI.UCs.DelimitorModels
{
    /// <summary>
    /// Interaction logic for JointInput.xaml
    /// </summary>
    public partial class JointInput : UserControl
    {
        public Delimiter Delimiter;
        Master Master => (App.Current as App).Master;
        public JointInput(Hand hand, char Indexer)
        {
            Delimiter = new(Indexer);
            
            InitializeComponent();
            hand.MyDelimiters.Add(Delimiter);
        }
    }
}
