using System;
using System.Collections.Generic;
using System.Collections;
using Leander.Nr1;

namespace WebApplication1.Models
{
    public class DayDateDiaryBytesInDiary
    {
        public int Day { get; set; }
        public string Date { get; set; }
        public string Diary { get; set; }
        public long BytesInDiary { get; set; }

        public DayDateDiaryBytesInDiary() { }

        public DayDateDiaryBytesInDiary(int day, string date, string diary, long bytesInDiary)
        {
            this.Day = day;
            this.Date = date;
            this.Diary = diary;
            this.BytesInDiary = bytesInDiary;
        }

        public static class DayDateDiaryBytesInDiaryUtility
        {
            private const long _minNumberOfBytesInDiaryFileAccepted = 200;


            public static List<DayDateDiaryBytesInDiary> ReturnListWithDayDateDiaryBytesInDiary(string baseFolder, out bool todaysDayIsInFile, out string errorMessage)
            {
                string fileNameFullPath;
                ArrayList day, date, diary, bytesInfile;
                List<DayDateDiaryBytesInDiary> listWithDayDateDiaryBytesInDiary;
                int i;

                todaysDayIsInFile = false; //Default
                errorMessage = null;

                try
                {
                    fileNameFullPath = string.Format("{0}\\DayDateDiaryBytesInDiary.txt", baseFolder);

                    if (!System.IO.File.Exists(fileNameFullPath))
                    {
                        errorMessage = string.Format("The following file does not exist as expected: {0}", fileNameFullPath);
                        return null;
                    }

                    if (!fileContentIsCorrect(fileNameFullPath, out todaysDayIsInFile, out day, out date, out errorMessage))
                        return null;

                    if (!AllDiaryFilesExist(baseFolder, day, date, out diary, out bytesInfile, out errorMessage))
                        return null;

                    listWithDayDateDiaryBytesInDiary = new List<DayDateDiaryBytesInDiary>();

                    for(i = 0; i < day.Count; i++)
                    {
                        listWithDayDateDiaryBytesInDiary.Add(new DayDateDiaryBytesInDiary((int)day[i], Utility.ReturnDateTimeAsLongSwedishString((DateTime)date[i]), (string)diary[i], (long)bytesInfile[i]));

                        if ((errorMessage == null) && ((long)bytesInfile[i] < _minNumberOfBytesInDiaryFileAccepted))
                            errorMessage = string.Format("Diary for day {0} contains only {1} bytes, which is less than accepted (which is {2} bytes)!", day[i].ToString(), bytesInfile[i].ToString(), _minNumberOfBytesInDiaryFileAccepted.ToString());
                    }
                }
                catch (Exception e)
                {
                    errorMessage = string.Format("ERROR!! An Exception occured in method ReturnListWithDayDateDiaryBytesInDiary! e.Message:\r\n{0}", e.Message);
                    return null;
                }

                return listWithDayDateDiaryBytesInDiary;
            }
        }

        private static bool fileContentIsCorrect(string fileNameFullPath, out bool todaysDayIsInFile, out ArrayList day, out ArrayList date, out string errorMessage)
        {
            string fileContents;
            string[] rows, columns;
            int i, currentDay, previousDay;
            DateTime currentDate, previousDate;

            fileContents = Utility.ReturnFileContents(fileNameFullPath);

            todaysDayIsInFile = false;
            day = new ArrayList();
            date = new ArrayList();
            errorMessage = null;
            currentDay = 0;
            previousDay = 0;
            currentDate = DateTime.MinValue;
            previousDate = DateTime.MinValue;

            if (string.IsNullOrEmpty(fileContents))
                return true;

            rows = fileContents.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            i = 1;

            while ((errorMessage == null) && (i <= rows.Length))
            {
                columns = rows[i - 1].Split(' ');

                if (columns.Length != 2)
                    errorMessage = string.Format("Row {0} in file {1} does not have 2 columns with day and date blank separated!", i.ToString(), fileNameFullPath);
                else if (!int.TryParse(columns[0],  out currentDay))
                    errorMessage = string.Format("Value \"{0}\" in row {1} in file {2} is not a valid positive integer representing day!", columns[0], i.ToString(), fileNameFullPath);
                else if (!Utility.TryParse(columns[1], out currentDate))
                    errorMessage = string.Format("Value \"{0}\" in row {1} in file {2} is not a valid date in format yyyy-MM-dd!", columns[0], i.ToString(), fileNameFullPath);
                else
                {
                    if (i > 1)
                    {
                        if ((currentDay + 1) != previousDay)
                            errorMessage = string.Format("In row {0} in file {1}, day ({2}) is not one less than day on row above ({3}) as expected!", i.ToString(), fileNameFullPath, currentDay.ToString(), previousDay.ToString());
                        else if (currentDate >= previousDate)
                            errorMessage = string.Format("In row {0} in file {1}, date ({2}) is not less than date on row above ({3}) as expected!", i.ToString(), fileNameFullPath, currentDate.ToString("yyyy-MM-dd"), previousDate.ToString("yyyy-MM-dd"));
                    }
                    else
                    {
                        if ((DateTime.Now.ToString("yyyy-MM-dd")) == columns[1])
                            todaysDayIsInFile = true;
                    }
                }

                if (errorMessage == null)
                {
                    previousDay = currentDay;
                    previousDate = currentDate;
                    day.Add(currentDay);
                    date.Add(currentDate);
                }
            }

            if (errorMessage == null)
                return true;
            else
                return false;
        }

        private static bool AllDiaryFilesExist(string baseFolder, ArrayList day, ArrayList date, out ArrayList diary, out ArrayList bytesInfile, out string errorMessage)
        {
            string str, fileNameShort, fileNameFullPath;
            DateTime dt;
            int i, d;

            diary = new ArrayList();
            bytesInfile = new ArrayList();
            errorMessage = null;

            i = 0;

            while ((i < date.Count) && (errorMessage == null))
            {
                d = (int)day[i];
                dt = (DateTime)date[i];
                str = dt.ToString("yyyy-MM-dd");
                fileNameShort = string.Format("Day{0}Date{1}{2}{3}.txt", d.ToString().PadLeft(4, '0'), str.Substring(2, 2), str.Substring(5, 2), str.Substring(8, 2));
                fileNameFullPath = string.Format("{0}\\{1}", baseFolder, fileNameShort);

                if (!System.IO.File.Exists(fileNameFullPath))
                    errorMessage = string.Format("The diary file {0} does not exist as expected", fileNameFullPath);
                else
                {
                    diary.Add(fileNameShort);
                    bytesInfile.Add(Utility.ReturnNumberOfBytesInFile(fileNameFullPath, true));
                }

                i++;
            }

            if (errorMessage == null)
                return true;
            else
                return false;
        }
    }
}