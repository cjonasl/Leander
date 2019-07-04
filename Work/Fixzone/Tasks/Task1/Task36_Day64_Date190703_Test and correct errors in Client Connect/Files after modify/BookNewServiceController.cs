using System;
using System.IO;
using System.Web.Mvc;
using ClientConnect.Configuration;
using ClientConnect.Infrastructure;
using ClientConnect.Logging;
using ClientConnect.Models;
using ClientConnect.Models.Aep;
using ClientConnect.Models.Job;
using ClientConnect.Models._3C.Send;
using ClientConnect.Process;
using ClientConnect.Services;
using ClientConnect.Validation;
using ClientConnect.ViewModels.Aep;
using ClientConnect.ViewModels.BookNewService;
using FluentValidation;
using Omu.ValueInjecter;
using ClientConnect.Models.Product;
using System.Linq;
using ClientConnect.Models.BookNewService;
using ClientConnect.Models.Appliance;
using ClientConnect.ViewModels.Job;
using ClientConnect.CustomerProduct;
using Newtonsoft.Json;
using ClientConnect.Properties;
using System.Collections.Generic;
using System.Globalization;
using ClientConnect.Models.Store;
using ClientConnect.Home;
using System.Text;
using ClientConnect.Models.BookRepair;
using ClientConnect.IOnlineSpareParts;
using System.Xml;

namespace ClientConnect.Controllers
{

    public class BookNewServiceController : Controller
    {
        /// <summary>
        /// Service
        /// </summary>  
        private AdministrationService AdminService { get; set; }
        private CustomerService CustomerService { get; set; }
        private HomeService HomeService { get; set; }
        private CustomerProductService CustProductService { get; set; }
        private OnlineBookingService onlineBookingService { get; set; }
        private JobService JobService { get; set; }
        private BookNewServiceService BookService { get; set; }
        private EngineerService EngineerService { get; set; }
        private ProductService ProductService { get; set; }
        //private _3CService _3CService { get; set; }
        private FileService FileService { get; set; }
        private FieldsFromDBService FieldsFromDbService { get; set; }
        private StoreService storeService { get; set; }
        public TemplateService tempService { get; set; }

        private readonly RuleSetKeys _ruleSets;

        private Validator Validator { get; set; }

        private ApplianceService applianceService { get; set; }
        public StoreInfoModel storeinfo { get; set; }
        bool showOrderNumber { get; set; }
        bool ShowPNC { get; set; }
        bool LowCostCal { get; set; }
        bool NOAppliancePage { get; set; }
        public bool ShowAppointmentreason { get; set; }
        public bool Offlinebooking { get; set; }
        public bool SubmitTemplate { get; set; }
        public bool MobileTheft { get; set; }
        public bool ReplacementCover { get; set; }
        public bool ManufactWarranty { get; set; }
        public bool AdditionalJob { get; set; }
        public bool NOApplianceWarrantyInfo { get; set; }
        public bool StopBookingClientModelMissing { get; set; }
        public bool RestrictedAddressChange { get; set; }
        public bool PartOrder { get; set; }
        public bool DOPMandatory { get; set; }
        public bool WarrantyfromDOP { get; set; }
        public bool BlockAccountNo { get; set; }
        public bool ShowRegno { get; set; }
        public List<SpecialJob> specialJob;

        public BookNewServiceController()
        {
            HomeService = (HomeService)Ioc.Get<HomeService>();
            CustomerService = (CustomerService)Ioc.Get<CustomerService>();
            JobService = (JobService)Ioc.Get<JobService>();
            BookService = (BookNewServiceService)Ioc.Get<BookNewServiceService>();
            EngineerService = (EngineerService)Ioc.Get<EngineerService>();
            ProductService = (ProductService)Ioc.Get<ProductService>();
            //_3CService = (_3CService)Ioc.Get<_3CService>();
            FieldsFromDbService = (FieldsFromDBService)Ioc.Get<FieldsFromDBService>();
            storeService = (StoreService)Ioc.Get<StoreService>();
            applianceService = (ApplianceService)Ioc.Get<ApplianceService>();
            onlineBookingService = (OnlineBookingService)Ioc.Get<OnlineBookingService>();
            AdminService = (AdministrationService)Ioc.Get<AdministrationService>();
            CustProductService = (CustomerProductService)Ioc.Get<CustomerProductService>();
            tempService = (TemplateService)Ioc.Get<TemplateService>();


            //  BookService.SessionInfo.AepInfo.IsAepAviable = true;
            Validator = (Validator)Ioc.Get<Validator>();
            _ruleSets = new RuleSetKeys();
            storeinfo = storeService.GetStoreInfo(storeService.StoreId);
            var BusinessRules = HomeService.GetBusinessRuleList(storeService.StoreId);
            specialJob = HomeService.GetSpecialJobMappingList(storeService.StoreId);
            try
            {
                showOrderNumber = BusinessRules.Where(x => x.Key == BusinessRuleKey.ShowOrderNumber.ToString()).FirstOrDefault().Checked;
            }
            catch
            {
                showOrderNumber = false;
            }
            try
            {
                ShowPNC = BusinessRules.Where(x => x.Key == BusinessRuleKey.ShowPNC.ToString()).FirstOrDefault().Checked;
            }
            catch
            {
                ShowPNC = false;
            }
            try
            {
                LowCostCal = BusinessRules.Where(x => x.Key == BusinessRuleKey.LowCostCal.ToString()).FirstOrDefault().Checked;
            }
            catch
            {
                LowCostCal = false;
            }

            try
            {
                ReplacementCover = BusinessRules.Where(x => x.Key == BusinessRuleKey.ReplacementCover.ToString()).FirstOrDefault().Checked;
            }
            catch
            {
                ReplacementCover = false;
            }
            try
            {
                PartOrder = BusinessRules.Where(x => x.Key == BusinessRuleKey.PartOrder.ToString()).FirstOrDefault().Checked;
            }
            catch
            {
                PartOrder = false;
            }
            try
            {
                NOApplianceWarrantyInfo = BusinessRules.Where(x => x.Key == BusinessRuleKey.NOApplianceWarrantyInfo.ToString()).FirstOrDefault().Checked;
            }
            catch
            {
                NOApplianceWarrantyInfo = false;
            }

            try
            {
                RestrictedAddressChange = BusinessRules.Where(x => x.Key == BusinessRuleKey.RestrictedAddressChange.ToString()).FirstOrDefault().Checked;
            }
            catch
            {
                RestrictedAddressChange = false;
            }
            try
            {
                NOAppliancePage = BusinessRules.Where(x => x.Key == BusinessRuleKey.NOAppliancePage.ToString()).FirstOrDefault().Checked;
            }
            catch { NOAppliancePage = false; }
            try
            {
                ShowAppointmentreason = BusinessRules.Where(x => x.Key == BusinessRuleKey.ShowAppointmentreason.ToString()).FirstOrDefault().Checked;
            }
            catch
            {
                ShowAppointmentreason = false;
            }
            try
            {
                Offlinebooking = BusinessRules.Where(x => x.Key == BusinessRuleKey.Offlinebooking.ToString()).FirstOrDefault().Checked;
            }
            catch { Offlinebooking = false; }
            try
            {
                MobileTheft = BusinessRules.Where(x => x.Key == BusinessRuleKey.MobileTheft.ToString()).FirstOrDefault().Checked;
            }
            catch { MobileTheft = false; }
            try
            {
                ShowRegno = BusinessRules.Where(x => x.Key == BusinessRuleKey.ShowRegno.ToString()).FirstOrDefault().Checked;
            }
            catch { ShowRegno = false; }
            try
            {
                ManufactWarranty = BusinessRules.Where(x => x.Key == BusinessRuleKey.ManufactWarranty.ToString()).FirstOrDefault().Checked;
            }
            catch { ManufactWarranty = false; }
            try
            {
                AdditionalJob = BusinessRules.Where(x => x.Key == BusinessRuleKey.AdditionalJob.ToString()).FirstOrDefault().Checked;
            }
            catch
            {
                AdditionalJob = false;
            }
            try
            {
                SubmitTemplate = BusinessRules.Where(x => x.Key == BusinessRuleKey.SubmitTemplate.ToString()).FirstOrDefault().Checked;
            }
            catch
            {
                SubmitTemplate = false;
            }
            try
            {
                StopBookingClientModelMissing = BusinessRules.Where(x => x.Key == BusinessRuleKey.StopBookingClientModelMissing.ToString()).FirstOrDefault().Checked;
            }
            catch
            {
                StopBookingClientModelMissing = false;
            }
            try
            {
                WarrantyfromDOP = BusinessRules.Where(x => x.Key == BusinessRuleKey.WarrantyfromDOP.ToString()).FirstOrDefault().Checked;
            }
            catch
            {
                WarrantyfromDOP = false;
            }
            try
            {
                DOPMandatory = BusinessRules.Where(x => x.Key == BusinessRuleKey.DOPMandatory.ToString()).FirstOrDefault().Checked;
            }
            catch
            {
                DOPMandatory = false;
            }
            try
            {
                BlockAccountNo = BusinessRules.Where(x => x.Key == BusinessRuleKey.BlockAccountNo.ToString()).FirstOrDefault().Checked;
            }
            catch
            {
                BlockAccountNo = false;
            }
        }
        [HttpGet]
        public ActionResult ChangeCustomerAddress()
        {
            Log.File.Info(BookService.Msg.GenerateLogMsg("View customer info in booking process.", "Customer id = " + BookService.SessionInfo.CustomerId.ToString()));
            // var customer = BookService.GetCustomerDetails(CustomerService.SessionInfo.CustomerId);
            var customer = CustomerService.SessionInfo;
            var model = new CustomerPageModel();

            model.InjectFrom(customer);
            if (model.CustomerId == 0)
                BookService.SessionInfo.OnlinebookingFailed = false;
            model.RetailClientList = CustomerService.GetRetailClientList();
            model.CountryList = CustomerService.GetCountryList(model.Country);
            model.ContactMethodList = CustomerService.GetContactMethodList(model.ContactMethod.ToString());
            model.TitleList = CustomerService.GetTitlesList(model.Title);
            ViewBag.CustomerLogged = AdminService.StoreId == 0;
           
            //if ( RestrictedAddressChange && (BookService.SessionInfo.Jobtype != JobType.MobileCollection && BookService.SessionInfo.Jobtype != JobType.Collection))
            //{
            //    ViewBag.AddressChangeNotAllowed = true;
            //    return RedirectToAction("CustomerPreviewPage");
            //}
            return View(model);
        }

        [HttpPost]

        public ActionResult ChangeCustomerAddress(CustomerPageModel model)
        {
            ModelState.Clear();
            if (!string.IsNullOrEmpty(model.MobileTel)) model.MobileTel = model.MobileTel.Replace(" ", "");
            if (!string.IsNullOrEmpty(model.LandlineTel)) model.LandlineTel = model.LandlineTel.Replace(" ", "");

            //VALIDATION
            foreach (var error in Validator.Validate(model, _ruleSets.defaultRule))
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            //if (ModelState.IsValid)
            //{
            //    bool URNExist = CustomerService.CustomerURNExists(model.CLIENTCUSTREF, model.Email);
            //    if (URNExist && BookService.SessionInfo.CustomerId == 0)
            //        ModelState.AddModelError("CLIENTCUSTREF", "Account number exists already.Please contact support team");
            //}
            // If all ok then go to next page
            if (ModelState.IsValid)
            {
                var customer = CustomerService.SessionInfo;
                if (model.Postcode != customer.Postcode || model.Addr1 != customer.Addr1 || model.Addr2 != customer.Addr2 || model.Addr3 != customer.Addr3 || model.LandlineTel != customer.LandlineTel || model.MobileTel != customer.MobileTel)
                {

                    if (RestrictedAddressChange)
                    {
                        //BookService.SessionInfo.ServiceNotes = String.Format("Please note the collection address :<FONT face='Tahoma' size='12'><b style='font-size: medium><b>Collection Address: {0} {1} {2}</b></b></font><br/> <FONT face='Tahoma'><b>{3}</b></br/>"+
                        //                                                     "<b>{4}</b><br/><b>{5}</b><br/><b>{6}</b><br/><b>{7}</b><br/><b>Mobile no:{8}</b><br/><b>Tel no{9}</b><br/></font>",
                        //                                                     model.Title, model.Forename, model.Surname, model.Addr1, model.Addr2, model.Addr3, model.Town, model.Postcode, model.MobileTel, model.LandlineTel);

                        BookService.SessionInfo.ServiceNotes = String.Format("Please note the collection address :Collection Address: {0} {1} {2} , {3} {4} {5} {6} Postcode:{7} ;Mobile no:{8};Tel no{9}",
                                                                             model.Title, model.Forename, model.Surname, model.Addr1, model.Addr2, model.Addr3, model.Town, model.Postcode, model.MobileTel, model.LandlineTel);
                        return Redirect(Url.ProcessNextStep());
                    }
                    else
                    {

                        model.CLIENTCUSTREF = "";
                        int newcustomerid = BookService.CreateCustomer(model);

                        BookService.SessionInfo.CustProd.RetailClientId = model.RetailClient;

                        Log.File.Info(BookService.Msg.GenerateLogMsg("new customer created {0}", BookService.SessionInfo.CustomerId));
                        if (newcustomerid != 0 && model.CustomerId != newcustomerid)
                        {

                            CustomerService.SessionInfo.InjectFrom(model);

                            BookService.SessionInfo.OwnerCustomerId = model.CustomerId;
                            BookService.SessionInfo.CustomerId = newcustomerid;

                            model.CustomerId = BookService.SessionInfo.CustomerId;

                            BookService.SessionInfo.OnlinebookingFailed = false;
                            BookService.SessionInfo.CustomerCreationFailed = false;

                            BookService.SaveCustomer(model);

                            CustomerProductModel custprodModel = new CustomerProductModel();
                            custprodModel.InjectFrom(BookService.SessionInfo.CustProd);
                            custprodModel.CustomerId = newcustomerid;
                            custprodModel.ModelId = BookService.SessionInfo.ModelId;
                            OnlineCustomerApplianceResponse result = onlineBookingService.UpdateCustomerAppliance(custprodModel);

                            return Redirect(Url.ProcessNextStep());
                        }
                        else if (model.CustomerId == newcustomerid)
                        {
                            // just update customer information

                            BookService.SaveCustomer(model);

                            return Redirect(Url.ProcessNextStep());

                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Problem on updating customer details";
                            int tempCustomerId = BookService.CreateCustomerBackup(model);
                            model.CustomerId = tempCustomerId;
                            BookService.SessionInfo.CustomerId = tempCustomerId;

                            BookService.SessionInfo.OnlinebookingFailed = true;
                            BookService.SessionInfo.CustomerCreationFailed = true;
                            return Redirect(Url.ProcessNextStep());
                            //   return View(model);
                        }

                    }
                }
                else
                {
                    return Redirect(Url.ProcessNextStep());
                }

                // method for customer update

                // CreateCustomer using Fzonline booking and mark it as added in retailconnect

            }

            // fill lists
            model.TitleList = CustomerService.GetTitlesList(model.Title);
            model.ContactMethodList = CustomerService.GetContactMethodList(model.ContactMethod.ToString());
            model.RetailClientList = CustomerService.GetRetailClientList();
            model.CountryList = CustomerService.GetCountryList(model.Country);
            return View(model);
        }

