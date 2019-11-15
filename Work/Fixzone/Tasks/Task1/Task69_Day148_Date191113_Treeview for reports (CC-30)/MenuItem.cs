using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Web;

namespace ClientConnect.ViewModels.ViewDash
{
    [Serializable]
    public class MenuItem
    {
        public int Id { get; set; }
        public string MenuName { get; set; }
        public string FileNameFullPath { get; set; }
        public int ParentMenuId { get; set; }

        public MenuItem(int id, string menuName, string fileNameFullPath, int parentMenuId)
        {
            this.Id = id;
            this.MenuName = menuName;
            this.FileNameFullPath = fileNameFullPath;
            this.ParentMenuId = parentMenuId;
        }

        public static List<MenuItem> GetMenuItemList(string baseDir, string[] fileNames)
        {
            string fileName, fimeNameFullPath;
            string[] v;
            int i, j, id, parentId;
            bool folderFound;
            ArrayList a;
            List<MenuItem> list = new List<MenuItem>();

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
                fileName = ((string)arrayListFileNames[i]).ToLower().Trim() + ".mrt";
                folderFound = false;
                j = 0;

                while (j < arrayListFolderNames.Count && !folderFound)
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

            id = 0;

            for (i = 0; i < arrayListFolderNames.Count; i++)
            {
                a = (ArrayList)filesInFolders[i];

                if (a.Count > 0)
                {
                    id++;
                    parentId = id;
                    list.Add(new MenuItem(id, (string)arrayListFolderNames[i], "#", 0));
                    
                    for (j = 0; j < a.Count; j++)
                    {
                        id++;
                        fimeNameFullPath = string.Format("{0}\\{1}\\{2}.mrt", baseDir, (string)arrayListFolderNames[i], (string)a[j]);
                        list.Add(new MenuItem(id, (string)a[j], fimeNameFullPath, parentId));
                    } 
                }
            }

            return list;
        }
    }
}