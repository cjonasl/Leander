using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using System.Web.Mvc;

namespace CarlJonas
{
    public class RequestToApplication
    {
        public string DateTimeUTC { get; set; }
        public MvcHtmlString RequestHeader { get; set; }

        public RequestToApplication() { }
        public RequestToApplication(string dateTimeUTC, MvcHtmlString requestHeader)
        {
            this.DateTimeUTC = dateTimeUTC;
            this.RequestHeader = requestHeader;
        }
    }

    public static class RequestToApplicationUtility
    {
        private static string ReturnKeyValuePairsInHeader(NameValueCollection nameValueCollection)
        {
            StringBuilder sb = new StringBuilder();

            string[] allKeys = nameValueCollection.AllKeys;

            for (int i = 0; i < (allKeys.Length - 1); i++)
            {
                sb.Append(string.Format("[{0}]&nbsp;[{1}]&nbsp;&nbsp;&nbsp;&nbsp;", allKeys[i], nameValueCollection[allKeys[i]]));
            }

            sb.Append(string.Format("[{0}]&nbsp;[{1}]", allKeys[allKeys.Length - 1], nameValueCollection[allKeys[allKeys.Length - 1]]));

            return sb.ToString();
        }

        public static void LogRequest(int applicationId, string prefix, NameValueCollection nameValueCollection)
        {
            try
            {
                string str = prefix + ReturnKeyValuePairsInHeader(nameValueCollection);

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbAddressBook"].ConnectionString))
                {
                    SqlCommand com = new SqlCommand(string.Format("INSERT INTO RequestToApplication(ApplicationId, DateTimeUTC, RequestHeader) VALUES({0}, getutcdate(), '{1}')", applicationId.ToString(), str), conn);
                    conn.Open();
                    com.ExecuteNonQuery();
                }
            }
            catch { }
        }
    }
}