using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leander.Nr1
{
    public partial class Utility
    {
        public static bool WordStartWithUpperCaseLetterAndThenOnlyLowerCaseAndDigits(string word)
        {
            bool returnValue = true; //Default
            int i = 0;

            if (word.Length == 0)
                return false;

            while ((i < word.Length) && (returnValue))
            {
                if (!char.IsLetterOrDigit(word[i]))
                    returnValue = false;
                else
                {
                    if ((i == 0) && (char.IsDigit(word[0])))
                        returnValue = false;
                    else if ((i == 0) && (char.IsLower(word[0])))
                        returnValue = false;
                    else if ((i > 0) && char.IsUpper(word[0]))
                        returnValue = false;
                }

                i++;
            }

            return returnValue;
        }

        public static bool NavigationRowIsCorrect(string row, int rowNr, out string errorMessage)
        {
            errorMessage = null;
            string[] words;
            int n, i = 0;

            n = row.IndexOf("  ");
            n++;

            if (n >= 1)
            {
                errorMessage = string.Format("A double space is found on row {0} position {1}, which is not allowed!", rowNr.ToString(), n.ToString());
                return false;
            }

            while ((i < row.Length) && (errorMessage == null))
            {
                if ((row[i] != ' ') && (!char.IsLetterOrDigit(row[i])))
                {
                    n = i + 1;
                    errorMessage = string.Format("A non-allowed character is found on row {0} position {1}!", rowNr.ToString(), n.ToString());
                }

                i++;
            }

            words = row.Split(' ');

            i = 0;

            while ((i < words.Length) && (errorMessage == null))
            {
                if (WordStartWithUpperCaseLetterAndThenOnlyLowerCaseAndDigits(words[i]))
                {
                    n = i + 1;
                    errorMessage = string.Format("Word number {0} on row {1} does not start with upper case letter and then only digits and/or lower case letters as expeced!", n.ToString(), rowNr.ToString());
                }

                i++;
            }

            return (errorMessage == null);
        }

        public static bool NamesInArrayAreDistinct(string[] v, int startIndex, bool caseSensitive)
        {
            bool returnValue = true;
            string str;
            ArrayList al = new ArrayList();
            int i = startIndex;

            while ((i < v.Length) && (returnValue))
            {
                str = caseSensitive ? v[i] : v[i].ToLower();

                if (al.IndexOf(str) == -1)
                    al.Add(str);
                else
                    returnValue = false;

                i++;
            }

            return returnValue;
        }

        public static string ReturnItemsCommaSeparated(string[] v, int startIndex)
        {
            StringBuilder sb = new StringBuilder("");

            for (int i = startIndex; i < v.Length; i++)
            {
                if (i == startIndex)
                {
                    sb.Append(v[i].ToString());
                }
                else
                {
                    sb.Append(", " + v[i].ToString());
                }
            }

            return sb.ToString();
        }

        public static string ReturnMenuItemName(string row, int rowNr, out string[] subMenuItemNames, out string errorMessage)
        {
            string[] words;

            subMenuItemNames = null;
            errorMessage = null;

            words = row.Split(' ');

            if (words.Length > 1)
            {
                if (!NamesInArrayAreDistinct(words, 1, false))
                    errorMessage = string.Format("The sub item names given on row {0}, {1}, are not distinct!", rowNr.ToString(), ReturnItemsCommaSeparated(words, 1));
                else
                {
                    subMenuItemNames = new string[words.Length - 1];

                    for(int i = 1; i < words.Length; i++)
                    {
                        subMenuItemNames[i - 1] = words[i];
                    }
                }
            }

            return words[0];
        }

        public static int ReturnStartIndex(string row, int rowNr, ArrayList allowAbleStartIndexes, out string errorMessage)
        {
            int i = 0, startIndex = -1;

            errorMessage = null;

            while ((i < row.Length) && (startIndex == -1))
            {
                if (row[i] != ' ')
                    startIndex = i;
                else
                    i++;
            }

            if (startIndex == -1)
            {
                errorMessage = string.Format("Can't find start index on row {0}!", rowNr.ToString());
                return -1;
            }

            if (allowAbleStartIndexes.IndexOf(startIndex) == -1)
            {
                errorMessage = string.Format("Start index on row {0} is incorrect! It must be within the following list: {1}", rowNr.ToString(), ReturnItemsCommaSeparated(allowAbleStartIndexes));
                return -1;
            }

            return startIndex;
        }

        public static int ReturnIndexForChar(int startIndex, string str, char c)
        {
            int returnValue = -1;
            int i = startIndex;

            while ((i < str.Length) && (returnValue == -1))
            {
                if (str[i] == c)
                    returnValue = i;
                else
                    i++;
            }

            return returnValue;
        }

        public static void GetKeyValueOfAssignment(string expression, out string key, out string value)
        {
            string[] v = expression.Split('=');

            if (v.Length != 2)
                throw new Exception("Error in GetKeyValueOfAssignmen!");

            key = v[0].Trim();
            value = v[1].Trim();
        }

        public static string ReplaceCharWithAnotherCharWithinAString(string str, char from, char to)
        {
            char[] charArray = new char[str.Length];
            bool isWithinAString = false;
            int i;

            for(i = 0; i < str.Length; i++)
            {
                if ((str[i] == from) && (isWithinAString))
                    charArray[i] = to;
                else
                    charArray[i] = str[i];

                if ((str[i] == '"') && (!isWithinAString))
                    isWithinAString = true;
                else if ((str[i] == '"') && (isWithinAString))
                    isWithinAString = false;
            }

            return new string(charArray);
        }

        public static bool IsSearchTermUniqueInString(string str, string searchTerm, out int startIndexSearchTerm, out string errorMessage)
        {
            int index;

            errorMessage = null; //Default

            startIndexSearchTerm = str.IndexOf(searchTerm);

            if (startIndexSearchTerm == -1)
            {
                errorMessage = string.Format("The following string was not found as expected:\r\n{0}", searchTerm);
                return false;
            }
            else
            {
                index = str.IndexOf(searchTerm, startIndexSearchTerm + searchTerm.Length - 1);

                if (index >= 0)
                {
                    errorMessage = string.Format("The following string was found more than once:\r\n{0}", searchTerm);
                    return false;
                }
            }

            return true;
        }

        public static bool PhrasesInArrayListAreAllPresentInString(ArrayList v, string str)
        {
            int i;
            bool returnValue = true;

            i = 0;

            while ((i < v.Count) && returnValue)
            {
                if (str.IndexOf((string)v[i]) == -1)
                    returnValue = false;
                else
                    i++;
            }

            return returnValue;
        }

        public static bool AtLeastOnePhraseInArrayListIsPresentInString(ArrayList v, string str)
        {
            int i;
            bool returnValue = false;

            i = 0;

            while ((i < v.Count) && !returnValue)
            {
                if (str.IndexOf((string)v[i]) >= 0)
                    returnValue = true;
                else
                    i++;
            }

            return returnValue;
        }
    }
}
