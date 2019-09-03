using System;
using System.Collections;
using System.IO;
using System.Text;
using Leander.Nr1;

namespace InsertLogStatements
{
    class Program
    {
        static void Main(string[] args)
        {
            string errorMessage, fileNameFullPathFilesMoved;

            try
            {
                if (args.Length != 1)
                {
                    Console.Write("ERROR!! The program should have exactly one command argument!");
                    return;
                }

                fileNameFullPathFilesMoved = args[0] + "\\FilesMoved.txt";

                if (!File.Exists(fileNameFullPathFilesMoved))
                {
                    Console.Write("ERROR!! The following files does not exist: {0}", fileNameFullPathFilesMoved);
                    return;
                }

                string[] directories = new string[3]
                {
                    args[0] + "\\Original files",
                    args[0] + "\\Files with log statements inserted",
                    args[0] + "\\Log"
                };

                if (!Utility.AllDirectoriesExist(directories, out errorMessage))
                {
                    Console.Write(errorMessage);
                    return;
                }

                FileInfo fi = new FileInfo(fileNameFullPathFilesMoved);

                if (fi.Length == 1)
                {
                    ProcessInsertLogStatements(args[0]);
                }
                else
                {
                    Reset(fileNameFullPathFilesMoved, directories[0], directories[1]);
                }
            }
            catch(Exception e)
            {
                Console.Write(string.Format("ERROR!! An exception happened! e.Message = {0}", e.Message));
            }
        }

        private static string ReturnShortFileName(string fileNameFullPath)
        {
            int index = 1 + fileNameFullPath.LastIndexOf("\\");
            return fileNameFullPath.Substring(index);
        }

        private static int ReturnUniquePosition(string fileNameFullPath, string searchString, out string errorMessage)
        {
            int position, tmp1, tmp2;
            char c = '0';

            errorMessage = null;

            string fileContents = File.ReadAllText(fileNameFullPath);

            position = fileContents.IndexOf(searchString);

            if (position == -1)
            {
                errorMessage = "The serach string is not found in file!";
            }
            else
            {
                tmp1 = position + searchString.Length;

                if (tmp1 + searchString.Length <= fileContents.Length)
                {
                    tmp2 = fileContents.IndexOf(searchString, tmp1);

                    if (tmp2 >= 0)
                    {
                        errorMessage = "The serach string is not unique in file!";
                        position = -1;
                    }
                }
            }

            if (position >= 0)
            {
                position += searchString.Length;

                while (c != '{' && position < fileContents.Length && position >= 0)
                {
                    c = fileContents[position];

                    if (c == '\r' || c == '\n' || c == ' ' || c == '{')
                    {
                        position++;
                    }
                    else
                    {
                        errorMessage = "Can not find character { after search string!";
                        position = -1;
                    }
                }
            }

            return position;
        }

        private static void InsertNewLogStatement(int position, string insertStatement, ArrayList arrayListInsertPosition, ArrayList arrayListInsertStatement)
        {
            int p, index = 0, i = 0;
            bool foundIndex = false;

            if (arrayListInsertPosition.Count == 0)
            {
                arrayListInsertPosition.Add(position);
                arrayListInsertStatement.Add(insertStatement);
            }
            else
            {
                p = (int)arrayListInsertPosition[i];

                while (!foundIndex)
                {
                    if (position < p)
                    {
                        index = i;
                        foundIndex = true;
                    }
                    else if (i == (arrayListInsertPosition.Count - 1))
                    {
                        index = i + 1;
                        foundIndex = true;
                    }
                    else
                    {
                        i++;
                        p = (int)arrayListInsertPosition[i];
                    }
                }

                if (index == arrayListInsertPosition.Count)
                {
                    arrayListInsertPosition.Add(position);
                    arrayListInsertStatement.Add(insertStatement);
                }
                else
                {
                    arrayListInsertPosition.Insert(index, position);
                    arrayListInsertStatement.Insert(index, insertStatement);
                }
            }
        }

        private static void CreateFileForMovedFiles(ArrayList arrayListOriginalFiles, string fileNameFullPath)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < arrayListOriginalFiles.Count; i++)
            {
                sb.Append((string)arrayListOriginalFiles[i] + "\r\n");
            }

