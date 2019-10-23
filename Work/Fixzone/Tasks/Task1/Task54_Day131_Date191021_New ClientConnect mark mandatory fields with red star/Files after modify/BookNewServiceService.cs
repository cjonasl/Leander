using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ClientConnect.Customer;
using ClientConnect.Infrastructure;
using ClientConnect.Logging;
using ClientConnect.Models.Aep;
using ClientConnect.Models.BookNewService;
using ClientConnect.Models.Job;
using ClientConnect.Models._3C.Send;
using ClientConnect.ViewModels.BookNewService;
using ClientConnect.ViewModels.FieldsFromDb;
//using NUnit.Framework;
using Omu.ValueInjecter;
using Newtonsoft.Json;

namespace ClientConnect.Services
{
    public class BookNewServiceService : Service, IService
    {
       
        private CustomerService CustomerService { get; set; }
     
        private JobService JobService { get; set; }
        private ProductService ProductService { get; set; }
        private EngineerService EngineerService { get; set; }
        private FieldsFromDBService FieldsFromDbService { get; set; }
        private OnlineBookingService onlineBookingService { get; set; }
        private AccountService accountService { get; set; }
        /// <summary>
        /// Session info
        /// </summary>
        public BookNewServiceSessionModel SessionInfo
        {
            get { return Session.Load(new BookNewServiceSessionModel()); }
        }


        public BookNewServiceService(CustomerService customerService, JobService jobService, ProductService productService, EngineerService engineerService, FieldsFromDBService fieldsFromDbService)
        {
            CustomerService = customerService;
            
            JobService = jobService;
            ProductService = productService;
            EngineerService = engineerService;
            FieldsFromDbService = fieldsFromDbService;
        }

        /// <summary>
        /// get customer info
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public CustomerPageModel GetCustomerDetails(int customerId)
        {
            var result = new CustomerPageModel();
            result.InjectFrom(CustomerService.GetCustomerInfo(customerId));
            // fill lists
            result.TitleList = CustomerService.GetTitlesList(result.Title);
            result.ContactMethodList = CustomerService.GetContactMethodList(result.ContactMethod.ToString());
            result.CountryList = CustomerService.GetCountryList(result.Country);
            return result;
        }
        /// <summary>
        /// save customer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public int SaveCustomer<T>(T model)
        {
            var customer = new CustomerModel();
            customer.InjectFrom(model);
            return CustomerService.SaveCustomer(customer);
        }

        // create a customer using fzonlinebooking and create the customer account in client connect database.
        public int CreateCustomer<T>(T model, string notes = "")
        {
            onlineBookingService = new OnlineBookingService();
            var customer = new CustomerModel();
            customer.InjectFrom(model);

            OnlineBookCustomerDetails onlineCustomer = new OnlineBookCustomerDetails();
            onlineCustomer.InjectFrom(model);
            onlineCustomer.ClientCustRef = customer.CLIENTCUSTREF;
       
            onlineCustomer.ClientID = int.Parse(customer.ClientId); 
            onlineCustomer.RetailClientId = customer.RetailClient;
            onlineCustomer.PreferredContactMethod = customer.ContactMethod;
          //  onlineCustomer.CustomerID = customer.CustomerId;
            if (notes !="")
            {
                onlineCustomer.CustomerNotes = notes;
            }
            //onlineCustomer.ClientCustRef =  customer.CLIENTCUSTREF
            //  OnlineBookCustomerDetailsResponse response = new OnlineBookCustomerDetailsResponse();
            OnlineBookCustomerDetailsResponse result = onlineBookingService.SaveCustomer(onlineCustomer);
            //  response.CustomerResponse.InjectFrom(result);

            // if (result.CustomerResponse.CustomerID != 0)
            //     customer.CustomerId = result.CustomerResponse.CustomerID;

            //return CustomerService.SaveCustomer(customer);
            Log.File.Info(Msg.GenerateLogMsg("Creating customer"));
            Log.File.InfoFormat("Input :{0}", JsonConvert.SerializeObject(onlineCustomer));
            Log.File.InfoFormat("Output :{0}", JsonConvert.SerializeObject(result));
            return result.CustomerResponse.CustomerID;
        }

       
        /// <summary>
        /// get job info
        /// </summary>
        /// <param name="sessionInfo"></param>
        /// <returns></returns>
        public JobPageModel GetJobInfo(BookNewServiceSessionModel sessionInfo)
        {
            var result = new JobPageModel();
            result.InjectFrom(sessionInfo);
            result.AdditionalFields.AdditionalFields = Functions.ConvertListToNewListType<FieldsFromDB, FieldsFromDBSessionModel>(sessionInfo.AdditionalFields);
            if (result.DateOfPurchase.Year > 1900)
            {
                result.DateOfPurchaseString = Functions.DateTimeToString(result.DateOfPurchase);
            }
            return result;
        }

