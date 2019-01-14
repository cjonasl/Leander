using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class IconTitleTabs
    {
        public string Icon { get; set; }
        public string Title { get; set; }

        public string[] Tab { get; set; }

        public IconTitleTabs()
        {
            this.Icon = "fa-anchor";
            this.Title = "Title";

            this.Tab = new string[10];

            for (int i = 1; i <= 10; i++)
            {
                this.Tab[i - 1] = string.Format("Tab{0}", i.ToString());
            }
        }
    }
}