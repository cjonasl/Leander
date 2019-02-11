using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class User
    {
        public string Name { get; set; }
        public string Password { get; set; }

        public User() { }

        public User(string name, string password)
        {
            this.Name = name;
            this.Password = password;
        }
    }
}