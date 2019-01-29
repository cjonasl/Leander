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
            DateTime dateTime = DateTime.Now;
            string str = Utility.ReturnDateTimeAsLongSwedishString(dateTime);

            //R8.Execute();
        }
    }
}
