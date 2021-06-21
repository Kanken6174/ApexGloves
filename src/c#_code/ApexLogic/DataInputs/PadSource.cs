using ApexLogic.DataInputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexLogic.DataFormats
{
    public class PadSource : Pad
    {
        public List<PadSink> InContact;
        public List<PadSink> CanContact;

        public new void Update()
        { 
        }
    }
}
