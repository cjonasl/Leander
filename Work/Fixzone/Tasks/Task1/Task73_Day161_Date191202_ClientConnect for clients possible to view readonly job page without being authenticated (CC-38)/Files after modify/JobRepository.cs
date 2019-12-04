using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Linq;
using System.Web.Mvc;
using ClientConnect.Configuration;
using ClientConnect.Jobs;
using ClientConnect.Models.Job;
using ClientConnect.ViewModels.Job;
using ClientConnect.Properties;
using ClientConnect.Services;
using ClientConnect.Models.CustomerProduct;

namespace ClientConnect.Repositories
{
    public class JobRepository : Repository
    {
        /// <summary>
        /// Initializes the instance
        /// </summary>
        public JobRepository()
        {
        }
        public int Partcount(int serviceid)
        {
            JobRepository repository = new JobRepository();
            int PartsCount = repository.GetPartsByCallId(serviceid);
            return PartsCount;
        }
        /// <summary>
        /// Customer types
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetCustomerTypeList()
        {
            var result = new List<SelectListItem>
                {
                    new SelectListItem {Selected = true, Text = "End User", Value = "1"},
                    new SelectListItem {Text = "Dealer", Value = "0"}
                };
            return result;
        }
        
        /// <summary>
        /// Get warranty list
        /// </summary>
        public List<SelectListItem> GetWarrantyList(int warrantyId)
        {
            // set selected value
            var list = GetWarranties().Select(c => new SelectListItem { Value = c.WarrantyId.ToString(), Text = c.WarrantyText }).ToList();
            var firstOrDefault = list.FirstOrDefault(x => x.Value == warrantyId.ToString());
            if (firstOrDefault != null)
                firstOrDefault.Selected = true;
            return list;
        }
        public List<Guarantee_Model> GetWarranties()
        {
            return Query<Guarantee_Model>("Retrieve_Warranties", CommandType.StoredProcedure).ToList();
        }
        public Guarantee_Model GetWarrantyDetails(int warrantyId)
        {
            return GetWarranties().FirstOrDefault(x => x.WarrantyId == warrantyId);
        }

        /// <summary>
        /// Execute store procedure 'UpdateJob', which update job info or create new
        /// </summary>
        /// <param name="model"> Model of data from pag  </param>
        /// <returns> Return Job ID  </returns>
        public int SaveServiceWithoutEngineer(ServiceInfoModel model)
        {
            var result = Query<int>("UpdateJob", parameters: new
                                {
                                    model.CustomerId,
                                    model.ModelId,
                                    model.SerialNumber,
                                    model.DateOfPurchase,
                                    model.DateOfBookRepairClick,
                                    model.RetailerName,
                                    model.ProofOfPurchase,
                                    model.RetailerReference,
                                    model.RetailedInvoiceDate,
                                    model.SelectedType,
                                    model.FaultDescr,
                                    model.UserID,
                                    model.EngineerId,
                                    model.ClientId,
                                    model.ServiceId,
                                    model.ClientRef,
                                    model.ModelNumber,
                                    model.ProductCategory,
                                    model.WarrantyFg,
                                    model.CustomerType,
                                    model.PolicyNumber,
                                    StatusID = model.StatusId,
                                    model.AgentReferenceNo,
                                    @SymptomIRISCode = model.symptomIRISCode,
                                    @ConditionIRISCode = model.conditionIRISCode,
                                    @RetailerId=model.RetailerId
                                }, commandType: CommandType.StoredProcedure).FirstOrDefault();
            return result;
        }


        /// <summary>
        /// Get condition IRIS codes
        /// </summary>
        /// <returns></returns>
        public List<ConditionIrisCodesModel> GetConditionIrisCodes()
        {
            return Query<ConditionIrisCodesModel>("Retrieve_ConditionIrisCodes", commandType: CommandType.StoredProcedure).ToList();
        }
        public List<RetailerModel> GetRetailerNameList()
        {
            return Query<RetailerModel>("Get_supplier", commandType: CommandType.StoredProcedure).ToList();
        }
        /// <summary>
        /// Get symptom IRIS codes
        /// </summary>
        /// <returns></returns>
        public List<SymptomIrisCodesModel> GetSymptomIrisCodes()
        {
            return Query<SymptomIrisCodesModel>("Retrieve_SymptomIrisCodes", commandType: CommandType.StoredProcedure).ToList();
        }

