using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexLogic.Anatomics
{
    public static class DefaultHandFactory
    {
        public static Hand HandFactory_Default(char Type='R')
        {
            Hand hand = new(Type);
            
            return hand;
        }
    }
}
