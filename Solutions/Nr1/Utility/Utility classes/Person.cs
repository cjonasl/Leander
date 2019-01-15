using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leander.Nr1
{
    public class Person : IEqualityComparer<Person>, IEquatable<Person>, IComparable, IComparable<Person>
    { 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }

        public Person() { }

        public Person(string firstName, string lastName, int age)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Age = age;
        }

        public int CompareNumber
        {
            get
            {
                return (this.FirstName == null ? 0 : this.FirstName.Length) + (this.LastName == null ? 0 : this.LastName.Length) + this.Age;
            }
        }

        public bool Equals(Person x, Person y)
        {
            if (Object.ReferenceEquals(x, y)) return true; //Same object or both null

            if (((x == null) && (y != null)) || ((x != null) && (y == null))) //One null and the other not
                return false;

            return (x.CompareNumber == y.CompareNumber);
        }

        public bool Equals(Person other)
        {
            if (Object.ReferenceEquals(this, other)) return true;

            if (other == null)
                return false;

            return (this.CompareNumber == other.CompareNumber);
        }

        public int GetHashCode(Person obj)
        {
            int n = CompareNumber;
            return n.GetHashCode();
        }

        public int CompareTo(object other)
        {
            if (Object.ReferenceEquals(this, other)) return 0;

            if (other == null)
                return 1;

            if (this.CompareNumber < ((Person)other).CompareNumber)
                return -1;
            else
                return 1;
        }

        public int CompareTo(Person other)
        {
            if (Object.ReferenceEquals(this, other)) return 0;

            if (other == null)
                return 1;

            if (this.CompareNumber < other.CompareNumber)
                return -1;
            else
                return 1;
        }

        public static bool operator ==(Person p1, Person p2)
        {
            if (Object.ReferenceEquals(p1, p2))
                return true;

            if (Object.ReferenceEquals(p1, null))
                return false;

            if (Object.ReferenceEquals(p2, null))
                return false;

            return (p1.CompareNumber == p2.CompareNumber);
        }

        public static bool operator !=(Person p1, Person p2)
        {
            return !(p1 == p2);
        }

        public static bool operator <(Person p1, Person p2)
        {
            if (Object.ReferenceEquals(p1, p2))
                return false;
            else if ((p1 == null) && (p2 != null))
                return true;
            else if ((p1 != null) && (p2 == null))
                return false;
            else
                return (p1.CompareNumber < p2.CompareNumber);
        }

        public static bool operator <=(Person p1, Person p2)
        {
            if (Object.ReferenceEquals(p1, p2))
                return true;
            else if ((p1 == null) && (p2 != null))
                return true;
            else if ((p1 != null) && (p2 == null))
                return false;
            else
                return (p1.CompareNumber <= p2.CompareNumber);
        }

        public static bool operator >(Person p1, Person p2)
        {
            if (Object.ReferenceEquals(p1, p2))
                return false;
            else if ((p1 == null) && (p2 != null))
                return false;
            else if ((p1 != null) && (p2 == null))
                return true;
            else
                return (p1.CompareNumber > p2.CompareNumber);
        }

        public static bool operator >=(Person p1, Person p2)
        {
            if (Object.ReferenceEquals(p1, p2))
                return true;
            else if ((p1 == null) && (p2 != null))
                return false;
            else if ((p1 != null) && (p2 == null))
                return true;
            else
                return (p1.CompareNumber >= p2.CompareNumber);
        }

        public static new bool Equals(Object objA, Object objB)
        {
            if (objA != null)
                return objA.Equals(objB);
            else if (objB != null)
                return objB.Equals(objA);
            else
                return true;
        }

        public override int GetHashCode()
        {
            return GetHashCode(this);
        }

        public override bool Equals(Object obj)
        {
            return Equals((Person)obj);
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2}, {3})", this.FirstName, this.LastName, this.Age, this.CompareNumber.ToString());
        }
    }
}
