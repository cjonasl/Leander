using System;
using System.IO;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using Leander.Nr1;

namespace InsertWriteStatements
{
    class Program
    {
        /// <summary>
        /// One parameter to the program: config-folder full name
        /// </summary>
        static void Main(string[] args)
        {
            string configFolderFullPath, fileNameFullPath, fileContents, fileConfig, folderOriginal, folderTmp, state, errorMessage;
            string[] stringArray;
            ArrayList suffix, files;
            int i, index1, index2;

            try
            {
                if (args.Length != 1)
                {
                    Console.WriteLine("There should be exactly one parameter to the program, the config-folder full name");
                    Console.ReadKey();
                    return;
                }

                configFolderFullPath = args[0].Trim();

                if (!Directory.Exists(configFolderFullPath))
                {
                    Console.WriteLine("The given config-folder does not exist!");
                    Console.ReadKey();
                    return;
                }

                stringArray = new string[] { "Config.txt", "MovedFiles.txt", "Log.txt", "State.txt" };

                for (i = 0; i < 4; i++)
                {
                    fileNameFullPath = configFolderFullPath + "\\" + stringArray[i];

                    if (!File.Exists(fileNameFullPath))
                    {
                        Console.WriteLine(string.Format("The following file does not exist as expected: {0}", fileNameFullPath));
                        Console.ReadKey();
                        return;
                    }
                }

                fileNameFullPath = configFolderFullPath + "\\Config.txt";
                fileContents = Utility.ReturnFileContents(fileNameFullPath);

                index1 = fileContents.IndexOf("Suffix: ");

                if (index1 == -1)
                {
                    Console.WriteLine(string.Format("Can not find \"Suffix: \" in the file {0}", fileNameFullPath));
                    Console.ReadKey();
                    return;
                }

                index2 = fileContents.IndexOf("\r\n", index1);

                if (index2 == -1)
                {
                    Console.WriteLine(string.Format("Can not find end of row for \"Suffix: \" in the file {0}", fileNameFullPath));
                    Console.ReadKey();
                    return;
                }

                stringArray = fileContents.Substring(index1 + 8, index2 - index1 - 8).Split(',');

                suffix = new ArrayList();

                for(i = 0; i < stringArray.Length; i++)
                {
                    suffix.Add(stringArray[i].Trim());
                }

                index1 = fileContents.IndexOf("Files\r\n");

                if (index1 == -1)
                {
                    Console.WriteLine(string.Format("Can not find \"FilesNewRow\" in the file {0}", fileNameFullPath));
                    Console.ReadKey();
                    return;
                }

                index2 = fileContents.IndexOf("End Files\r\n");

                if (index2 == -1)
                {
                    Console.WriteLine(string.Format("Can not find \"End FilesNewRow\" in the file {0}", fileNameFullPath));
                    Console.ReadKey();
                    return;
                }

                fileConfig = fileContents.Substring(7 + index1, index2 - index1 - 7);

                if (index1 == -1)
                {
                    Console.WriteLine(string.Format("Can not find \"FolderMap: \" in the file {0}", fileNameFullPath));
                    Console.ReadKey();
                    return;
                }

                stringArray = fileContents.Substring(11 + index1).Split(new string[] { " ##### " }, StringSplitOptions.None);

                if (stringArray.Length != 2)
                {
                    Console.WriteLine(string.Format("Configuration of \"FolderMap\" is incorrect in the file {0}", fileNameFullPath));
                    Console.ReadKey();
                    return;
                }

                folderOriginal = stringArray[0];
                folderTmp = stringArray[1];

                if (!Directory.Exists(folderOriginal))
                {
                    Console.WriteLine(string.Format("Folder original \"{0}\" does not exist!", folderOriginal));
                    Console.ReadKey();
                    return;
                }

                if (!Directory.Exists(folderTmp))
                {
                    Console.WriteLine(string.Format("Folder tmp \"{0}\" does not exist!", folderTmp));
                    Console.ReadKey();
                    return;
                }

                fileNameFullPath = configFolderFullPath + "\\State.txt";
                state = Utility.ReturnFileContents(fileNameFullPath);

                if (state != "FilesNotMoved" && state != "FilesMoved")
                {
                    Console.WriteLine(string.Format("Incorrect value, \"{0}\", of state in the file {1}! It should be \"FilesNotMoved\" or \"FilesMoved\"", state, fileNameFullPath));
                    Console.ReadKey();
                    return;
                }

                if (state == "FilesNotMoved")
                {
                    fileNameFullPath = configFolderFullPath + "\\MovedFiles.txt";

                    if (!string.IsNullOrEmpty(Utility.ReturnFileContents(fileNameFullPath)))
                    {
                        Console.WriteLine(string.Format("The content in the file {0} is not empty as expected!", fileNameFullPath));
                        Console.ReadKey();
                        return;
                    }

                    files = Utility.ReturnFiles(fileConfig, suffix, out errorMessage);

                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        Console.WriteLine(errorMessage);
                        Console.ReadKey();
                        return;
                    }

                    files.Sort();

                    CheckMoveFilesToTempFolders(files, folderOriginal, folderTmp, out errorMessage);

                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        Console.WriteLine(errorMessage);
                        Console.ReadKey();
                        return;
                    }

                    MoveFilesToTempFolders(files, folderOriginal, folderTmp, out errorMessage);

                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        Console.WriteLine(errorMessage + " OBS OBS! May have moved some files! You need to check!");
                        Console.ReadKey();
                        return;
                    }

