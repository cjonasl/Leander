using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Command
    {
        public int Page { get; set; }
        public int Menu { get; set; }
        public int Sub1 { get; set; }
        public int Sub2 { get; set; }
        public int Tab { get; set; }
        public string Cmd { get; set; }
        public string Val { get; set; }

        public Command() { }
    }
}