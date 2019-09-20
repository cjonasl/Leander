using System;
using System.Web.Mvc;
using ClientConnect.Configuration;
using ClientConnect.Process;
using ClientConnect.Services;
using ClientConnect.ViewModels.JobStatuses;
using System.Collections.Generic;
using System.Linq;
using ClientConnect.Jobs;
using PagedList;
//using CAST.Roles;
using ClientConnect.Properties;
using System.IO;
using System.Web.UI;
using System.Web;
using System.Data;
using System.Linq.Dynamic;
namespace ClientConnect.Controllers
{
    /// <summary>
    /// Controller for job status
    ///// </summary>
    //  [AuthorizeUser(Role = UserRoles.Levels.Manager)]
    public class JobStatusController : Controller
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public JobStatusController()
        {
            //Service = (JobStatusesService) Ioc.Get<JobStatusesService>();
            accountService = (AccountService)Ioc.Get<AccountService>();
            jobStatusService = (JobStatusesService)Ioc.Get<JobStatusesService>();
            JobService = (JobService)Ioc.Get<JobService>();
        }

       // public JobStatusesService Service { get; set; }
        private AccountService accountService { get; set; }
        private JobStatusesService jobStatusService { get; set; } 
        private JobService JobService { get; set; }
        
        public ActionResult JobStatuses()
        {
            return View();
        }

        /// <summary>
        /// Shows jobs by the status as a part of process
        /// </summary>
        /// <returns>Jobs list</returns>
        public ActionResult JobList(int? month = 0, int? export = 0)
        {
            if (month.Value == 0)
                month = jobStatusService.Months;
            else

                jobStatusService.Months = month.Value;
            var model = new JobStatus_SearchModel();
            model.Title = jobStatusService.Title;
            //model.JobStatusCount = _jobStatus.GetSummaryJobsStatuses();
            //if (export == 2)
            //{
            //    model.JobStatusSearchResults = jobStatusService.GetUnExportedRejectedJobList(jobStatusService.Months);

            //    if (model.JobStatusSearchResults.Count > 0)
            //    {
            //        foreach (JobStatus_Search JS in model.JobStatusSearchResults)
            //        {
            //            jobStatusService.SetJobExported(JS.ServiceId, DateTime.Now, "1");
            //        }
            //     //   jobStatusService.SetJobExported()
            //    }
            //}
            //else
            //{
            model.JobStatusSearchResults = jobStatusService.GetJobsListByAdminStatisticid(jobStatusService.StatusId, jobStatusService.Months);
                //model.JobStatusSearchResults = jobStatusService.GetRejectedJobList(jobStatusService.Months);
            //}
            model.AdminJobStatisticList = jobStatusService.AdminJobsStatistics(jobStatusService.Months);
           
            model.JobDurationList = jobStatusService.GetJobSearchDurationList(jobStatusService.Months);
            model.JobDuration = jobStatusService.Months;
            model.StatusId = jobStatusService.StatusId;

            //if (export == 1 || export == 2)
            //{
            //    Export exportt = new Export();
            //    exportt.JobListToExcel(Response, model.JobStatusSearchResults);
            //}
            ViewBag.AdminStatisticId= jobStatusService.StatusId;
            return View(model);
        }

