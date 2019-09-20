using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using ClientConnect.Customer;
using ClientConnect.Infrastructure;
using ClientConnect.Models.Customer;
using ClientConnect.ViewModels.Customer;
using ClientConnect.Products;
using ClientConnect.Models.BookNewService;
using ClientConnect.Models.Product;

namespace ClientConnect.Repositories
{
    public class CustomerRepository : Repository
    {
        /// <summary>
        /// Initializes the instance
        /// </summary>
        public CustomerRepository()
        {
        }


        /// <summary>
        /// Get title list
        /// </summary>
        /// <param name="title">Selected value in list</param>
        /// <returns></returns>
        public List<SelectListItem> GetTitlesList(string title)
        {
            // read from db
            var titles = Query<Customer_TitlesModel>("Retrieve_CustomerTitles", CommandType.StoredProcedure);
            // set selected value
            var list = titles.Select(c => new SelectListItem { Value = c.Title.ToString(), Text = c.Title }).ToList();
            var firstOrDefault = list.FirstOrDefault(x => x.Value == title);
            if (firstOrDefault != null)
                firstOrDefault.Selected = true;
            return list;

        }

        /// <summary>
        /// Get countries list
        /// </summary>
        /// <param name="title">Selected value in list</param>
        /// <returns></returns>
        public List<SelectListItem> GetCountryList(string title)
        {
            // read from db
            var countries = Query<Customer_CountriesModel>("Retrieve_Countries", CommandType.StoredProcedure);
            // set selected value
            var list = countries.Select(c => new SelectListItem { Value = c.CountryCode.ToString(), Text = c.ContryName }).ToList();
            var firstOrDefault = list.FirstOrDefault(x => x.Value == title);
            if (firstOrDefault != null)
                firstOrDefault.Selected = true;
            return list;
        }
        public List<SelectListItem> GetSupplierList(int supplierId)
        {
            // read from db
            var suppliers = Query<Customer_suppliersModel>("Get_supplier", CommandType.StoredProcedure);
            // set selected value
            var list = suppliers.Select(c => new SelectListItem { Value = c.supplierId.ToString(), Text = c.supplier }).ToList();
            var firstOrDefault = list.FirstOrDefault(x => x.Value == supplierId.ToString());
            if (firstOrDefault != null)
                firstOrDefault.Selected = true;
            return list;
        }
        /// <summary>
        /// Execute store procedure 'RetrieveCustomer', which get customer info
        /// </summary>
        /// <param name="customerId">Customer Id</param>
        /// <returns>Customer info model</returns>
        public CustomerModel GetCustomerInfo(int customerId)
        {
            if (customerId > 0)
            {
                return Query<CustomerModel>("RetrieveCustomer",
                                                                   new { CustomerID = customerId },
                                                                   CommandType.StoredProcedure).FirstOrDefault();
            }
            return new CustomerModel();

        }

        public bool CustomerURNExists(string Accountnumber, string EMAIL ,int Clientid)
        {
                return (Query<int>("VerifyCustomerURN",    new { EMAIL = EMAIL, Accountnumber = Accountnumber, Clientid = Clientid },
                                                                   CommandType.StoredProcedure).ToList()).Count > 0;
            

        } 

        /// <summary>
        /// Updates customer information
        /// </summary>
        /// <param name="customer">Represent update command parameters</param>
        /// 
        public int SaveCustomer(CustomerModel customer, string userId, int clientid)
        {
            return Query<int>("UpdateCustomer",
                new
                {
                    customer.CustomerId,
                    customer.Title,
                    customer.Forename,
                    customer.Surname,
                    customer.Postcode,
                    customer.Addr1,
                    customer.Addr2,
                    customer.Addr3,
                    customer.MobileTel,
                    customer.LandlineTel,
                    customer.Email,
                    customer.ContactMethod,
                    customer.Town,
                    customer.County,
                    customer.Country,
                    customer.Department,
                    customer.Organization,
                    clientid,

                    userId,
                    customer.Customer_Survey,
                    customer.RetailClient,customer.CLIENTCUSTREF
                }
                , CommandType.StoredProcedure).FirstOrDefault();
        }

