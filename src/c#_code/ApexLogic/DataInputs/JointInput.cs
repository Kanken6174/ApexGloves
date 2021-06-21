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
        string Raw;
        float Angle;
        Finger Managed;

        public override void Update()
        {
            float.TryParse(Raw, out Angle);
            foreach (KeyValuePair<int, Joint> j in Managed.FingerJoints)
            {
                j.Value.setAngle(Angle);
            }
        }
    }

    
}
