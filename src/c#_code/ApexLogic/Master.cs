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

        public Master()
        {
             
        }

        public void RefreshPorts()
        {
            ValidPorts = COMFinder.AutodetectArduinoPort().Result;
        }
    }
}