        /// <summary>
        /// Get Failure reasons
        /// </summary>
        /// <returns></returns>
        public List<FailureReasonResultModel> GetFailureRepairReasonsList()
        {
            return Query<FailureReasonResultModel>("Retrieve_FailureReasons", CommandType.StoredProcedure);
        }
        /// <summary>
        /// Save customer refuse reason
        /// </summary>
        /// <param name="model">Data model</param>
        /// <param name="userId">User id</param>
        /// <param name="clientId">Client id</param>
        public void SaveCustomerRefuseRepair(CustomerModel model, string userId, int clientId)
        {
            Execute("Update_RefuseRepairInfo", new
            {
                @TitleName = model.Title,
                model.Forename,
                model.Surname,
                model.Addr1,
                model.Addr2,
                model.Addr3,
                model.Town,
                model.County,
                model.Postcode,
                @UserId = userId,
                model.FailureRepairReason,
                @ClientId = clientId
            }, CommandType.StoredProcedure);
        }

        /// <summary>
        /// Update collect info
        /// </summary>
        /// <param name="model"></param>
        public void SaveCollectInfo(CustomerModel model)
        {
            Execute("UpdateCollectCustomerInfo", new
            {
                model.CustomerId,
                Addr1 = model.CollectAddr1,
                Addr2 = model.CollectAddr2,
                Addr3 = model.CollectAddr3,
                Postcode = model.CollectPostcode,
                Town = model.CollectTown,
                Country = model.CollectCounty,
                Name = model.Surname,
                TelNo1 = model.MobileTel,
                TelNo2 = model.LandlineTel,
                model.Title,
                Firstname = model.Forename,
                model.Email
            },
            CommandType.StoredProcedure);

        }

        /// <summary>
        /// Updates customer information
        /// </summary>
        /// <param name="updateCommand">Represent update command parameters</param>
        public void UpdateCustomerInfo(Customer_InfoModel customerinfo)
        {
            Execute("UpdateCustomerInfo",
                new
                    {
                        CustomerId = customerinfo.CustomerId,
                        FirstName = customerinfo.Forename,
                        Surname = customerinfo.Surname,
                        Address1 = customerinfo.Addr1,
                        Address2 = customerinfo.Addr2,
                        Address3 = customerinfo.Addr3,
                        County = customerinfo.County,
                        Country = customerinfo.Country,
                        PostCode = customerinfo.Postcode,
                        Town = customerinfo.Town
                        // Customer_nosurvey = customerinfo.Customer_NoSurvey

                    }

        , CommandType.StoredProcedure);
        }

        /// <summary>
        /// Updates contact info
        /// </summary>
        /// <param name="updateCommand">Update contact info command</param>
        public void UpdateContactInfo(Contact_InfoModel contactinfo)
        {
            Execute("UpdateContactInfo", new
                {
                    CustomerId = contactinfo.CustomerId,
                    Email = contactinfo.Email,
                    MobileNumber = contactinfo.MobileNumber,
                    LandlineNumber = contactinfo.LandlineNumber,
                    PreferredMethod = contactinfo.PreferredMethod,
                    Customer_survey = contactinfo.Customer_Survey
                }, CommandType.StoredProcedure);
        }



        public List<Products.Product_InfoModel> GetCustomerProducts(int customerId)
        {
            if (customerId > 0)
            {
                return Query<Product_InfoModel>("GetCustomerProducts",
                                                                   new { CustomerID = customerId },
                                                                   CommandType.StoredProcedure).ToList();
            }
            return new List<Products.Product_InfoModel>();
        }