        /// <summary>
        /// get model for additional answers
        /// </summary>
        /// <returns></returns>
        public List<FieldsFromDB> GetAdditionalAnswersModel<T>(List<T> additionalFields)
        {

            var answers = new List<FieldsFromDB>();
            foreach (var field in additionalFields)
            {
                var answer = new FieldsFromDB();
                answer.InjectFrom(field);
                answers.Add(answer);
            }
            return answers;
        }

        /// <summary>
        /// get model for additional answers
        /// </summary>
        /// <param name="sessionInfo"></param>
        /// <returns></returns>
        public JobPageModel FillJobPageLists(JobPageModel model, BookNewServiceSessionModel sessionInfo)
        {
            //fill lists
            model.AdditionalFields.AdditionalFields = FieldsFromDbService.GetAdditionalJobFields(sessionInfo.ServiceId, GetAdditionalAnswersModel(sessionInfo.AdditionalFields));
            model.ProductCategory = ProductService.GetProductCategory(sessionInfo.ModelId);
            model.CustomerTypeList = JobService.GetCustomerTypeList(model.CustomerType.ToString());
            AccountService accService = new AccountService();
            var useraccount=accService.GetAccountDetails(UserId);
            model.RetailerNameList = JobService.GetRetailerList(model.RetailerId.ToString());
            model.RetailerName = sessionInfo.RetailerName;
            model.WarrantyList = JobService.GetWarrantyList(model.WarrantyFg);
            model.ServiceTypesList = JobService.GetJobTypesList(model.SelectedType);
            model.conditionIRISCodesList = JobService.GetConditionIRISCodesList(model.conditionIRISCode);
            model.symptomIRISCodesList = JobService.GetSymptomIRISCodesList(model.symptomIRISCode);
            model.FileTypeCategoryList = JobService.GetFileTypesCategoryList();
            return model;
        }

        /// <summary>
        /// dill model with info from 3C service
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JobPageModel FillModelWithUnitInfo(JobPageModel model, GetUnitInfoResultModel unitInfo)
        {
            model.DateOfPurchase = unitInfo.purchaseDate ?? model.DateOfPurchase;
            model.DateOfPurchaseString = Functions.DateTimeToString(model.DateOfPurchase);
            model.DateOfPurchaseAccepted = unitInfo.purchaseDateAccepted;
            model.AdditionalFields.WarrantyEndDate.SelectedAnswer =
                Functions.DateTimeToString(unitInfo.warrantyEndDate ?? new DateTime());
            model.AdditionalFields.AepType.SelectedAnswer = unitInfo.aepType;
            model.AdditionalFields.SerialNumberStatus.SelectedAnswer = unitInfo.serialNumberStatus;
            model.AdditionalFields.SalesType.SelectedAnswer = unitInfo.productSalesType;
            model.PolicyNumber = unitInfo.premiumServicesInfo != null ? unitInfo.premiumServicesInfo.premiumServiceReference : model.PolicyNumber;

            //save in session model
            SessionInfo.InjectFrom(model);
            SessionInfo.AdditionalFields =
                Functions.ConvertListToNewListType<FieldsFromDBSessionModel, FieldsFromDB>(model.AdditionalFields.AdditionalFields);
            return model;
        }

