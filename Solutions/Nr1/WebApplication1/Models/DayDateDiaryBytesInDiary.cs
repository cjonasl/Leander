using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using Leander.Nr1;

namespace WebApplication1.Models
{
    public class DayDateDiaryBytesInDiary
    {
        public int Day { get; set; }
        public string Date { get; set; }
        public string Diary { get; set; }
        public long BytesInDiary { get; set; }
        public string WarningMessage { get; set; }

        public DayDateDiaryBytesInDiary() { }

        public DayDateDiaryBytesInDiary(int day, string date, string diary, long bytesInDiary, string warningMessage)
        {
            this.Day = day;
            this.Date = date;
            this.Diary = diary;
            this.BytesInDiary = bytesInDiary;
            this.WarningMessage = warningMessage;
        }
    }


    public static class DayDateDiaryBytesInDiaryUtility
    {
        private const long _minNumberOfBytesInDiaryFileAccepted = 250;

        private static string ReturnDiaryFileName(int day, DateTime date)
        {
            string dateStr = date.ToString("yyyy-MM-dd");
            return string.Format("Day{0}Date{1}{2}{3}.txt", day.ToString().PadLeft(4, '0'), dateStr.Substring(2, 2), dateStr.Substring(5, 2), dateStr.Substring(8, 2));
        }

        private static long ReturnNumberOfBytesInFile(string diaryFolder, int day, DateTime date)
        {
            string dateStr = date.ToString("yyyy-MM-dd");
            string fileNameFullPath = string.Format("{0}\\Day{1}Date{2}{3}{4}.txt", diaryFolder, day.ToString().PadLeft(4, '0'), dateStr.Substring(2, 2), dateStr.Substring(5, 2), dateStr.Substring(8, 2));

            if (System.IO.File.Exists(fileNameFullPath))
                return Utility.ReturnNumberOfBytesInFile(fileNameFullPath, true);
            else
                return 0;
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
                    errorMessage = string.Format("ERROR!! Row {0} in file {1} does not have 2 columns with day and date blank separated!", i.ToString(), fileNameFullPath);
                else if (!int.TryParse(columns[0], out currentDay))
                    errorMessage = string.Format("ERROR!! Value \"{0}\" in row {1} in file {2} is not a valid positive integer representing day!", columns[0], i.ToString(), fileNameFullPath);
                else if (!Utility.TryParse(columns[1], out currentDate))
                    errorMessage = string.Format("ERROR!! Value \"{0}\" in row {1} in file {2} is not a valid date in format yyyy-MM-dd!", columns[0], i.ToString(), fileNameFullPath);
                else
                {
                    if (currentDay <= 0)
                    {
                        errorMessage = string.Format("ERROR!! Value \"{0}\" in row {1} in file {2} is not a valid positive integer representing day!", columns[0], i.ToString(), fileNameFullPath);
                    }
                    else if (i > 1)
                    {
                        if ((currentDay + 1) != previousDay)
                            errorMessage = string.Format("ERROR!! In row {0} in file {1}, day ({2}) is not one less than day on row above ({3}) as expected!", i.ToString(), fileNameFullPath, currentDay.ToString(), previousDay.ToString());
                        else if (currentDate >= previousDate)
                            errorMessage = string.Format("ERROR!! In row {0} in file {1}, date ({2}) is not less than date on row above ({3}) as expected!", i.ToString(), fileNameFullPath, currentDate.ToString("yyyy-MM-dd"), previousDate.ToString("yyyy-MM-dd"));
                    }
                    else
                    {
                        if ((DateTime.Now.ToString("yyyy-MM-dd")) == columns[1])
                            todaysDayIsInFile = true;

                        //For test:
                        //if (GetTodaysDate().ToString("yyyy-MM-dd") == columns[1])
                        //todaysDayIsInFile = true;
                    }
                }

                if (errorMessage == null)
                {
                    previousDay = currentDay;
                    previousDate = currentDate;
                    day.Add(currentDay);
                    date.Add(currentDate);
                }

                i++;
            }

