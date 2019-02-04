using System;
using System.Globalization;

namespace Leander.Nr1
{
    public static class R8
    {
        public static void Execute()
        {
            DateTime dt = new DateTime(2018, 10, 4, 15, 10, 10);

            CultureInfo ci1 = new CultureInfo("en-GB");
            string str1 = dt.ToString(ci1);
            string info = ci1.EnglishName;

            CultureInfo ci2 = new CultureInfo("en-IE");
            string str2 = dt.ToString(ci2);
            info = ci2.EnglishName;

            CultureInfo ci3 = new CultureInfo("en-US");
            string str3 = dt.ToString(ci3);
            info = ci3.EnglishName;

            CultureInfo ci4 = new CultureInfo("en-CA");
            string str4 = dt.ToString(ci4);
            info = ci4.EnglishName;

            CultureInfo ci5 = new CultureInfo("en-AU");
            string str5 = dt.ToString(ci5);
            info = ci5.EnglishName;

            CultureInfo ci6 = new CultureInfo("en-NZ");
            string str6 = dt.ToString(ci6);
            info = ci6.EnglishName;
        }
    }
}
