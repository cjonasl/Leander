using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;
using Stimulsoft.Report.Web;
using ClientConnect.Services;
using ClientConnect.Configuration;

namespace ClientConnect.Controllers
{
    public class DashboardController : Controller
    {
        public ActionResult Index(string fileName)
        {
            AccountService accService = (AccountService)Ioc.Get<AccountService>();

            if (!accService.SessionInfo.IsSuperAdm)
            {
                return RedirectToAction("Index", "Home");
            }
            
            if (!string.IsNullOrEmpty(fileName))
            {
                StoreService storeService = (StoreService)Ioc.Get<StoreService>();
                string basePath = Server.MapPath("~/");
                string reportFolder = string.Format("{0}\\Content\\DashboardReports\\ClientId{1}", basePath, storeService.StoreId.ToString());
                string fileNameFullPath = string.Format("{0}\\{1}.mrt", reportFolder, fileName);
                System.Web.HttpContext.Current.Session["CurrentStimulsoftFileNameToDesign"] = fileNameFullPath;
            }
            else if (System.Web.HttpContext.Current.Session["CurrentStimulsoftFileNameToDesign"] != null)
            {
                System.Web.HttpContext.Current.Session.Remove("CurrentStimulsoftFileNameToDesign");
            }

            return View();
        }

        public ActionResult GetReport()
        {
            StiReport report = new StiReport();

            if (System.Web.HttpContext.Current.Session["CurrentStimulsoftFileNameToDesign"] != null)
            {
                report.Load(System.Web.HttpContext.Current.Session["CurrentStimulsoftFileNameToDesign"].ToString());
            }

            return StiMvcDesigner.GetReportResult(report);
        }

        public ActionResult OpenReport()
        {
            StiRequestParams requestParams = StiMvcDesigner.GetRequestParams();
            System.Web.HttpContext.Current.Session["CurrentStimulsoftFileNameToDesign"] = requestParams.Designer.FileName;
            return StiMvcDesigner.GetReportResult();
        }

        public ActionResult CreateReport()
        {
            if (System.Web.HttpContext.Current.Session["CurrentStimulsoftFileNameToDesign"] != null)
            {
                System.Web.HttpContext.Current.Session.Remove("CurrentStimulsoftFileNameToDesign");
            }

            return StiMvcDesigner.GetReportResult(new StiReport());
        }

        public ActionResult DesignerEvent()
        {
            return StiMvcDesigner.DesignerEventResult();
        }

        public ActionResult SaveReportAs()
        {
            string fileNameFullPath = "";
            
            try
            {
                StiReport report = StiMvcDesigner.GetReportObject();

                StiRequestParams requestParams = StiMvcDesigner.GetRequestParams();
                fileNameFullPath = requestParams.Designer.FileName;

                FileStream f = new FileStream(fileNameFullPath, FileMode.Create, FileAccess.Write);
                f.Close();
                report.Save(fileNameFullPath);
                System.Web.HttpContext.Current.Session["CurrentStimulsoftFileNameToDesign"] = fileNameFullPath;
                return StiMvcDesigner.SaveReportResult(report);
            }
            catch
            {
                return StiMvcDesigner.SaveReportResult(string.Format("Unable to save file \"{0}\"! Please enter a valid file name with FULL path.", fileNameFullPath));
            }
        }

        public ActionResult SaveReport()
        {
            try
            {
                StiReport report = StiMvcDesigner.GetReportObject();

                string fileName = System.Web.HttpContext.Current.Session["CurrentStimulsoftFileNameToDesign"] != null ? System.Web.HttpContext.Current.Session["CurrentStimulsoftFileNameToDesign"].ToString() : null;

                if (fileName != null)
                {
                    if (!System.IO.File.Exists(fileName))
                    {
                        throw new Exception(string.Format("Unable to save file \"{0}\"! The path to the file is not available. Please use Sava As.", fileName));
                    }

                    report.Save(fileName);
                }
                else
                {
                    StiRequestParams requestParams = StiMvcDesigner.GetRequestParams();
                    string fileNameFullPath = requestParams.Designer.FileName;

                    try
                    {
                        FileStream f = new FileStream(fileNameFullPath, FileMode.Create, FileAccess.Write);
                        f.Close();
                        report.Save(fileNameFullPath);
                        System.Web.HttpContext.Current.Session["CurrentStimulsoftFileNameToDesign"] = fileNameFullPath;
                    }
                    catch
                    {
                        throw new Exception(string.Format("Unable to save file \"{0}\"! Please enter a valid file name with FULL path.", fileNameFullPath));
                    }
                }
            }
            catch (Exception e)
            {
                System.Web.HttpContext.Current.Session.Remove("CurrentStimulsoftFileNameToDesign");
                return StiMvcDesigner.SaveReportResult(string.Format("An error happened: {0}", e.Message));
            }

            return StiMvcDesigner.SaveReportResult();
        }
    }
}