        public ActionResult ExportJobList(int AdminStatisticId ,int? month = 0, int? export = 0)
        {
            bool UnexportedItemNoExists = false;
            if (month.Value == 0)
                month = jobStatusService.Months;
            else

                jobStatusService.Months = month.Value;
            var model = new JobStatus_SearchModel();
            model.Title = jobStatusService.Title;
            //model.JobStatusCount = _jobStatus.GetSummaryJobsStatuses();
            if (export == 2)
            {
                model.JobStatusSearchResults = jobStatusService.GetUnExportedRejectedJobList(jobStatusService.Months);

                if (model.JobStatusSearchResults.Count > 0)
                {
                    foreach (JobStatus_Search JS in model.JobStatusSearchResults)
                    {
                        jobStatusService.SetJobExported(JS.ServiceId, DateTime.Now, accountService.UserId);
                    }
                    //   jobStatusService.SetJobExported()
                }
            }
            else
            {
                model.JobStatusSearchResults = jobStatusService.GetJobsListByAdminStatisticid(jobStatusService.StatusId, jobStatusService.Months);
                //model.JobStatusSearchResults = jobStatusService.GetRejectedJobList(jobStatusService.Months);
                UnexportedItemNoExists = true;
            }
            //model.AdminJobStatisticList = jobStatusService.AdminJobsStatistics(jobStatusService.Months);

            model.JobDurationList = jobStatusService.GetJobSearchDurationList(jobStatusService.Months);
            model.JobDuration = jobStatusService.Months;
            model.StatusId = jobStatusService.StatusId;

            if (export == 1 || export == 2)
            {
                if (model.JobStatusSearchResults.Count==0)
                {
                  //  if No items are available newly to export 
                    model.JobStatusSearchResults = jobStatusService.GetJobsListByAdminStatisticid(jobStatusService.StatusId, jobStatusService.Months);
                      ViewBag.ExportRecordEmpty = "No records to download";
                }
                else{
                Export exportt = new Export();
                exportt.JobListToExcel(model.Title, Response, model.JobStatusSearchResults,export.Value);
                }
            }
            ViewBag.AdminStatisticId = jobStatusService.StatusId;
            model.AdminJobStatisticList = jobStatusService.AdminJobsStatistics(jobStatusService.Months);
            return View("JobList",   model);
        }

        //helper class
        public class Export
        {
            public void ToExcel(HttpResponseBase Response, object clientsList)
            {
                try
                {
                    var grid = new System.Web.UI.WebControls.GridView();
                    grid.DataSource = clientsList;
                    grid.DataBind();
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", "attachment; filename=FileName.xls");
                    Response.ContentType = "application/excel";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);

                    grid.RenderControl(htw);
                    Response.Write(sw.ToString());
                    Response.End();
                }
                catch (Exception ex)
                {

                }
            }

