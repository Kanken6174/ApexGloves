using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace ApexLogic.COMMasters
{
    public class COMFinder
    {
        internal static List<string> AutodetectArduinoPort()
        {
            ManagementScope connectionScope = new();
            SelectQuery serialQuery = new("SELECT * FROM Win32_SerialPort");
            ManagementObjectSearcher searcher = new(connectionScope, serialQuery);
            List<string> ValidPorts = new();
            try
            {
                foreach (ManagementObject item in searcher.Get())
                {
                    string desc = item["Description"].ToString();
                    string deviceId = item["DeviceID"].ToString();

                    if (desc.Contains("Arduino") || desc.Contains("CH340"))
                    {
                        ValidPorts.Add(deviceId);
                    }
                }
                return ValidPorts;
            }
            catch (ManagementException)
            {
                /* Something went wrong but we'll ignore it */
            }

            return null;
        }
    }
}
