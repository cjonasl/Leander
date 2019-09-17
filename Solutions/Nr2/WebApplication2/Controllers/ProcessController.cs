using System;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication2.Controllers
{
    public class ProcessController : Controller
    {
        public ContentResult GoBackward()
        {
            CachedResponseWrapper cachedResponseWrapperObj = CachedResponseWrapper.GetCachedResponseWrapperObj();
            return Content(cachedResponseWrapperObj.GetPreviousResponse);
        }

        public ContentResult GoForward()
        {
            CachedResponseWrapper cachedResponseWrapperObj = CachedResponseWrapper.GetCachedResponseWrapperObj();
            return Content(cachedResponseWrapperObj.GetNextResponse);
        }
    }
}