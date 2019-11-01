using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;

namespace HandleTwoFolders
{
    public enum CompareResult
    {
        SameContents,
        DifferentContents,
        FileExistInTargetFolderButNotInCompare,
        FileExistInCompareFolderButNotInTarget
    }


    class Program
    {
        static void Main(string[] args)
        {
            string fileContents, currentDir, fileNameFullPath, targetFolder, compareFolder;
            string[] v, tmp;
            ArrayList fileTypesToSearch;

            if (args.Length != 1 || (args[0] != "0" && args[0] != "1"))
            {
                Console.WriteLine("There must be exactly one parameter to the program, 0 (analyze and print files) or 1 (update target folder)");
                return;
            }

            currentDir = Directory.GetCurrentDirectory();

            fileNameFullPath = currentDir + "\\config.txt";

            if (!File.Exists(fileNameFullPath))
            {
                Console.WriteLine("Error! The following file does not exist as expected: " + fileNameFullPath);
                return;
            }

            fileContents = File.ReadAllText(fileNameFullPath);

            v = fileContents.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            if (v.Length < 4)
            {
                Console.WriteLine("Error! At least 4 rows expected in file: " + fileNameFullPath);
                return;
            }

            targetFolder = v[0];
            compareFolder = v[1];
            tmp = v[2].Split(',');

            fileTypesToSearch = new ArrayList();

            for (int i = 0; i < tmp.Length; i++)
            {
                fileTypesToSearch.Add(tmp[i].Trim().ToLower());
            }

            if (!Directory.Exists(targetFolder))
            {
                Console.WriteLine(string.Format("Error! Target folder \"{0}\", first row in file \"{1}\", does not exist!", targetFolder, fileNameFullPath));
                return;
            }

            if (!Directory.Exists(compareFolder))
            {
                Console.WriteLine(string.Format("Error! Compare folder \"{0}\", second row in file \"{1}\", does not exist!", compareFolder, fileNameFullPath));
                return;
            }

            if (args[0] == "0")
                Analyse(targetFolder, compareFolder, currentDir, fileTypesToSearch, v);
            else
                UpdateTargetFolder(targetFolder, compareFolder, currentDir);
        }


