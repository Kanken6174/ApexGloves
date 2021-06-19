using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.IO.Ports;
using System.IO;

namespace ApexLogic.COMMasters
{
    public class COMFinder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal static async Task<List<string>> AutodetectArduinoPort()
        {
            List<string> ValidPorts = new();
            foreach (string str in SerialPort.GetPortNames())
            {
                try
                {
                    string result = new("");
                    SerialPort sp = new();
                    sp.ReadTimeout = 50;
                    sp.WriteTimeout = 50;
                    sp.PortName = str;
                    sp.BaudRate = 38400;
                    try
                    {
                        sp.Open();
                    }
                    catch(Exception e)
                    {
                        //Port was protected
                    }

                    if (sp.IsOpen)
                    {
                        sp.Write("!");

                        try
                        {
                            result = sp.ReadLine();
                            sp.Close();
                        }

                        catch (TimeoutException ex)
                        {
                            sp.Close();
                            //timeout
                        }
                        if (result == "R" || result == "L")
                        {
                            ValidPorts.Add(result + str);
                        }
                        if (sp.IsOpen)
                            sp.Close();
                    }
                }
                catch (IOException)
                {

                }
            }

            
            return ValidPorts;

        }

    }

}
