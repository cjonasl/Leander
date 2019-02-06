using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leander.Nr1
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileNameFullPath = @"C:\git_cjonasl\Leander\Solutions\Nr1\WebApplication1\Text\Page1Menu1Sub1Sub2Tab3.txt";

            int[] v = new int[] {6, 1, 2, 3, 4, 5, 6, 0, 0, 0 };
            RemoveNumberFromArrayIfItExists(v, 7);
            Print(v);
            return;

            DateTime d1, d2;
            Random random = new Random((int)(DateTime.Now.Ticks % 96776L));
            Communication communication;
            string message, errorMessage;
            int i, n;

            d1 = new DateTime(2018, 10, 1);
            d2 = new DateTime(2018, 10, 5);

            while(d1 < d2)
            {
                n = random.Next(2, 11);

                for(i = 0; i < n; i++)
                {
                    message = Utility.SimulateMessage(3, 8, 60, 2, 12);
                    communication = new Communication();
                    communication.Message = message.Replace("\r\n", "\n");

                    if (random.Next(2) == 0)
                        communication.Sender = "Jonas";
                    else
                        communication.Sender = "Joe";

                    CommunicationUtility.InsertNewCommunicationMessageTest(fileNameFullPath, communication, d1.ToString("yyMMdd"), out errorMessage);
                }

                d1 = d1.AddDays(1);
            }

            //Utility.CreateNewFile("C:\\AAA\\aaa.txt", Utility.SimulateMessage(3, 8, 60, 2, 12));
            //R17.Execute();
        }

        public static void RemoveNumberFromArrayIfItExists(int[] v, int number)
        {
            int i, n, index = -1;

            n = v[0];
            i = 1;
            while (i <= n && index == -1)
            {
                if (v[i] == number)
                    index = i;
                else
                    i++;
            }
            
            if (index != -1)
            {
                while (index + 1 <= n)
                {
                    v[index] = v[index + 1];
                    index++;
                }

                v[0]--;
            }
        }

        public static void Print(int[] v)
        {
            Console.WriteLine(string.Format("[{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}]", v[0], v[1], v[2], v[3], v[4], v[5], v[6], v[7], v[8], v[9]));
        }
    }
}
