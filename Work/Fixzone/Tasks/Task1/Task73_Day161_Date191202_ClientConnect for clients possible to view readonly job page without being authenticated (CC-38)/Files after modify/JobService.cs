using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ClientConnect.Configuration;
using ClientConnect.Infrastructure;
using ClientConnect.Jobs;
using ClientConnect.Logging;
using ClientConnect.Models;
using ClientConnect.Models.Job;
using ClientConnect.Properties;
using ClientConnect.Repositories;
using ClientConnect.ViewModels.Job;
using Omu.ValueInjecter;
using ClientConnect.Models.Product;
using System.Linq;

namespace ClientConnect.Services
{
    public class JobService : Service, IService
    {
        private JobRepository Repository { get; set; }
        private JobPartsRepository PartsRepository { get; set; }
        private CustomerService CustomerService { get; set; }
        private FieldsFromDBService FieldsFromDbService { get; set; }
        private AccountService accountService { get; set; }
        private TemplateRepository templaterep { get; set; }
        
        public JobService(CustomerService customerService, FieldsFromDBService fieldsFromDbService)
        {
            CustomerService = customerService;
            FieldsFromDbService = fieldsFromDbService;

            Repository = (JobRepository)Ioc.Get<JobRepository>();
            accountService = (AccountService)Ioc.Get<AccountService>();
            PartsRepository = (JobPartsRepository)Ioc.Get<JobPartsRepository>();
            templaterep = (TemplateRepository)Ioc.Get<TemplateRepository>();
            
        }

        public Job_SessionModel SessionInfo
        {
            get { return Session.Load(new Job_SessionModel()); }
        }


        /// <summary>
        /// Save new service
        /// </summary>
        /// <param name="model">Info model</param>
        /// <returns>Service id</returns>
        public int CreateJobWithoutEngineer(ServiceInfoModel model)
        {
            Log.File.Info(Msg.GenerateLogMsg("Saving job info..."));
            var serviceId = Repository.SaveServiceWithoutEngineer(model);
            return serviceId;
        }
        public int CreateRepeatedJobWithoutEngineer(ServiceInfoModel model)
        {
            Log.File.Info(Msg.GenerateLogMsg("Saving job info..."));
            model.StatusId = 12;
            var serviceId = Repository.SaveServiceWithoutEngineer(model);
           serviceId= Repository.UpdateStatusId(serviceId, model.StatusId);
            return serviceId;
        }
        //public void RejectJob(string notes)
        //{
        //    Log.File.Info(Msg.GenerateLogMsg("Rejecting the job ..."));
        //    var jobInfo = new ServiceInfoModel();
        //    jobInfo.InjectFrom(SessionInfo);
        //    jobInfo.ClientId = StoreId;
        //    jobInfo.UserID = UserId;


        //    Repository.RejectJob(jobInfo,notes);
          

        //}

        public void SetStatusID(int serviceid, int statusid,int subStatusId)
        {
            Log.File.Info(Msg.GenerateLogMsg("updating the status of the job ..."));
            var jobInfo = new ServiceInfoModel();
            jobInfo.ServiceId=serviceid;
            jobInfo.SubStatus = subStatusId;
            jobInfo.StatusId = statusid;
            Repository.SetStatusID(jobInfo);
        }
        /// <summary>
        /// Update attached file in database
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public int UpdateAttachedFile(UploadedFileModel file, int serviceId)
        {
            return Repository.UpdateAttachedFile(file, serviceId);
        }


