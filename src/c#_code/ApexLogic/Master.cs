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
        public Hand Hands = new Hand('R');

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
