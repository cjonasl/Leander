using System;
using System.IO;
using System.Web.Mvc;
using ClientConnect.Configuration;
using ClientConnect.Services;
using Stimulsoft.Report;

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
    }
}
