using ApexLogic.Anatomics;
using ApexLogic.COMMasters;
using ApexLogic.Delimiters;
using System;
using System.Collections.Generic;

namespace ApexLogic
{
    public class Master
    {
        public List<string> ValidPorts = COMFinder.AutodetectArduinoPort().Result;
        public string ToConnectR = "";
        public string ToConnectL = "";
        public Dictionary<char, Delimiter> MyDelimitors = new();
        public Hand RightHand = new Hand('R');
        public Hand LeftHand = new Hand('L');

        public Master()
        {
             
        }
        public void Update()
        {
            
        }

        public void RefreshPorts()
        {
            ValidPorts = COMFinder.AutodetectArduinoPort().Result;
        }
    }
}
