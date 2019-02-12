using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AddressBook
{
    public class PersonInfo
    {
        public int PersonId { get; set; }
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

        public PersonInfo() { }

        public PersonInfo
            (
              int personId,
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
            this.PersonId = personId;
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

        public PersonInfo(int id, string firstName, string lastName) :
            this(id, firstName, lastName, null, null, null, null, null, null, null, false)
        { }
    }
}