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
            Utility.CreateNewFile("C:\\AAA\\aaa.txt", Utility.SimulateMessage(3, 8, 60, 2, 12));
            //R17.Execute();
        }
    }
}
