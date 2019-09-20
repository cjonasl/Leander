using ClientConnect.Configuration;
using ClientConnect.Logging;
using ClientConnect.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClientConnect.Process;
using Omu.ValueInjecter;
using ClientConnect.Products;
using ClientConnect.Home;

namespace ClientConnect.Controllers
{
    public class CustProdController : Controller
    {
         private CustomerService customerService { get; set; }
         public ProductService productService { get; set; }
        private ApplianceService applianceService { get; set; }
        private CustomerProductService customerProductService { get; set; }
        private StoreService storeService { get; set; }
        private HomeService HomeService { get; set; }
        public bool BookJobAllWarranty { get; set; }
        //
        public bool StopBookingClientModelMissing { get; set; }
        // GET: /Customer/
        public CustProdController()
        {
            customerService = (CustomerService)Ioc.Get<CustomerService>();
            productService = (ProductService)Ioc.Get<ProductService>();
            applianceService = (ApplianceService)Ioc.Get<ApplianceService>();
            customerProductService = (CustomerProductService)Ioc.Get<CustomerProductService>();
            storeService = (StoreService)Ioc.Get<StoreService>();
            HomeService = (HomeService)Ioc.Get<HomeService>();
            var BusinessRules = HomeService.GetBusinessRuleList(storeService.StoreId);
            try
            {
                BookJobAllWarranty = BusinessRules.Where(x => x.Key == BusinessRuleKey.BookJobAllWarranty.ToString()).FirstOrDefault().Checked;
            }
            catch
            {
                BookJobAllWarranty = false;
            }
            try
            {
                StopBookingClientModelMissing = BusinessRules.Where(x => x.Key == BusinessRuleKey.StopBookingClientModelMissing.ToString()).FirstOrDefault().Checked;
            }
            catch
            {
                StopBookingClientModelMissing = false;
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowDetails(int CustAplid)
        {
           // customerProductService.ClearSessionBeforeProcess();
            customerProductService.ClearFromSession();
                Log.File.Info(applianceService.Msg.GenerateLogMsg("Getting customer product details.", "custapl id = " + CustAplid.ToString()));
                customerProductService.SessionInfo.CustProductId = CustAplid;
             
            
              return Redirect(Url.ProcessNextStep());
        }
        public ActionResult ApplianceDetails()
        {
            if (customerProductService.SessionInfo.CustProductId != 0)
            {
                var model = customerProductService.GetCustomerAppliance(customerProductService.SessionInfo.CustProductId);

                customerProductService.SessionInfo.InjectFrom(model);
                //BusinessRules
                ViewBag.BookJobAllWarranty = BookJobAllWarranty;
                 var ProductRepairProfiles = productService.GetClientModelDetails(customerProductService.SessionInfo.ModelId,storeService.StoreId);

                 ViewBag.StopBookingClientModelMissing = StopBookingClientModelMissing && ProductRepairProfiles==null;
                return View(model);
            }
            else
                return RedirectToAction("Index","Home");
        }

        public ActionResult BookRepairShow(int Custaplid)
        {
           
            // customerProductService.ClearFromSession();

                var bookNewService = (BookNewServiceService)Ioc.Get<BookNewServiceService>();
                bookNewService.SessionInfo.InjectFrom(customerProductService.SessionInfo);
                //BookService.SessionInfo.CustomerId =customerProductService.SessionInfo.cu
                //bookNewService.SessionInfo.ModelId = customerProductService.SessionInfo.ModelId;
                var details = productService.GetDetails(customerProductService.SessionInfo.ModelId);
                
                productService.SessionInfo.InjectFrom(details);
               
                var CustProductModel = customerProductService.GetCustomerProdInfo(Custaplid);
                var ProductRepairProfiles = productService.GetClientModelDetails(customerProductService.SessionInfo.ModelId,storeService.StoreId);
                
                    bookNewService.SessionInfo.CustProd.InjectFrom(CustProductModel);
                    bookNewService.SessionInfo.CustProd.DateofPurchase = customerProductService.SessionInfo.DateofPurchase.HasValue ? customerProductService.SessionInfo.DateofPurchase.Value.ToShortDateString() : "";
                    bookNewService.SessionInfo.CustProd.ModelId = customerProductService.SessionInfo.ModelId;
                    bookNewService.SessionInfo.SerialNumber = customerProductService.SessionInfo.SerialNumber;
               if (ProductRepairProfiles == null)
                {
                    bookNewService.SessionInfo.Jobtype=JobType.ModelMissing;
                    bookNewService.SessionInfo.DeviceType = (DeviceType.Default);
                }
                else
                {     bookNewService.SessionInfo.Jobtype = ((DeviceType)ProductRepairProfiles.DeviceType == DeviceType.Mobile && ProductRepairProfiles.CollectRepairFlag) ? JobType.MobileCollection : (ProductRepairProfiles.CollectRepairFlag ? JobType.Collection : JobType.Defaulttype);
                    bookNewService.SessionInfo.DeviceType = (DeviceType)ProductRepairProfiles.DeviceType;//bookNewService.SessionInfo.RepairId
                }
                bookNewService.SessionInfo.TimeBookRepairClick = DateTime.Now;
                //bookNewService.SessionInfo.SoftId = details.SoftId;
            return Redirect(Url.Process(PredefinedProcess.CustjBook));
        }
        public ActionResult ManufactureWarrantyBookRepair(int Custaplid)
        {

            // customerProductService.ClearFromSession();

            var bookNewService = (BookNewServiceService)Ioc.Get<BookNewServiceService>();
            bookNewService.SessionInfo.InjectFrom(customerProductService.SessionInfo);
            //BookService.SessionInfo.CustomerId =customerProductService.SessionInfo.cu
            //bookNewService.SessionInfo.ModelId = customerProductService.SessionInfo.ModelId;
            var details = productService.GetDetails(customerProductService.SessionInfo.ModelId);

            productService.SessionInfo.InjectFrom(details);

            var CustProductModel = customerProductService.GetCustomerProdInfo(Custaplid);

            bookNewService.SessionInfo.CustProd.InjectFrom(CustProductModel);
            bookNewService.SessionInfo.CustProd.DateofPurchase = customerProductService.SessionInfo.DateofPurchase.HasValue ? customerProductService.SessionInfo.DateofPurchase.Value.ToShortDateString() : "";
            bookNewService.SessionInfo.CustProd.ModelId = customerProductService.SessionInfo.ModelId;
            bookNewService.SessionInfo.TimeBookRepairClick = DateTime.Now;

            return RedirectToAction("BookManufactureWarrantyJob", "BookNewService");
        }

        public ActionResult ExpiredWarrantyBookRepair(int Custaplid)
        {

            // customerProductService.ClearFromSession();

            var bookNewService = (BookNewServiceService)Ioc.Get<BookNewServiceService>();
            bookNewService.SessionInfo.InjectFrom(customerProductService.SessionInfo);
            //BookService.SessionInfo.CustomerId =customerProductService.SessionInfo.cu
            //bookNewService.SessionInfo.ModelId = customerProductService.SessionInfo.ModelId;
            var details = productService.GetDetails(customerProductService.SessionInfo.ModelId);

            productService.SessionInfo.InjectFrom(details);

            var CustProductModel = customerProductService.GetCustomerProdInfo(Custaplid);

            bookNewService.SessionInfo.CustProd.InjectFrom(CustProductModel);
            bookNewService.SessionInfo.CustProd.DateofPurchase = customerProductService.SessionInfo.DateofPurchase.HasValue ? customerProductService.SessionInfo.DateofPurchase.Value.ToShortDateString() : "";
            bookNewService.SessionInfo.CustProd.ModelId = customerProductService.SessionInfo.ModelId;
            bookNewService.SessionInfo.TimeBookRepairClick = DateTime.Now;

            return RedirectToAction("BookExpiredWarrantyJob", "BookNewService");
        }

        public ActionResult ModelMissingBookRepair(int Custaplid)
        {

            // customerProductService.ClearFromSession();

            var bookNewService = (BookNewServiceService)Ioc.Get<BookNewServiceService>();
            bookNewService.SessionInfo.InjectFrom(customerProductService.SessionInfo);
            //BookService.SessionInfo.CustomerId =customerProductService.SessionInfo.cu
            //bookNewService.SessionInfo.ModelId = customerProductService.SessionInfo.ModelId;
            var details = productService.GetDetails(customerProductService.SessionInfo.ModelId);

            productService.SessionInfo.InjectFrom(details);

            var CustProductModel = customerProductService.GetCustomerProdInfo(Custaplid);

            bookNewService.SessionInfo.CustProd.InjectFrom(CustProductModel);
            bookNewService.SessionInfo.CustProd.DateofPurchase = customerProductService.SessionInfo.DateofPurchase.HasValue ? customerProductService.SessionInfo.DateofPurchase.Value.ToShortDateString() : "";
            bookNewService.SessionInfo.CustProd.ModelId = customerProductService.SessionInfo.ModelId;
            bookNewService.SessionInfo.TimeBookRepairClick = DateTime.Now;

            return RedirectToAction("ModelMissingBookRepair", "BookNewService");
        }
        
    }
}
