using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class ModelDataPageWithKeyWords
    {
        public IconTitleTabs IconTitleTabs { get; set; }
        public List<KeyWord> listWithKeyWords { get; set; }

        public ModelDataPageWithKeyWords() { }
    }
}