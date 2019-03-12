using System;
using System.Collections.Generic;
using Leander.Nr1;

namespace WebApplication1.Models
{
    public class CommunicationCounterparty
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FileNameFullPath { get; set; }

        public CommunicationCounterparty() { }
        public CommunicationCounterparty(int id, string name, string fileNameFullPath)
        {
            this.Id = id;
            this.Name = name;
            this.FileNameFullPath = fileNameFullPath;
        }
    }

    public class CommunicationCounterpartyUtility
    {
        private static CommunicationCounterparty DeserializeCommunicationCounterparty(string communicationCounterparty)
        {
            string[] v = communicationCounterparty.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            return new CommunicationCounterparty(int.Parse(v[0]), v[1], v[2]);
        }

        private static string SerializeCommunicationCounterparty(CommunicationCounterparty communicationCounterparty)
        {
            return string.Format("{0}\r\n{1}\r\n{2}", communicationCounterparty.Id.ToString(), communicationCounterparty.Name, communicationCounterparty.FileNameFullPath);
        }

        public static List<CommunicationCounterparty> ReturnListWithCommunicationCounterparties(string fileNameFullPathToConfigFile, out string errorMessage)
        {
            List<CommunicationCounterparty> listWithCommunicationCounterparties;
            string fileContents;
            int index;
            string[] v;
            int i;

            try
            {
                errorMessage = null;
                listWithCommunicationCounterparties = new List<CommunicationCounterparty>();
                fileContents = Utility.ReturnFileContents(fileNameFullPathToConfigFile);
                index = fileContents.IndexOf("*/");
                v = fileContents.Substring(4 + index).Split(new string[] { "\r\n----- New communication -----\r\n" }, StringSplitOptions.None);

                for (i = 0; i < v.Length; i++)
                {
                    listWithCommunicationCounterparties.Add(DeserializeCommunicationCounterparty(v[i]));
                }
            }
            catch (Exception e)
            {
                errorMessage = string.Format("ERROR!! An Exception occured in method ReturnListWithCommunicationCounterparties! e.Message:\r\n{0}", e.Message);
                return null;
            }

            return listWithCommunicationCounterparties;
        }


        public static IdText[] ReturnArrayWithIdText(string fileNameFullPathToConfigFile, out string errorMessage)
        {
            List<CommunicationCounterparty> listWithCommunicationCounterparties;
            IdText[] arrayWithIdText;

            errorMessage = null;

            try
            {
                listWithCommunicationCounterparties = ReturnListWithCommunicationCounterparties(fileNameFullPathToConfigFile, out errorMessage);

                if (errorMessage != null)
                    return null;

                arrayWithIdText = new IdText[listWithCommunicationCounterparties.Count];

                for (int i = 0; i < listWithCommunicationCounterparties.Count; i++)
                {
                    arrayWithIdText[i] = new IdText(listWithCommunicationCounterparties[i].Id, listWithCommunicationCounterparties[i].Name);
                }
            }
            catch (Exception e)
            {
                errorMessage = string.Format("ERROR!! An Exception occured in method CommunicationCounterpartyUtility.ReturnArrayWithIdText! e.Message:\r\n{0}", e.Message);
                return null;
            }

            return arrayWithIdText;
        }

        public static string ReturnFileNameFullPathForCounterparty(string fileNameFullPathToConfigFile, int id, out string errorMessage)
        {
            List<CommunicationCounterparty> listWithCommunicationCounterparties;

            errorMessage = null;

            try
            {
                listWithCommunicationCounterparties = ReturnListWithCommunicationCounterparties(fileNameFullPathToConfigFile, out errorMessage);

                if (errorMessage != null)
                    return null;
            }
            catch (Exception e)
            {
                errorMessage = string.Format("ERROR!! An Exception occured in method ReturnFileNameFullPathForCounterparty! e.Message:\r\n{0}", e.Message);
                return null;
            }

            return listWithCommunicationCounterparties[id - 1].FileNameFullPath;
        }
    }
}