using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class IconTitle
    {
        public string Icon { get; set; }
        public string Title { get; set; }

        public IconTitle()
        {
            this.Icon = "fa-anchor";
            this.Title = "Title";
        }
    }
}