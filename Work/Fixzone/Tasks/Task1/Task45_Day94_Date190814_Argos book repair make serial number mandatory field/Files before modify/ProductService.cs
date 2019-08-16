using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CAST.Controllers;
using CAST.Infrastructure;
using CAST.Logging;
using CAST.Models.Product;
using CAST.Process;
using CAST.Products;
using CAST.Properties;
using CAST.Repositories;
using CAST.Sessions;
using CAST.UserAccount;

namespace CAST.Services
{
    public class ProductService 
    {
        /// <summary>
        /// Product data access object
        /// </summary>
        private readonly ProductRepository _reporsitory;
        
        /// <summary>
        /// Data context
        /// </summary>
        private DataContext _dataContext;

        /// <summary>
        /// Represents _bookRepairState of bookRepair processes
        /// </summary>
        private readonly ProductStateHolder _productStateHolder;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data"></param>
        public ProductService(DataContext data)
        {
            _dataContext = data;
            _reporsitory = new ProductRepository(data);
            _productStateHolder = new ProductStateHolder();
        }

        
    #region PRODUCT SEARCH

        /// <summary>
        /// Set search criteria
        /// </summary>
        /// <param name="criteria"></param>
        public void SetSearchCriteria(string criteria)
        {
            var product = _productStateHolder.Load();
            product.SearchCriteria = criteria;
            _productStateHolder.UpdateFrom(product);
        }

        /// <summary>
        /// Set search criteria
        /// </summary>
        /// <param name="criteria"></param>
        public void SetPageNumOfSearchResult(int page)
        {
            var product = _productStateHolder.Load();
            product.PageNumOfSearchResult = page;
            _productStateHolder.UpdateFrom(product);
        }


        /// <summary>
        /// Get product list by search criteris
        /// </summary>
        /// <param name="searchCriteria">Search criteris</param>
        /// <param name="pageNum">Current page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns></returns>
        public Product_SearchModel GetProducts(string searchCriteria, int? pageNum, int pageSize)
        {
            var model = new Product_SearchModel();
            model.SearchCriteria = searchCriteria;
            
            // set page number = 1 if empty
            if (pageNum == null) pageNum = 1;

            if (!string.IsNullOrEmpty(searchCriteria))
            {
                // get products
                var list = _reporsitory.GetProductsList(searchCriteria, pageNum.Value, pageSize);

                // if list empty
                if ((list.Count == null) || (list.Count == 0))
                {
                    model.SearchResults = new List<Product_SearchResult>();
                    model.PageNum = (int)pageNum;
                    model.StartElem = 0;
                    model.ElemCount = 0;
                    model.EndElem = 0;

                    //Paginator fields
                    model.PaginatorInfo.ElemCount = 0;
                    model.PaginatorInfo.CurrentPage = (int) pageNum;
                    model.PaginatorInfo.ItemsPerPage = 15;

                }
                else
                {
                    model.PageNum = (int)pageNum;
                    model.SearchResults = list;
                    model.StartElem = list[0].StartElem;
                    model.ElemCount = list[0].ElemCount;
                    model.EndElem = list[0].LastElem;

                    //Paginator fields
                    model.PaginatorInfo.ElemCount = list[0].ElemCount;
                    model.PaginatorInfo.CurrentPage = (int)pageNum;
                    model.PaginatorInfo.ItemsPerPage = 15;
                }
            }
            else
            {
                model.SearchResults = new List<Product_SearchResult>();
                model.PageNum = (int)pageNum;
                model.StartElem = 0;
                model.ElemCount = 0;
                model.EndElem = 0;

                //Paginator fields
                model.PaginatorInfo.ElemCount = 0;
                model.PaginatorInfo.CurrentPage = (int)pageNum;
                model.PaginatorInfo.ItemsPerPage = 15;
            }

            return model;
        }


#endregion


#region GET PRODUCT DETAILS
        /// <summary>
        /// Replace parametres
        /// </summary>
        /// <param name="urlForReplace">string for changing</param>
        /// <param name="model">parametres</param>
        /// <returns>Replaced string</returns>
        private string ChargeableUrlReplace(string urlForReplace, ProductDetailsModel model)
        {
            urlForReplace = urlForReplace.Replace("~model~", model.ItemCode)
                          .Replace("~Model~", model.ItemCode)
                          .Replace("~mfr~", model.MFR)
                          .Replace("~altcode~", model.ModelNum);

            return urlForReplace;
        }

