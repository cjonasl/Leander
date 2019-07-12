using System;
using System.Web.Mvc;
using System.Web.Security;
using ClientConnect.Configuration;
using ClientConnect.Infrastructure;
using ClientConnect.Models.Account;
using ClientConnect.Process;
using ClientConnect.Services;
using ClientConnect.Validation;
using ClientConnect.Validation.Account;
using ClientConnect.ViewModels.Customer;
using FluentValidation;
using Omu.ValueInjecter;
using ClientConnect.Models.Customer;
using ClientConnect.Customer;
using ClientConnect.Logging;
using System.Linq;
using ClientConnect.Models.JobStatuses;
using ClientConnect.Models.Appliance;
using PagedList;
using ClientConnect.Properties;
using ClientConnect.Models.BookRepair;
using ClientConnect.Products;
using ClientConnect.Home;
using System.Collections.Generic;
using ClientConnect.Models.Job;

namespace ClientConnect.Controllers
{

    public class CustomerController : Controller
    {
        private CustomerService customerService { get; set; }
        public CustCheckWorkFlowService DPAservice { get; set; }
        private CustomerModel CustomerModel { get; set; }
        private ApplianceService applianceService { get; set; }
        private StoreService storeService { get; set; }
        private BookNewServiceService BookService { get; set; }
        public List<SpecialJob> specialJob;
        private HomeService HomeService { get; set; } //
        private OnlineBookingService onlineBookingService { get; set; }   
        private JobService JobService { get; set; }
        // GET: /Customer/
        public CustomerController()
        { 
            HomeService = (HomeService)Ioc.Get<HomeService>();
            customerService = (CustomerService)Ioc.Get<CustomerService>();
            applianceService = (ApplianceService)Ioc.Get<ApplianceService>();
            DPAservice = (CustCheckWorkFlowService)Ioc.Get<CustCheckWorkFlowService>();
            storeService = (StoreService)Ioc.Get<StoreService>();
            BookService = (BookNewServiceService)Ioc.Get<BookNewServiceService>(); 
            onlineBookingService = (OnlineBookingService)Ioc.Get<OnlineBookingService>();
            var BusinessRules = HomeService.GetBusinessRuleList(storeService.StoreId);
            specialJob = HomeService.GetSpecialJobMappingList(storeService.StoreId);
            JobService = (JobService)Ioc.Get<JobService>();
        }
        public ActionResult ApplianceList()
        {
            var model  = customerService.GetCustomerInfo(customerService.SessionInfo.CustomerId);
            return View(model);
        }

        [HttpGet]
        public ActionResult DPAVerification( bool? success =false)
        {
            if (DPAservice.SessionInfo.DPAWorkflowSuccess)
            {
                return RedirectToAction("VerificationResult");
            }
            CustomerModel Custmodel = new CustomerModel();
            var model = customerService.GetCustomerInfo(customerService.SessionInfo.CustomerId);
            customerService.SessionInfo.InjectFrom(model);
            Custmodel.InjectFrom(customerService.SessionInfo);
            CustCheckWorkFlowModel flowmodel = new CustCheckWorkFlowModel();
            flowmodel.custCheckWorkFlow = DPAservice.GetCustCheckWorkFlow(storeService.StoreId);
            flowmodel.ProdInfo = model.ProductInfoModel;
            flowmodel.customerModel = Custmodel;
            DPAservice.SessionInfo.InjectFrom(model);
            DPAservice.SessionInfo.ProdInfo.Clear();
            foreach (var item in model.ProductInfoModel)
            {
                CustomerProduct_SessionModel prod = new CustomerProduct_SessionModel();
                prod.InjectFrom(item);
                if(prod.PolicyNumber!=null)
                prod.ApplianceSelected = prod.PolicyNumber.ToUpper() == customerService.SessionInfo.SearchCriteria.ToUpper();
                prod.CustAplId = item.CustaplId;
                DPAservice.SessionInfo.ProdInfo.Add(prod);
                if (prod.ApplianceSelected)
                    BookService.SessionInfo.SelectedCustAplid = item.CustaplId;

            }
            
            ViewBag.DPASuccess = success.Value;
            return View(flowmodel);
        
        }
        [HttpGet]

