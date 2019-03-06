using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leander.Nr1;

namespace InsertWriteStatements
{
    public class CshtmlFile
    {
        private string _formatString, _folder;
        private int _errors;

        public CshtmlFile(string folder)
        {
            string tmp = "global::System.IO.File.AppendAllText(\"#####REPLACE#####\\\\Log.txt\", \"[{0}] [{1}]\\r\\n\", global::System.Text.Encoding.UTF8);";
            _formatString = tmp.Replace("#####REPLACE#####", folder.Replace("\\", "\\\\"));
            this._folder = folder;
            this._errors = 0;
        }

        public bool Handle(string fileNameFullPath)
        {
            string fileContents, newFileContent, fileName;
            Encoding encoding;

            try
            {
                fileContents = Utility.ReturnFileContents(fileNameFullPath, out encoding);
                fileName = fileNameFullPath.Substring(1 + fileNameFullPath.LastIndexOf('\\'));
                newFileContent = fileContents + "\r\n@{\r\n//CarlJonasLeander\r\n" + string.Format(_formatString, fileName, fileNameFullPath.Replace("\\", "\\\\")) + "\r\n }";
                Utility.CreateNewFile(fileNameFullPath, newFileContent, encoding);
            }
            catch (Exception e)
            {
                _errors++;
                Utility.CreateNewFile(string.Format("{0}\\Errors\\cshtmlError{1}.txt", this._folder, _errors.ToString()), string.Format("Error when handle file: {0}. e.Message:\r\n{1}", fileNameFullPath, e.Message));
                return false;
            }

            return true;
        }
    }
}

