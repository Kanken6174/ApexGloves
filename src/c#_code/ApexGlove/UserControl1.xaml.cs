using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO.Ports;
using System.Linq;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.IO.Pipes;
using System.IO;



namespace ApexGlove
{
    /// <summary>
    /// Logique d'interaction pour UserControl1.xaml
    /// </summary>
    enum UI
    {
        pinkieRight = 0,    //0
        ringRight = 1,      //1
        middleRight = 2,    //2
        indexRight = 3,     //...
        thumbYRight = 4,
        thumbXRight = 5,    //5
        pinkieLeft = 6,     //6
        ringLeft = 7,
        middleLeft = 8,
        indexLeft = 9,
        thumbYLeft = 10,
        thumbXLeft = 11   //11
    }
    public partial class UserControl1 : UserControl
    {
        private SerialPort _serialPortL = new SerialPort();
        private SerialPort _serialPortR = new SerialPort();
        public String dataBuf;
        int[] indexes = new int[12];     //index of the sensor value in the received serial buffer
        string[] Sensordata = new string[12]; //actual value measuered by the sensor
        static readonly char[] indexAlph = new[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L' };//data indexes used by the arduino nano's serial code
        static readonly String[] baudRates = new[] { "38400" };
        static int TTW = 50;    //time elapsed between each data request
        static int[,] oldR = new int[6, 4];
        static int[,] oldL = new int[6, 4];
        public bool runTasks = true;
        public bool RranOnce = false;
        public bool LranOnce = false;



        public struct Glove //data structure for a full Apex glove
        {
            public int[] values;
            public bool rightHand;
            public Glove(bool rightHand)
            {
                this.values = new int[11] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                this.rightHand = rightHand;
            }
        }
        public static int[,] setters = new int[,] { { 1023, 1023, 1023, 1023, 1023, 1023, 1023, 1023, 1023, 1023, 1023 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } }; //0- max 1- min; 0-5 left , 6-11 right


        public UserControl1()
        {
            InitializeComponent();

            for (int i = 0; i < 6; i++)
                for (int n = 0; n < 4; n++)
                {
                    oldL[i, n] = 500;
                    oldR[i, n] = 500;
                }
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



        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)  //fired upon data recevied on port
        {

            SerialPort SpIn = (SerialPort)sender;
            String serialDataIn = SpIn.ReadLine();
            Application.Current.Dispatcher.Invoke(new Action(() => { COMDATA.Text = serialDataIn; }));
            ProcessData(sender, e, serialDataIn);

        }

        private void ProcessData(object sender, EventArgs e, string serialDataIn)
        {
            try
            {
                bool wrong = false;
                SerialPort SpIn = (SerialPort)sender;

                for (int i = 0; i <= 11; i++)
                {
                    char temp = indexAlph[i];
                    indexes[i] = serialDataIn.IndexOf(temp);
                    if (indexes[i] == -1)
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

                    //---------------------------------------------------------------Thread change (to UI thread)
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            Glove glove = new Glove(SpIn.DtrEnable);
                            for (int i = 0; i < 11; i++)
                            {
                                glove.values[i] = Int32.Parse(Sensordata[i]);
                            }

                            if (glove.rightHand)
                            {
                                if (RranOnce)
                                    for (int i = 0; i < 5; i++)
                                        for (int n = 1; n < 3; n++)
                                            if (shortDetected(glove, oldR, i) || incorrectSmoothing(glove, oldR, i))
                                                glove.values[i] = oldR[i, 0];
                                            else
                                                RranOnce = true;

                                for (int i = 0; i < 5; i++)
                                {
                                    glove.values[i] -= setters[1, i];
                                /*    if (maxes[i, 1] < glove.values[i]) glove.values[i] = maxes[1, i];
                                    if (mins[i, 1] > glove.values[i]) glove.values[i] = mins[1, i];*/
                                }

                                //Saves finger values to the UI
                                for (var z = 0; z < 6; z++)
                                {
                                    ((ProgressBar)this.FindName(((UI)z).ToString())).Value = glove.values[z];
                                }

                                //Save gyro/accelerometer values to the UI
                                int tempX = (glove.values[7] / 400) + 30;//y axis
                                //targetR.SetValue(Canvas.LeftProperty, (double)tempY);
                                int tempY = (glove.values[6] / 400) + 40;//x axis
                                //targetR.SetValue(Canvas.TopProperty, (double)tempX);
                                int tempZ = (glove.values[8] / 400) + 30;//z axis

                                rotate(true, tempX, tempY, tempZ);

                                for (int i = 0; i < 6; i++)
                                {
                                    for (int n = 0; n < 3; n++)
                                    {
                                        oldR[i, n + 1] = oldR[i, n];  //saves older values for averages
                                    }
                                    oldR[i, 0] = glove.values[i];
                                }
                            }
                            else
                            {
                                if (LranOnce)
                                    for (int i = 0; i < 5; i++)
                                        for (int n = 1; n < 3; n++)
                                            if (shortDetected(glove, oldL, i) || incorrectSmoothing(glove, oldL, i))
                                            {
                                                glove.values[i] = oldL[i, 0];
                                            }
                                            else
                                                RranOnce = true;

                                for (int i = 6; i < 10; i++)
                                {
                                    glove.values[i] -= setters[1, i];
                                   /* if (maxes[i, 0] < glove.values[i]) glove.values[i] = maxes[i, 0];
                                    if (mins[i, 0] > glove.values[i]) glove.values[i] = mins[i, 0];*/
                                }
                                //Saves finger values to the UI
                                for (var z = 0; z < 6; z++)
                                {
                                    ((ProgressBar)this.FindName(((UI)(z+6)).ToString())).Value = glove.values[z];
                                }

                                //Save gyro/accelerometer values to the UI
                                int tempY = (glove.values[6] / 400);//x axis
                                Yb.Text = tempY.ToString();
                                // targetL.SetValue(Canvas.TopProperty, (double)tempX);
                                int tempX = (glove.values[7] / 400);//y axis
                                Xb.Text = tempX.ToString();
                                // targetL.SetValue(Canvas.LeftProperty, (double)tempY);
                                int tempZ = (glove.values[8] / 400);//z axis
                                Zb.Text = tempZ.ToString();

                                rotate(false, tempX, tempY, tempZ);

                                for (int i = 0; i < 6; i++)
                                {
                                    for (int n = 0; n < 3; n++)
                                    {
                                        oldL[i, n + 1] = oldL[i, n];  //saves older vaules for averages
                                    }
                                    oldL[i, 0] = glove.values[i];
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

        private bool shortDetected(Glove glove, int[,] old, int i)
        {
            return (glove.values[i] >= 1023 || glove.values[i] <= 0);
        }

        private bool incorrectSmoothing(Glove glove, int[,] old, int i)
        {
            return (glove.values[i] > old[i, 0] + 100 || glove.values[i] < old[i, 0] - 100);
        }

        private void Cal_left(object sender, RoutedEventArgs e)
        {
            LranOnce = false; //calibration will be set at next query
            setters = new int[,] { { 1023, 1023, 1023, 1023, 1023, 1023, 1023, 1023, 1023, 1023, 1023 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } };

            for(int i = 0; i < 5; i++)
            {
                ((ProgressBar)this.FindName(((UI)(i)).ToString())).Maximum = 1023;
            }
        }

        private void Cal_right(object sender, RoutedEventArgs e)
        {
            RranOnce = false; //calibration will be set at next query
            setters = new int[,] { { 1023, 1023, 1023, 1023, 1023, 1023, 1023, 1023, 1023, 1023, 1023 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } };
            for (int i = 6; i < 11; i++)
            {
                ((ProgressBar)this.FindName(((UI)(i)).ToString())).Maximum = 1023;
            }
        } 

        private void rotate(bool right, double x, double y, double z)
        {

            if (right)
            {
                RX.Angle = x;
                RY.Angle = y;
                RZ.Angle = z;
            }
            else
            {
                LX.Angle = x;
                LY.Angle = y;
                LZ.Angle = z;
            }
        }

        private void CalibrateTÄndBs(object sender, RoutedEventArgs e)
        {
            int c, w;
            bool min;

            if (sender.Equals(CTL))
            {
                c = 6;  //number to start FOR
                w = 11;  //number to end FOR
                min = false;
            }
            else if (sender.Equals(CTR))
            {
                c = 0;
                w = 5;
                min = false;
            }
            else if (sender.Equals(CBL))
            {
                c = 6;
                w = 11;
                min = true;
            }
            else
            {
                c = 0;
                w = 5;
                min = true;
            }

            for (int i = c; i < w; i++)
            {
                var enumUI = (UI)i;
                string UIname = enumUI.ToString();
                if (min)
                {
                    setters[1, i] = (int)(((ProgressBar)this.FindName(UIname)).Value);
                    ((ProgressBar)this.FindName(UIname)).Maximum = setters[1, i]+100;
                }
                else
                {
                    setters[0, i] = (int)(((ProgressBar)this.FindName(UIname)).Value);
                }
            }

        }   //Bs stands for bottoms, not whatever you were thinking, this sets the mins and maxes

        public void sendToDriver(int[] arr)
        {
            string strok = "";

            for(int i = 0; i <= arr.Length && i <= indexAlph.Length; i++)
            {
                strok += indexAlph[i];
                strok += arr[i];
            }
            var toDriver = new NamedPipeServerStream("my-very-cool-pipe-example", PipeDirection.InOut, 1, PipeTransmissionMode.Byte);
            var streamToDriver = new StreamReader(toDriver);

            toDriver.WaitForConnection();

            var writer = new StreamWriter(toDriver);
            writer.Write(arr);
            writer.Write((char)0);
            writer.Flush();
            toDriver.WaitForPipeDrain();
            
            string returned = streamToDriver.ReadLine();

            if (returned != "ok")
            {
                //error handling
            }

            toDriver.Dispose();
        }
    }
    public class TextboxText
    {
        public string serialDataIn { get; set; }

    }
}