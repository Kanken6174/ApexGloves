using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ApexLogic;

namespace ApexLogic.Anatomics
{
    class Joint
    {
        public float RawAngle = Settings.DefaultMinAngleRaw; //0-1023
        public float ClosedRaw = 500;
        public float OpenRaw = 1023;
        private float UsableOpenRaw => OpenRaw - ClosedRaw;
        public float AnglePercentage => ((RawAngle - ClosedRaw) / UsableOpenRaw)*100; //min-max

        public float MinAngle = -10;
        public double TrueAngle => IntegratePositionToAngleFactor();

        public int Index = 0; //position in the finger

        public void setAngle(float angle)
        {
            RawAngle = angle; //angle of the potentiometer usually.       
        }

        private float IntegratePositionToAngleFactor()
        {
            float toReturn = 0;
            switch (Index)
            {
                case 1:         //is at the root of the finger
                    toReturn = (30 - MinAngle) * (AnglePercentage / 100);
                    break;
                case 2:         //is at the first joint
                    toReturn = (90 - MinAngle) * (AnglePercentage / 100);
                    break;
                case 3:         //Second joint
                    toReturn = (20 - MinAngle) * (AnglePercentage / 100);
                    break;
                default:        //You failed the human test
                    toReturn = (90)*(AnglePercentage / 100);
                    break;
            }
            return toReturn;
        }
    }
}
