using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClientConnect.Configuration;
using ClientConnect.Customer;
using ClientConnect.Infrastructure;
using ClientConnect.Logging;
using ClientConnect.Models.Customer;
using ClientConnect.Repositories;
using ClientConnect.ViewModels.Customer;
using ClientConnect.Models.Job;
using ClientConnect.Jobs;
using ClientConnect.ViewModels.Job;
using Omu.ValueInjecter;
using PagedList;
using ClientConnect.Properties;
using ClientConnect.Models.BookNewService;
using ClientConnect.Products;
//usinClientConnectST.Controllers;
using ClientConnect.Models.Product;

namespace ClientConnect.Services
{
    public class CustomerService : Service, IService
    {

        /// <summary>
        /// Product data access object
        /// </summary>
        private CustomerRepository Repository { get; set; }

        private JobRepository RepositoryJob { get; set; }
        
        /// <summary>
        /// Constructor
        /// </summary>
        public CustomerService()
        {
            Repository = (CustomerRepository)Ioc.Get<CustomerRepository>();
            RepositoryJob = (JobRepository)Ioc.Get<JobRepository>();
        }
        public  bool CustomerURNExists(string Accountnumber, string EMAIL)
        {
            return Repository.CustomerURNExists(Accountnumber, EMAIL, StoreId);
        }
        /// <summary>
        /// Get customer info
        /// </summary>
        /// <returns></returns>
        public CustomerModel GetCustomerInfo(int customerId)
        {
            try
            {
             //   var res = new CustomerModel();
                // Get customer info
                var model = Repository.GetCustomerInfo(customerId) ?? new CustomerModel();
                var country = string.IsNullOrEmpty(model.Country) ? StoreCountry : model.Country;
                model.TitleList = GetTitlesList(model.Title);
                model.ContactMethodList = GetContactMethodList(model.ContactMethod.ToString());
                model.CountryList = GetCountryList(country);
                model.FailureRepairReasonsList = GetFailureRepairReasons();
                 var CustomerProduct=CLC_GetCustomerProductsbyCustomerid(customerId); // GetCustomerProducts(customerId);

                 List<Product_InfoModel> Productinfos = new List<Product_InfoModel>();
                 foreach (CustAplModel item in CustomerProduct)
                 {
                     Product_InfoModel prodModel = new Product_InfoModel();
                     prodModel.InjectFrom(item);
                     prodModel.DateOfPurchase = item.DateOfPurchase;
                     prodModel.CONTRACTSTART = item.CONTRACTSTART;
                     prodModel.CONTRACTEXPIRES = item.CONTRACTEXPIRES;
                     AddressDetails address = new AddressDetails();
                     address.InjectFrom(item);
                     prodModel.addressDetails = address;
                     Productinfos.Add(prodModel);
                 }
              //  Productinfos.InjectFrom(CustomerProduct);
                model.ProductInfoModel = Productinfos;
               
               
                //var result = new ClientConnect.ViewModels.Job.JobDetailsModel();

              //  var JobNotes = RepositoryJob.GetJobNotesForShop(customerId);
                
                
              //  foreach (var item in model.ProductInfoModel)
              //  {
              //      //item.JobNotes  = RepositoryJob.GetJobDetailsByCustomerAppliance(item.serviceId);
              //     item. ServiceDetails = RepositoryJob.GetJobDetailsByCustomerAppliance(item.serviceId);
              //    // model.JobDetails = RepositoryJob.GetJobDetailsByCustomerAppliance(item.serviceId);//Job_NoteDto
                   
              //     item.JobNotes = JobNotes.Select(noteDto =>
              //new NoteRecordModelShop()
              //{
              //    ServiceId=noteDto.ServiceId,
              //    DateTime = noteDto.DateTime,
              //    From = noteDto.From,
              //    Notes = noteDto.Notes
              //}).ToList();


              //     item.noteModelDetails.AllPageofNotes = item.JobNotes.ToPagedList(1, 20);
              //     item.noteModelDetails.NotesModel = item.JobNotes;


              //    // item.JobNotes.ToPagedList(page ?? 1, 20);
              //     //res.JobDetails = RepositoryJob.GetJobDetailsByCustomerAppliance(item.serviceId).ToList();//Job_NoteDto
                    
              //     // item.JobNotes = RepositoryJob.GetJobDetailsByCustomerAppliance(item.ServiceDetails.ServiceId);
              //  }

             //   res = model;


                //model.NotesModel = Repository.getCustomerNotes(customerId);

                return model;
            }
            catch (Exception e)
            {
                throw new HttpException(500, string.Format("Error in method. Method:{0}. Error:{1}", "GetCustomerInfo", e.Message));
                
            }
            
        }

        private List<Products.Product_InfoModel> GetCustomerProducts(int customerId)
        {
            return Repository.GetCustomerProducts(customerId);
        }
     
        private List<Products.CustAplModel> CLC_GetCustomerProductsbyCustomerid(int customerId)
        {
            return Repository.CLC_GetCustomerProductsbyCustomerid(customerId);
        }

        public List<Products.Product_InfoModel> GetCustomerJobDetails(int customerId,int modelid)
        {
            return Repository.GetCustomerProducts(customerId);
        }

       
        