        /// <summary>
        /// filter support service
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private ProductDetailsModel FilterSupporService(ProductDetailsModel model)
        {
            //filtering product support
            model.ProductServices.SupportService.ServiceName = "Product Support";
            model.ProductServices.SupportService.ServiceImageSrc = "/Content/Icons/ContactCentreActivity.PNG";
            //if (model.ProductServices.SupportService.ServiceFlag)
            //{
            //    var func = new Functions();
               
            //        if (!string.IsNullOrEmpty(model.Model))
            //        {
            //            model.ProductServices.SupportService.ServiceText = func.BuildLinkHtmlString("btn-argos",
            //                                                                                       "Product Support Site",
            //                                                                                       System.Configuration.ConfigurationManager.AppSettings["SupportGuideUrl"] + model.Model,
            //                                                                                       true);
            //    }
            //}
            return model;
        }

        /// <summary>
        /// filter repair service
        /// </summary>
        /// <param name="model">model</param>
        /// <returns>model</returns>
        private ProductDetailsModel FilterRepairService(ProductDetailsModel model)
        {
            model.ProductServices.RepairService.ServiceImageSrc = "/Content/Icons/BookRepair.PNG";
            model.ProductServices.RepairService.ServiceName = model.IsCallCenter ? "Register" : "Store Repair";
            var func = new Functions();
            if (model.ProductServices.RepairService.ServiceFlag)
            {
                //filtering repair
                model.ProductServices.RepairService.ServiceText = func.BuildLinkHtmlString("btn-argos", "Book",
                                                                                            "/Product/BookRepairShow?processId=" +
                                                                                            model.BookRepairProcessId);
            }
            
            //if call center then show Register
            if ((model.IsCallCenter) && model.ProductServices.RepairService.ServiceFlag)
            {
                model.ProductServices.RepairService.ServiceUrlText = string.Empty;
                model.ProductServices.RepairService.ServiceEngineerTelNo = string.Empty;
                model.ProductServices.RepairService.ServiceEngineerNotes = string.Empty;
                model.ProductServices.RepairService.ServiceEngineerName = "";
                              //"Return the product to store where a repair is available.";
                model.BookRepairProcessId = (int)PredefinedProcess.RegisteringJob;
              //  model.ProductServices.RepairService.ServiceText = func.BuildLinkHtmlString("btn-argos-disabled",
               //                                                                            "Register", "#");

                model.ProductServices.RepairService.ServiceText = func.BuildLinkHtmlString("btn-argos", "Register",
                                                                                         "/Product/BookRepairShow?processId=" +
                                                                                         model.BookRepairProcessId);
            }

            return model;
        }

        /// <summary>
        /// filter free spares service
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private ProductDetailsModel FilterFreeSparesService(ProductDetailsModel model)
        {
            model.ProductServices.FreeSparesService.ServiceImageSrc = "/Content/Icons/FreeSpares.PNG";
            model.ProductServices.FreeSparesService.ServiceName = "Free Spares";
          // model.ProductServices.FreeSparesService.Model = "";
            if (model.ProductServices.FreeSparesService.ServiceFlag)
            {
                var func = new Functions();
                //filtering free spares
                if (model.ProductServices.FreeSparesService.ServiceEngineerName != null &&
                    model.ProductServices.FreeSparesService.ServiceEngineerName.ToUpper().Equals("CLICK SPARES"))
                {
                    var _store = new StoreService(_dataContext);
                        var _user = new UserService(_dataContext);
                   
                    bool Iscallcenter = _store.IsCallCenter();
                    string UserId = _user.GetUserId();
                    string Storeid = Iscallcenter? "500":_store.GetStoreId().ToString();
                    model.ProductServices.FreeSparesService.ServiceUrlText = Settings.Default.FreeSparesUrl.Replace("{CatalogueNum}", model.ItemCode).Replace("{empId}", UserId
                        ).Replace("{storeId}", Storeid);
                  
                }

                if (model.PartAvailable)
                {
                    model.ProductServices.FreeSparesService.ServiceText = func.BuildLinkHtmlString("btn-argos", "Order",
                                                                                                   model.ProductServices
                                                                                                        .FreeSparesService
                                                                                                        .ServiceUrlText,
                                                                                                   true);
                }
                else
                {
                    model.ProductServices.FreeSparesService.ServiceText = func.BuildLinkHtmlString(
                        "btn-argos-disabled", "Order", "#");
                    model.ProductServices.FreeSparesService.ServiceUrlText = string.Empty;
                }
            }
            return model;
        }

