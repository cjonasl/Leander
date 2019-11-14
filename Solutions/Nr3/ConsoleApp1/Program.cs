using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseDir, fileName;
            string[] fileNames, v;
            int i, j;
            bool folderFound;
            ArrayList a;

            baseDir = "C:\\A";
            fileNames = new string[] { "AwaitingForApproval.cshtml", "Index.cshtml", "UserList.cshtml", "Jonas.cshtml", "Knut.cshtml", "Malin.cs", "Sara.cs" };

            string[] folderNamesFullPath = Directory.GetDirectories(baseDir);
            string[] folderNames = folderNamesFullPath.Select(x => new DirectoryInfo(x).Name).ToArray();

            ArrayList arrayListFolderNames = new ArrayList(folderNames);
            arrayListFolderNames.Sort();

            ArrayList filesInFolders = new ArrayList();
            ArrayList allFilesInFolders = new ArrayList();

            for (i = 0; i < arrayListFolderNames.Count; i++)
            {
                filesInFolders.Add(new ArrayList());
                v = Directory.GetFiles(baseDir + "\\" + (string)arrayListFolderNames[i]);
                v = v.Select(x => new FileInfo(x).Name.ToLower().Trim()).ToArray();
                allFilesInFolders.Add(new ArrayList(v));
            }

            ArrayList arrayListFileNames = new ArrayList(fileNames);
            arrayListFileNames.Sort();

            for (i = 0; i < arrayListFileNames.Count; i++)
            {
                fileName = ((string)arrayListFileNames[i]).ToLower().Trim();
                folderFound = false;
                j = 0;

                while(j < arrayListFolderNames.Count && !folderFound)
                {
                    a = (ArrayList)allFilesInFolders[j];

                    if (a.IndexOf(fileName) >= 0)
                    {
                        folderFound = true;
                        a = (ArrayList)filesInFolders[j];
                        a.Add(arrayListFileNames[i]);
                    }
                    else
                    {
                        j++;
                    }
                }
           
                if (!folderFound)
                {
                    throw new Exception(string.Format("Can not find file {0}!", (string)arrayListFileNames[i]));
                }
            }

            StringBuilder sb = new StringBuilder();

            for (i = 0; i < arrayListFolderNames.Count; i++)
            {
                a = (ArrayList)filesInFolders[i];

                if (a.Count > 0)
                {
                    sb.Append((string)arrayListFolderNames[i] + "\r\n");

                    for (j = 0; j < a.Count; j++)
                    {
                        sb.Append("  " + (string)a[j] + ": " + baseDir + "\\" + (string)arrayListFolderNames[i] + "\\" + (string)a[j] + "\r\n");
                    }
                }
            }

            File.WriteAllText("C:\\A\\Tmp20191113\\aaa.txt", sb.ToString());

            //Test2.Program.Run();
        }
    }
}
