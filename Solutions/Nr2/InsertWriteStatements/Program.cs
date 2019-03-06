using System;
using System.IO;
using System.Collections;
using System.Text;
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
            string configFolderFullPath, fileNameFullPath, fileContents, fileConfig, folderOriginal, folderTmp, state, errorsDirectory, s, errorMessage;
            string[] stringArray;
            ArrayList suffix, files;
            int i, index1, index2, numberOfErrors = 0, n = 0;
            bool success = false;
            int[] v = new int[] { 0, 0, 0, 0, 0}; //aspx, ascx, js, cshtml, cs
            AspxFile aspxFile;
            AscxFile ascxFile;
            JsFile jsFile;
            CshtmlFile cshtmlFile;
            CsFile csFile;

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

                index2 = fileContents.IndexOf("\r\nEnd Files\r\n");

                if (index2 == -1)
                {
                    Console.WriteLine(string.Format("Can not find \"End FilesNewRow\" in the file {0}", fileNameFullPath));
                    Console.ReadKey();
                    return;
                }

                fileConfig = fileContents.Substring(7 + index1, index2 - index1 - 7);

                CheckSuffix(suffix, fileConfig, out errorMessage);

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    Console.WriteLine(errorMessage);
                    Console.ReadKey();
                    return;
                }

                index1 = fileContents.IndexOf("FolderMap: ");

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

                    if (files.Count == 0)
                    {
                        Console.WriteLine("There is no file to move!");
                        Console.ReadKey();
                        return;
                    }

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

                    CheckCopyFilesToOriginalFolders(files, folderOriginal, folderTmp, out errorMessage);

                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        Console.WriteLine(errorMessage + " OBS OBS! Original files moved to tmp location! Check the file " + fileNameFullPath + ".");
                        Console.ReadKey();
                        return;
                    }

                    CopyFilesToOriginalFolders(files, folderOriginal, folderTmp, out errorMessage);

                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        Console.WriteLine(errorMessage + " OBS OBS! Original files moved to tmp location! Check the file " + fileNameFullPath + ". Some original files may also have been copied back to original location.");
                        Console.ReadKey();
                        return;
                    }

                    aspxFile = new AspxFile(configFolderFullPath);
                    ascxFile = new AscxFile(configFolderFullPath);
                    jsFile = new JsFile(configFolderFullPath);
                    cshtmlFile = new CshtmlFile(configFolderFullPath);
                    csFile = new CsFile(configFolderFullPath);

                    errorsDirectory = string.Format("{0}\\Errors", configFolderFullPath);

                    if (Directory.Exists(errorsDirectory))
                        Utility.DeleteAllFilesInDirectory(errorsDirectory);
                    else
                        Directory.CreateDirectory(errorsDirectory);

                    for(i = 0; i < files.Count; i++)
                    {
                        s = ((string)files[i]).Substring(1 + ((string)files[i]).LastIndexOf('.'));

                        switch(s)
                        {
                            case "aspx":
                                success = aspxFile.Handle((string)files[i]);
                                v[0]++;
                                break;
                            case "ascx":
                                success = ascxFile.Handle((string)files[i]);
                                v[1]++;
                                break;
                            case "js":
                                success = jsFile.Handle((string)files[i]);
                                v[2]++;
                                break;
                            case "cshtml":
                                success = cshtmlFile.Handle((string)files[i]);
                                v[3]++;
                                break;
                            case "cs":
                                success = csFile.Handle((string)files[i]);
                                v[4]++;
                                break;
                            default:
                                throw new Exception("Incorrect suffix " + s + "!!");
                        }

                        if (!success)
                            numberOfErrors++;

                        n++;
                        Console.Write(string.Format("\rHandle file {0} of {1}", n.ToString(), files.Count.ToString()));
                    }

                    Console.WriteLine();
                    Console.WriteLine(string.Format("{0} files handled ({1} success and {2} error):", files.Count, files.Count - numberOfErrors, numberOfErrors));

                    if (v[0] > 0)
                        Console.WriteLine(string.Format("aspx: {0}", v[0]));

                    if (v[1] > 0)
                        Console.WriteLine(string.Format("ascx: {0}", v[1]));

                    if (v[2] > 0)
                        Console.WriteLine(string.Format("js: {0}", v[2]));

                    if (v[3] > 0)
                        Console.WriteLine(string.Format("cshtml: {0}", v[3]));

                    if (v[4] > 0)
                        Console.WriteLine(string.Format("cs: {0}", v[4]));

                    if (numberOfErrors > 0)
                        Console.WriteLine("Check files in folder {0} for error messages", errorsDirectory);
                    else
                        Directory.Delete(errorsDirectory);

                    fileNameFullPath = configFolderFullPath + "\\State.txt";
                    Utility.CreateNewFile(fileNameFullPath, "FilesMoved");
                    Console.ReadKey();
                }
                else
                {
                    fileNameFullPath = configFolderFullPath + "\\MovedFiles.txt";
                    fileContents = Utility.ReturnFileContents(fileNameFullPath);
                    stringArray = fileContents.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                    CheckDeleteFiles(stringArray, out errorMessage);

                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        Console.WriteLine(errorMessage);
                        Console.ReadKey();
                        return;
                    }

                    DeleteFiles(stringArray, out errorMessage);

                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        Console.WriteLine(errorMessage + " OBS OBS! Some tmp files may have been deleted.");
                        Console.ReadKey();
                        return;
                    }

                    CheckMoveFilesToOriginalFolder(stringArray, folderOriginal, folderTmp, out errorMessage);

                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        Console.WriteLine(errorMessage + " OBS OBS! Tmp files have already been deleted.");
                        Console.ReadKey();
                        return;
                    }

                    MoveFilesToOriginalFolder(stringArray, folderOriginal, folderTmp, out errorMessage);

                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        Console.WriteLine(errorMessage + " OBS OBS! Tmp files have already been deleted. Some original files may have been successfully copied back.");
                        Console.ReadKey();
                        return;
                    }

                    Utility.CreateNewFile(fileNameFullPath, ""); //Clear contents in MovedFiles.txt

                    fileNameFullPath = configFolderFullPath + "\\State.txt";
                    Utility.CreateNewFile(fileNameFullPath, "FilesNotMoved");

                    Console.WriteLine(string.Format("{0} original files were successfully copied back.", stringArray.Length.ToString()));
                    Console.ReadKey();
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
            string fileContents;
            Encoding encoding;

            fileContents = Utility.ReturnFileContents(fileNameFullPath, out encoding);

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
                        errorMessage = string.Format("Error!! The following file had unexpectedly \"CarlJonasLeander\" it it: {0}", fileNameFullPathFrom);
                        return;
                    }
                    else if (File.Exists(fileNameFullPathTo))
                    {
                        errorMessage = string.Format("Error!! The following file was not expected to exist: {0}", fileNameFullPathTo);
                        return;
                    }
                }
            }
            catch(Exception e)
            {
                errorMessage = string.Format("An exception happened in the method CheckMoveFilesToTempFolders! e.Message = {0}", e.Message);
            }
        }

        private static void MoveFilesToTempFolders(ArrayList files, string folderOriginal, string folderTmp, out string errorMessage)
        {
            int i, numberOfFiles;
            string fileNameFullPathFrom, fileNameFullPathTo, dir;

            errorMessage = null;

            try
            {
                numberOfFiles = files.Count;

                for (i = 0; i < numberOfFiles; i++)
                {
                    fileNameFullPathFrom = (string)files[i];
                    fileNameFullPathTo = fileNameFullPathFrom.Replace(folderOriginal, folderTmp);

                    dir = (new FileInfo(fileNameFullPathTo)).DirectoryName;

                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    File.Move(fileNameFullPathFrom, fileNameFullPathTo);
                }
            }
            catch (Exception e)
            {
                errorMessage = string.Format("An exception happened in the method MoveFilesToTempFolders! e.Message = {0}", e.Message);
            }
        }

        private static void CheckCopyFilesToOriginalFolders(ArrayList files, string folderOriginal, string folderTmp, out string errorMessage)
        {
            int i, numberOfFiles;
            string fileNameFullPathFrom, fileNameFullPathTo;

            errorMessage = null;

            try
            {
                numberOfFiles = files.Count;

                for (i = 0; i < numberOfFiles; i++)
                {
                    fileNameFullPathTo = (string)files[i];
                    fileNameFullPathFrom = fileNameFullPathTo.Replace(folderOriginal, folderTmp);

                    if (!File.Exists(fileNameFullPathFrom))
                    {
                        errorMessage = string.Format("Error!! When copy files to original folder, the following file did not exist as expected: {0}", fileNameFullPathFrom);
                        return;
                    }
                    else if (File.Exists(fileNameFullPathTo))
                    {
                        errorMessage = string.Format("Error!! When copy files to original folder, the following file did unexpectedly already exist: {0}", fileNameFullPathTo);
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                errorMessage = string.Format("An exception happened in the method CheckCopyFilesToOriginalFolders! e.Message = {0}", e.Message);
            }
        }

        private static void CopyFilesToOriginalFolders(ArrayList files, string folderOriginal, string folderTmp, out string errorMessage)
        {
            int i, numberOfFiles;
            string fileNameFullPathFrom, fileNameFullPathTo;

            errorMessage = null;

            try
            {
                numberOfFiles = files.Count;

                for (i = 0; i < numberOfFiles; i++)
                {
                    fileNameFullPathTo = (string)files[i];
                    fileNameFullPathFrom = fileNameFullPathTo.Replace(folderOriginal, folderTmp);
                    File.Copy(fileNameFullPathFrom, fileNameFullPathTo);
                }
            }
            catch (Exception e)
            {
                errorMessage = string.Format("An exception happened in the method CopyFilesToOriginalFolders! e.Message = {0}", e.Message);
            }
        }

        private static void CheckSuffix(ArrayList suffix, string fileConfig, out string errorMessage)
        {
            int i, n = 0;
            string s;
            string[] rows;

            errorMessage = null;

            for(i = 0; i < suffix.Count; i++)
            {
                s = (string)suffix[i];

                if ((s != "aspx") && (s != "ascx") && (s != "cs") && (s != "cshtml") && (s != "js"))
                {
                    errorMessage = string.Format("Error!! An invalid suffix, \"{0}\", is given! Must be aspx, ascx, cs, cshtml or js.", s);
                    return;
                }
            }

            rows = fileConfig.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            for(i = 0; i < rows.Length; i++)
            {
                n++;

                if (rows[i][0] == '0' || rows[i][0] == '1')
                {
                    if (!rows[i].EndsWith(".aspx") && !rows[i].EndsWith(".ascx") && !rows[i].EndsWith(".cs") && !rows[i].EndsWith(".cshtml") && !rows[i].EndsWith(".js"))
                    {
                        errorMessage = string.Format("Error!! An invalid suffix is found on row {0} in Config.txt. The suffix for 0 (include file) and 1 (exclude file) must be aspx, ascx, cs, cshtml or js.", n.ToString());
                        return;
                    }
                }
            }
        }

        private static void CheckDeleteFiles(string[] files, out string errorMessage)
        {
            int i, numberOfFiles;
            string fileNameFullPath = "";
            bool fileHasCarlJonasLeanderInIt;

            errorMessage = null;

            numberOfFiles = files.Length;

            for (i = 0; i < numberOfFiles; i++)
            {
                try
                {
                    fileNameFullPath = files[i];
                    fileHasCarlJonasLeanderInIt = FileHasCarlJonasLeanderInIt(fileNameFullPath);

                    if (!fileHasCarlJonasLeanderInIt)
                    {
                        errorMessage = string.Format("Error!! The following file does not have \"CarlJonasLeander\" it it: {0}", fileNameFullPath);
                        return;
                    }
                    else if (!File.Exists(fileNameFullPath))
                    {
                        errorMessage = string.Format("Error!! The following file does not exist: {0}", fileNameFullPath);
                        return;
                    }

                }
                catch (Exception e)
                {
                    errorMessage = string.Format("An exception happened in the method CheckDeleteFiles! e.Message = {0}", e.Message);
                }
            }
        }

        private static void DeleteFiles(string[] files, out string errorMessage)
        {
            int i, numberOfFiles;
            string fileNameFullPath;

            errorMessage = null;

            numberOfFiles = files.Length;

            for (i = 0; i < numberOfFiles; i++)
            {
                try
                {
                    fileNameFullPath = (string)files[i];
                    File.Delete(fileNameFullPath);
                }
                catch (Exception e)
                {
                    errorMessage = string.Format("An exception happened in the method DeleteFiles! e.Message = {0}", e.Message);
                }
            }
        }

        private static void CheckMoveFilesToOriginalFolder(string[] files, string folderOriginal, string folderTmp, out string errorMessage)
        {
            int i, numberOfFiles;
            string fileNameFullPathFrom, fileNameFullPathTo;
            bool fileHasCarlJonasLeanderInIt;

            errorMessage = null;

            numberOfFiles = files.Length;

            for (i = 0; i < numberOfFiles; i++)
            {
                try
                {
                    fileNameFullPathTo = files[i];
                    fileNameFullPathFrom = fileNameFullPathTo.Replace(folderOriginal, folderTmp);
                    fileHasCarlJonasLeanderInIt = FileHasCarlJonasLeanderInIt(fileNameFullPathFrom);

                    if (!File.Exists(fileNameFullPathFrom))
                    {
                        errorMessage = string.Format("Error!! The following file does not exist as expected: {0}", fileNameFullPathFrom);
                        return;
                    }
                    else if (File.Exists(fileNameFullPathTo))
                    {
                        errorMessage = string.Format("Error!! The following file exists already: {0}", fileNameFullPathTo);
                        return;
                    }
                    else if (fileHasCarlJonasLeanderInIt)
                    {
                        errorMessage = string.Format("Error!! The following file has CarlJonasLeander in it: {0}", fileNameFullPathFrom);
                        return;
                    }
                }
                catch (Exception e)
                {
                    errorMessage = string.Format("An exception happened in the method CheckMoveFilesToOriginalFolder! e.Message = {0}", e.Message);
                }
            }
        }

        private static void MoveFilesToOriginalFolder(string[] files, string folderOriginal, string folderTmp, out string errorMessage)
        {
            int i, numberOfFiles;
            string fileNameFullPathFrom, fileNameFullPathTo;

            errorMessage = null;
            numberOfFiles = files.Length;

            for (i = 0; i < numberOfFiles; i++)
            {
                try
                {
                    fileNameFullPathTo = files[i];
                    fileNameFullPathFrom = fileNameFullPathTo.Replace(folderOriginal, folderTmp);
                    File.Move(fileNameFullPathFrom, fileNameFullPathTo);
                }
                catch (Exception e)
                {
                    errorMessage = string.Format("An exception happened in the method MoveFilesToOriginalFolder! e.Message = {0}", e.Message);
                }
            }
        }
    }
}
