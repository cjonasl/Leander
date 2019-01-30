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
        public int PreviousResource { get; set; }
        public int CurrentResource { get; set; }
        public int NextResource { get; set; }

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
            this.PreviousResource = 0;
            this.CurrentResource = 0;
            this.NextResource = 0;
        }

        public DataDefaultLocation(DocumentReadyDataForNonDefaultLocation data) //Everything for DataDefaultLocation is in DocumentReadyDataForNonDefaultLocation except Text, Width and Height
        {
            this.Icon = data.Icon;
            this.Title = data.Title;

            this.Tab = new string[10];

            for (int i = 0; i < 10; i++)
            {
                this.Tab[i] = data.TabNames[i];
            }

            this.Text = "";
            this.Width = "250px";
            this.Height = "50px";
            this.PreviousResource = data.PreviousResource;
            this.CurrentResource = data.CurrentResource;
            this.NextResource = data.NextResource;
        }
    }
}