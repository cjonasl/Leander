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

        public static List<PersonInfo1> GetPersonInfos()
        {

            List<PersonInfo1> list = new List<PersonInfo1>();
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand com = new SqlCommand("SELECT Id, FirstName, LastName FROM PersonInfo1 ORDER BY FirstName, LastName", conn);

            conn.Open();
            SqlDataReader reader = com.ExecuteReader();

            while(reader.Read())
            {
                list.Add(new PersonInfo1(int.Parse(reader["Id"].ToString()), reader["FirstName"].ToString(), reader["LastName"].ToString()));
            }
            conn.Close();

            return list;
        }
    }
}