        /// <summary>
        /// Update Aep type in databse with values from Sony 3C
        /// </summary>
        /// <param name="aepType"></param>
        /// <returns></returns>
        public bool UpdateAepInfo(int modelId, string aepType)
        {
            if (IsAepProduct(aepType))
            {
                ProductService.UpdateAepInfo(modelId, "Y");
                return true;
            }
            ProductService.UpdateAepInfo(modelId, "");
            return true;
        }

        /// <summary>
        /// Is aep or not by 
        /// </summary>
        /// <param name="aepType"></param>
        /// <returns></returns>
        public bool IsAepProduct(string aepType)
        {

            return aepType.ToUpper().Equals("B2B") || aepType.ToUpper().Equals("B2C");
        }

        /// <summary>
        /// Create service record 
        /// </summary>
        /// <returns></returns>
        public SaveServiceResult CreateServiceWithoutDiaryEnt()
        {
            var result = new SaveServiceResult();
            try
            {
                //save job in database
                var jobInfo = new ServiceInfoModel();
                jobInfo.InjectFrom(SessionInfo);
                jobInfo.ClientId = StoreId;
                jobInfo.UserID = UserId;
                var additionalInfo = new AdditionalFieldsModel();
                additionalInfo.AdditionalFields = Functions.ConvertListToNewListType<FieldsFromDB, FieldsFromDBSessionModel>(SessionInfo.AdditionalFields);
                jobInfo.symptomIRISCode = additionalInfo.SymptomIRISCode.SelectedAnswer;
                jobInfo.conditionIRISCode = additionalInfo.ConditionIRISCode.SelectedAnswer;

                //create job in database without DiaryEnt
                Log.File.Info(Msg.GenerateLogMsg("Creating new service without DiaryEnt..."));
                var newServiceId = (!SessionInfo.RepeatDetected)?JobService.CreateJobWithoutEngineer(jobInfo):JobService.CreateRepeatedJobWithoutEngineer(jobInfo);

                //save additiola fields
                var additionalFields =
                    Functions.ConvertListToNewListType<FieldsFromDB, FieldsFromDBSessionModel>(
                        SessionInfo.AdditionalFields);
                FieldsFromDbService.SaveFields(newServiceId, additionalFields);

                //save aep fields
                if (SessionInfo.AepInfo.IsAepAviable)
                {
                    var aepFields =
                    Functions.ConvertListToNewListType<FieldsFromDB, AepSessionResultModel>(
                        SessionInfo.AepInfo.AepFields);
                    FieldsFromDbService.SaveFields(newServiceId, aepFields,true);
                }

                //if file uploaded then save into database
                JobService.UpdateAttachedFile(SessionInfo.UploadedFile, newServiceId);

                // notes for job
                if (newServiceId > 0)
                {
                    if (newServiceId != SessionInfo.ServiceId)
                    {
                        // Add log record in ServiceUsage
                        Log.Database.Job.Add.Inserted(newServiceId);

                        // Add notes to the job
                        JobService.AddNote(newServiceId, "The job is created.","*","C");
                    }
                }

                Log.File.Info(Msg.GenerateLogMsg("Created new service without DiaryEnt. ServiceId", newServiceId));

                //set flag new service or not
                if ((newServiceId > 0) && (SessionInfo.ServiceId != newServiceId))
                {
                    SessionInfo.IsNewService = true;
                }
                SessionInfo.ServiceId = newServiceId;
                result.IsSuccess = true;

            }
            catch (Exception e)
            {
                Log.File.Error(Msg.GenerateLogMsg("Error in CreateServiceWithoutDiaryEnt module...Error", e.Message));
                result.IsSuccess = false;
                result.ErrorMessage = e.Message;
            }
            return result;
        }


