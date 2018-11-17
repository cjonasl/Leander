using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leander.Nr1
{
    public static class R8
    {
        public static void Execute()
        {
            DateTime dt = new DateTime(2018, 10, 4, 15, 10, 10);

            CultureInfo ci1 = new CultureInfo("en-GB");
            string str1 = dt.ToString(ci1);

            CultureInfo ci2 = new CultureInfo("en-IE");
            string str2 = dt.ToString(ci2);

            CultureInfo ci3 = new CultureInfo("en-US");
            string str3 = dt.ToString(ci3);

            CultureInfo ci4 = new CultureInfo("en-CA");
            string str4 = dt.ToString(ci4);

            CultureInfo ci5 = new CultureInfo("en-AU");
            string str5 = dt.ToString(ci5);

            CultureInfo ci6 = new CultureInfo("en-NZ");
            string str6 = dt.ToString(ci6);
        }
    }
}
