using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leander.Nr1
{
    public partial class Utility
    {
        public static void AddIfNotExistsAlready(ArrayList v, int n)
        {
            if (v.IndexOf(n) == -1)
            {
                v.Add(n);
            }
        }

        public static string ReturnItemsCommaSeparated(ArrayList v)
        {
            StringBuilder sb = new StringBuilder("");

            for (int i = 0; i < v.Count; i++)
            {
                if (i == 0)
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

        public static string ReturnItems(ArrayList v, string separator)
        {
            StringBuilder sb = new StringBuilder("");

            for (int i = 0; i < v.Count; i++)
            {
                if (i == 0)
                {
                    sb.Append(v[i].ToString());
                }
                else
                {
                    sb.Append(separator + v[i].ToString());
                }
            }

            return sb.ToString().Trim();
        }

        public static bool PhrasesInArrayListAreAllPresentInCommaSeparatedListWithPhrases(ArrayList v, string commaSeparatedListWithPhrases)
        {
            string[] phrases;
            ArrayList arrayListPhrases;
            int i;
            bool returnValue = true;

            phrases = commaSeparatedListWithPhrases.Split(',');
            arrayListPhrases = new ArrayList(phrases);

            i = 0;

            while ((i < v.Count) && returnValue)
            {
                if (arrayListPhrases.IndexOf(v[i]) == -1)
                    returnValue = false;
                else
                    i++;
            }

            return returnValue;
        }

        public static bool AtLeastOnePhraseInArrayListIsPresentInCommaSeparatedListWithPhrases(ArrayList v, string commaSeparatedListWithPhrases)
        {
            string[] phrases;
            ArrayList arrayListPhrases;
            int i;
            bool returnValue = false;

            phrases = commaSeparatedListWithPhrases.Split(',');
            arrayListPhrases = new ArrayList(phrases);

            i = 0;

            while ((i < v.Count) && !returnValue)
            {
                if (arrayListPhrases.IndexOf(v[i]) >= 0)
                    returnValue = true;
                else
                    i++;
            }

            return returnValue;
        }
    }
}
