using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// A284B284C284D284E284F284G284H284I0J0K0L0M-2.90N2.57O-0.17P-0.00Q-0.00R-0.00

namespace ApexLogic.COMMasters
{
    public static class COMConnectAndTalk
    {
        public static string ConnectAndReadOnceFrom(string ComPort)
        {
            if (!ComPort.Contains("COM"))
                return null;
            SerialPort sp = new();
            sp.PortName = ComPort;
            sp.ReadTimeout = 50;
            sp.WriteTimeout = 50;
            sp.BaudRate = 38400;
            try
            {
                sp.Open();
                sp.Write("#");
                string ToReturn = sp.ReadLine();
                sp.Close();
                return ToReturn;
            }
            catch (IOException)
            {
                if(sp.IsOpen)
                    sp.Close();
                return null;
            }
            catch (TimeoutException)
            {
                if (sp.IsOpen)
                    sp.Close();
                return null;
            }

        }
    }
}
