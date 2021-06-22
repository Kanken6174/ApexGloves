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
using ApexLogic.Anatomics;
using ApexLogic.Delimiters;

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
        public Delimiter ActiveDel = new(' ');
        private Hand ActiveHand;
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
            if(SourceCOMBOX.SelectedIndex == 0)
                CurrentSnap = COMConnectAndTalk.ConnectAndReadOnceFrom(Master.ToConnectR);
            else
                CurrentSnap = COMConnectAndTalk.ConnectAndReadOnceFrom(Master.ToConnectL);
            Wrappy.Children.Clear();
            int i = 0, u = 0;
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
                        btn.Tag = u;
                        Wrappy.Children.Add(btn);
                        MyButtons.Add(u, btn);
                        u++;
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
            if (SourceCOMBOX.SelectedIndex == 0)
                ActiveHand = Master.RightHand;
            else
                ActiveHand = Master.LefttHand;

            ActiveHand.MyDelimiters.Clear();

            foreach (Button btn in Wrappy.Children)
            {
                if (btn.IsEnabled)  //is a Delimiter index (a letter, not a value)
                {
                    Delimiter Del = new(btn.Content.ToString()[0]);
                    ActiveHand.MyDelimiters.Add(Del);
                }
            }
        }

        /// <summary>
        /// An Index has been clicked, display the corresponding delimiter and co
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BCR_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                Button ic = (Button)sender;
                ic.BorderBrush = Brushes.Black;
                ActiveDel = ActiveHand.MyDelimiters[Int32.Parse(ic.Tag.ToString())];
                ActiveDel.MySource ??= new();
                DescritpionBox.Text = ActiveDel.Description ??=new("Describe what this is here");
                string str = ActiveDel.MySource.GetType().Name;
                Inputs.SelectedItem = str;

            }
        }

        private void LoadOrNewDelimitor()
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ReadFromGloveAndSetupDelimitors();
        }

        private void DescritpionBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ActiveDel.Description = DescritpionBox.Text;
        }

        /// <summary>
        /// i'm getting all the types that inherit the "input" class on startup
        /// i put them in a List
        /// and i fill a listbox from that list
        /// then the user picks a type (Non-fullname so there's no trailing assembly name)
        /// i get the index from the listbox and instantiate the corresponding class from the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Inputs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(Inputs.SelectedValue != null)
            {
                ActiveDel.MySource = (InputData)Activator.CreateInstance((InheritedTypes[Inputs.SelectedIndex].GetType()));
            }
        }
    }
}
