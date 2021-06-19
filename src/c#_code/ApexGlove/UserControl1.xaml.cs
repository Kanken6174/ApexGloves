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
using System.Windows.Shapes;
using System.Globalization;



namespace ApexGlove
{
    /// <summary>
    /// Logique d'interaction pour UserControl1.xaml
    /// </summary>
    enum UI     //give the x:Name of the progressBar on the UI
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

    enum finger_contacts    //give the x:Name for the ellipse of the finger's aluminium contacts
    {
        AindexRight = 0,    //0
        AmiddleRight = 1,      //1
        AringRight = 2,    //2
        ApinkieRight = 3,     //...

        AthumbRight = 4,
        ApinkieLeft = 5,     //6
        AringLeft = 6,
        AmiddleLeft = 7,
        AindexLeft = 8,
        AthumbLeft = 9,
    }


    public partial class UserControl1 : UserControl
    {
        private SerialPort _serialPortL = new SerialPort();
        private SerialPort _serialPortR = new SerialPort();
        public String dataBuf;
        int[] indexes = new int[25];     //index of the sensor value in the received serial buffer
        string[] Sensordata = new string[25]; //actual value measuered by the sensor
        static readonly char[] indexAlph = new[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
                                                   'N' , 'O' , 'P' , 'Q' , 'R' , 'S', 'T', 'U', 'V', 'W', 'Y', 'Z' };//data indexes used by the arduino nano's serial code

        static readonly String[] baudRates = new[] { "38400" };
        static int TTW = 50;    //time elapsed between each data request
        static int[,] oldR = new int[6, 4];
        static int[,] oldL = new int[6, 4];
        public bool runTasks = true;
        public bool RranOnce = false;
        public bool LranOnce = false;

        public struct MPU6050
        {
            public int gyro_X;  //12    M
            public int gyro_Y;  //13    N
            public int gyro_Z; //Z axis DOES NOT work properly on MPU 6050s. It will be ignored for now
            public int accel_X; //15    P
            public int accel_Y; //16    Q
            public int accel_Z; //17    R   also EOF (for glove V1.8)
            //int temperature; //useless so unused
            public MPU6050(int finalZ)
            {
                gyro_X = 0;
                gyro_Y = 0;
                gyro_Z = finalZ;
                accel_X = 0;
                accel_Y = 0;
                accel_Z = 0;
            }
        }

        public struct Glove //data structure for a full Apex glove
        {
            public int[] values;//0-5 [A-F] is articulations 

            public MPU6050 mpu;
            public int joyX;// 6 [G]
            public int joyY;// 7 [H]
            public bool[] contacts;//8-11 [I-L] is aluminium pads
            public bool rightHand;
            public bool joyBtn;
            public Glove(bool rightHand)
            {
                values = new int[11] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };   //i keep extras for later implementation of Y axis for all fingers
                contacts = new bool[] { false, false, false, false };
                joyX = 0;
                joyY = 0;
                joyBtn = false;
                this.rightHand = rightHand;
                mpu = new MPU6050(0);
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
                _serialPortR.Close();
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
                    if (_serialPortL.IsOpen) _serialPortL.Write("#");

