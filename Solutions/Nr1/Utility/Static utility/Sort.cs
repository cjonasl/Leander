using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leander.Nr1
{
    public partial class Utility
    {
        public static bool IsLower(string str1, string str2)
        {
            ArrayList v = new ArrayList();

            v.Add(str1);
            v.Add(str2);

            v.Sort();

            if (((string)v[0]) == str1)
                return true;
            else
                return false;
        }

        public static void Sort(ArrayList v1, ArrayList v2)
        {
            string tmp;

            for (int i = 0; i < (v1.Count - 1); i++)
            {
                for (int j = (i + 1); j < v1.Count; j++)
                {
                    if (IsLower((string)v1[j], (string)v1[i]))
                    {
                        tmp = (string)v1[j];
                        v1[j] = v1[i];
                        v1[i] = tmp;

                        tmp = (string)v2[j];
                        v2[j] = v2[i];
                        v2[i] = tmp;
                    }
                }
            }
        }

        public static void Sort2(ArrayList v1, ArrayList v2)
        {
            string tmp;
            int a, b;

            for (int i = 0; i < (v1.Count - 1); i++)
            {
                for (int j = (i + 1); j < v1.Count; j++)
                {
                    a = (int)v1[i];
                    b = (int)v1[j];

                    if (a < b)
                    {
                        v1[i] = b;
                        v1[j] = a;

                        tmp = (string)v2[j];
                        v2[j] = v2[i];
                        v2[i] = tmp;
                    }
                }
            }
        }
    }
}
