using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexLogic.DataFormats
{
    public abstract class InputData
    {
        public static string RawIn;
        public abstract void Update();
    }
}
