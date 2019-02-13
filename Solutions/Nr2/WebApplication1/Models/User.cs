using System;

namespace AddressBook
{
    public class User
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public DateTime? CreatedDate { get; set; }

        public User() { }

        public User(string name, string password, DateTime? createdDate)
        {
            this.Name = name;
            this.Password = password;
            this.CreatedDate = createdDate;
        }
    }
}