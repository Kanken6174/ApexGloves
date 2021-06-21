using ApexLogic.Anatomics;
using ApexLogic.COMMasters;
using ApexLogic.Delimiters;
using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace ApexLogic
{
    public class Master
    {
        public List<string> ValidPorts => COMFinder.AutodetectArduinoPort().Result;
        public string toConnectR = "";
        public string toConnectL = "";
        public SerialPort RPort = new();
        public SerialPort LPort = new();
        public Hand RightHand = new('R', null);
        public Hand LefttHand = new('R', null);
        private COMTasks RT = new();
        private COMTasks LT = new();

        public string ToConnectR
        {
            get => toConnectR;
            set
            {
                toConnectR = value;
                RPort.SetupPort(value);
                RT._serialPort = RPort;
            }
        }
        public string ToConnectL
        {
            get => toConnectL;
            set
            {
                toConnectL = value;
                LPort.SetupPort(value);
                LT._serialPort = LPort;
            }
        }

        public Master()
        {
            RightHand.Port = RPort;
            LefttHand.Port = LPort;
            RT._serialPort = RPort;
            LT._serialPort = LPort;
        }
        public void Update()
        {
            if (RPort.IsOpen)
                RPort.Write("#");
            if (LPort.IsOpen)
                LPort.Write("#");
        }
    }
}
