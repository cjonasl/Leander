using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            WhileStatement();
        }

        private static void ForStatement()
        {
            int i, sumInteger1To10 = 0;

            for (i = 1; i <= 10; i++)
            {
                sumInteger1To10 += i;
            }

            //or (curly braces optioinal if only one statement)
            sumInteger1To10 = 0;
            for (i = 1; i <= 10; i++)
                sumInteger1To10 += i;

            for (i = 1; i <= 10; i++) //For several statements then must have curly braces
            {
                if (i == 1)
                    sumInteger1To10 = 0;

                sumInteger1To10 += i;
            }
        }

        private static void WhileStatement()
        {
            int i = 0, sumInteger1To10 = 0;

            while (++i <= 10)
            {
                sumInteger1To10 += i;
            }

            //or (curly braces optioinal if only one statement)
            i = 0;
            sumInteger1To10 = 0;
            while (++i <= 10)
                sumInteger1To10 += i;

            i = 1;
            sumInteger1To10 = 0;
            while (i <= 10) //For several statements then must have curly braces
            {
                sumInteger1To10 += i;
                i++;
            }
        }
    }
}
