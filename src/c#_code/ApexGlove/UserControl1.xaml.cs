using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO.Ports;
using System.Linq;



namespace ApexGlove
{
    /// <summary>
    /// Logique d'interaction pour UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        private SerialPort _serialPortL = new SerialPort();
        private SerialPort _serialPortR = new SerialPort();
        public String dataBuf;
        string serialDataIn;
        int[] indexes = new int[12];     //index of the sensor value in the received serial buffer
        string[] Sensordata = new string[12]; //actual value measuered by the sensor
        static readonly char[] indexAlph = new[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L' };//data indexes used by the arduino nano's serial code
        static readonly String[] baudRates = new[] { "38400" };
        static int TTW = 50;
        int[] oldR = new int[6] { 0, 0, 0, 0, 0 ,0};
        int[] oldL = new int[6] { 0, 0, 0, 0, 0 ,0};
        public bool runTasks = true;

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

            _serialPortL.Handshake = Handshake.None;    //Is this safe? Probably...
            _serialPortR.Handshake = Handshake.None;
            _serialPortL.DtrEnable = true;
            _serialPortR.DtrEnable = false; //utilisé pour idehntifier les ports, mauvais mais ça marche

            this._serialPortL.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort_DataReceived);
            this._serialPortR.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort_DataReceived);
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
                if (Selected != null)
                {
                    _serialPortL.PortName = Selected;
                    _serialPortL.BaudRate = Convert.ToInt32(comboBox_baudRate.Items.CurrentItem.ToString());
                    _serialPortL.Open();
                }

                Selected = (String)comboBox_comPortR.SelectedItem;
                if (Selected != null)
                {
                    _serialPortR.PortName = Selected;
                    _serialPortR.BaudRate = Convert.ToInt32(comboBox_baudRateR.Items.CurrentItem.ToString());
                    _serialPortR.Open();
                }

                if (_serialPortL.IsOpen && _serialPortL.IsOpen)
                {
                    fillUpStatus("green");
                    button_open.IsEnabled = false;
                    button_close.IsEnabled = true;
                    req_button.IsEnabled = true;
                    COMID.Text = _serialPortL.PortName;
                }
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
                _serialPortL.Close();

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
                comboBox_comPortR.Items.Clear();
                comboBox_comPort.ItemsSource = portLists;
                comboBox_comPortR.ItemsSource = portLists;
            }
            catch (System.InvalidOperationException)
            {

            }
        }

        private void comboBox_baudRate_DropDown(object sender, EventArgs e)
        {
            try
            {
                comboBox_baudRateR.Items.Clear();
                comboBox_baudRateR.ItemsSource = baudRates;
            }
            catch (System.InvalidOperationException)
            {

            }
        }

        private void requestData(object sender, EventArgs e)
        {
            Task.Run(new Action(async () =>
            {
               
                do
                {
                    await Task.Delay(TTW);
                    _serialPortL.Write("#");
                    _serialPortR.Write("#");

                } while (runTasks);
            }));
        }



        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
          /*  try
            {*/
                SerialPort SpIn = (SerialPort)sender;
                serialDataIn = SpIn.ReadLine();
                Application.Current.Dispatcher.Invoke(new Action(() => { COMDATA.Text = serialDataIn; }));
                ProcessData(sender, e);
            //}
            /*catch (Exception err)
            {
                MessageBox.Show("DataReceivedException " + err.Message);
            }*/
        }

        private void ProcessData(object sender, EventArgs e)
        {
            try
            {
                bool wrong = false;
                SerialPort SpIn = (SerialPort)sender;

                for (int i = 0; i <= 11; i++)
                {
                    char temp = indexAlph[i];
                        indexes[i] = serialDataIn.IndexOf(temp);
                        if(indexes[i] == -1)
                        {
                        wrong = true; //will be discarded
                        break;
                        }
                }
                if (!wrong)
                {
                    for (int i = 1; i <= 11; i++)
                    {
                        Sensordata[i - 1] = serialDataIn.Substring(indexes[i - 1] + 1, (indexes[i] - indexes[i - 1]) - 1);
                    }
                    Sensordata[10] = serialDataIn.Substring(indexes[10] + 1, (indexes[11] - indexes[10]) - 1);
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            Glove glove = new Glove(SpIn.DtrEnable);
                            for (int i = 0; i < 11; i++)
                            {
                                glove.values[i] = Int32.Parse(Sensordata[i]);
                            }

                            if (glove.rightHand)
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    if (glove.values[i] >= 1020 || glove.values[i] == 0)
                                    {
                                        glove.values[i] = oldR[i];
                                    }
                                }

                                pinkieRight.Value = glove.values[0];
                                ringRight.Value = glove.values[1];
                                middleRight.Value = glove.values[2];
                                indexRight.Value = glove.values[3];
                                thumbYRight.Value = glove.values[4];
                                thumbXRight.Value = glove.values[5];

                                int temp = (glove.values[7] / 750) + 30;//y axis
                            targetR.SetValue(Canvas.LeftProperty, (double)temp);
                                temp = (glove.values[6] / 750) + 40;//x axis
                            targetR.SetValue(Canvas.TopProperty, (double)temp);
                                for (int i = 0; i < 6; i++)
                                {
                                    oldR[i] = glove.values[i];
                                }
                            }
                            else
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    if (glove.values[i] >= 1020 || glove.values[i] == 0)
                                    {
                                        glove.values[i] = oldL[i];
                                    }
                                }

                                pinkieLeft.Value = glove.values[0];
                                ringLeft.Value = glove.values[1];
                                middleLeft.Value = glove.values[2];
                                indexLeft.Value = glove.values[3];
                                thumbYLeft.Value = glove.values[4];
                                thumbXLeft.Value = glove.values[5];

                                int temp = (glove.values[7] / 750) + 30;//y axis
                            targetL.SetValue(Canvas.LeftProperty, (double)temp);
                                temp = (glove.values[6] / 750) + 40;//x axis
                            targetL.SetValue(Canvas.TopProperty, (double)temp);
                                for (int i = 0; i < 6; i++)
                                {
                                    oldL[i] = glove.values[i];
                                }
                            }
                        }));
                }
                else
                {
                    wrong = false;
                    SpIn.Write("?");
                }


            }


            catch (System.FormatException)
            {
                Application.Current.Dispatcher.Invoke(new Action(() => { COMID.Text = "Arduino crashed"; }));
            }

        }

        private void Calibrate_UP(object sender, RoutedEventArgs e)
        {
            _serialPortL.Write("#");
        }

    }
    public class TextboxText
    {
        public string serialDataIn { get; set; }

    }
}