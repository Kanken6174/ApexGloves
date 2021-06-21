using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApexLogic;

namespace ApexLogic.COMMasters
{
    public class COMTasks
    {
        public bool runTasks = true;
        public SerialPort _serialPort;
        

        private void requestData(object sender, EventArgs e)
        {
            Task.Run(new Action(async () =>
            {
                do
                {
                    await Task.Delay(Settings.TimeToWait);  //To not overstrain the arduino, wait a bit
                    if (_serialPort.IsOpen) _serialPort.Write("#"); //Requesting data

                } while (runTasks); //will run as long as runtasks is true
            }));
        }
    }
}