            public void JobListToExcel(string title, HttpResponseBase Response, List<JobStatus_Search> jobList,int export)
            {
                try
                {
                    var grid = new System.Web.UI.WebControls.GridView(); 
                    
                    bool hasCustColumn1 = jobList.Any(x => x.CustColumnname1 != null);
                    string CustColumn1name = hasCustColumn1 ? jobList.Select(x => x.CustColumnname1).FirstOrDefault().ToString() : string.Empty;
                    bool hasCustColumn2 = jobList.Any(x => x.CustColumnname2 != null);
                    string CustColumn2name = hasCustColumn2 ?  jobList.Select(x => x.CustColumnname2).FirstOrDefault():string.Empty;
                    bool hasCustColumn3 = jobList.Any(x => x.CustColumnname3 != null);
                    string CustColumn3name = hasCustColumn3?  jobList.Select(x => x.CustColumnname3).FirstOrDefault():string.Empty;
                    bool hasCustColumn4 = jobList.Any(x => x.CustColumnname4 != null);
                    string CustColumn4name = hasCustColumn4 ?  jobList.Select(x => x.CustColumnname4).FirstOrDefault():string.Empty;
                    // grid.AutoGenerateColumns = true;
                  //var result = BindData(title, jobList);

                    if (export == 1)
                    {
                        if (hasCustColumn1 && hasCustColumn2 && hasCustColumn3 && hasCustColumn4)
                        {
                            grid.DataSource =// result;  
                                from d in jobList
                                select new
                                {
                                    Repair_no = d.ServiceId,
                                    SD_Account_Number = d.ClientRef,
                                    Customer_Title = d.Title,
                                    Customer_Name = d.CustomerName,
                                    Customer_Address = d.FullAddress,
                                    Customer_PostCode = d.Postcode,
                                    Item_Number = d.AltCode,
                                    Item_Description = d.ItemDescr,
                                    CustColumn1name = d.CustColumnvalue1,
                                    CustColumn2name = d.CustColumnvalue2,
                                    CustColumn3name = d.CustColumnvalue3,
                                    CustColumn4name = d.CustColumnvalue4,
                                };
                        }
                        else if (hasCustColumn1 && hasCustColumn2 && hasCustColumn3)
                        {
                            grid.DataSource =// result;  
                                from d in jobList
                                select new
                                {
                                    Repair_no = d.ServiceId,
                                    SD_Account_Number = d.ClientRef,
                                    Customer_Title = d.Title,
                                    Customer_Name = d.CustomerName,
                                    Customer_Address = d.FullAddress,
                                    Customer_PostCode = d.Postcode,
                                    Item_Number = d.AltCode,
                                    Item_Description = d.ItemDescr,
                                    CustColumn1name = d.CustColumnvalue1,
                                    CustColumn2name = d.CustColumnvalue2,
                                    CustColumn3name = d.CustColumnvalue3,
                                };
                        }
                        else if (hasCustColumn1 && hasCustColumn2)
                        {
                            grid.DataSource =// result;  
                                from d in jobList
                                select new
                                {
                                    Repair_no = d.ServiceId,
                                    SD_Account_Number = d.ClientRef,
                                    Customer_Title = d.Title,
                                    Customer_Name = d.CustomerName,
                                    Customer_Address = d.FullAddress,
                                    Customer_PostCode = d.Postcode,
                                    Item_Number = d.AltCode,
                                    Item_Description = d.ItemDescr,
                                    CustColumn1name = d.CustColumnvalue1,
                                    CustColumn2name = d.CustColumnvalue2,


                                };
                        }
                        else if (hasCustColumn1)
                        {
                            grid.DataSource =// result;  
                                from d in jobList
                                select new
                                {
                                    Repair_no = d.ServiceId,
                                    SD_Account_Number = d.ClientRef,
                                    Customer_Title = d.Title,
                                    Customer_Name = d.CustomerName,
                                    Customer_Address = d.FullAddress,
                                    Customer_PostCode = d.Postcode,
                                    Item_Number = d.AltCode,
                                    Item_Description = d.ItemDescr,
                                    CustColumn1name = d.CustColumnvalue1,


                                };
                        }
                        else
                            grid.DataSource =// result;  
                               from d in jobList
                               select new
                               {
                                   Repair_no = d.ServiceId,
                                   SD_Account_Number = d.ClientRef,
                                   Customer_Title = d.Title,
                                   Customer_Name = d.CustomerName,
                                   Customer_Address = d.FullAddress,
                                   Customer_PostCode = d.Postcode,
                                   Item_Number = d.AltCode,
                                   Item_Description = d.ItemDescr,

                                   //Rejection_Reason = d.RejectionNotes,
                                   //Work_Undertaken_Report = d.WorkDone
                               };
                    }
                    else
                    {
                        grid.DataSource =// result;  
                                 from d in jobList
                                 select new
                                 {
                                     Repair_no = d.ServiceId,
                                     SD_Account_Number = d.ClientRef,
                                     Customer_Title = d.Title,
                                     Customer_Name = d.CustomerName,
                                     Customer_Address = d.FullAddress,
                                     Customer_PostCode = d.Postcode,
                                     Item_Number = d.AltCode,
                                     Item_Description = d.ItemDescr,

                                     Rejection_Reason = d.RejectionNotes,
                                     Work_Undertaken_Report = d.WorkDone
                                 };
                    }
                    grid.DataBind();

                    if (hasCustColumn1 )
                        grid.HeaderRow.Cells[8].Text = CustColumn1name;
                    if (hasCustColumn2 )
                        grid.HeaderRow.Cells[9].Text = CustColumn2name;
                    if (hasCustColumn3 )
                        grid.HeaderRow.Cells[10].Text = CustColumn3name;
                    if (hasCustColumn4 )
                        grid.HeaderRow.Cells[11].Text = CustColumn4name;

                  
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", string.Format("attachment; filename={0}_{1}.xls", title, DateTime.Now.ToShortDateString()));
                    Response.ContentType = "application/excel";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);

                    grid.RenderControl(htw);
                    Response.Write(sw.ToString());
                    Response.End();
                }
                catch (Exception ex)
                {
                }
            }
            
            private object BindData(string title, List<JobStatus_Search> jobList)
            {

                

              // string selectStatement= "new (   Repair_no = d.ServiceId, "                   
              //            +" SD_Account_Number = d.ClientRef,            " 
              //            +" Customer_Title = d.Title,                   " 
              //            +" Customer_Name = d.CustomerName,             " 
              //            +" Customer_Address = d.FullAddress,           " 
              //            +" Customer_PostCode = d.Postcode,             " 
              //            +" Item_Number = d.AltCode,                    " 
              //            +" Item_Description = d.ItemDescr,        ";

              //  if(hasCustColumn1 )
              //  {
              //    selectStatement += CustColumn1name +"=d.CustColumnvalue1, ";
              //  }
              //   if (hasCustColumn2  )
              //  {
              //       selectStatement += CustColumn2name +"=d.CustColumnvalue2, ";
              //  }
              //   if(hasCustColumn3)
              //  {
              //          selectStatement += CustColumn3name + "=d.CustColumnvalue3, ";                   
              //  }
              //   if (hasCustColumn4  )
              //  {

              //      selectStatement += CustColumn4name + "=d.CustColumnvalue4, ";
              //  }
              //  selectStatement +=")";
              //  //else
              //  //{
              //  //    return from d in jobList
              //  //           select new
              //  //           {
              //  //               Repair_no = d.ServiceId,
              //  //               SD_Account_Number = d.ClientRef,
              //  //               Customer_Title = d.Title,
              //  //               Customer_Name = d.CustomerName,
              //  //               Customer_Address = d.FullAddress,
              //  //               Customer_PostCode = d.Postcode,
              //  //               Item_Number = d.AltCode,
              //  //               Item_Description = d.ItemDescr,

              //  //           };
              //  //}
              //return   jobList.AsQueryable().Select(selectStatement);  
                
                return from d in jobList
                       select new
                       {
                           Repair_no = d.ServiceId,
                           SD_Account_Number = d.ClientRef,
                           Customer_Title = d.Title,
                           Customer_Name = d.CustomerName,
                           Customer_Address = d.FullAddress,
                           Customer_PostCode = d.Postcode,
                           Item_Number = d.AltCode,
                           Item_Description = d.ItemDescr,
                           CustColumn1 =d.CustColumnname1,
                           CustColumnV1 = d.CustColumnvalue1,
                           CustColumn2 = d.CustColumnname2,
                           CustColumnV2 = d.CustColumnvalue2,
                           CustColumn3 = d.CustColumnname3,
                           CustColumnV3 = d.CustColumnvalue3,
                           CustColumn4 = d.CustColumnname4,
                           CustColumnV4 = d.CustColumnvalue4,

                       };

            }
        }
        public ActionResult JobStatusList(int? page)
        {
            var model = new Job_SearchModel();
            JobService.SessionInfo.PageNumber = page ?? 1;
           // var model = new JobStatus_SearchModel();
          //  model.Title = Service.Title;
            ViewBag.JobStatus = jobStatusService.GetSummaryJobsStatuses();
            string StatusKey = jobStatusService.StatusSearch;
            var list = jobStatusService.GetJobsListByKey(StatusKey, JobService.SessionInfo.PageNumber, Settings.Default.ProductSearchPageSize);
            if ((list != null) && (list.Count > 0))
            {
                model.CurrentPage = JobService.SessionInfo.PageNumber;
                model.SearchResults = list;
                model.FirstItemIndex = list[0].StartElem;
                model.TotalRecords = list[0].ElemCount;
                model.LastItemIndex = list[0].LastElem;
             
              
            }
            ViewBag.OnePageOfJobs = new StaticPagedList<Job_SearchResult>(model.SearchResults, JobService.SessionInfo.PageNumber, Settings.Default.ProductSearchPageSize, model.TotalRecords);
          
            ////model.JobStatusCount = _jobStatus.GetSummaryJobsStatuses();
           // model.JobStatusSearchResults = Service.GetJobsListByKey(StatusKey);
           // model. = Service.GetJobsListByKey(StatusKey);
            ////if (model.JobStatusSearchResults.Count > 0)
            ////{
            ////    model.ChangeStatusText = model.JobStatusSearchResults[0].ChangeStatusText;
            ////    model.ChangeStatusId = model.JobStatusSearchResults[0].ChangeStatusId;
            ////}
            //model.JobDurationList = Service.GetJobSearchDurationList(Service.Months);
            //model.JobDuration = Service.Months;
            //model.StatusId = Service.StatusId;
            return View(model);
        }
        /// <summary>
        /// Redirects to the process page of jobs by specified status
        /// </summary>
        /// <param name="title"> Title of job </param>
        /// <param name="status"> The status </param>
        /// <returns> View of jobs </returns>
        public ActionResult OpenJobList(string title, int? status)
        {
            jobStatusService.StatusId = status ?? 0;
            jobStatusService.Title = title;

            return Redirect(Url.Process(PredefinedProcess.JobsByStatus));
        }
        public ActionResult OpenJobLists(string title, string statusSearch)
        {
            jobStatusService.StatusSearch = statusSearch;
            jobStatusService.Title = title;

            return Redirect(Url.Process(PredefinedProcess.JobList));
        }
        /// <summary>
        /// Shows jobs by the status as a part of process
        /// </summary>
        /// <returns>Jobs list</returns>
        public ActionResult AdminJobsStatistics(int? months=1)
        {
            AdminJobStatus_SearchModel model = new AdminJobStatus_SearchModel();
            jobStatusService.Months = months.Value;
            //var model = new List<AdminJobStatistic_Model>();
            model.AdminJobStatisticmodel = jobStatusService.AdminJobsStatistics(months.Value);
            model.JobDurationList = jobStatusService.GetJobSearchDurationList(months.Value);
          //
          //
          model.JobDuration = months.Value;
         
            return View(model);
        }
         public ActionResult RepeatStatics(int? page = 1)
        {
            if (accountService.IsSuperAdmin)
            {
                var model = new JobStatus_SearchModel();
                model.Title = jobStatusService.Title;
                model.JobStatusCount = jobStatusService.GetSummaryJobsStatuses();
                model.JobStatusSearchResults = jobStatusService.GetJobsList();
            if (model.JobStatusSearchResults.Count > 0)
            {
                model.ChangeStatusText = model.JobStatusSearchResults[0].ChangeStatusText;
                model.ChangeStatusId = model.JobStatusSearchResults[0].ChangeStatusId;
            }
            model.StatusId = jobStatusService.StatusId;
            return View(model);
            }
            else
            {
                return RedirectToAction("AccessDenied", "Account");
            }
        }
        
        ///// <summary>
        ///// Change status of selected items
        ///// </summary>
        ///// <param name="ServiceId">id of the Job</param>
        ///// <param name="ChangeStatusId">current status</param>
        //public void ChangeJobsStatus(string ServiceId, string ChangeStatusId)
        //{
        //   if (!String.IsNullOrEmpty(ServiceId))
        //    {
        //        jobStatusService.ChangeStatus(ServiceId, ChangeStatusId);
        //    }
        //}
    }
}
