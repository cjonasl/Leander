using System;
using System.Web;
using System.Collections.Generic;

using System.Web.Mvc;
using CAST.BookRepair;
using CAST.Controllers;
using CAST.Infrastructure;
using CAST.Products;
using CAST.Repositories;
using CAST.ViewModels.Address;
using CAST.ViewModels.BookRepair;
using CAST.ViewModels.Customer;
using CAST.ViewModels.EngravingPlaces;
using CAST.ViewModels.Job;
using CAST.ViewModels.Inspection;
using CAST.Models;
using CAST.Logging;

namespace CAST.Services
{
    public class BookRepairService
    {
        /// <summary>
        /// Product data access object
        /// </summary>
        private readonly BookRepairRepository _reporsitory;

        /// <summary>
        /// Represents _bookRepairState of bookRepair processes
        /// </summary>
        private BookRepairStateHolder _bookStateHolder;

        /// <summary>
        /// dataContext
        /// </summary>
        private DataContext _dataContext;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data"></param>
        public BookRepairService(DataContext data)
        {
            _dataContext = data;
            _reporsitory = new BookRepairRepository(data);
            _bookStateHolder = new BookRepairStateHolder();
        }


        #region GET INFO

        public BookRepair_CustomerModel GetCustomerInfo()
        {
            var result = new BookRepair_CustomerModel();
            var cust = new CustomerService(_dataContext);
            var model = cust.GetCustomerInfo();

            // Get contact method
            result.ContactMethod = cust.GetContactMethod();
            result.TitleName = model.TitleName;
            result.Forename = model.Forename;
            result.Surname = model.Surname;
            result.County = model.County;
            result.Town = model.Town;

            result.Addr1 = model.Addr1;
            result.Addr2 = model.Addr2;
            result.Addr3 = model.Addr3;
            result.ContactMethod = model.ContactMethod;
            result.MobileTel = model.MobileTel;
            result.LandlineTel = model.LandlineTel;
            result.Email = model.Email;
            result.Postcode = model.Postcode;

            if (!string.IsNullOrEmpty(model.MobileTel)) result.ContactMethod = ContactMethod.ContactMethod.SMS;
            else
                if (!string.IsNullOrEmpty(model.LandlineTel)) result.ContactMethod = ContactMethod.ContactMethod.Telephone;
                else
                    if (!string.IsNullOrEmpty(model.Email)) result.ContactMethod = ContactMethod.ContactMethod.Email;
            return result;
        }

        /// <summary>
        /// Get job info
        /// </summary>
        /// <returns>Return model</returns>
        public BookRepair_JobModel GetJobInfo()
        {
            var book = _bookStateHolder.Load();
            var Inspection = new InspectionService(_dataContext);
            var result = new BookRepair_JobModel();
            var store = new StoreService(_dataContext);

            // if mandatory fields are null and service id is exist, then load from DB
            if ((book.FaultDescr == null) && (book.DateOfPurchase == null) && (book.ServiceId.HasValue))
            {
                result = _reporsitory.GetAcceptingJobInfo(book.ServiceId ?? 0);

                //book.AcceptJobFlag = false;
                _bookStateHolder.UpdateFrom(book);
            }

            // if normal booking process, then get info from session
            //if (!book.AcceptJobFlag)
            else
            {
                var product = new ProductService(_dataContext);

                var prodInfo = product.GetGeneralInfoFromSession();

                result.SerialNumber = prodInfo.SerialNumber;
                result.ItemCondition = prodInfo.OriginalCondition;
                result.DateOfPurchase = prodInfo.DateOfPurchase;

                result.StoreNumber = book.StoreNumber;// == null ?store.GetStoreId().ToString(): book.StoreNumber;
                result.TillNumber = book.TillNumber;
                result.TransNumber = prodInfo.TransactionInfo;
                result.SelectedType = book.Type;
                result.FaultDescr = book.FaultDescr;
                result.EngineerId = book.EngineerId;
                result.DateOfPurchaseString = book.DateOfPurchaseString;
                result.AdditionalFields = book.FieldsForInspection == null ? Inspection.GetSpecificInspection() : book.FieldsForInspection; //prodInfo.Additionalfields
            }
            //  result.AdditionalFields = book.FieldsForInspection == null ? Inspection.GetSpecificInspection() : book.FieldsForInspection;
            result.Type = GetJobTypesList();
            //  result.AdditionalFields = Inspection.GetSpecificInspection();
            result.AppointmentDate = book.AppointmentDate;
            //result.StoreCollection = book.StoreCollection;
            result.OnlineBookingFailed = book.OnlineBookingFailed;
            var repairAgent = GetAgentRepairInfo();
            result.BookingUrl = repairAgent.BookingUrl;
            result.InHomeAvailable = repairAgent.InHomeAvailable;
            if(!string.IsNullOrEmpty(repairAgent.BookingUrl) )
                result.StoreCollection =book.StoreCollection;// !repairAgent.InHomeAvailable ? true : 
            return result;
        }

