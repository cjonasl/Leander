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

        public ActionResult Log()
        {
            ArrayList model, signedInUsers, cachedResponses;
            CachedResponseWrapper cr;

            model = new ArrayList();

            signedInUsers = (ArrayList)HttpContext.Cache["SignedInUsers"];
            cachedResponses = (ArrayList)HttpContext.Cache["CachedResponses"];

            for (int i = 0; i < signedInUsers.Count; i++) 
            {
                cr = (CachedResponseWrapper)cachedResponses[i];
                model.Add(new string[] {(string)signedInUsers[i], cr._v.Count.ToString(), cr._currentIndex.ToString() });
            }

            return View(model);
        }

    }
}