        /// <summary>
        /// Save new customer or update exist
        /// </summary>
        /// <param name="model">Customer info</param>
        public int SaveCustomer(CustomerModel model)
        {
            if (model.Country.Equals(DefaultValues.IrelandCountryCode))
                model.Postcode = DefaultValues.IrelandCountryCode;
            if (!string.IsNullOrEmpty(model.MobileTel)) model.MobileTel = model.MobileTel.Replace(" ","");
            if (!string.IsNullOrEmpty(model.LandlineTel)) model.LandlineTel = model.LandlineTel.Replace(" ", "");
            Log.File.Info(Msg.GenerateLogMsg("Saving or updating customer info.", model));
            int customerId = 0;
            //if(model.CustomerId==0)
            customerId = Repository.SaveCustomer(model, UserId, StoreId);
            //else
            //       customerId = Repository.SaveCustomer(model, UserId,StoreId);
            return customerId;
        }

        /// <summary>
        /// Update contact info
        /// </summary>
        /// <param name="model"></param>
        public void UpdateContactInfo(Contact_InfoModel model)
        {
            Log.File.Info(Msg.GenerateLogMsg("Update contact info...", model));
            Repository.UpdateContactInfo(model);
        }

        /// <summary>
        /// Update customer info
        /// </summary>
        /// <param name="model"></param>
        public void UpdateCustomerInfo(Customer_InfoModel model)
        {
            // write data into the database
            Log.File.Info(Msg.GenerateLogMsg("Update customer info...", model));
            model.Postcode = model.Country == DefaultValues.IrelandCountryCode ? DefaultValues.IrelandCountryCode : model.Postcode;
            Repository.UpdateCustomerInfo(model);
        }

        /// <summary>
        /// Get fauld reasons
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetFailureRepairReasons()
        {
            return Repository.GetFailureRepairReasonsList()
                    .Select(reason => new SelectListItem
                    {
                        Value = reason.FailureReasonID.ToString(),
                        Text = reason.FailureReasonDesc
                    })
                    .ToList();
        }
        
        /// <summary>
        /// Get title list
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetTitlesList(string title)
        {
            return Repository.GetTitlesList(title); ;
        }

        /// <summary>
        /// get countries list
        /// </summary>
        /// <returns>List with selected value</returns>
        public List<SelectListItem> GetCountryList(string Country)
        {
            // Fill list of titles
            return Repository.GetCountryList(Country);
        }

        /// <summary>
        /// Contact methods list
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public List<SelectListItem> GetContactMethodList(string method)
        {
            return ContactMethod.ContactMethod.GetPrefferList(method);
        }

        public void ClearFromSession()
        {
            Session.Clear(SessionInfo);
        }

        public CustomerSessionResultModel SessionInfo
        {
            get { return Session.Load(new CustomerSessionResultModel()); }
        }


        public List<SelectListItem> GetRetailClientList()
        {
            // Fill list of titles
            return Repository.GetRetailClientList(StoreId);
        }

        public  int SaveCustomerBackup(CustomerModel model)
        {
            if (model.Country.Equals(DefaultValues.IrelandCountryCode))
                model.Postcode = DefaultValues.IrelandCountryCode;
            if (!string.IsNullOrEmpty(model.MobileTel)) model.MobileTel = model.MobileTel.Replace(" ", "");
            if (!string.IsNullOrEmpty(model.LandlineTel)) model.LandlineTel = model.LandlineTel.Replace(" ", "");
            Log.File.Info(Msg.GenerateLogMsg("Saving or updating customer info.", model));
            int customerId = 0;
            //if(model.CustomerId==0)
            customerId = Repository.SaveCustomerBackup(model, UserId, StoreId);
            //else
            //       customerId = Repository.SaveCustomer(model, UserId,StoreId);
            return customerId;
        }

        internal Customer_SearchModel GetCustomersList(string searchCriteria, int pageNumber)
        {
            var result = new Customer_SearchModel();
            result.SearchCriteria = searchCriteria;
            result.CurrentPage = pageNumber;

            if (!string.IsNullOrEmpty(searchCriteria))
            {
                //Logging
                Log.File.Info(Msg.GenerateLogMsg("Finding Customers... Criteria:", searchCriteria));

                // get products
                var list = Repository.GetCustomers(searchCriteria, pageNumber, Settings.Default.JobSearchPageSize, StoreId);

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
        public List<AccidentDamageClaimList> GetAccidentalDamageList(bool ismobile=false)
        {
            // Fill list of titles
            return Repository.GetAccidentalDamageList(ismobile);
        }



        internal List<ClientConnect.Models.Product.MobileAccidentClaim> GetMobileAccidentalDamageList()
        {
            return Repository.GetMobileAccidentalDamageList();
        }



       

         
        public Customer_SearchModel GetCustomersList(ClientConnect.Customer.AdvSearchCriteria model, int pageNumber)
        {
            var result = new Customer_SearchModel();


            result.AdvSearchCriteria = model;
            result.CurrentPage = pageNumber;

            if ( !string.IsNullOrEmpty(model.Surname) || !string.IsNullOrEmpty(model.TelNo) || !string.IsNullOrEmpty(model.Postcode) || !string.IsNullOrEmpty(model.PolicyNumber) || !string.IsNullOrEmpty(model.ClientCustRef) || !string.IsNullOrEmpty(model.Address))
            {
                //Logging
                Log.File.Info(Msg.GenerateLogMsg("Finding customers:"));

                var list = Repository.GetCustomers(model.Surname, model.Postcode, model.TelNo, model.PolicyNumber, model.ClientCustRef, model.Address, model.UseAndInWhereCondition.Value, pageNumber, Settings.Default.JobSearchPageSize, StoreId);

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
    }

}
