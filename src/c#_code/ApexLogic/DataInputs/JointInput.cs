using ApexLogic.Anatomics;
using ApexLogic.DataFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexLogic.DataInputs
{
    public class JointInput : InputData
    {
        float Angle;
        Finger Managed;

        public new void Update()
        {
            float.TryParse(RawIn, out Angle);
            foreach (KeyValuePair<int, Joint> j in Managed.FingerJoints)
            {
                j.Value.setAngle(Angle);
            }
        }
    }

    
}
