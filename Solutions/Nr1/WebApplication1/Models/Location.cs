using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Location
    {
        public int Page { get; set; }
        public int Menu { get; set; }
        public int Sub1 { get; set; }
        public int Sub2 { get; set; }
        public int Tab { get; set; }
        public bool? NewLocationByChangeOfTab { get; set; }

        public Location(int page, int menu, int sub1, int sub2, int tab)
        {
            this.Page = page;
            this.Menu = menu;
            this.Sub1 = sub1;
            this.Sub2 = sub2;
            this.Tab = tab;
            this.NewLocationByChangeOfTab = null;
        }

        public Location() { }
    }
}