        /// <summary>
        /// Get agent repair info
        /// </summary>
        /// <returns></returns>
        public BookRepair_AgentInfoModel GetAgentRepairInfo()
        {
            var product = new ProductService(_dataContext);
            var result = _reporsitory.GetRepairAgentInfo(product.GetModelId());
            if (result == null) result = new BookRepair_AgentInfoModel();
            return result;
        }



        /// <summary>
        /// Get confirm informartion
        /// </summary>
        /// <returns></returns>
        public BookRepair_ConfirmInfoModel GetConfirmBookInfo()
        {
            // get product info
            var prodS = new ProductService(_dataContext);
            var product = prodS.GetGeneralInfo(prodS.GetModelId());
            //product.add
            //get repair info
            var custS = new CustomerService(_dataContext);

            var inspS = new InspectionService(_dataContext);
            var inspection = inspS.GetGeneralinspectionInfoFromSession();
            product.Additionalfields = inspection;
            var customer = custS.GetGeneralCustomerInfoFromSession();

            var result = new BookRepair_ConfirmInfoModel();

            // get agent info
            var agent = GetAgentRepairInfo();

            // set values
            result.AgentInfo.Address = agent.Address;
            result.AgentInfo.CollectionInfo = agent.CollectionInfo;
            result.AgentInfo.EngineerId = agent.EngineerId;
            result.AgentInfo.ExtraInfo = agent.ExtraInfo;
            result.AgentInfo.Name = agent.Name;
            result.AgentInfo.NextStep = agent.NextStep;
            result.AgentInfo.OpeningHours = agent.OpeningHours;
            result.AgentInfo.Postcode = agent.Postcode;
            result.AgentInfo.TelephoneNumber = agent.TelephoneNumber;
            result.AgentInfo.BookingUrl = agent.BookingUrl;
            result.AgentInfo.InHomeAvailable = agent.InHomeAvailable;
            // set customer info
            result.CustomerInfo.CustomerId = customer.CustomerId;
            result.CustomerInfo.Address1 = customer.Address1;
            result.CustomerInfo.Address2 = customer.Address2;
            result.CustomerInfo.Address3 = customer.Address3;
            result.CustomerInfo.CustomerName = customer.CustomerName;
            result.CustomerInfo.PostCode = customer.PostCode;

            // set product info
            result.ProductInfo.ItemNumber = product.ItemNumber;
            result.ProductInfo.Description = product.Description;
            result.ProductInfo.OriginalCondition = product.OriginalCondition;
            result.ProductInfo.SerialNumber = product.SerialNumber;
            result.ProductInfo.TransactionInfo = product.TransactionInfo;
            result.ProductInfo.DateOfPurchase = product.DateOfPurchase;
            result.ProductInfo.ItemCode = product.ItemCode;
            result.ProductInfo.Additionalfields = inspection;
            // update info in sssion
            var repairState = _bookStateHolder.Load();
            repairState.EngineerId = result.AgentInfo.EngineerId;
            // repairState.Appointmentdate = _bookStateHolder.;

            repairState.NextStep = result.AgentInfo.NextStep;
            _bookStateHolder.UpdateFrom(repairState);
            result.SelectedAppointment = repairState.AppointmentDate;
            return result;
        }

        /// <summary>
        /// Get addresses list
        /// </summary>
        /// <param name="addresses"></param>
        /// <returns></returns>
        public BookRepair_AddressModel GetAddresesList(Address_Model addresses)
        {
            var result = new BookRepair_AddressModel();

            // Fill organisations adresses list
            for (int index = 0; index < addresses.Address1.Count; index++)
            {

                // Add to list of addresses
                result.OrgAddressesList.Add(new SelectListItem
                                                {
                                                    Selected = index == 0,
                                                    Text = addresses.DepartmentList[index].Text + " " +
                                                           addresses.OrganizationList[index].Text + " " +
                                                           addresses.Address1[index].Text + " " +
                                                           addresses.Address2[index].Text + " " +
                                                           addresses.Address3[index].Text,
                                                    Value = index.ToString()
                                                });
            }

            result.Address1 = addresses.Address1;
            result.Address2 = addresses.Address2;
            result.Address3 = addresses.Address3;
            result.DepartmentList = addresses.DepartmentList;
            result.OrganizationList = addresses.OrganizationList;
            result.County = addresses.County;
            result.Town = addresses.Town;
            return result;
        }

