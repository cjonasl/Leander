using System.Linq;
using System.Web.Mvc;
using CAST.Logging;
using CAST.Notifications;
using CAST.Process;
using CAST.Products;
using CAST.Properties;
using CAST.Services;
using CAST.Jobs;

namespace CAST.Controllers
{
    /// <summary>
    /// Controller for product task
    /// </summary>
    public class ProductController : DataController
    {
        /// <summary>
        /// Product data access object
        /// </summary>
        private readonly ProductService _product;

        /// <summary>
        /// Job service
        /// </summary>
        readonly JobService _jobService;
        
        /// <summary>
        /// Constructor
        /// </summary>
        public ProductController()
        {
            _product = new ProductService(Data);
            _jobService = new JobService(Data);
        }

        /// <summary>
        /// Set criteria parameter and page number in Session and redirect
        /// </summary>
        /// <param name="productSearchCriteria">Criteria of search</param>
        /// <param name="pageNum">Number of page, which must to show</param>
        /// <returns>Redirect to search</returns>
        public ActionResult FindProduct(string productSearchCriteria)
        {
            _product.SetSearchCriteria(productSearchCriteria);
            _product.SetPageNumOfSearchResult(1);
            return Redirect(Url.Process(PredefinedProcess.ProductSearch));
        }

        /// <summary>
        /// Default search page
        /// </summary>
        /// <returns>List of products</returns>
        public ActionResult Search(int? pageNum)
        {
            if (pageNum == null) pageNum = _product.GetPageNumOfSearchResult();
            var model = _product.GetProducts(_product.GetSearchCriteria(), pageNum, Settings.Default.ProductSearchPageSize);
            
            _product.SetPageNumOfSearchResult(pageNum ?? 1);

            // If only one product finded, redirect to details
            if (model.ElemCount == 1)
            {
                // Checking if user push back button on ShowDetails
                if (Request.UrlReferrer.ToString().IndexOf("Product/ShowDetails") > 0)
                    return View(model);
                return RedirectToAction("Details", new { modelID = model.SearchResults[0].ModelID, modelNumber = model.SearchResults[0].Model });
            }
            else if(model.ElemCount ==0)
                return RedirectToRoute("Default", new { controller = "Home", action = "Index", id = UrlParameter.Optional });
            return View(model);
        }
        
        /// <summary>
        /// Get alternative products view
        /// </summary>
        /// <param name="modelId">Model id</param>
        /// <returns></returns>
        public ActionResult GetAlternativeProducts(int modelId)
        {
            var model = _product.GetAlternativeProductsList(modelId);
            if (model != null) return PartialView("~/Views/Product/_AlternativeProducts.cshtml", model);
         
            return null;
        }

        /// <summary>
        /// Save ID of model in Session and redirect to show a details
        /// </summary>
        /// <param name="modelId">ID of model</param>
        /// <returns>Redirect ot Show details action</returns>
        public ActionResult Details(int modelId, string modelNumber)
        {
            _product.SetModelId(modelId);
            
            // Get model info for log Product View
            var log = new Log(Data);
            log.Database.ProductView.AddNote(modelId);
            return Redirect(Url.Process(PredefinedProcess.ProductDetails));
        }

        /// <summary>
        /// Show details of product
        /// </summary>
        /// <returns>View of details</returns>
        public ActionResult ShowDetails()
        {
            if (_product.GetModelId() > 0)
            {
                var model = _product.GetDetails();
                model.AdditioalLinks = _product.GetAddServicesLinks();

                //set parameters in session
                if (model.IsCallCenter)
                    _product.SetBookingParameters(model.ModelNum, model.Description, model.SoftId, model.CCRepairFaq, model.ItemCode,model.ModelBrand,model.RepairID);
                else
                    _product.SetBookingParameters(model.ModelNum, model.Description, model.SoftId, model.RepairFaq, model.ItemCode, model.ModelBrand,model.RepairID);

                return View(model);
            }
            return new HttpNotFoundResult();
            
        }