        /// <summary>
        /// filter collect service
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private ProductDetailsModel FilterCollectService(ProductDetailsModel model)
        {
            model.ProductServices.CollectService.ServiceImageSrc = "/Content/Icons/BookRepair.PNG";
            model.ProductServices.CollectService.ServiceName = "Collect/Repair";

            //filtering collect
            model.ProductServices.CollectService.ServiceFlag = model.ProductServices.CollectService.ServiceFlag &&
                                                               model.IsCallCenter;
            var func = new Functions();
                
            if (!model.ProductServices.CollectService.ServiceFlag)
            {
                model.ProductServices.Remove(model.ProductServices.Keys.CollectKey);
            }
            else
            {
                model.ProductServices.CollectService.ServiceText = func.BuildLinkHtmlString("btn-argos",
                                                                                            "Collection",
                                                                                            "/Product/BookRepairShow?processId=" +
                                                                                            (int)PredefinedProcess.CollectJob);
            }
            
            return model;
        }

        /// <summary>
        /// filter support service
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private ProductDetailsModel FilterChargeSparesService(ProductDetailsModel model)
        {
            model.ProductServices.ChargeableService.ServiceImageSrc = "/Content/Icons/ChargeableSpares.PNG";
            model.ProductServices.ChargeableService.ServiceName = "Chargeable Spares";
            if (model.ProductServices.ChargeableService.ServiceFlag)
            {
                var func = new Functions();
                //filtering chargeable
                model.ProductServices.ChargeableService.ServiceUrlText =
                    model.ProductServices.ChargeableService.ServiceUrlText != null
                        ? ChargeableUrlReplace(model.ProductServices.ChargeableService.ServiceUrlText, model)
                        : null;
                model.ProductServices.ChargeableService.ServiceText = "Chargeable Spares";
                
                if (string.IsNullOrEmpty(model.ProductServices.ChargeableService.ServiceUrlText))
                {
                    model.ProductServices.ChargeableService.ServiceText = func.BuildLinkHtmlString(
                        "btn-argos-disabled", "Argos Spares", "#");
                }
                else
                {
                    model.ProductServices.ChargeableService.ServiceText = func.BuildLinkHtmlString("btn-argos",
                                                                                                   "Argos Spares",
                                                                                                   model.ProductServices
                                                                                                        .ChargeableService
                                                                                                        .ServiceUrlText,
                                                                                                   true);
                }
            }
            return model;
        }

        /// <summary>
        /// Filter product details
        /// </summary>
        /// <param name="model">Product details</param>
        /// <returns>filtered details</returns>
        private ProductDetailsModel FilterServicesInfo(ProductDetailsModel model)
        {
            // image file
            if (!string.IsNullOrEmpty(model.ImageFileName))
            {
                model.ImageFileName = string.Format(Settings.Default.ProductImageUrlTemplate, model.ImageFileName);
            }
            else model.ImageFileName = "/Content/Icons/photo_not_available.png";

            // Call center or not
            var _store = new StoreService(_dataContext);
            model.IsCallCenter = _store.IsCallCenter();
            
            //filtering
            model = FilterSupporService(model);
            model = FilterCollectService(model);
            model = FilterRepairService(model);
            model = FilterFreeSparesService(model);
            model = FilterChargeSparesService(model);

            return model;
        }

