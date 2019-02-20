using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CarlJonas;

namespace Sudoku
{
    public class SudokuController : Controller
    {
        public ActionResult Solve()
        {
            RequestToApplicationUtility.LogRequest(2, "Solve:&nbsp;", this.Request.Headers);
            return View();
        }

        public ActionResult Description()
        {
            RequestToApplicationUtility.LogRequest(2, "Description:&nbsp;", this.Request.Headers);
            return View();
        }

        public FileResult DownloadSourceCodeForTheAlgorithmCsharp()
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath("~/Deployment/SudokuCsharp.zip"));
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Zip, "SudokuCsharp.zip");
        }

        public FileResult DownloadSourceCodeForTheAlgorithmJavaScript()
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath("~/Deployment/SudokuJavaScript.zip"));
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Zip, "SudokuJavaScript.zip");
        }
    }
}