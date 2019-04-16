using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CAST.Validation
{
    public class DateOfBirthValidation
    {
        private const int minimumAcceptableAgeOfEmployee = 15;
        private const int maximumAcceptableAgeOfEmployee = 80;

        /// <summary>
        /// Check that date of birth is correct
        /// </summary>
        /// <param name="year">Assume it is between 1900 and 2100</param>
        /// <param name="month">Assume it is between 1 and 12</param>
        /// <param name="day">Assume it is between 1 and 31</param>
        /// <returns>Error message if incorrect date of birth, otherwise null</returns>
        public static string Check(int year, int month, int day, out bool? errorVariableIsYear)
        {
            errorVariableIsYear = null;

            if (DateTime.DaysInMonth(year, month) < day)
            {
                errorVariableIsYear = false;

                if (month != 2)
                    return string.Format("Day is incorrect. Max is {0} for given month.", DateTime.DaysInMonth(year, month));
                else
                    return string.Format("Day is incorrect. Max is {0} for given month and year.", DateTime.DaysInMonth(year, month));
            }
            else
            {
                DateTime dataOfBirth = new DateTime(year, month, day);
                DateTime dateToday = DateTime.Today;
                TimeSpan ts = dateToday - dataOfBirth;

                if (dateToday <= dataOfBirth || ts.TotalDays < (minimumAcceptableAgeOfEmployee * 365) || ts.TotalDays > (maximumAcceptableAgeOfEmployee * 365))
                {
                    errorVariableIsYear = true;

                    if (dateToday <= dataOfBirth )
                        return "Incorrect year. Must be before current year.";
                    else if (ts.TotalDays < (minimumAcceptableAgeOfEmployee * 365))
                        return "Incorrect year. The employee is too young.";
                    else
                        return "Incorrect year. The employee is too old.";
                }
            }

            return null;
        }
    }
}