        /// <summary>
        /// Get model ID
        /// </summary>
        /// <returns></returns>
        public int GetModelId()
        {
            var prod = _productStateHolder.Load();
            return prod.ModelID.HasValue ? prod.ModelID.Value : 0;
        }


        public bool HasModelId()
        {
            return GetModelId() > 0;
        }
        /// <summary>
        /// Get model number
        /// </summary>
        /// <returns></returns>
        public string GetModelNumber()
        {
            var prod = _productStateHolder.Load();
            return prod.ModelNumber;
        }

        /// <summary>
        /// Get model number
        /// </summary>
        /// <returns></returns>
        public string GetModelDescr()
        {
            var prod = _productStateHolder.Load();
            return prod.ModelDescription;
        }

        /// <summary>
        /// Get product details
        /// </summary>
        /// <returns></returns>
        public ProductDetailsModel GetDetails()
        {
            var prod = _productStateHolder.Load();
            var model = _reporsitory.GetDetails(prod.ModelID.Value);
            return FilterServicesInfo(model);
        }

        /// <summary>
        /// Get Additional services links
        /// </summary>
        /// <returns></returns>
        public List<Product_AdditServiceModel> GetAddServicesLinks()
        {
            var prod = _productStateHolder.Load();
            return _reporsitory.GetAddServicesLins(prod.ModelID.Value);
        }

        /// <summary>
        /// Get general info about product from session
        /// </summary>
        /// <returns>Model of general info</returns>
        public Product_InfoModel GetGeneralInfoFromSession()
        {
            var prod = _productStateHolder.Load();
            return new Product_InfoModel
                       {
                           ItemNumber = prod.ModelNumber,
                           ItemCode = prod.ItemCode,
                           Description = prod.ModelDescription,
                           OriginalCondition = prod.OriginalCondition,
                           SerialNumber = prod.SerialNumber,
                           TransactionInfo = prod.TransactionNumber,
                           DateOfPurchase = prod.DateOfPurchase,
                           RepairFaq = prod.RepairFaq,
                           Brand = prod.Brand,
                           ModelBrand=prod.ModelBrand
                       };
        }

        /// <summary>
        /// Get general info about product from session
        /// </summary>
        /// <returns>Model of general info</returns>
        public Product_InfoModel GetGeneralInfo(int modelId)
        {
            var prod = _reporsitory.GetDetails(modelId);
            var prodSt = _productStateHolder.Load();
            return new Product_InfoModel
            {
                ItemNumber = prod.ModelNum,
                Description = prod.Description,
                OriginalCondition = prodSt.OriginalCondition,
                SerialNumber = prodSt.SerialNumber,
                TransactionInfo = prodSt.TransactionNumber,
                DateOfPurchase = prodSt.DateOfPurchase,
                ItemCode = prod.ItemCode,
                Brand =prod.Brand
            };
        }

        /// <summary>
        /// Return search criteria
        /// </summary>
        /// <returns></returns>
        public string GetSearchCriteria()
        {
            var prod = _productStateHolder.Load();
            return prod.SearchCriteria;
        }
        
        /// <summary>
        /// Return page number
        /// </summary>
        /// <returns></returns>
        public int GetPageNumOfSearchResult()
        {
            var prod = _productStateHolder.Load();
            return prod.PageNumOfSearchResult;
        }

        /// <summary>
        /// Get soft if for product
        /// </summary>
        /// <returns></returns>
        public int? GetSoftIdFromSession()
        {
            var prod = _productStateHolder.Load();
            return prod.SoftId;
        }

        /// <summary>
        /// Get alternative product list
        /// </summary>
        /// <param name="prodId"></param>
        /// <returns></returns>
        public Product_SearchModel GetAlternativeProductsList(int modelId)
        {
            var result = new Product_SearchModel();
            result.SearchResults = _reporsitory.GetAlternativeProductsList(modelId);
            return result;
        }

        /// <summary>
        /// Get model Id by model
        /// </summary>
        /// <param name="modelNumber"></param>
        /// <returns></returns>
        public int GetModelIdByModelNumber(string modelNumber)
        {
            return _reporsitory.GetModelIdByModelNumber(modelNumber) ?? 0;
        }

