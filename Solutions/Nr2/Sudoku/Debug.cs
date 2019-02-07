using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public static class Debug
    {
        public static string ReturnMap(int[][] v)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < 9; i++)
            {
                sb.Append(string.Format("[{0},{1}] ", v[i][0], v[i][1]));
            }

            return sb.ToString().TrimEnd();
        }

        public static string ReturnFullMapp(int[][][] v)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < 9; i++)
            {
                sb.Append(ReturnMap(v[i]) + "\r\n");
            }

            return sb.ToString().TrimEnd();
        }

        public static void CreateNewFile(string fileNameFullPath, string fileContent)
        {
            FileStream fileStream = new FileStream(fileNameFullPath, FileMode.Create, FileAccess.Write);
            StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
            streamWriter.Write(fileContent);
            streamWriter.Flush();
            fileStream.Flush();
            streamWriter.Close();
            fileStream.Close();
        }
    }
}
