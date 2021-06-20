using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApexLogic.DataFormats;
using ApexLogic.Positionning;

namespace ApexLogic.Anatomics
{
    class Hand
    {
        public char HandType;
        public Point3d HandPos;
        List<IInputData> Inputs;
        List<Finger> Fingers;

        public Hand(char HandType)
        {
            this.HandType = HandType;
            HandPos.Reset();
        }
    }
}