        /// <summary>
        /// Get List of products
        /// </summary>
        /// <param name="searchCriteria">searching criteria</param>
        /// <param name="pageNum">page</param>
        /// <param name="pageSize">items on one page</param>
        /// <returns></returns>
        public List<Product_SearchResult> GetProductsList(string searchCriteria, int pageNum, int pageSize)
        {
            return _reporsitory.GetProductsList(searchCriteria, pageNum, pageSize);
        }

        #endregion

#region PRODUCT UPDATE
        /// <summary>
        /// Update product info
        /// </summary>
        /// <param name="prodInfoModel">Product model</param>
        public void UpdateProductInfo(Product_InfoModel prodInfoModel)
        {
            if (prodInfoModel.DateOfPurchase.Year < 1900)
                prodInfoModel.DateOfPurchase = new DateTime(1900, 1, 1);

            // TODO validate
            _reporsitory.UpdateProductInfo(new UpdateProductInfoCommand
            {
                CustaplId = prodInfoModel.CustaplId,
                DateOfPurchase = prodInfoModel.DateOfPurchase,
                Description = prodInfoModel.Description,
                DiaryId = prodInfoModel.DiaryId,
                ItemNumber = prodInfoModel.ItemNumber,
                ModelId = prodInfoModel.ModelId,
                OriginalCondition = prodInfoModel.OriginalCondition,
                SerialNumber = prodInfoModel.SerialNumber,
                TransactionInfo = prodInfoModel.TransactionInfo
            });

            // Add log record in ServiceUsage
            var job = new JobService(_dataContext);
            var Log = new Log(_dataContext);
            Log.Database.Job.Add.Updated(job.GetJobIdFromSession());
        }

#endregion

#region SET INFO
        /// <summary>
        /// Set model id
        /// </summary>
        /// <param name="modelId">Model id</param>
        public void SetModelId(int modelId)
        {
            var prod = _productStateHolder.Load();
            prod.ModelID = modelId;
            _productStateHolder.UpdateFrom(prod);
        }

        /// <summary>
        /// Set parameters for booking
        /// </summary>
        /// <param name="modelNum">Model number</param>
        /// <param name="descr">Description of model</param>
        /// <param name="softId">Soft id</param>
        public void SetBookingParameters(string modelNum, string descr, int softId, string repairFaq,string ItemCode,string ModelBrand)
        {
            var prod = _productStateHolder.Load();
            prod.TimeBookRepairClick = DateTime.Now;
            prod.ModelNumber = modelNum;
            prod.ModelDescription = descr;
            prod.SoftId = softId;
            prod.RepairFaq = repairFaq;
            prod.ItemCode = ItemCode;
            prod.ModelBrand = ModelBrand;
            _productStateHolder.UpdateFrom(prod);
        }

        /// <summary>
        /// Get Date time when 'Book' button was pressed
        /// </summary>
        /// <returns></returns>
        public DateTime? GetTimeBookRepairClick()
        {
            var prod = _productStateHolder.Load();
            return prod.TimeBookRepairClick ?? DateTime.Now;
        }


        public void SetGeneralProductInfoInSession(Product_InfoModel model)
        {
            var prod = _productStateHolder.Load();
            prod.ModelNumber = model.ItemNumber;
            prod.ModelDescription = model.Description;
            prod.OriginalCondition = model.OriginalCondition;
            prod.SerialNumber = model.SerialNumber;
            prod.TransactionNumber = model.TransactionInfo;
            prod.DateOfPurchase = model.DateOfPurchase; 
            prod.Brand = model.Brand;
            prod.ModelBrand = model.ModelBrand;
            _productStateHolder.UpdateFrom(prod);
           
        }

#endregion

        /// <summary>
        /// Clear info from session
        /// </summary>
        public void ClearInfoFromSession()
        {
            _productStateHolder.Clear();
        }

        internal string GetModelBrand()
        {
            var prod = _productStateHolder.Load();
            return prod.ModelBrand;
        }
    }
}
