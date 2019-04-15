using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Leander.Nr1;

namespace WebApplication1.Models
{
    public class KeyWord
    {
        public int? Id { get; set; }
        public string Created { get; set; } //In format: yyyy-MM-dd HH:mm:ss
        public string Phrase { get; set; }    
        public string Note { get; set; }

        public KeyWord() { }

        public KeyWord(int id, string created, string phrase, string note)
        {
            this.Id = id;
            this.Created = created;
            this.Phrase = phrase;
            this.Note = note;
        }
    }

    public class KeyWordShort
    {
        public int Id { get; set; }
        public string Phrase { get; set; }

        public KeyWordShort() { }

        public KeyWordShort(int id, string phrase)
        {
            this.Id = id;
            this.Phrase = phrase;
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

       public static KeyWord AddKeyWord(KeyWord keyWord, out string errorMessage) //keyWord not complete. newKeyWord will be complete
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
                errorMessage = string.Format("ERROR!! An Exception occured in method AddKeyWord! e.Message:\r\n{0}", e.Message);
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
                errorMessage = string.Format("ERROR!! An Exception occured in method UpdateKeyWord! e.Message:\r\n{0}", e.Message);
                return null;
            }

            return newKeyWord;
        }

        public static IdText[] ReturnArrayWithKeyWords(out string errorMessage)
        {
            List<KeyWord> listWithKeyWords;
            IdText[] idText;

            errorMessage = null;

            try
            {
                listWithKeyWords = GetKeyWords().OrderBy(x => x.Phrase).ToList();
                idText = new IdText[listWithKeyWords.Count];

                for (int i = 0; i < listWithKeyWords.Count; i++)
                {
                    idText[i] = new IdText(listWithKeyWords[i].Id.Value, listWithKeyWords[i].Phrase);
                }
            }
            catch (Exception e)
            {
                errorMessage = string.Format("ERROR!! An Exception occured in method ReturnArrayWithKeyWords! e.Message:\r\n{0}", e.Message);
                return null;
            }

            return idText;
        }

        public static string ReturnCommaSeparatedListWithKeyWords(string commaSeparatedListWithKeyWordId)
        {
            List<KeyWord> listWithKeyWords = GetKeyWords();
            ArrayList id, keyWord;
            StringBuilder sb;
            string[] v;
            int i, index1, index2;

            id = new ArrayList();
            keyWord = new ArrayList();

            for (i = 0; i < listWithKeyWords.Count; i++)
            {
                id.Add(listWithKeyWords[i].Id.Value);
                keyWord.Add(listWithKeyWords[i].Phrase);
            }

            v = commaSeparatedListWithKeyWordId.Split(',');
            sb = new StringBuilder();

            for (i = 0; i < v.Length; i++)
            {
                index1 = int.Parse(v[i]);
                index2 = id.IndexOf(index1);

                if (index2 == -1)
                {
                    throw new Exception(string.Format("ERROR!! Can not find KeyWord Id = {0} in method ReturnCommaSeparedListWithKeyWords!", index1.ToString()));
                }

                if (i == 0)
                {
                    sb.Append(keyWord[index2]);
                }
                else
                {
                    sb.Append(string.Format(",{0}", keyWord[index2]));
                }
            }

            return sb.ToString();
        }

        public static bool ResourceHasKeyWordId(string commaSeparatedListWithKeyWordId, int id)
        {
            string[] v;
            int i;
            bool returnValue = false; //Default

            v = commaSeparatedListWithKeyWordId.Split(',');

            i = 0;

            while (i < v.Length && !returnValue)
            {
                if (id == (int.Parse(v[i])))
                {
                    returnValue = true;
                }
                else
                {
                    i++;
                }
            }

            return returnValue;
        }

        public static string ReplaceWithKeyWordId(string[] kWords, out string errorMessage)
        {
            List<KeyWord> list;
            ArrayList keyWords, ids, v;
            int i, index;
            string keyWordIds = null;

            errorMessage = null;

            try
            {
                list = GetKeyWords();
                keyWords = new ArrayList();
                ids = new ArrayList();

                for (i = 0; i < list.Count; i++)
                {
                    keyWords.Add(list[i].Phrase);
                    ids.Add(list[i].Id.Value);
                }

                v = new ArrayList();

                for (i = 0; i < kWords.Length; i++)
                {
                    index = keyWords.IndexOf(kWords[i]);

                    if (index == -1)
                    {
                        errorMessage = string.Format("The key word \"{0}\" does not exist as expected", kWords[i]);
                        return null;
                    }

                    v.Add(ids[index]);
                }

                v.Sort();

                keyWordIds = Utility.ReturnItems(v, ",");
            }
            catch (Exception e)
            {
                errorMessage = string.Format("ERROR!! An Exception occured in method ReplaceWithKeyWordId! e.Message:\r\n{0}", e.Message);
                return null;
            }

            return keyWordIds;
        }
    }
}