        static void Analyse(string targetFolder, string compareFolder, string currentDir, ArrayList fileTypesToSearch, string[] v)
        {
            string str;
            ArrayList searchFolders, fileNamesFullPathTargetFolder, fileNamesFullPathCompareFolder;
            StringBuilder sameContents, differentContents, fileExistInTargetFolderButNotInCompare, fileExistInCompareFolderButNotInTarget;
            CompareResult compareResult;
            int i, numberOfFilesSameContents1 = 0, numberOfFilesNotSameContents1 = 0, numberOfFilesSameContents2 = 0, numberOfFilesNotSameContents2 = 0;
            int numberOfFilesExistInTargetButNotInCompare = 0, numberOfFilesExistInCompareButNotInTarget = 0;
            bool errorFound;

            searchFolders = new ArrayList();

            errorFound = false;
            i = 3;

            while (!errorFound && i < v.Length)
            {
                if (!Directory.Exists(v[i]))
                {
                    errorFound = true;
                }
                else
                {
                    searchFolders.Add(v[i]);
                    i++;
                }
            }

            if (errorFound)
            {
                Console.WriteLine("Errior! The following search folder does not exist: " + v[i]);
                return;
            }

            fileNamesFullPathTargetFolder = new ArrayList();

            for (i = 0; i < searchFolders.Count; i++)
            {
                GetFiles((string)searchFolders[i], fileTypesToSearch, fileNamesFullPathTargetFolder, targetFolder);
            }

            fileNamesFullPathCompareFolder = new ArrayList();

            for (i = 0; i < searchFolders.Count; i++)
            {
                str = (string)searchFolders[i];
                GetFiles(str.Replace(targetFolder, compareFolder), fileTypesToSearch, fileNamesFullPathCompareFolder, compareFolder);
            }

            sameContents = new StringBuilder();
            differentContents = new StringBuilder();
            fileExistInTargetFolderButNotInCompare = new StringBuilder();
            fileExistInCompareFolderButNotInTarget = new StringBuilder();

            for(i = 0; i < fileNamesFullPathTargetFolder.Count; i++)
            {
                compareResult = Check((string)fileNamesFullPathTargetFolder[i], fileNamesFullPathCompareFolder, targetFolder, compareFolder, true);

                if (compareResult == CompareResult.SameContents)
                {
                    sameContents.Append((string)fileNamesFullPathTargetFolder[i] + "\r\n");
                    numberOfFilesSameContents1++;
                }
                else if (compareResult == CompareResult.DifferentContents)
                {
                    differentContents.Append((string)fileNamesFullPathTargetFolder[i] + "\r\n");
                    numberOfFilesNotSameContents1++;
                }
                else if (compareResult == CompareResult.FileExistInTargetFolderButNotInCompare)
                {
                    fileExistInTargetFolderButNotInCompare.Append((string)fileNamesFullPathTargetFolder[i] + "\r\n");
                    numberOfFilesExistInTargetButNotInCompare++;
                }
                else
                {
                    throw new Exception("Unexpected compareResult!!");
                }
            }

            for (i = 0; i < fileNamesFullPathCompareFolder.Count; i++)
            {
                compareResult = Check((string)fileNamesFullPathCompareFolder[i], fileNamesFullPathTargetFolder, targetFolder, compareFolder, false);

                if (compareResult == CompareResult.SameContents)
                {
                    numberOfFilesSameContents2++;
                }
                else if (compareResult == CompareResult.DifferentContents)
                {
                    numberOfFilesNotSameContents2++;
                }
                else if (compareResult == CompareResult.FileExistInCompareFolderButNotInTarget)
                {
                    fileExistInCompareFolderButNotInTarget.Append((string)fileNamesFullPathCompareFolder[i] + "\r\n");
                    numberOfFilesExistInCompareButNotInTarget++;
                }
                else
                {
                    throw new Exception("Unexpected compareResult!!");
                }
            }

            if (numberOfFilesSameContents1 != numberOfFilesSameContents2)
            {
                throw new Exception("Error!! numberOfFilesSameContents1 != numberOfFilesSameContents2");
            }

            if (numberOfFilesNotSameContents1 != numberOfFilesNotSameContents2)
            {
                throw new Exception("numberOfFilesNotSameContents1 != numberOfFilesNotSameContents2");
            }

            File.WriteAllText(currentDir + "\\SameContents.txt", sameContents.ToString().TrimEnd());
            File.WriteAllText(currentDir + "\\DifferentContents.txt", differentContents.ToString().TrimEnd());
            File.WriteAllText(currentDir + "\\FileExistInTargetFolderButNotInCompare.txt", fileExistInTargetFolderButNotInCompare.ToString().TrimEnd());
            File.WriteAllText(currentDir + "\\FileExistInCompareFolderButNotInTarget.txt", fileExistInCompareFolderButNotInTarget.ToString().TrimEnd());

            Console.WriteLine(string.Format("SameContents: {0} files", numberOfFilesSameContents1.ToString()));
            Console.WriteLine(string.Format("DifferentContents: {0} files", numberOfFilesNotSameContents1.ToString()));
            Console.WriteLine(string.Format("FileExistInTargetFolderButNotInCompare: {0} files", numberOfFilesExistInTargetButNotInCompare.ToString()));
            Console.WriteLine(string.Format("FileExistInCompareFolderButNotInTarget: {0} files", numberOfFilesExistInCompareButNotInTarget.ToString()));
        }

        private static void GetFiles(string targetFolder, ArrayList fileTypesToSearch, ArrayList fileNamesFullPath, string prefixPath)
        {
            string[] v;
            int i, n, index;
            string fileExtension;

            v = Directory.GetFiles(targetFolder);
            n = v.Length;

            for(i = 0; i < n; i++)
            {
                index = v[i].LastIndexOf('.');

                if (index >= 0 && v[i].Length > (index + 1))
                {
                    fileExtension = v[i].Substring(1 + index).Trim().ToLower();

                    if (fileTypesToSearch.IndexOf(fileExtension) >= 0)
                    {
                        fileNamesFullPath.Add(v[i].Replace(prefixPath, "").Trim().ToLower());
                    }
                }
            }

            v = Directory.GetDirectories(targetFolder);
            n = v.Length;

            for (i = 0; i < n; i++)
            {
                GetFiles(v[i], fileTypesToSearch, fileNamesFullPath, prefixPath);
            }
        }

