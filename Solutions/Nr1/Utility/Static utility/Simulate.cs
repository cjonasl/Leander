using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leander.Nr1
{
    public partial class Utility
    {
        public static string SimulateWord(int minNumberOfChars, int maxNumberOfChars, int number)
        {
            Random r;
            int i, numberOfChars, n, seed;
            char[] charArray;

            seed = ((int)(DateTime.Now.Ticks % 64765L)) % number;

            r = new Random(seed);
            numberOfChars = r.Next(minNumberOfChars, 1 + maxNumberOfChars);
            charArray = new char[numberOfChars];

            for (i = 0; i < numberOfChars; i++)
            {
                n = r.Next(97, 123);
                charArray[i] = Convert.ToChar(n);
            }

            return new string(charArray);
        }

        public static string SimulateRow(int maxNumberOfCharsInRow, int minNumberOfCharsInWord, int maxNumberOfCharsInWord, int number)
        {
            string word;
            StringBuilder sb = new StringBuilder("");
            int n = 10;

            while(sb.ToString().Length < maxNumberOfCharsInRow)
            {
                word = SimulateWord(minNumberOfCharsInWord, maxNumberOfCharsInWord, n + number);

                if (sb.ToString().Length == 0)
                    sb.Append(word);
                else
                    sb.Append(" " + word);

                n += 5;
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
                row = SimulateRow(maxNumberOfCharsInRow, minNumberOfCharsInWord, maxNumberOfCharsInWord, i + 8);

                if (i == 0)
                    sb.Append(row);
                else
                    sb.Append("\r\n" + row);
            }

            return sb.ToString();
        }
    }
}
