using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangCSharp
{
    /*
     Expression
     Statement
     ForLoop
     ReadWriteTextFromFile
     WhileLoop
    */
    class Program
    {
        static void Main(string[] args)
        {
            Expression();
            Console.WriteLine(Statement());
            ForLoop();
            ReadWriteTextFromFile();
            WhileLoop();
            string str = Console.ReadLine();
        }

        private static void Expression()
        {
            int a;

            //An expression must be terminated by a semicolon
            a = 2;
            Console.WriteLine(a);
        }

        private static double Statement()
        {
            return 3.14; //A statement must be terminated by a semicolon
        }

        private static void ForLoop()
        {
            int i, sumInteger1To10 = 0;

            for (i = 1; i <= 10; i++)
            {
                sumInteger1To10 += i;
            }

            Console.WriteLine(sumInteger1To10);

            //Curly braces are optioinal if only one expression/statement
            sumInteger1To10 = 0;
            for (i = 1; i <= 10; i++)
                sumInteger1To10 += i;

            Console.WriteLine(sumInteger1To10);

            for (i = 1; i <= 10; i++) //Must have curly braces when several expressions/statements
            {
                if (i == 1) //Does not work without the parentheses
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

        private static void WhileLoop()
        {
            int i = 0, sumInteger1To10 = 0;

            while (++i <= 10)  //Does not work without the parentheses
            {
                sumInteger1To10 += i;
            }

            Console.WriteLine(sumInteger1To10);

            //Curly braces are optioinal if only one expression/statement
            i = 0;
            sumInteger1To10 = 0;
            while (++i <= 10)
                sumInteger1To10 += i;

            Console.WriteLine(sumInteger1To10);

            i = 1;
            sumInteger1To10 = 0;
            while (i <= 10) //Must have curly braces when several expressions/statements
            {
                sumInteger1To10 += i;
                i++;
            }

            Console.WriteLine(sumInteger1To10);
        }
    }
}
