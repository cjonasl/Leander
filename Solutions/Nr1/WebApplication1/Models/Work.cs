using System;
using System.Collections.Generic;
using Leander.Nr1;

namespace WebApplication1.Models
{
    public class Work
    {
        public string FullName { get; set; }
        public string ShortName { get; set; }  //Obs, no blank in ShortName
        public string Folder { get; set; }  //In this folder, the following files should exist: DayDate.txt and NextTaskId.txt and the following folders: Diary and Tasks

        public Work() { }
        public Work(string fullName, string shortName, string folder)
        {
            this.FullName = fullName;
            this.ShortName = shortName;
            this.Folder = folder;
        }
    }

    public class WorkUtility
    {
        private const string _fileNameFullPathToConfigFile = "C:\\git_cjonasl\\Leander\\Solutions\\Nr1\\WebApplication1\\Text\\Page1Menu1Sub1Sub3Tab1.txt";

        private static Work DeserializeWork(string work)
        {
            string[] v = work.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            return new Work(v[0], v[1], v[2]);
        }

        private static string SerializeWork(Work work)
        {
            return string.Format("{0}\r\n{1}\r\n{2}", work.FullName, work.ShortName, work.Folder);
        }

        private static List<Work> ReturnListWithWorks(out string errorMessage)
        {
            List<Work> listWithWorks;
            string fileContents;
            int index;
            string[] v;
            int i;

            try
            {
                errorMessage = null;
                listWithWorks = new List<Work>();
                fileContents = Utility.ReturnFileContents(_fileNameFullPathToConfigFile);
                index = fileContents.IndexOf("*/");
                v = fileContents.Substring(4 + index).Split(new string[] { "\r\n----- New work -----\r\n" }, StringSplitOptions.None);

                for (i = 0; i < v.Length; i++)
                {
                    listWithWorks.Add(DeserializeWork(v[i]));
                }
            }
            catch (Exception e)
            {
                errorMessage = string.Format("ERROR!! An Exception occured in method ReturnListWithWorks! e.Message:\r\n{0}", e.Message);
                return null;
            }

            return listWithWorks;
        }

        public static void CreateTask(string shortNameTitle, out string message)
        {
            message = null;

            try
            {
                string shortName, title, fileNameFullPath, fileContents, str, dateStr = "", folder;
                bool todaysDateInFileDayDatetxt = false;
                DateTime tmpDate, dateToday;
                string[] v;
                int day = 0, index, i, taskId, nextTaskId, folderIndex;
                List<Work> list;

                index = shortNameTitle.IndexOf(' ');
                shortName = shortNameTitle.Substring(0, index);
                title = shortNameTitle.Substring(index).Trim();

                list = ReturnListWithWorks(out message);

                if (!string.IsNullOrEmpty(message))
                    return;

                index = -1;
                i = 0;

                while (i < list.Count && index == -1)
                {
                    if (shortName == list[i].ShortName)
                        index = i;
                    else
                        i++;
                }

                if (index == -1)
                {
                    message = string.Format("Can not find shortName \"{0}\" in the config file \"{1}\"!", shortName, _fileNameFullPathToConfigFile);
                    return;
                }

                fileNameFullPath = string.Format("{0}\\DayDate.txt", list[index].Folder);

                if (!System.IO.File.Exists(fileNameFullPath))
                {
                    message = string.Format("The following file does not exist as expected: {0}", fileNameFullPath);
                    return;
                }

                fileContents = Utility.ReturnFileContents(fileNameFullPath);
                v = fileContents.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                if (v.Length >= 1)
                {
                    str = v[0];
                    v = str.Split(' ');

                    if (v.Length == 2)
                    {
                        tmpDate = DateTime.Now;
                        dateToday = new DateTime(tmpDate.Year, tmpDate.Month, tmpDate.Day);
                        dateStr = dateToday.ToString("yyyy-MM-dd");

                        if (dateStr == v[1])
                            todaysDateInFileDayDatetxt = true;

                        day = int.Parse(v[0]);
                    }
                }

                if (!todaysDateInFileDayDatetxt)
                {
                    message = string.Format("Today's date is not registered in file {0}!", fileNameFullPath);
                    return;
                }


                fileNameFullPath = string.Format("{0}\\NextTaskId.txt", list[index].Folder);

                if (!System.IO.File.Exists(fileNameFullPath))
                {
                    message = string.Format("The following file does not exist as expected: {0}", fileNameFullPath);
                    return;
                }

                fileContents = Utility.ReturnFileContents(fileNameFullPath);

                taskId = int.Parse(fileContents);
                nextTaskId = 1 + taskId;

                folderIndex = taskId / 100;

                if ((taskId % 100) != 0)
                    folderIndex++;

                folder = string.Format("{0}\\Tasks\\Task{1}\\Task{2}_Day{3}_Date{4}{5}{6}_{7}", list[index].Folder, folderIndex, taskId.ToString(), day.ToString(), dateStr.Substring(2, 2), dateStr.Substring(5, 2), dateStr.Substring(8, 2), title);

                System.IO.Directory.CreateDirectory(folder);
                Utility.CreateNewFile(fileNameFullPath, nextTaskId.ToString());
                message = string.Format("Task number {0} was successfully created for {1}", taskId, list[index].FullName);
            }
            catch (Exception e)
            {
                message = string.Format("ERROR!! An Exception occured in method CreateTask! e.Message:\r\n{0}", e.Message);
                return;
            }
        }
    }
}