        public ActionResult VerificationResult()
        {
            CustomerModel Custmodel = new CustomerModel();
            var model = customerService.GetCustomerInfo(customerService.SessionInfo.CustomerId);
            customerService.SessionInfo.InjectFrom(model);
            Custmodel.InjectFrom(customerService.SessionInfo);
            CustCheckWorkFlowModel flowmodel = new CustCheckWorkFlowModel();
            
            flowmodel.ProdInfo = model.ProductInfoModel;
            flowmodel.customerModel = Custmodel;


            DPAservice.SessionInfo.InjectFrom(model);
            DPAservice.SessionInfo.DPAWorkflowSuccess = true;

            flowmodel.ProdInfo.Where(w => w.CustaplId ==  BookService.SessionInfo.SelectedCustAplid).ToList().ForEach(s => s.ApplianceSelected =true);
       
            return View(flowmodel);
        }
        [HttpPost]
        public ActionResult VerificationResult(CustCheckWorkFlowModel model)
        {
            return Redirect(Url.ProcessNextStep());

        }
        [HttpGet]
        public ActionResult DPAVerificationflow(string error)
        {
            CustomerModel Custmodel = new CustomerModel();
            Custmodel.InjectFrom(customerService.SessionInfo);
            
            CustCheckWorkFlowModel flowmodel = new CustCheckWorkFlowModel();
            flowmodel.SelectedCustAplId= BookService.SessionInfo.SelectedCustAplid;
            ViewBag.SelectedApplianceId = flowmodel.SelectedCustAplId;
            if (DPAservice.SessionInfo.custCheckWorkFlow.Count==0 )
            {
                flowmodel.custCheckWorkFlow = DPAservice.GetCustCheckWorkFlow(storeService.StoreId);
               
            }
            else
            {
                flowmodel.custCheckWorkFlow = DPAservice.SessionInfo.custCheckWorkFlow;
            

            }
            foreach (var item in DPAservice.SessionInfo.ProdInfo)
            {
                Product_InfoModel prodinfo = new Product_InfoModel();
                prodinfo.InjectFrom(item);
                prodinfo.CustaplId = item.CustAplId;
                flowmodel.ProdInfo.Add(prodinfo);
            }
            flowmodel.CurrentFlowQOrderID = DPAservice.SessionInfo.CurrentFlowQOrderID == 0 ? 2 : DPAservice.SessionInfo.CurrentFlowQOrderID;
            flowmodel.customerModel = Custmodel;
            
            DPAservice.SessionInfo.InjectFrom(flowmodel);
            DPAservice.SessionInfo.custCheckWorkFlow.InjectFrom(flowmodel.custCheckWorkFlow);
            if (flowmodel.SelectedCustAplId != 0  &&  BookService.SessionInfo.SelectedCustAplid!=0)
            {
               // var itemToRemove = flowmodel.ProdInfo.FindAll(x => x.CustaplId != flowmodel.SelectedCustAplId).OfType<Product_InfoModel>().ToList();
                flowmodel.ProdInfo.RemoveAll(x => x.CustaplId != flowmodel.SelectedCustAplId);
        }
            if(error!="")
                ViewBag.Error = error;
            
            return View(flowmodel);

        }
      
    