        /// <summary>
        /// get service id from session
        /// </summary>
        /// <returns></returns>
        public int GetServiceIdFromSession()
        {
            var book = _bookStateHolder.Load();
            return book.ServiceId ?? 0;
        }

        public bool IsNeedCourierFileGenerate(int jobId)
        {
            return _reporsitory.IsNeedCourierFileGenerate(jobId);
        }

        /// <summary>
        /// Is need to show repair address
        /// </summary>
        /// <returns></returns>
        public bool IsRepairAddressShow()
        {
            var book = _bookStateHolder.Load();
            return _reporsitory.IsShowRepairAgentAddress(book.ServiceId ?? 0);
        }

        /// <summary>
        /// Return 'Next Step' text
        /// </summary>
        /// <returns></returns>
        public string GetNextStepText()
        {
            var book = _bookStateHolder.Load();
            return book.NextStep;
        }

        /// <summary>
        /// Get flag of accept job
        /// </summary>
        /// <returns></returns>
        public bool GetAcceptingJobFlag()
        {
            var book = _bookStateHolder.Load();
            return book.AcceptJobFlag;
        }

        /// <summary>
        /// Get flag of accept job
        /// </summary>
        /// <returns></returns>
        public bool GetTrackingNumberSuccess()
        {
            var book = _bookStateHolder.Load();
            return book.TrackingNumberSuccess;
        }
        /// <summary>
        /// Get flag of accept job
        /// </summary>
        /// <returns></returns>
        public void SetTrackingNumberSuccess(bool value)
        {
            var book = _bookStateHolder.Load();
            book.TrackingNumberSuccess = value;
            _bookStateHolder.UpdateFrom(book);
        }
        #endregion

        #region SAVE INFO

        public void SaveEngravingAnswers(SpecificEngravingPlaces_Model model)
        {
            var book = _bookStateHolder.Load();
            book.FieldsForEngraving = model.FieldsForEngraving;
            _bookStateHolder.UpdateFrom(book);
        }

        /// <summary>
        /// Set job info into session
        /// </summary>
        /// <param name="model"></param>
        public void SetJobInfo(BookRepair_JobModel model)
        {
            var product = new ProductService(_dataContext);
            var prodInfo = new Product_InfoModel();
            var book = _bookStateHolder.Load();
            // fill model for product info
            prodInfo.ItemNumber = product.GetModelNumber();
            prodInfo.Description = product.GetModelDescr();
            prodInfo.ModelBrand = product.GetModelBrand();
            prodInfo.OriginalCondition = model.ItemCondition;
            prodInfo.SerialNumber = model.SerialNumber;
            prodInfo.TransactionInfo = model.TransNumber;
            prodInfo.DateOfPurchase = model.DateOfPurchase ?? DateTime.Now;
            prodInfo.Additionalfields = model.AdditionalFields;
            // save product info in session
            product.SetGeneralProductInfoInSession(prodInfo);

            book.DateOfPurchase = model.DateOfPurchase;
            book.StoreNumber = model.StoreNumber;
            book.TillNumber = model.TillNumber;

            book.Type = model.SelectedType;
            book.FaultDescr = model.FaultDescr;
            book.DateOfPurchaseString = model.DateOfPurchaseString;
            book.EngineerId = model.EngineerId;
            book.FieldsForInspection = model.AdditionalFields;
            book.AppointmentDate = model.AppointmentDate;
            book.StoreCollection = model.StoreCollection;
            book.Slotid = model.Slotid;
            _bookStateHolder.UpdateFrom(book);
        }

