using System.Web.Mvc;
using CAST.Process;
using CAST.Services;

namespace CAST.Controllers
{
    /// <summary>
    /// Question process
    /// </summary>
    public class QuestionController : DataController
    {
        
        /// <summary>
        /// Product data access object
        /// </summary>
        private readonly QuestionService _question;

        /// <summary>
        /// Initializes process controller
        /// </summary>
        public QuestionController()
        {
            _question = new QuestionService(Data);
        } 

        /// <summary>
        /// Show Question form
        /// </summary>
        /// <param name="qId">Id of question</param>
        /// <returns>View with question</returns>
        public ActionResult Show(int? qId)
        {
            //var model = _questionData.GetQuestion(qId, prod.SoftId);
            var model = _question.GetQuestions(qId);
            if (Request.UrlReferrer != null)
            {
                if (Request.UrlReferrer.ToString().IndexOf("BookRepair/ShowCustomerPage") > 0)
                    return Redirect(Url.ProcessPreviousStep());
            }
            else
                return RedirectToRoute("Default", new { controller = "Home", action = "Index", id = UrlParameter.Optional });
            
            if (model == null) return Redirect(Url.ProcessNextStep());
            return View(model);
        }

        /// <summary> Save answer and go to next question </summary>
        /// <param name="qText">Question text</param>
        /// <param name="qAnswer">Question answer</param>
        /// <param name="qNextId">Next question id</param>
        /// <returns>Redirect to question page or to book repair page</returns>
        public ActionResult SaveAnswer(string qText, string qAnswer, int qNextId)
        {
            _question.SaveAnswerInSession(qText, qAnswer);

            if (qNextId == 0) return Redirect(Url.ProcessNextStep());
            return RedirectToAction("Show", new { qId = qNextId });
        }
    }
}
