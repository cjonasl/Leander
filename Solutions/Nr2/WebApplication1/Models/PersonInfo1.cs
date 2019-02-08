using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class PersonInfo1
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Town { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }
        public bool IsCloseFriend { get; set; } //Default is false

        public PersonInfo1() { }

        public PersonInfo1
            (
              int id,
              string firstName,
              string lastName,
              string gender,
              string dateOfBirth,
              string phone,
              string address,
              string town,
              string postCode,
              string country,
              bool isCloseFriend
            )
        {
            this.Id = id;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Gender = gender;
            this.DateOfBirth = dateOfBirth;
            this.Phone = phone;
            this.Address = address;
            this.Town = town;
            this.PostCode = postCode;
            this.Country = country;
            this.IsCloseFriend = isCloseFriend;
        }

        public PersonInfo1 (int id, string firstName, string lastName) : 
            this(id, firstName, lastName, null, null, null, null, null, null, null, false){}
    }
}