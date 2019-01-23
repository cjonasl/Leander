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
        /// Check if a DateTime is within limits.
        /// </summary>
        /// <param name="l">Lower as string yyyy-MM-dd HH:mm:ss or empty</param>
        /// <param name="u">Upper as string yyyy-MM-dd HH:mm:ss or empty</param>
        /// <param name="dateTime">DateTime to compare</param>
        /// <returns>true if fulfill requirement otherwise false</returns>
        public static bool DateTimeFulfillRequirement(string l, string u, DateTime dateTime)
        {
            DateTime lower, upper;

            if (string.IsNullOrEmpty(l))
            {
                lower = DateTime.MinValue;
            }
            else
            {
                lower = ReturnDateTimeFromString(l);
            }

            if (string.IsNullOrEmpty(u))
            {
                upper = DateTime.MaxValue;
            }
            else
            {
                upper = ReturnDateTimeFromString(u);
            }

            return ((dateTime >= lower) && (dateTime <= upper));
        }
    }
}
