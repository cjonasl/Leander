using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class LocationExtension : Location
    {
        public string Width { get; set; }
        public string Height { get; set; }
        public string Text { get; set; }

        public LocationExtension() { }

        public LocationExtension(int page, int menu, int sub1, int sub2, int tab, string width, string height, string text)
        {
            this.Page = page;
            this.Menu = menu;
            this.Sub1 = sub1;
            this.Sub2 = sub2;
            this.Tab = tab;
            this.Width = width;
            this.Height = height;
            this.Text = text;
        }

    }
}