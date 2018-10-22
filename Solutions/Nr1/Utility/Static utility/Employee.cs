using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leander.Nr1
{
    public partial class Utility
    {
        public static List<Employee> ReturnListWithEmployees()
        {
            List<Employee> list = new List<Employee>();

            list.Add(new Employee(1, "Jonas", "Leander", 49));
            list.Add(new Employee(2, "Malin", "Andersson", 32));
            list.Add(new Employee(3, "Ivan", "Lendl", 58));
            list.Add(new Employee(4, "Martina", "Navratilova", 62));
            list.Add(new Employee(5, "Bjorn", "Borg", 62));
            list.Add(new Employee(6, "Nina", "Johansson", 12));
            list.Add(new Employee(7, "Jimmy", "Connors", 66));
            list.Add(new Employee(8, "Anna", "Hagberg", 88));
            list.Add(new Employee(9, "Daniel", "Knaust", 2));
            list.Add(new Employee(10, "Sara", "Ljungberg", 21));

            return list;
        }
    }
}
