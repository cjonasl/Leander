using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Web;

namespace WebApplication1.Models
{
    public static class PersonInfo1Db
    {
        private const string connectionString = "Data Source=LAPTOP-I366KH96;Initial Catalog=JONAS;Integrated Security=True";

        public static List<PersonInfo1> GetAll(out string errorMessage)
        {
            List<PersonInfo1> list = null;
            errorMessage = null;

            try
            {
                list = new List<PersonInfo1>();
                SqlConnection conn = new SqlConnection(connectionString);
                SqlCommand com = new SqlCommand("SELECT Id, FirstName, LastName FROM PersonInfo1 ORDER BY FirstName, LastName", conn);

                conn.Open();
                SqlDataReader reader = com.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new PersonInfo1(int.Parse(reader["Id"].ToString()), reader["FirstName"].ToString(), reader["LastName"].ToString()));
                }

                conn.Close();
            }
            catch(Exception e)
            {
                errorMessage = string.Format("Error! An exception happened: {0}", e.Message);
                return null;
            }

            return list;
        }

        public static PersonInfo1 GetSingle(int id, out string errorMessage)
        {
            errorMessage = null;
            PersonInfo1 perdonInfo = null;

            try
            {
                string sqlQuery = "SELECT FirstName," +
                                          "LastName," +
                                          "Gender," +
                                          "DateOfBirth," +
                                          "Phone," +
                                          "[Address]," +
                                          "Town," +
                                          "PostCode," +
                                          "Country," +
                                          "IsCloseFriend " +
                                          "FROM PersonInfo1 WHERE Id = " + id.ToString();

                SqlConnection conn = new SqlConnection(connectionString);
                SqlCommand com = new SqlCommand(sqlQuery, conn);
                conn.Open();
                SqlDataReader reader = com.ExecuteReader();
                reader.Read();
                perdonInfo = new PersonInfo1(id,
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
                conn.Close();
            }
            catch (Exception e)
            {
                errorMessage = string.Format("Error! An exception happened: {0}", e.Message);
                return null;
            }

            return perdonInfo;
        }

        public static int Add(PersonInfo1 person, out string errorMessage)
        {
            errorMessage = null;
            int id;

            try
            {
                string sqlQuery = string.Format("INSERT INTO PersonInfo1 (FirstName, LastName, Gender, DateOfBirth, Phone, [Address], Town, PostCode, Country, IsCloseFriend) " +
                                          "VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', {9}) SELECT TOP 1 Id FROM PersonInfo1 ORDER BY Id desc",
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



                SqlConnection conn = new SqlConnection(connectionString);
                SqlCommand com = new SqlCommand(sqlQuery, conn);
                conn.Open();
                id = (int)com.ExecuteScalar();
                conn.Close();
            }
            catch (Exception e)
            {
                errorMessage = string.Format("Error! An exception happened: {0}", e.Message);
                return -1;
            }

            return id;
        }

        public static void Update(PersonInfo1 person, out string errorMessage)
        {
            errorMessage = null;

            try
            {
                string sqlQuery = string.Format("UPDATE PersonInfo1 " +
                                                "SET FirstName = '{0}'," +
                                                     "LastName = '{1}'," +
                                                     "Gender = '{2}'," +
                                                     "DateOfBirth = '{3}'," +
                                                     "Phone = '{4}'," +
                                                     "Address = '{5}',"  +
                                                     "Town = '{6}'," +
                                                     "PostCode = '{7}'," +
                                                     "Country = '{8}'," +
                                                     "IsCloseFriend = {9} WHERE Id = {10}",
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
                                          person.Id);



                SqlConnection conn = new SqlConnection(connectionString);
                SqlCommand com = new SqlCommand(sqlQuery, conn);
                conn.Open();
                com.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception e)
            {
                errorMessage = string.Format("Error! An exception happened: {0}", e.Message);
                return;
            }
        }

        public static void Delete(int id, out string errorMessage)
        {
            errorMessage = null;

            try
            {
                string sqlQuery = "DELETE FROM PersonInfo1 WHERE Id = " + id.ToString();
                SqlConnection conn = new SqlConnection(connectionString);
                SqlCommand com = new SqlCommand(sqlQuery, conn);
                conn.Open();
                com.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception e)
            {
                errorMessage = string.Format("Error! An exception happened: {0}", e.Message);
                return;
            }
        }
    }
}