            if (errorMessage == null)
                return true;
            else
                return false;
        }

        public static string ReturnWarningMessage(string workFolder)
        {
            string fileNameFullPath, fileContents, warningMessage;
            string[] rows, columns;
            int i, n;
            long numberOfBytesInFile;

            try
            {
                warningMessage = null;

                fileNameFullPath = string.Format("{0}\\DayDate.txt", workFolder);

                if (!System.IO.File.Exists(fileNameFullPath))
                    return string.Format("ERROR!! The following file does not exist as expected: {0}!", fileNameFullPath);

                fileContents = Utility.ReturnFileContents(fileNameFullPath);

                if (string.IsNullOrEmpty(fileContents))
                    return null;

                rows = fileContents.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                n = rows.Length;
                i = 0;

                while ((i < n) && (warningMessage == null))
                {
                    columns = rows[n - 1 - i].Split(' ');
                    fileNameFullPath = string.Format("{0}\\Diary\\Day{1}Date{2}{3}{4}.txt", workFolder, columns[0].PadLeft(4, '0'), columns[1].Substring(2, 2), columns[1].Substring(5, 2), columns[1].Substring(8, 2));

                    if (!System.IO.File.Exists(fileNameFullPath))
                        warningMessage = string.Format("The diary file \"{0}\" for day {1} does not exist!", fileNameFullPath, columns[0]);
                    else if (i < (n - 3)) //Do not warn for less than acceptable number of bytes for last 3 days, because they are about to be written
                    {
                        numberOfBytesInFile = Utility.ReturnNumberOfBytesInFile(fileNameFullPath, true);

                        if (numberOfBytesInFile < _minNumberOfBytesInDiaryFileAccepted)
                        {
                            warningMessage = string.Format("Number of bytes in file \"{0}\" for day {1} is {2}. which is less than acceptable (which is {3})!", fileNameFullPath, columns[0], numberOfBytesInFile.ToString(), _minNumberOfBytesInDiaryFileAccepted.ToString());
                        }
                    }

                    i++;
                }
            }
            catch (Exception e)
            {
                return string.Format("ERROR!! An Exception occured in method ReturnWarningMessage! e.Message:\r\n{0}", e.Message);
            }


            return warningMessage;
        }

        public static List<DayDateDiaryBytesInDiary> ReturnListWithDayDateDiaryBytesInDiary(string workFolder, out bool todaysDayIsInFile, out string errorMessage)
        {
            string fileNameFullPath;
            ArrayList day, date;
            List<DayDateDiaryBytesInDiary> listWithDayDateDiaryBytesInDiary;
            int i;

            todaysDayIsInFile = false; //Default
            errorMessage = null;

            try
            {
                fileNameFullPath = string.Format("{0}\\DayDate.txt", workFolder);

                if (!System.IO.File.Exists(fileNameFullPath))
                {
                    errorMessage = string.Format("ERROR!! The following file does not exist as expected: {0}!", fileNameFullPath);
                    return null;
                }

                if (!fileContentIsCorrect(fileNameFullPath, out todaysDayIsInFile, out day, out date, out errorMessage))
                    return null;

                listWithDayDateDiaryBytesInDiary = new List<DayDateDiaryBytesInDiary>();

                for (i = 0; i < day.Count; i++)
                {
                    listWithDayDateDiaryBytesInDiary.Add(new DayDateDiaryBytesInDiary((int)day[i], Utility.ReturnDateTimeAsLongSwedishString((DateTime)date[i]), ReturnDiaryFileName((int)day[i], (DateTime)date[i]), ReturnNumberOfBytesInFile(workFolder + "\\Diary", (int)day[i], (DateTime)date[i]), null));
                }

                errorMessage = ReturnWarningMessage(workFolder);
            }
            catch (Exception e)
            {
                errorMessage = string.Format("ERROR!! An Exception occured in method ReturnListWithDayDateDiaryBytesInDiary! e.Message:\r\n{0}", e.Message);
                return null;
            }

            return listWithDayDateDiaryBytesInDiary;
        }

