using System.Web.Mvc;
using ClientConnect.Configuration;
using ClientConnect.Infrastructure;
using ClientConnect.Logging;
using ClientConnect.Services;
using ClientConnect.Process;

namespace ClientConnect.Controllers
{
    /// <summary>
    /// Controller for home screens of an app
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Represents _storeState
        /// </summary>
        private HomeService HomeService { get; set; }
        private ProcessService ProcessService { get; set; }
        private StoreService StoreService { get; set; }
        private JobStatusesService jobStatusService { get; set; }

        /// <summary>
        /// Initializes process controller
        /// </summary>
        public HomeController( )
        {
            HomeService = (HomeService)Ioc.Get<HomeService>();
            StoreService = (StoreService)Ioc.Get<StoreService>();
            //UserService = (UserService)Ioc.Get<UserService>();
            ProcessService = (ProcessService)Ioc.Get<ProcessService>();
            jobStatusService = (JobStatusesService)Ioc.Get<JobStatusesService>();
        }

        public ActionResult ClientHome(int ClientId)
        {
            var ClientList = StoreService.ClientList().Find(x => x.StoreId == ClientId);
            StoreService.StoreId = ClientId;
            StoreService.StoreName = ClientList.StoreName;
            return RedirectToAction("Index","Home");
        }

        /// <summary>
        /// Provides index page
        /// </summary>
        /// <returns>Index page view</returns>
        public ActionResult Index()
        {
            // Clear all info
            HomeService.ClearSessions();

            //Stop processes
            ProcessService.StopAll();

            // ask user to log in if no current store available
            //if (StoreService.IsStoreInfoExist && !StoreService.IsCallCenter)
            //{

                ViewBag.JobStatus = jobStatusService.GetSummaryJobsStatuses();
                ViewBag.FreeSparesEnquiry = HomeService.FreeSparesEnquiry;
            //}

            // Is call center

            ViewBag.IsCallCenter = StoreService.IsStoreInfoExist && !StoreService.IsCallCenter ;
            ViewBag.IsStoreSet = StoreService.IsStoreInfoExist;
            ViewBag.SuperAdmin = StoreService.IsSuperAdmin;
            //if (StoreService.CallcenterUser && StoreService.StoreId == 0)
            //    return RedirectToAction("Index", "CallCenter");
            //if (StoreService.StoreId == 0)
            //{
            //    return Redirect(Url.Process(PredefinedProcess.SignIn));
            //}

            if (!StoreService.CallcenterUser)
            {
                return Redirect(Url.Process(PredefinedProcess.CallcenterSignIn));
            }
            else
            {
                //return RedirectToAction("Index", "CallCenter");
            

            ViewBag.ClientBookingType = StoreService.GetStoreInfo(StoreService.StoreId).ClientBookingType;// StoreService.ClientBookingType;
            // Set flag for false
            //UserService.IsAutoSignIn = false;
           // ViewBag.IsUserAuthenticated = AuthInfo.IsAuthenticated();
            return View();}

        }

        /// <summary>
        /// Get userGuide.pdf
        /// </summary>
        /// <returns>Training Manual - Retail Connect.pdf</returns>
        public FileResult GetUserGuide()
        {
            var msgService = (MsgService)Ioc.Get<MsgService>();
            Log.File.Info(msgService.GenerateLogMsg("Downloading guide."));
            string fileName = @"~\Content\Documents\Retail Connect - user guide.pdf";
            string contentType = "application/pdf";
            string DownloadName = "Retail Connect - user guide.pdf";
            return File(fileName, contentType, DownloadName);
        }
    }
}