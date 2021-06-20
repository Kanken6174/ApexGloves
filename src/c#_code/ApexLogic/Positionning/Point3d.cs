using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace ApexLogic.Positionning
{
    public class Point3d
    {
        public double XP = new();
        public double YP = new();
        public double ZP = new();
        public double XR = new();
        public double YR = new();
        public double ZR = new();

        public override bool Equals(object obj)
        {
            return obj is Point3d d &&
                   XP == d.XP &&
                   YP == d.YP &&
                   ZP == d.ZP &&
                   XR == d.XR &&
                   YR == d.YR &&
                   ZR == d.ZR;
        }

        public void Reset()
        {
            XP = 0;
            YP = 0;
            ZP = 0;
            XR = 0;
            YR = 0;
            ZR = 0;
        }
    }
}
