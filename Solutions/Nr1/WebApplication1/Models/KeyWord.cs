using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Leander.Nr1;

namespace WebApplication1.Models
{
    public class KeyWord
    {
        public int? Id { get; set; }
        public string IdString { get; set; } //Needed when update a key word (to set id on the td)
        public string Created { get; set; } //In format: yyyy-MM-dd HH:mm:ss
        public string Phrase { get; set; }    
        public string Note { get; set; }

        public KeyWord() { }

        public KeyWord(int id, string created, string phrase, string note)
        {
            this.Id = id;
            this.IdString = string.Format("keyWord{0}", id.ToString());
            this.Created = created;
            this.Phrase = phrase;
            this.Note = note;
        }
    }

    public static class KeyWordUtility
    {
        private const string _fileNameFullPathKeyWords = "C:\\git_cjonasl\\Leander\\Design Leander\\KeyWords.txt";

        private static string SerializeKeyWord(KeyWord keyWord)
        {
            return string.Format("{0}\r\n\r\n----- New property -----\r\n\r\n{1}\r\n\r\n----- New property -----\r\n\r\n{2}\r\n\r\n----- New property -----\r\n\r\n{3}", keyWord.Id.Value.ToString(), keyWord.Created, keyWord.Phrase, keyWord.Note);
        }

        private static KeyWord DeserializeKeyWord(string keyWord)
        {
            string[] v;
            int id;
            string created, keyPhrase, note;

            v = keyWord.Split(new string[] { "\r\n\r\n----- New property -----\r\n\r\n" }, StringSplitOptions.None);

            id = int.Parse(v[0]);
            created = v[1];
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

            if (!string.IsNullOrEmpty(list[0].Trim()))
            {
                for (i = 0; i < list.Length; i++)
                {
                    listWithKeyWords.Add(DeserializeKeyWord(list[i]));
                }
            }

            return listWithKeyWords;
        }

       public static KeyWord AddKeyWord(KeyWord keyWord, out string errorMessage)
       {
            KeyWord newKeyWord = null;
            string keyWordSerialized, fileContents;
            List<KeyWord> listWithKeyWords;
            ArrayList arrayListWithPhrases;
            int i;

            try
            {
                errorMessage = null;

                listWithKeyWords = GetKeyWords();

                arrayListWithPhrases = new ArrayList();

                for (i = 0; i < listWithKeyWords.Count; i++)
                {
                    arrayListWithPhrases.Add(listWithKeyWords[i].Phrase.ToLower());
                }

                if (arrayListWithPhrases.IndexOf(keyWord.Phrase.ToLower()) >= 0)
                {
                    errorMessage = "The phrase exists already!";
                    return null;
                }

                newKeyWord = new KeyWord(1 + arrayListWithPhrases.Count, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), keyWord.Phrase, keyWord.Note);
                keyWordSerialized = SerializeKeyWord(newKeyWord);

                fileContents = Utility.ReturnFileContents(_fileNameFullPathKeyWords);

                if (string.IsNullOrEmpty(fileContents))
                    Utility.AppendToFile(_fileNameFullPathKeyWords, keyWordSerialized);
                else
                    Utility.AppendToFile(_fileNameFullPathKeyWords, string.Format("\r\n\r\n---------- New key word ----------\r\n\r\n{0}", keyWordSerialized));
            }
            catch(Exception e)
            {
                errorMessage = string.Format("An Exception occured in method AddKeyWord! e.Message:\r\n{0}", e.Message);
                return null;
            }

            return newKeyWord;
        }

        public static KeyWord EditKeyWord(KeyWord keyWord, out string errorMessage)
        {
            KeyWord newKeyWord = null;
            bool foundKeyWord = false;
            StringBuilder sb;
            List<KeyWord> listWithKeyWords;
            int i;

            try
            {
                errorMessage = null;
                listWithKeyWords = GetKeyWords();

                if (keyWord.Id == listWithKeyWords[0].Id)
                {
                    listWithKeyWords[0].Note = keyWord.Note;
                    foundKeyWord = true;
                    newKeyWord = listWithKeyWords[0];
                }

                sb = new StringBuilder(SerializeKeyWord(listWithKeyWords[0]));

                for (i = 1; i < listWithKeyWords.Count; i++)
                {
                    if (keyWord.Id == listWithKeyWords[i].Id)
                    {                     
                        listWithKeyWords[i].Note = keyWord.Note;
                        foundKeyWord = true;
                        newKeyWord = listWithKeyWords[i];
                    }

                    sb.Append(string.Format("\r\n\r\n---------- New key word ----------\r\n\r\n{0}", SerializeKeyWord(listWithKeyWords[i])));
                }

                if (!foundKeyWord)
                {
                    errorMessage = "Error! Can not find the key word!";
                }
                else
                {
                    Utility.CreateNewFile(_fileNameFullPathKeyWords, sb.ToString());
                }
            }
            catch (Exception e)
            {
                errorMessage = string.Format("An Exception occured in method UpdateKeyWord! e.Message:\r\n{0}", e.Message);
                return null;
            }

            return newKeyWord;
        }
    }
}