using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexLogic.Anatomics
{
    class Joint
    {
        public float RawAngle = 0; //0-1023
        public float Angle; //min-max
        public float MinAngle = -10;
        public float MaxAngle = 90;
        public int Index; //position in the finger

        public void setAngle(float angle)
        {
            RawAngle = angle; //angle of the potentiometer usually.
            
        }
    }
}
