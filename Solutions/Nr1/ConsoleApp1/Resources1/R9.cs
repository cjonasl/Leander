using System.Collections;
using System.IO;
using System.Text;

namespace Leander.Nr1
{

    /// <summary>
    /// Remove multiline comments in a file
    /// </summary>
    public static class R9
    {
        public static void Execute(string fileNameFullPath1, string fileNameFullPath2)
        {
            string errorMessage, str;
            ArrayList v;
            StringBuilder sb;
            int index, indexStart, indexEnd, length;

            string fileContents = Utility.ReturnFileContents(fileNameFullPath1, out errorMessage);

            if (!File.Exists(fileNameFullPath2))
            {
                Utility.Print("The following file does not exist: " + fileNameFullPath2);
                return;
            }

            if (errorMessage != null)
            {
                Utility.Print(errorMessage);
                return;
            }

            v = new ArrayList();
            v.Add(0);
            indexStart = fileContents.IndexOf("/*");
            indexEnd = fileContents.IndexOf("*/");

            if (indexEnd < indexStart)
            {
                Utility.Print(string.Format("IndexEnd ({0}) is found to be less than indexStart ({1})!", indexEnd.ToString(), indexStart.ToString()));
                return;
            }

            while (indexStart != -1)
            {
                v.Add(indexStart - 1);
                v.Add(2 + indexEnd);

                if ((2 + indexEnd) >= fileContents.Length)
                {
                    indexStart = -1;
                }
                else
                {
                    indexStart = fileContents.IndexOf("/*", 2 + indexEnd);
                    indexEnd = fileContents.IndexOf("*/", 2 + indexEnd);

                    if (indexEnd < indexStart)
                    {
                        Utility.Print(string.Format("IndexEnd ({0}) is found to be less than indexStart ({1})!", indexEnd.ToString(), indexStart.ToString()));
                        return;
                    }
                }
            }

            v.Add(fileContents.Length - 1);

            sb = new StringBuilder();
            index = 0;

            while (index < v.Count)
            {
                if ((int)v[1 + index] > (int)v[index]) //Equal if /* is first in file
                {
                    indexStart = (int)v[index];
                    length = (int)v[1 + index] - (int)v[index] + 1;
                    str = fileContents.Substring(indexStart, length).Trim();

                    if (!string.IsNullOrEmpty(str))
                    {
                        sb.Append(str + "\r\n\r\n");
                    }
                }
                index += 2;
            }

            Utility.CreateNewFile(fileNameFullPath2, sb.ToString().Trim());
        }
    }
}