        public JobUserSessionInfo GetJobSessionInfo(int serviceId, string UserId)
        {
             return Repository.GetJobSessionInfo(serviceId, UserId);
        }
        internal List<Job_DetailsDto> GetServiceList(int Custaplid)
        {
            List<Job_DetailsDto> list = new List<Job_DetailsDto>();
            var Appliances = Repository.GetJobDetailsByCustomerAppliance(Custaplid);
            //foreach (var item in Appliances)
            //{
            //    JobDetailsModel model = new JobDetailsModel();
            //    model.JOBSEQUENCEID = item.JOBSEQUENCEID;
            //    model.JobId = item.JobId;
            //    model.

            //}
            return Appliances;
        }
        /// <summary>
        /// Get job details
        /// </summary>
        /// <returns></returns>
        public JobDetailsModel JobDetails(int serviceId)
        {
            //_sessionModel = _session.Load(_sessionModel);
            Log.File.Info(Msg.GenerateLogMsg("Getting job details...", "Service Id = " + serviceId));

            // Get details
            var jobDetailsDto = Repository.GetJobDetails(serviceId, UserId);
            var JobStatusHistory = Repository.GetJobStatusHistory(serviceId); 
             
            // initialization of result modle
            var result = new JobDetailsModel();
            result.InjectFrom(jobDetailsDto);
            result.JobStatusHistory=JobStatusHistory;
            //uploaded file
            result.UploadedFile.InjectFrom(jobDetailsDto);

            // Customer information
            result.CustomerInformation.InjectFrom(jobDetailsDto);
            result.CustomerInformation.CountryList = CustomerService.GetCountryList(result.CustomerInformation.Country);
            result.JobFaultInfo.InjectFrom(jobDetailsDto);
            // Contact information
            result.ContactInformation.InjectFrom(jobDetailsDto);

            // Product information
            result.ProductInformation.InjectFrom(jobDetailsDto);
            result.ProductInformation.WarrantyList = GetWarrantyList(jobDetailsDto.GuaranteeType);
            result.ProductInformation.DateOfPurchaseString =
               Functions.DateTimeToString(result.ProductInformation.DateOfPurchase);
            result.ProductInformation.RetailedInvoiceDateStrig =
                Functions.DateTimeToString(result.ProductInformation.RetailedInvoiceDate);
           // result.ProductInformation.ApplianceType = jobDetailsDto.ApplianceType
            // Repair information
            result.RepairInformation.InjectFrom(jobDetailsDto);
            //result.RepairInformation.RepairCost = jobDetailsDto.RepairCost;
            //result.RepairInformation.RepairType = jobDetailsDto.SelectedType;
            //result.RepairInformation.JobTypesList = GetJobTypesList(jobDetailsDto.SelectedType);
            //result.RepairInformation.CustomerTypeList = GetCustomerTypeList(jobDetailsDto.CustomerType.ToString());
            //result.RepairInformation.RepairCostPaidList = RepairCostPaidList(result.RepairInformation.RepairCostPaid);

            // Jos statuses
            result.Status.InjectFrom(jobDetailsDto);
            result.Status.AllStatuses = Repository.GetTrackingStatuses();
            result.Status.IsJobCancelled = (jobDetailsDto.CurrentStatus == Settings.Default.JobClosedTrackingStatus);

            // Add job notes
            var jobNotes = Repository.GetJobNotes(serviceId);//, (Settings.Default.DeploymentTarget != "Clients")?0: StoreId);  // show all c/s notes for call center ; show only client notes to client 
            result.JobNotes = jobNotes.Select(noteDto =>
                new NoteRecordModel()
                {
                    DateTime = noteDto.DateTime,
                    From = noteDto.From,
                    Notes = noteDto.Notes,
                    Read= noteDto.Read,
                    ServiceId= noteDto.ServiceId
                    
                }).ToList();

            ////Additional Job Information
            result.JobAdditionalInfo.InjectFrom(jobDetailsDto);
           
            //FieldsFromDbService.GetAdditionalJobFields(serviceId, new List<FieldsFromDB>());
         //   result.JobAdditionalInfo.AEPFieldsFromDB.AepFields = FieldsFromDbService.GetAepFields(serviceId, result.JobAdditionalInfo.AEPFieldsFromDB.AepFields);
            result.JobAdditionalInfo.JobFields.AdditionalFields = FieldsFromDbService.GetAdditionalJobFields(serviceId, result.JobAdditionalInfo.JobFields.AdditionalFields);

       //     result.JobAdditionalInfo.IsAepType = DefaultValues.IsAepType(result.RepairInformation.RepairType);
            AccountService accService = new AccountService();
          
            result.JobAdditionalInfo.RetailerNameList = FillRetailerList(jobDetailsDto.RetailerId);//useraccount.UserStoreCountry);
            result.JobAdditionalInfo.RetailerName = result.JobAdditionalInfo.RetailerNameList.FirstOrDefault(x => x.Selected).Text.ToString();
            //result.Parts = GetPartsByServiceId(serviceId);
            result.Parts = GetPartsByServiceId(serviceId);
            result.PartsCost.callPartCostModel = Repository.RetrieveJobCost(serviceId);
            result.PartsCost.CallPartsCost = jobDetailsDto.CallPartsCost;// Math.Round((decimal)jobDetailsDto.CallPartsCost, 2);
            //result.InspectionData =templaterep. GetSpecificTemplateFieldsbyServiceid(serviceId);
            result.LinkedJobs = Repository.LinkedJobs(serviceId);
         result.partsCount=   Repository.Partcount(serviceId);
         result.JobClosed = jobDetailsDto.JobClosed;
            return result;
        }
        private List<SelectListItem> FillRetailerList(int RetailerId)
        {
            AccountService accService = new AccountService();
            var useraccount = accService.GetAccountDetails(UserId);
            return GetRetailerList(RetailerId.ToString());// String.IsNullOrEmpty(useraccount.UserStoreCountry)?"GB":useraccount.UserStoreCountry);//temp comment
        }

