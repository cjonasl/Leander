using System.Web.Mvc;
using ClientConnect.Configuration;
using ClientConnect.Infrastructure;
using ClientConnect.Jobs;
using ClientConnect.Logging;
using ClientConnect.Models.Job;
using ClientConnect.Process;
using ClientConnect.Products;
using ClientConnect.Properties;
using ClientConnect.Services;
using ClientConnect.ViewModels.Customer;
using ClientConnect.ViewModels.Job;
using FluentValidation;
using Omu.ValueInjecter;
using PagedList;
using System;
using System.Collections.Generic;
using ClientConnect.ViewModels.Status;
using Stimulsoft.Report;
using System.IO;
using System.Linq;
using System.Net.Mail;
using ClientConnect.Repositories;
using Newtonsoft.Json;
using ClientConnect.ViewModels.BookNewService;
using ClientConnect.Models.Store;
using ClientConnect.Roles;
using ClientConnect.Home;

namespace ClientConnect.Controllers
{
    [Authorize]
    /// <summary>
    /// Controller for jobs-related tasks
    /// </summary>
    public class JobController : Controller
    {
        private HomeService HomeService { get; set; }
        public StoreInfoModel storeinfo { get; set; }
        private JobService JobService { get; set; }
        private CustomerService CustomerService { get; set; }
        private ProductService ProductService { get; set; }
        private FieldsFromDBService FieldsFromDbService { get; set; }
        private ReportsService ReportsService { get; set; }
        private StoreService storeService { get; set; }
        private AccountService accountService { get; set; }
        private EmailService emailService { get; set; }
        private Service service { get; set; }
        private OnlineBookingService onlinebookingService { get; set; }
        private BookNewServiceService BookService { get; set; }
        public List<SpecialJob> specialJob;
        private readonly RuleSetKeys _ruleSets = new RuleSetKeys();
        List<JobCancelOption> list;
        public JobController()
        {
            JobService = (JobService)Ioc.Get<JobService>();
            CustomerService = (CustomerService)Ioc.Get<CustomerService>();
            ProductService = (ProductService)Ioc.Get<ProductService>();
            FieldsFromDbService = (FieldsFromDBService)Ioc.Get<FieldsFromDBService>();
            ReportsService = (ReportsService)Ioc.Get<ReportsService>();
            accountService = (AccountService)Ioc.Get<AccountService>();
            emailService = (EmailService)Ioc.Get<EmailService>();
            storeService = (StoreService)Ioc.Get<StoreService>();
            service = (Service)Ioc.Get<Service>();
            onlinebookingService = (OnlineBookingService)Ioc.Get<OnlineBookingService>();
            HomeService = (HomeService)Ioc.Get<HomeService>();
            storeinfo = storeService.GetStoreInfo(storeService.StoreId);
            BookService = (BookNewServiceService)Ioc.Get<BookNewServiceService>();
            specialJob = HomeService.GetSpecialJobMappingList(storeService.StoreId); 
          
        }

        /// <summary>
        /// Starts job search process
        /// </summary>
        /// <param name="jobSearchCriteria">Job search string</param>        
        /// <returns>Redirect to job search start page</returns>
        public ActionResult FindJob(string jobSearchCriteria)
        {
            JobService.SessionInfo.SearchCriteria = jobSearchCriteria;
            JobService.SessionInfo.PageNumber = 1;
            return Redirect(Url.Process(PredefinedProcess.JobSearch));
        }

        public ActionResult Marknotesread()
        {
            var model = JobService.MarkCustomerNotesRead(JobService.SessionInfo.ServiceId);
            return RedirectToAction("GoToDetails", new { id = JobService.SessionInfo.ServiceId });
        }


        /// <summary>
        /// Searches for jobs
        /// </summary>
        /// <param name="pageNum">Requested page number</param>
        /// <returns>View with search results</returns>
        public ActionResult Search(int? page)
        {
            if (page == null) page = JobService.SessionInfo.PageNumber;
            JobService.SessionInfo.PageNumber = page ?? 1;

            var model = JobService.GetJobsList(JobService.SessionInfo.SearchCriteria, JobService.SessionInfo.PageNumber);

            // If only one product finded, redirect to details
            if (model.TotalRecords == 1)
            {
                // Checking if user push back button on ShowDetails
                if (JobService.IsBackButtonPressed) return View(model);
                return RedirectToAction("GoToDetails", new { model.SearchResults[0].Id });
            }
            ViewBag.OnePageOfJobs = new StaticPagedList<Job_SearchResult>(model.SearchResults, JobService.SessionInfo.PageNumber, Settings.Default.JobSearchPageSize, model.TotalRecords);
            return View(model);
        }

        public ActionResult AwaitingForApproval(int? page = 1)
        {
            if (accountService.IsSuperAdmin)
            {
                if (page == null) page = JobService.SessionInfo.PageNumber;
                JobService.SessionInfo.PageNumber = page ?? 1;

                var model = JobService.AwaitingForApprovalList(JobService.SessionInfo.PageNumber);
                ViewBag.OnePageOfJobs = new StaticPagedList<Job_SearchResult>(model.SearchResults, JobService.SessionInfo.PageNumber, Settings.Default.ProductSearchPageSize, model.TotalRecords);
                return View(model);
            }
            else
            {
                return RedirectToAction("AccessDenied", "Account");
            }

        }

        [HttpPost]
        public ActionResult CancelJob(int SubStatusId, string notes)
        {
            string UpdateMessage = onlinebookingService.CancelJob(JobService.SessionInfo.ServiceId, notes, storeService.UserId, SubStatusId);
            if (UpdateMessage == string.Empty)
              //  return Redirect(Url.ProcessPreviousStep());
                return     Json(new { success = true,  formpath="Job/JobDetails"});
            else
            {
                ViewBag.Error = "Unfortunately we cannot cancel this appointment at this time.  You will need to go to Complete Service to complete this task.";

                
                    return    Json(new { success = false,  errorMessage = UpdateMessage});
    
            }
        }
        public ActionResult RejectJob(int Id, string RejectionReason, string notes)
        {
            string UpdateMessage = onlinebookingService.RejectJob(JobService.SessionInfo.ServiceId, notes, storeService.UserId, Id, RejectionReason);
            if (UpdateMessage == string.Empty)
                return Redirect(Url.ProcessPreviousStep());

            else
            {
                ViewBag.Error = UpdateMessage;

                if (JobService.SessionInfo.ServiceId > 0)
                {
                    var result = JobService.JobDetails(JobService.SessionInfo.ServiceId);
                    ViewBag.IsCallCenter = JobService.IsCallCenter;
                    ViewBag.AllPageOfNotes = result.JobNotes.ToPagedList(1, 20);
                    ViewBag.SuperAdmin = storeService.IsSuperAdmin;
                    result.StoreId = JobService.StoreId;
                    result.ReportList = ReportsService.GetReportList(JobService.SessionInfo.ServiceId);
                    return View("JobDetails", result);
                }

                else
                    return RedirectToAction("JobDetails");
            }


        }