        [HttpPost]
        public ActionResult DPAVerification(CustCheckWorkFlowModel model, string comments, string SubmitClick)
        {
            CustomerModel Custmodel = new CustomerModel();
            Custmodel.InjectFrom(customerService.SessionInfo);

            model.customerModel.InjectFrom(Custmodel);
            BookService.SessionInfo.SelectedCustAplid = model.SelectedCustAplId;
            var DPAsessionmodel = DPAservice.SessionInfo.custCheckWorkFlow;
        
            model.custCheckWorkFlow = (DPAsessionmodel);
            foreach (var item in DPAservice.SessionInfo.ProdInfo)
            {
                Product_InfoModel prodinfo = new Product_InfoModel();
                prodinfo.InjectFrom(item);
                prodinfo.CustaplId = item.CustAplId;
                prodinfo.ApplianceSelected = (prodinfo.CustaplId == model.SelectedCustAplId);
                model.customerModel.ProductInfoModel.Add(prodinfo);
            }
            var currentWF = model.custCheckWorkFlow.Where(y => y.FlowQOrderID == DPAservice.SessionInfo.CurrentFlowQOrderID);
            var v = currentWF.Select(x => x.FieldName).First();
            string CurrentInputFieldName =   string.IsNullOrEmpty(currentWF.Select(x => x.FieldName).First()) ? string.Empty: currentWF.Select(x => x.FieldName).First().Trim().ToUpper();
            if (comments != string.Empty && CurrentInputFieldName.Length > 0 && CurrentInputFieldName == "CUSTOMERNOTES")
            {
               // NeedCustomerNote = true;
                comments = DateTime.Now.ToString() + " Main account holder granted the permission ." + comments;
               onlineBookingService. AddCustomerNote(Custmodel.CustomerId, storeService.UserId, comments);
               // BookService.CreateCustomer(Custmodel,comments);//todo:
            }
            if (SubmitClick=="YES")
            {
                

                if (!model.customerModel.ProductInfoModel.Exists(x => x.ApplianceSelected) && ((currentWF.First().FieldName==null) ? "N/A" :currentWF.First().FieldName.ToUpper() )== "APPLIANCE")
                {  // model.CurrentFlowQOrderID 
                    ViewBag.Error = "Please select Appliance. If you dont find the appliance information, please select NO.";
                       int? nextflowid = currentWF.Select(x => x.FlowQNoOrderID).First();
                    ViewBag.DPASuccess = !nextflowid.HasValue;
                }
                else
                {
                    int? nextflowid = currentWF.Select(x => x.FlowQYesOrderID).First();
                    if (nextflowid.HasValue)
                    {
                        model.CurrentFlowQOrderID = nextflowid.Value;
                    }

                   
                    ViewBag.DPASuccess = !nextflowid.HasValue;
                    
                }
               


            }
            else if (SubmitClick == "OK")
            {
                string page = currentWF.Select(x => x.FlowQYesPage).First();
                 string comment = currentWF.Select(x => x.Message).First();
                DPAFail(DPAservice.SessionInfo.CurrentFlowQOrderID,comment);
                //ViewBag.DPASuccess 
                ViewBag.returnUrl = "/Home/Index";
           
            }
            else if (SubmitClick == "NO")
            {
                int? nextflowid = currentWF.Select(x => x.FlowQNoOrderID).First();
                if (nextflowid.HasValue)
                {
                    model.CurrentFlowQOrderID = nextflowid.Value;
                }

             
                ViewBag.DPASuccess = !nextflowid.HasValue;

            } 
            else
            {
                int? nextflowid = currentWF.Select(x => x.FlowQNoOrderID).First();
                    model.CurrentFlowQOrderID = DPAservice.SessionInfo.CurrentFlowQOrderID;
                    ViewBag.DPASuccess = !nextflowid.HasValue;
                }
               
            DPAservice.SessionInfo.CurrentFlowQOrderID = model.CurrentFlowQOrderID;
           
              
            return View(model);

        }
        public void DPAFail(int flowid, string comment)
        {
           
            CustomerModel Custmodel = new CustomerModel();
            Custmodel.InjectFrom(customerService.SessionInfo);//todo:
            onlineBookingService.AddCustomerNote(Custmodel.CustomerId, storeService.UserId, comment);
           //  RedirectToAction("BookNow", "BookNewService", new { EventDate = DateTime.Now.AddDays(2).ToShortDateString(), Engineerid = 0, faultdesc = comment, reason = comment, type = JobType.DPAfailure });
            BookDPAFAILJob(DateTime.Now.AddDays(2).ToShortDateString(), 0, comment, comment, JobType.DPAfailure);
        }
        public void BookDPAFAILJob(string EventDate, int Engineerid, string faultdesc, string reason="",JobType type=JobType.Defaulttype)
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
          //  model.InjectFrom(ProductService.SessionInfo);
            model.Postcode = customerService.SessionInfo.Postcode;
            //model.EngineerID = Engineerid ;
            model.VisitDate =  DateTime.Parse(EventDate);
            //model.StatusID = 4;
          //  model.Model = DPAservice.SessionInfo. ProductService.SessionInfo.ItemCode;
            model.CustomerID = customerService.SessionInfo.CustomerId;// CustomerService.SessionInfo.CustomerId;
            model.CustAplID = onlineBookingService.addCustApl(model.CustomerID,JobType.DPAfailure) ;
            //model.CustAplID = BookService.SessionInfo.CustProd.CustAplId;// ProductService.SessionInfo.CustaplId;

