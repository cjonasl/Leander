using System;
using System.Text;

namespace Leander.Nr1
{
    public static class R17
    {
        public static void Execute()
        {
            string[] v;
            int i, workDay;
            string fileNameShort, year, month, day, str;
            ArrayList arrayList, dayDate, numberOfBytes;
            StringBuilder sb;
            FileInfo fi;

            v = Directory.GetFiles("C:\\git_cjonasl\\Leander\\Work\\Employer");
            arrayList = new ArrayList(v);
            dayDate = new ArrayList();
            numberOfBytes = new ArrayList();
            arrayList.Sort();

            sb = new StringBuilder();

            for(i = 0; i < arrayList.Count; i++)
            {
                fi = new FileInfo((string)arrayList[arrayList.Count - 1 - i]);
                fileNameShort = fi.Name;
                workDay = int.Parse(fileNameShort.Substring(3, 4));
                year = "20" + fileNameShort.Substring(11, 2);
                month = fileNameShort.Substring(13, 2);
                day = fileNameShort.Substring(15, 2);
                str = string.Format("{0} {1}-{2}-{3}", workDay, year, month, day);
                sb.Append(str + "\r\n");
                dayDate.Add(str);
                numberOfBytes.Add(fi.Length);
            }       

            Utility.CreateNewFile("C:\\git_cjonasl\\Leander\\Work\\Employer\\DayDate.txt", sb.ToString().TrimEnd());

            Utility.SortWrtLong(numberOfBytes, dayDate, false); //Sort desc
            sb.Clear();

            for(i = 0; i < numberOfBytes.Count; i++)
            {
                sb.Append(string.Format("{0} {1}\r\n", (string)dayDate[i], numberOfBytes[i].ToString()));
            }

            Utility.CreateNewFile("C:\\git_cjonasl\\Leander\\Work\\Employer\\DayDateNumberOfBytesInFile.txt", sb.ToString().TrimEnd());
        }
    }
}