            File.WriteAllText(fileNameFullPath, sb.ToString().TrimEnd());
        }

        private static void InsertLogStatements(string fileNameFullPath, ArrayList arrayListInsertPosition, ArrayList arrayListInsertStatement)
        {
            int i, n = 0;
            string str1, str2, fileContents = File.ReadAllText(fileNameFullPath);

            for (i = 0; i < arrayListInsertPosition.Count; i++)
            {
                str1 = fileContents.Substring(0, n + (int)arrayListInsertPosition[i]);
                str2 = fileContents.Substring(n + (int)arrayListInsertPosition[i]);
                fileContents = str1 + (string)arrayListInsertStatement[i] + str2;
                n += ((string)arrayListInsertStatement[i]).Length;
            }

            File.WriteAllText(fileNameFullPath, fileContents);
        }

        private static void ProcessInsertLogStatements(string dir)
        {
            string fileContents, shortFileName, fileNameFullPathErrorFound, errorMessage;
            int i, j, index, position;
            string[] insertInfoFiles, strArray1, strArray2, strArray3;
            ArrayList arrayListOriginalFiles, arrayListShortFileName, arrayListOfarrayListInsertPosition, arrayListOfarrayListInsertStatement, arrayListInsertPosition, arrayListInsertStatement;
            bool errorFound = false;

            arrayListOriginalFiles = new ArrayList();
            arrayListShortFileName = new ArrayList();
            arrayListOfarrayListInsertPosition = new ArrayList();
            arrayListOfarrayListInsertStatement = new ArrayList();

            fileNameFullPathErrorFound = dir + "\\ErrorFound.txt";

            insertInfoFiles = Directory.GetFiles(dir, "InsertInfo*");

            i = 0;
            while (i < insertInfoFiles.Length && !errorFound)
            {
                fileContents = File.ReadAllText(insertInfoFiles[i]);

                strArray1 = fileContents.Split(new string[] { "\r\n--NewInsert--\r\n" }, StringSplitOptions.None);

                j = 0;
                while (j < strArray1.Length && !errorFound)
                {
                    strArray2 = strArray1[j].Split(new string[] { "\r\n--InsertLogStatement--\r\n" }, StringSplitOptions.None);

                    if (strArray2.Length != 2)
                    {
                        Console.Write(string.Format("An error found in file {0} at insert instruction {1}!", ReturnShortFileName(insertInfoFiles[i]), j + 1));
                        File.WriteAllText(fileNameFullPathErrorFound, strArray1[j]);
                        errorFound = true;
                    }
                    else
                    {
                        strArray3 = strArray2[0].Split(new string[] { "\r\n" }, StringSplitOptions.None);

                        if (strArray3.Length != 2)
                        {
                            Console.Write(string.Format("An error found in file {0} at insert instruction {1}!", ReturnShortFileName(insertInfoFiles[i]), j + 1));
                            File.WriteAllText(fileNameFullPathErrorFound, strArray1[j]);
                            errorFound = true;
                        }
                        else
                        {
                            if (!File.Exists(strArray3[0]))
                            {
                                Console.Write(string.Format("An error found in file {0} at insert instruction {1}! The given file does not exist!", ReturnShortFileName(insertInfoFiles[i]), j + 1));
                                File.WriteAllText(fileNameFullPathErrorFound, strArray1[j]);
                                errorFound = true;
                            }
                            else
                            {
                                index = arrayListOriginalFiles.IndexOf(strArray3[0]);

                                if (index == -1)
                                {
                                    shortFileName = ReturnShortFileName(strArray3[0]);

                                    if (arrayListShortFileName.IndexOf(shortFileName) >= 0)
                                    {
                                        Console.Write(string.Format("An error found in file {0} at insert instruction {1}! The file name without path is not unique!", ReturnShortFileName(insertInfoFiles[i]), j + 1));
                                        File.WriteAllText(fileNameFullPathErrorFound, strArray1[j]);
                                        errorFound = true;
                                    }
                                    else
                                    {
                                        index = arrayListOriginalFiles.Count;
                                        arrayListOriginalFiles.Add(strArray3[0]);
                                        arrayListShortFileName.Add(shortFileName);
                                        arrayListOfarrayListInsertPosition.Add(new ArrayList());
                                        arrayListOfarrayListInsertStatement.Add(new ArrayList());
                                    }
                                }

                                if (!errorFound)
                                {
                                    position = ReturnUniquePosition(strArray3[0], strArray3[1], out errorMessage);

                                    if (position == -1)
                                    {
                                        Console.Write(string.Format("An error found in file {0} at insert instruction {1}! {2}", ReturnShortFileName(insertInfoFiles[i]), j + 1, errorMessage));
                                        File.WriteAllText(fileNameFullPathErrorFound, strArray1[j]);
                                        errorFound = true;
                                    }
                                    else
                                    {
                                        arrayListInsertPosition = (ArrayList)arrayListOfarrayListInsertPosition[index];
                                        arrayListInsertStatement = (ArrayList)arrayListOfarrayListInsertStatement[index];
                                        InsertNewLogStatement(position, strArray2[1], arrayListInsertPosition, arrayListInsertStatement);
                                    }
                                }
                            }
                        }
                    }

                    j++;
                }

                i++;
            }

            if (!errorFound)
            {
                CreateFileForMovedFiles(arrayListOriginalFiles, dir + "\\FilesMoved.txt");

                for (i = 0; i < arrayListOriginalFiles.Count; i++)
                {
                    File.Move((string)arrayListOriginalFiles[i], dir + "\\Original files\\" + arrayListShortFileName[i]);
                }

                for (i = 0; i < arrayListOriginalFiles.Count; i++)
                {
                    File.Copy(dir + "\\Original files\\" + arrayListShortFileName[i], (string)arrayListOriginalFiles[i]);
                }

                for (i = 0; i < arrayListOfarrayListInsertPosition.Count; i++)
                {
                    arrayListInsertPosition = (ArrayList)arrayListOfarrayListInsertPosition[i];
                    arrayListInsertStatement = (ArrayList)arrayListOfarrayListInsertStatement[i];
                    InsertLogStatements((string)arrayListOriginalFiles[i], arrayListInsertPosition, arrayListInsertStatement);
                }

                for (i = 0; i < arrayListOriginalFiles.Count; i++)
                {
                    File.Copy((string)arrayListOriginalFiles[i], dir + "\\Files with log statements inserted\\" + arrayListShortFileName[i]);
                }

                Console.Write("Insert statements were executed successfully!");
            }
        }

        private static void Reset(string fileNameFullPathMovedFiles, string directoryOriginalFiles, string directoryFilesWithLogStatementsInserted)
        {
            int i, n, index;
            string errorMessage, fileContents = File.ReadAllText(fileNameFullPathMovedFiles).Trim();

            string[] movedFiles = fileContents.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            if (!Utility.AllFilesExist(movedFiles, out errorMessage))
            {
                Console.Write(errorMessage);
                return;
            }

            n = movedFiles.Length;

            string[] fileNamesShort = new string[n];

            for (i = 0; i < n; i++)
            {
                fileNamesShort[i] = ReturnShortFileName(movedFiles[i]);
            }

            string[] originalFiles = new string[n];
            string[] FilesWithLogStatementsInserted = new string[n];

            for (i = 0; i < n; i++)
            {
                originalFiles[i] = directoryOriginalFiles + "\\" + fileNamesShort[i];
                FilesWithLogStatementsInserted[i] = directoryFilesWithLogStatementsInserted + "\\" + fileNamesShort[i];
            }

            if (!Utility.AllFilesExist(originalFiles, out errorMessage))
            {
                Console.Write(errorMessage);
                return;
            }

            if (!Utility.AllFilesExist(FilesWithLogStatementsInserted, out errorMessage))
            {
                Console.Write(errorMessage);
                return;
            }

            if (!Utility.ContentsInFilesAreSame(movedFiles, FilesWithLogStatementsInserted, out errorMessage))
            {
                Console.Write(errorMessage);
                return;
            }

            for (i = 0; i < n; i++)
            {
                File.Delete(movedFiles[i]);
                File.Delete(FilesWithLogStatementsInserted[i]);
                File.Move(originalFiles[i], movedFiles[i]);
            }

            Console.Write(string.Format("{0} original file(s) were successfully moved back", n));
            File.WriteAllText(fileNameFullPathMovedFiles, " ");
        }
    }
}
