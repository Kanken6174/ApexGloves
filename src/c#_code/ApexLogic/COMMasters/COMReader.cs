using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexLogic.COMMasters
{
    public static class COMReader
    {
        public static string ReadData(this SerialPort sp)
        {
            if (sp.IsOpen)
                return sp.ReadLine();
            else
            {
                sp.Open();
                return sp.ReadLine();
            }

        }
    }
}
