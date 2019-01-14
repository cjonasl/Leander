using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Leander.Nr1;

namespace WebApplication1.Models
{
    public class KeyWord
    {
        public int Id { get; set; }
        public DateTime? Created { get; set; }
        public string KeyPhrase { get; set; }    
        public string Note { get; set; }

        public KeyWord() { }

        public KeyWord(int id, DateTime created, string keyPhrase, string note)
        {
            this.Id = id;
            this.Created = created;
            this.KeyPhrase = keyPhrase;
            this.Note = note;
        }
    }

    public static class KeyWordUtility
    {
        private const string _fileNameFullPathKeyWords = "C:\\git_cjonasl\\Leander\\Design Leander\\KeyWords.txt";

        private static string SerializeKeyWord(KeyWord keyWord)
        {
            return string.Format("{0}\r\n\r\n----- New property -----\r\n\r\n{1}----- New property -----\r\n\r\n{2}", keyWord.Created.Value.ToString("yyyy-MM-dd HH:mm:ss"), keyWord.KeyPhrase, keyWord.Note);
        }

        private static KeyWord DeserializeKeyWord(string keyWord)
        {
            string[] v;
            int id, year, month, day, hour, minute, second;
            DateTime created;
            string keyPhrase, note;

            v = keyWord.Split(new string[] { "\r\n\r\n----- New property -----\r\n\r\n" }, StringSplitOptions.None);

            id = int.Parse(v[0]);

            year = int.Parse(v[1].Substring(0, 4));
            month = int.Parse(v[1].Substring(5, 2));
            day = int.Parse(v[1].Substring(8, 2));
            hour = int.Parse(v[1].Substring(11, 2));
            minute = int.Parse(v[1].Substring(14, 2));
            second = int.Parse(v[1].Substring(17, 2));

            created = new DateTime(year, month, day, hour, minute, second);

            keyPhrase = v[2];
            note = v[3];

            return new KeyWord(id, created, keyPhrase, note);
        }

        public static List<KeyWord> GetKeyWords()
        {
            string[] list;
            List<KeyWord> listWithKeyWords;
            int i;

            list = Utility.ReturnFileContents(_fileNameFullPathKeyWords).Split(new string[] { "\r\n\r\n---------- New key word ----------\r\n\r\n" }, StringSplitOptions.None);

            listWithKeyWords = new List<KeyWord>();

            for (i = 0; i < list.Length; i++)
            {
                listWithKeyWords.Add(DeserializeKeyWord(list[i]));
            }

            return listWithKeyWords;
        }

       public static void SaveNewKeyWord(string keyPhrase, string note)
        {
            int id;
            KeyWord keyWord;
            string keyWordSerialized, fileContents;

            fileContents = Utility.ReturnFileContents(_fileNameFullPathKeyWords);

            id = 1 + fileContents.Split(new string[] { "\r\n\r\n---------- New key word ----------\r\n\r\n" }, StringSplitOptions.None).Length;

            keyWord = new KeyWord(id, DateTime.Now, keyPhrase, note);
            keyWordSerialized = SerializeKeyWord(keyWord);

            if (string.IsNullOrEmpty(fileContents))
                Utility.AppendToFile(_fileNameFullPathKeyWords, keyWordSerialized);
            else
                Utility.AppendToFile(_fileNameFullPathKeyWords, string.Format("\r\n\r\n---------- New key word ----------\r\n\r\n{0}", keyWordSerialized));

        }
    }
}