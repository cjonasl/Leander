using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leander.Nr1
{
    public static class R2
    {
        public static void Execute()
        {
            List<Employee> list = Utility.ReturnListWithEmployees();

            list.ForEach(x => x.Note = x.Note + "ABC");

            foreach(var employee in list)
            {
                Console.WriteLine(employee.Note);
            }
        }
    }
}