        /// <summary>
        /// Add note to job
        /// </summary>
        /// <param name="noteText"></param>
        public void AddNote(int serviceId, string noteText,string Visibility,string  communication,int NotesId=0)
        {
            Log.File.Info(Msg.GenerateLogMsg("Add job note...", "Service Id = " + serviceId));
            Repository.AddNote(serviceId, UserId, noteText,Visibility,  communication,NotesId);
        }
        public CallPartModel GetPartsByServiceId(int ServiceId)
        {
            CallPartModel result = new CallPartModel();
          // List<StockCallPartModel> stockparts
            List<CallPart> part = GetPartsByCallId(ServiceId);
            


            //List<CallPart> StockParts = part.Where(x => x.StatusID != "V").OfType<CallPart>().ToList();

            //List<CallPart> VANParts = part.Where(x => x.StatusID == "V").OfType<CallPart>().ToList();

            List<CallPart> StockParts = part.Where(x => x.StockPart).OfType<CallPart>().ToList();

            List<CallPart> VANParts = part.Where(x => !x.StockPart ).OfType<CallPart>().ToList(); 
           
            CallPartModel resultItem = new CallPartModel();
            foreach (CallPart item in StockParts)
            { StockCallPartModel partitem = new StockCallPartModel();
                partitem.InjectFrom(item);
                result.stockPart.Add(partitem);
               
            }
            foreach (CallPart item in VANParts)
            {VanCallPartModel vanpartitem = new VanCallPartModel();
                vanpartitem.InjectFrom(item);
                result.vanparts.Add(vanpartitem);
               
            }
            return result;
        }
        public List<CallPart> GetPartsByCallId(int ServiceId)
        {
            Log.File.Info(Msg.GenerateLogMsg("Fetch part details ...", "Service Id = " + ServiceId));
            JobPartsRepository partsRepository = new JobPartsRepository();
            try
            {
                var parts = partsRepository.GetPartsListByCallId(ServiceId);
                return parts;
            }
            catch (Exception ex)
            {
                Log.File.ErrorFormat(Msg.GenerateLogMsg("Error on Fetching part details ..."+ex.InnerException, "Service Id = " + ServiceId));
                return new List<CallPart>();
            }
            
        }

