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
    public class Hand
    {
        public char HandType;
        public Point3d HandPos;
        Dictionary<int,Finger> Fingers;
        public Dictionary<int, InputData> MyInputs;

        public Hand(char HandType, int fingers = 5)
        {
            this.HandType = HandType;
            HandPos.Reset();

            for (int i = fingers; i > 0; i--)
            {
                Finger f = new(i);
                Fingers.Add(i, f);
            }
        }

        public void Update()
        {

        }
    }
}