        private static CompareResult Check(string fileNameFullpath, ArrayList fileNamesFullPath, string targetFolder, string compareFolder, bool fileNameFullpathIsFromTarget)
        {
            CompareResult compareResult;
            int index = fileNamesFullPath.IndexOf(fileNameFullpath);
            bool fileContensAreTheSame;

            if (index == -1)
            {
                compareResult = fileNameFullpathIsFromTarget ? CompareResult.FileExistInTargetFolderButNotInCompare : CompareResult.FileExistInCompareFolderButNotInTarget;
            }
            else
            {
                string fileNameFullPath1 = (fileNameFullpathIsFromTarget ? compareFolder : targetFolder) + (string)fileNamesFullPath[index];
                string fileNameFullPath2 = (fileNameFullpathIsFromTarget ? targetFolder : compareFolder) + fileNameFullpath;
                fileContensAreTheSame = FileContensAreTheSame(fileNameFullPath1, fileNameFullPath2);

                if (fileContensAreTheSame)
                    compareResult = CompareResult.SameContents;
                else
                    compareResult = CompareResult.DifferentContents;
            }

            return compareResult;
        }

        private static bool FileContensAreTheSame(string fileNameFullPath1, string fileNameFullPath2)
        {
            bool returnValue = true; //default

            byte[] v1 = File.ReadAllBytes(fileNameFullPath1);
            byte[] v2 = File.ReadAllBytes(fileNameFullPath2);

            if (v1.Length != v2.Length)
                returnValue = false;
            else
            {
                int i, n = v1.Length;
                i = 0;

                while (returnValue && i < n)
                {
                    if (v1[i] != v2[i])
                    {
                        returnValue = false;
                    }
                    else
                    {
                        i++;
                    }
                }
            }

            return returnValue;
        }