        //public void AddNoteCustomerEntity(int Customerid, int NotesId, string noteText)
        //{
        //  //  Log.File.Info(Msg.GenerateLogMsg("Add job note...", "Service Id = " + serviceId));
        //    Repository.AddNoteCustomerEntity(Customerid,NotesId, UserId, noteText);
        //}
        /// <summary>
        /// Update repair info
        /// </summary>
        /// <param name="model">Model data</param>
        internal void UpdateRepairInfo(Repair_InfoModel model)
        {
            model.RepairCost = model.RepairCost;// ?? 0.00;
            Repository.UpdateRepairInfo(model);
        }

        /// <summary>
        /// Get list of customer types
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetCustomerTypeList(string value)
        {
            var list = Repository.GetCustomerTypeList();
            var firstOrDefault = list.FirstOrDefault(x => x.Value == value);
            if (firstOrDefault != null)
                firstOrDefault.Selected = true;
            return list;
        }
       

        /// <summary>
        /// Get warranty list with selected valueSelectLis
        /// </summary>
        /// <param name="value">Selected value</param>
        /// <returns></returns>
        public List<SelectListItem> GetWarrantyList(int warrantyId)
        {
            var list = Repository.GetWarranties().Select(c => new SelectListItem { Value = c.WarrantyId.ToString(), Text = c.WarrantyText }).ToList();
            var firstOrDefault = list.FirstOrDefault(x => x.Value == warrantyId.ToString());
            if (firstOrDefault != null)
                firstOrDefault.Selected = true;
            return list;
        }

        /// <summary>
        /// Get list of types with selected value
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetJobTypesList(string selectedType)
        {
            var list = Repository.GetRepairTypeList().Select(c => new SelectListItem { Value = c.Visitcd.ToString(), Text = c.Desc }).ToList();
            var firstOrDefault = list.FirstOrDefault(x => x.Value == selectedType);
            if (firstOrDefault != null)
                firstOrDefault.Selected = true;
            return list;
        }