        private static string GetFileContents(string fileNameFullPath, out int lastDayInFile, out DateTime lastDateInFile, out string errorMessage)
        {
            string fileContents;
            string[] rows, columns;

            lastDayInFile = 0;
            lastDateInFile = DateTime.MinValue;
            errorMessage = null;

            fileContents = Utility.ReturnFileContents(fileNameFullPath);

            if (string.IsNullOrEmpty(fileContents))
                return "";
            else
            {
                rows = fileContents.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                columns = rows[0].Split(' ');

                if (columns.Length != 2)
                    errorMessage = string.Format("ERROR!! First row in file {0} does not have 2 columns with day and date blank separated!", fileNameFullPath);
                else if (!int.TryParse(columns[0], out lastDayInFile))
                    errorMessage = string.Format("ERROR!! Value \"{0}\" in first row in file {1} is not a valid positive integer representing day!", columns[0], fileNameFullPath);
                else if (!Utility.TryParse(columns[1], out lastDateInFile))
                    errorMessage = string.Format("ERROR!! Value \"{0}\" in first row in file {1} is not a valid date in format yyyy-MM-dd!", columns[0], fileNameFullPath);

                if (lastDayInFile <= 0)
                {
                    errorMessage = string.Format("ERROR!! Value \"{0}\" in first row in file {1} is not a valid positive integer representing day!", columns[0], fileNameFullPath);
                }
            }

            if (errorMessage == null)
                return fileContents;
            else
                return "";
        }

        private static string ReturnWarningMessage(DateTime lastDateInFile, DateTime dateToday)
        {
            DateTime dt;
            StringBuilder sb;
            string str;
            bool dateTodayIsAWeekendDate = false;
            bool foundNonWeekendDateNotRegistered = false;
            int n;

            sb = new StringBuilder("");

            if (dateToday.DayOfWeek == DayOfWeek.Saturday)
            {
                sb.Append("OBS! The newly registered date, i.e. the date today, is a saturday!");
                dateTodayIsAWeekendDate = true;
            }
            else if (dateToday.DayOfWeek == DayOfWeek.Sunday)
            {
                sb.Append("OBS! The newly registered date, i.e. the date today, is a sunday!");
                dateTodayIsAWeekendDate = true;
            }

            dt = lastDateInFile.AddDays(1.0);
            sb.Append(string.Format("{0}OBS! #####REPLACE#####", string.IsNullOrEmpty(sb.ToString()) ? "" : " "));

            n = 0;

            while (dt < dateToday)
            {
                if ((dt.DayOfWeek != DayOfWeek.Saturday) && (dt.DayOfWeek != DayOfWeek.Sunday))
                {
                    sb.Append(string.Format("\r\n{0} ({1})", dt.ToString("yyyy-MM-dd"), dt.DayOfWeek.ToString().ToLower()));
                    foundNonWeekendDateNotRegistered = true;
                    n++;
                }

                dt = dt.AddDays(1.0);
            }

            str = sb.ToString();

            if (n == 1)
                str = str.Replace("#####REPLACE#####", "This weekday, i.e. not saturday or sunday, has not been registered as a day in the diary:");
            else if (n > 1)
                str = str.Replace("#####REPLACE#####", "These weekdays, i.e. not saturday or sunday, have not been registered as days in the diary:");

            if (dateTodayIsAWeekendDate || foundNonWeekendDateNotRegistered)
                return str;
            else
                return null;
        }