        /// <summary>
        /// Save job into database
        /// </summary>
        /// <returns></returns>
        public int SaveJob()
        {
            var book = _bookStateHolder.Load();
            //if(book.AppointmentDate!="")
            
            var cust = new CustomerService(_dataContext);
            var prod = new ProductService(_dataContext);
            var user = new UserService(_dataContext);
            var store = new StoreService(_dataContext);
            var job = new BookRepair_JobModel();
            var quest = new QuestionService(_dataContext);
            var jobService = new JobService(_dataContext);

            var prodInfo = prod.GetGeneralInfoFromSession();
          var repairAgent =  GetAgentRepairInfo();
            job.DateOfPurchase = book.DateOfPurchase;
            job.SerialNumber = prodInfo.SerialNumber;
            job.ItemCondition = prodInfo.OriginalCondition;
            job.DateOfPurchase = prodInfo.DateOfPurchase;
            job.StoreNumber = book.StoreNumber;
            job.TillNumber = book.TillNumber;
            job.TransNumber = prodInfo.TransactionInfo;
            job.SelectedType = book.Type;
            job.FaultDescr = book.FaultDescr + " \n" + quest.GetAnswersFromSession();
            job.UserID = user.GetUserId();
            job.EngineerId = book.EngineerId;
            job.AppointmentDate = book.AppointmentDate;
            job.StoreCollection =   book.StoreCollection;
            SetAcceptJobFlag(false);
            int custId =cust.GetCustomerIdFromSession();
            var customer = cust.GetCustomerInfo();
            int Jobid= _reporsitory.UpdateJob(job, custId, prod.GetModelId(), (DateTime)prod.GetTimeBookRepairClick(), book.ServiceId, store.GetStoreId());
            if (!string.IsNullOrEmpty(repairAgent.BookingUrl))
            {
                OnlineBookingService onlineBookingService = new OnlineBookingService();
                OnlineBookRequestDetails model = new OnlineBookRequestDetails();
                // model.InjectFrom(ProductService.SessionInfo);
                model.ServiceID =  Jobid;
              model.CustomerTitle= customer.TitleName;
                model.CustomerForename = customer.Forename;
                model.CustomerSurname= customer.Surname ;
                model.CustomerStreet = customer.Addr1;
                model.CustomerPostcode = customer.Postcode;
                model.CustomerAddressTown = customer.Town ;
                model.CustomerWorkTelNo =  customer.LandlineTel ;
                model.CustomerMobileNo = customer.MobileTel ;
                model.CustomerEmailAddress = customer.Email ;
                model.SupplyDat = prodInfo.DateOfPurchase.ToString("yyyy-MM-dd");
                //model.CustAplID = prodInfo.CustaplId;
                model.ApplianceCD ="";
                //model.Model = prodInfo.ItemNumber;   
                //model.AltCode = prodInfo.ItemCode;
                // cast online booking - skyline  Argos Cat No (MODEL) should go into Skyline ProductCode field
//Argos Brand Model (ALTCODE)  should go into Skyline ModelNo field

                model.Model =  prodInfo.ItemCode;
                model.AltCode = prodInfo.ItemNumber;
                model.SNO = job.SerialNumber;
                model.MFR = prodInfo.ModelBrand;
                
                //model.PolicyNumber =prodInfo.p
                model.ReportFault = job.FaultDescr;
                model.ClientID =  store.GetStoreId();
                model.StatusID = 4;
               
              //  model.VisitDate = DateTime.Parse(job.AppointmentDate);
              //  model.EngineerID = job.EngineerId.Value;
                model.SlotID = book.Slotid;
             //   model.CallType = job.StoreCollection ? "1 Store Collection" : "2 In Home Repair";//repairAgent.InHomeAvailable?"1 Store Collection":( job.StoreCollection ? "1 Store Collection" : "2 In Home Repair");
                model.CallType = !repairAgent.InHomeAvailable ? "1 Store Collection":( job.StoreCollection ? "1 Store Collection" : "2 In Home Repair");
                var response = onlineBookingService.BookJob(model);
                  Log.File.ErrorFormat("Error {0}: {1}. ", response.ErrorMsg, response.BookSuccessfully);
                               
              

                if (response.BookSuccessfully)
                {

                    _reporsitory.UpdateJobClientRef(Jobid, response.ServiceID);

                    HttpContext.Current.Session["ClientRef"] = response.ServiceID;

                }
                else
                    book.OnlineBookingFailed = true;

            }
            if (book.FieldsForInspection!=null && book.FieldsForInspection.Count > 0)
            {
                InspectionRepository ins = new InspectionRepository(_dataContext);
                book.FieldsForInspection.ForEach(s => s.ServiceId = Jobid);
                ins.SaveSpecificInspection(book.FieldsForInspection);
            }
            return Jobid;
        }

        /// <summary>
        /// Set service id
        /// </summary>
        /// <returns></returns>
        public void SetServiceIdIntoSession(int serviceId)
        {
            var book = _bookStateHolder.Load();
            book.ServiceId = serviceId;
            _bookStateHolder.UpdateFrom(book);
        }

