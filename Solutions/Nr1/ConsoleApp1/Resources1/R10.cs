using System;
using System.IO;
using System.Text;

namespace Leander.Nr1
{
    /// <summary>
    /// Create a file with specified size (bug #20181122.1 File size restriction on document upload for objects)
    /// size = Number of chars, for example 1024*1024 for 1Mb
    /// </summary>
    public static class R10
    {
        public static void Execute(int size, string fileNameFullPath)
        {
            if (!File.Exists(fileNameFullPath))
            {
                Utility.Print("The following file does not exist: " + fileNameFullPath);
                return;
            }

            StringBuilder sb = new StringBuilder();

            for(int i = 0; i < size; i++)
            {
                sb.Append("A");
            }

            Utility.CreateNewFile(fileNameFullPath, sb.ToString());
        }
    }
}
