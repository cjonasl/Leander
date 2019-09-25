using System;
using System.Collections;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApplication2
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void Log(string str)
        {
            FileStream f = null;
            StreamWriter w = null;

            try
            {
                f = new FileStream("C:\\LogRequestResponse\\Nr2Web2\\Log.txt", FileMode.Append, FileAccess.Write);
                w = new StreamWriter(f, System.Text.Encoding.ASCII);
                w.WriteLine(str);
                w.Flush();
                f.Flush();
            }
            catch
            {

            }
            finally
            {
                if (w != null)
                    w.Close();

                if (f != null)
                    f.Close();
            }
        }

        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();
            Log("Application_Start");
            AreaRegistration.RegisterAllAreas();
            WebApplication1.FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            ArrayList names = new ArrayList();
            names.Add("Gustav");
            names.Add("Jonas");
            names.Add("Daniel");
            names.Add("Jimmy");
            names.Add("Ivan");

            ArrayList nameTaken = new ArrayList();
            nameTaken.Add(false);
            nameTaken.Add(false);
            nameTaken.Add(false);
            nameTaken.Add(false);
            nameTaken.Add(false);

            Application["names"] = names;
            Application["nameTaken"] = nameTaken;

            Context.Cache["SignedInUsers"] = new ArrayList();
            Context.Cache["CachedResponses"] = new ArrayList();
        }

        protected void Application_End()
        {
            Log("Application_End");
        }

        protected void Session_Start()
        {
            Log("Session_Start");
            ArrayList names = (ArrayList)Application["names"];
            ArrayList nameTaken = (ArrayList)Application["nameTaken"];
            bool b, foundFreeName = false;
            int i = 0;

            while (i < 5 && !foundFreeName)
            {
                b = (bool)nameTaken[i];

                if (!b)
                {
                    this.Session["name"] = (string)names[i];
                    nameTaken[i] = true;
                    foundFreeName = true;
                }
                else
                {
                    i++;
                }
            }
        }

        protected void Session_End()
        {
            Log("Session_End");

            if (this.Session["name"] != null)
            {
                ArrayList names = (ArrayList)Application["names"];
                ArrayList nameTaken = (ArrayList)Application["nameTaken"];
                string name = (string)this.Session["name"];
                bool foundName = false;
                int i = 0;

                while (i < 5 && !foundName)
                {
                    if (name == (string)names[i])
                    {
                        foundName = true;
                        nameTaken[i] = false;
                    }
                    else
                    {
                        i++;
                    }
                }
            }

            if (HttpContext.Current.User != null)
            {

                ArrayList v = (ArrayList)HttpContext.Current.Cache["SignedInUsers"];
                int index = v.IndexOf(HttpContext.Current.User.Identity.Name);

                if (index >= 0)
                {
                    v.RemoveAt(index);
                    v = (ArrayList)HttpContext.Current.Cache["CachedResponses"];
                    v.RemoveAt(index);
                }
            }
        }

        protected void Application_BeginRequest()
        {
            if (HttpContext.Current.User != null) //Always null
            {
                var aaa = HttpContext.Current.User.Identity.Name;
            }

            CarlJonasLeander.ApplicationBeginRequest(HttpContext.Current.Response, this.Context);
            Log("BeginRequest, URL = " + this.Request.Url.AbsolutePath + ", " + this.Request.HttpMethod);
        }

        protected void Application_EndRequest()
        {
            if (HttpContext.Current.User != null) //Never null
            {
                var aaa = HttpContext.Current.User.Identity.Name;
            }

            CarlJonasLeander.ApplicationEndRequest(HttpContext.Current.Request, this.Context);
            Log("EndRequest URL = " + this.Request.Url.AbsolutePath + ", " + this.Request.HttpMethod);

            if (HttpContext.Current.User.Identity.IsAuthenticated && this.Request.Url.AbsolutePath.IndexOf('.') == -1 && this.Request.Url.AbsolutePath.IndexOf("GoBackward") == -1 && this.Request.Url.AbsolutePath.IndexOf("GoForward") == -1 && this.Request.Url.AbsolutePath.IndexOf("Log") == -1)
            {
                CachedResponseWrapper cachedResponseWrapperObj = CachedResponseWrapper.GetCachedResponseWrapperObj();

                if (cachedResponseWrapperObj != null)
                {
                    CarlJonasLeanderOutputFilterStream filter = (CarlJonasLeanderOutputFilterStream)this.Context.Items["filter"];
                    string response = filter.ReadStream();
                    cachedResponseWrapperObj.CachNewResponse(response);
                    int returnCode = this.Response.StatusCode;
                }
            }
        }
    }

    public class CachedResponseWrapper
    {
        public ArrayList _v;
        public int _currentIndex;

        public CachedResponseWrapper()
        {
            _v = new ArrayList();
            _currentIndex = -1;
        }

        public bool BackButtonEnabled
        {
            get
            {
                return _currentIndex > 0;
            }
        }

        public bool ForwardButtonEnabled
        {
            get
            {
                return _currentIndex + 1 < _v.Count;
            }
        }

        public string GetNextResponse
        {
            get
            {
                if (_currentIndex < (_v.Count - 1))
                    _currentIndex++;

                return (string)_v[_currentIndex];
            }
        }

        public string GetPreviousResponse
        {
            get
            {
                if (_currentIndex > 0)
                    _currentIndex--;

                return (string)_v[_currentIndex];
            }
        }

        public void CachNewResponse(string response)
        {
            int n = _v.Count;
            for (int i = _currentIndex + 1; i < n; i++)
            {
                _v.RemoveAt(i);
            }

            if (_v.Count == 10)
            {
                _v.RemoveAt(0);
            }

            _v.Add(response);
            _currentIndex++;
        }

        public static CachedResponseWrapper GetCachedResponseWrapperObj()
        {
            ArrayList v = (ArrayList)HttpContext.Current.Cache["SignedInUsers"];
            int index = v.IndexOf(HttpContext.Current.User.Identity.Name);

            if (index >= 0)
            {
                ArrayList cachedResponses = (ArrayList)HttpContext.Current.Cache["CachedResponses"];
                return (CachedResponseWrapper)cachedResponses[index];
            }
            else
            {
                return null;
            }
        }

        public static string ReturnString(object obj)
        {
            string[] v = (string[])obj;
            return "Log in name: " + v[0] + ", Number of saved responses: " + v[1] + ", Current index: " + v[2];
        }
    }
}