        /// <summary>
        /// Get condition codes
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetConditionIRISCodesList()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "", Value = "", Selected = true });
            list.AddRange(Repository.GetConditionIrisCodes().Select(c => new SelectListItem { Value = c.Code.ToString(), Text = c.Code.ToString() + " - " + c.Description }).ToList());

            return list;
        }
        public List<SelectListItem> GetConditionIRISCodesList(string value)
        {

            var list = GetConditionIRISCodesList();

            var firstOrDefault = list.FirstOrDefault(x => x.Value == value);
            if (firstOrDefault != null)
                firstOrDefault.Selected = true;
            return list;
        }
        /// <summary>
        /// Get condition codes
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetRetailerList()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "", Value = "0" ,Selected=true});
            var result = Repository.GetRetailerNameList();
            list.AddRange(result.Select(c => new SelectListItem { Value =c.supplierId.ToString(), Text =  c.supplier }).ToList());

            return list;
        }
       

        public List<SelectListItem> GetRetailerList(string value)
        {

            var list = GetRetailerList();
            if (value == "0")
            {
                list[0].Selected = true;

            }
            else
            {
                var firstOrDefault = list.FirstOrDefault(x => x.Value==value);
                if (firstOrDefault != null)
                {
                    var Default = list.FirstOrDefault(x => x.Selected == true);
                    if (Default != null)
                        Default.Selected = false;
                    firstOrDefault.Selected = true;
                }
            }
            return list;
        }
        public string GetRetailername(int value)
        {
            var retailer=Repository.GetRetailerNameList().FirstOrDefault(c => c.supplierId == value);
            return retailer.supplier;

        }

        public string GetRetailerdetails(string id)
        {
            if (id != "1")
            {
                var result = Repository.GetRetailerNameList();
                  var filter = result.FirstOrDefault(x => x.supplierId == int.Parse(id));
                return filter.Code;
            }
            else 
            {
                return "0000000000";

            }
           

        }
        /// <summary>
        /// Get symptom codes
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetSymptomIRISCodesList()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "", Value = "", Selected = true });
            list.AddRange(Repository.GetSymptomIrisCodes().Select(c => new SelectListItem { Value = c.Code.ToString(), Text = c.Code.ToString() + " - " + c.Description }).ToList());
            return list;
        }
        public List<SelectListItem> GetSymptomIRISCodesList(string value)
        {
            var list = GetSymptomIRISCodesList();

            var firstOrDefault = list.FirstOrDefault(x => x.Value == value);
            if (firstOrDefault != null)
                firstOrDefault.Selected = true;
            return list;
        }

        /// <summary>
        /// Get symptom codes
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetFileTypesCategoryList()
        {
            return Repository.GetFileTypeCategories().Select(c => new SelectListItem { Value = c.TypeId.ToString(), Text = c.TypeName }).ToList();
        }
        public List<SelectListItem> GetFileTypesCategoryList(string value)
        {
            var list = GetFileTypesCategoryList();

            var firstOrDefault = list.FirstOrDefault(x => x.Value == value);
            if (firstOrDefault != null)
                firstOrDefault.Selected = true;
            return list;
        }

        /// <summary>
        /// Get warranty details
        /// </summary>
        /// <param name="warrantyId">Warranty id</param>
        /// <returns></returns>
        public Guarantee_Model GetWarrantyDetails(int warrantyId)
        {
            return Repository.GetWarranties().FirstOrDefault(x => x.WarrantyId == warrantyId);
        }

        /// <summary>
        /// Get list of types
        /// </summary>
        /// <param name="answer"></param>
        /// <returns></returns>
        public List<SelectListItem> RepairCostPaidList(string answer)
        {
            // Fill list of titles
            var list = Repository.GetRepairCostPaidList().Select(x => new SelectListItem
            {
                Value = x.Key,
                Text = x.Value,
                Selected = x.Value == answer
            }).ToList();
            return list;
        }

        /// <summary>
        /// Update diary end and service status
        /// </summary>
        /// <param name="diaryEnt">diary ent</param>
        /// <returns></returns>
        public int UpdateDaryEnt(DieryEntInfoModel diaryEnt)
        {
            return Repository.UpdateDiaryEnt(diaryEnt);
        }

        /// <summary>
        /// Update Case id from 3C
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="caseid"></param>
        /// <returns></returns>
        public bool UpdateCaseId(int serviceId, string caseid)
        {
            return Repository.UpdateCaseId(serviceId, caseid);
        }

        /// <summary>
        /// Updating date of purchase
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="dateOfPurchase"></param>
        /// <returns></returns>
        public SaveServiceResult UpdateDateOfPurchase(int serviceId, DateTime dateOfPurchase)
        {
            return Repository.UpdateDateOfPurchase(serviceId, dateOfPurchase);
        }

        /// <summary>
        /// Guarantee information
        /// </summary>
        /// <param name="guaranteeID"></param>
        /// <returns></returns>
        public Guarantee_Model GetGuarantee(int guaranteeID)
        {
            return Repository.GetGuarantee(guaranteeID);
        }

        /// <summary>
        /// Cancel job
        /// </summary>
        /// <param name="reason"></param>
        //public void CancelJob(int serviceId, string reason)
        //{
        //    Log.File.Info(Msg.GenerateLogMsg("Cancel job...", "Service Id = " + serviceId));
        //    Repository.JobCancel(serviceId, reason, Settings.Default.JobClosedTrackingStatus, UserId);
        //}

        /// <summary>
        /// Add to database log about product viewed
        /// </summary>
        /// <param name="serviceId"></param>
        public void AddToDBLog_Viewed(int serviceId)
        {
            Log.Database.Job.Add.Viewed(serviceId);
        }

        public void AddToDBLog_Updated(int serviceId)
        {
            Log.Database.Job.Add.Updated(serviceId);
        }
        
        /// <summary>
        /// Get product list by search criteris
        /// </summary>
        /// <returns></returns>
        public Job_SearchModel GetJobsList(string searchCriteria, int pageNumber)
        {
            var result = new Job_SearchModel();
            result.SearchCriteria = searchCriteria;
            result.CurrentPage = pageNumber;

            if (!string.IsNullOrEmpty(searchCriteria))
            {
                //Logging
                Log.File.Info(Msg.GenerateLogMsg("Finding jobs... Criteria:", searchCriteria));

                // get products
                var list = Repository.FindJobs(searchCriteria, pageNumber, Settings.Default.JobSearchPageSize, StoreId);

                // if list empty
                if ((list != null) && (list.Count > 0))
                {
                    result.CurrentPage = pageNumber;
                    result.SearchResults = list;
                    result.FirstItemIndex = list[0].StartElem;
                    result.TotalRecords = list[0].ElemCount;
                    result.LastItemIndex = list[0].LastElem;
                }
            }
            return result;
        }


        public Job_SearchModel GetJobsList(AdvSearchCriteria model, int pageNumber)
        {
            var result = new Job_SearchModel();


            result.AdvSearchCriteria = model;
            result.CurrentPage = pageNumber;

            if (!string.IsNullOrEmpty(model.JobId) || !string.IsNullOrEmpty(model.Surname) || !string.IsNullOrEmpty(model.TelNo) || !string.IsNullOrEmpty(model.Postcode) || !string.IsNullOrEmpty(model.PolicyNumber) || !string.IsNullOrEmpty(model.ClientCustRef) || !string.IsNullOrEmpty(model.Address))
            {
                //Logging
                Log.File.Info(Msg.GenerateLogMsg("Finding jobs... Criteria:"));

                // get products
                var list = Repository.FindJobs(model.JobId,model.Surname,model.Postcode,model.TelNo,model.PolicyNumber,model.ClientCustRef,model.Address, pageNumber, Settings.Default.JobSearchPageSize, StoreId);

                // if list empty
                if ((list != null) && (list.Count > 0))
                {
                    result.CurrentPage = pageNumber;
                    result.SearchResults = list;
                    result.FirstItemIndex = list[0].StartElem;
                    result.TotalRecords = list[0].ElemCount;
                    result.LastItemIndex = list[0].LastElem;
                }
            }
            return result;
        }
        /// <summary>
        /// get not booked jobs
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        internal Job_SearchModel GetNotBookedJobList(int pageNumber)
        {
            var result = new Job_SearchModel();
            result.CurrentPage = pageNumber;

            //Logging
            Log.File.Info(Msg.GenerateLogMsg("Finding not booked jobs..."));

            // get products
            var list = Repository.FindNotBookedJobs(pageNumber, Settings.Default.JobSearchPageSize, StoreId);

            // if list empty
            if ((list != null) && (list.Count > 0))
            {
                result.CurrentPage = pageNumber;
                result.SearchResults = list;
                result.FirstItemIndex = list[0].StartElem;
                result.TotalRecords = list[0].ElemCount;
                result.LastItemIndex = list[0].LastElem;
                Log.File.Info(Msg.GenerateLogMsg("Finded not booked jobs...Count: " + list.Count.ToString()));
            }
            else
            {
                Log.File.Info(Msg.GenerateLogMsg("Finded not booked jobs...Count: 0"));
            }
            return result;
        }
        internal Job_SearchModel AdminGetNotBookedJobList(int pageNumber)
        {
            var result = new Job_SearchModel();
            result.CurrentPage = pageNumber;

            //Logging
            Log.File.Info(Msg.GenerateLogMsg("Finding not booked jobs..."));

            // get products
            var list = Repository.AdminFindNotBookedJobs(pageNumber, Settings.Default.JobSearchPageSize);

            // if list empty
            if ((list != null) && (list.Count > 0))
            {
                result.CurrentPage = pageNumber;
                result.SearchResults = list;
                result.FirstItemIndex = list[0].StartElem;
                result.TotalRecords = list[0].ElemCount;
                result.LastItemIndex = list[0].LastElem;
                Log.File.Info(Msg.GenerateLogMsg("Finded not booked jobs...Count: " + list.Count.ToString()));
            }
            else
            {
                Log.File.Info(Msg.GenerateLogMsg("Finded not booked jobs...Count: 0"));
            }
            return result;
        }

        /// <summary>
        /// Clear session
        /// </summary>
        public void ClearFromSession()
        {
            Session.Clear(SessionInfo);
        }






        //public RepeatRepairResult RepeatRepairCheck(string SerialNumber , string modelCode)
        //{
        //    RepeatRepairResult result = new RepeatRepairResult();
        //    OnlineBookingService onlineService = new OnlineBookingService();

        //    result = onlineService.RepeatRepairCheck(0, "", modelCode,SerialNumber, Settings.Default.RepeatHistoryCheckinDays);
        //   // result.RepeatDetectedFlag = true;
        //    if (result.RepeatDetectedFlag)
        //    {
        //        result.repeatHistory.HistoryResult = result.repeatHistory.HistoryResult.Replace('"', '\'');
        //    }
        //    return result;
        //}

        internal Job_SearchModel AwaitingForApprovalList(int pageNumber)
        {
            var result = new Job_SearchModel();
            result.CurrentPage = pageNumber;

            //Logging
            Log.File.Info(Msg.GenerateLogMsg("Finding not booked jobs..."));

            // get products
            var list = Repository.AwaitingForApprovalListJobs(pageNumber, Settings.Default.JobSearchPageSize);

            // if list empty
            if ((list != null) && (list.Count > 0))
            {
                result.CurrentPage = pageNumber;
                result.SearchResults = list;
                result.FirstItemIndex = list[0].StartElem;
                result.TotalRecords = list[0].ElemCount;
                result.LastItemIndex = list[0].LastElem;
                Log.File.Info(Msg.GenerateLogMsg("Found  jobs...Count: " + list.Count.ToString()));
            }
            else
            {
                Log.File.Info(Msg.GenerateLogMsg("not Found  jobs...Count: 0"));
            }
            return result;
        }

        internal Job_SearchModel RepeatedJobs(int pageNumber,int? Statusid )
        {
            var result = new Job_SearchModel();
            result.CurrentPage = pageNumber;

            //Logging
            Log.File.Info(Msg.GenerateLogMsg("Found not repeated jobs..."));

            // get products
            List<Job_SearchResult> list = new List<Job_SearchResult>();
            if (accountService.IsSuperAdmin)
             list = Repository.RepeatedJobs(pageNumber, Settings.Default.JobSearchPageSize, 0, Statusid);
            else
                list = Repository.RepeatedJobs(pageNumber, Settings.Default.JobSearchPageSize, StoreId, Statusid);
            // if list empty
            if ((list != null) && (list.Count > 0))
            {
                result.CurrentPage = pageNumber;
                result.SearchResults = list;
                result.FirstItemIndex = list[0].StartElem;
                result.TotalRecords = list[0].ElemCount;
                result.LastItemIndex = list[0].LastElem;
                Log.File.Info(Msg.GenerateLogMsg("Found  jobs...Count: " + list.Count.ToString()));
            }
            else
            {
                Log.File.Info(Msg.GenerateLogMsg("Finded  jobs...Count: 0"));
            }
            return result;
        }



        public  void UpdateJobSONYStatus(int serviceid )
        {
            Repository.UpdateJobSonyStatus(serviceid, 0);
        }

        internal Job_SearchModel ApprovedJobs(int pageNumber)
        {
            var result = new Job_SearchModel();
            result.CurrentPage = pageNumber;

            //Logging
            Log.File.Info(Msg.GenerateLogMsg("Found Approved repeated jobs..."));

            // get products
            List<Job_SearchResult> list = new List<Job_SearchResult>();
            if (accountService.IsSuperAdmin)
                list = Repository.ApprovedJobs(pageNumber, Settings.Default.JobSearchPageSize, 0);
            else
                list = Repository.ApprovedJobs(pageNumber, Settings.Default.JobSearchPageSize, StoreId);
            // if list empty
            if ((list != null) && (list.Count > 0))
            {
                result.CurrentPage = pageNumber;
                result.SearchResults = list;
                result.FirstItemIndex = list[0].StartElem;
                result.TotalRecords = list[0].ElemCount;
                result.LastItemIndex = list[0].LastElem;
                Log.File.Info(Msg.GenerateLogMsg("Found  jobs...Count: " + list.Count.ToString()));
            }
            else
            {
                Log.File.Info(Msg.GenerateLogMsg("Finded  jobs...Count: 0"));
            }
            return result;
        }


        internal bool MarkCustomerNotesRead(int ServiceID)
        {
          return  Repository.MarkNotesRead(ServiceID, true);
        }

        public int CreateJobwithEngineer(ServiceModel model)
        {
            Log.File.Info(Msg.GenerateLogMsg("Saving job info..."));
            var serviceId = Repository.CreateJobwithEngineer(model);
            return serviceId;
        }
        public int CreateJobBackup(ServiceModel model,bool CustplFailed, bool CustomerFailed,string policyNo, string UserId,string error)
        {
            Log.File.Info(Msg.GenerateLogMsg("Saving job info..."));
            int serviceId = Repository.CreateJobBackup(model, CustplFailed, CustomerFailed, policyNo, UserId,error);
            return serviceId;
        }

        internal  void SaveAppointmentTrack(int Serviceid, DateTime FirstDateoffered, DateTime DateChosen)
        {
            Repository.SaveAppointmentTrack(Serviceid,  FirstDateoffered,  DateChosen);
        }
        //public void EnterErrorLog(string entity,string userid, string errormessage, string detailinput)
        //{
        //    Log.File.Info(Msg.GenerateLogMsg("Error log"));
        //    Repository.SaveErrorLog(entity, userid,  errormessage,  detailinput);

        //}





        internal  void UpdateSupplierID(int RetailerId, int CustAplid, bool failed)
        {
            Repository.UpdateSupplierID(RetailerId, CustAplid, failed);
        }

        public Job_SearchModel GetJobsListByClientId(AdvSearchCriteria model, int pageNumber)
        {
            var result = new Job_SearchModel();


            result.AdvSearchCriteria = model;
            result.CurrentPage = pageNumber;

            if (!string.IsNullOrEmpty(model.ServiceId) || !string.IsNullOrEmpty(model.Surname) || !string.IsNullOrEmpty(model.TelNo) || !string.IsNullOrEmpty(model.Postcode) || !string.IsNullOrEmpty(model.PolicyNumber) || !string.IsNullOrEmpty(model.ClientRef) || !string.IsNullOrEmpty(model.Address))
            {
                //Logging
                Log.File.Info(Msg.GenerateLogMsg("Finding jobs... Criteria:"));

                // get products
                var list = Repository.FindJobsByClientId(model.ServiceId, model.Surname, model.Postcode, model.TelNo, model.PolicyNumber, model.ClientRef, model.Address, model.UseAndInWhereCondition.Value, pageNumber, Settings.Default.JobSearchPageSize);

                // if list empty
                if ((list != null) && (list.Count > 0))
                {
                    result.CurrentPage = pageNumber;
                    result.SearchResults = list;
                    result.FirstItemIndex = list[0].StartElem;
                    result.TotalRecords = list[0].ElemCount;
                    result.LastItemIndex = list[0].LastElem;
                }
            }
            return result;
        }

        /// <summary>
        /// Returns:
        /// 0: serviceId exists and match given postCode
        /// 1: Error, serviceId does not exist
        /// 2: Error, postCode does not match post code for serviceId
        /// </summary>
        public int CustomerViewJob(int serviceId, string postCode)
        {
            return Repository.CustomerViewJob(serviceId, postCode);
        }
    }
}
