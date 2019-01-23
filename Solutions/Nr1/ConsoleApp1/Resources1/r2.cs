using System;
using System.Collections.Generic;

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
