
namespace AddressBook
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