                    fileNameFullPath = configFolderFullPath + "\\MovedFiles.txt";
                    Utility.CreateNewFile(fileNameFullPath, files);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("An exception happened! e.Message = {0}", e.Message));
                Console.ReadKey();
                return;
            }

        }

        private static bool FileHasCarlJonasLeanderInIt(string fileNameFullPath)
        {
            string fileContents, errorMessage;

            fileContents = Utility.ReturnFileContents(fileNameFullPath, out errorMessage);

            if (fileContents.IndexOf("CarlJonasLeander") >= 0)
                return true;
            else
                return false;
        }

        private static void CheckMoveFilesToTempFolders(ArrayList files, string folderOriginal, string folderTmp, out string errorMessage)
        {
            int i, numberOfFiles;
            string fileNameFullPathFrom, fileNameFullPathTo;
            bool fileHasCarlJonasLeanderInIt;


            errorMessage = null;

            try
            {
                numberOfFiles = files.Count;

                for (i = 0; i < numberOfFiles; i++)
                {
                    fileNameFullPathFrom = (string)files[i];
                    fileNameFullPathTo = fileNameFullPathFrom.Replace(folderOriginal, folderTmp);
                    fileHasCarlJonasLeanderInIt = FileHasCarlJonasLeanderInIt(fileNameFullPathFrom);

                    if (fileHasCarlJonasLeanderInIt)
                    {
                        errorMessage = string.Format("The following file had unexpectedly \"CarlJonasLeander\" it it: {0}", fileNameFullPathFrom);
                        return;
                    }
                    else if (File.Exists(fileNameFullPathTo))
                    {
                        errorMessage = string.Format("The following file was not expected to exist: {0}", fileNameFullPathTo);
                        return;
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(string.Format("An exception happened in the method CheckMoveFilesToTempFolders! e.Message = {0}", e.Message));
            }
        }

        private static void MoveFilesToTempFolders(ArrayList files, string folderOriginal, string folderTmp, out string errorMessage)
        {
            int i, numberOfFiles;
            string fileNameFullPathFrom, fileNameFullPathTo;

            errorMessage = null;

            try
            {
                numberOfFiles = files.Count;

                for (i = 0; i < numberOfFiles; i++)
                {
                    fileNameFullPathFrom = (string)files[i];
                    fileNameFullPathTo = fileNameFullPathFrom.Replace(folderOriginal, folderTmp);
                    File.Move(fileNameFullPathFrom, fileNameFullPathTo);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("An exception happened in the method MoveFilesToTempFolders! e.Message = {0}", e.Message));
            }
        }
    }
}
