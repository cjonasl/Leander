using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangCSharp
{
    /*
     ForStatement
     ReadWriteTextFromFile
     WhileStatement
    */
    class Program
    {
        static void Main(string[] args)
        {
            ForStatement();
            ReadWriteTextFromFile();
            WhileStatement();
            string str = Console.ReadLine();
        }

        private static void ForStatement()
        {
            int i, sumInteger1To10 = 0;

            for (i = 1; i <= 10; i++)
            {
                sumInteger1To10 += i;
            }

            Console.WriteLine(sumInteger1To10);

            //or (curly braces optioinal if only one statement for-body)
            sumInteger1To10 = 0;
            for (i = 1; i <= 10; i++)
                sumInteger1To10 += i;

            Console.WriteLine(sumInteger1To10);

            for (i = 1; i <= 10; i++) //For several statements then must have curly braces
            {
                if (i == 1)
                    sumInteger1To10 = 0;

                sumInteger1To10 += i;
            }

            Console.WriteLine(sumInteger1To10);
        }

        private static void ReadWriteTextFromFile()
        {
            string s = System.IO.File.ReadAllText("C:\\tmp\\tmp.txt");
            Console.WriteLine(s);
            System.IO.File.WriteAllText("C:\\tmp\\tmp.txt", s + " Hello World!");
            s = System.IO.File.ReadAllText("C:\\tmp\\tmp.txt");
            Console.WriteLine(s);
        }

        private static void WhileStatement()
        {
            int i = 0, sumInteger1To10 = 0;

            while (++i <= 10)
            {
                sumInteger1To10 += i;
            }

            Console.WriteLine(sumInteger1To10);

            //or (curly braces optioinal if only one statement in while-body)
            i = 0;
            sumInteger1To10 = 0;
            while (++i <= 10)
                sumInteger1To10 += i;

            Console.WriteLine(sumInteger1To10);

            i = 1;
            sumInteger1To10 = 0;
            while (i <= 10) //For several statements then must have curly braces
            {
                sumInteger1To10 += i;
                i++;
            }

            Console.WriteLine(sumInteger1To10);
        }
    }
}
