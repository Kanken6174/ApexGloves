using ApexLogic.COMMasters;
using System;
using System.Collections.Generic;

namespace ApexLogic
{
    public class Master
    {
        public List<string> ValidPorts = COMFinder.AutodetectArduinoPort();
        public Master()
        {
             
        }
    }
}
