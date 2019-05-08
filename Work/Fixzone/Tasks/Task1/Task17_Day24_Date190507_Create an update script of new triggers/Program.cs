using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateAnUpdateScriptOfNewTriggers
{
    class Program
    {
        static void Main(string[] args)
        {
            string str;
            int trId;
            string[] triggerId;
            StringBuilder sb = new StringBuilder();

            string[] v = Directory.GetFiles("C:\\tmp");

            triggerId = new string[v.Length];

            for (int i = 0; i < v.Length; i++)
            {
                trId = ReturnTriggerID(v[i]);

                if (Leander.Nr1.Utility.ReturnFileContents(v[i]).IndexOf(string.Format("res.TRIGGERID = {0}", trId.ToString())) == -1)
                {
                    throw new Exception(v[i]);
                }

                triggerId[i] = trId.ToString();
            }

            for (int i = 0; i < v.Length; i++)
            {
                str = Leander.Nr1.Utility.ReturnFileContents(v[i]);
                sb.Append(string.Format("UPDATE [Triggers]\r\nSET TRIGGERSQL = '{0}'\r\nWHERE TRIGGERID = {1}\r\n\r\n", str.Replace("'", "''"), (string)triggerId[i]));
            }

            Leander.Nr1.Utility.CreateNewFile("C:\\tmp\\UpdateScript.sql", sb.ToString());
        }

        private static int ReturnTriggerID(string fileNameFullPath)
        {
            int index1, index2;

            index1 = fileNameFullPath.IndexOf(' ');
            index2 = fileNameFullPath.IndexOf('.');

            return int.Parse(fileNameFullPath.Substring(1 + index1, index2 - index1 - 1));
        }
    }
}
