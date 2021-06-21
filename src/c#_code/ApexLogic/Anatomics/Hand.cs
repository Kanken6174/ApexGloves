using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApexLogic.DataFormats;
using ApexLogic.Positionning;
using ApexLogic.Delimiters;
using ApexLogic.COMMasters;
using System.IO.Ports;

namespace ApexLogic.Anatomics
{
    public class Hand
    {
        public char HandType;
        public Point3d HandPos = new();
        Dictionary<int,Finger> Fingers = new();
        public Dictionary<int, InputData> MyInputs = new();
        public List<Delimiter> MyDelimiters = new();
        public SerialPort Port;
        public string RawIn = "";
        public Hand(char HandType, SerialPort Port = null, int fingers = 5)
        {   
            if(Port is not null)
                Port.DataReceived += Port_DataReceived;

            this.HandType = HandType;
            HandPos.Reset();

            for (int i = fingers; i > 0; i--)
            {
                Finger f = new(i);
                Fingers.Add(i, f);
            }
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            RawIn = COMReader.ReadData(Port);
            Update();
        }

        private void Update()
        {
            foreach(Delimiter Del in MyDelimiters)
            {
                RawIn = Del.ChainedProcessing(RawIn);
            }
        }
    }
}
