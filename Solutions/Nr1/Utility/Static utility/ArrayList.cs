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
    }
}