        public ActionResult ApproveJobNotes(string notes)
        {
            string UpdateMessage = onlinebookingService.ApproveJob(JobService.SessionInfo.ServiceId, notes, storeService.UserId);
            if (UpdateMessage == string.Empty)
                return Redirect(Url.ProcessPreviousStep());

            else
            {
                ViewBag.Error = UpdateMessage;

                if (JobService.SessionInfo.ServiceId > 0)
                {
                    var result = JobService.JobDetails(JobService.SessionInfo.ServiceId);
                    ViewBag.IsCallCenter = JobService.IsCallCenter;
                    ViewBag.AllPageOfNotes = result.JobNotes.ToPagedList(1, 20);
                    ViewBag.SuperAdmin = storeService.IsSuperAdmin;
                    result.StoreId = JobService.StoreId;
                    result.ReportList = ReportsService.GetReportList(JobService.SessionInfo.ServiceId);
                    return View("JobDetails", result);
                }

                else
                    return RedirectToAction("JobDetails");
            }
        }

        public ActionResult HighCostQueryJob(string notes)
        {
            string UpdateMessage = onlinebookingService.QueryJob(JobService.SessionInfo.ServiceId, notes, storeService.UserId);
            if (UpdateMessage == string.Empty)
                return Redirect(Url.ProcessPreviousStep());

            else
            {
                ViewBag.Error = UpdateMessage;

                if (JobService.SessionInfo.ServiceId > 0)
                {
                    var result = JobService.JobDetails(JobService.SessionInfo.ServiceId);
                    ViewBag.IsCallCenter = JobService.IsCallCenter;
                    ViewBag.AllPageOfNotes = result.JobNotes.ToPagedList(1, 20);
                    ViewBag.SuperAdmin = storeService.IsSuperAdmin;
                    result.StoreId = JobService.StoreId;
                    result.ReportList = ReportsService.GetReportList(JobService.SessionInfo.ServiceId);
                    return View("JobDetails", result);
                }

                else
                    return RedirectToAction("JobDetails");
            }
        }
        public ActionResult RepeatedJobs(int? page = 1)
        {

            if (page == null) page = JobService.SessionInfo.PageNumber;
            JobService.SessionInfo.PageNumber = page ?? 1;

            var model = JobService.RepeatedJobs(JobService.SessionInfo.PageNumber, null);
            ViewBag.OnePageOfJobs = new StaticPagedList<Job_SearchResult>(model.SearchResults, JobService.SessionInfo.PageNumber, Settings.Default.ProductSearchPageSize, model.TotalRecords);
            return View(model);


        }
        public ActionResult RejectedJobs(int? page = 1)
        {

            if (page == null) page = JobService.SessionInfo.PageNumber;
            JobService.SessionInfo.PageNumber = page ?? 1;

            var model = JobService.RepeatedJobs(JobService.SessionInfo.PageNumber, (int)JobStatus.Rejected);
            ViewBag.OnePageOfJobs = new StaticPagedList<Job_SearchResult>(model.SearchResults, JobService.SessionInfo.PageNumber, Settings.Default.ProductSearchPageSize, model.TotalRecords);
            return View("RejectedJobs", model);


        }

        public ActionResult ApprovedJobs(int? page = 1)
        {

            if (page == null) page = JobService.SessionInfo.PageNumber;
            JobService.SessionInfo.PageNumber = page ?? 1;

            var model = JobService.ApprovedJobs(JobService.SessionInfo.PageNumber);
            ViewBag.OnePageOfJobs = new StaticPagedList<Job_SearchResult>(model.SearchResults, JobService.SessionInfo.PageNumber, Settings.Default.ProductSearchPageSize, model.TotalRecords);
            return View("ApprovedJobs", model);


        }
        /// <summary>
        /// Not booked jobs
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult FindNotBookedJobs()
        {
            JobService.SessionInfo.PageNumber = 1;
            return Redirect(Url.Process(PredefinedProcess.NotBookedJobList));

        }

        /// <summary>
        /// Search of not booked jobs
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult SearchNotBookedJobs(int? page)
        {
            if (page == null) page = JobService.SessionInfo.PageNumber;
            JobService.SessionInfo.PageNumber = page ?? 1;
            Job_SearchModel model = new Job_SearchModel();
            if (accountService.IsSuperAdmin)
            {
                model = JobService.AdminGetNotBookedJobList(JobService.SessionInfo.PageNumber);
            }
            else
                model = JobService.GetNotBookedJobList(JobService.SessionInfo.PageNumber);
            ViewBag.OnePageOfJobs = new StaticPagedList<Job_SearchResult>(model.SearchResults, JobService.SessionInfo.PageNumber, Settings.Default.ProductSearchPageSize, model.TotalRecords);
            return View(model);
        }
        /// <summary>
        /// Navigates to the job details process
        /// </summary>
        /// <param name="id">Job identifier</param>
        /// <returns>Redirect to job details result</returns>
        public ActionResult GoToDetails(int id)
        {
            JobService.SessionInfo.ServiceId = id;
            JobService.AddToDBLog_Viewed(id);
            //   return Redirect(Url.ProcessNextStep());
            return Redirect(Url.Process(PredefinedProcess.JobDetails));
        }

        [AllowAnonymous]
        public ActionResult Info(int id, string pc)
        {
           /*
             0: serviceId exists and match given postCode
             1: Error, serviceId does not exist
             2: Error, postCode does not match post code for serviceId
           */
            int indicator = JobService.CustomerViewJob(id, pc.Trim().Replace(" ", ""));

            if (indicator == 0)
            {
                var result = JobService.JobDetails(id);

                ViewBag.JobAccessMessage = "";
                ViewBag.Error = "";
                ViewBag.IsCallCenter = false;
                ViewBag.SuperAdmin = false;
                ViewBag.AllPageOfNotes = result.JobNotes.ToPagedList(1, 20);
                ViewBag.CustomerViewJob = true;

                return View("JobDetails", result);
            }
            else
            {
                if (indicator == 1)
                    ViewBag.ErrorMessage = "Error! The job does not exist!";
                else
                    ViewBag.ErrorMessage = "Error! The post code does not match!";

                return View("CustomerJobViewError");
            }
        }

        /// <summary>
        /// Returns job details view
        /// </summary>
        /// <returns>Job details view</returns>
        public ActionResult JobDetails(int? page, string Error = default(string))
        {
            // get job details
            if (JobService.SessionInfo.ServiceId > 0)
            {

                var JobSessionInfo = JobService.GetJobSessionInfo(JobService.SessionInfo.ServiceId, accountService.UserId);
                ViewBag.JobAccessMessage = "";
                if (JobSessionInfo.UserId != accountService.UserId)
                {
                    ViewBag.JobAccessMessage = String.Format("<span class='blink_me'>Please note that <b>'{0}'</b> is currently viewing the job.</span>", JobSessionInfo.UserName);
                }
                var result = JobService.JobDetails(JobService.SessionInfo.ServiceId);
                result.Status.IsCallCenter = Settings.Default.DeploymentTarget == "Clients" ? false : true;
                ViewBag.IsCallCenter = JobService.IsCallCenter;
                ViewBag.AllPageOfNotes = result.JobNotes.ToPagedList(page ?? 1, 20);
                ViewBag.SuperAdmin = storeService.IsSuperAdmin;
                result.StoreId = JobService.StoreId;
                result.ReportList = ReportsService.GetReportList(JobService.SessionInfo.ServiceId);
                result.TotalAmount = result.Parts.vanparts.AsEnumerable().Sum(x => (Math.Round(x.UnitPrice, 2) * x.Quantity)) +
                      result.Parts.stockPart.AsEnumerable().Sum(x => (Math.Round(x.UnitPrice, 2) * x.Quantity));
                // result.Media=
                ViewBag.Error = Error;
                ViewBag.ClientBookingType = storeService.ClientBookingType;
                return View(result);
            }
            return RedirectToAction("AdvSearch");
            //  return RedirectToAction("Search");
        }