        public List<Products.CustAplModel> CLC_GetCustomerProductsbyCustomerid(int customerId)
        {
            if (customerId > 0)
            {
                return Query<CustAplModel>("CLC_GetCustomerProductsbyCustomerid",
                                                                   new { CustomerID = customerId },
                                                                   CommandType.StoredProcedure).ToList();
            }
            return new List<Products.CustAplModel>();
        }
        public List<ViewModels.Job.NoteRecordModelShop> getCustomerNotes(int customerId)
        {
            if (customerId > 0)
            {
                return Query<ViewModels.Job.NoteRecordModelShop>("JobNotesListShop",
                                                                   new { CustomerID = customerId },
                                                                   CommandType.StoredProcedure).ToList();

            }


            return new List<ViewModels.Job.NoteRecordModelShop>();
        }

        public List<SelectListItem> GetRetailClientList(int StoreId)
        {
            string query = "SELECT [RetailCode] as RetailClientId,[RetailClientName] as RetailClientName FROM [dbo].[Retailclient] where RetailClientId=@StoreId ";

            var result = Query<ClientConnect.Models.Customer.ClientModel>(query, new { @StoreId = StoreId }, CommandType.Text); var list = result.Select(s => new SelectListItem { Value = s.RetailClientId.ToString(), Text = s.RetailClientName }).ToList();
            //  var firstOrDefault = list.FirstOrDefault(x => x.Value == title);
            return list;
        }

        internal int SaveCustomerBackup(CustomerModel customer,string error, string UserId, int StoreId)
        {
            return Query<int>("SaveCustomerBackup",
                  new
                  {
                      customer.CustomerId,
                      customer.Title,
                      customer.Forename,
                      customer.Surname,
                      customer.Postcode,
                      customer.Addr1,
                      customer.Addr2,
                      customer.Addr3,
                      customer.MobileTel,
                      customer.LandlineTel,
                      customer.Email,
                      customer.ContactMethod,
                      customer.Town,
                      customer.County,
                      customer.Country,
                      customer.Department,
                      customer.Organization,
                      StoreId,
                      UserId,
                     // customer.Customer_Survey,
                      customer.RetailClient,
                      customer.CLIENTCUSTREF,error
                  }
                  , CommandType.StoredProcedure).FirstOrDefault();
        }

        
            public List<Customer_SearchResult> GetCustomers(string searchCriteria, int pageNumber, int pageSize, int StoreId)
        {
            return Query<Customer_SearchResult>("CustomersList", new { Criteria = searchCriteria, ReturnLines = pageSize, PageNumber = pageNumber, StoreId = StoreId },
                                                                   commandType: CommandType.StoredProcedure).ToList();
        }



            internal List<AccidentDamageClaimList> GetAccidentalDamageList(bool ismobile)
            {
                return Query<AccidentDamageClaimList>("GetAccidentalDamageList", new {ismobile=ismobile},CommandType.StoredProcedure).ToList();
                // set selected value


            }



            internal List<MobileAccidentClaim> GetMobileAccidentalDamageList()
            {
               //  return Query<FailureReasonResultModel>("Retrieve_FailureReasons", CommandType.StoredProcedure)
                return Query<MobileAccidentClaim>("GetMobileAccidentalDamageList", CommandType.StoredProcedure).ToList();
            }

            internal List<Customer_SearchResult> GetCustomers(string surname, string postcode, string telNo, string policyNumber, string clientCustRef, string address, bool useAndInWhereCondition, int pageNumber, int pageSize, int storeId)
            {
                return Query<Customer_SearchResult>("SearchCustomers", new
                {
                    Surname = surname,
                    Address = address,
                    PolicyNumber = policyNumber,
                    Postcode = postcode,
                    TelNo = telNo,
                    ClientCustRef = clientCustRef,
                    UseAndInWhereCondition = useAndInWhereCondition,
                    ReturnLines = pageSize,
                    PageNumber = pageNumber,
                    StoreId = storeId
                    
               
                },
               commandType: CommandType.StoredProcedure).ToList();
            }

           
    }
}