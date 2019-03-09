using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class SaveFileTextData
    {
        public string Str { get; set; } //Id for resource or file name full path for a file
        public string Width { get; set; } //Need to be of type string since "px" is appened after the height
        public string Height { get; set; } //Need to be of type string since "px" is appened after the height
        public string Text { get; set; }

        public SaveFileTextData(){ }

        public SaveFileTextData(string str, string width, string height, string text)
        {
            this.Str = str;
            this.Width = width;
            this.Height = height;
            this.Text = text;
        }
    }
}