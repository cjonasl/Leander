﻿using System.Collections;
using System.Text;

namespace Leander.Nr1
{
    public static class R6
    {
        public static void Execute()
        {
            int numberOfFiles, numberOfRows, i, j;
            ArrayList suffix = new ArrayList();
            ArrayList fileNamesFullPath = new ArrayList();
            ArrayList handledFiles = new ArrayList();
            StringBuilder stringBuilderIncomplete, stringBuilderIncorrect, stringBuilderException;
            string errorMessage, str;
            string[] rows;
            int total, numberOfInComplete = 0, numberOfCorrect = 0, numberOfIncorrect = 0, numberOfExceptions = 0;
            ColumnDefinition columnDefinition;

            suffix.Add(".cs");

            Utility.GetFiles("C:\\Code\\Harmony\\Application\\Harmony.Core\\DomainModel\\Imports\\LoadJobs", fileNamesFullPath, suffix, true);

            /*
            fileNamesFullPath.Remove("C:\\Code\\Harmony\\Application\\Harmony.Core\\DomainModel\\Imports\\LoadJobs\\LoadProjects.cs");
            fileNamesFullPath.Remove("C:\\Code\\Harmony\\Application\\Harmony.Core\\DomainModel\\Imports\\LoadJobs\\LoadStaffTargets.cs");
            fileNamesFullPath.Remove("C:\\Code\\Harmony\\Application\\Harmony.Core\\DomainModel\\Imports\\LoadJobs\\LoadJob.cs");
            fileNamesFullPath.Remove("C:\\Code\\Harmony\\Application\\Harmony.Core\\DomainModel\\Imports\\LoadJobs\\LoadLeads.cs");
            fileNamesFullPath.Remove("C:\\Code\\Harmony\\Application\\Harmony.Core\\DomainModel\\Imports\\LoadJobs\\LoadStaffMembers.cs");
            fileNamesFullPath.Remove("C:\\Code\\Harmony\\Application\\Harmony.Core\\DomainModel\\Imports\\LoadJobs\\Contracts\\LoadConsumptionRates.cs");
            fileNamesFullPath.Remove("C:\\Code\\Harmony\\Application\\Harmony.Core\\DomainModel\\Imports\\LoadJobs\\ExternalSystems\\LoadExternalSystemAddresses.cs");
            fileNamesFullPath.Remove("C:\\Code\\Harmony\\Application\\Harmony.Core\\DomainModel\\Imports\\LoadJobs\\ExternalSystems\\LoadExternalSystemAssets.cs");
            fileNamesFullPath.Remove("C:\\Code\\Harmony\\Application\\Harmony.Core\\DomainModel\\Imports\\LoadJobs\\ExternalSystems\\LoadExternalSystemCustomers.cs"); */

            numberOfFiles = fileNamesFullPath.Count;

            stringBuilderIncomplete = new StringBuilder("");
            stringBuilderIncorrect = new StringBuilder("");
            stringBuilderException = new StringBuilder("");

            for (i = 0; i < numberOfFiles; i++)
            {
                rows = Utility.ReturnRowsInFile((string)fileNamesFullPath[i], out errorMessage);

                numberOfRows = rows.Length;

                for(j = 0; j < numberOfRows; j++)
                {
                    if ((rows[j].IndexOf("new ColumnDefinition") >= 0) || (rows[j].IndexOf("new  ColumnDefinition") >= 0) || (rows[j].IndexOf("new   ColumnDefinition") >= 0))
                    {
                        columnDefinition = new ColumnDefinition((string)fileNamesFullPath[i], rows, j);

                        if (handledFiles.IndexOf((string)fileNamesFullPath[i]) == -1)
                            handledFiles.Add((string)fileNamesFullPath[i]);

                        if (columnDefinition.isInComplete)
                        {
                            numberOfInComplete++;
                            stringBuilderIncomplete.Append("----------------------------------------------------------------- Incomplete -----------------------------------------------------------------\r\n" + columnDefinition.ToString() + "\r\n----------------------------------------------------------------------------------------------------------------------------------------------\r\n\r\n");
                        }
                        else if (columnDefinition.isIncorrect)
                        {
                            numberOfIncorrect++;
                            stringBuilderIncorrect.Append("----------------------------------------------------------------- Incorrect -----------------------------------------------------------------\r\n" + columnDefinition.ToString() + "\r\n---------------------------------------------------------------------------------------------------------------------------------------------\r\n\r\n");
                        }
                        else if (columnDefinition.isException)
                        {
                            numberOfExceptions++;
                            stringBuilderException.Append("----------------------------------------------------------------- Exception -----------------------------------------------------------------\r\n" + columnDefinition.ToString() + "\r\n----------------------------------------------------------------------------------------------------------------------------------------------\r\n\r\n");
                        }
                        else
                            numberOfCorrect++;
                    }
                }
            }

            total = numberOfCorrect + numberOfInComplete +  numberOfIncorrect + numberOfExceptions;

            str = string.Format("Found new ColumnDefinition: {0}, correct: {1}, incomplete: {2}, incorrect: {3} and exception: {4}\r\n\r\n", total.ToString(), numberOfCorrect.ToString(), numberOfInComplete.ToString(), numberOfIncorrect.ToString(), numberOfExceptions.ToString());
            str += stringBuilderIncomplete.ToString();
            str += stringBuilderIncorrect.ToString();
            str += stringBuilderException.ToString();
            Utility.CreateNewFile("C:\\tmp\\Result validate feature 20171122.15.txt", str);

            handledFiles.Sort();
            Utility.CreateNewFile("C:\\tmp\\Handled files feature 20171122.15.txt", Utility.ReturnItems(handledFiles, "\r\n"));
        }
    }
}