        /// <summary>
        /// Saved time click and redirect to Book repair page
        /// </summary>
        /// <param name="modelNum"> The model number   </param>
        /// <param name="descr"> The Description of model.   </param>
        /// <param name="softId"> The soft Id.  </param>
        /// <param name="processId"> The process Id. </param>
        /// <returns> Redirect to the book repair   </returns>
        public ActionResult BookRepairShow(int? processId)
        {
            if (processId != null)
            {
                // clear customer info from session
                var custState = new CustomerService(Data);
                custState.ClearInfoFromSession();

                // clear job and repair info from session
                var book = new BookRepairService(Data);
                book.ClearInfoFromSession();

                // engineer 
                var engr = new EngravingService(Data);
                engr.ClearInfoFromSession();
                
                // register job
                var regjob = new RegisterJobService(Data);
                regjob.ClearInfoFromSession();

                // collect info
                var collect = new CollectRepairJobService(Data);
                collect.ClearInfoFromSession();


                // clear previous state of book repair so old values won't appear on forms
                return Redirect(Url.Process((int)processId));
            }
            return Redirect(Url.Process(PredefinedProcess.ProductDetails));
        }


        /// <summary>
        /// Updates product information
        /// </summary>
        /// <param name="prodInfoModel">Product info</param>
        /// <returns>Empty result</returns>
        [HttpPost]
        public ActionResult UpdateProductInfo(Product_InfoModel prodInfoModel)
        {
            if (!ModelState.IsValid)
            {
                var errorResult = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { MemberNames = x.Key, ErrorMessage = x.Value.Errors.ElementAt(0).ErrorMessage })
                    .ToList();

                return Json(errorResult);
            }

            else
            {
                _product.UpdateProductInfo(prodInfoModel);
                _jobService.AddNote("Change product info");
                return new EmptyResult();
            }
        }

        /// <summary>
        /// Sms sending
        /// </summary>
        /// <param name="model">Model of details</param>
        /// <returns>Refresh page</returns>
        public ActionResult SendSmsNotification(Product_DetailModel model)
        {
            var notification = new Notifications_Sender(Data);
            notification.ProductNotificationSms(model.NotificationMobileNumber, _product.GetModelId());
            return RedirectToAction("ShowDetails");
        }

        /// <summary>
        /// Email sending
        /// </summary>
        /// <param name="model">Model of details</param>
        /// <returns>Refresh page</returns>
        public ActionResult SendEmailNotification(Product_DetailModel model)
        {

            // Send notification on email
            var notification = new Notifications_Sender(Data);
            if (_product.GetModelId() != null)
                notification.ProductNotificationEmail(model.NotificationEmailAddress, _product.GetModelId());
            return RedirectToAction("ShowDetails");
        }

        /// <summary>
        /// Show product details report
        /// </summary>
        /// <param name="ModelId">Model id</param>
        /// <returns>Redirect to report page</returns>
        public ActionResult ShowDetailsReport(int? ModelId)
        {
            _product.SetModelId(ModelId ?? 0);
            return Redirect(Url.Process(PredefinedProcess.ShowProductDetailsReport));
     
        }

         [HttpPost]
        public ActionResult UpdateAdditionalInfo(Job_DetailsModel Model)
        {
            InspectionService insp = new InspectionService(Data);
           
            //if (!ModelState.IsValid)
            //{

            //    var errorResult = ModelState
            //        .Where(x => x.Value.Errors.Count > 0)
            //        .Select(x => new { MemberNames = x.Key, ErrorMessage = x.Value.Errors.ElementAt(0).ErrorMessage })
            //        .ToList();

            //    return Json(errorResult);
            //}

            //else
            //{
            insp.UpdateAdditionalInfo(Model.ProductInformation.Additionalfields);
                // Add log record in ServiceUsage
            //    Log.Database.Job.Add.Updated(_bookRepair.GetServiceIdFromSession());
                _jobService.AddNote("Change additional info");
                return new EmptyResult();
            }
        

    }
}
