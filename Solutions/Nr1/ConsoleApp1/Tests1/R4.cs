using hiJump.Infrastructure.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Utility_classes;

namespace Leander.Nr1
{
    public static class R4
    {
        public static void Execute()
        {
            StringBuilder sb = new StringBuilder("CompanyName,ContactFirstName,ContactLastName,ContactLocation,ContactTitle,ContactPosition,ContactTelephone,ContactFax,ContactMobile,ContactEmail,IsPrimaryContact,IsPrimaryInvoiceContact\r\n");
            Random random = new Random((int)(DateTime.Now.Ticks % (long)int.MaxValue));
            string r1, r2, r3;

            for (int i = 1; i <= 1000; i++)
            {
                r1 = random.Next(10000, int.MaxValue).ToString();
                r2 = random.Next(10000, int.MaxValue).ToString();
                r3 = random.Next(10000, int.MaxValue).ToString();
                sb.Append(string.Format("Fujitsu Caribbean (Barbados) Limited (Internal Legal Entity),CCC{0},CCC{0},Head Office,,Electrical Engineer,{1},{2},{3},CCC{0}@gmail.com,,\r\n", i.ToString(), r1, r2, r3));
            }

            Utility.CreateNewFile("C:\\tmp\\ContactFile2000.csv", sb.ToString().TrimEnd());
        }
    }
}