        /// <summary>
        /// Add record in table for courier file generation
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public int AddRecordForCourierFileGeneration(int jobId)
        {
            var store = new StoreService(_dataContext);
            return _reporsitory.AddRecordForCourierFileGeneration(store.GetStoreId(), jobId);
        }

        /// <summary>
        /// Add record in table for courier file generation
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public bool IsTrackingNumberUniq(int jobId)
        {
            var trackingNumber = _reporsitory.CheckTrackingNumber(jobId);
            return !string.IsNullOrEmpty(trackingNumber);
        }

        /// <summary>
        /// Set flag is accepting job process
        /// </summary>
        /// <param name="flag"></param>
        public void SetAcceptJobFlag(bool flag)
        {
            var book = _bookStateHolder.Load();
            book.AcceptJobFlag = flag;
            _bookStateHolder.UpdateFrom(book);
        }

        /// <summary>
        /// Update repair info 
        /// </summary>
        /// <param name="model"></param>
        public void UpdateRepairInfo(RepairInfoModel model)
        {
            model.RepairCost = model.RepairCost ?? 0.00;
            _reporsitory.UpdateRepairInfo(new UpdaterepairInfoCommand
            {
                FaultDescription = model.FaultDescription,
                FaultType = model.FaultType,
                RepairAgent = model.RepairAgent,
                RepairCost = model.RepairCost,
                RepairCostPaid = model.RepairCostPaid,
                RepairType = model.RepairType,
                DiaryId = model.DiaryId,
                EngineerId = model.EngineerId,
                ServiceId = model.ServiceId,
                VisitCd = model.VisitCd
            });
        }
        #endregion

        #region FILL LISTS



        /// <summary>
        /// Get list of answers
        /// </summary>
        /// <returns>List of answers</returns>
        public List<SelectListItem> GetShareAnswersList()
        {
            // Get answer list
            return _reporsitory.GetShareAnswerList();
        }
        public List<SelectListItem> StoreCollectionAnswerList()
        {
            // Get answer list
            return _reporsitory.StoreCollectionAnswerList();
        }
        
        public SelectList GetJobTypesList()
        {
            var book = _bookStateHolder.Load();
            return new SelectList(_reporsitory.GetRepairTypeList(), "Visitcd", "Desc", book.Type);
        }

        #endregion

        public void UpdateCustomer(BookRepair_CustomerModel model)
        {
            // method for customer update
            var user = new UserService(_dataContext);
            var store = new StoreService(_dataContext);
            var cust = new CustomerService(_dataContext);
            var func = new FunctionsController();
            model.Forename = func.UppercaseFirst(model.Forename);
            model.Surname = func.UppercaseFirst(model.Surname);

            // Save in database
            var customerId = _reporsitory.UpdateCustomer(model, cust.GetCustomerIdFromSession(), store.GetStoreId(), user.GetUserId());

            // Fill class by info
            cust.SetGeneralCustomerInfoIntoSession(new Customer_InfoModel
                                                            {
                                                                Address1 = model.HouseNumber + " " + model.Addr1 + "," + model.Organization,
                                                                Address2 = model.Addr2,
                                                                Address3 = model.Addr3,
                                                                CustomerName = model.Forename + " " + model.Surname,
                                                                PostCode = model.Postcode,
                                                                CustomerId = customerId,
                                                                ContactMethod = model.ContactMethod
                                                            });
            // Update session holder
            //_bookStateHolder.UpdateFrom(_bookState);
        }

        /// <summary>
        /// Check validation of contact methods
        /// </summary>
        /// <param name="model">Model of data</param>
        /// <returns>Dictionary (ElementId, Error message)</returns>
        public Dictionary<string, string> CheckContactMethods(BookRepair_CustomerModel model)
        {
            // Contact methods check
            if (string.IsNullOrEmpty(model.MobileTel) && model.ContactMethod == ContactMethod.ContactMethod.SMS)
                return new Dictionary<string, string>() { { "MobileTel", "Input telephone" } };

            if (string.IsNullOrEmpty(model.LandlineTel) && model.ContactMethod == ContactMethod.ContactMethod.Telephone)
                return new Dictionary<string, string>() { { "LandlineTel", "Input telephone" } };

            if (string.IsNullOrEmpty(model.Email) && model.ContactMethod == ContactMethod.ContactMethod.Email)
                return new Dictionary<string, string>() { { "Email", "Input email" } };

            return null;
        }

        /// <summary>
        /// Clear info from session
        /// </summary>
        public void ClearInfoFromSession()
        {
            _bookStateHolder.Clear();
        }



    }
}