        static void UpdateTargetFolder(string targetFolder, string compareFolder, string currentDir)
        {
            string fileNameFullPath, fileContents, folderNameFullPath, shortFileName, suffix, fileNameWithoutSuffix;
            string[] differentContent = null, fileExistInCompareFolderButNotInTarget = null, fileExistInTargetFolderButNotInCompare = null;
            bool errorFound;
            FileInfo fi;
            int i, n, index;

            fileNameFullPath = currentDir + "\\DifferentContents.txt";

            if (!File.Exists(fileNameFullPath))
            {
                Console.WriteLine("Error! The following file does not exist as expected: " + fileNameFullPath);
                return;
            }

            fileNameFullPath = currentDir + "\\FileExistInCompareFolderButNotInTarget.txt";

            if (!File.Exists(fileNameFullPath))
            {
                Console.WriteLine("Error! The following file does not exist as expected: " + fileNameFullPath);
                return;
            }

            fileNameFullPath = currentDir + "\\FileExistInTargetFolderButNotInCompare.txt";

            if (!File.Exists(fileNameFullPath))
            {
                Console.WriteLine("Error! The following file does not exist as expected: " + fileNameFullPath);
                return;
            }

            fileNameFullPath = currentDir + "\\DifferentContents.txt";
            fileContents = File.ReadAllText(fileNameFullPath).Trim();

            if (!string.IsNullOrEmpty(fileContents))
            {
                differentContent = fileContents.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                errorFound = false;
                n = differentContent.Length;
                i = 0;

                while (!errorFound && i < n)
                {
                    fileNameFullPath = targetFolder + differentContent[i];

                    if (!File.Exists(fileNameFullPath))
                    {
                        errorFound = true;
                    }
                    else
                    {
                        i++;
                    }
                }

                if (errorFound)
                {
                    Console.WriteLine("Error! The following file does not exist as expected: " + fileNameFullPath);
                    return;
                }
                else
                {
                    i = 0;

                    while (!errorFound && i < n)
                    {
                        fileNameFullPath = compareFolder + differentContent[i];

                        if (!File.Exists(fileNameFullPath))
                        {
                            errorFound = true;
                        }
                        else
                        {
                            i++;
                        }
                    }

                    if (errorFound)
                    {
                        Console.WriteLine("Error! The following file does not exist as expected: " + fileNameFullPath);
                        return;
                    }
                }
            }


            fileNameFullPath = currentDir + "\\FileExistInCompareFolderButNotInTarget.txt";
            fileContents = File.ReadAllText(fileNameFullPath).Trim();

            if (!string.IsNullOrEmpty(fileContents))
            {
                fileExistInCompareFolderButNotInTarget = fileContents.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                errorFound = false;
                n = fileExistInCompareFolderButNotInTarget.Length;
                i = 0;

                while (!errorFound && i < n)
                {
                    fileNameFullPath = targetFolder + fileExistInCompareFolderButNotInTarget[i];

                    if (File.Exists(fileNameFullPath))
                    {
                        errorFound = true;
                    }
                    else
                    {
                        i++;
                    }
                }

                if (errorFound)
                {
                    Console.WriteLine("Error! The following file exists, which it shouldn't: " + fileNameFullPath);
                    return;
                }
                else
                {
                    i = 0;

                    while (!errorFound && i < n)
                    {
                        fileNameFullPath = compareFolder + fileExistInCompareFolderButNotInTarget[i];

                        if (!File.Exists(fileNameFullPath))
                        {
                            errorFound = true;
                        }
                        else
                        {
                            i++;
                        }
                    }

                    if (errorFound)
                    {
                        Console.WriteLine("Error! The following file does not exist as expected: " + fileNameFullPath);
                        return;
                    }
                }
            }


            fileNameFullPath = currentDir + "\\FileExistInTargetFolderButNotInCompare.txt";
            fileContents = File.ReadAllText(fileNameFullPath).Trim();

            if (!string.IsNullOrEmpty(fileContents))
            {
                fileExistInTargetFolderButNotInCompare = fileContents.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                errorFound = false;
                n = fileExistInTargetFolderButNotInCompare.Length;
                i = 0;

                while (!errorFound && i < n)
                {
                    fileNameFullPath = targetFolder + fileExistInTargetFolderButNotInCompare[i];

                    if (!File.Exists(fileNameFullPath))
                    {
                        errorFound = true;
                    }
                    else
                    {
                        i++;
                    }
                }

                if (errorFound)
                {
                    Console.WriteLine("Error! The following file does not exist as expected: " + fileNameFullPath);
                    return;
                }
                else
                {
                    i = 0;

                    while (!errorFound && i < n)
                    {
                        fileNameFullPath = compareFolder + fileExistInTargetFolderButNotInCompare[i];

                        if (File.Exists(fileNameFullPath))
                        {
                            errorFound = true;
                        }
                        else
                        {
                            i++;
                        }
                    }

                    if (errorFound)
                    {
                        Console.WriteLine("Error! The following file exists, which it shouldn't " + fileNameFullPath);
                        return;
                    }
                }
            }

            folderNameFullPath = currentDir + "\\Update_" + DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss");
            Directory.CreateDirectory(folderNameFullPath);

            Directory.CreateDirectory(folderNameFullPath + "\\Modified files");
            Directory.CreateDirectory(folderNameFullPath + "\\Modified files\\Before modify");
            Directory.CreateDirectory(folderNameFullPath + "\\Modified files\\After modify");
            Directory.CreateDirectory(folderNameFullPath + "\\New files");
            Directory.CreateDirectory(folderNameFullPath + "\\Deleted files");

            if (differentContent != null)
            {
                for (i = 0; i < differentContent.Length; i++)
                {
                    fi = new FileInfo(targetFolder + differentContent[i]);
                    shortFileName = fi.Name;

                    if (File.Exists(folderNameFullPath + "\\Modified files\\Before modify\\" + shortFileName))
                    {
                        index = shortFileName.LastIndexOf('.');
                        suffix = shortFileName.Substring(index);
                        fileNameWithoutSuffix = shortFileName.Substring(0, shortFileName.Length - suffix.Length);
                        n = 1;
                        shortFileName = fileNameWithoutSuffix + "__" + n.ToString() + suffix;

                        while (File.Exists(folderNameFullPath + "\\Modified files\\Before modify\\" + shortFileName))
                        {
                            n++;
                            shortFileName = fileNameWithoutSuffix + "__" + n.ToString() + suffix;
                        }
                    }

                    File.Move(targetFolder + differentContent[i], folderNameFullPath + "\\Modified files\\Before modify\\" + shortFileName);

                    fi = new FileInfo(compareFolder + differentContent[i]);
                    shortFileName = fi.Name;

                    if (File.Exists(folderNameFullPath + "\\Modified files\\After modify\\" + shortFileName))
                    {
                        index = shortFileName.LastIndexOf('.');
                        suffix = shortFileName.Substring(index);
                        fileNameWithoutSuffix = shortFileName.Substring(0, shortFileName.Length - suffix.Length);
                        n = 1;
                        shortFileName = fileNameWithoutSuffix + "__" + n.ToString() + suffix;

                        while (File.Exists(folderNameFullPath + "\\Modified files\\After modify\\" + shortFileName))
                        {
                            n++;
                            shortFileName = fileNameWithoutSuffix + "__" + n.ToString() + suffix;
                        }
                    }

                    File.Copy(compareFolder + differentContent[i], folderNameFullPath + "\\Modified files\\After modify\\" + shortFileName);
                    File.Copy(compareFolder + differentContent[i], targetFolder + differentContent[i]);
                }
            }

            if (fileExistInCompareFolderButNotInTarget != null)
            {
                for (i = 0; i < fileExistInCompareFolderButNotInTarget.Length; i++)
                {


                    fi = new FileInfo(compareFolder + fileExistInCompareFolderButNotInTarget[i]);
                    shortFileName = fi.Name;

                    if (File.Exists(folderNameFullPath + "\\New files\\" + shortFileName))
                    {
                        index = shortFileName.LastIndexOf('.');
                        suffix = shortFileName.Substring(index);
                        fileNameWithoutSuffix = shortFileName.Substring(0, shortFileName.Length - suffix.Length);
                        n = 1;
                        shortFileName = fileNameWithoutSuffix + "__" + n.ToString() + suffix;

                        while (File.Exists(folderNameFullPath + "\\New files\\" + shortFileName))
                        {
                            n++;
                            shortFileName = fileNameWithoutSuffix + "__" + n.ToString() + suffix;
                        }
                    }

                    File.Copy(compareFolder + fileExistInCompareFolderButNotInTarget[i], folderNameFullPath + "\\New files\\" + shortFileName);

                    fi = new FileInfo(targetFolder + fileExistInCompareFolderButNotInTarget[i]);

                    if (!Directory.Exists(fi.DirectoryName))
                    {
                        Directory.CreateDirectory(fi.DirectoryName);
                    }

                    File.Copy(compareFolder + fileExistInCompareFolderButNotInTarget[i], targetFolder + fileExistInCompareFolderButNotInTarget[i]);
                }
            }

            if (fileExistInTargetFolderButNotInCompare != null)
            {
                for (i = 0; i < fileExistInTargetFolderButNotInCompare.Length; i++)
                {
                    fi = new FileInfo(targetFolder + fileExistInTargetFolderButNotInCompare[i]);
                    shortFileName = fi.Name;

                    if (File.Exists(folderNameFullPath + "\\Deleted files\\" + shortFileName))
                    {
                        index = shortFileName.LastIndexOf('.');
                        suffix = shortFileName.Substring(index);
                        fileNameWithoutSuffix = shortFileName.Substring(0, shortFileName.Length - suffix.Length);
                        n = 1;
                        shortFileName = fileNameWithoutSuffix + "__" + n.ToString() + suffix;

                        while (File.Exists(folderNameFullPath + "\\Deleted files\\" + shortFileName))
                        {
                            n++;
                            shortFileName = fileNameWithoutSuffix + "__" + n.ToString() + suffix;
                        }
                    }

                    File.Move(targetFolder + fileExistInTargetFolderButNotInCompare[i], folderNameFullPath + "\\Deleted files\\" + shortFileName);
                }
            }
        }
    }
}
