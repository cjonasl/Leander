using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leander.Nr1
{
    public class Employee
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Note { get; set; }

        public Employee(int Id, string firstName, string lastName, int age, string note = "")
        {
            this.ID = Id;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Age = age;
            this.Note = note;
        }
    }
}
