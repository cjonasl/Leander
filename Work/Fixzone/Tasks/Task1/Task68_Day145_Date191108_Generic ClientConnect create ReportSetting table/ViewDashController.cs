using ClientConnect.Configuration;
using ClientConnect.Repositories;
using ClientConnect.Services;
using Stimulsoft.Report;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Mvc;
using Stimulsoft.Report.Web;
using System;
using System.Data;
using System.IO;
using System.Web.Mvc;

using System.Linq;

namespace ClientConnect.Controllers
{
    public class ViewDashController : Controller
    {
        string Report = "CSV";
        public ActionResult Index()
        {
            string basePath = Server.MapPath("~/");
            string[] filenames = Directory.GetFiles(string.Format("{0}\\Content\\DashboardReports", basePath), "*.mrt",
                                         SearchOption.TopDirectoryOnly).Select(f => Path.GetFileNameWithoutExtension(f)).ToArray();
            //string[] Reports = new string[5] { "WIP", "SimpleList", "WebsiteAnalytics", "Element 4", "Element 5" };
            return View(filenames);
        }

        public ActionResult GetReport(string fileName = "WIP")
        {
            Report = fileName;
             Repository repository = new Repository();
            var report = new StiReport();
            StoreService storeService = (StoreService)Ioc.Get<StoreService>();
            string basePath = Server.MapPath("~/");
            //string reportFolder = string.Format("{0}\\Content\\DashboardReports\\ClientId{1}", basePath, storeService.StoreId.ToString());
            string reportFolder = string.Format("{0}Content\\DashboardReports", basePath);
            string fileNameFullPath = string.Format("{0}\\{1}.mrt", reportFolder, fileName);
           // var path = Server.MapPath("~/Reports/WIP.mrt");
            report.Load(fileNameFullPath);
            try {
            var dbConnection = (StiSqlDatabase)report.Dictionary.Databases["Connection"];
            dbConnection.ConnectionString = repository.ConnectionString;

            report.Dictionary.Variables["Clientid"].ValueObject =  storeService.StoreId.ToString();
            report.Dictionary.Variables["BaseUrl"].ValueObject = Request.Url.Scheme + "://" + Request.Url.Authority; 
            }
            catch
            {
            }
            return StiMvcViewer.GetReportResult(report);
        }

        public ActionResult ViewerEvent()
        {
            return StiMvcViewer.ViewerEventResult();
        }

        public ActionResult PrintPdf()
        {
            StiReport report = this.GetReport();
            StiReportResponse.PrintAsPdf(report);
            return View();
        }

        public ActionResult PrintHtml()
        {
            StiReport report = this.GetReport();
            StiReportResponse.PrintAsHtml(report);
            return View();
        }

        public ActionResult ExportPdf()
        {
            StiReport report = this.GetReport();
            StiReportResponse.ResponseAsPdf(report);
            return View();
        }

        public ActionResult ExportHtml()
        {
            StiReport report = this.GetReport();
            StiReportResponse.ResponseAsHtml(report);
            return View();
        }

        public ActionResult ExportXls()
        {
            StiReport report = this.GetReport();
            StiReportResponse.ResponseAsXls(report);
            return View();
        }
        public ActionResult ExportCSV()
        {
            StiReport report = this.GetReport();
            StiReportResponse.ResponseAsCsv(report);
            return View();
        }
        private StiReport GetReport()
        {
            string basePath = Server.MapPath("~/");
            //string reportFolder = string.Format("{0}\\Content\\DashboardReports\\ClientId{1}", basePath, storeService.StoreId.ToString());
            string reportFolder = string.Format("{0}Content\\DashboardReports", basePath);
            string fileNameFullPath = string.Format("{0}\\{1}.mrt", reportFolder, Report);
            Repository repository = new Repository();
            var report = new StiReport();
            StoreService storeService = (StoreService)Ioc.Get<StoreService>();
            report.Load(fileNameFullPath);
            try
            {
                var dbConnection = (StiSqlDatabase)report.Dictionary.Databases["Connection"];
                dbConnection.ConnectionString = repository.ConnectionString;

                report.Dictionary.Variables["Clientid"].ValueObject = storeService.StoreId.ToString();
                report.Dictionary.Variables["BaseUrl"].ValueObject = Request.Url.Scheme + "://" + Request.Url.Authority;
            }
            catch
            {
            }
          //  report.RegData(data);

            return report;
        }
    }
}
