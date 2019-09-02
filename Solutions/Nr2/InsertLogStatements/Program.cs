using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Leander.Nr1;

namespace InsertLogStatements
{
    class Program
    {
        static void Main(string[] args)
        {
            string errorMessage, fileContents;
            int i, n, index;

            try
            {
                if (args.Length != 1)
                {
                    Console.Write("ERROR!! The program should have exactly one command argument!");
                    return;
                }

                string[] files = new string[2]
                {
                    args[0] + "\\FilesMoved.txt",
                    args[0] + "\\InsertLogStatements.txt"
                };

                if (!Utility.AllFilesExist(files, out errorMessage))
                {
                    Console.Write(errorMessage);
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

                fileContents = File.ReadAllText(files[0]).Trim();

                if (string.IsNullOrEmpty(fileContents))
                {

                }
                else
                {
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
                        index = 1 + fileNamesShort[i].LastIndexOf("\\");
                        fileNamesShort[i] = fileNamesShort[i].Substring(index);
                    }

                    string[] originalFiles = new string[n];
                    string[] FilesWithLogStatementsInserted = new string[n];

                    for (i = 0; i < n; i++)
                    {
                        originalFiles[i] = directories[0] + "\\" + fileNamesShort[i];
                        FilesWithLogStatementsInserted[i] = directories[1] + "\\" + fileNamesShort[i];
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
                    File.WriteAllText(files[0], " ");
                }
            }
            catch(Exception e)
            {
                Console.Write(string.Format("ERROR!! An exception happened! e.Message = {0}", e.Message));
            }
        }
    }
}
