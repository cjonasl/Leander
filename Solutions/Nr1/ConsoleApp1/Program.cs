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

                    CommunicationUtility.InsertNewMessageTest(fileNameFullPath, communication, d1.ToString("yyMMdd"), out errorMessage);
                }

                d1 = d1.AddDays(1);
            }


            //Utility.CreateNewFile("C:\\AAA\\aaa.txt", Utility.SimulateMessage(3, 8, 60, 2, 12));
            //R17.Execute();
        }
    }
}
