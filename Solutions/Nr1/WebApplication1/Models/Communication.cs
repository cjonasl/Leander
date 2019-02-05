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
            return string.Format("{0}-- New property --{1}-- New property --{2}-- New message --{3}", communication.MessageId, communication.Date, communication.Sender.ToString().ToLower(), communication.Message);
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

                v = fileContents.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                for(int i = 0; i < v.Length; i++)
                {
                    listWithCommunication.Add(DeserializeCommunication(v[i]));
                }
            }
            catch (Exception e)
            {
                errorMessage = string.Format("ERROR!! An Exception occured in method ReturnListWithCommunications! e.Message:\r\n{0}", e.Message);
                return null;
            }

            return listWithCommunication;
        }

        public static Communication InsertNewMessage(string fileNameFullPath, Communication communication, out string errorMessage)
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
                errorMessage = string.Format("ERROR!! An Exception occured in method InsertNewMessage! e.Message:\r\n{0}", e.Message);
                return null;
            }

            return newCommunication;
        }
    }
}