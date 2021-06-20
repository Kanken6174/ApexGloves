using ApexLogic.Anatomics;
using ApexLogic.DataFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexLogic.DataInputs
{
    public class JointInput : IInputData
    {
        string Raw;
        float Angle;
        Finger Managed;

        public void Process()
        {
            float.TryParse(Raw, out Angle);
            foreach(Joint j in Managed.FingerJoints)
            {
                j.setAngle(Angle);
            }
        }
    }

    
}
