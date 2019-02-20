using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;
using CarlJonas;

namespace AddressBook.Controllers
{
    public class SudokuDebugController : Controller
    {
        public const int _applicationId = 2;

        public ActionResult Jkm78Hjjk6E84B7ACCF9E47418AB8B6D781D6C20Cmfewi9fvu7d()
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