        /// <summary>
        /// Get symptom IRIS codes
        /// </summary>
        /// <returns></returns>
        public List<FileTypeCategoryModel> GetFileTypeCategories()
        {
            return Query<FileTypeCategoryModel>("Retrieve_FileTypeCategories", commandType: CommandType.StoredProcedure).ToList();
        }

        /// <summary>
        /// Retrieves list of repair cost answers
        /// </summary>
        /// <returns>Collection of (id, name) pair for contact methods</returns>
        public IEnumerable<KeyValuePair<string, string>> GetRepairCostPaidList()
        {
            yield return new KeyValuePair<string, string>("Y", "Yes");
            yield return new KeyValuePair<string, string>("N", "No");
        }

        /// <summary>
        /// Update diaryEnt table
        /// </summary>
        /// <param name="diaryEnt"></param>
        /// <returns></returns>
        public int UpdateDiaryEnt(DieryEntInfoModel diaryEnt)
        {
            return
                    Query<int>("Update_DieryEnt", new
                    {
                        ServiceID = diaryEnt.ServiceId,
                        CustomerID = diaryEnt.CustomerId,
                        ItemCondition = diaryEnt.ItemCondition,
                        UserID = diaryEnt.UserID,
                        EngineerId = diaryEnt.EngineerId,
                        IsUpdateStatus = diaryEnt.IsUpdateStatus,
                        StatusId = diaryEnt.StatusId
                    }, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        /// <summary>
        /// Update diaryEnt table
        /// </summary>
        /// <param name="diaryEnt"></param>
        /// <returns></returns>
        public int UpdateStatusId(int ServiceId, int StatusId)
        {
            Execute("Update_ServiceStatus", new
            {
                ServiceID =ServiceId,
                StatusId = StatusId
            }, commandType: CommandType.StoredProcedure);
            return ServiceId;
        }

        /// <summary>
        /// Update diaryEnt table
        /// </summary>
        /// <param name="diaryEnt"></param>
        /// <returns></returns>
        public bool UpdateCaseId(int ServiceId, string CaseId)
        {
            Query<int>("Update_CaseId", new
            {
                ServiceID = ServiceId,
                CaseId = CaseId
            }, CommandType.StoredProcedure).FirstOrDefault();

            return true;
        }

        /// <summary>
        /// Update uploaded file in database
        /// </summary>
        /// <param name="file"></param>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        public int UpdateAttachedFile(Models.UploadedFileModel file, int serviceId)
        {
            Binary fileBinary = new Binary(file.FileContent);
            return Query<int>("Update_UploadedFiles",new
                {
                    ServiceId = serviceId,
	                FileName = file.FileName,
	                FileLength = file.FileSize,
                    Content = fileBinary
                }, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        /// <summary>
        /// Update 
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="dateOfPurchase"></param>
        /// <returns></returns>
        public SaveServiceResult UpdateDateOfPurchase(int serviceId, DateTime dateOfPurchase)
        {
            var result = new SaveServiceResult();
            Execute("Update_DateOfPurchase",
                                                new { @ServiceId = serviceId, @DateOfPurchase = dateOfPurchase },
                                                commandType: CommandType.StoredProcedure);
            result.IsSuccess = true;
            return result;
        }

        
        /// <summary>
        /// Finds jobs according to passed search criteria
        /// </summary>
        /// <param name="searchCriteria">Text to be found in different job-related columns</param>
        /// <param name="pageNumber">Page number to retrieve</param>
        /// <param name="pageSize">Number of records on a page</param>
        /// <returns>Job search results data transfer object</returns>
        public List<Job_SearchResult> FindJobs(string searchCriteria, int pageNumber, int pageSize, int StoreId)
        {
            return Query<Job_SearchResult>("JobList", new { Criteria = searchCriteria, ReturnLines = pageSize, PageNumber = pageNumber, StoreId = StoreId },
                                                                   commandType: CommandType.StoredProcedure).ToList();
        }
        // Repository.FindJobs(model.Surname, model.Postcode, model.TelNo, model.PolicyNumber, model.ClientCustRef, model.Address, pageNumber, Settings.Default.JobSearchPageSize, StoreId);
        public List<Job_SearchResult> FindJobs(string jobid,string Surname, string Postcode, string TelNo, string PolicyNumber, string ClientCustRef, string Address, int pageNumber, int pageSize, int StoreId)
        {
            return Query<Job_SearchResult>("SearchJobs", new
            {
                Jobid = jobid == null ? string.Empty : jobid,
                Surname = Surname == null ? string.Empty : Surname,
                Postcode = Postcode == null ? string.Empty : Postcode,
                                                             TelNo = TelNo == null ? string.Empty :TelNo,
                                                             PolicyNumber = PolicyNumber == null ? string.Empty :PolicyNumber,
                                                             ClientCustRef = ClientCustRef == null ? string.Empty :ClientCustRef,
                                                             Address= Address == null ? string.Empty :Address,
                                                             ReturnLines = pageSize,
                                                             PageNumber = pageNumber,
                                                             StoreId = StoreId
            },
                                                                 commandType: CommandType.StoredProcedure).ToList();
        }
        /// <summary>
        /// Find not booked jobs
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="StoreId"></param>
        /// <returns></returns>
        public List<Job_SearchResult> FindNotBookedJobs(int pageNumber, int pageSize, int StoreId)
        {
            return Query<Job_SearchResult>("Retrieve_NotBookedJobs", new { ReturnLines = pageSize, PageNumber = pageNumber, StoreId },
                                                                 commandType: CommandType.StoredProcedure).ToList();
        }
        // admin
        public List<Job_SearchResult> AdminFindNotBookedJobs(int pageNumber, int pageSize)
        {
            return Query<Job_SearchResult>("Retrieve_AdminNotBookedJobs", new { ReturnLines = pageSize, PageNumber = pageNumber },
                                                                 commandType: CommandType.StoredProcedure).ToList();
        }
        /// <summary>
        /// Retrieves detailed information for the job
        /// </summary>
        /// <param name="jobId"> Job identifier </param>
        /// <param name="userId"> The user Id. </param>
        /// <returns> Data model with all required info </returns>
        public Job_DetailsDto GetJobDetails(int jobId, string userId)
        {
            return Query<Job_DetailsDto>("RetrieveJob", new { ServiceId = jobId, UserId = userId }, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

        public List<Job_DetailsDto> GetJobDetailsByCustomerAppliance(int custaplid)
        {
            
                return Query<Job_DetailsDto>("CLC_RetrieveJobByCustaplid", new { custaplid = custaplid }, commandType: CommandType.StoredProcedure)
                    .ToList();
            


        }

        /// <summary>
        /// Retrieves all job tracking statues
        /// </summary>
        /// <returns>Collection of tracking statuses</returns>
        public List<Job_TrackingStatus> GetTrackingStatuses()
        {
            return Query<Job_TrackingStatus>("TrackingStatusList", commandType: CommandType.StoredProcedure).ToList();
        }        

        /// <summary>
        /// Retrieves job notes for specified job
        /// </summary>        
        /// <param name="jobId">Job identifier</param>
        /// <returns>Collection of job note data transfer objects</returns>
        public List<Job_NoteDto> GetJobNotes(int jobId, int StoreId=0)
        {
            return Query<Job_NoteDto>("JobNotesList", new { ServiceID = jobId, StoreId = StoreId }, commandType: CommandType.StoredProcedure).ToList();
        }

        public List<Job_NoteDtoShop> GetJobNotesForShop(int customerId)
        {
            return Query<Job_NoteDtoShop>("JobNotesListShop", new { customerid = customerId }, commandType: CommandType.StoredProcedure).ToList();
        }

        /// <summary>
        /// Adds job note record
        /// </summary>
        /// <param name="jobId">Job identifier</param>
        /// <param name="source">Note source (from whom does it originate)</param>
        /// <param name="text">Note text</param>
        //public void AddNote(int jobId, string source, string text)
        //{
        //    if (!string.IsNullOrEmpty(text))
        //    {
        //        Execute("CreateJobNote", new
        //                                {
        //                                    ServiceID = jobId,
        //                                    UserID = source,
        //                                    Text = text
        //                                },
        //                                commandType: CommandType.StoredProcedure);
        //    }
        //}

        /// <summary>
        /// Cancel Job
        /// </summary>
        /// <param name="jobId"> Job Id </param>
        /// <param name="Reason"> Reson why job cancel </param>
        /// <param name="closeStatus"> The close Status. </param>
        //public void JobCancel(int? jobId, string Reason, int closeStatus, string userId)
        //{
        //    if (jobId > 0)
        //    {
        //        Execute("CancelJob", new
        //                                {
        //                                    ServiceId = jobId,
        //                                    Reason,
        //                                    CloseStatus = closeStatus,
        //                                    UserId = userId
        //                                },
        //                                commandType: CommandType.StoredProcedure);
        //    }
        //}

        /// <summary>
        /// Performs job tracking status update
        /// </summary>
        /// <param name="jobId">Job identifier</param>
        /// <param name="statusId">Status identifier</param>
        /// <param name="userId">User identifier</param>
        public void UpdateJobStatus(int jobId, int statusId, string userId)
        {
            Execute("UpdateJobStatus", new { ServiceID = jobId, TrackingStatusID = statusId, UserId = userId }, commandType: CommandType.StoredProcedure);
        }


        

        /// <summary>
        /// Guarantee information
        /// </summary>
        /// <param name="guaranteeID"></param>
        /// <returns></returns>
        public Guarantee_Model GetGuarantee(int guaranteeID)
        {

            return
                Query<Guarantee_Model>("Retrieve_Guarantee", new { GuaranteeId = guaranteeID },
                                                               commandType: CommandType.StoredProcedure).First();
        }

        /// <summary>
        /// Get list of repair types
        /// </summary>
        /// <returns>List of types</returns>
        public IEnumerable<Job_RepairTypesModel> GetRepairTypeList()
        {
            return Query<Job_RepairTypesModel>("GetRepairTypeList", commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Updates product info
        /// </summary>
        /// <param name="updateCommand">Update contact info command</param>
        public void UpdateRepairInfo(Repair_InfoModel model)
        {
            Execute("UpdateRepairInfo", new
                {
                    model.RepairType, model.FaultType, model.FaultDescription, model.RepairAgent,model.RepairCost,
                    model.RepairCostPaid, model.VisitCd, model.ServiceId, model.EngineerId, model.DiaryId,
                    model.CustomerType
                }, commandType: CommandType.StoredProcedure);
        }

        internal List<Job_SearchResult> AwaitingForApprovalListJobs(int pageNumber, int pageSize)
        {
            return Query<Job_SearchResult>("[Retrieve_AwaitingForApprovalJobs]", new { ReturnLines = pageSize, PageNumber = pageNumber},
                                                                 commandType: CommandType.StoredProcedure).ToList();
        }

        internal List<Job_SearchResult> RepeatedJobs(int pageNumber, int pageSize, int storeid, int? Statusid)
        {
            return Query<Job_SearchResult>("Retrieve_RepeatedJobs", new { ReturnLines = pageSize, PageNumber = pageNumber, @storeid = storeid, @Statusid = Statusid },
                                                                 commandType: CommandType.StoredProcedure).ToList();
        }
     
        internal void RejectJob(ServiceInfoModel model,string notes)
        {
            Execute("RejectJob", new
            {
               
                model.ServiceId,
                model.UserID,
                notes,
                Settings.Default.JobClosedTrackingStatus
            }, commandType: CommandType.StoredProcedure);
        }

        internal void UpdateJobSonyStatus(int ServiceId, int statusid)
        {
            Execute("UpdateJobSonyStatus", new
            {

                ServiceId,
                statusid,
               
                Settings.Default.JobClosedTrackingStatus
            }, commandType: CommandType.StoredProcedure);
        }

        internal List<Job_SearchResult> ApprovedJobs(int pageNumber, int pageSize, int storeid)
        {
            return Query<Job_SearchResult>("[Retrieve_ApprovedJobs]", new { ReturnLines = pageSize, PageNumber = pageNumber, @storeid = storeid},
                                                                  commandType: CommandType.StoredProcedure).ToList();
        }

        public void SetStatusID(ServiceInfoModel jobInfo)
        {
            Execute("SetStatusID", new
            {
                jobInfo.ServiceId,           
                jobInfo.StatusId, 
              SubStatusId=  jobInfo.SubStatus
            },
                                                                  commandType: CommandType.StoredProcedure); 
          // Addadd note and change the status of the job to high cost query
        }

        public List<JobStatusHistory> GetJobStatusHistory(int serviceId)
        {
            return Query<JobStatusHistory>("RetrieveJobStatusHistory", new { ServiceId = serviceId}, commandType: CommandType.StoredProcedure).ToList();
        }


        public void AddNote(int serviceId,string UserId, string noteText,string Visibility, string communication ,int NotesId=0 )
        {
            Execute("CreateJobNote", new
            {
                ServiceID = serviceId,
                NotesId =NotesId,
                UserID = UserId,
                Text = noteText,
                Visibility = Visibility,
                communication = communication
            },
                                       commandType: CommandType.StoredProcedure);
        }

        internal string GetEngineerSaediId(int ServiceId)
        {
            return Query<string>("RetrieveJobEngineerSaediid", new { ServiceId = ServiceId }, commandType: CommandType.StoredProcedure).FirstOrDefault();
        }

       

        internal bool MarkNotesRead(int ServiceID, bool CustomerNotes)
        {
            Execute("MarkNotesRead", new {ServiceID=ServiceID, CustomerNotes = CustomerNotes }, commandType: CommandType.StoredProcedure);
            return true;
        }

              internal int CreateJobwithEngineer(ServiceModel model)
        {
            return Query<int>("CreateJobwithEngineer", new { serviceid = model.ServiceID, customerid = model.CustomerID, DiaryId = model.DiaryID,
                                                                EngineerId = model.Engineerid,Custaplid= model.CustAplID , eventDate= model.EventDate , Fault= model.Reportfault ,clientid=model.ClientId, AuthNo= model.AuthNo,
                                                                VisitCode= model.VisitCode,Clientref= model.Clientref ,StatusId=model.StatusId,jobId =model.JobId,JobSequenceid=model.JobSequenceId },
                                                                  commandType: CommandType.StoredProcedure).FirstOrDefault();
       
        }
       

         internal void  SaveAppointmentTrack(int Serviceid,DateTime FirstDateoffered ,DateTime DateChosen)
        {
            Execute("SaveAppointmentTrack", new { serviceid = Serviceid, FirstDateoffered = FirstDateoffered,DateChosen= DateChosen },
                                                                  commandType: CommandType.StoredProcedure);
       
        }

      internal List<CallPartCostModelDetail> RetrieveJobCost(int serviceId)
      {
          return Query<CallPartCostModelDetail>("RetrieveJobCost", new { ServiceId = serviceId }, commandType: CommandType.StoredProcedure).ToList();
      }

      internal JobUserSessionInfo GetJobSessionInfo(int serviceId, string UserId)
      {
          return Query<JobUserSessionInfo>("GetJobSessionInfo", new { ServiceId = serviceId, UserId = UserId }, commandType: CommandType.StoredProcedure).FirstOrDefault();
      }

      internal void SaveErrorLog(string entity, string userid, string errormessage, string detailinput)
      {
          Execute("SaveAppointmentTrack", new { entity = entity, userid = userid, errormessage = errormessage,detailinput=detailinput },
                                                       commandType: CommandType.StoredProcedure);
      }

      internal int CreateJobBackup(ServiceModel model, bool CustplFailed, bool CustomerFailed, string PolicyNo,string userid,string error)
      {
          return Query<int>("CreateJobBackup", new
          {
              serviceid = model.ServiceID,
              customerid = model.CustomerID,
              DiaryId = model.DiaryID,
              EngineerId = model.Engineerid,
              Custaplid = model.CustAplID,
              eventDate = model.EventDate,
              Fault = model.Reportfault,
              clientid = model.ClientId,
              AuthNo = model.AuthNo,
              CustplFailed = CustplFailed,
              CustomerFailed = CustomerFailed,
              UserId = userid,
              PolicyNo = PolicyNo,
              Clientref = model.Clientref,
              VisitCode = model.VisitCode,
              statusid=model.StatusId,
              error=error
          }, commandType: CommandType.StoredProcedure).FirstOrDefault();
    
      }

      internal List<AccidentalDamageCA> GetAccidentalByCustomerAppliance(int custaplid)
      {
          return Query<AccidentalDamageCA>("Fetch_AccidentalDamagebyCustaplid", new { custaplid = custaplid }, commandType: CommandType.StoredProcedure).ToList(); 
      }



      internal List<LinkedJobs> LinkedJobs(int serviceId)
      {
          List<LinkedJobs> jobs = new List<LinkedJobs>();
          jobs = Query<LinkedJobs>("Fetch_LinkedJobsbyServiceid", new { serviceId = serviceId }, commandType: CommandType.StoredProcedure).ToList(); 

              return jobs;
      }

      internal int GetPartsByCallId(int serviceid)
      {
          var parts = Query<CallPart>("Fetch_PartsbyServiceid", new { serviceId = serviceid }, commandType: CommandType.StoredProcedure).ToList();
       return parts.Count();
      }

      internal void UpdateSupplierID(int RetailerId, int CustAplid,bool failed=false)
      {
          Execute("UpdateSupplierID", new { RetailerId = RetailerId, CustAplid = CustAplid, failed = failed },
                                                                  commandType: CommandType.StoredProcedure);
      }

      internal List<CallPart> GetPartsListByCallId(int ServiceId)
      {
          var parts = Query<CallPart>("GEtPartDetailsByCallid", new { serviceId = ServiceId }, commandType: CommandType.StoredProcedure).ToList();
          return parts;
      }

      public List<Job_SearchResult> FindJobsByClientId(string serviceId, string Surname, string Postcode, string TelNo, string PolicyNumber, string ClientRef, string Address, bool useAndInWhereCondition, int pageNumber, int pageSize)
      {
          int serviceIdAsInt;

          if (string.IsNullOrEmpty(serviceId) || !int.TryParse(serviceId.Trim(), out serviceIdAsInt))
          {
              serviceIdAsInt = 0;
          }

          AccountService accService = (AccountService)Ioc.Get<AccountService>();

          return Query<Job_SearchResult>("GetJobsByClientId", new
          {
              ClientId = accService.SessionInfo.ClientId,
              ServiceId = serviceIdAsInt,
              Surname = Surname == null ? string.Empty : Surname.Trim(),
              Postcode = Postcode == null ? string.Empty : Postcode.Trim(),
              TelNo = TelNo == null ? string.Empty : TelNo.Trim(),
              PolicyNumber = PolicyNumber == null ? string.Empty : PolicyNumber.Trim(),
              ClientRef = ClientRef == null ? string.Empty : ClientRef.Trim(),
              Address = Address == null ? string.Empty : Address.Trim(),
              UseAndInWhereCondition = useAndInWhereCondition,
              ReturnLines = pageSize,
              PageNumber = pageNumber
          },
          commandType: CommandType.StoredProcedure).ToList();
      }

      /// <summary>
      /// Returns:
      /// 0: serviceId exists and match given postCode
      /// 1: Error, serviceId does not exist
      /// 2: Error, postCode does not match post code for serviceId
      /// </summary>
      public int CustomerViewJob(int serviceId, string postCode)
      {
          return Query<int>("CustomerViewJob", new { ServiceId = serviceId, PostCode = postCode }, commandType: CommandType.StoredProcedure).FirstOrDefault();
      }
    }
    
}