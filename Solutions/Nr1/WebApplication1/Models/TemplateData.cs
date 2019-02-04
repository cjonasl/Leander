using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class TemplateData
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string KeyWords { get; set; } //Comma separated list with KeyWord Id:s.
        public string Note { get; set; }
        public string CodeText { get; set; }

        public TemplateData() { }

        public TemplateData(int id, string title, string keyWords, string note, string codeText)
        {
            this.Id = id;
            this.Title = title;
            this.KeyWords = keyWords;
            this.Note = note;
            this.CodeText = codeText;
        }
    }
}