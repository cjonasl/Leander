using System;
using System.Collections;
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

        public static void CreateNewFile(string fileNameFullPath, ArrayList v)
        {
            FileStream fileStream = new FileStream(fileNameFullPath, FileMode.Create, FileAccess.Write);
            StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
            
            for(int i = 0; i < v.Count; i++)
            {
                if (i == (v.Count - 1))
                    streamWriter.Write(v[i].ToString());
                else
                    streamWriter.WriteLine(v[i].ToString());
            }

            streamWriter.Flush();
            fileStream.Flush();
            streamWriter.Close();
            fileStream.Close();
        }

        public static void AppendToFile(string fileNameFullPath, string contentsToAppend)
        {
            FileStream fileStream = new FileStream(fileNameFullPath, FileMode.Append, FileAccess.Write);
            StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
            streamWriter.Write(contentsToAppend);
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

            Encoding encoding = GetEncoding(fileNameFullPath);

            FileStream fileStream = new FileStream(fileNameFullPath, FileMode.Open, FileAccess.Read);
            StreamReader streamReader = new StreamReader(fileStream, encoding);
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

        public static string[] ReturnRowsInFile(string fileNameFullPath, out string errorMessage)
        {
            string fileContens = ReturnFileContents(fileNameFullPath, out errorMessage);

            if (errorMessage == null)
                return fileContens.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            else
                return null;
        }

        public static bool AllFilesExist(string[] fileNamesFullPath, out string errorMessage)
        {
            bool allFilesExist = true; //Default
            int i = 0, n = fileNamesFullPath.Length;

            errorMessage = null;

            while ((allFilesExist == true) && (i < n))
            {
                if (!File.Exists(fileNamesFullPath[i]))
                {
                    errorMessage = string.Format("The following file does not exist: {0}", fileNamesFullPath[i]);
                    allFilesExist = false;
                }
                else
                    i++;
            }

            return allFilesExist;
        }

        public static void PutContentsInFilesInOneFile(string[] fileNamesFullPath, string fileNameFullPath, out string errorMessage)
        {
            int i = 0, n = fileNamesFullPath.Length;
            StringBuilder sb = new StringBuilder();
            FileInfo fi;

            if (!File.Exists(fileNameFullPath))
            {
                errorMessage = string.Format("The following file does not exist: {0}", fileNameFullPath);
                return;
            }
            else
                errorMessage = null;

            for (i = 0; i < n; i++)
            {
                fi = new FileInfo(fileNamesFullPath[i]);
                sb.Append("/*File: " + fi.Name + "*/\r\n" + ReturnFileContents(fileNamesFullPath[i]) + "\r\n\r\n");
            }

            CreateNewFile(fileNameFullPath, sb.ToString().TrimEnd());
        }

        public static bool FileSuffixIsInSuffixArray(string fileNameFullPath, ArrayList suffix)
        {
            bool suffixIsInSuffixArray = false;
            int i = 0, n = suffix.Count;
            string s;

            while ((!suffixIsInSuffixArray) && (i < n))
            {
                s = (string)suffix[i];

                if (fileNameFullPath.ToLower().EndsWith(s))
                    suffixIsInSuffixArray = true;
                else
                    i++;
            }

            return suffixIsInSuffixArray;
        }

        public static void GetFiles(string startDirectory, ArrayList fileNameFullPath, ArrayList suffix, bool includeSubFolders)
        {
            int i, n;
            string[] v = Directory.GetFiles(startDirectory);

            n = v.Length;

            for(i = 0; i < n; i++)
            {
                if ((FileSuffixIsInSuffixArray(v[i].Trim(), suffix)) && (fileNameFullPath.IndexOf(v[i].Trim()) == -1))
                    fileNameFullPath.Add(v[i].Trim());
            }

            if (includeSubFolders)
            {
                v = Directory.GetDirectories(startDirectory);

                for (i = 0; i < v.Length; i++)
                    GetFiles(v[i], fileNameFullPath, suffix, true);
            }
        }

        public static ArrayList ReturnRowsInFileInArrayList(string fileNameFullPath, out string errorMessage)
        {
            ArrayList arrayList = null;
            string[] rows = ReturnRowsInFile(fileNameFullPath, out errorMessage);

            if (errorMessage == null)
            {
                arrayList = new ArrayList();

                for(int i = 0; i < rows.Length; i++)
                {
                    arrayList.Add(rows[i]);
                }
            }

            return arrayList;
        }

        public static void AddFileInfo(string fileNameFullPath, ArrayList fileNamesShort, ArrayList directoryNames, ArrayList fileCreationDate, ArrayList fileUpdatedDate)
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(fileNameFullPath);

            string str1 = fi.Name;
            string str2 = fi.DirectoryName;
            string str3 = fi.CreationTime.ToString("yyyy-MM-dd HH:mm:ss");
            string str4 = fi.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");

            fileNamesShort.Add(str1);
            directoryNames.Add(str2);
            fileCreationDate.Add(str3);
            fileUpdatedDate.Add(str4);
        }

        public static long ReturnNumberOfBytesInFile(string fileNameFullPath, bool isTextFileUtf8)
        {
            long numberOfBytesInFile;

            FileInfo fi = new FileInfo(fileNameFullPath);

            numberOfBytesInFile = fi.Length;

            if (isTextFileUtf8)
                numberOfBytesInFile -= 3;  //Do not count the 3 initial bytes in the file to indicate that it is in format UTF-8

            return numberOfBytesInFile;
        }

        public static void DeleteAllFilesInDirectory(string directory)
        {
            string[] files;

            files = Directory.GetFiles(directory);

            foreach(string file in files)
            {
                File.Delete(file);
            }
        }

        public static bool InsertNoneIncludeFileInCsProj(
            int resourceId,  //Might maybe later be something else than a resource Id
            string filePrefix, //When it is a resource filePrefix = "R"
            string fileExtension, //When it is a resource fileExtension should be ".html"
            string fileNameFullPathCjProjFile, 
            string folderPrefix, 
            int currentResourceFolderIndex, 
            int maxNumberOfFilesInAFolder,
            string resourceFolder,
            string textForNonIncludeFileToCreate,
            out int nextResourceFolderIndex, //Will be (1 + currentResourceFolderIndex) if number of files in current folder is maxNumberOfFilesInAFolder, otherwise nextResourceFolderIndex = currentResourceFolderIndex
            out string errorMessage)
        {
            string folderNameFullPath, fileNameFullPath, tmpStr, str1, str2, str3, fileContents;
            int index1, index2;
            string[] files;

            nextResourceFolderIndex = currentResourceFolderIndex;
            errorMessage = null;

            try
            {
                if (!File.Exists(fileNameFullPathCjProjFile))
                {
                    errorMessage = string.Format("ERROR in method InsertNoneIncludeFileInCsProj! The file {0} does not exist as expected!", fileNameFullPathCjProjFile);
                    return false;
                }

                fileContents = Utility.ReturnFileContents(fileNameFullPathCjProjFile);
                tmpStr = "  <ItemGroup>\r\n    <None Include=\"";
                index1 = fileContents.IndexOf(tmpStr);

                if (index1 == -1)
                {
                    errorMessage = string.Format("ERROR in method InsertNoneIncludeFileInCsProj! Can not find string \"{0}\" in the file {1} as expected!", tmpStr, fileNameFullPathCjProjFile);
                    return false;
                }

                index2 = fileContents.IndexOf("\r\n", tmpStr.Length + index1);
                str1 = fileContents.Substring(0, 2 + index2);
                str3 = fileContents.Substring(2 + index2);

                folderNameFullPath = string.Format("{0}\\{1}{2}", resourceFolder, folderPrefix, currentResourceFolderIndex);

                if (!Directory.Exists(folderNameFullPath))
                {
                    errorMessage = string.Format("ERROR in method InsertNoneIncludeFileInCsProj! The folder {0} does not exist as expected!", folderNameFullPath);
                    return false;
                }

                files = Directory.GetFiles(folderNameFullPath);

                if (files.Length > maxNumberOfFilesInAFolder)
                {
                    errorMessage = string.Format("ERROR in method InsertNoneIncludeFileInCsProj! The folder {0} contains more files ({1}) files than allowed ({2}) files!", folderNameFullPath, files.Length.ToString(), maxNumberOfFilesInAFolder.ToString());
                    return false;
                }
                else if (files.Length == maxNumberOfFilesInAFolder)
                {
                    nextResourceFolderIndex = 1 + currentResourceFolderIndex;
                    folderNameFullPath = string.Format("{0}\\{1}{2}", resourceFolder, folderPrefix, nextResourceFolderIndex);
                }

                fileNameFullPath = string.Format("{0}\\{1}{2}{3}", folderNameFullPath, filePrefix, resourceId.ToString(), fileExtension);

                if (File.Exists(fileNameFullPath))
                {
                    errorMessage = string.Format("ERROR in method InsertNoneIncludeFileInCsProj! The file {0} was about to be created, but it exist already, which was unexpected!", fileNameFullPath);
                    return false;
                }

                if (!Directory.Exists(folderNameFullPath))
                {
                    Directory.CreateDirectory(folderNameFullPath);
                }

                Utility.CreateNewFile(fileNameFullPath, textForNonIncludeFileToCreate);
                str2 = string.Format("    <None Include=\"{0}{1}\\{2}{3}{4}\" />\r\n", folderPrefix, nextResourceFolderIndex, filePrefix, resourceId.ToString(), fileExtension);
                Utility.CreateNewFile(fileNameFullPathCjProjFile, str1 + str2 + str3);
            }
            catch (Exception e)
            {
                errorMessage = string.Format("ERROR!! An Exception occured in method InsertNoneIncludeFileInCsProj! e.Message:\r\n{0}", e.Message);
                return false;
            }

            return true;
        }

        public static bool FileNameFullPathContainsExcludePattern(string fileNameFullPath, ArrayList excludePattern)
        {
            bool suffixIsInSuffixArray = false;
            int i = 0, n = excludePattern.Count;
            string s;

            while ((!suffixIsInSuffixArray) && (i < n))
            {
                s = (string)excludePattern[i];

                if (fileNameFullPath.IndexOf(s) >= 0)
                    suffixIsInSuffixArray = true;
                else
                    i++;
            }

            return suffixIsInSuffixArray;
        }

        /// <summary>
        /// config is a row separated string with configuration
        /// 0FileNameFullPath: Include that file
        /// 1FileNameFullPath: Exclude that file
        /// 2DirectoryNameFullPath: Include all files in directory and sub directories
        /// 3DirectoryNameFullPath: Include all files in directory but not in sub directories
        /// 4FileNamePattern: Exclude file if fileNameFullPath.indexof(FileNamePattern) >= 0
        /// </summary>
        public static ArrayList ReturnFiles(string config, ArrayList suffix, out string errorMessage)
        {
            errorMessage = null;
            ArrayList returnArrayList = new ArrayList();
            
            try
            {
                string[] commands = config.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                char command;
                string fileNameFullPath;
                int i;
                ArrayList include = new ArrayList();
                ArrayList exclude = new ArrayList();
                ArrayList excludePattern = new ArrayList();

                for (i = 0; i < commands.Length; i++)
                {
                    command = commands[i][0];

                    switch (command)
                    {
                        case '0':
                            if (!File.Exists(commands[i].Substring(1).Trim()))
                            {
                                errorMessage = string.Format("Error in method ReturnFiles! The following file does not exist: {0}", commands[i].Substring(1).Trim());
                                return null;
                            }
                            else if (include.IndexOf(commands[i].Substring(1).Trim()) == -1)
                            {
                                include.Add(commands[i].Substring(1).Trim());
                            }
                        break;
                        case '1':
                            if (!File.Exists(commands[i].Substring(1).Trim()))
                            {
                                errorMessage = string.Format("Error in method ReturnFiles! The following file does not exist: {0}", commands[i].Substring(1).Trim());
                                return null;
                            }
                            else if (exclude.IndexOf(commands[i].Substring(1).Trim()) == -1)
                            {
                                exclude.Add(commands[i].Substring(1).Trim());
                            }
                            break;
                        case '2':
                            if (!Directory.Exists(commands[i].Substring(1).Trim()))
                            {
                                errorMessage = string.Format("Error in method ReturnFiles! The following directory does not exist: {0}", commands[i].Substring(1).Trim());
                                return null;
                            }
                            else
                                GetFiles(commands[i].Substring(1).Trim(), include, suffix, true);
                            break;
                        case '3':
                            if (!Directory.Exists(commands[i].Substring(1).Trim()))
                            {
                                errorMessage = string.Format("Error in method ReturnFiles! The following directory does not exist: {0}", commands[i].Substring(1).Trim());
                                return null;
                            }
                            else
                                GetFiles(commands[i].Substring(1).Trim(), include, suffix, false);
                            break;
                        case '4':
                            if (excludePattern.IndexOf(commands[i].Substring(1).Trim()) == -1)
                            {
                                excludePattern.Add(commands[i].Substring(1).Trim());
                            }
                            break;
                        default:
                            errorMessage = string.Format("Error in method ReturnFiles! Incorrect command: {0}", command.ToString());
                            return null;
                    }
                }

                for(i = 0; i < include.Count; i++)
                {
                    fileNameFullPath = (string)include[i];
                    if (exclude.IndexOf(fileNameFullPath) == -1 && !FileNameFullPathContainsExcludePattern(fileNameFullPath, excludePattern))
                    {
                        returnArrayList.Add(fileNameFullPath);
                    }
                }

                
            }
            catch(Exception e)
            {
                errorMessage = string.Format("Error in method ReturnFiles! e.Message = {0}", e.Message);
                return null;
            }

            return returnArrayList;
        }
    }
}