        /// <summary>
        /// Adds note to the job
        /// </summary>
        /// <param name="noteText">Note text</param>
        /// <returns>Redirect page</returns>
        public ActionResult AddJobNote(string noteText)
        {
            if (!string.IsNullOrEmpty(noteText))
            {

                onlinebookingService.AddNote(JobService.SessionInfo.ServiceId, JobService.UserId, noteText);

            }
            return Redirect(Url.Process(PredefinedProcess.JobDetails));
        }

        /// <summary>
        /// Update customer info
        /// </summary>
        /// <returns>Redirect</returns>
        [HttpGet]
        public ActionResult CustomerInfoEdit()
        {
            var model = JobService.JobDetails(JobService.SessionInfo.ServiceId);
            return View(model.CustomerInformation);
        }

        /// <summary>
        /// Update customer info
        /// </summary>
        /// <returns>Redirect</returns>
        [HttpPost]
        public ActionResult CustomerInfoEdit(CustomerInfoModel model)
        {
            ViewBag.ResultSuccess = string.Empty;
            //VALIDATION
            var validator = new Validation.Job.CustomerValidation();
            var result = validator.Validate(model, ruleSet: _ruleSets.defaultRule);
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            //update Customer Information
            if (ModelState.IsValid)
            {
                var customer = new Customer_InfoModel();
                customer.InjectFrom(model);
                CustomerService.UpdateCustomerInfo(customer);
                JobService.AddToDBLog_Updated(JobService.SessionInfo.ServiceId);
                JobService.AddNote(JobService.SessionInfo.ServiceId, "Changed customer info", "", "");
                ViewBag.ResultSuccess = "Saved success.";
            }
            model.CountryList = CustomerService.GetCountryList(model.Country);
            return View(model);
        }

        /// <summary>
        /// Update customer info
        /// </summary>
        /// <returns>Redirect</returns>
        [HttpGet]
        public ActionResult ContactInfoEdit()
        {
            var model = JobService.JobDetails(JobService.SessionInfo.ServiceId);
            return View(model.ContactInformation);
        }

