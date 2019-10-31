using System;
using System.IO;
using System.Web.Mvc;
using ClientConnect.Configuration;
using ClientConnect.Services;
using Stimulsoft.Report;
using Stimulsoft.Report.Dashboard;
using Stimulsoft.Report.Mvc;
using ClientConnect.Properties;

namespace ClientConnect.Controllers
{
    /// <summary>
    /// Report print controller
    /// </summary>

    public class ReportsController : Controller
    {

        /// <summary>
        /// Service
        /// </summary>
        private ReportsService Service { get; set; }

        private ReportsService ReportService { get; set; }
        public ReportsController()
        {
            Service = (ReportsService) Ioc.Get<ReportsService>();
            ReportService = (ReportsService)Ioc.Get<ReportsService>();
        }
        

        /// <summary>
        /// Returns stimulsoft report as PDF
        /// </summary>
        /// <param name="report">Report to convert to PDF</param>
        /// <returns>Request with PDF file</returns>
        private ActionResult ConvertReportToPdf(StiReport report)
        {
            using (var stream = new MemoryStream())
            {
                // If report is not rendered, then render.
                if (!report.IsRendered)
                    report.Render(false);

                report.ExportDocument(StiExportFormat.Pdf, stream);

                // NOTE: do not use fileDownloadName parameter if planning to embed PDF inside HTML page, as Chrome will not be able to display it
                return File(stream.ToArray(), "application/pdf");
            }
        }

        [HttpGet]
        public ActionResult RepairInstructionReport(int serviceId, int modelId)
        {
            try
            {
                var report = ReportService.GetRepairInstruction(serviceId, modelId);
                return ConvertReportToPdf(report);
            }
            catch (Exception)
            {
                return ConvertReportToPdf(ReportService.GetErrorReport());
            }
        }

        /// <summary>
        /// Generate report 'Product details'
        /// </summary>
        /// <returns>Return created report</returns>
        [HttpGet]
        public ActionResult ProductDetailsReport(int modelId)
        {
            try
            {
                var report = ReportService.GetProductDetailsReport(modelId);
                return ConvertReportToPdf(report);
            }
            catch (Exception)
            {
                return ConvertReportToPdf(ReportService.GetErrorReport());
            }
        }

        /// <summary>
        /// Get report in pdf
        /// </summary>
        /// <returns>Stream of pdf file</returns>
        [HttpGet]
        public ActionResult ReportByTemplate(int modelId, int serviceId, string template, string groupName)
        {
            try
            {
                var report = Service.GetReportByTemplate(modelId, serviceId, template, groupName);
                return ConvertReportToPdf(report);
            }
            catch (Exception)
            {
                return ConvertReportToPdf(Service.GetErrorReport());
            }
        }

        public ActionResult Dashboards(string fileName)
        {
            AccountService accService = (AccountService)Ioc.Get<AccountService>();

            if (!accService.SessionInfo.IsAdm && !accService.SessionInfo.IsSuperAdm)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.IsSuperAdm = accService.SessionInfo.IsSuperAdm;
            
            StoreService storeService = (StoreService)Ioc.Get<StoreService>();
            string[] fileNameWithoutExtension = null;
            string basePath = Server.MapPath("~/");
            string reportFolder = string.Format("{0}\\Content\\DashboardReports\\ClientId{1}", basePath, storeService.StoreId.ToString());

            if (Directory.Exists(reportFolder))
            {
                string[] files = Directory.GetFiles(reportFolder, "*.mrt");

                if (files.Length > 0)
                {
                    fileNameWithoutExtension = new string[files.Length];
                    int n = 0;

                    foreach (string str in files)
                    {
                        fileNameWithoutExtension[n++] = Path.GetFileNameWithoutExtension(str);
                    }

                    ViewBag.FileNames = fileNameWithoutExtension;
                }

                if (!string.IsNullOrEmpty(fileName))
                {
                    ViewBag.CurrentFileName = fileName;
                    System.Web.HttpContext.Current.Session["CurrentStimulsoftFileNameToView"] = fileName;
                }
                else if (files.Length > 0)
                {
                    ViewBag.CurrentFileName = fileNameWithoutExtension[0];
                    System.Web.HttpContext.Current.Session["CurrentStimulsoftFileNameToView"] = fileNameWithoutExtension[0];
                }
            }

            return View();
        }

        public ActionResult Design()
        {
            string file = System.Web.HttpContext.Current.Session["CurrentStimulsoftFileNameToView"].ToString();
            System.Web.HttpContext.Current.Session.Remove("CurrentStimulsoftFileNameToView");
            return RedirectToAction("Index", "Dashboard", new { fileName = file });
        }

        public ActionResult GetReport(string fileName)
        {
            StoreService storeService = (StoreService)Ioc.Get<StoreService>();
            string file, basePath = Server.MapPath("~/");
            string reportFolder = string.Format("{0}\\Content\\DashboardReports\\ClientId{1}", basePath, storeService.StoreId.ToString());

            StiReport report = new StiReport();

            if (!string.IsNullOrEmpty(fileName))
            {
                file = string.Format("{0}\\{1}.mrt", reportFolder, fileName);
            }
            else if (Directory.Exists(reportFolder))
            {
                string[] files = Directory.GetFiles(reportFolder, "*.mrt");
                file = files.Length == 0 ? null : files[0];
            }
            else
            {
                file = null;
            }

            if (file != null)
            {
                report.Load(file);
                StiCacheCleaner.Clean();
                report.Dictionary.Synchronize();
                report.Dictionary.SynchronizeBusinessObjects();
            }

            return StiMvcViewer.GetReportResult(report);
        }

        public ActionResult ViewerEvent()
        {
            return StiMvcViewer.ViewerEventResult();
        }
    }
}
