using System;
using Ninject;
using Omu.ValueInjecter;

namespace ConsoleApp2
{
    public interface ICustomer
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        int Age { get; set; }
    }


    public class Customer : ICustomer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DOB { get; set; }
    }


    class Program
    {
        private static Lazy<IKernel> _kernel = new Lazy<IKernel>(() => new StandardKernel());

        static void Main(string[] args)
        {
            Customer customer = _kernel.Value.Get<Customer>();
            customer.FirstName = "Jonas";

            Person person = new Person()
            {
                FirstName = "Knut",
                LastName = "Andersson",
                DOB = new DateTime(1969, 10, 4)
            };

            customer.InjectFrom(person);

            PrintFirstName();

            Console.WriteLine("FirstName: {0}, LastName: {1} and Age: {2}", customer.FirstName, customer.LastName, customer.Age);
        }

        private static void PrintFirstName()
        {
            Customer customer = _kernel.Value.Get<Customer>();
            Console.WriteLine("FirstName: {0}", customer.FirstName);
        }
    }
}