        /// <summary>
        /// Update customer info
        /// </summary>
        /// <returns>Redirect</returns>
        [HttpPost]
        public ActionResult ContactInfoEdit(ContactInfoModel model)
        {
            ViewBag.ResultSuccess = string.Empty;
            //VALIDATION
            var validator = new Validation.Job.ContactsValidation();
            foreach (var error in validator.Validate(model, ruleSet: _ruleSets.defaultRule).Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            //update Customer Information
            if (ModelState.IsValid)
            {
                // Update contact info
                var contact = new Contact_InfoModel();
                contact.InjectFrom(model);
                CustomerService.UpdateContactInfo(contact);
                JobService.AddToDBLog_Updated(JobService.SessionInfo.ServiceId);
                JobService.AddNote(JobService.SessionInfo.ServiceId, "Changed contact info", "", "");
                ViewBag.ResultSuccess = "Saved success.";
            }
            return View(model);
        }

        /// <summary>
        /// Update customer info
        /// </summary>
        /// <returns>Redirect</returns>
        [HttpGet]
        public ActionResult ProductInfoEdit()
        {
            var model = JobService.JobDetails(JobService.SessionInfo.ServiceId);
            return View(model.ProductInformation);
        }

        /// <summary>
        /// Update customer info
        /// </summary>
        /// <returns>Redirect</returns>
        [HttpPost]
        public ActionResult ProductInfoEdit(ProductInfoModel model)
        {
            ViewBag.ResultSuccess = string.Empty;
            model.Guarantee = JobService.GetGuarantee(model.GuaranteeType);
            model.DateOfPurchase = Functions.StringToDateTime(model.DateOfPurchaseString);
            model.RetailedInvoiceDate = Functions.StringToDateTime(model.RetailedInvoiceDateStrig);

            //VALIDATION
            var validator = new Validation.Job.ProductValidation();
            foreach (var error in validator.Validate(model, ruleSet: _ruleSets.defaultRule).Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            //update Customer Information
            if (ModelState.IsValid)
            {
                // Update contact info
                var product = new Product_InfoModel();
                product.InjectFrom(model);
                ProductService.UpdateProductInfo(product);
                JobService.AddToDBLog_Updated(JobService.SessionInfo.ServiceId);

                //save additional fields
                var jobDetails = JobService.JobDetails(JobService.SessionInfo.ServiceId);
                jobDetails.JobAdditionalInfo.InjectFrom(model);
                FieldsFromDbService.UpdateServiceInfo(JobService.SessionInfo.ServiceId, jobDetails.JobAdditionalInfo);

                //jobDetails.JobAdditionalInfo.JobFields.

                //JobService.AddNote(JobService.SessionInfo.ServiceId, "Changed additionsl info");

                JobService.AddNote(JobService.SessionInfo.ServiceId, "Changed product info", "", "");
                ViewBag.ResultSuccess = "Saved success.";
            }
            model.WarrantyList = JobService.GetWarrantyList(model.GuaranteeType);
            return View(model);
        }

        /// <summary>
        /// Update customer info
        /// </summary>
        /// <returns>Redirect</returns>
        [HttpGet]
        public ActionResult RepairInfoEdit()
        {
            var model = JobService.JobDetails(JobService.SessionInfo.ServiceId);
            return View(model.RepairInformation);
        }

        /// <summary>
        /// Update customer info
        /// </summary>
        /// <returns>Redirect</returns>
        [HttpPost]
        public ActionResult RepairInfoEdit(RepairInfoModel model)
        {
            ViewBag.ResultSuccess = string.Empty;

            //VALIDATION
            var validator = new Validation.Job.RepairInfoValidator();
            foreach (var error in validator.Validate(model, ruleSet: _ruleSets.defaultRule).Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            //update Customer Information
            if (ModelState.IsValid)
            {
                // Update contact info
                var repair = new Repair_InfoModel();
                repair.InjectFrom(model);
                JobService.UpdateRepairInfo(repair);
                JobService.AddToDBLog_Updated(JobService.SessionInfo.ServiceId);
                JobService.AddNote(JobService.SessionInfo.ServiceId, "Changed repair info", "", "");
                ViewBag.ResultSuccess = "Saved success.";
            }
            model.JobTypesList = JobService.GetJobTypesList(model.RepairType);
            model.CustomerTypeList = JobService.GetCustomerTypeList(model.CustomerType.ToString());
            model.RepairCostPaidList = JobService.RepairCostPaidList(model.RepairCostPaid);
            return View(model);
        }

        /// <summary>
        /// Update customer info
        /// </summary>
        /// <returns>Redirect</returns>
        [HttpGet]
        public ActionResult AdditionalJobEdit()
        {
            var model = JobService.JobDetails(JobService.SessionInfo.ServiceId);


            //      model.JobAdditionalInfo.RetailerNameList = JobService.GetRetailerList(model.JobAdditionalInfo.RetailerName, useraccount.UserStoreCountry);

            return View(model.JobAdditionalInfo);
        }
        [HttpGet]
        public ActionResult RejectReason()
        {
            List<SelectListItem> list = service.GetRejectionList();

            return View(list);
        }
        [HttpGet]
        public ActionResult AdditionalJob()
        {

            //List<SelectListItem> list = service.AdditionalJobList();//todo

            //return View(list);
            return View();
        }

        [HttpGet]
        public ActionResult AdditionalJobList()
        {
            JobTypesRepository service = new JobTypesRepository();

            List<JobCancelOption> list = service.GetJobCancelOption();//todo
            JobCancelOptionModel model = new JobCancelOptionModel();
            model.JobCancelOptionlist = list;
            return View(model);
        }


        [HttpPost]
        public ActionResult FollowOnCall(int Optionid)
        {

            bool CancelAlertNeeeded = false;
            bool success = true;
            string errorMessgae = string.Empty;
            bool showForm = false; string formPath = string.Empty;
            JobTypesRepository service = new JobTypesRepository();

            list = service.GetJobCancelOption();
            try
            {
             
                var jobOption = list.Where(x => x._ID == Optionid).FirstOrDefault();
                //  JobDetailsModel result = JobService.JobDetails(JobService.SessionInfo.ServiceId);


                BookService.ClearFromSession();
                var jobDetails = JobService.JobDetails(JobService.SessionInfo.ServiceId);
                try
                {

                    int modelId = jobDetails.ProductInformation.ModelId;
                    int custId = jobDetails.CustomerInformation.CustomerId;
                    var product = ProductService.GetDetails(modelId);
                    BookService.SessionInfo.InjectFrom(product, jobDetails.ProductInformation, jobDetails.RepairInformation);
                    BookService.SessionInfo.WarrantyFg = jobDetails.ProductInformation.GuaranteeType;
                    BookService.SessionInfo.FaultDescr = jobDetails.RepairInformation.FaultDescription;
                    BookService.SessionInfo.CustomerId = custId;
                    BookService.SessionInfo.ServiceId = JobService.SessionInfo.ServiceId;
                    BookService.SessionInfo.ModelId = modelId;
                    BookService.SessionInfo.SelectedType = jobDetails.RepairInformation.RepairType;
                    BookService.SessionInfo.RetailerId = jobDetails.JobAdditionalInfo.RetailerId;
                    BookService.SessionInfo.RetailerName = jobDetails.JobAdditionalInfo.RetailerName;

                    BookService.SessionInfo.StatusID = jobDetails.StatusId;


                    success = true;
                }

                catch (Exception ex)
                {
                    success = false;
                    errorMessgae = ex.InnerException.ToString();
                }
                if (jobDetails.partsCount > 0 && jobOption.Cancel && BookService.SessionInfo.StatusID != 2 && !jobOption.NextStepActive)
                {
                    CancelAlertNeeeded = jobDetails.CallDateTime.HasValue ? CancelConditionCheck(jobDetails.RepairInformation.VisitDate.Value) : true;
                }
                if (jobOption.Cancel && ( BookService.SessionInfo.StatusID != 2 || BookService.SessionInfo.StatusID != 8))// check Cancel need to be done prior to create selected job option
                {

                    string UpdateMessage = onlinebookingService.CancelJob(JobService.SessionInfo.ServiceId, "Job is cancelled before booking " + jobOption.OptionDesc, storeService.UserId, 0);
                    Log.File.Info(JsonConvert.SerializeObject(UpdateMessage));
                    if (UpdateMessage.Length > 0)
                    {
                        showForm = false;
                        success = false;
                        errorMessgae = "Unfortunately we cannot reschedule/cancel this appointment at this time.  You will need to go to Complete Service to complete this task. " + UpdateMessage;
                    }
                }
                if (jobOption.NextStepActive && success)
                {
                    showForm = true;
                    AppointmentModel model = new AppointmentModel();
                    RequestDetails requestdetails = new RequestDetails();
                    //todo:  model.Visitcode=??
                    requestdetails.InjectFrom(jobDetails);

                    requestdetails.SaediID = Settings.Default.SaediID; //"SHOPDIRECT";
                    requestdetails.ClientID = storeService.StoreId;


                    requestdetails.BookImmediately = false;
                    requestdetails.BookingOptions = 5;
                    requestdetails.Postcode = jobDetails.CustomerInformation.Postcode;
                    requestdetails.AddressLine1 = jobDetails.CustomerInformation.Addr1;
                    ////request.Town = CustomerService.SessionInfo.Applianceaddress.Town;
                    requestdetails.ApplianceCode = BookService.SessionInfo.ApplianceCD;
                    requestdetails.Skill = BookService.SessionInfo.Skills;

                    model.availabiltyModel = requestdetails;
                    if( !string.IsNullOrEmpty(jobOption.DefaultVisitCd))
                    model.Visitcode = jobOption.DefaultVisitCd;
                    BookService.SessionInfo.appointmentModel = model;

                    formPath = "Job/RequestDate";
                }
            }
            catch (Exception ex)
            {
                Log.File.ErrorFormat("UserId {0} . error on FollowOnCall {1}", storeService.UserId, JsonConvert.SerializeObject(JobService.SessionInfo.ServiceId) + ex.Message.ToString());
                success = false;
                showForm = false;
                errorMessgae = ex.Message.ToString();

            }
            return Json(new { success = success, errorMessgae = errorMessgae, showForm = showForm, formPath = formPath,CancelAlertNeeeded=CancelAlertNeeeded });
        }

        private bool CancelConditionCheck(DateTime calldateTime)
        {
            bool CancelAlertNeeded = false;
            // for restricting the cancel & rescheduling for jobs linked with Parts.
            //2.	For cancel/reschedule jobs the following rules should apply: (only if parts are involved)
            //•	Mon before 5pm & appointment on Weds – can be cancelled & rescheduled with no other action required. correct
            //•	Mon after 5pm or anytime Tues & appointment on Weds -  can be cancelled & rescheduled but need to display message advising parts must be re-ordered via Complete Service. LR to provide wording – 

            //“You are attempting to reschedule an appointment that may have parts allocated after a pick run has taken place. If parts are required on this job please ensure you access complete service to reorder parts failure to do so may result in the engineer attending without parts. To check go to the notes field in the parts tab and select the pick tab – if the comments advised parts picked then you must reorder all parts and quantities on the rebooked visit – if there are no comments advising a pick has been run then you do not need to reorder the parts – please check with your line manager for clarification” 
            //If cancel and reschedule for an initial visit (no parts involved) 
            //•	Monday up to 8pm  – appointment Tuesday you can still cancel this appointment and reschedule for an alternative available date – no same day cancellations can happen – these must be called through to the attending engineer to abandon.

            //•	Sat and Sun will be treated as non-working for the purposes of this rule. 

           
            List<DateTime> Holidays = HomeService.GetHolidaysList(DateTime.Now.Year);
            DateTime calldate = new DateTime(calldateTime.Year, calldateTime.Month, calldateTime.Day, 0, 0, 0);
            var now = DateTime.Now;

            DateTime todayDate = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);//DateTime.Now;
        SKIPHOLIDAYS: TimeSpan ts = calldate - todayDate;
            if (ts.TotalHours <= 31) // if cancelling is after 5pm for Day after tommorow appointment
                CancelAlertNeeded = true;//  Alert needed. because  cancelling is after 5pm for tomorrow appointment");
            else
            {
                List<DateTime> DatesList = GetDatesBetween(todayDate, calldate);
                foreach (var item in DatesList)
                {
                    int matchCount = 0;

                    if (Holidays.Any(x => x.Date == item))
                    {
                       
                        matchCount += 1;
                    }
                    if (matchCount > 0)
                    {
                        todayDate = todayDate.AddDays(matchCount);
                        goto SKIPHOLIDAYS;
                    }
                }
                
                
            }
            return CancelAlertNeeded;

        }
        private List<DateTime> GetDatesBetween(DateTime startDate, DateTime endDate)
        {
            List<DateTime> allDates = new List<DateTime>();
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                allDates.Add(date);
            return allDates;

        }
        public ActionResult BookNow(string EventDate, int Engineerid, string faultdesc = "", string reason = "", JobType type = JobType.Defaulttype)
        {
            int serviceid = JobService.SessionInfo.ServiceId;

            bool tempServiceId = false;
            string serviceNotes = string.Empty;
            var result = new OnlineBookResponseDetails();
            bool success = false;
            string FormPath = string.Empty;
            int NEWserviceid = 0;
            string visitdate = string.Empty;
            string userid = storeService.UserId;
            var errorMsg = "Job Booking failed";
            var errorDetails = string.Empty;

            var jobDetails = JobService.JobDetails(JobService.SessionInfo.ServiceId);

            OnlineBookRequestDetails model = new OnlineBookRequestDetails();
            model.InjectFrom(jobDetails);
            model.Postcode = jobDetails.CustomerInformation.Postcode;
            model.EngineerID = Engineerid == 0 ? Settings.Default.DumpId : Engineerid;
            model.VisitDate = (EventDate == "") ? DateTime.Now.AddDays(storeinfo.Clientbookingdelaydays) : DateTime.Parse(EventDate);
            model.StatusID = 4;
            model.JobID = jobDetails.JobId == 0 ? serviceid : jobDetails.JobId;
            model.Model = BookService.SessionInfo.ItemCode;
            model.CustomerID = BookService.SessionInfo.CustProd.CustomerId == 0 ? BookService.SessionInfo.CustomerId : BookService.SessionInfo.CustProd.CustomerId;
            model.CustAplID = jobDetails.ProductInformation.CustaplId;
            if (!BookService.SessionInfo.appointmentModel.IsGetAvailabiltyInfoPressed)
                BookService.SessionInfo.FaultDescr = faultdesc;
            model.ReportFault = string.IsNullOrEmpty(BookService.SessionInfo.appointmentModel.TroubleShootDescr) ? BookService.SessionInfo.FaultDescr : BookService.SessionInfo.appointmentModel.TroubleShootDescr + "Additional fault info:  " + BookService.SessionInfo.FaultDescr;
            model.SNO = jobDetails.ProductInformation.SerialNumber;
            model.PolicyNumber = jobDetails.ProductInformation.PolicyNumber;
            model.AuthNo = "";//jobDetails.ProductInformation.AuthNo;
            model.ClientID = storeService.StoreId;
            model.PolicyExpireyDate = jobDetails.ProductInformation.CONTRACTEXPIRES;//.HasValue? BookService.SessionInfo.CustProd.contractexpires.Value : new DateTime();
            model.VisitCode = reason == "" ? "000" : reason;
            model.ClientRef = BookService.SessionInfo.CustProd.ClientRef;
            model.ApplianceCD = BookService.SessionInfo.ApplianceCD;


           
            if (type != JobType.Defaulttype)
            {
                if (specialJob != null && specialJob.Count() > 0)
                {
                    model.EngineerID = specialJob.Where(x => x.JobTypeid == (int)type).First().EngId;
                    model.VisitCode = specialJob.Where(x => x.JobTypeid == (int)type).First().VisitType;
                    model.StatusID = specialJob.Where(x => x.JobTypeid == (int)type).First().StatusId;
                    model.Skills = specialJob.Where(x => x.JobTypeid == (int)type).First().Skill;
                }
            }
            if (Session["FirstDayOffered"] != null)
            {
                model.RequestedLaterAppt = (DateTime)Session["FirstDayOffered"] < model.VisitDate;
            }


            var response = onlinebookingService.BookJob(model);
            if (response.BookSuccessfully)
            {
                FormPath = string.Format("/BookNewService/BookedJobConfirmation?Serviceid={0}&engineerid={1}&eventdate={2}&tempServiceId={3}", response.ServiceID, Engineerid, EventDate, tempServiceId);
                success = response.BookSuccessfully;
                serviceid = response.ServiceID;
                visitdate = model.VisitDate.ToString();
                ServiceModel serviceModel = new ServiceModel();
                serviceModel.InjectFrom(response);
                serviceModel.Engineerid = Engineerid;
                serviceModel.CustomerID = BookService.SessionInfo.CustProd.CustomerId == 0 ? BookService.SessionInfo.CustomerId : BookService.SessionInfo.CustProd.CustomerId;
                serviceModel.CustAplID = BookService.SessionInfo.CustProd.CustAplId == 0 ? model.CustAplID : BookService.SessionInfo.CustProd.CustAplId;
                serviceModel.EventDate = model.VisitDate.ToString();
                serviceModel.Reportfault = model.ReportFault;
                serviceModel.ClientId = model.ClientID;
                serviceModel.VisitCode = model.VisitCode;
                serviceModel.Clientref = model.ClientRef;
                serviceModel.StatusId = model.StatusID;
                serviceModel.JobId = model.JobID;
                serviceModel.JobSequenceId =(serviceid != model.JobID)? jobDetails.JOBSEQUENCEID + 1 : 0;
                int Serviceid = JobService.CreateJobwithEngineer(serviceModel); BookService.SessionInfo.ServiceId = serviceid;
                if (model.StatusID != 4 && type != JobType.Defaulttype)
                {
                    onlinebookingService.SetStatusId(response.ServiceID, model.StatusID, 0);
                }
                serviceNotes += string.Format("Job is booked from Client connect");
                if (serviceNotes != string.Empty)
                    onlinebookingService.AddNote(serviceid, string.Empty, serviceNotes);

                BookService.SessionInfo.ServiceId = serviceid;
            }
            else
            {
                tempServiceId = true;
                ServiceModel serviceModel = new ServiceModel();
                serviceModel.InjectFrom(response);
                serviceModel.AuthNo = BookService.SessionInfo.CustProd.AuthNo;
                serviceModel.Engineerid = (Engineerid == 0 && type != JobType.Defaulttype) ? model.EngineerID : Engineerid;
                serviceModel.CustomerID = BookService.SessionInfo.CustProd.CustomerId;
                serviceModel.CustAplID = BookService.SessionInfo.CustProd.CustAplId;
                serviceModel.EventDate = model.VisitDate.ToString();
                serviceModel.Reportfault = model.ReportFault;
                serviceModel.ClientId = model.ClientID;
                serviceModel.VisitCode = model.VisitCode;
                serviceModel.Clientref = model.ClientRef;
                serviceModel.StatusId = model.StatusID;
                int serviceId = onlinebookingService.BookBackupJob(serviceModel, false, false, model.PolicyNumber, BookService.SessionInfo.OnlinebookingFailedReason);

                FormPath = string.Format("/BookNewService/BookedJobConfirmation?Serviceid={0}&engineerid={1}&eventdate={2}&tempServiceId={3}", serviceId, Engineerid, EventDate, tempServiceId);
                success = false;// response.BookSuccessfully;
                //success = false;
                //errorMsg = response.ErrorMsg;
            }
            return Json(new { success = success, FormPath = FormPath, errorMessage = errorMsg, tempServiceid = tempServiceId, type = (int)type, ApplianceType = (int)BookService.SessionInfo.DeviceType });
        }

        [HttpGet]
        public ActionResult RequestDate()
        {
            RequestDetails request = new RequestDetails();
            ResponseDetails response = new ResponseDetails();
            request.InjectFrom(BookService.SessionInfo.appointmentModel.availabiltyModel);
            AppointmentModel appointment = new AppointmentModel();
            //appointment.InjectFrom(BookService.SessionInfo);

            List<string> ExcludeVisitType = Settings.Default.ExcludeVisitType.Split(',').ToList();
            if(!string.IsNullOrEmpty(BookService.SessionInfo.appointmentModel.Visitcode))
                appointment.Visitcodes = JobService.GetJobTypesList(BookService.SessionInfo.appointmentModel.Visitcode).Where(x => x.Value== BookService.SessionInfo.appointmentModel.Visitcode).ToList();
            else
                appointment.Visitcodes = JobService.GetJobTypesList(string.Empty).Where(x => !ExcludeVisitType.Any(y => y == x.Value)).ToList();
            appointment.FaultDescr = BookService.SessionInfo.FaultDescr;

            //if (appointment.PreferredVisitDate == null )
            //{
            // var storeinfo = storeService.GetStoreInfo(storeService.StoreId);
            DateTime AppDate = DateTime.Now.AddDays(storeinfo.Clientbookingdelaydays);
            List<DateTime> Holidays = HomeService.GetHolidaysList(DateTime.Now.AddDays(storeinfo.Clientbookingdelaydays).Year);
            while (Holidays.Contains(AppDate) || AppDate.DayOfWeek == DayOfWeek.Saturday || AppDate.DayOfWeek == DayOfWeek.Sunday)
                AppDate = AppDate.AddDays(1);
            appointment.PreferredVisitDate = AppDate;// DateTime.Now.AddDays(3)// DateTime.Now.AddDays(3);
            //}
            //else
            //{
            //    appointment.PreferredVisitDate = AppointmentDate.Value;
            //}
            //return View(appointment);


            request.SaediID = Settings.Default.SaediID; //"SHOPDIRECT";
            request.ClientID = storeService.StoreId;

            request.RequestedDate = appointment.PreferredVisitDate.Value.ToShortDateString();
            request.BookImmediately = false;
            request.BookingOptions = 5;


            response = onlinebookingService.AppointmentRequest(request);

            Log.File.Info("Appointment request input:" + JsonConvert.SerializeObject(request) + "Output:" + JsonConvert.SerializeObject(response));

            if (response.RequestSuccess)
            {
                BookService.SessionInfo.AppointmentRetreiveFailed = false;
                ViewBag.GetUnitError = string.Empty;
                var IsappointmentAvailable = response.BookingOptionResult.Any(x => x.EventDate.DayOfYear == appointment.PreferredVisitDate.Value.DayOfYear);
                if (IsappointmentAvailable)
                    ViewBag.GetUnitSuccess = "Engineer is available for the date chosen ";

                else
                {  // else(model.PreferredVisitDate
                    ViewBag.GetUnitSuccess = "Engineer is not available for the date chosen .Please choose one from following.";

                }
                Session["FirstDayOffered"] = response.BookingOptionResult.OrderBy(x => x.EventDate).FirstOrDefault().EventDate;

                appointment.availabiltyModel = request;
                appointment.availabiltyResponseModel = response;
                ModelState.Clear();
            }


            else
            {
                BookService.SessionInfo.AppointmentRetreiveFailed = true;
               // BookService.SessionInfo.
                response = onlinebookingService.AppointmentRequestBackUp(request);
                appointment.availabiltyModel = request;
                appointment.availabiltyResponseModel = response;
                return View("RequestDate", appointment);
                //ViewBag.GetUnitError = string.Format("Error: {0}", response.ErrorCode);
                //ViewBag.GetUnitSuccess = response.ErrorText;
            }



            Log.File.Info(storeService.UserId + " ,Appointment list to user:" + appointment);
            return View("RequestDate", appointment);
        }

        public ActionResult GetAvailability(AppointmentModel AppointmentModel)//string AppointmentDatestring)
        {
            //string AppointmentDatestring = "09/09/2019";
            DateTime? AppointmentDate = AppointmentModel.PreferredVisitDate;
            RequestDetails request = new RequestDetails();
            ResponseDetails response = new ResponseDetails();
            request.InjectFrom(BookService.SessionInfo.appointmentModel.availabiltyModel);
            AppointmentModel appointment = new AppointmentModel();
            //appointment.InjectFrom(BookService.SessionInfo);
            List<string> ExcludeVisitType = Settings.Default.ExcludeVisitType.Split(',').ToList();
            if (!string.IsNullOrEmpty(AppointmentModel.Visitcode))
                appointment.Visitcodes = JobService.GetJobTypesList(AppointmentModel.Visitcode).Where(x => x.Value == AppointmentModel.Visitcode).ToList();
            else
            appointment.Visitcodes = JobService.GetJobTypesList(string.Empty).Where(x => !ExcludeVisitType.Any(y => y == x.Value)).ToList();
            appointment.FaultDescr = AppointmentModel.FaultDescr;


            appointment.PreferredVisitDate = AppointmentModel.PreferredVisitDate.Value;
          


            request.SaediID = Settings.Default.SaediID; //"SHOPDIRECT";
            request.ClientID = storeService.StoreId;
            
            request.RequestedDate = appointment.PreferredVisitDate.Value.ToShortDateString();
            request.BookImmediately = false;
            request.BookingOptions = 5;
            

            response = onlinebookingService.AppointmentRequest(request);

            Log.File.Info("Appointment request input:" + JsonConvert.SerializeObject(request) + "Output:" + JsonConvert.SerializeObject(response));

            if (response.RequestSuccess)
            {
                BookService.SessionInfo.AppointmentRetreiveFailed = false;
                ViewBag.GetUnitError = string.Empty;
                var IsappointmentAvailable = response.BookingOptionResult.Any(x => x.EventDate.DayOfYear == appointment.PreferredVisitDate.Value.DayOfYear);
                if (IsappointmentAvailable)
                    ViewBag.GetUnitSuccess = "Engineer is available for the date chosen ";

                else
                {  // else(model.PreferredVisitDate
                    ViewBag.GetUnitSuccess = "Engineer is not available for the date chosen .Please choose one from following.";

                }
                Session["FirstDayOffered"] = response.BookingOptionResult.OrderBy(x => x.EventDate).FirstOrDefault().EventDate;

                appointment.availabiltyModel = request;
                appointment.availabiltyResponseModel = response;
                ModelState.Clear();
            }


            else
            {
                BookService.SessionInfo.AppointmentRetreiveFailed = true;

                response = onlinebookingService.AppointmentRequestBackUp(request);
                appointment.availabiltyModel = request;
                appointment.availabiltyResponseModel = response;
                return View("RequestDate", appointment);
                //ViewBag.GetUnitError = string.Format("Error: {0}", response.ErrorCode);
                //ViewBag.GetUnitSuccess = response.ErrorText;
            }


            //}
            //else
            //{
            //    ViewBag.Replacement = "True";

            //    BookService.SessionInfo.Jobtype = JobType.Replacement;
            //    return View("RequestDate", appointment);
            //}
            ////model = BookService.FillJobPageLists(model, BookService.SessionInfo);
            ////model.FileName = BookService.SessionInfo.UploadedFile.FileName;
            //// ViewBag.SuperAdmin = storeService.IsSuperAdmin;
            Log.File.Info(storeService.UserId + " ,Appointment list to user:" + appointment);
            return View("RequestDate", appointment);
        }

        [HttpGet]
        public ActionResult CancelReason()
        {

            List<SelectListItem> list = service.GetSubStatusList((int)JobStatus.Cancelled, "");//todo

            return View(list);
        }
        [HttpGet]
        public ActionResult QueryJob()
        {
            return View(JobService.SessionInfo.ServiceId);
        }

        [HttpGet]
        public ActionResult ApproveJob()
        {
            return View(JobService.SessionInfo.ServiceId);
        }
        /// <summary>
        /// Update customer info
        /// </summary>
        /// <returns>Redirect</returns>
        [HttpPost]
        public ActionResult AdditionalJobEdit(Job_AdditionalInfo model)
        {
            ViewBag.ResultSuccess = string.Empty;

            //update Customer Information
            if (ModelState.IsValid)
            {
                AccountService accService = new AccountService();
                var useraccount = accService.GetAccountDetails(JobService.UserId);
                model.RetailerNameList = JobService.GetRetailerList(model.RetailerId.ToString());
                model.RetailerName = model.RetailerNameList.Find(x => x.Selected).Text.ToString();

                FieldsFromDbService.UpdateServiceInfo(JobService.SessionInfo.ServiceId, model);
                FieldsFromDbService.SaveFields(JobService.SessionInfo.ServiceId, model.JobFields.AdditionalFields);
                JobService.AddToDBLog_Updated(JobService.SessionInfo.ServiceId);
                FieldsFromDbService.SaveFields(JobService.SessionInfo.ServiceId, model.AEPFieldsFromDB.AepFields, true);
                JobService.AddNote(JobService.SessionInfo.ServiceId, "Changed additional info", "", "");
                ViewBag.ResultSuccess = "Saved success.";
            }
            var details = JobService.JobDetails(JobService.SessionInfo.ServiceId);
            return View(details.JobAdditionalInfo);
        }


        /// <summary>
        /// Job cancel
        /// </summary>
        /// <param name="jobId">Job Id</param>
        /// <param name="Reason">Reasen why job has been canceled</param>
        /// <returns></returns>
        //[HttpPost]
        //public ActionResult CancelJob(int? jobId, string Reason)
        //{
        //    if (jobId != null)
        //    {
        //        JobService.CancelJob((int)jobId, "Job canceled. Reason: " + Reason);
        //        // Add log record in ServiceUsage
        //        Log.Database.Job.Add.Updated((int)jobId);
        //    }
        //    return new EmptyResult();
        //}

        /// <summary>
        /// Accept job
        /// </summary>
        /// <returns></returns>
        public ActionResult AcceptJob(int jobId, int custId, int modelId)
        {

            string Notes = "Job is accepted";
            string UpdateMessage = onlinebookingService.ApproveJob(JobService.SessionInfo.ServiceId, Notes, storeService.UserId);
            if (UpdateMessage == string.Empty)
                return Redirect(Url.ProcessPreviousStep());

            else
            {
                ViewBag.Error = UpdateMessage;

                if (JobService.SessionInfo.ServiceId > 0)
                {
                    var result = JobService.JobDetails(JobService.SessionInfo.ServiceId);
                    ViewBag.IsCallCenter = JobService.IsCallCenter;
                    ViewBag.AllPageOfNotes = result.JobNotes.ToPagedList(1, 20);
                    ViewBag.SuperAdmin = storeService.IsSuperAdmin;
                    result.StoreId = JobService.StoreId;
                    result.ReportList = ReportsService.GetReportList(JobService.SessionInfo.ServiceId);
                    return View("JobDetails", result);
                }

                else
                    return RedirectToAction("JobDetails");
            }


        }
        public ActionResult SendEmailNotification(JobDetailsModel model)
        {
            var reportInfo = ReportsService.GetReportsInfo(75);
            int serviceId = JobService.SessionInfo.ServiceId;
            string groupName = reportInfo.GroupName;
            string template = reportInfo.ReportTemplate;
            int modelId = model.ProductInformation.ModelId;
            return RedirectToAction("JobDetails");


            // try
            //{
            //    ReportsService repService = new ReportsService();
            //    var report = repService.GetReportByTemplate(modelId, serviceId, template, groupName);
            // return   ConvertReportToPdf(report);
            //}
            //catch (Exception)
            //{
            //    return ConvertReportToPdf(Service.GetErrorReport());
            //}
            //return View();
        }
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
        [HttpPost]
        public ActionResult ReportPage(int reportId, int modelId)
        {
            var reportInfo = ReportsService.GetReportsInfo(reportId);
            ViewBag.ServiceId = JobService.SessionInfo.ServiceId;
            ViewBag.GroupName = reportInfo.GroupName;
            ViewBag.ReportTemplate = reportInfo.ReportTemplate;
            ViewBag.ModelId = modelId;
            return View();
        }
        [HttpPost]
        public ActionResult EmailJobSheet(int modelId, int ServiceId, string notificationEmailAddress)
        {
            string EmailSentFailed = string.Empty;

            var report = ReportsService.GetRepairInstruction(ServiceId, modelId);
            using (var stream = new MemoryStream())
            {
                // If report is not rendered, then render.
                if (!report.IsRendered)
                    report.Render(false);

                report.ExportDocument(StiExportFormat.Pdf, stream);

                FileContentResult result = File(stream.ToArray(), "application/pdf");

                bool sent = emailService.SendEmail(stream.ToArray(), ServiceId, notificationEmailAddress);


                if (!sent)
                    EmailSentFailed = "There is a problem on sending email to " + notificationEmailAddress;

                return RedirectToAction("JobDetails", new { id = ServiceId, Error = EmailSentFailed });
            }
        }

        public ActionResult AdvSearch(int? page)
        {
            if (page == null) page = JobService.SessionInfo.PageNumber == 0 ? 1 : JobService.SessionInfo.PageNumber;
            JobService.SessionInfo.PageNumber = page ?? 1;
            Job_SearchModel modelsession = new Job_SearchModel();
            AdvSearchCriteria AdvSearchCriteria = new AdvSearchCriteria();
            AdvSearchCriteria.Postcode = JobService.SessionInfo.Postcode;
            AdvSearchCriteria.JobId = JobService.SessionInfo.SearchJobId;
            AdvSearchCriteria.Surname = JobService.SessionInfo.Surname;
            AdvSearchCriteria.PolicyNumber = JobService.SessionInfo.PolicyNumber;
            AdvSearchCriteria.ClientCustRef = JobService.SessionInfo.ClientCustRef;
            AdvSearchCriteria.Address = JobService.SessionInfo.Address;
            AdvSearchCriteria.TelNo = JobService.SessionInfo.TelNo;

            AdvSearchCriteria.JobId = JobService.SessionInfo.SearchCriteria;
            modelsession.AdvSearchCriteria = AdvSearchCriteria;
            // model.SearchCriteria = JobService.SessionInfo.SearchCriteria;
            //model.CurrentPage = JobService.SessionInfo.PageNumber;
            var model = JobService.GetJobsList(modelsession.AdvSearchCriteria, JobService.SessionInfo.PageNumber);
            if (model.TotalRecords == 1)
            {
                // Checking if user push back button on ShowDetails
                if (JobService.IsBackButtonPressed) return View(model);
                return RedirectToAction("GoToDetails", new { model.SearchResults[0].Id });
            }
            ViewBag.OnePageOfJobs = new StaticPagedList<Job_SearchResult>(model.SearchResults, JobService.SessionInfo.PageNumber, Settings.Default.JobSearchPageSize, model.TotalRecords);
            // return RedirectToAction("AdvSearchCriteria", new { model.AdvSearchCriteria, page });
            return View(model);
        }
        [HttpPost]
        public ActionResult AdvSearchCriteria(AdvSearchCriteria model, int? page)
        {
            if (page == null) page = JobService.SessionInfo.PageNumber;
            JobService.SessionInfo.PageNumber = page ?? 1;
            JobService.SessionInfo.SearchJobId = string.IsNullOrEmpty(model.JobId) ? string.Empty : model.JobId;
            JobService.SessionInfo.Surname = string.IsNullOrEmpty(model.Surname) ? string.Empty : model.Surname;
            JobService.SessionInfo.Postcode = string.IsNullOrEmpty(model.Postcode) ? string.Empty : model.Postcode;
            JobService.SessionInfo.PolicyNumber = string.IsNullOrEmpty(model.PolicyNumber) ? string.Empty : model.PolicyNumber;
            JobService.SessionInfo.TelNo = string.IsNullOrEmpty(model.TelNo) ? string.Empty : model.TelNo;
            JobService.SessionInfo.ClientCustRef = string.IsNullOrEmpty(model.ClientCustRef) ? string.Empty : model.ClientCustRef;
            JobService.SessionInfo.Address = string.IsNullOrEmpty(model.Address) ? string.Empty : model.Address;
            //  JobService.SessionInfo.SearchCriteria = string.IsNullOrEmpty(model.Address) ? string.Empty : model.Address;
            var result = JobService.GetJobsList(model, JobService.SessionInfo.PageNumber);

            result.AdvSearchCriteria = model;
            if (result.TotalRecords == 1)
            {
                // Checking if user push back button on ShowDetails
                if (JobService.IsBackButtonPressed) return View(model);
                return RedirectToAction("GoToDetails", new { result.SearchResults[0].Id });
            }
            ViewBag.OnePageOfJobs = new StaticPagedList<Job_SearchResult>(result.SearchResults, JobService.SessionInfo.PageNumber, Settings.Default.JobSearchPageSize, result.TotalRecords);
            return View("AdvSearch", result);
        }

        public ActionResult FindJobsByClientId(int? page)
        {
            if (page == null) page = JobService.SessionInfo.PageNumber;
            JobService.SessionInfo.PageNumber = page ?? 1;

            string sc = JobService.SessionInfo.SearchCriteria;

            //Decided on meeting with Leanne Starke 25/9/2019 to only search by serviceId in simple search so set other search conditions to null
            AdvSearchCriteria model = new AdvSearchCriteria(sc, null, null, null, null, null, null, true);

            var result = JobService.GetJobsListByClientId(model, JobService.SessionInfo.PageNumber);

            result.SearchCriteria = sc;

            // If only one product finded, redirect to details
            if (result.TotalRecords == 1 && !JobService.IsBackButtonPressed)
            {
                if (JobService.SessionInfo.FromIndex)
                {
                    ((ProcessService)Ioc.Get<ProcessService>()).RemoveCurrentProcess();
                }

                return RedirectToAction("GoToDetails", new { id = result.SearchResults[0].Id });
            }
            ViewBag.OnePageOfJobs = new StaticPagedList<Job_SearchResult>(result.SearchResults, JobService.SessionInfo.PageNumber, Settings.Default.JobSearchPageSize, result.TotalRecords);
            return View(result);
        }

        public ActionResult ClientFindJobsByClientId(string JobSearchCriteria, bool? fromIndex)
        {
            JobService.SessionInfo.SearchCriteria = string.IsNullOrEmpty(JobSearchCriteria) ? string.Empty : JobSearchCriteria.Trim();
            JobService.SessionInfo.PageNumber = 1;
            JobService.SessionInfo.FromIndex = fromIndex.HasValue ? true : false;
            return Redirect(Url.Process(PredefinedProcess.FindJobsByClientId));
        }

        public ActionResult StartAdvSearchByClientId()
        {
            ViewBag.UseAndInWhereConditionANDChecked = "checked";
            ViewBag.UseAndInWhereConditionORChecked = "";
            return Redirect(Url.Process(PredefinedProcess.JobAdvSearchByClientId));
        }

        public ActionResult AdvSearchByClientId(AdvSearchCriteria model, int? page)
        {
            bool takenFromSession = false; //Default
            bool isBackButtonPressed = JobService.IsBackButtonPressed;

            if (Request.HttpMethod == "GET" && !page.HasValue && !isBackButtonPressed)
            {
                ViewBag.UseAndInWhereConditionANDChecked = "checked";
                ViewBag.UseAndInWhereConditionORChecked = "";
                return View(new Job_SearchModel());
            }
            else if (page.HasValue || isBackButtonPressed) //page.HasValue means user clicked on paging, otherwise page is null
            {
                model = new AdvSearchCriteria(
                    JobService.SessionInfo.SearchServiceId,
                     JobService.SessionInfo.Surname,
                     JobService.SessionInfo.Postcode,
                     JobService.SessionInfo.ClientRef,
                     JobService.SessionInfo.PolicyNumber,
                     JobService.SessionInfo.Address,
                     JobService.SessionInfo.TelNo,
                     JobService.SessionInfo.UseAndInWhereCondition);

                ViewBag.UseAndInWhereConditionANDChecked = JobService.SessionInfo.UseAndInWhereCondition ? "checked" : "";
                ViewBag.UseAndInWhereConditionORChecked = JobService.SessionInfo.UseAndInWhereCondition ? "" : "checked";

                takenFromSession = true;
            }
            else
            {
                JobService.SessionInfo.SearchServiceId = string.IsNullOrEmpty(model.ServiceId) ? string.Empty : model.ServiceId;
                JobService.SessionInfo.Surname = string.IsNullOrEmpty(model.Surname) ? string.Empty : model.Surname;
                JobService.SessionInfo.Postcode = string.IsNullOrEmpty(model.Postcode) ? string.Empty : model.Postcode;
                JobService.SessionInfo.PolicyNumber = string.IsNullOrEmpty(model.PolicyNumber) ? string.Empty : model.PolicyNumber;
                JobService.SessionInfo.TelNo = string.IsNullOrEmpty(model.TelNo) ? string.Empty : model.TelNo;
                JobService.SessionInfo.ClientRef = string.IsNullOrEmpty(model.ClientRef) ? string.Empty : model.ClientRef;
                JobService.SessionInfo.Address = string.IsNullOrEmpty(model.Address) ? string.Empty : model.Address;
                JobService.SessionInfo.UseAndInWhereCondition = (bool)model.UseAndInWhereCondition;

                ViewBag.UseAndInWhereConditionANDChecked = (bool)model.UseAndInWhereCondition ? "checked" : "";
                ViewBag.UseAndInWhereConditionORChecked = (bool)model.UseAndInWhereCondition ? "" : "checked";
            }

            if (!isBackButtonPressed)
                JobService.SessionInfo.PageNumber = page ?? 1;
            else
                ((ClientConnect.Models.General.General_SessionModel)Session["General_SessionModel"]).IsBackButtonPressed = false; //Reset

            var result = JobService.GetJobsListByClientId(model, JobService.SessionInfo.PageNumber);

            result.AdvSearchCriteria = model;

            if (result.TotalRecords == 1 && !takenFromSession)
            {
                return RedirectToAction("GoToDetails", new { id = result.SearchResults[0].Id });
            }
            else if (result.TotalRecords > 1)
            {
                ViewBag.OnePageOfJobs = new StaticPagedList<Job_SearchResult>(result.SearchResults, JobService.SessionInfo.PageNumber, Settings.Default.JobSearchPageSize, result.TotalRecords);
            }

            return View(result);
        }

    }
}