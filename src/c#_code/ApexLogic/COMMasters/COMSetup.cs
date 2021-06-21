using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexLogic.COMMasters
{
    public static class COMSetup
    {
        /// <summary>
        /// Setups an existing Serial port from a string.
        /// </summary>
        /// <param name="sp">Serial port to setup</param>
        /// <param name="COMPort">COM port to connect to</param>
        /// <returns></returns>
        public static SerialPort SetupPort(this SerialPort sp, string COMPort)
        {
            if (!COMPort.Contains("COM"))
                return null;
            if (sp is null)
                sp = new();
            else if (sp.IsOpen)
                sp.Close();

            sp.PortName = COMPort;
            sp.ReadTimeout = 50;
            sp.WriteTimeout = 50;
            sp.BaudRate = 38400;
            sp.Handshake = Handshake.None;
            
            try
            {
                sp.Open();
                sp.Close();
                return sp;
            }
            catch (IOException)
            {
                if (sp.IsOpen)
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
