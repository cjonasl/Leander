using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Leander.Nr1;

namespace InsertWriteStatements
{
    public class JsFile
    {
        private string _folder;
        private int _errors;

        public JsFile(string folder)
        {
            this._folder = folder;
            this._errors = 0;
        }

        private string ReplaceOnlyNewRowCharWithCarriageReturnAndNewRow(string fileNameFullPath, string fileContents)
        {
            int numberOfReplace, i;
            ArrayList v;
            string returnString;
            char[] charArray;

            numberOfReplace = 0;
            v = new ArrayList();

            for (i = 0; i < fileContents.Length; i++)
            {
                if ((i > 0) && (fileContents[i - 1] != '\r') && (fileContents[i] == '\n'))
                {
                    v.Add('\r');
                    numberOfReplace++;
                }

                v.Add(fileContents[i]);
            }

            if (numberOfReplace > 0)
            {
                charArray = new char[v.Count];

                for (i = 0; i < v.Count; i++)
                {
                    charArray[i] = (char)v[i];
                }

                returnString = new string(charArray);
            }
            else
            {
                returnString = fileContents;
            }

            return returnString;
        }

        private string FixSuchThatCurlyBraceNotFirstInLine(string fileContents)
        {
            ArrayList v;
            int i;
            char[] charArray;
            string returnString;

            v = new ArrayList();
            i = 0;

            while (i < fileContents.Length)
            {
                if (((i + 2) < fileContents.Length) && (fileContents[i] == '\r') && (fileContents[i + 1] == '\n') && (fileContents[i + 2] == '{'))
                {
                    v.Add(' ');
                    v.Add('{');
                    v.Add('\r');
                    v.Add('\n');
                    i += 3;
                }
                else
                {
                    v.Add(fileContents[i]);
                    i++;
                }
            }

            charArray = new char[v.Count];

            for (i = 0; i < v.Count; i++)
            {
                charArray[i] = (char)v[i];
            }

            returnString = new string(charArray);

            return returnString;
        }

        private int ReturnWriteIndex(string row, int startIndex)
        {
            bool leftParentesFound = false;
            bool rightParentesFound = false;
            bool continueSearch = true;
            int writeIndex = -1; //Default
            int i = 0;
            char c;
            Stack stack = new Stack();

            while (((i + startIndex) < row.Length) && continueSearch)
            {
                c = row[startIndex + i];

                if ((c == '(') && (!leftParentesFound || !rightParentesFound))
                {
                    leftParentesFound = true;

                    if (stack.Count == 0)
                    {
                        stack.Push('(');
                    }
                    else
                    {
                        if ((char)stack.Peek() == ')') //Error
                        {
                            continueSearch = false;
                        }
                        else
                        {
                            stack.Push('(');
                        }
                    }
                }
                else if ((c == ')') && (!leftParentesFound || !rightParentesFound))
                {
                    if (stack.Count == 0) //Error
                    {
                        continueSearch = false;
                    }
                    else
                    {
                        stack.Pop();

                        if (stack.Count == 0)
                        {
                            rightParentesFound = true;
                        }
                    }
                }
                else if (c == '{')
                {
                    continueSearch = false;

                    if (leftParentesFound && rightParentesFound)
                    {
                        writeIndex = startIndex + i + 1;
                    }
                }

                i++;
            }

            return writeIndex;
        }

        private string ReturnNewRow(string row, int rowNumber, string fileName)
        {
            int index, writeIndex;
            string newRow, s1, s2, s3;

            newRow = row; //Default

            index = row.IndexOf("function");

            if (index >= 0)
            {
                writeIndex = ReturnWriteIndex(row, 8 + index);

                if (writeIndex >= 0)
                {
                    s1 = row.Substring(0, writeIndex);

                    s2 = string.Format("console.log(\"{0}\"); ", "Row: " + rowNumber.ToString() + ", File: " + fileName + ", " + row.Replace('"', '\''));

                    if (writeIndex < row.Length)
                    {
                        s3 = row.Substring(writeIndex);
                    }
                    else
                    {
                        s3 = "";
                    }

                    newRow = s1 + s2 + s3;
                }
                else
                {
                    throw new Exception(string.Format("Failed to parse row {0} in file {1}", rowNumber.ToString(), fileName));
                }
            }

            return newRow;
        }

        public bool Handle(string fileNameFullPath)
        {
            string fileContents, fileName, newFileContent;
            string[] rows;
            int i;
            StringBuilder sb;
            Encoding encoding;

            try
            {
                fileContents = Utility.ReturnFileContents(fileNameFullPath, out encoding);
                fileContents = ReplaceOnlyNewRowCharWithCarriageReturnAndNewRow(fileNameFullPath, fileContents);
                fileContents = FixSuchThatCurlyBraceNotFirstInLine(fileContents);

                fileName = (new FileInfo(fileNameFullPath)).Name;

                rows = fileContents.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                for (i = 0; i < rows.Length; i++)
                {
                    rows[i] = ReturnNewRow(rows[i], 1 + i, fileName);
                }

                sb = new StringBuilder();
                sb.Append("//CarlJonasLeander\r\n\r\n");

                for (i = 0; i < rows.Length; i++)
                {
                    sb.Append(rows[i] + "\r\n");
                }

                newFileContent = sb.ToString().TrimEnd();
                Utility.CreateNewFile(fileNameFullPath, newFileContent, encoding);
            }
            catch (Exception e)
            {
                _errors++;
                Utility.CreateNewFile(string.Format("{0}\\Errors\\jsError{1}.txt", this._folder, _errors.ToString()), string.Format("Error when handle file: {0}. e.Message:\r\n{1}", fileNameFullPath, e.Message));
                return false;
            }

            return true;
        }
    }
}
