using System;
using System.Web.Mvc;
using CAST.Models.Client;
using CAST.Properties;
using CAST.Roles;
using CAST.Services;
using CAST.Process;

namespace CAST.Controllers
{
    public class ClientController : DataController
    {
        private readonly ClientService _clientService;
        
        public static string userId;

        public ClientController()
        {
            UserService userService = new UserService(Data);
            userId = userService.GetUserId();
            _clientService = new ClientService(Data);
        }

        public ActionResult ClientSearch(Client_SearchModel model, int? pageNum)
        {
            if (HttpContext.Session["IsSuperAdm"] == null)
            {
                return Redirect(Url.Process(PredefinedProcess.SignIn));
            }
            
            if (Request.HttpMethod == "GET" && !pageNum.HasValue)
            {
                return View(new Client_SearchModel());
            }
            else
            {
                if (pageNum.HasValue)
                {
                     model.SearchType = _clientService.SessionInfo.SearchType;
                     model.ClientID = _clientService.SessionInfo.ClientID;
                     model.Name = _clientService.SessionInfo.Name;
                     model.Location = _clientService.SessionInfo.Location;
                     model.Postcode = _clientService.SessionInfo.Postcode;
                     model.Contact = _clientService.SessionInfo.Contact;
                     model.ClientType = _clientService.SessionInfo.ClientType;
                }
                else
                {
                    _clientService.SessionInfo.SearchType = model.SearchType;
                    _clientService.SessionInfo.ClientID = model.ClientID;
                    _clientService.SessionInfo.Name = model.Name;
                    _clientService.SessionInfo.Location = model.Location;
                    _clientService.SessionInfo.Postcode = model.Postcode;
                    _clientService.SessionInfo.Contact = model.Contact;
                    _clientService.SessionInfo.ClientType = model.ClientType;
                }

                ClientTableModel clientTableModel = _clientService.GetClientsList(model, pageNum.HasValue ? pageNum.Value : 1, Settings.Default.UserSearchPageSize);
                model.ClientTable = clientTableModel;
                return View(model);
            }
        }
    }
}
