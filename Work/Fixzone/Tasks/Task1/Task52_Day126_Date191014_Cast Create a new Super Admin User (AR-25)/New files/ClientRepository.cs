using System;
using System.Data;
using System.Linq;
using CAST.Administration;
using CAST.Infrastructure;
using CAST.Models.Administration;
using CAST.Sessions;
using Dapper;
using CAST.Client;
using CAST.Models.Client;

namespace CAST.Repositories
{
    public class ClientRepository 
    {
        /// <summary>
        /// Data context
        /// </summary>
        private DataContext _dataContext;

        public ClientRepository(DataContext data)
        {
            _dataContext = data;
        }

        public ClientTableModel GetClientsList(Client_SearchModel model, int pageNum, int pageSize)
        {
            var clientSearchResult = new ClientTableModel();
            var param = new DynamicParameters();

            int clientID, clientPriorityBooking;
            string name, location, postcode, contact;

            if (!int.TryParse(model.ClientID, out clientID))
            {
                clientID = 0;
            }

            name = string.IsNullOrEmpty(model.Name) ? "" : model.Name.Trim();
            location = string.IsNullOrEmpty(model.Location) ? "" : model.Location.Trim();
            postcode = string.IsNullOrEmpty(model.Postcode) ? "" : model.Postcode.Trim();
            contact = string.IsNullOrEmpty(model.Contact) ? "" : model.Contact.Trim();
            clientPriorityBooking = model.ClientType == "Callcenter" ? 0 : (model.ClientType == "Store" ? 1 : 2);

            if (clientID == 0 && name == "" && location == "" && postcode == "" && contact == "" && clientPriorityBooking == 2)
            {
                clientSearchResult.SearchResults = new System.Collections.Generic.List<ClientsSearchResult>();
            }
            else
            {
                param.Add("@UseAndInWhereCondition", model.SearchType == "AND" ? true : false);
                param.Add("@ClientID", clientID);
                param.Add("@Name", name);
                param.Add("@Location", location);
                param.Add("@Postcode", postcode);
                param.Add("@Contact", contact);
                param.Add("@ClientPriorityBooking", clientPriorityBooking);
                param.Add("@ReturnLines", pageSize);
                param.Add("@PageNumber", pageNum);
                param.Add("@countItems", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("@startRowNum", dbType: DbType.Int32, direction: ParameterDirection.Output);
                param.Add("@endRowNum", dbType: DbType.Int32, direction: ParameterDirection.Output);
                clientSearchResult.SearchResults = _dataContext.Connection.Query<ClientsSearchResult>("ClientsList", param, commandType: CommandType.StoredProcedure).ToList();
                clientSearchResult.ElemCount = param.Get<int>("@countItems");
                clientSearchResult.StartElem = param.Get<int>("@startRowNum");
                clientSearchResult.EndElem = param.Get<int>("@endRowNum");

                //new fields 
                clientSearchResult.PaginatorInfo.CurrentPage = pageNum;
                clientSearchResult.PaginatorInfo.ElemCount = param.Get<int>("@countItems");
                clientSearchResult.PaginatorInfo.ItemsPerPage = 5;
            }

            return clientSearchResult;
        }
    }
}