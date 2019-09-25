using log4net;

namespace WebApplication2
{
    public class Log
    {
        public static ILog File = LogManager.GetLogger(typeof(MvcApplication));
    }
}