        /// <summary>jobInfo.DateOfPurchase
        /// Generate 3C model
        /// </summary>
        /// <returns></returns>
        public Registration_SendModelResult Generate3CModel()
        {
            var result = new Registration_SendModelResult();
            var jobInfo = GetJobInfo(SessionInfo);
            string retailerinfo = GetRetailerInfo(jobInfo.RetailerId.ToString());
            var customer = GetCustomerDetails(SessionInfo.CustomerId);
            var engineer = EngineerService.GetEngineerDetailsFor3C(SessionInfo.EngineerId, SessionInfo.ModelId);
            var phonePrefix = GetPhonePrefix(customer.Country);

            // if no Engineer that return empty result
            if (engineer == null)
            {
                result.HasEngineer = false;
                return result;
            }

            result.EngineerCurrency = engineer.EngineerCurrency;

            //info
            result.info.additionalInfo = string.Empty;
            result.info.doaIsPreSales = false;
            result.info.eventStartDate = DateTime.UtcNow.Date;
            result.info.eventStartDateSpecified = true;
            if (SessionInfo.SelectedType == "000")
            {
                result.info.logisticsType = "";
                result.info.serviceEventType = "IHREP";
            }
            if (SessionInfo.SelectedType == "001")
            {
                result.info.logisticsType = "0";
                result.info.serviceEventType = "DEPREP";
            }
            if (SessionInfo.SelectedType == "002")
            {
                result.info.logisticsType = "0";
                result.info.serviceEventType = "DEPREP";
            }

            if (SessionInfo.SelectedType == "003")
            {
                result.info.logisticsType = "0";
                result.info.serviceEventType = "DOASCR";
            }

            if (SessionInfo.SelectedType == "004")
            {
                result.info.logisticsType = "0";
                result.info.serviceEventType = "LOWCST";
            }

            if (SessionInfo.SelectedType == "005")
            {
                result.info.logisticsType = "0";
                result.info.serviceEventType = "REFURB";
            }

            if (SessionInfo.SelectedType == "097")
            {
                result.info.logisticsType = "";
                result.info.serviceEventType = "AEPB2BIH";
                result.HasModelAEP = true;
            }

            if (SessionInfo.SelectedType == "098")
            {
                result.info.logisticsType = "0";
                result.info.serviceEventType = "AEPB2BDEP";
                result.HasModelAEP = true;
            }

            if (SessionInfo.SelectedType == DefaultValues.AepCode)
            {
                result.info.logisticsType = "";
                result.info.serviceEventType = "AEPB2BIH";
                result.HasModelAEP = true;
            }
            jobInfo.ServiceId = jobInfo.ServiceId == null ? 0 : jobInfo.ServiceId;
            result.info.mainAscReferenceId = jobInfo.ServiceId.ToString();
            result.info.subAscId = engineer.EngineerSubAscId;
            result.info.subAscReferenceId = "";                 //SAEDICalls.ClaimNumber -- SAEDICalls.CustomerId = customer.CUSTAPLID
            result.info.aepBookingReference = SessionInfo.AepBookingReference > 0 ? SessionInfo.AepBookingReference.ToString() : string.Empty;
            //unitInfo
            var model = ProductService.GetDetails(SessionInfo.ModelId);
            result.unitInfo.arrivedAtDealer = DateTime.UtcNow.Date;
            result.unitInfo.arrivedAtDealerSpecified = true;
            result.unitInfo.hasPhysicalDamage = jobInfo.AdditionalFields.PhysicalDamageSuspected.SelectedAnswer == "Yes" ? 1 : 0;              //read from other field
            result.unitInfo.modelCode = string.Empty; //without modelcod for now model.ItemCode;
            result.unitInfo.modelName = model.ItemCode;
            result.unitInfo.premiumServiceReference = jobInfo.PolicyNumber;
            result.unitInfo.purchaseDate = jobInfo.DateOfPurchase;
            result.unitInfo.serialNumber = jobInfo.SerialNumber;
            result.info.subAscId = engineer.EngineerSubAscId;
            result.unitInfo.symptomIrisCode = jobInfo.AdditionalFields.SymptomIRISCode.SelectedAnswer;
            result.unitInfo.conditionIrisCode = jobInfo.AdditionalFields.ConditionIRISCode.SelectedAnswer;

            //customer info
            if (!string.IsNullOrEmpty(customer.Tel1)) customer.Tel1 = customer.Tel1.Trim();
            if (!string.IsNullOrEmpty(customer.Tel2)) customer.Tel2 = customer.Tel2.Trim();
            result.unitInfo.customerInfo.addressInfo.address1 = customer.Addr1;
            result.unitInfo.customerInfo.addressInfo.address2 = customer.Addr2;
            result.unitInfo.customerInfo.addressInfo.address3 = customer.Addr3;
            result.unitInfo.customerInfo.addressInfo.city = customer.Town;

            //if Republic of Ireland then blank postcode
            if (customer.Country.Equals(DefaultValues.IrelandCountryCode)) customer.Postcode = "0";

            result.unitInfo.customerInfo.addressInfo.zipcode = customer.Postcode;
            result.unitInfo.customerInfo.addressInfo.countryISO2 = customer.Country;
            result.unitInfo.customerInfo.communicationLanguageISO2 = "EN";
            result.unitInfo.customerInfo.companyName = retailerinfo;// StoreName;//
            result.unitInfo.customerInfo.email = customer.Email;
            result.unitInfo.customerInfo.faxField = string.Empty;
            result.unitInfo.customerInfo.firstName = customer.Forename;
            result.unitInfo.customerInfo.fixedPhone = string.IsNullOrEmpty(customer.Tel1) ? string.Empty : phonePrefix + customer.Tel1.TrimStart('0');
            result.unitInfo.customerInfo.lastName = customer.Surname;
            result.unitInfo.customerInfo.mobilePhone = string.IsNullOrEmpty(customer.Tel2) ? string.Empty : phonePrefix + customer.Tel2.TrimStart('0');
            result.unitInfo.customerInfo.noSurvey = !customer.Customer_Survey;
            result.unitInfo.customerInfo.noSurveySpecified = true;
            result.unitInfo.customerInfo.notSendAdverts =  !(jobInfo.CustomerType==0);
            result.unitInfo.customerInfo.notSendAdvertsSpecified = true;
            result.unitInfo.customerInfo.outOfOfficePhone = string.Empty;
            result.unitInfo.customerInfo.title = customer.Title;

            //delivery info
            result.unitInfo.deliveryInfo.companyName = engineer.EngineerName;
            result.unitInfo.deliveryInfo.deliveryAddressInfo.address1 = engineer.EngineerAddress1; //to do correct split
            result.unitInfo.deliveryInfo.deliveryAddressInfo.address2 = engineer.EngineerAddress2; //to do correct split
            result.unitInfo.deliveryInfo.deliveryAddressInfo.address3 = engineer.EngineerAddress3; //to do correct split
            result.unitInfo.deliveryInfo.deliveryAddressInfo.city = engineer.EngineerCity;
            result.unitInfo.deliveryInfo.deliveryAddressInfo.zipcode = engineer.EngineerPostcode;
            result.unitInfo.deliveryInfo.deliveryAddressInfo.countryISO2 = "GB";
            result.unitInfo.deliveryInfo.email = string.IsNullOrEmpty(engineer.EngineerEmail) ? "aaa@aa.com" : engineer.EngineerEmail;
            result.unitInfo.deliveryInfo.firstName = engineer.EngineerName;
            result.unitInfo.deliveryInfo.lastName = engineer.EngineerName;

            return result;
        }

