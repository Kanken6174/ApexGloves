using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
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
using ApexLogic.COMMasters;
using ApexLogic;
using ApexLogic.Utilities;
using ApexLogic.DataFormats;

namespace ApexGUI.UCs
{
    /// <summary>
    /// Interaction logic for Delimitors.xaml
    /// </summary>
    public partial class Delimitors : UserControl
    {
        public SortedList<int,Button> MyButtons = new();
        List<InputData> InheritedTypes = ReflectiveEnumerator.GetEnumerableOfType<InputData>().ToList();    //This method is a blessing.
        Master Master => (App.Current as App).Master;
        string CurrentSnap = "";
        public List<string> InputTypes = new();
        public Delimitors()
        {
            InitializeComponent();
            foreach(InputData ID in InheritedTypes)
            {
                Inputs.Items.Add(ID.GetType().Name);
            }
        }

        public void ReadFromGloveAndSetupDelimitors()
        {
            string strBump = "";
            CurrentSnap = COMConnectAndTalk.ConnectAndReadOnceFrom(Master.ToConnectR);
            Wrappy.Children.Clear();
            int i = 0;
            MyButtons.Clear();
            if (CurrentSnap is not null)
            {
                foreach (char c in CurrentSnap)
                {
                    if (char.IsLetter(c))
                    {
                        if (strBump != "" && i != 0)
                        {
                            Button btnBump = new();
                            btnBump.Content = strBump;
                            btnBump.Tag = (i - strBump.Length) + 1;
                            btnBump.IsEnabled = false;
                            Wrappy.Children.Add(btnBump);
                            strBump = "";
                        }
                        Button btn = new();
                        btn.Content = c;
                        btn.Click += BCR_Click;
                        btn.Tag = i;
                        Wrappy.Children.Add(btn);
                        MyButtons.Add(i, btn);
                    }
                    else
                    {
                        if (c != ' ')
                            strBump += c;
                    }
                    i++;
                }
                Button bt = new();
                bt.Content = strBump;
                bt.Tag = (i - strBump.Length) + 1;
                bt.IsEnabled = false;
                Wrappy.Children.Add(bt);
            }
        }

        private void BCR_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                Button ic = (Button)sender;
                Button thisBtn = MyButtons[Int32.Parse(ic.Tag.ToString())];
                thisBtn.BorderBrush = Brushes.Green;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ReadFromGloveAndSetupDelimitors();
        }
    }
}
