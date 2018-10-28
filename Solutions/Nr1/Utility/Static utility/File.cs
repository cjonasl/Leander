using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Leander.Nr1
{
    public partial class Utility
    {
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

        public static string ReturnFileContents(string fileNameFullPath)
        {
            FileStream fileStream = new FileStream(fileNameFullPath, FileMode.Open, FileAccess.Read);
            StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8);
            string str = streamReader.ReadToEnd();
            streamReader.Close();
            fileStream.Close();

            return str;
        }

        public static string ReturnFileContents(string fileNameFullPath, out string errorMessage)
        {
            errorMessage = null;

            if (!File.Exists(fileNameFullPath))
            {
                errorMessage = string.Format("The following file does not exist as expected: {0}", fileNameFullPath);
                return null;
            }

            if (!FileIsUTF8(fileNameFullPath))
            {
                errorMessage = string.Format("The following file is not in encoding UTF8 as expected: {0}", fileNameFullPath);
                return null;
            }

            FileStream fileStream = new FileStream(fileNameFullPath, FileMode.Open, FileAccess.Read);
            StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8);
            string str = streamReader.ReadToEnd();
            streamReader.Close();
            fileStream.Close();

            if (str.Trim() == string.Empty)
            {
                errorMessage = string.Format("The following does not have contents: {0}", fileNameFullPath);
                return null;
            }

            return str;
        }

        public static bool FileIsUTF8(string fileNameFullPath)
        {
            bool fileIsUTF8;

            byte[] bom = new byte[4];
            int n;

            using (var file = new FileStream(fileNameFullPath, FileMode.Open, FileAccess.Read))
            {
                n = file.Read(bom, 0, 4);
            }

            if ((n >= 3) && (bom[0] == 0xef) && (bom[1] == 0xbb) && (bom[2] == 0xbf))
                fileIsUTF8 = true;
            else
                fileIsUTF8 = false;

            return fileIsUTF8;
        }

        public static Encoding GetEncoding(string fileNameFullPath)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(fileNameFullPath, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
            return Encoding.ASCII;
        }

        public static void CreateNewFile(string fileNameFullPath, string newFileNameFullPath, string fileContent)
        {
            Encoding encoding;

            if (fileNameFullPath == "utf8")
            {
                encoding = Encoding.UTF8;
            }
            else if (fileNameFullPath == "ascii")
            {
                encoding = Encoding.ASCII;
            }
            else
            {
                encoding = GetEncoding(fileNameFullPath);
            }

            FileStream fileStream = new FileStream(newFileNameFullPath, FileMode.Create, FileAccess.Write);
            StreamWriter streamWriter = new StreamWriter(fileStream, encoding);
            streamWriter.Write(fileContent);
            streamWriter.Flush();
            fileStream.Flush();
            streamWriter.Close();
            fileStream.Close();
        }
    }
}
