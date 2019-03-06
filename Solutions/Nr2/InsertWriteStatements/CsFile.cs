using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leander.Nr1;

namespace InsertWriteStatements
{
    public class CsFile
    {
        private string _formatString, _folder;
        private int _errors;

        public CsFile(string folder)
        {
            string tmp = "global::System.IO.File.AppendAllText(\"#####REPLACE#####\\\\Log.txt\", \"[{0}] [{1}] [{2}]\\r\\n\", global::System.Text.Encoding.UTF8);";
            _formatString = tmp.Replace("#####REPLACE#####", folder.Replace("\\", "\\\\"));
            this._folder = folder;
            this._errors = 0;
        }

        public bool Handle(string fileNameFullPath)
        {
            string fileContents, str1, str2, fileName;
            bool insertHasBeenMade = false;
            char? c;
            int index, idx1, idx2, idx3;
            Encoding encoding;

            try
            {
                fileContents = Utility.ReturnFileContents(fileNameFullPath, out encoding);
                fileName = fileNameFullPath.Substring(1 + fileNameFullPath.LastIndexOf('\\'));

                index = 0;

                while (index < fileContents.Length)
                {
                    if (fileContents[index] == '{')
                    {
                        c = Utility.ReturnNextNonWhiteSpaceChar(fileContents, true, index - 1, out idx1);

                        if (c.HasValue && c.Value == ')')
                        {
                            idx2 = Utility.ReturnMatchingIndex(fileContents, true, idx1, '(');

                            if (idx2 != -1)
                            {
                                idx3 = Utility.ReturnIndex(fileContents, true, idx2 - 1, "\r\n", 1);

                                if (idx3 != -1)
                                {
                                    str1 = fileContents.Substring(idx3, idx2 - idx3);

                                    if (
                                        (str1.Trim() != "if") &&
                                        (str1.Trim() != "while") &&
                                        (str1.Trim() != "foreach") &&
                                        (str1.Trim() != "do") &&
                                        (str1.Trim() != "for") &&
                                        (str1.Trim() != "using") &&
                                        (str1.Trim() != "this") &&
                                        (str1.Trim() != "catch")
                                        )
                                    {
                                        str2 = string.Format(_formatString, fileName, str1.Trim(), fileNameFullPath.Replace("\\", "\\\\"));
                                        fileContents = Utility.InsertText(fileContents, str2, index + 1);
                                        index = index + str2.Length + 1;
                                        insertHasBeenMade = true;
                                    }
                                }
                            }
                        }
                    }

                    if (insertHasBeenMade)
                        insertHasBeenMade = false;
                    else
                        index++;
                }

                Utility.CreateNewFile(fileNameFullPath, "//CarlJonasLeander\r\n" + fileContents, encoding);
            }
            catch (Exception e)
            {
                _errors++;
                Utility.CreateNewFile(string.Format("{0}\\Errors\\csError{1}.txt", this._folder, _errors.ToString()), string.Format("Error when handle file: {0}. e.Message:\r\n{1}", fileNameFullPath, e.Message));
                return false;
            }

            return true;
        }

        private bool ShouldWriteMethodName(string[] rows, int currentRow)
        {
            if (currentRow == 1)
                return false;
            else if (currentRow == rows.Length)
                return false;
            else
            {
                bool b;

                b = ((rows[currentRow - 2].IndexOf(" class ") == -1) && ((rows[currentRow - 2].IndexOf(" public ") >= 0) || (rows[currentRow - 2].IndexOf(" private ") >= 0) || (rows[currentRow - 2].IndexOf(" protected ") >= 0)));

                if (!b)
                    return false;

                if (rows[currentRow - 1].Trim() != "{")
                    return false;

                if (rows[currentRow].IndexOf("get") >= 0)
                    return false;

                return true;
            }
        }
    }
}


