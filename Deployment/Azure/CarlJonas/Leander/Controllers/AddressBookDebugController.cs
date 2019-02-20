using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;
using CarlJonas;

namespace AddressBook
{
    public class DebugInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CreatedDate { get; set; }
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
        public bool IsCloseFriend { get; set; }

        public DebugInfo() { }
        public DebugInfo(int id, string name, string createdDate, int personId, string firstName, string lastName, string gender, string dateOfBirth, string phone, string address, string postCode, string town, string country, bool isCloseFriend)
        {
            this.Id = id;
            this.Name = name;
            this.CreatedDate = createdDate;
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
    }


    public class AddressBookDebugController : Controller
    {
        public const int _applicationId = 1;

        public ActionResult dfFdfnifElowi0690776A46344E2A87DD7560B0D14F1Aninohug()
        {
            List<DebugInfo> list;

            try
            {
                ViewBag.ErrorMessage = "";

                list = new List<DebugInfo>();

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbAddressBook"].ConnectionString))
                {
                    SqlCommand com = new SqlCommand("SELECT u.Id As UserId, u.[Name], u.CreatedDate, peri.PersonId, peri.FirstName, peri.LastName, peri.Gender, peri.DateOfBirth, peri.Phone, peri.[Address], peri.Town, peri.PostCode, peri.Country, peri.IsCloseFriend FROM [User] u LEFT OUTER JOIN PersonInfo peri ON u.Id = peri.UserId ORDER BY u.Id, peri.PersonId", conn);

                    conn.Open();
                    SqlDataReader reader = com.ExecuteReader();

                    while (reader.Read())
                    {
                        if (reader["PersonId"] != DBNull.Value)
                            list.Add(new DebugInfo(int.Parse(reader["UserId"].ToString()), reader["Name"].ToString(), ((DateTime)reader["CreatedDate"]).ToString("yyyy-MM-dd"), int.Parse(reader["PersonId"].ToString()), reader["FirstName"].ToString(), reader["LastName"].ToString(), reader["Gender"].ToString(), reader["DateOfBirth"].ToString(), reader["Phone"].ToString(), reader["Address"].ToString(), reader["Town"].ToString(), reader["PostCode"].ToString(), reader["Country"].ToString(), bool.Parse(reader["IsCloseFriend"].ToString())));
                        else
                            list.Add(new DebugInfo(int.Parse(reader["UserId"].ToString()), reader["Name"].ToString(), ((DateTime)reader["CreatedDate"]).ToString("yyyy-MM-dd"), 0, "", "", "", "", "", "", "", "", "", false));
                    }
                }
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Format("Error! An exception happened: {0}", e.Message);
                return View(new List<DebugInfo>());
            }

            return View(list);
        }

        public ContentResult Yu64VcdaDFCC346859F4844206AF10B89E5D92t803Dkk45bkiss(string userName, string password)
        {
            string errorMessage;

            Security.ChangePassword(userName, new ChangePassword(null, password), false, out errorMessage);

            if (errorMessage == null)
                return Content("The password was successfully changed");
            else
                return Content(errorMessage);
        }

        public ActionResult Ujf56hg462A903895757B4D52B5AE355B77458A37jdij99dkddd()
        {
            List<RequestToApplication> list;

            try
            {
                ViewBag.ErrorMessage = "";

                list = new List<RequestToApplication>();

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbAddressBook"].ConnectionString))
                {
                    SqlCommand com = new SqlCommand(string.Format("SELECT DateTimeUTC, RequestHeader FROM RequestToApplication WHERE ApplicationId = {0} ORDER BY Id desc", _applicationId.ToString()), conn);

                    conn.Open();
                    SqlDataReader reader = com.ExecuteReader();

                    while (reader.Read())
                    {
                        list.Add(new RequestToApplication(((DateTime)reader["DateTimeUTC"]).ToString("yyyy-MM-dd HH:mm:ss"), MvcHtmlString.Create(reader["RequestHeader"].ToString())));
                    }
                }
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Format("Error! An exception happened: {0}", e.Message);
                return View(new List<RequestToApplication>());
            }

            return View(list);
        }
    }
}