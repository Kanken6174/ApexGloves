using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexLogic.DataFormats
{
    public class Accelerometer : InputData
    {
        public int RawX, RawY, RawZ;
        public double X, Y, Z;

        public new void Update()
        {
        }
    }
}