        /// <summary>
        /// Show normal customer page
        /// </summary>
        /// <returns>Customer view</returns>
        [HttpGet]
        public ActionResult CustomerPage()
        {
            //if(AdditionalJob
            Log.File.Info(BookService.Msg.GenerateLogMsg("View customer info in booking process.", "Customer id = " + BookService.SessionInfo.CustomerId.ToString()));
            // var customer = BookService.GetCustomerDetails(CustomerService.SessionInfo.CustomerId);
            var customer = CustomerService.SessionInfo;
            var model = new CustomerPageModel();
            ViewBag.BlockAccountNo = BlockAccountNo;
            if (ProductService.SessionInfo.AdditionalJob)
            {
                int serviceid = JobService.SessionInfo.ServiceId > 0 ? JobService.SessionInfo.ServiceId : 0;
                JobDetailsModel jobdetails = JobService.JobDetails(serviceid);
                BookService.SessionInfo.CustomerId = jobdetails.CustomerInformation.CustomerId;
                CustomerService.SessionInfo.InjectFrom(jobdetails.CustomerInformation);
                return Redirect(Url.ProcessNextStep());
            }
            model.InjectFrom(customer);
            if (model.CustomerId == 0)
                BookService.SessionInfo.OnlinebookingFailed = false;
            model.RetailClientList = CustomerService.GetRetailClientList();
            model.CountryList = CustomerService.GetCountryList(model.Country);
            model.ContactMethodList = CustomerService.GetContactMethodList(model.ContactMethod.ToString());
            model.TitleList = CustomerService.GetTitlesList(model.Title);
            ViewBag.CustomerLogged = AdminService.StoreId == 0;
            return View(model);
        }

        /// <summary>
        /// Validate and save customer info
        /// </summary>
        /// <param name="model">Customer model info</param>
        /// <returns>View or redirect</returns>
        [HttpPost]

        public ActionResult CustomerPage(CustomerPageModel model)
        {
            ModelState.Clear();
            if (!string.IsNullOrEmpty(model.MobileTel)) model.MobileTel = model.MobileTel.Replace(" ", "");
            if (!string.IsNullOrEmpty(model.LandlineTel)) model.LandlineTel = model.LandlineTel.Replace(" ", "");
            ViewBag.BlockAccountNo = BlockAccountNo;
            //VALIDATION
            foreach (var error in Validator.Validate(model, _ruleSets.defaultRule))
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            if (ModelState.IsValid)
            {
                bool URNExist = CustomerService.CustomerURNExists(model.CLIENTCUSTREF, model.Email);
                if (URNExist && BookService.SessionInfo.CustomerId == 0 && !BlockAccountNo)
                    ModelState.AddModelError("CLIENTCUSTREF", "Account number exists already.Please contact support team");
            }
            // If all ok then go to next page
            if (ModelState.IsValid)
            {
                int tempCustomerId = 0;
                // method for customer update
                CustomerService.SessionInfo.InjectFrom(model);
                CustomerService.SessionInfo.Applianceaddress.InjectFrom(model);
                // CreateCustomer using Fzonline booking and mark it as added in retailconnect

                BookService.SessionInfo.CustomerId = BookService.CreateCustomer(model);//BookService.SessionInfo.CustomerId == 0 ? BookService.CreateCustomer(model) : BookService.SessionInfo.CustomerId;
                BookService.SessionInfo.CustProd.RetailClientId = model.RetailClient;
                //   if (BookService.SessionInfo.CustomerId == 0)
                Log.File.Info(BookService.Msg.GenerateLogMsg("OnlinebookingFailed {0}", BookService.SessionInfo.OnlinebookingFailed));
                if (BookService.SessionInfo.OnlinebookingFailed)
                {

                    ViewBag.ErrorMessage = "Problem on creating customer";
                    model.CustomerId = BookService.SessionInfo.CustomerId;
                    tempCustomerId = BookService.CreateCustomerBackup(model);
                    model.CustomerId = tempCustomerId;
                    BookService.SessionInfo.CustomerId = tempCustomerId;
                    return Redirect(Url.ProcessNextStep());
                }
                else
                {
                    model.CustomerId = BookService.SessionInfo.CustomerId;
                    BookService.SessionInfo.OnlinebookingFailed = false;
                    BookService.SessionInfo.CustomerCreationFailed = false;
                    BookService.SaveCustomer(model);

                    return Redirect(Url.ProcessNextStep());
                }
            }

            // fill lists
            model.TitleList = CustomerService.GetTitlesList(model.Title);
            model.ContactMethodList = CustomerService.GetContactMethodList(model.ContactMethod.ToString());
            model.RetailClientList = CustomerService.GetRetailClientList();
            model.CountryList = CustomerService.GetCountryList(model.Country);
            return View(model);
        }

        [HttpGet]
        public ActionResult ApplianceDetails()
        {
            if (!NOAppliancePage && ProductService.IsBackButtonPressed == true)
            {
                return Redirect(Url.ProcessPreviousStep());
            }

            Log.File.Info(BookService.Msg.GenerateLogMsg("View ApplianceDetails info in booking process.", "Customer id = " + BookService.SessionInfo.CustomerId.ToString()));
            ViewBag.ClientBookingType = storeService.ClientBookingType;
            ViewBag.NOShowWarrantyInfo = NOApplianceWarrantyInfo;
            ViewBag.ShowRegno = ShowRegno;
            CustomerProductModel custProd = new CustomerProductModel();
            custProd.InjectFrom(BookService.SessionInfo.CustProd);
            custProd.ModelId = BookService.SessionInfo.ModelId;
            custProd.ItemCode = BookService.SessionInfo.ItemCode;
            custProd.SerialNumber = BookService.SessionInfo.CustProd.SerialNumber;
            custProd.PNC = BookService.SessionInfo.CustProd.PNC;
            custProd.Regno = BookService.SessionInfo.CustProd.Regno;
            DateTime Dop;
            custProd.DateofPurchase = (DateTime.TryParse(BookService.SessionInfo.CustProd.DateofPurchase, out Dop)) ? (DateTime?)Dop : null;
            custProd.CustomerId = BookService.SessionInfo.CustomerId;
            custProd.CustAplId = BookService.SessionInfo.CustProd.CustAplId;
            // custProd.ClientRef = BookService.SessionInfo.CustProd.CustAplId;
            ViewBag.ShowOrderNumber = showOrderNumber;
            ViewBag.ShowPNC = ShowPNC;
            if (!NOAppliancePage)
            {
                return View(custProd);
            }
            else
            {
                return ApplianceDetails(custProd, novalidation: true);
            }
        }
        [HttpPost]
        public ActionResult ApplianceDetails(CustomerProductModel model, bool novalidation = false)
        {
            ModelState.Clear();
            ViewBag.NOShowWarrantyInfo = NOApplianceWarrantyInfo;
            ViewBag.ClientBookingType = storeService.ClientBookingType;
            ViewBag.ShowOrderNumber = showOrderNumber;
            ViewBag.ShowRegno = ShowRegno;
            ViewBag.ShowPNC = ShowPNC;
            //BookNewService_ApplianceDetailsValidation ApplianceDetailsValidation = new BookNewService_ApplianceDetailsValidation();
            //BookNewService_NonPolicyApplianceDetailsValidation NonPolicyApplianceDetailsValidation = new BookNewService_NonPolicyApplianceDetailsValidation();
            ////foreach (var error in ApplianceDetailsValidation.Validate(model, _ruleSets.defaultRule).Errors)
            //{
            //    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            //}
            if (!novalidation)
            {
                foreach (var error in Validator.Validate(model, _ruleSets.defaultRule))
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                // if (!storeService.ClientBookingType)
                //if (!NOApplianceWarrantyInfo)
                //{
                //    ModelState.Remove("CONTRACTSTART");
                //    ModelState.Remove("AuthNo");
                //    ModelState.Remove("contractexpires");
                //    ModelState.Remove("PolicyNumber");

                //}
                //if (!showOrderNumber) ModelState.Remove("ClientRef");
                //if (!DOPMandatory) ModelState.Remove("DateofPurchase");
            }
            if (ModelState.IsValid)
            {

                model.CustAplId = BookService.SessionInfo.CustProd.CustAplId;
                model.CustomerId = BookService.SessionInfo.CustomerId;
                Log.File.Info("Creating appliance");
                Log.File.InfoFormat(BookService.Msg.GenerateLogMsg("Input :" + JsonConvert.SerializeObject(model)));


                OnlineCustomerApplianceResponse result = onlineBookingService.AddCustomerAppliance(model);
                Log.File.InfoFormat(BookService.Msg.GenerateLogMsg("Output :" + JsonConvert.SerializeObject(result)));
                //if( result.CustAplID ==0)
                //{ ViewBag.error = "Error On creating customer Appliance . please contact support";
                //return View(model);
                //}
                //else{
                BookService.SessionInfo.CustProd.InjectFrom(result);

                BookService.SessionInfo.CustProd.SerialNumber = result.SNO;
                BookService.SessionInfo.CustProd.DateofPurchase = result.SupplyDat.HasValue ? result.SupplyDat.Value.ToShortDateString() : "";
                BookService.SessionInfo.CustProd.PNC = result.PNC;
                BookService.SessionInfo.CustProd.CustAplId = result.CustAplID;
                BookService.SessionInfo.CustProd.ClientRef = model.ClientRef;
                BookService.SessionInfo.CustProd.Regno = model.Regno;
                //if (model.PolicyNumber.ToUpper().EndsWith("MPI") && MobileTheft)
                //{
                //    return RedirectToAction("TheftDetails");
                //}
                //else
                //{
                return Redirect(Url.ProcessNextStep());
                // }


                //}
            }
            else
                return View(model);
            // return View("CustomerRegister"); 
        }
        //[HttpGet]
        //public ActionResult MobileTheftProcess()
        //{
        //    MobileTheftInfo model = new MobileTheftInfo();
        //    return View(model);
        //}
        //[HttpPost]
        //public ActionResult MobileTheftProcess()
        //{

        //    return View(model);
        //}


