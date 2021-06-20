using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApexLogic.DataFormats;
using ApexLogic.DataInputs;

namespace ApexLogic.Delimiters
{
    public class Delimiter
    {
        public char Indexer;//Letter or symbol bound to the delimiter
        public string Description;//User-added description
        private string StrValue;
        public int Value;
        private List<int> ValuesOverTime;
        IInputData MySource;

        public Delimiter(char Indexer)
        {
            this.Indexer = Indexer;
        }

        public string ChainOfCommand(string Data)
        {
            int i = 0;
            for(i = 0; i < Data.Length; i++)
            {
                if (!char.IsDigit(Data[i]) && !char.IsSymbol(Data[i]))
                    break;
            }
            if (i > 0)
            {
                StrValue = Data.Substring(0, i - 1);
                int.TryParse(StrValue, out Value);
            }

            Data = Data.Substring(i, Data.Length - i);
            MySource.Process();

            return Data;
        }
    }
}
