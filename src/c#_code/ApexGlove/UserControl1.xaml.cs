using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO.Ports;



namespace ApexGlove
{
    /// <summary>
    /// Logique d'interaction pour UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        private SerialPort _serialPort = new SerialPort();
        public String dataBuf;
        string serialDataIn;
        int[] indexes = new int[12];     //index of the sensor value in the received serial buffer
        string[] Sensordata = new string[12]; //actual value measuered by the sensor
        static readonly char[] indexAlph = new[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L' };//data indexes used by the arduino nano's serial code
        static readonly String[] baudRates = new[] { "38400" };

        public struct Glove //data structure for a full Apex glove
        {
            public int[] values;
            public int[] maxes;
            public int[] mins;
            public bool rightHand;
            public Glove(bool rightHand)
            {
                this.values = new int[11] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                this.maxes = new int[11] { 1023, 1023, 1023, 1023, 1023, 1023, 1023, 1023, 1023, 1023, 1023 };
                this.mins = new int[11] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                this.rightHand = rightHand;
            }
        }



        public UserControl1()
        {
            InitializeComponent();
            COMDATA.DataContext = new TextboxText() { serialDataIn = "" };
            _serialPort.Handshake = Handshake.None;
            this._serialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox_baudRate.ItemsSource = baudRates;
        }

        public void fillUpStatus(String str)
        {
            Color color = (Color)ColorConverter.ConvertFromString(str);
            SolidColorBrush brush = new SolidColorBrush(color);
            StatusElipse.Fill = brush;
        }

        private void button_open_Click(object sender, EventArgs e)
        {
            try
            {
                String Selected;
                //Selected = comboBox_comPort.Items.CurrentItem.ToString();
                Selected = (String)comboBox_comPort.SelectedItem;

                _serialPort.PortName = Selected;
                _serialPort.BaudRate = Convert.ToInt32(comboBox_baudRate.Items.CurrentItem.ToString());
                _serialPort.Open();

                fillUpStatus("green");
                button_open.IsEnabled = false;
                button_close.IsEnabled = true;
                req_button.IsEnabled = true;
                COMID.Text = _serialPort.PortName;
            }
            catch (System.NullReferenceException)
            {
                fillUpStatus("yellow");
                COMDATA.Text = "catched";
                button_open.IsEnabled = true;
                button_close.IsEnabled = false;
                req_button.IsEnabled = false;
            }
            catch (System.IO.IOException)
            {
                MessageBox.Show("Impossible de se connecter au port COM, timeout ou non-disponible.");
                comboBox_comPort.SelectedItem = null;
            }
        }

        private void button_close_Click(object sender, EventArgs e)
        {
            try
            {
                _serialPort.Close();

                button_open.IsEnabled = true;
                button_close.IsEnabled = false;
                req_button.IsEnabled = false;
                fillUpStatus("red");
                COMID.Text = "";
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void comboBox_comPort_DropDown(object sender, EventArgs e)
        {
            try
            {
                string[] portLists = SerialPort.GetPortNames();
                comboBox_comPort.Items.Clear();
                comboBox_comPort.ItemsSource = portLists;
            }
            catch (System.InvalidOperationException)
            {

            }
        }

        private void comboBox_baudRate_DropDown(object sender, EventArgs e)
        {
        }

        private void requestData(object sender, EventArgs e)
        {
            Task.Run(new Action(async () => { 
                bool isChecked = true;
                do { await Task.Delay(50); _serialPort.Write("#");
                     
                }while (isChecked) ;
            }));
        }



    private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            SerialPort SpIn = (SerialPort)sender;
            serialDataIn = SpIn.ReadLine();
            Application.Current.Dispatcher.Invoke(new Action(() => { COMDATA.Text = serialDataIn; }));
            ProcessData(sender, e);
        }
        catch (Exception err)
        {
            MessageBox.Show("DataReceivedException " + err.Message);
        }
    }

    private void ProcessData(object sender, EventArgs e)
    {
        try
        {

            for (int i = 0; i <= 11; i++)
            {
                char temp = indexAlph[i];
                indexes[i] = serialDataIn.IndexOf(temp);
            }

            for (int i = 1; i <= 11; i++)
            {
                Sensordata[i - 1] = serialDataIn.Substring(indexes[i - 1] + 1, (indexes[i] - indexes[i - 1]) - 1);
            }
            Sensordata[10] = serialDataIn.Substring(indexes[10] + 1, (indexes[11] - indexes[10]) - 1);
            Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    Glove ungant = new Glove(false);
                    for (int i = 0; i < 11; i++)
                    {
                        ungant.values[i] = Int32.Parse(Sensordata[i]);
                    }

                /*for (int i = 0; i <= 11; i++)
                {
                    ungant.values[i] -= ungant.mins[i];
                    if (ungant.values[i] > 1023) ungant.values[i] = 1023;
                }*/

                    pinkieLeft.Value = ungant.values[0];
                    ringLeft.Value = ungant.values[1];
                    middleLeft.Value = ungant.values[2];
                    indexLeft.Value = ungant.values[3];
                    thumbYLeft.Value = ungant.values[4];
                    thumbXLeft.Value = ungant.values[5];

                    int temp = (ungant.values[7] / 750) + 30;//y axis
                    targetL.SetValue(Canvas.LeftProperty, (double)temp);
                    temp = (ungant.values[6] / 750) + 40;//x axis
                    targetL.SetValue(Canvas.TopProperty, (double)temp);
                }));


        }
        catch (Exception error)
        {
            MessageBox.Show("ProcessDataException " + error.Message);
        }
    }


}
public class TextboxText
{
    public string serialDataIn { get; set; }

}
}