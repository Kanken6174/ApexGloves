using ApexLogic.DataFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexLogic.DataInputs
{
    public class Pad : InputData
    {
        public float position;
        public bool IsInContact;

        public override void Update()
        {
        }
    }
}