        private string GetRetailerInfo(string supplierid)
        {

            return JobService.GetRetailerdetails(supplierid);
        
         
        }

        private string GetPhonePrefix(string country)
        {
            //set telephone prefix
            if (country.Equals("GB")) return "0044";
            if (country.Equals("IE")) return "00353";
            return "00";
        }


        /// <summary>
        /// generate attachment model
        /// </summary>
        /// <returns></returns>
        public AttachmentModel GenerateAttachModel()
        {
            Log.File.Info(Msg.GenerateLogMsg("Generating attachment model..."));
            var customer = GetCustomerDetails(SessionInfo.CustomerId);
            var engineer = EngineerService.GetEngineerDetailsFor3C(SessionInfo.EngineerId, SessionInfo.ModelId);

            var result = new AttachmentModel
            {
                caseId = string.Empty,
                EngineerCurrency = engineer.EngineerCurrency
            };
            var file = SessionInfo.UploadedFile;
            if (file.FileSize > 0)
            {
                result.caseId = SessionInfo.CaseId;
                result.fileName = file.FileName;
                result.message = string.Empty;
                result.dateOfPurchase = SessionInfo.DateOfPurchase;
                result.attachment = file.FileContent;
            }
            result.dateOfPurchase = SessionInfo.DateOfPurchase;
            result.country = customer.Country;
            Log.File.Info(Msg.GenerateLogMsg("Generated attachment model. Model:", result));
            return result;
        }

