using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class WidthHeightTextData
    {
        public string Str { get; set; } //Id for resource or file name full path for a file
        public string Width { get; set; } //Need to be of type string since "px" is appened after the height
        public string Height { get; set; } //Need to be of type string since "px" is appened after the height
        public string Text { get; set; }

        public WidthHeightTextData() { }
    }
}