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
        public int Flag { get; set; } //0=Nothing needs to be done. 1=It is a diary text file and need to update Bytes in diary file and maybe also warning message

        public SaveFileTextData() { }
    }
}