        [HttpGet]
        public ActionResult CustomerPreviewPage()
        {
            Log.File.Info(BookService.Msg.GenerateLogMsg("View customer info in booking process.", "Customer id = " + BookService.SessionInfo.CustomerId.ToString()));
            LowCost(BookService.SessionInfo.CustProd.CustAplId);
            var customer = CustomerService.SessionInfo;
            var model = new CustomerPageModel();
            if (BookService.SessionInfo.CustProd.CustomerId != 0)
            {
                model = BookService.GetCustomerDetails(BookService.SessionInfo.CustProd.CustomerId);
                CustomerService.SessionInfo.Applianceaddress.InjectFrom(model);
            }
            else
            {
                model.InjectFrom(customer);
                CustomerService.SessionInfo.Applianceaddress.InjectFrom(model);
            }
            model.RetailClientList = CustomerService.GetRetailClientList();
            model.CountryList = CustomerService.GetCountryList(model.Country);
            model.ContactMethodList = CustomerService.GetContactMethodList(model.ContactMethod.ToString());
            model.TitleList = CustomerService.GetTitlesList(model.Title);
            if (RestrictedAddressChange && (BookService.SessionInfo.Jobtype == JobType.Collection || BookService.SessionInfo.Jobtype == JobType.MobileCollection))
            {
                ViewBag.Collection = true;
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult CustomerPreviewPage(CustomerPageModel model)
        { //RequestDate()
            return Redirect(Url.ProcessNextStep());
        }

        [HttpGet]
        public ActionResult MobileCover()
        {
            var mobileAccidentClaim = CustomerService.GetMobileAccidentalDamageList();
            BookService.SessionInfo.MobileAccidentClaimModel.MobileAccidentClaim.Clear();
            MobileAccidentClaimModel model = new MobileAccidentClaimModel();

            if (mobileAccidentClaim.Exists(x => x._id == BookService.SessionInfo.SelectedADCover))
            {
                model.SelectedCover = BookService.SessionInfo.SelectedADCover;
            }

            foreach (var item in mobileAccidentClaim)
            {
                var ClaimModel = new ClientConnect.Controllers.MobileAccidentClaim();
                ClaimModel.InjectFrom(item);
                BookService.SessionInfo.MobileAccidentClaimModel.MobileAccidentClaim.Add(ClaimModel);

            }
            model.MobileAccidentClaim = BookService.SessionInfo.MobileAccidentClaimModel.MobileAccidentClaim;

            return View(model);
        }
        [HttpPost]
        public ActionResult MobileCover(MobileAccidentClaimModel model)
        {

            BookService.SessionInfo.BookMobileADclaim = false;
            BookService.SessionInfo.CustProd.mobileTheft = false;
            // BookService.SessionInfo.Jobtype = JobType.Defaulttype;
            var claimList = BookService.SessionInfo.MobileAccidentClaimModel.MobileAccidentClaim;
            model.MobileAccidentClaim = claimList;
            if ((model.SelectedCover) == 0)
            {
                ModelState.AddModelError("SelectedQuestionid", "Choose ");
                ViewBag.error = "Please answer for accidential damage claim.";
                return View(model);
            }
            else
            {
                var selectModel = model.MobileAccidentClaim.Where(x => x._id == model.SelectedCover).Single();
                BookService.SessionInfo.SelectedADCover = model.SelectedCover;
                BookService.SessionInfo.FaultDescr = selectModel.Question;
                if (selectModel.IsAccidentalDamage)// theft //todo:
                {
                    BookService.SessionInfo.CustProd.mobileTheft = true;
                    BookService.SessionInfo.Jobtype = JobType.MobileReplacement;
                    return Redirect(Url.ProcessNextStep());

                }
                else if (selectModel.ClaimFail) // Raise declined claim
                {
                    ViewBag.DeclinedClaim = true;
                    BookService.SessionInfo.Jobtype = JobType.Defaulttype;
                    ViewBag.SelectedId = model.SelectedCover;
                    return View(model);
                }
                else // book appointment 
                {
                    BookService.SessionInfo.BookMobileADclaim = true;
                    // BookService.SessionInfo.Jobtype = JobType.MobileCollection;
                    return Redirect(Url.ProcessNextStep());
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult MobileCoverDecline(int SelectedClaim)
        {

            MobileAccidentClaim model = new MobileAccidentClaim();
            var Filteredclaim = BookService.SessionInfo.MobileAccidentClaimModel.MobileAccidentClaim.Where(x => x._id == SelectedClaim).Single();
            model.InjectFrom(Filteredclaim);
            return View(model);
        }

        // [HttpPost]
        public ActionResult CreateMobileCoverDecline(int SelectedId)
        {
            var Claimmodel = BookService.SessionInfo.MobileAccidentClaimModel.MobileAccidentClaim;
            string SelectedDamage = Claimmodel.Where(x => x._id == SelectedId).Single().Question;
            var comment = new StringBuilder();
            comment.AppendFormat("{0} .", SelectedDamage);
            //if(Comment.Length>0)
            //comment.AppendFormat("Comment : {0}", Comment);
            BookMobileCoverDeclineJob(DateTime.Now.AddDays(2).ToShortDateString(), 0, comment.ToString(), comment.ToString(), JobType.Accidental);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ExceedTheftDateDeclineJob(string Reason)
        {
            var Claimmodel = BookService.SessionInfo.MobileAccidentClaimModel.MobileAccidentClaim;
            int SelectedId = BookService.SessionInfo.SelectedADCover;
            string SelectedDamage = Claimmodel.Where(x => x._id == SelectedId).Single().Question;
            StringBuilder comment = new StringBuilder();
            comment.AppendFormat("{0} .{1}.{2}", SelectedDamage, Reason, "Crime date can not be exceeded than 28 days.");
            //if(Comment.Length>0)
            //comment.AppendFormat("Comment : {0}", Comment);
            BookMobileCoverDeclineJob(DateTime.Now.AddDays(2).ToShortDateString(), 0, comment.ToString(), comment.ToString(), JobType.Accidental);
            return RedirectToAction("Index", "Home");
        }
        public ActionResult ExceedTheftDateAcceptJob(string Reason)
        {
            BookService.SessionInfo.FaultDescr = string.Format("{0};Crime date is exceeded than 28 days .Reason {1}", BookService.SessionInfo.FaultDescr, Reason);
            BookService.SessionInfo.Jobtype = JobType.MobileReplacement;
            return Redirect(Url.ProcessNextStep());
        }

        public void BookMobileCoverDeclineJob(string EventDate, int Engineerid, string faultdesc, string reason = "", JobType type = JobType.Defaulttype)
        {

            bool tempServiceId = false;
            var result = new OnlineBookResponseDetails();
            bool success = false;
            string FormPath = string.Empty;
            int serviceid = 0;
            string visitdate = string.Empty;
            string userid = storeService.UserId;
            var errorMsg = "Job Booking failed";
            var errorDetails = string.Empty;

            OnlineBookRequestDetails model = new OnlineBookRequestDetails();

            model.Postcode = CustomerService.SessionInfo.Applianceaddress.Postcode;

            model.VisitDate = DateTime.Parse(EventDate);

            model.CustomerID = CustomerService.SessionInfo.CustomerId;// CustomerService.SessionInfo.CustomerId;
            model.CustAplID = BookService.SessionInfo.CustProd.CustAplId;//onlineBookingService.addCustApl(model.CustomerID,JobType.Accidental);
            model.SNO = BookService.SessionInfo.CustProd.SerialNumber;
            model.PolicyNumber = BookService.SessionInfo.CustProd.PolicyNumber;
            model.AuthNo = BookService.SessionInfo.CustProd.AuthNo;
            model.ReportFault = faultdesc;

            model.ClientID = storeService.StoreId;

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



            var response = onlineBookingService.BookJob(model);
            if (response.BookSuccessfully)
            {
                //FormPath = string.Format("BookedJobConfirmation?Serviceid={0}&engineerid={1}&eventdate={2}&tempServiceId={3}", response.ServiceID, Engineerid, EventDate, tempServiceId);
                //success = response.BookSuccessfully;
                serviceid = response.ServiceID;
                visitdate = model.VisitDate.ToString();
                ServiceModel serviceModel = new ServiceModel();
                serviceModel.InjectFrom(response);
                serviceModel.Engineerid = Engineerid;
                serviceModel.CustomerID = BookService.SessionInfo.CustomerId;
                serviceModel.CustAplID = BookService.SessionInfo.CustProd.CustAplId;
                serviceModel.EventDate = model.VisitDate.ToString();
                serviceModel.Reportfault = model.ReportFault;
                serviceModel.ClientId = model.ClientID;
                serviceModel.VisitCode = model.VisitCode;
                serviceModel.Clientref = model.ClientRef;
                serviceModel.StatusId = model.StatusID;
                int Serviceid = JobService.CreateJobwithEngineer(serviceModel);

            }

        }
        //[HttpGet]
        //public ActionResult ValidationCheck()
        //{
        //    CustomValidation model = new CustomValidation();
        //    //if (StopBookingClientModelMissing && (BookService.SessionInfo.Jobtype == JobType.ModelMissing))
        //    //{
        //    //    BookNow(DateTime.Now.AddDays(2).ToShortDateString(), 0, "Model mapping missing. Model id :" + BookService.SessionInfo.ModelId, "Model mapping missing. Model id :" + BookService.SessionInfo.ModelId, JobType.ModelMissing);
        //    //    ViewBag.Error = Settings.Default.ClientModelMissing;
        //    //    ViewBag.BookedDeclined = true;
        //    //    return View(model);

        //    //}
        //}
        [HttpGet]
        public ActionResult ValidationCheck()
        {
            CustomValidation model = new CustomValidation();
            //if (StopBookingClientModelMissing && (BookService.SessionInfo.Jobtype == JobType.ModelMissing))
            //{
            //    BookNow(DateTime.Now.AddDays(2).ToShortDateString(), 0, "Model mapping missing. Model id :" + BookService.SessionInfo.ModelId, "Model mapping missing. Model id :" + BookService.SessionInfo.ModelId, JobType.ModelMissing);
            //    ViewBag.Error = Settings.Default.ClientModelMissing;
            //    ViewBag.BookedDeclined = true;
            //    return View(model);

            //}
            //else
            if (MobileTheft && (BookService.SessionInfo.CustProd.PolicyNumber.ToUpper().EndsWith("MPI") || BookService.SessionInfo.Jobtype == JobType.MobileCollection))
            {
                return RedirectToAction("MobileCover");
                //model.AccidentClaim = CustomerService.GetAccidentalDamageList(BookService.SessionInfo.CustProd.PolicyNumber.ToUpper().EndsWith("MPI"));
            }


            else
            {
                model.AccidentClaim = CustomerService.GetAccidentalDamageList();
                if (PartOrder)
                {
                    AccidentDamageClaimList item = new AccidentDamageClaimList();
                    item.Question = "Order spare parts";
                    item.OrderPart = true;
                    item.QuestionId = 1000;
                    model.AccidentClaim.Add(item);
                }
            }
            //if (MobileTheft && BookService.SessionInfo.CustProd.PolicyNumber.ToUpper().EndsWith("MPI"))
            //{


            //}
            return View(model);
        }

        [HttpPost]
        public ActionResult ValidationCheck(CustomValidation model)
        {
            bool OutofWarranty =false;
            BookService.SessionInfo.CustProd.mobileTheft = false;
            if (MobileTheft)
            {
                model.AccidentClaim = CustomerService.GetAccidentalDamageList(BookService.SessionInfo.CustProd.PolicyNumber.ToUpper().EndsWith("MPI"));
                if(!BookService.SessionInfo.CustProd.PolicyNumber.ToUpper().EndsWith("MPI") && PartOrder)
                
                {//additional question for order spares only
                    AccidentDamageClaimList item = new AccidentDamageClaimList();
                    item.Question = "Order spare parts";
                    item.OrderPart = true;
                    item.QuestionId = 1000;
                    model.AccidentClaim.Add(item);
                }
            }
            else
            {
                model.AccidentClaim = CustomerService.GetAccidentalDamageList();
                if (PartOrder)
                {//additional question for order spares only
                    AccidentDamageClaimList item = new AccidentDamageClaimList();
                    item.Question = "Order spare parts";
                    item.OrderPart = true;
                    item.QuestionId = 1000;
                    model.AccidentClaim.Add(item);
                }
            }
            Log.File.Info(BookService.Msg.GenerateLogMsg("ValidationCheck in booking process.", "Customer id = " + BookService.SessionInfo.CustomerId.ToString()));
            if ((model.SelectedQuestionid) == 0)
            {
                ModelState.AddModelError("SelectedQuestionid", "Choose ");
                ViewBag.error = "Please answer for accidential damage claim.";
                return View(model);
            }

            else
            {
                var ProductRepairProfiles = ProductService.GetClientModelDetails(BookService.SessionInfo.ModelId, storeService.StoreId);
                
                var SelectedQuestion = model.AccidentClaim.Where(x => x.QuestionId == model.SelectedQuestionid).FirstOrDefault();
                BookService.SessionInfo.FaultDescr = model.AccidentClaim.Where(x => x.QuestionId == model.SelectedQuestionid).Select(x => x.Question).FirstOrDefault();
                Log.File.Info(BookService.Msg.GenerateLogMsg("ValidationCheck in booking process.", "ManufactContactdetails = " + BookService.SessionInfo.CustProd.ManufactContactdetails));
                if (PartOrder && SelectedQuestion.OrderPart) //  if Part order  businessrule turned on and part order question choosen. then create the job and order part.
                {
                    return RedirectToAction("BookPartOrderJob");
                }
                if (ManufactWarranty)
                    model.InManufactWarranty = BookService.SessionInfo.CustProd.CONTRACTSTART >= DateTime.Now;

                  if(WarrantyfromDOP && DOPMandatory)
                  { 
                      
                      try{
                          // Howdens out of warranty - DOB+2 years
                      OutofWarranty=    DateTime.Parse(BookService.SessionInfo.CustProd.DateofPurchase).AddYears(Settings.Default.ApplianceWarrantyPeriod)< DateTime.Now;

                      }
                      catch(Exception Ex)
                      
                      {
                      }

                  }

                model.AccidentDamageClaim =
                model.AccidentClaim.Where(x => x.QuestionId == model.SelectedQuestionid).Select(x => x.IsAccidentalDamage).FirstOrDefault();
                //Log Accidental damage entry
                int QuestionId =           model.AccidentClaim.Where(x => x.QuestionId == model.SelectedQuestionid).Select(x => x.QuestionId).FirstOrDefault();
                CustProductService.AddAccidentalDamage(BookService.SessionInfo.CustProd.CustAplId, QuestionId, storeService.UserId);

                if (BookService.SessionInfo.CustProd.PolicyNumber.ToUpper().EndsWith("RPG") && ReplacementCover) // replacement policy
                { //  replacement covers the accidental damage and mechanical fault. difference is mechanical fault starts from MechanicalCoverStarts
                    if (model.AccidentDamageClaim)
                    {
                        BookService.SessionInfo.Jobtype = JobType.Replacement;
                        return Redirect(Url.ProcessNextStep());
                    }
                    else if (BookService.SessionInfo.CustProd.MechanicalCoverStarts.HasValue ? BookService.SessionInfo.CustProd.MechanicalCoverStarts > DateTime.Now : false)
                    {
                        BookService.SessionInfo.Jobtype = JobType.Replacement;
                        return Redirect(Url.ProcessNextStep());
                    }
                    else
                    {
                        String accidentalQ = model.AccidentClaim.Where(x => x.QuestionId == model.SelectedQuestionid).Select(x => x.Question).FirstOrDefault();
                        accidentalQ += " And Product is not in mechanical repair period.";
                        BookNow(DateTime.Now.AddDays(2).ToShortDateString(), 0, accidentalQ, accidentalQ, JobType.ManufactWarranty);
                        ViewBag.Error = Settings.Default.NotinMechanicalPeriod;
                        ViewBag.BookedDeclined = true;
                        return View(model);
                        //  return RedirectToAction("ContactDetails");
                    }
                }
                else if (OutofWarranty  && WarrantyfromDOP)
                {
                    ViewBag.Customerservice = "Advisor Note ";
                 
                    ViewBag.CustomerserviceDetails = "This product is out of warranty , If the customer wishes to proceed with a chargeable repair please enter job directly into complete service";
                    return View("CustomerService");
                }
                else if (model.InManufactWarranty && !model.AccidentDamageClaim)// non replacement , in manufact warranty , then the mechanical repair is not covered.
                {
                    ViewBag.ManufactContactDetails = BookService.SessionInfo.CustProd.ManufactContactdetails;
                    BookNow(DateTime.Now.AddDays(2).ToShortDateString(), 0, "In Manufacture Warranty", "In Manufacture Warranty", JobType.ManufactWarranty);
                    ViewBag.ManufactName = BookService.SessionInfo.CustProd.ManufactName;
                    ViewBag.Title = "Manufacture Contact Details";
                    return View("ManufactureContact");
                }
                else if (model.InManufactWarranty) // except replacement & mobile cover , accidental damamge is not covered. 
                {
                    ViewBag.Customerservice = "Customer Call center";
                    ViewBag.CustomerserviceDetails = BookService.SessionInfo.CustProd.ManufactName;
                    return View("CustomerService");
                }
                else if (ProductRepairProfiles.RepairBookRepairEngineerid != 0 && !model.AccidentDamageClaim)
                {
                    BookService.SessionInfo.EngineerId = ProductRepairProfiles.RepairBookRepairEngineerid;
                    BookService.SessionInfo.Jobtype = JobType.Replacement;
                    return Redirect(Url.ProcessNextStep());
                }

                else if (model.AccidentDamageClaim) // For service Guaraantee policies , accidental damage is not covered
                {
                    String accidentalQ = model.AccidentClaim.Where(x => x.QuestionId == model.SelectedQuestionid).Select(x => x.Question).FirstOrDefault();
                    BookNow(DateTime.Now.AddDays(2).ToShortDateString(), 0, accidentalQ, accidentalQ, JobType.Accidental);
                    ViewBag.Error = Settings.Default.NoAccidentalCOVER;// "Product is not covered for accidental damage.";
                    ViewBag.BookedDeclined = true;
                    return View(model);
                    // return RedirectToAction("ContactDetails");
                }
                else if (WarrantyfromDOP && !OutofWarranty)
                    return Redirect(Url.ProcessNextStep());
                else if (BookService.SessionInfo.CustProd.contractexpires > DateTime.Now && NOApplianceWarrantyInfo)// !storeService.ClientBookingType)
                    return Redirect(Url.ProcessNextStep());

                else
                {
                    //  return RedirectToAction("ContactDetails");
                    ViewBag.Customerservice = "Customer Call center";
                    //   BookNow(DateTime.Now.AddDays(2).ToShortDateString(), 0, "Not in warranty", "Warranty expired", JobType.Manuafct);
                    ViewBag.CustomerserviceDetails = "Your product is not in warranty , please contact customer service";
                    return View("CustomerService");
                }
                // Under manufacture warraty  ?
                //

                Log.File.Info(BookService.Msg.GenerateLogMsg("ValidationCheck done, next page.", "ManufactContactdetails = " + BookService.SessionInfo.CustProd.ManufactContactdetails));
            }
        }
        public ActionResult BookPartOrderJob()
        {

            string model = string.Empty;
            if (!string.IsNullOrEmpty(BookService.SessionInfo.PartOrderEnquiry))
                model = BookService.SessionInfo.PartOrderEnquiry;
            return View("BookPartOrderJob", model:model);
        }
        //[HttpPost]
        public ActionResult PartSearch(string PartEnquiry)
        {
            BookService.SessionInfo.PartOrderEnquiry = PartEnquiry;
            BookService.SessionInfo.Jobtype = JobType.PartsOnly;
            string errorMsg = "Problem on booking the job";

            BookNow(DateTime.Now.AddDays(2).ToShortDateString(), 0, "Part Enquiry : " + BookService.SessionInfo.PartOrderEnquiry, "Part Enquiry" + BookService.SessionInfo.PartOrderEnquiry, JobType.PartsOnly);
          
          ViewBag.ServiceID=BookService.SessionInfo.ServiceId;
          ViewBag.PartsEnquiry = PartEnquiry;
          return View("BookPartOrderJob",model: PartEnquiry);
        }
       
        public ActionResult BookManufactureWarrantyJob()
        {
            ViewBag.ManufactContactDetails = BookService.SessionInfo.CustProd.ManufactContactdetails;
            BookNow(DateTime.Now.AddDays(2).ToShortDateString(), 0, "In Manufacturer Warranty", "In Manufacturer Warranty", JobType.ManufactWarranty);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult BookExpiredWarrantyJob()
        {

            BookNow(DateTime.Now.AddDays(2).ToShortDateString(), 0, " Warranty Expired", "Expired Warranty", JobType.ExpiredWarranty);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult ModelMissingBookRepair()
        {

            BookNow(DateTime.Now.AddDays(2).ToShortDateString(), 0, "Model mapping is missing", "Model mapping is missing", JobType.ModelMissing);

            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public ActionResult BOOKDPAFailureService(DPACheck dpaCheck, string FailedDPACheck)
        {
            dpaCheck.InjectFrom(BookService.SessionInfo.dpaCheck);

            StringBuilder DPAFauilureReason = new StringBuilder();
            if (!dpaCheck.NameCheck)
                DPAFauilureReason.AppendFormat(",Customer Name ");
            if (!dpaCheck.AddressCheck)
                DPAFauilureReason.AppendFormat(",Address ");
            if (!dpaCheck.EmailAddressCheck)
                DPAFauilureReason.AppendFormat(",Email ");
            if (!dpaCheck.PolicyCheck)
                DPAFauilureReason.AppendFormat(",Policy Number ");
            if (!dpaCheck.ModelCheck)
                DPAFauilureReason.AppendFormat(",Model ");
            if (!dpaCheck.TelephoneCheck)
                DPAFauilureReason.AppendFormat(",Telephone ");
            if (!dpaCheck.AccountHolderCheck)
                DPAFauilureReason.AppendFormat(",Account Holder ");
            if (DPAFauilureReason.Length > 0)
                DPAFauilureReason.Remove(0, 1);
            string DPAfailedReason = String.Format("DPA check failed as {0} are failed to match", DPAFauilureReason.ToString());
            BookNow(DateTime.Now.AddDays(2).ToShortDateString(), 0, DPAfailedReason, DPAfailedReason, JobType.DPAfailure);
            return RedirectToAction("Index", "Home");
        }
        public ActionResult VerifyIMIE(CustomValidation model)
        {

            BookService.SessionInfo.customValidation.IsVerifyIMIEDone = true;
            //VALIDATION
            if (string.IsNullOrEmpty(model.MobileIMIE))
            {
                ModelState.AddModelError("MobileIMIE", "IMEI is required");


            }
            if (ModelState.IsValid)
            {
                if (true) // assume imie validation is successful
                {
                    BookService.SessionInfo.customValidation.IsVerifyIMIEDone = true;
                    BookService.SessionInfo.customValidation.IsIMIEValid = true;
                    ViewBag.VerifyMobileIMIEError = string.Empty;
                    ViewBag.VerifyMobileIMIESuccess = "Success";

                }
                else
                {
                    ViewBag.VerifyMobileIMIEError = string.Format("Error: {0} - {1}", "VerifyIMIE", "Error");
                    BookService.SessionInfo.customValidation.IsIMIEValid = false;
                    ViewBag.VerifyMobileIMIESuccess = string.Empty;
                }


                return View("ValidationCheck", model);
            }
            else
            {
                return View("ValidationCheck", model);
            }
        }
        [HttpGet]
        public ActionResult ManufactInstr()
        {
            return View();
        }


        [ActionName("ManufactInstr")]
        [HttpPost]
        public ActionResult ManufactInstrPost()
        {
            return Redirect(Url.ProcessNextStep());
        }

        //TheftDetails
        //[HttpGet]
        //public ActionResult TheftDetails()
        //{
        //    if (ProductService.IsBackButtonPressed == true && ProductService.SessionInfo.TroubleShootId == 0)
        //        return Redirect(Url.ProcessPreviousStep());
        //    else if (ProductService.SessionInfo.TroubleShootId == 0)
        //        return Redirect(Url.ProcessNextStep());

        //    else
        //    {

        //        TroubleshootModel TroubleshootModel = applianceService.GetApplianceTroubleShoot(ProductService.SessionInfo.TroubleShootId);
        //        return View(TroubleshootModel);
        //    }
        //}


        [HttpGet]
        public ActionResult MobileDetails()
        {
            var applianceDetails = BookService.SessionInfo.CustProd;
            ViewBag.ApplianceInformation = string.Format("Model :<b> {0} </b><br/> Serial no: <b>{1}</b><br/> PolicyNumber: <b>{2}</b> <br/> Price :£{3} ", applianceDetails.ItemCode, applianceDetails.SerialNumber, applianceDetails.PolicyNumber, applianceDetails.AppliancePrice);
            if (ProductService.IsBackButtonPressed == true && !BookService.SessionInfo.BookMobileADclaim)
                return Redirect(Url.ProcessPreviousStep());
            else if (BookService.SessionInfo.CustProd.PolicyNumber.ToUpper().EndsWith("MPI") && BookService.SessionInfo.BookMobileADclaim)
            {
                MobileTheftInfo model = new MobileTheftInfo();
                model.InjectFrom(BookService.SessionInfo.CustProd.mobileTheftInfo);
                return View(model);
            }

            else
            {
                return Redirect(Url.ProcessNextStep());


            }
        }
        [HttpPost]
        public ActionResult MobileDetails(MobileTheftInfo model)
        {
            ModelState.Clear();
            var validator = new MobilePhoneValidation();
            foreach (var errorval in validator.Validate(model, ruleSet: _ruleSets.defaultRule).Errors)
            {
                ModelState.AddModelError(errorval.PropertyName, errorval.ErrorMessage);
            }
            var applianceDetails = BookService.SessionInfo.CustProd;
            ViewBag.ApplianceInformation = string.Format("Model :<b> {0} </b><br/> Serial no: <b>{1}</b><br/> PolicyNumber: <b>{2}</b> <br/> Price :£{3} ", applianceDetails.ItemCode, applianceDetails.SerialNumber, applianceDetails.PolicyNumber, applianceDetails.AppliancePrice);

            ModelState.Remove("CRNLogged");
            ModelState.Remove("Description");
            ModelState.Remove("CrimeDate");
            ModelState.Remove("CrimeRefNo");


            if (ModelState.IsValid)
            {
                if (model.IMEInumberchecked == "False" || model.ExcessPaymentAgreed == "False")
                {
                    string error = string.Empty;
                    if (model.IMEInumberchecked == "False")
                        error = "IMEI number is not matched";
                    if (model.ExcessPaymentAgreed == "False")
                        error += "Excess Payment is not agreed";
                    //BookMobileCoverDeclineJob(DateTime.Now.AddDays(2).ToShortDateString(), 0, error, "", JobType.Accidental);
                    ViewBag.BookedMobileCoverDeclined = true;
                    ViewBag.Error = (model.IMEInumberchecked == "False") ? Settings.Default.IMEInumberNotchecked : (model.ExcessPaymentAgreed == "False" ? Settings.Default.ExcessPaymentNotAgreed : error);
                    ViewBag.Errormessge = error;
                    return View(model);
                }

                else
                {
                    BookService.SessionInfo.FaultDescr = string.Format("{1} ;IMEInumber: {0}", model.IMEInumber, BookService.SessionInfo.FaultDescr);
                    return Redirect(Url.ProcessNextStep());
                }
            }
            else
                return View(model);
        }
        [HttpGet]
        public ActionResult TheftDetails()
        {
            var applianceDetails = BookService.SessionInfo.CustProd;
            ViewBag.ApplianceInformation = string.Format("Model :<b> {0} </b><br/> Serial no: <b>{1}</b><br/> PolicyNumber: <b>{2}</b> <br/> Price :£{3} ", applianceDetails.ItemCode, applianceDetails.SerialNumber, applianceDetails.PolicyNumber, applianceDetails.AppliancePrice);
            if (ProductService.IsBackButtonPressed == true && !BookService.SessionInfo.CustProd.mobileTheft)
                return Redirect(Url.ProcessPreviousStep());
            else if (BookService.SessionInfo.CustProd.PolicyNumber.ToUpper().EndsWith("MPI") && BookService.SessionInfo.BookMobileADclaim)
                return RedirectToAction("MobileDetails");
            else if (!(BookService.SessionInfo.CustProd.PolicyNumber.ToUpper().EndsWith("MPI") && BookService.SessionInfo.CustProd.mobileTheft))
                return Redirect(Url.ProcessNextStep());

            else
            {
                MobileTheftInfo model = new MobileTheftInfo();
                model.InjectFrom(BookService.SessionInfo.CustProd.mobileTheftInfo);
                return View(model);

            }
        }

        [HttpPost]
        public ActionResult TheftDetails(MobileTheftInfo model)
        {
            ModelState.Clear();
            StringBuilder error = new StringBuilder();
            BookService.SessionInfo.Jobtype = JobType.Defaulttype;
            var applianceDetails = BookService.SessionInfo.CustProd;
            ViewBag.ApplianceInformation = string.Format("Model :<b> {0} </b><br/> Serial no: <b>{1}</b><br/> PolicyNumber <b>{2}</b>  ", applianceDetails.ItemCode, applianceDetails.SerialNumber, applianceDetails.PolicyNumber);

            if (model.CRNLogged == "False" || model.IMEInumberchecked == "False" || model.ExcessPaymentAgreed == "False")
            {
                string errormess = string.Empty; string Errormessge = string.Empty;
                if (model.CRNLogged == "False") // " Crime Reference number verification failed , " : "") + ((model.IMEInumberchecked == "False") ? " IMEI Reference number check failed, " : "") + ((model.ExcessPaymentAgreed == "False") ? " Excess Payment , " : "");
                {
                    errormess = Settings.Default.CrimenumberNotchecked;
                    Errormessge = "Crime Reference number verification failed";
                }
                else if (model.IMEInumberchecked == "False")
                {
                    errormess = Settings.Default.IMEInumberNotchecked;
                    Errormessge = "IMEI Reference number check failed";
                }
                else if (model.ExcessPaymentAgreed == "False")
                {
                    errormess = Settings.Default.ExcessPaymentNotAgreed;
                    Errormessge = "Excess Payment is not agreed";
                }
                ViewBag.BookedMobileCoverDeclined = true;
                ViewBag.Error = errormess;
                ViewBag.Errormessge = Errormessge;

                return View(model);
            }
            else
            {
                var validator = new MobilePhoneValidation();
                foreach (var errorval in validator.Validate(model, ruleSet: _ruleSets.defaultRule).Errors)
                {
                    ModelState.AddModelError(errorval.PropertyName, errorval.ErrorMessage);
                }

                if (ModelState.IsValid)
                {

                    if (model.IMEInumberchecked == "False")
                    {

                        error.AppendFormat("IMEI number is not matched, {0} .<br/>", model.IMEInumber);
                    }

                    if (model.CRNLogged == "False")
                    {
                        error.AppendFormat("Crime reference number is not logged.<br/>");
                    }
                    if (model.ExcessPaymentAgreed == "False")
                    {
                        error.AppendFormat("Excess payment is not agreed.<br/>");
                    }

                    if (error.Length == 0)
                    {

                        if (model.CrimeDate.Value < DateTime.Now.AddDays(-28))
                        {
                            //error.AppendFormat("Crime date is later than 28 days.<br/>");
                            ViewBag.AlertTheftdate = true;
                            return View(model);
                        }
                        BookService.SessionInfo.FaultDescr = string.Format("{4} Theft details :IMEInumber: {0}; Crime reference number {1} ; Theft Date {2} ; Theft Description : {3} ", model.IMEInumber, model.CrimeRefNo, model.CrimeDate, model.Description, BookService.SessionInfo.FaultDescr);
                        BookService.SessionInfo.Jobtype = JobType.Replacement;
                        return Redirect(Url.ProcessNextStep());
                    }
                    else
                    {
                        ViewBag.BookedMobileCoverDeclined = true;
                        ViewBag.Error = error.ToString();
                        BookMobileCoverDeclineJob(DateTime.Now.AddDays(2).ToShortDateString(), 0, error.ToString(), error.ToString(), JobType.Accidental);
                        return View(model);

                    }
                }
                return View(model);

            }
        }

        public ActionResult Mobiledecline(string errormess)
        {
            BookMobileCoverDeclineJob(DateTime.Now.AddDays(2).ToShortDateString(), 0, errormess.ToString(), errormess.ToString(), JobType.Accidental);
            return RedirectToAction("Index", "Home");
        }



        [HttpGet]
        public ActionResult SoftService()
        {
            //if (!BookService.SessionInfo.CustProd.mobileTheft)
            //{
            if (ProductService.IsBackButtonPressed == true && (ProductService.SessionInfo.TroubleShootId == 0 || BookService.SessionInfo.CustProd.PolicyNumber.ToUpper().EndsWith("MPI")))
                return Redirect(Url.ProcessPreviousStep());
            else if (ProductService.SessionInfo.TroubleShootId == 0)
                return Redirect(Url.ProcessNextStep());
            else if (BookService.SessionInfo.Jobtype == JobType.MobileCollection || BookService.SessionInfo.Jobtype == JobType.MobileReplacement)// && BookService.SessionInfo.BookMobileADclaim)
            {
                return Redirect(Url.ProcessNextStep());
            }
            else
            {

                TroubleshootModel TroubleshootModel = applianceService.GetApplianceTroubleShoot(ProductService.SessionInfo.TroubleShootId);
                return View(TroubleshootModel);
            }
            //}
            //else{
            //    return Redirect(Url.ProcessNextStep());
            //}
        }

        [HttpPost]
        public ActionResult SoftService(TroubleshootModel model)
        {
            return Redirect(Url.ProcessNextStep());
        }

        public ActionResult SaveSoftService(int TroubleshootQuestionID, int TroubleshootFaultID, bool issueFixed)
        {
            TroubleshootModel model = applianceService.GetApplianceTroubleShoot(ProductService.SessionInfo.TroubleShootId);
            var softservice = model.TroubleshootFaultlist.Where(x => x.TroubleshootFaultID == TroubleshootFaultID)
                  .SelectMany(n => n.troubleshootDetaillist).Where(y => y.TroubleshootQuestionID == TroubleshootQuestionID);
            //from parent in model.TroubleshootFaultlist
            //              where parent.troubleshootDetaillist.Find(c => c.TroubleshootQuestionID == TroubleshootQuestionID);
            //select parent.troubleshootDetaillist into p

            //select p;

            string TroubleShootText = softservice.OfType<TroubleshootDetail>().FirstOrDefault().TroubleshoorText;

            string softserviceTitle = softservice.OfType<TroubleshootDetail>().FirstOrDefault().TroubleShootTitle;
            if (issueFixed)
            {
                ViewBag.TroubleShootText = TroubleShootText;
                ViewBag.softserviceTitle = softserviceTitle;
                applianceService.UpdateTroubleShoot(BookService.SessionInfo.CustProd.CustAplId, TroubleshootQuestionID, TroubleshootFaultID);
                BookNow(DateTime.Now.AddDays(2).ToShortDateString(), 0, softserviceTitle + " " + TroubleShootText, "", JobType.SoftService);

                return View("IssueCorrected");
            }

            else
            {
                BookService.SessionInfo.appointmentModel.TroubleShootDescr = ".Customer tried the following trouble shoot : " + softserviceTitle + '.';
                BookService.SessionInfo.FaultDescr = string.Format("{0} ,{1}", BookService.SessionInfo.FaultDescr, BookService.SessionInfo.appointmentModel.TroubleShootDescr);
                return Redirect(Url.ProcessNextStep());
            }

        }

        //public ActionResult RequestDate()
        //{

        //    AppointmentModel appointment = new AppointmentModel();
        //    appointment.InjectFrom(BookService.SessionInfo);
        //    if (appointment.PreferredVisitDate == null)
        //        appointment.PreferredVisitDate = DateTime.Now.AddDays(3);
        //    //return View(appointment);

        //        AvailableRequestDetails request = new AvailableRequestDetails();
        //        AvailableResponseDetails response = new AvailableResponseDetails();
        //        request.PreferredVisitDate = appointment.PreferredVisitDate.Value.ToString(); 
        //         request.BookImmediately = false;
        //        request.BookToManual = true;
        //        request.BookingOptions = 5;
        //        request.Postcode = CustomerService.SessionInfo.Postcode;
        //        request.EngineerID = 1000;
        //        request.Skill = BookService.SessionInfo.Skills;
        //     response =   onlineBookingService.AvailabilityRequest(request);

        //     BookService.SessionInfo.FaultDescr = appointment.FaultDescr;
        //     if (response.RequestSuccess)
        //        {

        //            ViewBag.GetUnitError = string.Empty;
        //            var IsappointmentAvailable = response.AvailableList.Where(x => x.EventDate == appointment.PreferredVisitDate).OfType<TOneAvailableResult>();
        //         if (IsappointmentAvailable.Count() > 0)
        //            ViewBag.GetUnitSuccess = "Engineer is available for the date chosen ";

        //         else

        //        // else(model.PreferredVisitDate
        //             appointment.availabiltyModel = request;
        //         appointment.availabiltyResponseModel = response;
        //            ModelState.Clear();
        //        }
        //        else
        //        {
        //            ViewBag.GetUnitError = string.Format("Error: {0}", response.ErrorCode  );
        //            ViewBag.GetUnitSuccess =  response.ErrorText;
        //        }

        //    //model = BookService.FillJobPageLists(model, BookService.SessionInfo);
        //    //model.FileName = BookService.SessionInfo.UploadedFile.FileName;
        //   // ViewBag.SuperAdmin = storeService.IsSuperAdmin;
        //     return View("RequestDate", appointment);
        //}


        public ActionResult RequestDate()
        {

            AppointmentModel appointment = new AppointmentModel();
            appointment.InjectFrom(BookService.SessionInfo);
            appointment.Visitcodes = JobService.GetJobTypesList(string.Empty);
            appointment.FaultDescr = BookService.SessionInfo.FaultDescr;
            if (BookService.SessionInfo.Jobtype == JobType.Collection || BookService.SessionInfo.Jobtype == JobType.MobileCollection)
            {
                DateTime AppDate = DateTime.Now.AddDays(2);
                List<OneBookOptionResult> list = new List<OneBookOptionResult>();
                List<DateTime> Holidays = HomeService.GetHolidaysList(AppDate.Year);
                for (int i = 0; i < 20; i++)
                {
                    OneBookOptionResult bookOptionResult = new OneBookOptionResult();
                    bookOptionResult.EngineerID = specialJob.Where(x => x.JobTypeid == (int)BookService.SessionInfo.Jobtype).First().EngId;
                    bookOptionResult.EventDate = AppDate.AddDays(i);
                    if (!Holidays.Contains(bookOptionResult.EventDate) && bookOptionResult.EventDate.DayOfWeek != DayOfWeek.Saturday && bookOptionResult.EventDate.DayOfWeek != DayOfWeek.Sunday)
                        list.Add(bookOptionResult);

                    //  response.BookingOptionResult.Add( bookOptionResult);

                }
                appointment.availabiltyResponseModel.BookingOptionResult = list.GetRange(0, 5);
                ViewBag.Collection = "True";
                return View("RequestDate", appointment);
            }

            else if (BookService.SessionInfo.Jobtype == JobType.Replacement)
            {
                ViewBag.Replacement = true;
                return View("RequestDate", appointment);
            }

            if (!LowCost(BookService.SessionInfo.CustProd.CustAplId))
            {
                if (appointment.PreferredVisitDate == null)
                {
                    // var storeinfo = storeService.GetStoreInfo(storeService.StoreId);
                    DateTime AppDate = DateTime.Now.AddDays(storeinfo.Clientbookingdelaydays);
                    List<DateTime> Holidays = HomeService.GetHolidaysList(DateTime.Now.AddDays(storeinfo.Clientbookingdelaydays).Year);
                    while (Holidays.Contains(AppDate) || AppDate.DayOfWeek == DayOfWeek.Saturday || AppDate.DayOfWeek == DayOfWeek.Sunday)
                        AppDate = AppDate.AddDays(1);
                    appointment.PreferredVisitDate = AppDate;// DateTime.Now.AddDays(3)// DateTime.Now.AddDays(3);
                }
                //return View(appointment);

                RequestDetails request = new RequestDetails();
                ResponseDetails response = new ResponseDetails();
                request.SaediID = Settings.Default.SaediID; //"SHOPDIRECT";
                request.ClientID = storeService.StoreId;
                //  request.ClientPassword = "shopdirectCC";
                request.RequestedDate = appointment.PreferredVisitDate.Value.ToShortDateString();
                request.BookImmediately = false;
                request.BookingOptions = 5;
                request.Postcode = CustomerService.SessionInfo.Applianceaddress.Postcode;
                request.AddressLine1 = CustomerService.SessionInfo.Applianceaddress.Addr1;
                request.Town = CustomerService.SessionInfo.Applianceaddress.Town;
                request.ApplianceCode = BookService.SessionInfo.ApplianceCD;
                request.Skill = BookService.SessionInfo.Skills;

                BookService.SessionInfo.FaultDescr = appointment.FaultDescr;
                ViewBag.Clientbookingdelaydays = storeinfo.Clientbookingdelaydays;
                ViewBag.ShowAppointmentreason = true;// ShowAppointmentreason;
                if (!BookService.SessionInfo.OnlinebookingFailed)
                {
                    response = onlineBookingService.AppointmentRequest(request);



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

                    else if (!response.RequestSuccess && response.ErrorCode == 99 && !Offlinebooking)
                    {
                        ViewBag.GetUnitError = "No appointment available";

                        ModelState.Clear();
                    }
                    else
                    {
                        BookService.SessionInfo.AppointmentRetreiveFailed = true;

                        response = onlineBookingService.AppointmentRequestBackUp(request);
                        appointment.availabiltyModel = request;
                        appointment.availabiltyResponseModel = response;
                        return View("RequestDate", appointment);
                        //ViewBag.GetUnitError = string.Format("Error: {0}", response.ErrorCode);
                        //ViewBag.GetUnitSuccess = response.ErrorText;
                    }

                }
                else
                {

                    response = onlineBookingService.AppointmentRequestBackUp(request);
                    appointment.availabiltyModel = request;
                    appointment.availabiltyResponseModel = response;
                    return View("RequestDate", appointment);

                }
            }
            else
            {
                ViewBag.Replacement = "True";

                BookService.SessionInfo.Jobtype = JobType.Replacement;
                return View("RequestDate", appointment);
            }
            //model = BookService.FillJobPageLists(model, BookService.SessionInfo);
            //model.FileName = BookService.SessionInfo.UploadedFile.FileName;
            // ViewBag.SuperAdmin = storeService.IsSuperAdmin;
            return View("RequestDate", appointment);
        }
        [HttpPost]
        public ActionResult BookNow(string EventDate, int Engineerid, string faultdesc, string reason = "", JobType type = JobType.Defaulttype)
        {
            if (type == JobType.Defaulttype)
                type = BookService.SessionInfo.Jobtype;
            bool tempServiceId = false;
            var result = new OnlineBookResponseDetails();
            bool success = false;
            string FormPath = string.Empty;
            int serviceid = 0;
            string visitdate = string.Empty;
            string userid = storeService.UserId;
            var errorMsg = "Job Booking failed";
            var errorDetails = string.Empty;
            //if(Engineerid>0 && )
            //{
            OnlineBookRequestDetails model = new OnlineBookRequestDetails();
            model.InjectFrom(ProductService.SessionInfo);
            model.Postcode = CustomerService.SessionInfo.Applianceaddress.Postcode;
            model.EngineerID = Engineerid == 0 ? Settings.Default.DumpId : Engineerid;
            model.VisitDate = (EventDate == "") ? DateTime.Now.AddDays(storeinfo.Clientbookingdelaydays) : DateTime.Parse(EventDate);
            model.StatusID = 4;
            model.Model = ProductService.SessionInfo.ItemCode;
            model.CustomerID = BookService.SessionInfo.CustProd.CustomerId == 0 ? BookService.SessionInfo.CustomerId : BookService.SessionInfo.CustProd.CustomerId;
            model.CustAplID = BookService.SessionInfo.CustProd.CustAplId;// ProductService.SessionInfo.CustaplId;
            if (!BookService.SessionInfo.appointmentModel.IsGetAvailabiltyInfoPressed)
                BookService.SessionInfo.FaultDescr = faultdesc;
            model.ReportFault = string.IsNullOrEmpty(BookService.SessionInfo.appointmentModel.TroubleShootDescr) ? BookService.SessionInfo.FaultDescr : BookService.SessionInfo.appointmentModel.TroubleShootDescr + "Additional fault info:  " + BookService.SessionInfo.FaultDescr;
            model.SNO = BookService.SessionInfo.CustProd.SerialNumber;
            model.PolicyNumber = BookService.SessionInfo.CustProd.PolicyNumber;
            model.AuthNo = BookService.SessionInfo.CustProd.AuthNo;
            model.ClientID = storeService.StoreId;
            model.PolicyExpireyDate = BookService.SessionInfo.CustProd.contractexpires;//.HasValue? BookService.SessionInfo.CustProd.contractexpires.Value : new DateTime();
            model.VisitCode = reason == "" ? "000" : reason;
            model.ClientRef = BookService.SessionInfo.CustProd.ClientRef;

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
            if (!BookService.SessionInfo.OnlinebookingFailed)
            {

                var response = onlineBookingService.BookJob(model);
                if (response.BookSuccessfully)
                {
                    FormPath = string.Format("BookedJobConfirmation?Serviceid={0}&engineerid={1}&eventdate={2}&tempServiceId={3}", response.ServiceID, Engineerid, EventDate, tempServiceId);
                    success = response.BookSuccessfully;
                    serviceid = response.ServiceID;
                    visitdate = model.VisitDate.ToString();
                    ServiceModel serviceModel = new ServiceModel();
                    serviceModel.InjectFrom(response);
                    serviceModel.Engineerid = Engineerid;
                    serviceModel.CustomerID = BookService.SessionInfo.CustProd.CustomerId == 0 ? BookService.SessionInfo.CustomerId : BookService.SessionInfo.CustProd.CustomerId;
                    serviceModel.CustAplID = BookService.SessionInfo.CustProd.CustAplId;
                    serviceModel.EventDate = model.VisitDate.ToString();
                    serviceModel.Reportfault = model.ReportFault;
                    serviceModel.ClientId = model.ClientID;
                    serviceModel.VisitCode = model.VisitCode;
                    serviceModel.Clientref = model.ClientRef;
                    serviceModel.StatusId = model.StatusID;
                    int Serviceid = JobService.CreateJobwithEngineer(serviceModel); BookService.SessionInfo.ServiceId = serviceid;
                    if (SubmitTemplate)
                    {
                        var templateData = tempService.GetSpecificTemplateFieldsbyCstaplid(serviceModel.CustAplID);
                        onlineBookingService.AddInspection(templateData, serviceid, serviceModel.CustAplID);

                    }
                    if (Session["FirstDayOffered"] != null)
                    {
                        JobService.SaveAppointmentTrack(serviceid, (DateTime)Session["FirstDayOffered"], model.VisitDate);
                    }
                    //if (type != JobType.Defaulttype && type != JobType.Replacement)
                    //{
                    //    onlineBookingService.SetStatusId(response.ServiceID, 2, 0);
                    //}
                    if (model.StatusID != 4 && type != JobType.Defaulttype)
                    {
                        onlineBookingService.SetStatusId(response.ServiceID, model.StatusID, 0);
                    }
                    if (type == JobType.Replacement && !string.IsNullOrEmpty(BookService.SessionInfo.CustProd.mobileTheftInfo.IMEInumber))
                    {
                        var mobileTheftInfo = BookService.SessionInfo.CustProd.mobileTheftInfo;
                        onlineBookingService.AddNote(serviceid, "Customer", string.Format("Replacement has been booked :IMIE number :{0} ; Crimeref number: {1} ; Description :{2} ; {3} ", mobileTheftInfo.IMEInumber, mobileTheftInfo.CrimeRefNo, mobileTheftInfo.Description, mobileTheftInfo.CrimeDate.HasValue ? mobileTheftInfo.CrimeDate.Value.ToShortDateString() : ""));
                    }

                    else if (type == JobType.Replacement )
                    {
                        
                        onlineBookingService.AddNote(serviceid, "Customer", string.Format("Replacement has been requested as the Model {0} is not repairable. ", model.Model));
                    }
                    else if ((type == JobType.Collection || type == JobType.MobileCollection) && RestrictedAddressChange)
                        onlineBookingService.AddNote(serviceid, "Customer", "Collection job is booked. " + BookService.SessionInfo.ServiceNotes);
                    else if (type == JobType.PartsOnly)
                        onlineBookingService.AddNote(serviceid, "Customer", "PartsOnly job is booked. " + BookService.SessionInfo.PartOrderEnquiry);                   // onlineBookingService.AddNote(serviceid, "Customer", "Booked New Job");
                    else
                        onlineBookingService.AddNote(serviceid, "Customer", "Job is booked from Client connect");                   // onlineBookingService.AddNote(serviceid, "Customer", "Booked New Job");
                   
                    BookService.SessionInfo.ServiceId = serviceid;
                }
                else
                {
                    tempServiceId = true;
                    ServiceModel serviceModel = new ServiceModel();
                    serviceModel.InjectFrom(response);
                    serviceModel.AuthNo = BookService.SessionInfo.CustProd.AuthNo;
                    serviceModel.Engineerid = Engineerid;
                    serviceModel.CustomerID = BookService.SessionInfo.CustProd.CustomerId;
                    serviceModel.CustAplID = BookService.SessionInfo.CustProd.CustAplId;
                    serviceModel.EventDate = model.VisitDate.ToString();
                    serviceModel.Reportfault = model.ReportFault;
                    serviceModel.ClientId = model.ClientID;
                    serviceModel.VisitCode = model.VisitCode;
                    serviceModel.Clientref = model.ClientRef;
                    int serviceId = onlineBookingService.BookBackupJob(serviceModel, false, false, model.PolicyNumber);
                    if (SubmitTemplate)
                    {
                        var templateData = tempService.GetSpecificTemplateFieldsbyCstaplid(serviceModel.CustAplID);
                        // tempService.AddInspectionBackup(serviceId, serviceModel.CustAplID);
                        onlineBookingService.AddInspection(templateData, serviceid, serviceModel.CustAplID);
                    }
                    FormPath = string.Format("BookedJobConfirmation?Serviceid={0}&engineerid={1}&eventdate={2}&tempServiceId={3}", serviceId, Engineerid, EventDate, tempServiceId);
                    success = true;// response.BookSuccessfully;
                    //success = false;
                    //errorMsg = response.ErrorMsg;
                }
            }
            else
            {
                tempServiceId = true;
                ServiceModel serviceModel = new ServiceModel();
                serviceModel.AuthNo = BookService.SessionInfo.CustProd.AuthNo;
                serviceModel.Engineerid = Engineerid;
                serviceModel.CustomerID = BookService.SessionInfo.CustomerId;
                serviceModel.CustAplID = BookService.SessionInfo.CustProd.CustAplId;
                serviceModel.EventDate = model.VisitDate.ToString();
                serviceModel.Reportfault = model.ReportFault;
                serviceModel.ClientId = model.ClientID;
                serviceModel.VisitCode = model.VisitCode;
                serviceModel.Clientref = model.ClientRef;
                int serviceId = onlineBookingService.BookBackupJob(serviceModel, BookService.SessionInfo.CustAplCreationFailed, BookService.SessionInfo.CustomerCreationFailed, model.PolicyNumber);
                if (SubmitTemplate)
                {
                    var templateData = tempService.GetSpecificTemplateFieldsbyCstaplid(serviceModel.CustAplID);
                    onlineBookingService.AddInspection(templateData, serviceid, serviceModel.CustAplID);
                    //   tempService.AddInspectionBackup(serviceId, serviceModel.CustAplID);
                }
                FormPath = string.Format("BookedJobConfirmation?Serviceid={0}&engineerid={1}&eventdate={2}&tempServiceId={3}", serviceId, Engineerid, model.VisitDate.ToString(), tempServiceId);
                success = true;// response.BookSuccessfully;

            }
            //}
            //else
            return Json(new { success = success, FormPath = FormPath, errorMessage = errorMsg, tempServiceid = tempServiceId, type = (int)type, ApplianceType = (int)BookService.SessionInfo.DeviceType });
        }

        //[HttpGet]
        //public ActionResult DPACheck()
        //{

        //    int customerId = CustProductService.SessionInfo.CustomerId;

        //    int custAplId = CustProductService.SessionInfo.CustProductId;
        //    DPACheck model = new DPACheck();
        //    var Customer = CustomerService.GetCustomerInfo(customerId);
        //    model.InjectFrom(Customer);
        //    model.CustomerName = string.Format("{0} {1} {2}", Customer.Title, Customer.Forename, Customer.Surname);

        //    var custProd = CustProductService.GetCustomerProdInfo(custAplId);
        //    model.PolicyNumber = custProd.PolicyNumber;
        //    model.Appliance = custProd.ApplianceCD;
        //    model.Model = string.Format("{0} ({1})", custProd.ItemCode, custProd.Description);
        //    return View(model);
        //}

        //[HttpPost]
        //public ActionResult DPACheck(DPACheck dpaCheck, string NeedAdditionalCheck)
        //{
        //    BookService.SessionInfo.dpaCheck.InjectFrom(dpaCheck);
        //    //            if (dpaCheck.PolicyCheck && dpaCheck.AddressCheck && dpaCheck.AccountHolderCheck)
        //    //            {
        //    //                return Redirect(Url.ProcessNextStep());
        //    //            }
        //    //            else if(dpaCheck.PolicyCheck && dpaCheck.AccountHolderCheck && dpaCheck.ModelCheck && dpaCheck.EmailAddressCheck && dpaCheck.TelephoneCheck  )
        //    //            {
        //    //                return Redirect(Url.ProcessNextStep());
        //    //            }
        //    //            else if (!dpaCheck.AccountHolderCheck)
        //    //                ViewBag.FailedDPACheck = "We are happy to get a solution for your product as soon as possible but we can only do this by speaking with the main account holder. If the main account holder is there can I please speak with them?<br/>" +
        //    //                                        "If not, can you please ask them to call us to verify their details.";

        //    //            else if ((!dpaCheck.NameCheck || !dpaCheck.AddressCheck) && !(dpaCheck.TelephoneCheck && dpaCheck.ModelCheck && dpaCheck.EmailAddressCheck))
        //    //            {

        //    //                ViewBag.NeedAdditionalCheck = true;
        //    //                if (dpaCheck.TelephoneCheck || dpaCheck.ModelCheck || dpaCheck.EmailAddressCheck)
        //    //                {
        //    //                    if (!dpaCheck.PolicyCheck)
        //    //                    {
        //    //                        ViewBag.FailedDPACheck = "Unfortunately, I have not been able securely identify you as the main account holder. As we are committed to protecting our customers data I can only proceed with support when speaking with the main account holder.<br/> " +
        //    //                            "I need to speak to the main account holder who can verify the correct information stored on their account. If that is you please check these details on your account and give us a call back. <br/>If you are not the main account holder then please ask the right person to give us a call and we can proceed to support.";

        //    //                    }
        //    //                    else if (!dpaCheck.TelephoneCheck && dpaCheck.EmailAddressCheck && dpaCheck.ModelCheck)
        //    //                    {
        //    //                        ViewBag.FailedDPACheck = "Could it be possible that you have moved or change telephone number?<br/>" +
        //    //                       "If so could you give me the details that would be on the account please. ";
        //    //                        ViewBag.ShowAddressChange = true;
        //    //                    }
        //    //                    else
        //    //                    {
        //    //                        ViewBag.FailedDPACheck = "Unfortunately, I have not been able securely identify you as the main account holder. As we are committed to protecting our customers data I can only proceed with support when speaking with the main account holder.<br/> " +
        //    //"I need to speak to the main account holder who can verify the correct information stored on their account. If that is you please check these details on your account and give us a call back. <br/>If you are not the main account holder then please ask the right person to give us a call and we can proceed to support.";

        //    //                    }
        //    //                }
        //    //                else
        //    //                {
        //    //                    ViewBag.FailedDPACheck = "Unfortunately, I have not been able securely identify you as the main account holder. As we are committed to protecting our customers data I can only proceed with support when speaking with the main account holder.<br/> " +
        //    //"I need to speak to the main account holder who can verify the correct information stored on their account. If that is you please check these details on your account and give us a call back. <br/>If you are not the main account holder then please ask the right person to give us a call and we can proceed to support.";

        //    //                }
        //    //            }

        //    //            else if (!dpaCheck.AddressCheck || !dpaCheck.TelephoneCheck)
        //    //            {
        //    //                ViewBag.FailedDPACheck = "Could it be possible that you have moved or change telephone number?<br/>" +
        //    //"If so could you give me the details that would be on the account please. ";
        //    //                ViewBag.ShowAddressChange = true;
        //    //            }

        //    //            else
        //    //            {
        //    //                ViewBag.FailedDPACheck = "Unfortunately, I have not been able securely identify you as the main account holder. As we are committed to protecting our customers data I can only proceed with support when speaking with the main account holder.<br/> " +
        //    //"I need to speak to the main account holder who can verify the correct information stored on their account. If that is you please check these details on your account and give us a call back. <br/>If you are not the main account holder then please ask the right person to give us a call and we can proceed to support.";


        //    //            }

        //    if (!dpaCheck.AccountHolderCheck)
        //        ViewBag.FailedDPACheck = "We are happy to get a solution for your product as soon as possible but we can only do this by speaking with the main account holder. If the main account holder is there can I please speak with them?<br/>" +
        //                                "If not, can you please ask them to call us to verify their details.";
        //    else if (!dpaCheck.PolicyCheck)

        //        ViewBag.FailedDPACheck = "Unfortunately, I have not been able securely identify you as the main account holder. As we are committed to protecting our customers data I can only proceed with support when speaking with the main account holder.<br/> " +
        //            "I need to speak to the main account holder who can verify the correct information stored on their account. If that is you please check these details on your account and give us a call back. <br/>If you are not the main account holder then please ask the right person to give us a call and we can proceed to support.";

        //    else if (dpaCheck.PolicyCheck && dpaCheck.AddressCheck && dpaCheck.AccountHolderCheck && dpaCheck.NameCheck)
        //    {
        //        return Redirect(Url.ProcessNextStep());
        //    }
        //    else if (dpaCheck.PolicyCheck && dpaCheck.AccountHolderCheck && dpaCheck.ModelCheck && dpaCheck.EmailAddressCheck && dpaCheck.TelephoneCheck)
        //    {
        //        return Redirect(Url.ProcessNextStep());
        //    }
        //    else if (string.IsNullOrEmpty(NeedAdditionalCheck) && (!dpaCheck.AddressCheck || !dpaCheck.NameCheck))
        //    {
        //        ViewBag.NeedAdditionalCheck = true;
        //    }
        //    else if (!string.IsNullOrEmpty(NeedAdditionalCheck))
        //    {
        //        if (dpaCheck.PolicyCheck && dpaCheck.ModelCheck && dpaCheck.EmailAddressCheck)
        //        {
        //            if (dpaCheck.AddressCheck)
        //                return Redirect(Url.ProcessNextStep());
        //            else
        //            {
        //                //ViewBag.FailedDPACheck = "Could it be possible that you have moved or change telephone number?<br/>" +
        //                //                                "If so could you give me the details that would be on the account please. ";
        //                //ViewBag.NeedAdditionalCheck = true;
        //                ViewBag.ShowAddressChange = true;

        //                ViewBag.AddressChangeAlert = Url.ProcessNextStep();
        //            }
        //        }
        //        else
        //        {
        //            ViewBag.NeedAdditionalCheck = true;
        //            ViewBag.FailedDPACheck = "Unfortunately, I have not been able securely identify you as the main account holder. As we are committed to protecting our customers data I can only proceed with support when speaking with the main account holder.<br/> " +
        //                            "I need to speak to the main account holder who can verify the correct information stored on their account. If that is you please check these details on your account and give us a call back. <br/>If you are not the main account holder then please ask the right person to give us a call and we can proceed to support.";

        //        }
        //    }
        //    else if (string.IsNullOrEmpty(NeedAdditionalCheck))
        //    {
        //        ViewBag.FailedDPACheck = "Unfortunately, I have not been able securely identify you as the main account holder. As we are committed to protecting our customers data I can only proceed with support when speaking with the main account holder.<br/> " +
        //                             "I need to speak to the main account holder who can verify the correct information stored on their account. If that is you please check these details on your account and give us a call back. <br/>If you are not the main account holder then please ask the right person to give us a call and we can proceed to support.";

        //    }


        //    return View(dpaCheck);
        //}

        //[HttpGet]
        //public ActionResult AddressChange()
        //{
        //    return RedirectToAction("ChangeCustomerAddress");
        //    //var model = new CustomerPageModel();

        //    //var customer = CustomerService.GetCustomerInfo(CustProductService.SessionInfo.CustomerId);

        //    //model.InjectFrom(customer);
        //    //// }
        //    //model.CountryList = CustomerService.GetCountryList(model.Country);
        //    //model.ContactMethodList = CustomerService.GetContactMethodList(model.ContactMethod.ToString());
        //    //model.TitleList = CustomerService.GetTitlesList(model.Title);

        //    //return View(model);

        //}

        //[HttpPost]
        //public ActionResult AddressChange(CustomerPageModel model)
        //{
        //    ModelState.Clear();
        //    if (!string.IsNullOrEmpty(model.MobileTel)) model.MobileTel = model.MobileTel.Replace(" ", "");
        //    if (!string.IsNullOrEmpty(model.LandlineTel)) model.LandlineTel = model.LandlineTel.Replace(" ", "");
        //    if (string.IsNullOrEmpty(model.Country)) model.Country = "GB";



        //    //VALIDATION
        //    foreach (var error in Validator.Validate(model, _ruleSets.defaultRule))
        //    {
        //        ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        //    }
        //    if (ModelState.IsValid)
        //    {

        //        OnlineBookCustomerDetails request = new Services.OnlineBookCustomerDetails();
        //        request.InjectFrom(model);
        //        OnlineBookCustomerDetailsResponse res = onlineBookingService.SaveCustomer(request);


        //    }
        //    else
        //    {
        //        return View(model);
        //    }
        //    return Redirect(Url.ProcessNextStep());
        //}

        public ActionResult BookJob(AppointmentModel model)
        {
            if ((model.PreferredVisitDate == null) || ((DateTime)model.PreferredVisitDate) > DateTime.Now)
            {
                ModelState.AddModelError("PreferredVisitDate", "Please enter valid prefer date before processing.");
            }

            if (ModelState.IsValid)
            {
                BookService.SessionInfo.appointmentModel.InjectFrom(model);
                return View();
            }

            else
            {
                return View("RequestDate", model);
            }
        }


        public ActionResult BookedJobConfirmation(int Serviceid, int engineerid, string eventdate, bool tempServiceid = false)
        {
            JobBookingModel model = new JobBookingModel();

            model.customerModel.InjectFrom(CustomerService.SessionInfo);
            model.productmodel.InjectFrom(ProductService.SessionInfo);

            model.jobpageModel.ServiceId = Serviceid;
            model.EngineerId = engineerid;
            model.EventDate = eventdate;
            BookService.SessionInfo.ServiceId = Serviceid;
            ViewBag.ActionName = "RepairInstructionReport";
            ViewBag.ControllerName = "Reports";
            ViewBag.ReportName = @"/Reports/RepairInstructionReport";
            ViewBag.ServiceId = BookService.SessionInfo.ServiceId;
            ViewBag.ModelId = BookService.SessionInfo.ModelId;
            if (tempServiceid)
                ViewBag.tempServiceid = tempServiceid;
            return View("CustomerReportPage", model);

            //   return View(model);
        }
        /// <summary>
        /// create new job page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult JobPage()
        {
            Log.File.Info(BookService.Msg.GenerateLogMsg("View job info in booking process.", "Service id = " + BookService.SessionInfo.ServiceId.ToString()));
            var model = new JobPageModel();
            model.InjectFrom(BookService.SessionInfo);
            model.StatusId = BookService.SessionInfo.StatusID;

            //on opening unconfirmed job/ unapproved job, product session sno is null
            if (ProductService.SessionInfo.SerialNumber == null)
            {
                model.RepeatRepairChecked = BookService.SessionInfo.RepeatRepairChecked;
                model.RepeatDetected = BookService.SessionInfo.RepeatDetected;
            }
            else if (model.SerialNumber != ProductService.SessionInfo.SerialNumber)
            {
                model.RepeatRepairChecked = false;
                model.RepeatDetected = false;
            }
            if (model.DateOfPurchase.Year > 1900)
            {
                model.DateOfPurchaseString = Functions.DateTimeToString(model.DateOfPurchase);
            }

            //fill lists
            model = BookService.FillJobPageLists(model, BookService.SessionInfo);
            model.FileName = BookService.SessionInfo.UploadedFile.FileName;
            ViewBag.SuperAdmin = storeService.IsSuperAdmin;
            return View(model);
        }

        /// <summary>
        /// create new job page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult JobPage(JobPageModel model)
        {
            model.Guarantee = JobService.GetWarrantyDetails(model.WarrantyFg);
            if (model.SerialNumber != BookService.SessionInfo.SerialNumber)
            {
                model.RepeatRepairChecked = false;
                model.RepeatDetected = false;
                BookService.SessionInfo.IsGetUnitInfoPressed = false;
            }
            else
            {
                model.RepeatRepairChecked = true;
                model.RepeatDetected = BookService.SessionInfo.RepeatDetected;
            }
            //VALIDATION
            var validator = new BookNewService_JobPageValdation();
            foreach (var error in validator.Validate(model, ruleSet: _ruleSets.defaultRule).Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            if (!BookService.SessionInfo.IsGetUnitInfoPressed)
            {
                ModelState.AddModelError("SerialNumber", "Please use the Get Unit button to retrieve warranty information before processing.");
            }

            if (ModelState.IsValid)
            {
                model.RetailerName = JobService.GetRetailername(model.RetailerId);
                model.ServiceId = BookService.SessionInfo.ServiceId;
                model.DateOfPurchase = Functions.StringToDateTime(model.DateOfPurchaseString);
                BookService.SessionInfo.InjectFrom(model);
                BookService.SessionInfo.AdditionalFields =
                    Functions.ConvertListToNewListType<FieldsFromDBSessionModel, FieldsFromDB>(model.AdditionalFields.AdditionalFields);

                var answers = new AepModel();
                //JobAdditionalInfo.AEPFieldsFromDB.AepFields
                //foreach (var aepField in BookService.SessionInfo.AepInfo.AepFields)
                //{
                //     var answer = new AepResultModel();
                //     answer.InjectFrom(aepField);
                //     answers.AepFields.Add(answer);
                //}
                var aepAnswers = Functions.ConvertListToNewListType<FieldsFromDB, AepResultModel>(answers.AepFields);
                var Model = new AepModel();
                // var fieds = FieldsFromDbService.GetAepFields(BookService.SessionInfo.ServiceId, model.AepFields);
                //  Model.AepFields = Functions.ConvertListToNewListType<AepResultModel, FieldsFromDB>(fieds);
                //BookService.SessionInfo.AepInfo.AepFields =
                //    Functions.ConvertListToNewListType<AepSessionResultModel, FieldsFromDB>(fieds);
                BookService.SessionInfo.AepInfo.IsAepAviable = !String.IsNullOrEmpty(model.ModelPnc);
                return Redirect(Url.ProcessNextStep());
            }

            model.FileName = BookService.SessionInfo.UploadedFile.FileName;
            //fill lists
            model = BookService.FillJobPageLists(model, BookService.SessionInfo);
            model.FileName = BookService.SessionInfo.UploadedFile.FileName;
            return View(model);
        }

        [HttpPost]
        public ActionResult JobPageBackBtn(JobPageModel model)
        {
            model.DateOfPurchase = Functions.StringToDateTime(model.DateOfPurchaseString);
            BookService.SessionInfo.InjectFrom(model);
            BookService.SessionInfo.AdditionalFields =
                Functions.ConvertListToNewListType<FieldsFromDBSessionModel, FieldsFromDB>(model.AdditionalFields.AdditionalFields);
            return Redirect(Url.ProcessPreviousStep());
        }

        /// <summary>
        /// Upload file
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Uploadfile()
        {
            try
            {
                Log.File.Info(BookService.Msg.GenerateLogMsg("Uploading attached file.", "Service id = " + BookService.SessionInfo.ServiceId.ToString()));
                FileService = (FileService)Ioc.Get<FileService>();
                var file = FileService.GetFileFromRequest(Request);
                var extensions = new AllowedFileExtensions();
                //validation
                if (file.FileSize > 2000000)
                {
                    ModelState.AddModelError("FileName", "Wrong size.");
                    return Content("Error: File size must be less than 2Mb");
                }

                //validation
                if (file.FileName.Length > 50)
                {
                    ModelState.AddModelError("FileName", "File name is too long. Maximum 50 chars.");
                    return Content("Error. File name is too long. Maximum 50 chars.");
                }

                if (!extensions.IsExtensionAllowed(Path.GetExtension(file.FileName)))
                {
                    ModelState.AddModelError("FileName", "Wrong format.");
                    return Content("Error. Wrong file format");
                }

                //if all fine
                if (ModelState.IsValid)
                {
                    BookService.SessionInfo.UploadedFile = file;
                }
                return Content("File uploaded.");
            }
            catch (Exception)
            {
                return Content("Error in loading.");
            }
        }

        /// <summary>
        /// Upload file
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ClearFile()
        {
            try
            {
                Log.File.Info(BookService.Msg.GenerateLogMsg("Clearing attached file.", "Service id = " + BookService.SessionInfo.ServiceId.ToString()));
                BookService.SessionInfo.UploadedFile = new UploadedFileModel(); ;
                return Content("File cleared.");
            }
            catch (Exception)
            {
                return Content("Error while cleared.");
            }
        }

        //public ActionResult GetRepeatRepairInfo(JobPageModel model)
        //{
        //    RepeatRepairResult RepairResult = ProductService.RepeatRepairCheck(model.SerialNumber, model.ModelNumber);
        //    //if (RepairResult.RepeatDetectedFlag)
        //    //{
        //    //    return()

        //    //}
        //    model.RepeatRepairChecked = true;
        //    BookService.SessionInfo.RepeatRepairChecked = model.RepeatRepairChecked;
        //    BookService.SessionInfo.RepeatJobHistory = RepairResult.repeatHistory.HistoryResult;
        //    BookService.SessionInfo.RepeatDetected = RepairResult.RepeatDetectedFlag;
        //    return Json(new { result = RepairResult,success=true });
        //}
        ///// <summary>
        /// Press back button on page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult GetUnitInfo(JobPageModel model)
        {

            BookService.SessionInfo.IsGetUnitInfoPressed = true;
            //VALIDATION
            var validator = new BookNewService_JobPageValdation();
            foreach (var error in validator.Validate(model, ruleSet: _ruleSets.NewJob_SerialNumberOnly).Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            if (ModelState.IsValid)
            {

                //save model in session
                model.DateOfPurchase = Functions.StringToDateTime(model.DateOfPurchaseString);
                model.ServiceId = BookService.SessionInfo.ServiceId;
                model.RepeatDetected = BookService.SessionInfo.RepeatDetected;
                model.RepeatRepairChecked = BookService.SessionInfo.RepeatRepairChecked;
                BookService.SessionInfo.InjectFrom(model);
                BookService.SessionInfo.AdditionalFields =
                    Functions.ConvertListToNewListType<FieldsFromDBSessionModel, FieldsFromDB>(model.AdditionalFields.AdditionalFields);

                //RepeatRepairResult RepairResult=JobService. RepeatRepairCheck(model.SerialNumber);
                //if (RepairResult.RepeatDetectedFlag)
                //{
                //    return()

                //}
                //get unit reults
                var getunitResult = new GetUnitInfoResultModel();// _3CService.GettingUnitInfo(new GetUnitInfoModel
                //{
                //    modelName = BookService.SessionInfo.ItemCode,
                //    serialNumber = model.SerialNumber,
                //    purchaseCountry = BookService.GetCustomerDetails(BookService.SessionInfo.CustomerId).Country,
                //    purchaseDate = (!string.IsNullOrEmpty(model.DateOfPurchaseString)) ? Functions.StringToDateTime(model.DateOfPurchaseString) : (DateTime?)null
                //});

                if (getunitResult.successful)
                {
                    BookService.UpdateAepInfo(BookService.SessionInfo.ModelId, getunitResult.aepType);
                    model.ModelPnc = BookService.IsAepProduct(getunitResult.aepType) ? "AEP" : "";

                    //fill model with new values and save them in session
                    model = BookService.FillModelWithUnitInfo(model, getunitResult);
                    ViewBag.GetUnitError = string.Empty;
                    ViewBag.GetUnitSuccess = "Success";
                    ModelState.Clear();
                }
                else
                {
                    ViewBag.GetUnitError = string.Format("Error: {0} - {1}", getunitResult.errorCode,
                                                         getunitResult.errorMessage);
                    ViewBag.GetUnitSuccess = string.Empty;
                }
            }
            model = BookService.FillJobPageLists(model, BookService.SessionInfo);
            model.FileName = BookService.SessionInfo.UploadedFile.FileName;
            ViewBag.SuperAdmin = storeService.IsSuperAdmin;
            return View("JobPage", model);
        }

        public ActionResult RequestAlterDate(AppointmentModel appointment)
        {
            appointment.Visitcodes = JobService.GetJobTypesList(appointment.Visitcode ?? string.Empty);
            appointment.FaultDescr = appointment.FaultDescr ?? BookService.SessionInfo.FaultDescr;
            if (BookService.SessionInfo.Jobtype == JobType.Collection || BookService.SessionInfo.Jobtype == JobType.MobileCollection)
            {
                DateTime AppDate = DateTime.Now.AddDays(2);
                List<OneBookOptionResult> list = new List<OneBookOptionResult>();
                List<DateTime> Holidays = HomeService.GetHolidaysList(AppDate.Year);
                for (int i = 0; i < 20; i++)
                {
                    OneBookOptionResult bookOptionResult = new OneBookOptionResult();
                    bookOptionResult.EngineerID = specialJob.Where(x => x.JobTypeid == (int)BookService.SessionInfo.Jobtype).First().EngId;
                    bookOptionResult.EventDate = AppDate.AddDays(i);
                    if (!Holidays.Contains(bookOptionResult.EventDate) && bookOptionResult.EventDate.DayOfWeek != DayOfWeek.Saturday && bookOptionResult.EventDate.DayOfWeek != DayOfWeek.Sunday)
                        list.Add(bookOptionResult);

                    //  response.BookingOptionResult.Add( bookOptionResult);

                }
                appointment.availabiltyResponseModel.BookingOptionResult = list.GetRange(0, 5);
                ViewBag.Collection = "True";
                return View("RequestDate", appointment);
            }

            else if (BookService.SessionInfo.Jobtype == JobType.Replacement)
            {
                ViewBag.Replacement = true;
                return View("RequestDate", appointment);
            }

            if (!LowCost(BookService.SessionInfo.CustProd.CustAplId))
            {
                if (appointment.PreferredVisitDate == null)
                {
                    // var storeinfo = storeService.GetStoreInfo(storeService.StoreId);
                    DateTime AppDate = DateTime.Now.AddDays(storeinfo.Clientbookingdelaydays);
                    List<DateTime> Holidays = HomeService.GetHolidaysList(DateTime.Now.AddDays(storeinfo.Clientbookingdelaydays).Year);
                    while (Holidays.Contains(AppDate) || AppDate.DayOfWeek == DayOfWeek.Saturday || AppDate.DayOfWeek == DayOfWeek.Sunday)
                        AppDate = AppDate.AddDays(1);
                    appointment.PreferredVisitDate = AppDate;// DateTime.Now.AddDays(3)// DateTime.Now.AddDays(3);
                }
                //return View(appointment);

                RequestDetails request = new RequestDetails();
                ResponseDetails response = new ResponseDetails();
                request.SaediID = Settings.Default.SaediID; //"SHOPDIRECT";
                request.ClientID = storeService.StoreId;
                //  request.ClientPassword = "shopdirectCC";
                request.RequestedDate = appointment.PreferredVisitDate.Value.ToShortDateString();
                request.BookImmediately = false;
                request.BookingOptions = 5;
                request.Postcode = CustomerService.SessionInfo.Applianceaddress.Postcode;
                request.AddressLine1 = CustomerService.SessionInfo.Applianceaddress.Addr1;
                request.Town = CustomerService.SessionInfo.Applianceaddress.Town;
                request.ApplianceCode = BookService.SessionInfo.ApplianceCD;
                request.Skill = BookService.SessionInfo.Skills;

                BookService.SessionInfo.FaultDescr = appointment.FaultDescr;
                ViewBag.Clientbookingdelaydays = storeinfo.Clientbookingdelaydays;
                ViewBag.ShowAppointmentreason = true;
                if (!BookService.SessionInfo.OnlinebookingFailed)
                {
                    response = onlineBookingService.AppointmentRequest(request);



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

                    else if (!response.RequestSuccess && response.ErrorCode == 99 && !Offlinebooking)
                    {
                        ViewBag.GetUnitError = "No appointment available";

                        ModelState.Clear();
                    }
                    else
                    {
                        BookService.SessionInfo.AppointmentRetreiveFailed = true;

                        response = onlineBookingService.AppointmentRequestBackUp(request);
                        appointment.availabiltyModel = request;
                        appointment.availabiltyResponseModel = response;
                        return View("RequestDate", appointment);
                        //ViewBag.GetUnitError = string.Format("Error: {0}", response.ErrorCode);
                        //ViewBag.GetUnitSuccess = response.ErrorText;
                    }

                }
                else
                {

                    response = onlineBookingService.AppointmentRequestBackUp(request);
                    appointment.availabiltyModel = request;
                    appointment.availabiltyResponseModel = response;
                    return View("RequestDate", appointment);

                }
            }
            else
            {
                ViewBag.Replacement = "True";

                BookService.SessionInfo.Jobtype = JobType.Replacement;
                return View("RequestDate", appointment);
            }
            //model = BookService.FillJobPageLists(model, BookService.SessionInfo);
            //model.FileName = BookService.SessionInfo.UploadedFile.FileName;
            // ViewBag.SuperAdmin = storeService.IsSuperAdmin;
            return View("RequestDate", appointment);
        }
        public ActionResult GetAvailabilty(AppointmentModel model)
        {
            if (BookService.SessionInfo.BookMobileADclaim)
            {
                ViewBag.BookCollectionjob = true;
            }
            else if (BookService.SessionInfo.CustProd.mobileTheft)
            {
                ViewBag.mobileTheft = true;
            }
            // BookService.SessionInfo.appointmentModel.InjectFrom(model);
            BookService.SessionInfo.appointmentModel.IsGetAvailabiltyInfoPressed = true;
            BookService.SessionInfo.FaultDescr = model.FaultDescr;
            //VALIDATION
            //    var validator = new BookNewService_JobPageValdation();
            if (model.PreferredVisitDate == null || !model.PreferredVisitDate.HasValue)
            {
                ModelState.AddModelError(model.PreferredVisitDate.ToString(), "Please enter valid prefered date");
            }

            if (ModelState.IsValid)
            {

                RequestDetails request = new RequestDetails();
                ResponseDetails response = new ResponseDetails();
                request.SaediID = Settings.Default.SaediID;
                request.ClientID = storeService.StoreId;
                //  request.ClientPassword = "shopdirectCC";
                request.RequestedDate = model.PreferredVisitDate.Value.ToShortDateString();
                request.BookImmediately = false;
                request.BookingOptions = 5;
                request.Postcode = CustomerService.SessionInfo.Applianceaddress.Postcode;
                request.AddressLine1 = CustomerService.SessionInfo.Addr1;
                request.Town = CustomerService.SessionInfo.Town;
                request.ApplianceCode = BookService.SessionInfo.ApplianceCD;
                request.Skill = BookService.SessionInfo.Skills;
                if (!BookService.SessionInfo.OnlinebookingFailed)
                {
                    response = onlineBookingService.AppointmentRequest(request);
                    //       BookService.SessionInfo.FaultDescr = model.FaultDescr;
                    if (response.RequestSuccess)
                    {

                        ViewBag.GetUnitError = string.Empty;
                        var IsappointmentAvailable = response.BookingOptionResult.Any(x => x.EventDate.DayOfYear == model.PreferredVisitDate.Value.DayOfYear);
                        if (IsappointmentAvailable)
                            ViewBag.GetUnitSuccess = "Engineer is available for the date chosen ";
                        else
                        {
                            ViewBag.GetUnitSuccess = "Engineer is not available for the date chosen .Please choose one from following.";

                        }
                        Session["FirstDayOffered"] = response.BookingOptionResult.OrderBy(x => x.EventDate).FirstOrDefault().EventDate;
                        model.availabiltyModel = request;
                        model.availabiltyResponseModel = response;
                        ModelState.Clear();
                    }
                    else if (!response.RequestSuccess && !Offlinebooking)
                    {
                    }
                    else
                    {
                        BookService.SessionInfo.AppointmentRetreiveFailed = true;

                        response = onlineBookingService.AppointmentRequestBackUp(request);
                        model.availabiltyModel = request;
                        model.availabiltyResponseModel = response;
                        model.Visitcodes = JobService.GetJobTypesList(string.Empty);
                        ViewBag.ShowAppointmentreason = ShowAppointmentreason;
                        return View("RequestDate", model);
                        //ViewBag.GetUnitError = string.Format("Error: {0}", response.ErrorCode  );
                        //ViewBag.GetUnitSuccess =  response.ErrorText;
                    }
                }
                else
                {

                    BookService.SessionInfo.AppointmentRetreiveFailed = true;

                    response = onlineBookingService.AppointmentRequestBackUp(request);
                    model.availabiltyModel = request;
                    model.availabiltyResponseModel = response;
                    model.Visitcodes = JobService.GetJobTypesList(string.Empty);
                    ViewBag.ShowAppointmentreason = ShowAppointmentreason;
                    return View("RequestDate", model);


                }
            }
            model.Visitcodes = JobService.GetJobTypesList(string.Empty);
            //model = BookService.FillJobPageLists(model, BookService.SessionInfo);
            //model.FileName = BookService.SessionInfo.UploadedFile.FileName;
            // ViewBag.SuperAdmin = storeService.IsSuperAdmin;
            ViewBag.ShowAppointmentreason = ShowAppointmentreason;
            return View("RequestDate", model);
        }
        /// <summary>
        /// Aep page
        /// </summary>
        /// <returns>View</returns>
        [HttpGet]
        public ActionResult AepIndex()
        {
            //fill answers from session
            var answers = new AepModel();
            foreach (var aepField in BookService.SessionInfo.AepInfo.AepFields)
            {
                var answer = new AepResultModel();
                answer.InjectFrom(aepField);
                answers.AepFields.Add(answer);
            }

            var aepAnswers = Functions.ConvertListToNewListType<FieldsFromDB, AepResultModel>(answers.AepFields);
            var model = new AepModel();
            var fieds = FieldsFromDbService.GetAepFields(BookService.SessionInfo.ServiceId, aepAnswers);

            model.AepFields = Functions.ConvertListToNewListType<AepResultModel, FieldsFromDB>(fieds);
            Log.File.Info(BookService.Msg.GenerateLogMsg("View AEP info in booking process. Aep info: ", model.AepFields));
            if ((BookService.SessionInfo.SelectedType != DefaultValues.AepCode) || (model.AepFields.Count == 0))
            {
                if (BookService.IsBackButtonPressed)
                {
                    return Redirect(Url.ProcessPreviousStep());
                }
                return Redirect(Url.ProcessNextStep());
            }

            return View(model);
        }

        /// <summary>
        /// Aep page
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>View</returns>
        [HttpPost]
        public ActionResult AepIndex(AepModel model)
        {
            if (ModelState.IsValid)
            {
                //save fields into session
                BookService.SessionInfo.AepInfo = new AepSessionModel();
                var aepAnswers = Functions.ConvertListToNewListType<FieldsFromDB, AepResultModel>(model.AepFields);
                var result = FieldsFromDbService.GetAepFields(BookService.SessionInfo.ServiceId, aepAnswers);
                BookService.SessionInfo.AepInfo.IsAepAviable = true;
                foreach (var aepField in result)
                {
                    var field = new AepSessionResultModel();
                    field.InjectFrom(aepField);
                    BookService.SessionInfo.AepInfo.AepFields.Add(field);
                }
                return Redirect(Url.ProcessNextStep());
            }
            return View(model);
        }


        // in case of validation failure UK warranty contacts detail page
        [HttpGet]
        public ActionResult ContactDetails()
        {
            return View();
        }
        /// <summary>
        /// Show engineer choice
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EngineerChoice()
        {
            var model = new EngineerListModel();
            Log.File.Info(BookService.Msg.GenerateLogMsg("View engineer choice in booking process. ", "Customer id = " + BookService.SessionInfo.CustomerId));
            // get engineers list
            if (!BookService.GetCustomerDetails(BookService.SessionInfo.CustomerId).Country.Equals(DefaultValues.IrelandCountryCode))
            {
                model.InjectFrom(EngineerService.GetEngineers(BookService.SessionInfo.ModelId, BookService.StoreId, BookService.SessionInfo.CustomerId, BookService.SessionInfo.SelectedType, BookService.SessionInfo.RepeatDetected));
            }
            else
            {
                model.InjectFrom(EngineerService.GetEngineersForIreland(BookService.SessionInfo.ModelId, BookService.StoreId, BookService.SessionInfo.CustomerId, BookService.SessionInfo.SelectedType, BookService.SessionInfo.RepeatDetected));
            }
            ViewBag.RepeatDetected = BookService.SessionInfo.RepeatDetected;
            string[] ServiceTypes = { "097", "099", "098" };
            ViewBag.IsAEP = Array.Exists(ServiceTypes, x => x == BookService.SessionInfo.SelectedType);
            return View(model);
        }

        [HttpPost]
        public ActionResult EngineerChoice(EngineerListModel model)
        {
            if (model.SelectedId > 0)
            {
                BookService.SessionInfo.EngineerId = model.SelectedId;
                BookService.SessionInfo.EngineerSubAscId = EngineerService.GetEngineerDetails(model.SelectedId).EngineerSubAscId;
                return Redirect(Url.ProcessNextStep());
            }
            return View(model);
        }

        /// <summary>
        /// Confirm book
        /// </summary>
        /// <returns></returns>
        //      [HttpGet]
        public ActionResult ConfirmPage()
        {
            Log.File.Info(BookService.Msg.GenerateLogMsg("View confirm info in booking process. ", "Customer id = " + BookService.SessionInfo.CustomerId));
            var result = new ConfirmInfoModel();
            var customer = BookService.GetCustomerDetails(BookService.SessionInfo.CustomerId);
            var product = ProductService.GetDetails(BookService.SessionInfo.ModelId);
            var engineer = EngineerService.GetEngineerDetails(BookService.SessionInfo.EngineerId);
            var jobInfo = BookService.GetJobInfo(BookService.SessionInfo);
            result.InjectFrom(customer, product, engineer, jobInfo);
            result.RepeatDetected = BookService.SessionInfo.RepeatDetected;
            ViewBag.SuperAdmin = storeService.IsSuperAdmin;
            return View(result);
        }

        /// <summary>
        /// Confirm book
        /// </summary>
        /// <param name="model">Model info</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ConfirmPage(ConfirmInfoModel model)
        {
            var result = new ConfirmInfoModel();
            var customer = BookService.GetCustomerDetails(BookService.SessionInfo.CustomerId);
            var product = ProductService.GetDetails(BookService.SessionInfo.ModelId);
            var engineer = EngineerService.GetEngineerDetails(BookService.SessionInfo.EngineerId);
            ;
            var jobInfo = BookService.GetJobInfo(BookService.SessionInfo);
            result.InjectFrom(customer, product, engineer, jobInfo);
            //update status of job and assign engineer
            var saveResult = BookService.UpdateDiaryEntAndChangeStatus();
            // BookService.SessionInfo.AepInfo.IsAepAviable = model.ModelPnc;
            if (saveResult.IsSuccess)
            {
                return Redirect("/BookNewService/ReportPage"); //return Redirect("");//
            }
            else
                ViewBag.Error = saveResult.ErrorMessage;
            return View(model);
        }


        [HttpPost]
        public ActionResult ConfirmAEPPage(ConfirmInfoModel model)
        {
            var result = new ConfirmInfoModel();
            var customer = BookService.GetCustomerDetails(BookService.SessionInfo.CustomerId);
            var product = ProductService.GetDetails(BookService.SessionInfo.ModelId);
            var engineer = EngineerService.GetEngineerDetails(BookService.SessionInfo.EngineerId);
            ;
            var jobInfo = BookService.GetJobInfo(BookService.SessionInfo);
            result.InjectFrom(customer, product, engineer, jobInfo);
            //update status of job and assign engineer
            var saveResult = BookService.UpdateDiaryEntAndChangeStatus();
            // BookService.SessionInfo.AepInfo.IsAepAviable = model.ModelPnc;
            if (saveResult.IsSuccess)
            {
                return Redirect("/BookNewService/ReportPage");// return Redirect(Url.ProcessNextStep());
            }
            ViewBag.Error = saveResult.ErrorMessage;
            return View(model);
        }
        //  [HttpPost]
        public ActionResult RegisterRepeatJob(string value)
        {
            SaveServiceResult result = BookService.CreateServiceWithoutDiaryEnt();

            //if job was created not success
            if (!result.IsSuccess)
            {
                result.ErrorMessage = string.Format("Error on Server. Look in logs.");

                return RedirectToAction("ConfirmPage");
            }
            else
            {
                JobService.AddNote(BookService.SessionInfo.ServiceId, BookService.SessionInfo.RepeatJobHistory, "", "");
                return View(BookService.SessionInfo.ServiceId);
            }
        }


        [HttpPost]
        public ActionResult UpdateJobSONYStatus(Registration_SendModelResult model)
        {
            return Json(new { success = true });
        }
        /// <summary>
        /// Register Unit
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RegisterUnit(string value)
        {
            bool showForm = false;
            var errorMsg = "Unit registered failed";
            var successMsg = "Unit registered success";
            var formPath = string.Empty;

            var result = BookService.CreateServiceWithoutDiaryEnt();

            //if job was created not success
            if (!result.IsSuccess)
            {
                result.ErrorMessage = string.Format("Error on Server. Look in logs.");
                return Json(new { success = result.IsSuccess, successMessage = successMsg, errorDetails = "Error on Server. Look in logs.", errorMessage = "Error on Server.", formPath = string.Empty, showForm = false });
            }

            //if job already exist in session
            if (BookService.SessionInfo.IsSavedInto3CService)
            {
                result.IsSuccess = true;
                return Json(new { success = result.IsSuccess, successMessage = "Unit registered alredy", errorDetails = "", errorMessage = "", formPath = string.Empty, showForm = false });
            }

            // Generate model
            var model3C = BookService.Generate3CModel();

            if (!model3C.HasEngineer)
            {
                result.IsSuccess = false;
                return Json(new { success = result.IsSuccess, successMessage = "", errorDetails = "No Engineer in table 'SonyEngineerAddress'.", errorMessage = "No Engineer in Sony system.", formPath = string.Empty, showForm = false });
            }

            var resultRegUnit = new RegisterUnitResultModel();// _3CService.RegisteringUnit(model3C);
            result.IsSuccess = resultRegUnit.successful;

            if (resultRegUnit.successful)
            {
                //if problem with purchase date
                if ((resultRegUnit.successful) && (!resultRegUnit.purchaseDateAccepted))
                {
                    if (BookService.SessionInfo.IsSavedInto3CService) successMsg = "Call authorised already";
                    if (result.PurchaseDate != null)
                    {
                        showForm = true;
                        result.IsSuccess = false;
                        BookService.SessionInfo.SonyAuthResult.RegisterUnitResult.InjectFrom(resultRegUnit);
                        formPath = "/BookNewService/AcceptDop";
                    }
                    else
                    {
                        var attachmentFile = BookService.GenerateAttachModel();
                        if (attachmentFile.attachment == null)
                        {
                            showForm = true;
                            result.IsSuccess = false;
                            formPath = "/BookNewService/UploadDop";
                        }
                    }
                }
            }
            else
            {
                result.ErrorMessage = string.Format("Error: {0} - {1}", resultRegUnit.errorCode,
                                                    resultRegUnit.errorMessage);
            }

            return Json(new { success = result.IsSuccess, successMessage = successMsg, errorDetails = result.ErrorMessage, errorMessage = errorMsg, formPath = formPath, showForm = showForm });




            //if (result.IsSuccess)
            //{
            //    if (!BookService.SessionInfo.IsSavedInto3CService)
            //    {
            //        var model3C =
            //            BookService.Generate3CModel();

            //        var resultRegUnit = _3CService.RegisteringUnit(model3C);
            //        result.IsSuccess = resultRegUnit.successful;
            //        if (resultRegUnit.successful)
            //        {
            //            //if problem with purchase date
            //            if ((resultRegUnit.successful) && (!resultRegUnit.purchaseDateAccepted))
            //            {
            //                if (BookService.SessionInfo.IsSavedInto3CService) successMsg = "Call authorised already";
            //                if (result.PurchaseDate != null)
            //                {
            //                    showForm = true;
            //                    result.IsSuccess = false;
            //                    BookService.SessionInfo.SonyAuthResult.RegisterUnitResult.InjectFrom(resultRegUnit);
            //                    formPath = "/BookNewService/AcceptDop";
            //                }
            //                else
            //                {
            //                    var attachmentFile = BookService.GenerateAttachModel();
            //                    if (attachmentFile.attachment == null)
            //                    {
            //                        showForm = true;
            //                        result.IsSuccess = false;
            //                        formPath = "/BookNewService/UploadDop";
            //                    }
            //                }
            //            }
            //        }
            //        else
            //        {
            //            result.ErrorMessage = string.Format("Error: {0} - {1}", resultRegUnit.errorCode,
            //                                                resultRegUnit.errorMessage);
            //        }
            //    }
            //    else
            //    {
            //        result.IsSuccess = true;
            //        successMsg = "Unit registered alredy";
            //    }
            //}
            //else
            //{
            //    result.ErrorMessage = string.Format("Error on Server. Look in logs.");
            //}

            //return Json(new { success = result.IsSuccess, successMessage = successMsg, errorDetails = result.ErrorMessage, errorMessage = errorMsg, formPath = formPath, showForm = showForm });
        }

        /// <summary>
        /// Register AEP
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RegisterAEPReservation(string value)
        {
            var result = new SaveServiceResult();

            bool showForm = false;
            var errorMsg = "Create Aep failed";
            var errorDetails = string.Empty;
            var successMsg = "Aep created success";
            var formPath = string.Empty;

            var model3C = BookService.Generate3CModel();
            if (model3C.HasModelAEP)
            {
                if (!BookService.SessionInfo.IsSavedInto3CService)
                {
                    var resultRegAep = new CreateAEPSwapModelResultModel();// _3CService.RegisteringAep(model3C);
                    result.IsSuccess = resultRegAep.success;
                    if (resultRegAep.success)
                    {
                        BookService.SessionInfo.SonyAuthResult.AEPSwapResult.InjectFrom(resultRegAep);
                        formPath = "/BookNewService/AcceptAep";
                        showForm = true;
                    }
                    else
                    {
                        errorDetails = string.Format("Error: {0} - {1}", resultRegAep.errorCode,
                                                     resultRegAep.errorMessage);
                    }
                }
                else
                {
                    result.IsSuccess = true;
                    successMsg = "AEP registered already";
                }
            }
            else
            {
                result.IsSuccess = true;
                successMsg = "No AEP selected";
            }
            return Json(new { success = result.IsSuccess, successMessage = successMsg, errorDetails = errorDetails, errorMessage = errorMsg, formPath = formPath, showForm = showForm });
        }

        /// <summary>
        /// Register AEP
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RegisterNewServiceEvent(string value)
        {
            bool showForm = false;
            var errorMsg = "Call failed authorisation";
            var errorDetails = string.Empty;
            var successMsg = "Call authorised";
            var formPath = string.Empty;
            var result = new RegisterServiceEventResultModel();

            if (!BookService.SessionInfo.IsSavedInto3CService)
            {
                var model3C = BookService.Generate3CModel();
                result = new RegisterServiceEventResultModel();// _3CService.RegisteringNewService(model3C);
                if (result.successful)
                {
                    BookService.SessionInfo.CaseId = result.eventInfo.caseId;//"489746"; //
                    JobService.UpdateCaseId(BookService.SessionInfo.ServiceId, result.eventInfo.caseId);
                    var resultAttachment = new AddAttachmentResultModel();// _3CService.AttachFile(BookService.GenerateAttachModel());
                    result.successful = resultAttachment.success;
                    if (!resultAttachment.success)
                    {
                        errorDetails = string.Format("Error: {0} - {1}", resultAttachment.errorCode,
                                                     resultAttachment.errorMessage);
                    }
                    else
                    {
                        BookService.SessionInfo.IsSavedInto3CService = true;
                    }
                }
                else
                {
                    //if error about exist service and attachment file exist then attach file
                    var attachment = BookService.GenerateAttachModel();
                    if (result.errorCode.Equals("1305") && (attachment.attachment != null))
                    {
                        var resultAttachment = new AddAttachmentResultModel();// _3CService.AttachFile(attachment);
                        result.successful = resultAttachment.success;
                        if (!resultAttachment.success)
                        {
                            errorDetails = string.Format("Error: {0} - {1}", resultAttachment.errorCode,
                                                            resultAttachment.errorMessage);
                        }
                        else
                        {
                            BookService.SessionInfo.IsSavedInto3CService = true;
                        }
                    }
                    else
                    {
                        errorDetails = string.Format("Error: {0} - {1}", result.errorCode,
                                                        result.errorMessage);
                    }
                }
            }
            else
            {
                result.successful = true;
                successMsg = "Call authorised already";
            }
            return Json(new { success = result.successful, successMessage = successMsg, errorDetails = errorDetails, errorMessage = errorMsg, formPath = formPath, showForm = showForm });
        }

        /// <summary>
        /// Accept date of purchase
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AcceptDop()
        {
            var date = string.Empty;
            if (BookService.SessionInfo.SonyAuthResult.RegisterUnitResult.purchaseDate != null)
            {
                date = Functions.DateTimeToString(BookService.SessionInfo.SonyAuthResult.RegisterUnitResult.purchaseDate ?? DateTime.Now);
            }
            return View(model: date);
        }
        [HttpPost]
        public ActionResult AcceptDop(string value)
        {
            if (BookService.SessionInfo.SonyAuthResult.RegisterUnitResult.purchaseDate != null)
            {
                BookService.SessionInfo.DateOfPurchase = BookService.SessionInfo.SonyAuthResult.RegisterUnitResult.purchaseDate ?? DateTime.Now;
                JobService.UpdateDateOfPurchase(BookService.SessionInfo.ServiceId, BookService.SessionInfo.DateOfPurchase);
            }
            return RedirectToAction("ConfirmPage");
        }

        /// <summary>
        /// Accept date of purchase
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UploadDop()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadDop(string value)
        {
            return RedirectToAction("ConfirmPage");
        }


        /// <summary>
        /// Accept date of purchase
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AcceptAep()
        {
            var model = new AcceptAepModel();
            model.InjectFrom(BookService.SessionInfo.SonyAuthResult.AEPSwapResult);
            return View(model);
        }
        [HttpPost]
        public ActionResult AcceptAep(string value)
        {
            BookService.SessionInfo.AepBookingReference = BookService.SessionInfo.SonyAuthResult.AEPSwapResult.aepBookingReference;
            BookService.ChangeAepInfo(BookService.SessionInfo.SonyAuthResult.AEPSwapResult);
            return Json(new { success = true, successMessage = "Aep confirmed success.", error = false, errorMessage = "" });
        }


        public ActionResult ReportPage()
        {
            ViewBag.ActionName = "RepairInstructionReport";
            ViewBag.ControllerName = "Reports";
            ViewBag.ReportName = @"/Reports/RepairInstructionReport";
            ViewBag.ServiceId = BookService.SessionInfo.ServiceId;
            ViewBag.ModelId = BookService.SessionInfo.ModelId;
            return View();
        }



        public bool LowCost(int Custaplid)
        {
            //            If the “Market Value” of the item is less than £150.00.    The Market Value in Streamline means the original purchase price less 20% if breakdown occurs within 12 and 24 months from the purchase date and less a further 1% per month thereafter (12% per annum).
            //Therefore the portal will calculate the item Market Value and if this value is less an £150.00 the customer should be referred to the offline process for replacement.

            //  1.  APPLIANCEPRICE<150
            //    2.APPLIANCEPRICE >150   - deprecis <150


            //deprecis:

            //1 - 2 years 20% appliance  price

            //2 years >   =>
            //20% of appliance +(1 % for eachmonth after 2 years )
            bool IsLowCost = false;
            if (LowCostCal) // if low cost calculation Business rule activated 
            {
                //Double applianceCost = BookService.SessionInfo.CustProd.AppliancePrice;
                //int applnAgeInMonths = 0;

                //DateTime purchaseDate;
                //if (DateTime.TryParseExact(BookService.SessionInfo.CustProd.DateofPurchase, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out purchaseDate))
                //{
                //    //valid date
                //    applnAgeInMonths = (int)Math.Round(DateTime.Now.Subtract(purchaseDate).Days / (365.25 / 12));
                //    //  1.  APPLIANCEPRICE<150
                //    if (applianceCost < 150)
                //        IsLowCost = true;
                //    else if (applianceCost > 150 && applnAgeInMonths > 12 && applnAgeInMonths < 24)
                //    {

                //        IsLowCost = (applianceCost - applianceCost * 0.2) < 150;
                //    }
                //    else if (applianceCost > 150 && applnAgeInMonths > 24)
                //    {
                //        IsLowCost = (applianceCost - ((applianceCost * 0.2) + applnAgeInMonths * .01)) < 150;
                //    }
                //    else
                //        IsLowCost = false;
                //}
                //else
                //{
                //    IsLowCost = false;
                //}

                IsLowCost = CustProductService.CalculateLowCost(custAplid: Custaplid, StoreId: storeService.StoreId);
                if (IsLowCost)
                    BookService.SessionInfo.Jobtype = JobType.Replacement;
            }
            return IsLowCost;
        }



    }
}
