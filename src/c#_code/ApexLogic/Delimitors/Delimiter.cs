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
        private string StrValue; //Value read as string.
        public int InputValue;  //Value read as int (0-1023) for arduino
        private List<int> ValuesOverTime;   //Values over time, used for error correction.
        public InputData MySource; //Source type. Would usually be a potentiometer or Pad.

        public Delimiter(char Indexer)
        {
            this.Indexer = Indexer;
        }
        
        /// <summary>
        /// Processes data for this delimiter, and returning the unused data for the rest of the delimitier chain
        /// </summary>
        /// <param name="Data">Data gotten from the previous chain node, or the parent Updat function</param>
        /// <returns>Unused data for the rest of the chain</returns>
        public string ChainedProcessing(string Data)
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
                int.TryParse(StrValue, out InputValue);
            }

            Data = Data.Substring(i, Data.Length - i);
            MySource.RawIn = StrValue;
            MySource.Update();

            return Data;
        }
    }
}
