using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Leander.Nr1
{
    class Program
    {
        static void Main(string[] args)
        {
            int index = Utility.ReturnMatchingIndex("MyFunction((string)aaa, bbb)", true, 27, '(');

            //R17.Execute();
        }
    }
}
