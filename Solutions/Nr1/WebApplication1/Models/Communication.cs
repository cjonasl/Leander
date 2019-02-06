using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Leander.Nr1;

namespace WebApplication1.Models
{
    public class Communication
    {
        public string MessageId { get; set; }
        public string Date { get; set; }
        public string Sender { get; set; }
        public string Message { get; set; }

        public Communication() { }
        public Communication(string messageId, string date, string sender, string message)
        {
            this.MessageId = messageId;
            this.Date = date;
            this.Sender = sender;
            this.Message = message;
        }
    }

    public static class CommunicationUtility
    {
        private static Communication DeserializeCommunication(string communication)
        {
            string[] u, v;

            u = communication.Split(new string[] { "-- New property --" }, StringSplitOptions.None);
            v = u[2].Split(new string[] { "-- New message --" }, StringSplitOptions.None);

            return new Communication(u[0], u[1], v[0], v[1]);
        }

        private static string SerializeCommunication(Communication communication)
        {
            return string.Format("{0}-- New property --{1}-- New property --{2}-- New message --{3}", communication.MessageId, communication.Date, communication.Sender, communication.Message);
        }

        private static int ReturnNextPrefixSequenceNumber(string messageId)
        {
            return 1 + int.Parse(messageId.Substring(1, 6));
        }

        private static string ReturnLastDate(string messageId)
        {
            return messageId.Substring(8, 6);
        }

        private static int ReturnLastSequenceNumber(string messageId)
        {
            return int.Parse(messageId.Substring(15, 3));
        }

        public static List<Communication> ReturnListWithCommunications(string fileNameFullPath, out string errorMessage)
        {
            List<Communication> listWithCommunication;
            string fileContents;
            string[] v;

            errorMessage = null;

            try
            {
                listWithCommunication = new List<Communication>();
                fileContents = Utility.ReturnFileContents(fileNameFullPath);
                fileContents = fileContents.Length > 12 ? fileContents.Substring(12) : "";

                if (!string.IsNullOrEmpty(fileContents))
                {
                    v = fileContents.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                    for (int i = 0; i < v.Length; i++)
                    {
                        listWithCommunication.Add(DeserializeCommunication(v[i]));
                    }
                }
            }
            catch (Exception e)
            {
                errorMessage = string.Format("ERROR!! An Exception occured in method ReturnListWithCommunications! e.Message:\r\n{0}", e.Message);
                return null;
            }

            return listWithCommunication;
        }

        public static Communication InsertNewCommunicationMessage(string fileNameFullPath, Communication communication, out string errorMessage)
        {
            string dimension, fileContents, date, messageId, serializedCommunication;
            int s1, s2;
            DateTime dateTimeNow;
            Communication newCommunication;

            errorMessage = null;

            try
            {
                dateTimeNow = DateTime.Now;

                fileContents = Utility.ReturnFileContents(fileNameFullPath);
                dimension = fileContents.Substring(0, 12);
                fileContents = fileContents.Length > 12 ? fileContents.Substring(12) : "";

                date = dateTimeNow.ToString("yyMMdd");

                if (string.IsNullOrEmpty(fileContents))
                {
                    s1 = 1;
                    s2 = 1;
                }
                else
                {
                    messageId = fileContents.Substring(0, 18);

                    s1 = ReturnNextPrefixSequenceNumber(messageId);

                    date = dateTimeNow.ToString("yyMMdd");

                    if (date != ReturnLastDate(messageId))
                        s2 = 1;
                    else
                        s2 = 1 + ReturnLastSequenceNumber(messageId);
                }

                newCommunication = new Communication(string.Format("N{0}D{1}L{2}", s1.ToString().PadLeft(6, '0'), date, s2.ToString().PadLeft(3, '0')), dateTimeNow.ToString("yyyy-MM-dd HH:mm:ss"), communication.Sender, communication.Message.Replace("\n", "-- New Row --"));
                serializedCommunication = SerializeCommunication(newCommunication);

                if (string.IsNullOrEmpty(fileContents))
                    Utility.CreateNewFile(fileNameFullPath, dimension + serializedCommunication);
                else
                    Utility.CreateNewFile(fileNameFullPath, dimension + serializedCommunication + "\r\n" + fileContents);
            }
            catch (Exception e)
            {
                errorMessage = string.Format("ERROR!! An Exception occured in method InsertNewCommunicationMessage! e.Message:\r\n{0}", e.Message);
                return null;
            }

            return newCommunication;
        }

        public static Communication ReturnCommunication(string fileNameFullPath, string messageId, out string start, out string end, out string errorMessage)
        {
            Communication deserializedCommunication;
            string fileContents, serializedCommunication;
            int index1, index2;

            errorMessage = null;
            start = null;
            end = null;

            try
            {
                if (!System.IO.File.Exists(fileNameFullPath))
                {
                    errorMessage = string.Format("ERROR!! The following file does not exisat as expected: {0}", fileNameFullPath);
                    return null;
                }

                fileContents = Utility.ReturnFileContents(fileNameFullPath);

                index1 = fileContents.IndexOf(messageId);

                if (index1 == -1)
                {
                    errorMessage = string.Format("ERROR!! Can not find messageId = \"{0}\" in file {1}", messageId, fileNameFullPath);
                    return null;
                }

                start = fileContents.Substring(0, index1);

                if (messageId.StartsWith("N000001D"))
                {
                    serializedCommunication = fileContents.Substring(index1);
                    end = "";
                }
                else
                {
                    index2 = fileContents.IndexOf("\r\n", index1);
                    serializedCommunication = fileContents.Substring(index1, index2 - index1);
                    end = fileContents.Substring(2 + index2);
                }

                deserializedCommunication = DeserializeCommunication(serializedCommunication);
            }
            catch (Exception e)
            {
                errorMessage = string.Format("ERROR!! An Exception occured in method ReturnCommunication! e.Message:\r\n{0}", e.Message);
                return null;
            }

            return deserializedCommunication;
        }

        public static Communication UpdateCommunicationMessage(string fileNameFullPath, Communication communication, out string errorMessage)
        {
            string start, end, serializedCommunication;
            Communication tmpCommunication;

            errorMessage = null;

            try
            {
                tmpCommunication = ReturnCommunication(fileNameFullPath, communication.MessageId, out start, out end, out errorMessage);

                if (errorMessage != null)
                    return null;

                tmpCommunication.Sender = communication.Sender;
                tmpCommunication.Message = communication.Message.Replace("\n", "-- New Row --");
                serializedCommunication = SerializeCommunication(tmpCommunication);

                if (!string.IsNullOrEmpty(end))
                    serializedCommunication += "\r\n";

                Utility.CreateNewFile(fileNameFullPath, start + serializedCommunication + end);
            }
            catch (Exception e)
            {
                errorMessage = string.Format("ERROR!! An Exception occured in method UpdateCommunicationMessage! e.Message:\r\n{0}", e.Message);
                return null;
            }

            return DeserializeCommunication(serializedCommunication);
        }
    }
}