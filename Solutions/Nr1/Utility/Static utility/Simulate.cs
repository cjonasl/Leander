using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leander.Nr1
{
    public partial class Utility
    {
        public static Random random = new Random((int)(DateTime.Now.Ticks % 64765L));

        public static string SimulateWord(int minNumberOfChars, int maxNumberOfChars)
        {
            int i, numberOfChars;
            char[] charArray;

            numberOfChars = random.Next(minNumberOfChars, 1 + maxNumberOfChars);
            charArray = new char[numberOfChars];

            for (i = 0; i < numberOfChars; i++)
            {
                charArray[i] = Convert.ToChar(random.Next(97, 123));
            }

            return new string(charArray);
        }

        public static string SimulateRow(int maxNumberOfCharsInRow, int minNumberOfCharsInWord, int maxNumberOfCharsInWord)
        {
            string word;
            StringBuilder sb = new StringBuilder("");

            while(sb.ToString().Length < maxNumberOfCharsInRow)
            {
                word = SimulateWord(minNumberOfCharsInWord, maxNumberOfCharsInWord);

                if (sb.ToString().Length == 0)
                    sb.Append(word);
                else
                    sb.Append(" " + word);
            }

            return sb.ToString();
        }

        public static string SimulateMessage(int minNumberOfRows, int maxNumberOfRows, int maxNumberOfCharsInRow, int minNumberOfCharsInWord, int maxNumberOfCharsInWord)
        {
            Random r;
            string row;
            int i, numberOfRows;
            StringBuilder sb = new StringBuilder("");

            r = new Random((int)(DateTime.Now.Ticks % (long)int.MaxValue));
            numberOfRows = r.Next(minNumberOfRows, 1 + maxNumberOfRows);

            for (i = 0; i < numberOfRows; i++)
            {
                row = SimulateRow(maxNumberOfCharsInRow, minNumberOfCharsInWord, maxNumberOfCharsInWord);

                if (i == 0)
                    sb.Append(row);
                else
                    sb.Append("\r\n" + row);
            }

            return sb.ToString();
        }
    }
}
