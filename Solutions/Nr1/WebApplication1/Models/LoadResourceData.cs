using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class LoadResourceData
    {
        public Resource Resource { get; set; }
        public KeyWordShort[] ArrayWithKeyWordShort { get; set; }

        public LoadResourceData() { }
    }
}