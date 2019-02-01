using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Image
    {
        public string Src { get; set; }
        public string Text { get; set; }

        public Image() { }
        public Image(string src, string text)
        {
            this.Src = src;
            this.Text = text;
        }
    }
}