        public static DayDateDiaryBytesInDiary AddNewWorkDay(string workFolder, out string errorMessage)
        {
            string fileNameFullPathDayDateFile, fileNameFullPathDiaryFile, dateTimeAsLongSwedishString, fileNameShort, fileContents, dateStr, warningMessage;
            int lastDayInFile, d;
            DateTime lastDateInFile, dateToday, tmpDate;

            errorMessage = null;

            try
            {
                fileNameFullPathDayDateFile = string.Format("{0}\\DayDate.txt", workFolder);

                if (!System.IO.File.Exists(fileNameFullPathDayDateFile))
                {
                    errorMessage = string.Format("ERROR!! The following file does not exist as expected: {0}!", fileNameFullPathDayDateFile);
                    return null;
                }

                fileContents = GetFileContents(fileNameFullPathDayDateFile, out lastDayInFile, out lastDateInFile, out errorMessage);

                if (errorMessage != null)
                    return null;

                tmpDate = DateTime.Now;
                dateToday = new DateTime(tmpDate.Year, tmpDate.Month, tmpDate.Day); //for test: dateToday = GetTodaysDate();

                d = 1 + lastDayInFile;
                dateStr = dateToday.ToString("yyyy-MM-dd");
                fileNameShort = string.Format("Day{0}Date{1}{2}{3}.txt", d.ToString().PadLeft(4, '0'), dateStr.Substring(2, 2), dateStr.Substring(5, 2), dateStr.Substring(8, 2));
                fileNameFullPathDiaryFile = string.Format("{0}\\{1}", workFolder, fileNameShort);
                dateTimeAsLongSwedishString = Utility.ReturnDateTimeAsLongSwedishString(dateToday);

                if (string.IsNullOrEmpty(fileContents))
                {
                    Utility.CreateNewFile(fileNameFullPathDiaryFile, string.Format("Dag 1:\r\n{0}", dateTimeAsLongSwedishString));
                    Utility.CreateNewFile(fileNameFullPathDayDateFile, string.Format("1 {0}", dateStr));
                    return new DayDateDiaryBytesInDiary(1, dateTimeAsLongSwedishString, fileNameShort, Utility.ReturnNumberOfBytesInFile(fileNameFullPathDiaryFile, true), null);
                }

                if (lastDateInFile == dateToday)
                {
                    errorMessage = "ERROR!! Today's date is already added!";
                    return null;
                }
                else if (lastDateInFile > dateToday)
                {
                    errorMessage = string.Format("ERROR!! Last date in file, {0}, is in the future!", lastDateInFile.ToString("yyyy-MM-dd"));
                    return null;
                }
                else
                {
                    warningMessage = ReturnWarningMessage(lastDateInFile, dateToday);
                }
            }
            catch (Exception e)
            {
                errorMessage = string.Format("ERROR!! An Exception occured in method AddNewWorkDay! e.Message:\r\n{0}", e.Message);
                return null;
            }

            Utility.CreateNewFile(fileNameFullPathDiaryFile, string.Format("Dag {0}:\r\n{1}", d.ToString(), dateTimeAsLongSwedishString));
            Utility.CreateNewFile(fileNameFullPathDayDateFile, string.Format("{0} {1}\r\n{2}", d.ToString(), dateStr, fileContents));
            return new DayDateDiaryBytesInDiary(d, dateTimeAsLongSwedishString, fileNameShort, Utility.ReturnNumberOfBytesInFile(fileNameFullPathDiaryFile, true), warningMessage);
        }

        public static BytesInDiaryWarningMessage GetBytesInDiaryWarningMessage(string diaryFileNameFullPath, out string errorMessage)
        {
            long bytesInDiary;
            string warningMessage;
            FileInfo fi;

            try
            {
                errorMessage = null;
                bytesInDiary = Utility.ReturnNumberOfBytesInFile(diaryFileNameFullPath, true);
                fi = new FileInfo(diaryFileNameFullPath);
                warningMessage = ReturnWarningMessage(fi.DirectoryName);
            }
            catch (Exception e)
            {
                errorMessage = string.Format("ERROR!! An Exception occured in method GetBytesInDiaryWarningMessage! e.Message:\r\n{0}", e.Message);
                return null;
            }

            return new BytesInDiaryWarningMessage(bytesInDiary, warningMessage);
        }

        /// <summary>
        /// For test
        /// </summary>
        /// <returns></returns>
        public static DateTime GetTodaysDate()
        {
            string str = Utility.ReturnFileContents(@"C:\AAA\abc.txt");
            string[] v = str.Split(' ');
            return new DateTime(int.Parse(v[0]), int.Parse(v[1]), int.Parse(v[2]));
        }
    }
}