        /// <summary>
        /// Update Aep info with values from 3C
        /// </summary>
        /// <param name="aepModel"></param>
        public void ChangeAepInfo(CreateAEPSwapModelResultModel aepModel)
        {
            try
            {
                int clientNumberFieldNo = 1;
                int swapModelNameFieldNo = 2;
                int swapModelCodeFieldNo = 3;
                int originalModelCodeFieldNo = 4;
                int aepBookingReferenceFieldNo = 6;
                
                var aepFields =
                    Functions.ConvertListToNewListType<FieldsFromDB, AepSessionResultModel>(
                        SessionInfo.AepInfo.AepFields);

                var aepInfo = FieldsFromDbService.GetAepFields(SessionInfo.ServiceId, aepFields);

                var firstOrDefault = aepInfo.FirstOrDefault(x => x.FieldNo == clientNumberFieldNo);
                if (firstOrDefault != null)
                {
                    firstOrDefault.SelectedAnswer = aepModel.clientNumber;
                }

                firstOrDefault = aepInfo.FirstOrDefault(x => x.FieldNo == swapModelNameFieldNo);
                if (firstOrDefault != null)
                {
                    firstOrDefault.SelectedAnswer = aepModel.swapModelName;
                }

                firstOrDefault = aepInfo.FirstOrDefault(x => x.FieldNo == swapModelCodeFieldNo);
                if (firstOrDefault != null)
                {
                    firstOrDefault.SelectedAnswer = aepModel.swapModelCode;
                }

                firstOrDefault = aepInfo.FirstOrDefault(x => x.FieldNo == originalModelCodeFieldNo);
                if (firstOrDefault != null)
                {
                    firstOrDefault.SelectedAnswer = aepModel.originalModelCode;
                }

                firstOrDefault = aepInfo.FirstOrDefault(x => x.FieldNo == aepBookingReferenceFieldNo);
                if (firstOrDefault != null)
                {
                    firstOrDefault.SelectedAnswer = aepModel.aepBookingReference.ToString();
                }

               bool update = FieldsFromDbService.SaveFields(SessionInfo.ServiceId, aepInfo,true);
            }
            catch (Exception e)
            {
                throw new HttpException(500, string.Format("Error in saving AEP fields. Error: {0}", e.Message));
            }

        }


        /// <summary>
        /// Update diary ent and uploaded file
        /// </summary>
        /// <returns></returns>
        public SaveServiceResult UpdateDiaryEntAndChangeStatus()
        {
            var diaryEntModel = new DieryEntInfoModel();
            diaryEntModel.InjectFrom(SessionInfo);
            diaryEntModel.UserID = UserId;

            var result = new SaveServiceResult();
            try
            {
                Log.File.Info(Msg.GenerateLogMsg("Updateing DiaryEnt for service id ", SessionInfo.ServiceId));
                diaryEntModel.IsUpdateStatus = true;
                JobService.UpdateDaryEnt(diaryEntModel);
                JobService.UpdateCaseId(SessionInfo.ServiceId, SessionInfo.CaseId);
                Log.File.Info(Msg.GenerateLogMsg("Updated DiaryEnt."));
                result.IsSuccess = true;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.ErrorMessage = e.Message;
            }

            return result;
        }



        public void ClearFromSession()
        {
            Session.Clear(SessionInfo);
        }



        internal int CreateCustomerBackup(CustomerPageModel model,string error)
        {
            var customer = new CustomerModel();
            customer.InjectFrom(model);
            return CustomerService.SaveCustomerBackup(customer, error);
        }

        internal void UpdateOnlineBookingExcluded(int p)
        {
            throw new NotImplementedException();
        }
    }
}