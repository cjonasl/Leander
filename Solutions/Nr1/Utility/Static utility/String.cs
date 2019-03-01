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

        public static bool PhraseIsUniqueInString(string str, string phrase, int shouldBeAtIndexOrAfter, out int idx)
        {
            int index, startIndex;

            idx = -1;

            index = str.IndexOf(phrase);

            if ((index == -1) || (index < shouldBeAtIndexOrAfter))
                return false;

            startIndex = index + phrase.Length;

            if (str.Length > startIndex)
            {
                if (str.IndexOf(phrase, startIndex) > 0)
                    return false;
            }

            idx = index;

            return true;
        }

        public static string ReturnTextExceptFirstRow(string text, out string firstRow)
        {
            string fileContentsExceptFirstRow;
            int index;

            index = text.IndexOf("\r\n");

            if ((index == -1) || (text.Length < (3 + index)))
            {
                firstRow = text;
                fileContentsExceptFirstRow = "";
            }
            else
            {
                fileContentsExceptFirstRow = text.Substring(2 + index);
                firstRow = text.Substring(0, 2 + index);
            }

            return fileContentsExceptFirstRow;
        }

        /// <summary>
        /// Returns true if first row is correct, otherwise false
        /// </summary>
        public static bool CheckFirstRowInHtmlResource(string firstRow, bool isForIframe, out string firstRowTemplate, out int iFrameWidth, out int iFrameHeight, out int textareaWidth, out int textareaHeight, out string errorMessage)
        {
            int index1, index2, index3, index4;
            string str1, str2, str3, str4, iframeDimension;
            string[] v;

            //First row should be for example: <!DOCTYPE html> <!-- iframe dimension: [200,300] textarea dimension: [400px,500px] -->

            str1 = "<!DOCTYPE html> <!-- iframe dimension: [";
            str2 = "] textarea dimension: [";
            str3 = "px,";
            str4 = "px] -->\r\n";

            firstRowTemplate = null;
            iFrameWidth = 0;
            iFrameHeight = 0;
            textareaWidth = 0;
            textareaHeight = 0;
            errorMessage = null;

            //Error number 1
            if (!PhraseIsUniqueInString(firstRow, str1, 0, out index1))
            {
                errorMessage = "ERROR!! Error number 1 in method CheckFirstRowInHtmlResource";
                return false;
            }

            //Error number 2
            if (!PhraseIsUniqueInString(firstRow, str2, index1 + str1.Length, out index2))
            {
                errorMessage = "ERROR!! Error number 2 in method CheckFirstRowInHtmlResource";
                return false;
            }

            //Error number 3
            if (!PhraseIsUniqueInString(firstRow, str3, index2 + str2.Length, out index3))
            {
                errorMessage = "ERROR!! Error number 3 in method CheckFirstRowInHtmlResource";
                return false;
            }

            //Error number 4
            if (!PhraseIsUniqueInString(firstRow, str4, index3 + str3.Length, out index4))
            {
                errorMessage = "ERROR!! Error number 4 in method CheckFirstRowInHtmlResource";
                return false;
            }

            iframeDimension = firstRow.Substring(str1.Length, index2 - str1.Length);

            v = iframeDimension.Split(',');

            //Error number 5
            if (v.Length != 2)
            {
                errorMessage = "ERROR!! Error number 5 in method CheckFirstRowInHtmlResource";
                return false;
            }

            //Error number 6
            if (!int.TryParse(v[0], out iFrameWidth))
            {
                errorMessage = "ERROR!! Error number 6 in method CheckFirstRowInHtmlResource";
                return false;
            }

            //Error number 7
            if (!int.TryParse(v[1], out iFrameHeight))
            {
                errorMessage = "ERROR!! Error number 7 in method CheckFirstRowInHtmlResource";
                return false;
            }

            //Error number 8
            if ((iFrameWidth < 1) || (iFrameWidth > 10000) || (iFrameHeight < 1) || (iFrameHeight > 10000))
            {
                errorMessage = "ERROR!! Error number 8 in method CheckFirstRowInHtmlResource";
                return false;
            }

            //Error number 9
            if (!int.TryParse(firstRow.Substring(index2 + str2.Length, index3 - index2 - str2.Length), out textareaWidth))
            {
                errorMessage = "ERROR!! Error number 9 in method CheckFirstRowInHtmlResource";
                return false;
            }

            //Error number 10
            if (!int.TryParse(firstRow.Substring(index3 + str3.Length, index4 - index3 - str3.Length), out textareaHeight))
            {
                errorMessage = "ERROR!! Error number 10 in method CheckFirstRowInHtmlResource";
                return false;
            }

            if (isForIframe)
                firstRowTemplate = string.Format("<!DOCTYPE html> <!-- iframe dimension: [#####REPLACE#####] textarea dimension: [{0}px,{1}px] -->\r\n", textareaWidth.ToString(), textareaHeight.ToString());
            else
                firstRowTemplate = string.Format("<!DOCTYPE html> <!-- iframe dimension: [{0},{1}] textarea dimension: [#####REPLACE#####] -->\r\n", iFrameWidth.ToString(), iFrameHeight.ToString());

            return true;
        }

        public static string InsertText(string text, string textToInsert, int insertIndex)
        {
            return text.Substring(0, insertIndex) + textToInsert + text.Substring(insertIndex);
        }

        public static int ReturnIndex(string text, bool searchBackward, int startIndex, string pattern, int numberOfOccurenciesOfPattern)
        {
            int index, n, occurenciesOfPattern, length;

            index = startIndex;

            n = searchBackward ? -1 : 1;
            occurenciesOfPattern = 0;
            length = pattern.Length;

            while (index >= 0 && index < text.Length && occurenciesOfPattern < numberOfOccurenciesOfPattern)
            {
                if (text.Substring(index, length) == pattern)
                    occurenciesOfPattern++;

                if (occurenciesOfPattern < numberOfOccurenciesOfPattern)
                    index = index + n;
            }

            if (occurenciesOfPattern == numberOfOccurenciesOfPattern)
                return index;
            else
                return -1;
        }

        public static int ReturnMatchingIndex(string text, bool searchBackward, int startIndex, char matchingChar)
        {
            int index = startIndex;
            char c = text[startIndex];
            Stack stack = new Stack();
            int n = searchBackward ? -1 : 1;

            stack.Push(c);
            index = index + n;

            while (index >= 0 && index < text.Length && stack.Count > 0)
            {
                if (text[index] == c)
                    stack.Push(c);
                else if (text[index] == matchingChar)
                    stack.Pop();

                if (stack.Count > 0)
                    index = index + n;
            }

            if (stack.Count == 0)
                return index;
            else
                return -1;
        }

        public static char? ReturnNextNonWhiteSpaceChar(string text, bool searchBackward, int startIndex, out int idx)
        {
            int index, n;
            char? c = null;

            idx = -1;

            index = startIndex;

            n = searchBackward ? -1 : 1;

            while (index >= 0 && index < text.Length && c == null)
            {
                if (!char.IsWhiteSpace(text[index]))
                {
                    c = text[index];
                    idx = index;
                }
                else
                    index = index + n;
            }

            return c;
        }
    }
}
