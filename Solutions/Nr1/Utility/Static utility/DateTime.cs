using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leander.Nr1
{
    public partial class Utility
    {
        public static DateTime ReturnDateTimeFromString(string dateTime)
        {
            int year, month, day, hour, minute, second;

            //yyyy-MM-dd HH:mm:ss
            year = int.Parse(dateTime.Substring(0, 4));
            month = int.Parse(dateTime.Substring(5, 2));
            day = int.Parse(dateTime.Substring(8, 2));
            hour = int.Parse(dateTime.Substring(11, 2));
            minute = int.Parse(dateTime.Substring(14, 2));
            second = int.Parse(dateTime.Substring(17, 2));

            return new DateTime(year, month, day, hour, minute, second);
        }

        /// <summary>
        /// Check if a DateTime is within limits, i.e. dateTime is in [a, b]
        /// </summary>
        /// <param name="a">After as string yyyy-MM-dd HH:mm:ss or empty</param>
        /// <param name="b">Before as string yyyy-MM-dd HH:mm:ss or empty</param>
        /// <param name="dateTime">DateTime to compare</param>
        /// <returns>true if fulfill requirement otherwise false</returns>
        public static bool DateTimeFulfillRequirement(string a, string b, DateTime dateTime)
        {
            DateTime after, before;

            if (string.IsNullOrEmpty(a))
            {
                after = DateTime.MinValue;
            }
            else
            {
                after = ReturnDateTimeFromString(a);
            }

            if (string.IsNullOrEmpty(b))
            {
                before = DateTime.MaxValue;
            }
            else
            {
                before = ReturnDateTimeFromString(b);
            }

            return ((dateTime >= after) && (dateTime <= before));
        }
    }
}
