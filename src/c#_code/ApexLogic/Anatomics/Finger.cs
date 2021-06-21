using ApexLogic.DataFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexLogic.Anatomics
{
    class Finger
    {
        public Dictionary<int,Joint> FingerJoints = new(); //joints of the finger
        public int Index; //position in the hand, 0 is towards torso (thumb if you're a human), ++ is towards exterior (pinkie)
        public Dictionary<int, InputData> MyInputs = new();
        public Finger(int Index, int Joints = 3)
        {
            for(int i = Joints; i > 0; i--)
            {
                Joint j = new();
                j.Index = i;
                FingerJoints.Add(i, j);
            }
        }

        public void Update()
        {
            foreach(KeyValuePair<int, InputData> Input in MyInputs)
            {
                switch (Input.Key)
                {
                    case 0: //is rotary potentiometer
                        Input.Value.Update();
                        break;
                    case 1: //is linear potentiometer
                        break;
                    case 2: //is pad
                        break;
                    case 3: //others
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
