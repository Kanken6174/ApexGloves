using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexLogic.DataFormats
{
    public class InputData : IComparable<InputData>
    {
        public string RawIn = new("");

        public InputData()
        {

        }

        public int CompareTo(InputData other)
        {
            if (other.RawIn.Length > RawIn.Length)
                return 1;
            else if (other.RawIn.Length < RawIn.Length)
                return -1;
            else
                return 0;

        }

     public void Update()
        {

        }
    }
}
