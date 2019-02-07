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
        public string MiddleInitials { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Town { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }

        public PersonInfo1() { }

        public PersonInfo1
            (
              int id,
              string firstName,
              string middleInitials,
              string lastName,
              string gender,
              string dateOfBirth,
              string email,
              string phone,
              string mobile,
              string addressLine1,
              string addressLine2,
              string town,
              string postCode,
              string country
            )
        {
            this.Id = id;
            this.FirstName = firstName;
            this.MiddleInitials = middleInitials;
            this.LastName = lastName;
            this.Gender = gender;
            this.DateOfBirth = dateOfBirth;
            this.Email = email;
            this.Phone = phone;
            this.Mobile = mobile;
            this.AddressLine1 = addressLine1;
            this.AddressLine2 = addressLine2;
            this.Town = town;
            this.PostCode = postCode;
            this.Country = country;
        }

        public PersonInfo1 (int id, string firstName, string lastName) : 
            this(id, firstName, null, lastName, null, null, null, null, null, null, null, null, null, null){}
    }
}