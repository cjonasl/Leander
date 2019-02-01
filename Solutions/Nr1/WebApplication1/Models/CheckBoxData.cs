using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class IdText
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public IdText() { }
        public IdText(int id, string text)
        {
            this.Id = id;
            this.Text = text;
        }
    }
}