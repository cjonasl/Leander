using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class LoadResourceData
    {
        public Resource Resource { get; set; }
        public IdText[] ArrayWithKeyWords { get; set; }

        public LoadResourceData() { }
    }
}