                    if (_serialPortR.IsOpen) _serialPortR.Write("#");
                } while (runTasks);
            }));
        }



        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)  //fired upon data recevied on port
        {

            SerialPort SpIn = (SerialPort)sender;
            String serialDataIn = SpIn.ReadLine();
            serialDataIn.Replace("−", "-");
            serialDataIn.Replace(".", ",");
            Application.Current.Dispatcher.Invoke(new Action(() => { COMDATA.Text = serialDataIn; }));
            ProcessData(sender, e, serialDataIn);

        }

        private void ProcessData(object sender, EventArgs e, string serialDataIn)
        {
            try
            {
                bool wrong = false;     //hardware error handling without throwing an exception
                int EndOfStream = 0;    //Index at which the index letters can no longer be found (last 
                SerialPort SpIn = (SerialPort)sender;

                foreach (char c in indexAlph)    // sets EndOfStream and finds the indexes in the string
                {
                    int i = c - 65; //contains the position of the char c inside of the array indexAlph 
                    char temp = indexAlph[i];
                    indexes[i] = serialDataIn.IndexOf(temp);
                    if (indexes[i] == -1)    //if not found
                    {
                        if (i < 16)
                            wrong = true;
                        EndOfStream = i;    //EndOfStream contains the last index containing correct data
                        break;
                    }
                }
                if (!wrong)
                {
                    //this could be simplified by using  only LastIndexOf, and not locating the indexes beforehand. TO_FIX
                    for (int i = 1; i < EndOfStream; i++)
                    {
                        Sensordata[i - 1] = serialDataIn.Substring(indexes[i - 1] + 1, (indexes[i] - indexes[i - 1]) - 1);  //gets data between 2 indexes [ex : A|1234|B || being the data]
                    }
                    Sensordata[EndOfStream] = serialDataIn.Substring(serialDataIn.LastIndexOf(indexAlph[EndOfStream]) + 1);   //gets the very last bit of data (Z|1234 EOF)

                    //---------------------------------------------------------------Thread change (to UI thread)
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            bool truth = true;
                            Glove glove = new Glove(SpIn.DtrEnable);    //remember, DtrEnable is used to define if it's the right or left hand, horrendous but it works.
                            int[] values = new int[EndOfStream];
                            /*NumberStyles style;
                            CultureInfo culture;
                            style = NumberStyles.AllowDecimalPoint;
                            culture = CultureInfo.CreateSpecificCulture("fr-FR");*/
                            bool nega = false;

                            for (int i = 0; i < EndOfStream - 1; i++)
                            {
                                truth = Int32.TryParse(Sensordata[i], out values[i]);   //set it back to an int instead of char
                                if (!truth)
                                {
                                    double temp = 0;
                                    nega = (Sensordata[i].IndexOf("-") != -1) ? true : false;
                                 //   if (nega) Sensordata[i].Remove(0,1);
                                    truth = double.TryParse(Sensordata[i], NumberStyles.Number, CultureInfo.CreateSpecificCulture("en-US"), out temp);
                                 //   if (nega) temp *= -1;
                                    int rounded = Convert.ToInt32(temp);
                                    values[i] = rounded;
                                }
                            }

                            for (int i = 0; i < 6; i++)
                                glove.values[i] = values[i];

                            glove.joyX = values[6];
                            glove.joyY = values[7];

                            for (int i = 8; i < 12; i++)
                                glove.contacts[i - 8] = (values[i] == 1);

                            glove.mpu.gyro_X = values[12];
                            glove.mpu.gyro_Y = values[13];
                            glove.mpu.gyro_Z = values[14];

                            glove.mpu.accel_Y = values[15];
                            glove.mpu.accel_Z = values[16];

                            if (glove.rightHand)
                            {
                                if (RranOnce)
                                    for (int i = 0; i < 5; i++) // the 6 finger potentiometers
                                        if (shortDetected(glove, oldR, i) || incorrectSmoothing(glove, oldR, i))    //check if the values make sense, or discard them
                                            glove.values[i] = oldR[i, 0];
                                        else
                                            RranOnce = true;

                                for (int i = 0; i < 5; i++)
                                {
                                    glove.values[i] -= setters[1, i];   //susbtracts minimums
                                }

                                //Saves finger values to the UI
                                for (var z = 0; z < 6; z++)
                                {
                                    ((ProgressBar)this.FindName(((UI)z).ToString())).Value = glove.values[z];
                                }

                                //Save gyro/accelerometer values to the UI
                                int tempX = ((glove.mpu.gyro_X/2)*-1) + 30;//y axis
                                targetR.SetValue(Canvas.LeftProperty, (double)tempX);
                                int tempY = (glove.mpu.gyro_Y/2*-1) + 40;//x axis
                                targetR.SetValue(Canvas.TopProperty, (double)tempY);
                                int tempZ = -(glove.mpu.gyro_Z+180);
                                arrowRdir.Angle = (double)tempZ;


                                JoyR.SetValue(Canvas.LeftProperty, (double)((glove.joyY / 50) + 30));
                                JoyR.SetValue(Canvas.TopProperty, (double)((glove.joyX / 50) + 30));

                                for (int i = 0; i < 4; i++)
                                {
                                    String str = "Beige";
                                    if (glove.contacts[i])
                                        str = "Yellow";
                                    Color color = (Color)ColorConverter.ConvertFromString(str);
                                    SolidColorBrush brush = new SolidColorBrush(color);
                                    ((Ellipse)this.FindName(((finger_contacts)i).ToString())).Fill = brush;
                                }

                                for (int i = 0; i < 6; i++)
                                {
                                    for (int n = 0; n < 3; n++)
                                    {
                                        oldR[i, n + 1] = oldR[i, n];  //saves older values for averages
                                    }
                                    oldR[i, 0] = glove.values[i];
                                }

                               /* controller.mouseController.SetCursorPos(tempX*15, tempY*15);
                                if (glove.contacts[0])
                                {
                                   //controller.mouseController.sendMouseRightclick((int)tempX*15,(int)tempY*15);
                                }*/
                            }
                            else
                            {
                                if (LranOnce)
                                    for (int i = 0; i < 5; i++)
                                        if (shortDetected(glove, oldL, i) || incorrectSmoothing(glove, oldL, i))
                                        {
                                            glove.values[i] = oldL[i, 0];
                                        }
                                        else
                                            RranOnce = true;

                                for (int i = 6; i < 10; i++)
                                {
                                    glove.values[i] -= setters[1, i];
                                }
                                //Saves finger values to the UI
                                for (var z = 0; z < 6; z++)
                                {
                                    ((ProgressBar)this.FindName(((UI)(z + 6)).ToString())).Value = glove.values[z];
                                }

                                //Save gyro/accelerometer values to the UI
                                int tempX = (glove.mpu.gyro_Y / 400) + 30;//y axis
                                int tempY = (glove.mpu.gyro_X / 400) + 40;//x axis
                                Yb.Text = tempY.ToString();
                                Xb.Text = tempX.ToString();
                                targetL.SetValue(Canvas.LeftProperty, (double)tempX);
                                targetL.SetValue(Canvas.TopProperty, (double)tempY);
                                int tempZ = (glove.values[8] / 400);//z axis
                                Zb.Text = tempZ.ToString();


                                for (int i = 0; i < 6; i++)
                                {
                                    for (int n = 0; n < 3; n++)
                                    {
                                        oldL[i, n + 1] = oldL[i, n];  //saves older vaules for averages
                                    }
                                    oldL[i, 0] = glove.values[i];
                                }
                                // sendToDriver(glove.values);
                            }
                        }));
                }
                else
                {
                    wrong = false;
                    SpIn.Write("?");
                }


            }


            /*     catch (System.FormatException)
                 {
                     Application.Current.Dispatcher.Invoke(new Action(() => { COMID.Text = "Arduino crashed"; }));
                 }*/

            finally
                {

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

            for (int i = 0; i < 5; i++)
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

            /*            if (right)
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
                    */
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
                    ((ProgressBar)this.FindName(UIname)).Maximum = setters[1, i] + 100;
                }
                else
                {
                    setters[0, i] = (int)(((ProgressBar)this.FindName(UIname)).Value);
                }
            }

        }   //Bs stands for bottoms, not whatever you were thinking, this sets the mins and maxes

        public void sendToDriver(int[] arr)
        {
            var piper = Task.Run(() =>
            {
                string strok = "";

                for (int i = 0; i <= arr.Length && i <= indexAlph.Length; i++)
                {
                    strok += indexAlph[i];
                    strok += arr[i];
                }
                var toDriver = new NamedPipeServerStream("ApexPipe", PipeDirection.InOut, 1, PipeTransmissionMode.Byte);
                var streamToDriver = new StreamReader(toDriver);

                toDriver.WaitForConnection();

                var writer = new StreamWriter(toDriver);
                writer.Write(arr);
                writer.Write((char)0);
                writer.Flush();
                toDriver.WaitForPipeDrain();

                string returned = streamToDriver.ReadLine();

                while (returned != "ok")
                {
                    txt.Text = "still awaiting return...";
                }

                txt.Text = "token received :" + returned;

                toDriver.Dispose();
            });
            Task.WaitAll(piper);
        }
    }
    public class TextboxText
    {
        public string serialDataIn { get; set; }

    }
}