using System;
using System.Web;
using System.Web.Mvc;
using CAST.Infrastructure;
using CAST.Logging;

namespace CAST.Controllers
{
    /// <summary>
    /// Base class representing controllers working with the database
    /// Automatically opens connection on create and closes it on dispose
    /// </summary>
    public class DataController : Controller
    {
        /// <summary>
        /// Database access object
        /// </summary>
        public DataContext _dataContext;

        /// <summary>
        /// Initializes the controller
        /// </summary>
        public DataController()
        {
            if (_dataContext == null)
            {
                _dataContext = new DataContext();
                _dataContext.OpenConnection();
            }
        }        

        /// <summary>
        /// Frees resources used by the controller
        /// </summary>
        /// <param name="disposing">Whether it's called by user or framework code</param>
        protected override void Dispose(bool disposing)
        {
            _dataContext.Dispose();
            base.Dispose(disposing);
        }

        public void RemoveCookie(string name)
        {
            Request.Cookies.Remove(name);
            Response.Cookies.Remove(name);
            HttpCookie cookie = new HttpCookie(name) { Expires = new DateTime(1980, 1, 1) };
            Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// Database access object
        /// </summary>
        public DataContext Data { get { return _dataContext; } }

    }
}