using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CAST.Repositories;
using CAST.Client;
using CAST.Models.Client;
using CAST.Infrastructure;

namespace CAST.Services
{
    public class ClientService : Service, IService
    {
        private readonly ClientRepository _reporsitory;

        public ClientService(DataContext data)
        {
            _reporsitory = new ClientRepository(data);
        }

        public ClientTableModel GetClientsList(Client_SearchModel model, int pageNum, int pageSize)
        {
            return _reporsitory.GetClientsList(model, pageNum, pageSize);
        }

        public Client_SessionModel SessionInfo
        {
            get { return Session.Load(new Client_SessionModel()); }
        }

        /// <summary>
        /// Clear session
        /// </summary>
        public void ClearFromSession()
        {
            Session.Clear(SessionInfo);
        }
    }
}