using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexLogic.DataFormats
{
    class pad : IInputData
    {
        public bool IsInContact;
        public pad ContactWith;
        public int position;
    }
}
