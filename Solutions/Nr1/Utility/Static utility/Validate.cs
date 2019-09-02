using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Leander.Nr1
{
    public partial class Utility
    {
        public static bool IconExists(string icon)
        {
            bool returnValue;
            string errorMessage;

            ArrayList v = ReturnRowsInFileInArrayList("C:\\git_cjonasl\\Leander\\Design Leander\\Font Awesome free icons__Without_fasfa.txt", out errorMessage);

            if (v.IndexOf(icon) >= 0) 
                returnValue = true;
            else  
                returnValue = false;

            return returnValue;
        }

        public static bool AllDirectoriesExist(string[] directories, out string errorMessage)
        {
            bool returnValue = true; //Default
            int i = 0, n = directories.Length;

            errorMessage = null;

            while (i < n && returnValue)
            {
                if (!Directory.Exists(directories[i]))
                {
                    errorMessage = string.Format("The following directory soes not exist: {0}", directories[i]);
                    returnValue = false;
                }
                else
                {
                    i++;
                }
            }

            return returnValue;
        }

        public static bool ContentsInFilesAreSame(string[] fileNameFulPath1, string[] fileNameFulPath2, out string errorMessage)
        {
            bool returnValue = true; //Default
            int i = 0, n = fileNameFulPath1.Length;
            string s1, s2;

            errorMessage = null;

            if (fileNameFulPath1.Length != fileNameFulPath2.Length)
            {
                errorMessage = string.Format("ERROR in method ContentsInFilesAreSame! Array length of fileNameFulPath1 ({0}) and fileNameFulPath2 ({1})  are not same!", fileNameFulPath1.Length, fileNameFulPath2.Length);
                return false;
            }

            while (i < n && returnValue)
            {
                s1 = File.ReadAllText(fileNameFulPath1[i]);
                s2 = File.ReadAllText(fileNameFulPath2[i]);

                if (s1 != s2)
                {
                    errorMessage = string.Format("ERROR!! The contents in the following two files are not same: {0} and {1}", fileNameFulPath1[i], fileNameFulPath2[i]);
                    returnValue = false;
                }
                else
                {
                    i++;
                }
            }

            return returnValue;
        }

    }
}
