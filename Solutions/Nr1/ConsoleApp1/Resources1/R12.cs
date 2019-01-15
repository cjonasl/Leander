using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leander.Nr1
{
    public static class R12
    {
        public static void Execute()
        {
            Person[] persons = new Person[]
            {
                new Person("Jonas", "Leander", 49),
                new Person("Knut", "Andersson", 32),
                new Person("Adam", "Johansson", 8),
                new Person("Ivan", "Lendl", 58),
                new Person("Jimmy", "Dahl", 12)
            };

           
            foreach (Person p in persons)
            {
                Console.WriteLine(p.CompareNumber);
            }

            Console.WriteLine();

            Person[] p1 = persons.OrderBy(p => p.CompareNumber).ToArray();

            foreach (Person p in p1)
            {
                Console.WriteLine(p.ToString());
            }

            Console.WriteLine();

            Person[] p2 = persons.Distinct().ToArray();

            foreach (Person p in p2)
            {
                Console.WriteLine(p.ToString());
            }
        }
    }
}