            model.ReportFault = faultdesc;
            //model.SNO = BookService.SessionInfo.CustProd.SerialNumber;
            //model.PolicyNumber = BookService.SessionInfo.CustProd.PolicyNumber;
            //model.AuthNo = BookService.SessionInfo.CustProd.AuthNo;
            model.ClientID = storeService.StoreId;
           
            if (type != JobType.Defaulttype)
                {
                    if (specialJob != null && specialJob.Count() > 0)
                    {
                       model.EngineerID = specialJob.Where(x=>x.JobTypeid==(int)type).First().EngId;
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
               // return RedirectToAction("Index", "Home");
        }


        public ActionResult CustomerDetails(int id)
        {
            customerService.SessionInfo.CustomerId = id;
            return Redirect(Url.Process(PredefinedProcess.CustomerDetails));
        }
        public ActionResult GoToDetails()
        {

            return Redirect(Url.ProcessNextStep());
        }
        public ActionResult Search(int? page)
        {
            if (page == null) page = customerService.SessionInfo.PageNumber;
            customerService.SessionInfo.PageNumber = page ?? 1;

            string sc = customerService.SessionInfo.SearchCriteria;

            AdvSearchCriteria model = new AdvSearchCriteria(sc, sc, sc, sc, sc, sc, false);

            var result = customerService.GetCustomersList(model, customerService.SessionInfo.PageNumber);

            result.SearchCriteria = sc;

            // If only one product finded, redirect to details
            if (result.TotalRecords == 1 && !customerService.IsBackButtonPressed)
            {
                if (customerService.SessionInfo.FromIndex)
                {
                    ((ProcessService)Ioc.Get<ProcessService>()).RemoveCurrentProcess();
                }

                return RedirectToAction("CustomerDetails", new { id = result.SearchResults[0].CustomerId });
            }
            ViewBag.OnePageOfJobs = new StaticPagedList<Customer_SearchResult>(result.SearchResults, customerService.SessionInfo.PageNumber, Settings.Default.JobSearchPageSize, result.TotalRecords);
            return View(result);
        }
       // public ActionResult Details(int? Customerid)
         public ActionResult Details(int? CustomerId, bool allnotes, int? page)
        {
            //DPAservice.ClearFromSession();

            LoadCustomerDetails(CustomerId);

            if (allnotes)
            { 
                ViewBag.ShowAllNotes= true;
            }else
            ViewBag.AllPageOfNotes = CustomerModel.NotesModel.ToPagedList(page?? 1, 4);
            return View(CustomerModel);
            //Service=CustomerService.getServiceInfo();
           
        }

        private void LoadCustomerDetails(int? Customerid)
        {
            //if (customerService.SessionInfo == null)
            //{
                int id = 0;
                if (Customerid == null)
                    id = customerService.SessionInfo.CustomerId;
                else
                    id = Customerid.Value;
                var model = new Customer_InfoModel { CustomerId = id };

                CustomerModel = customerService.GetCustomerInfo(id);

                // CustomerService.SessionInfo.CustomerDetails.InjectFrom(CustomerModel);
                customerService.SessionInfo.InjectFrom(CustomerModel);
         
                customerService.SessionInfo.CustomerId = id;
            //}
        }


        // checking the customer has any closed claims (or for particular model) , then show the option to reopen the existing  claim
        public JsonResult ClaimConfirmMessage(int? CustomerId, int ModelId)
        {
            Log.File.Info("ClaimConfirmMessage is started...");
            if(CustomerId ==null)
                CustomerId = customerService.SessionInfo.CustomerId;

          
             var ShowWindow = false;
             string msg = "";
            
            try
            {  CustomerModel = customerService.GetCustomerInfo(CustomerId.Value);
                //var CustomerJobDetails = customerService.GetCustomerJobDetails(CustomerId.Value, ModelId);
           var jobexists=
                CustomerModel.ProductInfoModel.Where(x=>x.ModelId == ModelId && x.ServiceDetails.StatusId ==(int)Status.JobClosed).ToList();
           if (jobexists.Count > 0)
                {
                    ShowWindow = true;
                    msg = String.Format("<b> Open a new claim or Reopen the closed claim?</b> <br/>" +
                                       "You have a/some old claims for the same model. If you want to open new claim, click<b> OK</b><br/> or to open the closed claim click <b>No</b> ");
        }
                else
                {
                    ShowWindow = false;
                }

            }
            catch (Exception e)
            {
                Log.File.Error(string.Format("Claim Confirm Message error. Message: {0}", e.Message));
            }

            return Json(new { ShowWindow, msg,  }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FindCustomer(string customerSearchCriteria, bool? fromIndex)
        {

            customerService.SessionInfo.SearchCriteria = string.IsNullOrEmpty(customerSearchCriteria) ? string.Empty : customerSearchCriteria.Trim();
            customerService.SessionInfo.PageNumber = 1;
            customerService.SessionInfo.FromIndex = fromIndex.HasValue ? true : false;
            return Redirect(Url.Process(PredefinedProcess.CustSearch));
        }

        public ActionResult FindCustomerAdv()
        {
            return Redirect(Url.Process(PredefinedProcess.CustAdvSearch));
        }

        public ActionResult AdvSearch(AdvSearchCriteria model, int? page)
        {
            if (page.HasValue)
                customerService.SessionInfo.PageNumber = page.Value;

            if (Request.HttpMethod == "GET" && (customerService.SessionInfo.PageNumber == 0 || customerService.IsBackButtonPressed))
            {
                if (!customerService.IsBackButtonPressed && customerService.SessionInfo.PageNumber == 0)
                {
                    ViewBag.IsGet = true;
                    customerService.SessionInfo.PageNumber = 1;
                    return View(new Customer_SearchModel());
                }
                else
                {
                    customerService.SessionInfo.TakeFromSession = true;
                    return Redirect(Url.Process(PredefinedProcess.CustAdvSearch));
                }
            }
            else
            {
                if (page.HasValue || customerService.SessionInfo.TakeFromSession)
                {
                    model = new AdvSearchCriteria(
                        customerService.SessionInfo.Surname,
                        customerService.SessionInfo.Postcode,
                        customerService.SessionInfo.ClientCustRef,
                        customerService.SessionInfo.PolicyNumber,
                        customerService.SessionInfo.Address,
                        customerService.SessionInfo.TelNo,
                        true);
                }
                else
                {
                    page = customerService.SessionInfo.PageNumber;
                    customerService.SessionInfo.PageNumber = page ?? 1;
                    customerService.SessionInfo.Surname = string.IsNullOrEmpty(model.Surname) ? string.Empty : model.Surname;
                    customerService.SessionInfo.Postcode = string.IsNullOrEmpty(model.Postcode) ? string.Empty : model.Postcode;
                    customerService.SessionInfo.PolicyNumber = string.IsNullOrEmpty(model.PolicyNumber) ? string.Empty : model.PolicyNumber;
                    customerService.SessionInfo.TelNo = string.IsNullOrEmpty(model.TelNo) ? string.Empty : model.TelNo;
                    customerService.SessionInfo.ClientCustRef = string.IsNullOrEmpty(model.ClientCustRef) ? string.Empty : model.ClientCustRef;
                    customerService.SessionInfo.Address = string.IsNullOrEmpty(model.Address) ? string.Empty : model.Address;
                    model.UseAndInWhereCondition = true;
                }

                var result = customerService.GetCustomersList(model, customerService.SessionInfo.PageNumber);

                result.AdvSearchCriteria = model;

                if (result.TotalRecords == 1 && !customerService.SessionInfo.TakeFromSession)
                {
                    return RedirectToAction("CustomerDetails", new { id = result.SearchResults[0].CustomerId });
                }
                else if (result.TotalRecords > 1)
                {
                    ViewBag.OnePageOfJobs = new StaticPagedList<Customer_SearchResult>(result.SearchResults, customerService.SessionInfo.PageNumber, Settings.Default.JobSearchPageSize, result.TotalRecords);
                }

                customerService.SessionInfo.TakeFromSession = false;

                return View(result);
            }
        }
    }
}
