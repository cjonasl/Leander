using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class DataDefaultLocation
    {
        public string Icon { get; set; }
        public string Title { get; set; }
        public string[] Tab { get; set; }

        public string Text { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }

        public DataDefaultLocation()
        {
            this.Icon = "fa-anchor";
            this.Title = "Title";

            this.Tab = new string[10];

            for(int i = 1; i <= 10; i++)
            {
                this.Tab[i - 1] = string.Format("Tab{0}", i.ToString());
            }

            this.Text = "";
            this.Width = "250px";
            this.Height = "50px";
        }
    }
}