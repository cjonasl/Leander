using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace WebApplication2
{
public class CarlJonasLeanderOutputFilterStream : System.IO.Stream
{
    private readonly System.IO.Stream InnerStream;
    private readonly System.IO.MemoryStream CopyStream;

    public CarlJonasLeanderOutputFilterStream(System.IO.Stream inner)
    {
        this.InnerStream = inner;
        this.CopyStream = new System.IO.MemoryStream();
    }

    public string ReadStream()
    {
        lock (this.InnerStream)
        {
            if (this.CopyStream.Length <= 0L ||
                !this.CopyStream.CanRead ||
                !this.CopyStream.CanSeek)
            {
                return System.String.Empty;
            }

            long pos = this.CopyStream.Position;
            this.CopyStream.Position = 0L;
            try
            {
                return new System.IO.StreamReader(this.CopyStream).ReadToEnd();
            }
            finally
            {
                try
                {
                    this.CopyStream.Position = pos;
                }
                catch { }
            }
        }
    }


    public override bool CanRead
    {
        get { return this.InnerStream.CanRead; }
    }

    public override bool CanSeek
    {
        get { return this.InnerStream.CanSeek; }
    }

    public override bool CanWrite
    {
        get { return this.InnerStream.CanWrite; }
    }

    public override void Flush()
    {
        this.InnerStream.Flush();
    }

    public override long Length
    {
        get { return this.InnerStream.Length; }
    }

    public override long Position
    {
        get { return this.InnerStream.Position; }
        set { this.CopyStream.Position = this.InnerStream.Position = value; }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return this.InnerStream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, System.IO.SeekOrigin origin)
    {
        this.CopyStream.Seek(offset, origin);
        return this.InnerStream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        this.CopyStream.SetLength(value);
        this.InnerStream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        this.CopyStream.Write(buffer, offset, count);
        this.InnerStream.Write(buffer, offset, count);
    }
}


public static class CarlJonasLeander
{
    private static void PrintFile(string fileContents)
    {
        System.IO.FileStream fileStream = null;

        try
        {
            fileStream = new System.IO.FileStream("C:\\LogRequestResponse\\Nr3Web2\\RequestResponse\\RR" + System.Guid.NewGuid().ToString().Replace("-", "") + ".txt", System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None);
        }
        catch
        {
            if (fileStream != null)
                fileStream.Close();

            return;
        }

        System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(fileStream, System.Text.Encoding.UTF8);
        streamWriter.Write(fileContents);

        streamWriter.Flush();
        fileStream.Flush();
        streamWriter.Close();
        fileStream.Close();
    }

    private static string ReturnStringArray(string[] v)
    {
        if (v == null)
        {
            return "null";
        }
        else if (v.Length == 0)
        {
            return "Empty";
        }
        else
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            for (int i = 0; i < v.Length; i++)
            {
                if (i == 0)
                {
                    sb.Append(ReturnString(v[i]));
                }
                else
                {
                    sb.Append(", " + ReturnString(v[i]));
                }
            }

            return sb.ToString();
        }
    }

    private static string ReturnNameValueCollection(System.Collections.Specialized.NameValueCollection v)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        System.Collections.ArrayList arrayList = new System.Collections.ArrayList();
        int i;
        string key, value;

        if (v == null)
        {
            return "null";
        }

        if (v.Count == 0)
        {
            return "Empty";
        }

        for (i = 0; i < v.Count; i++)
        {
            arrayList.Add(v.AllKeys[i]);
        }

        arrayList.Sort();

        for (i = 0; i < v.Count; i++)
        {
            key = (string)arrayList[i];
            value = v[key];
            sb.Append(string.Format("Key[{0}] Value[{1}]\r\n", ReturnString(key), ReturnString(value)));
        }

        return sb.ToString().TrimEnd();
    }

    private static string ReturnString(string str)
    {
        if (str == null)
        {
            return "Null";
        }
        else if (string.IsNullOrWhiteSpace(str))
        {
            return "Empty string";
        }
        else
        {
            return str.Replace("\r\n", " ");
        }
    }

    private static string ReturnrequestSummary(System.Web.HttpRequest request)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        if (request.Url != null)
        {
            sb.Append("Url: " + ReturnString(request.Url.AbsolutePath) + ", ");
        }
        else
        {
            sb.Append("Url: Null, ");
        }

        if (request.Params != null)
        {
            sb.Append("Params: " + request.Params.Count.ToString() + ", ");
        }
        else
        {
            sb.Append("Params: 0, ");
        }

        if (request.QueryString != null)
        {
            sb.Append("QueryString: " + request.QueryString.Count.ToString() + ", ");
        }
        else
        {
            sb.Append("QueryString: 0, ");
        }

        if (request.Form != null)
        {
            sb.Append("Form: " + request.Form.Count.ToString() + ", ");
        }
        else
        {
            sb.Append("Form: 0, ");
        }

        if (request.Headers != null)
        {
            sb.Append("Headers: " + request.Headers.Count.ToString() + ", ");
        }
        else
        {
            sb.Append("Headers: 0, ");
        }

        if (request.ServerVariables != null)
        {
            sb.Append("ServerVariables: " + request.ServerVariables.Count.ToString() + ", ");
        }
        else
        {
            sb.Append("ServerVariables: 0, ");
        }

        if (request.Cookies != null)
        {
            sb.Append("Cookies: " + request.Cookies.Count.ToString() + ", ");
        }
        else
        {
            sb.Append("Cookies: 0, ");
        }

        if (request.Files != null)
        {
            sb.Append("Files: " + request.Files.Count.ToString());
        }
        else
        {
            sb.Append("Files: 0 ");
        }

        return sb.ToString();
    }

    private static string ReturnBasicVariables(System.Web.HttpRequest request)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        sb.Append(string.Format("string ApplicationPath: {0}\r\n", ReturnString(request.ApplicationPath)));
        sb.Append(string.Format("string PhysicalApplicationPath: {0}\r\n", ReturnString(request.PhysicalApplicationPath)));
        sb.Append(string.Format("string UserAgent: {0}\r\n", ReturnString(request.UserAgent)));
        sb.Append(string.Format("string[] UserLanguages: {0}\r\n", ReturnStringArray(request.UserLanguages)));
        sb.Append(string.Format("string UserHostName: {0}\r\n", ReturnString(request.UserHostName)));
        sb.Append(string.Format("string UserHostAddress: {0}\r\n", ReturnString(request.UserHostAddress)));
        sb.Append(string.Format("string RawUrl: {0}\r\n", ReturnString(request.RawUrl)));
        sb.Append(string.Format("int TotalBytes: {0}\r\n", request.TotalBytes.ToString()));
        sb.Append(string.Format("string PhysicalPath: {0}\r\n", ReturnString(request.PhysicalPath)));
        sb.Append(string.Format("string PathInfo: {0}\r\n", ReturnString(request.PathInfo)));
        sb.Append(string.Format("string AppRelativeCurrentExecutionFilePath: {0}\r\n", ReturnString(request.AppRelativeCurrentExecutionFilePath)));
        sb.Append(string.Format("string CurrentExecutionFilePathExtension: {0}\r\n", ReturnString(request.CurrentExecutionFilePathExtension)));
        sb.Append(string.Format("bool IsLocal: {0}\r\n", request.IsLocal.ToString()));
        sb.Append(string.Format("string requestType: {0}\r\n", ReturnString(request.RequestType)));
        sb.Append(string.Format("string ContentType: {0}\r\n", ReturnString(request.ContentType)));
        sb.Append(string.Format("int ContentLength: {0}\r\n", request.ContentLength.ToString()));
        sb.Append(string.Format("string HttpMethod: {0}\r\n", ReturnString(request.HttpMethod)));
        sb.Append(string.Format("bool IsAuthenticated: {0}\r\n", request.IsAuthenticated.ToString()));
        sb.Append(string.Format("string[] AcceptTypes: {0}\r\n", ReturnStringArray(request.AcceptTypes)));
        sb.Append(string.Format("string CurrentExecutionFilePath: {0}\r\n", ReturnString(request.CurrentExecutionFilePath)));
        sb.Append(string.Format("string FilePath: {0}\r\n", ReturnString(request.FilePath)));
        sb.Append(string.Format("string Path: {0}\r\n", ReturnString(request.Path)));
        sb.Append(string.Format("bool IsSecureConnection: {0}\r\n", request.IsSecureConnection.ToString()));
        sb.Append(string.Format("string Path: {0}", ReturnString(request.AnonymousID)));

        return sb.ToString();
    }

    private static string ReturnHttpCookieCollection(System.Web.HttpCookieCollection v)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        System.Collections.ArrayList arrayList = new System.Collections.ArrayList();
        int i;
        string key, value;

        if (v == null)
        {
            return "null";
        }

        if (v.Count == 0)
        {
            return "Empty";
        }

        for (i = 0; i < v.Count; i++)
        {
            arrayList.Add(v.AllKeys[i]);
        }

        arrayList.Sort();

        for (i = 0; i < v.Count; i++)
        {
            key = (string)arrayList[i];
            value = v[key].Value;
            sb.Append(string.Format("Key[{0}] Value[{1}]\r\n", ReturnString(key), ReturnString(value)));
        }

        return sb.ToString().TrimEnd();
    }

    private static string ReturnHttpFileCollection(System.Web.HttpFileCollection v)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        System.Collections.ArrayList arrayList = new System.Collections.ArrayList();
        int i;
        string key, value;

        if (v == null)
        {
            return "null";
        }

        if (v.Count == 0)
        {
            return "Empty";
        }

        for (i = 0; i < v.Count; i++)
        {
            arrayList.Add(v.AllKeys[i]);
        }

        arrayList.Sort();

        for (i = 0; i < v.Count; i++)
        {
            key = (string)arrayList[i];
            value = v[key].FileName;
            sb.Append(string.Format("Key[{0}] Value[{1}]\r\n", ReturnString(key), ReturnString(value)));
        }

        return sb.ToString().TrimEnd();
    }

    public static void PrintRequestResponse(System.Web.HttpRequest request, string response)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        sb.Append(ReturnrequestSummary(request));
        sb.Append("\r\n\r\n");

        sb.Append("--------------------- Basic variables ---------------------");
        sb.Append("\r\n\r\n");
        sb.Append(ReturnBasicVariables(request));
        sb.Append("\r\n\r\n");

        sb.Append("--------------------- QueryString ---------------------");
        sb.Append("\r\n\r\n");
        sb.Append(ReturnNameValueCollection(request.QueryString));
        sb.Append("\r\n\r\n");

        sb.Append("--------------------- Form ---------------------");
        sb.Append("\r\n\r\n");
        sb.Append(ReturnNameValueCollection(request.Form));
        sb.Append("\r\n\r\n");

        sb.Append("--------------------- Headers ---------------------");
        sb.Append("\r\n\r\n");
        sb.Append(ReturnNameValueCollection(request.Headers));
        sb.Append("\r\n\r\n");

        sb.Append("--------------------- ServerVariables ---------------------");
        sb.Append("\r\n\r\n");
        sb.Append(ReturnNameValueCollection(request.ServerVariables));
        sb.Append("\r\n\r\n");

        sb.Append("--------------------- Cookies ---------------------");
        sb.Append("\r\n\r\n");
        sb.Append(ReturnHttpCookieCollection(request.Cookies));
        sb.Append("\r\n\r\n");

        sb.Append("--------------------- Files ---------------------");
        sb.Append("\r\n\r\n");
        sb.Append(ReturnHttpFileCollection(request.Files));
        sb.Append("\r\n\r\n");

        sb.Append("---------------------------------------------------- RESPONSE ----------------------------------------------------");
        sb.Append("\r\n\r\n");
        sb.Append(response);

        PrintFile(sb.ToString());
    }

    public static void ApplicationBeginRequest(System.Web.HttpResponse response, System.Web.HttpContext context)
    {
        CarlJonasLeanderOutputFilterStream filter = new CarlJonasLeanderOutputFilterStream(response.Filter);
        response.Filter = filter;
        context.Items["filter"] = filter;
    }

    public static void ApplicationEndRequest(System.Web.HttpRequest request, System.Web.HttpContext context)
    {
        CarlJonasLeanderOutputFilterStream filter = (CarlJonasLeanderOutputFilterStream)context.Items["filter"];
        string response = filter.ReadStream();
        PrintRequestResponse(request, response);
    }
}

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_BeginRequest()
        {
            CarlJonasLeander.ApplicationBeginRequest(HttpContext.Current.Response, this.Context);
        }


        protected void Application_EndRequest()
        {
            CarlJonasLeander.ApplicationEndRequest(HttpContext.Current.Request, this.Context);
        }


        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
