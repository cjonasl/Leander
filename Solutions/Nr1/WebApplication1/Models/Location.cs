using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Location
    {
        public int Page { get; set; }
        public int Menu { get; set; }
        public int Sub1 { get; set; }
        public int Sub2 { get; set; }
        public int Tab { get; set; }
        public bool? NewLocationByChangeOfTab { get; set; }

        public Location(int page, int menu, int sub1, int sub2, int tab)
        {
            this.Page = page;
            this.Menu = menu;
            this.Sub1 = sub1;
            this.Sub2 = sub2;
            this.Tab = tab;
            this.NewLocationByChangeOfTab = null;
        }

        public Location() { }
    }

    public static class LocationUtility
    {
        private static bool LocationNameCheck(string locationName, int idx1, int idx2, string str1, string str2, int lower, int upper, out int number, out string errorMessage)
        {
            int index, n;
            string str;

            number = 0;
            errorMessage = null;

            if (locationName.Substring(idx1, idx2 - idx1) != str1)
            {
                errorMessage = string.Format("ERROR!! Method \"LocationNameCheck\" found that the following location name is incorrect (it does not have \"{0}\"): {1}", str1, locationName);
                return false;
            }

            index = locationName.IndexOf(str2, idx2);

            if (index == -1)
            {
                errorMessage = string.Format("ERROR!! Method \"LocationNameCheck\" found that the following location name is incorrect (it does not contain \"{0}\"): {1}", str2, locationName);
                return false;
            }

            str = locationName.Substring(idx2, index - idx2);

            if (!int.TryParse(str, out n))
            {
                errorMessage = string.Format("ERROR!! Method \"LocationNameCheck\" found that the following location name is incorrect (there is not a number after \"{0}\"): {1}", str1, locationName);
                return false;
            }

            if (n < lower || n > upper)
            {
                errorMessage = string.Format("ERROR!! Method \"LocationNameCheck\" found that the following location name is incorrect (the number, {0}, after \"{1}\" is not in the following range[{2}, {3}]): {4}", n.ToString(), str1, lower.ToString(), upper.ToString(), locationName);
                return false;
            }

            number = n;

            return true;
        }

        /// <summary>
        /// locationName should be in the format PageAMenuBSubCSubDTabE, where A, B, C, D and E are inetegetrs
        /// </summary>
        public static bool LocationNameIsCorrect(string locationName, out string locationNameSortAlias, out string errorMessage)
        {
            locationNameSortAlias = null;
            errorMessage = null;

            try
            {
                int number, idx1, idx2;
                string str, pageAlias, menuAlias, sub1, sub2, tabAlias;

                idx1 = 0;
                idx2 = 4;

                if (!LocationNameCheck(locationName, idx1, idx2, "Page", "Menu", 1, 15, out number, out errorMessage))
                    return false;
                pageAlias = (number < 10) ? string.Format("0{0}", number.ToString()) : number.ToString();
                idx1 += (4 + ((number < 10) ? 1 : 2));
                idx2 += (4 + ((number < 10) ? 1 : 2));

                if (!LocationNameCheck(locationName, idx1, idx2, "Menu", "Sub", 0, 10, out number, out errorMessage))
                    return false;
                menuAlias = (number < 10) ? string.Format("0{0}", number.ToString()) : number.ToString();
                idx1 += (4 + ((number < 10) ? 1 : 2));
                idx2 += (3 + ((number < 10) ? 1 : 2));

                if (!LocationNameCheck(locationName, idx1, idx2, "Sub", "Sub", 0, 5, out number, out errorMessage))
                    return false;
                sub1 = number.ToString();
                idx1 += 4;
                idx2 += 4;

                if (!LocationNameCheck(locationName, idx1, idx2, "Sub", "Tab", 0, 5, out number, out errorMessage))
                    return false;
                sub2 = number.ToString();

                idx1 = locationName.IndexOf("Tab");

                if ((3 + idx1) == locationName.Length)
                {
                    errorMessage = string.Format("ERROR!! Method \"LocationNameIsCorrect\" found that the following location name is incorrect (there is not a number after \"Tab\"): {0}", locationName);
                    return false;
                }

                str = locationName.Substring(3 + idx1);

                if (!int.TryParse(str, out number))
                {
                    errorMessage = string.Format("ERROR!! Method \"LocationNameIsCorrect\" found that the following location name is incorrect (there is not a number after \"Tab\"): {0}", locationName);
                    return false;
                }

                if (number < 1 || number > 10)
                {
                    errorMessage = string.Format("ERROR!! Method \"LocationNameIsCorrect\" found that the following location name is incorrect (the number, {0}, after \"Tab\" is not in the following range[1, 10]): {1}", number.ToString(), locationName);
                    return false;
                }

                tabAlias = (number < 10) ? string.Format("0{0}", number.ToString()) : number.ToString();

                locationNameSortAlias = string.Format("Page{0}Menu{1}Sub{2}Sub{3}Tab{4}", pageAlias, menuAlias, sub1, sub2, tabAlias);
            }
            catch (Exception e)
            {
                errorMessage = string.Format("ERROR!! An Exception occured in method LocationNameIsCorrect! e.Message:\r\n{0}", e.Message);
                return false;
            }

            return true;
        }
    }
}