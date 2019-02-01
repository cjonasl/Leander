using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class KeyWordsText
    {
        public string KeyWords { get; set; } //Key word ids comma separated
        public string Text { get; set; }

        public KeyWordsText() { }
        public KeyWordsText(string keyWord, string text)
        {
            this.KeyWords = keyWord;
            this.Text = text;
        }
    }
}