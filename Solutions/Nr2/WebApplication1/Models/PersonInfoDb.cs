using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;


namespace AddressBook
{
    public static class PersonInfoDb
    {
        public static List<PersonInfo> GetAll(int userId, out string errorMessage)
        {
            List<PersonInfo> list = null;
            errorMessage = null;

            try
            {
                list = new List<PersonInfo>();

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbAddressBook"].ConnectionString))
                {
                    SqlCommand com = new SqlCommand(string.Format("SELECT PersonId, FirstName, LastName FROM PersonInfo WHERE UserId = {0} ORDER BY FirstName, LastName", userId.ToString()), conn);

                    conn.Open();
                    SqlDataReader reader = com.ExecuteReader();

                    while (reader.Read())
                    {
                        list.Add(new PersonInfo(int.Parse(reader["Id"].ToString()), reader["FirstName"].ToString(), reader["LastName"].ToString()));
                    }
                }
            }
            catch (Exception e)
            {
                errorMessage = string.Format("Error! An exception happened: {0}", e.Message);
                return null;
            }

            return list;
        }

        public static PersonInfo GetSingle(int userId, int personId, out string errorMessage)
        {
            errorMessage = null;
            PersonInfo perdonInfo = null;

            try
            {
                string sqlQuery = string.Format("SELECT FirstName," +
                                          "LastName," +
                                          "Gender," +
                                          "DateOfBirth," +
                                          "Phone," +
                                          "[Address]," +
                                          "Town," +
                                          "PostCode," +
                                          "Country," +
                                          "IsCloseFriend " +
                                          "FROM PersonInfo WHERE UserId = {0} AND PersonId = {1}",
                                          userId.ToString(), personId.ToString());


                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbAddressBook"].ConnectionString))
                {
                    SqlCommand com = new SqlCommand(sqlQuery, conn);
                    conn.Open();
                    SqlDataReader reader = com.ExecuteReader();
                    reader.Read();
                    perdonInfo = new PersonInfo(personId,
                                                 reader["FirstName"].ToString(),
                                                 reader["LastName"].ToString(),
                                                 reader["Gender"].ToString(),
                                                 reader["DateOfBirth"].ToString(),
                                                 reader["Phone"].ToString(),
                                                 reader["Address"].ToString(),
                                                 reader["Town"].ToString(),
                                                 reader["PostCode"].ToString(),
                                                 reader["Country"].ToString(),
                                                 bool.Parse(reader["IsCloseFriend"].ToString()));
                }
            }
            catch (Exception e)
            {
                errorMessage = string.Format("Error! An exception happened: {0}", e.Message);
                return null;
            }

            return perdonInfo;
        }

        public static int Add(int userId, PersonInfo person, out string errorMessage)
        {
            errorMessage = null;
            int personId;

            try
            {
                string sqlQuery = string.Format("DECLARE @PersonId SELECT TOP 1 PersonId FROM PersonInfo WHERE UserId = {0} ORDER BY PersonId desc " +
                                          "INSERT INTO PersonInfo(UserId, PersonId, FirstName, LastName, Gender, DateOfBirth, Phone, [Address], Town, PostCode, Country, IsCloseFriend) " +
                                          "VALUES({0}, 1 + @PersonId, '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', {10}) SELECT TOP 1 PersonId FROM PersonInfo WHERE UserId = {0} ORDER BY PersonId desc",
                                          userId.ToString(),
                                          person.FirstName,
                                          person.LastName,
                                          person.Gender,
                                          person.DateOfBirth,
                                          string.IsNullOrEmpty(person.Phone) ? "" : person.Phone,
                                          string.IsNullOrEmpty(person.Address) ? "" : person.Address,
                                          string.IsNullOrEmpty(person.Town) ? "" : person.Town,
                                          string.IsNullOrEmpty(person.PostCode) ? "" : person.PostCode,
                                          person.Country,
                                          person.IsCloseFriend ? 1 : 0);

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbAddressBook"].ConnectionString))
                {
                    SqlCommand com = new SqlCommand(sqlQuery, conn);
                    conn.Open();
                    personId = (int)com.ExecuteScalar();
                }
            }
            catch (Exception e)
            {
                errorMessage = string.Format("Error! An exception happened: {0}", e.Message);
                return -1;
            }

            return personId;
        }

        public static void Update(int userId, PersonInfo person, out string errorMessage)
        {
            errorMessage = null;

            try
            {
                string sqlQuery = string.Format("UPDATE PersonInfo " +
                                                "SET FirstName = '{0}'," +
                                                     "LastName = '{1}'," +
                                                     "Gender = '{2}'," +
                                                     "DateOfBirth = '{3}'," +
                                                     "Phone = '{4}'," +
                                                     "Address = '{5}'," +
                                                     "Town = '{6}'," +
                                                     "PostCode = '{7}'," +
                                                     "Country = '{8}'," +
                                                     "IsCloseFriend = {9} WHERE UserId = {10} AND PersonId = {11}",
                                          person.FirstName,
                                          person.LastName,
                                          person.Gender,
                                          person.DateOfBirth,
                                          string.IsNullOrEmpty(person.Phone) ? "" : person.Phone,
                                          string.IsNullOrEmpty(person.Address) ? "" : person.Address,
                                          string.IsNullOrEmpty(person.Town) ? "" : person.Town,
                                          string.IsNullOrEmpty(person.PostCode) ? "" : person.PostCode,
                                          person.Country,
                                          person.IsCloseFriend ? 1 : 0,
                                          userId.ToString(),
                                          person.PersonId.ToString());

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbAddressBook"].ConnectionString))
                {
                    SqlCommand com = new SqlCommand(sqlQuery, conn);
                    conn.Open();
                    com.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                errorMessage = string.Format("Error! An exception happened: {0}", e.Message);
                return;
            }
        }

        public static void Delete(int userId, int personId, out string errorMessage)
        {
            errorMessage = null;

            try
            {
                string sqlQuery = string.Format("DELETE FROM PersonInfo WHERE UserId = {0} AND PersonId = {1}", userId.ToString(), personId.ToString());

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbAddressBook"].ConnectionString))
                {
                    SqlCommand com = new SqlCommand(sqlQuery, conn);
                    conn.Open();
                    com.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                errorMessage = string.Format("Error! An exception happened: {0}", e.Message);
                return;
            }
        }
    }
}