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
            R10.Execute(18 * 1024 * 1024, "C:\\tmp\\File18.txt");
            R10.Execute(19 * 1024 * 1024, "C:\\tmp\\File19.txt");
            R10.Execute(20 * 1024 * 1024, "C:\\tmp\\File20.txt");
            R10.Execute(21 * 1024 * 1024, "C:\\tmp\\File21.txt");
        }
    }
}
