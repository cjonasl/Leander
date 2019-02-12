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
            string password, hash, salt;
            bool b1, b2;
            int l1, l2;

            password = "abc";
            Utility.GenerateSaltedHash(password, out hash, out salt);
            b1 = Utility.VerifyPassword("London123", hash, salt);
            b2 = Utility.VerifyPassword("abc", hash, salt);
            l1 = hash.Length; //344
            l2 = salt.Length; //88

            //Utility.CreateNewFile("C:\\AAA\\aaa.txt", Utility.SimulateMessage(3, 8, 60, 2, 12));
            //R17.Execute();
        }
    }
}
