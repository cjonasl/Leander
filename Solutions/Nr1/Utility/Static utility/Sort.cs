using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leander.Nr1
{
    public partial class Utility
    {
        public static void Sort(ArrayList v1, ArrayList v2)
        {
            string tmp;

            for (int i = 0; i < (v1.Count - 1); i++)
            {
                for (int j = (i + 1); j < v1.Count; j++)
                {
                    if (string.Compare((string)v1[j], (string)v1[i]) < 0)
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

        public static void Sort(ArrayList v1, ArrayList v2, ArrayList v3)
        {
            string tmp;

            for (int i = 0; i < (v1.Count - 1); i++)
            {
                for (int j = (i + 1); j < v1.Count; j++)
                {
                    if (string.Compare((string)v1[j], (string)v1[i]) < 0)
                    {
                        tmp = (string)v1[j];
                        v1[j] = v1[i];
                        v1[i] = tmp;

                        tmp = (string)v2[j];
                        v2[j] = v2[i];
                        v2[i] = tmp;

                        tmp = (string)v3[j];
                        v3[j] = v3[i];
                        v3[i] = tmp;
                    }
                }
            }
        }

        public static void Sort(ArrayList v1, ArrayList v2, ArrayList v3, ArrayList v4)
        {
            string tmp;

            for (int i = 0; i < (v1.Count - 1); i++)
            {
                for (int j = (i + 1); j < v1.Count; j++)
                {
                    if (string.Compare((string)v1[j], (string)v1[i]) < 0)
                    {
                        tmp = (string)v1[j];
                        v1[j] = v1[i];
                        v1[i] = tmp;

                        tmp = (string)v2[j];
                        v2[j] = v2[i];
                        v2[i] = tmp;

                        tmp = (string)v3[j];
                        v3[j] = v3[i];
                        v3[i] = tmp;

                        tmp = (string)v4[j];
                        v4[j] = v4[i];
                        v4[i] = tmp;
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
