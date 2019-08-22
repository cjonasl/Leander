using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using CAST.Models.Account;
using CAST.ViewModels.Account;

namespace CAST.Repositories
{
    public class AccountRepository : Repository
    {
        /// <summary>
        /// Retrieves user details
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>User details DTO</returns>
        public AccountDetailsModel GetAccountDetails(string userId)
        {
            return Query<AccountDetailsModel>("Retrieve_UserDetails", new { UserID = userId }, commandType: CommandType.StoredProcedure)
               .FirstOrDefault();
          
        }

      

        public List<CustomerAccount> GetAccountDetailsByEmail(string email)
        {
            return Query<CustomerAccount>("Retrieve_CustomerAccountDetailsByEmail", new { Email = email }, commandType: CommandType.StoredProcedure)
               .ToList();
            
        }

        public List<CustomerAccount> GetAccountDetailsByEmailUrn(string Email,string CustomerUrn)
        {
            return Query<CustomerAccount>("Retrieve_CustomerAccountDetailsByEmailUrn", new { Email = Email, ClientCustRef=CustomerUrn }, commandType: CommandType.StoredProcedure)
               .ToList();
            
        }

        public CustomerAccount GetCustomerAccountDetails(string Email,string Postcode, string CustomerURN )
        {
            return Query<CustomerAccount>("[Retrieve_CustomerDetailsForCustomer]", new { Email = Email, Postcode=Postcode, CustomerURN = CustomerURN }, commandType: CommandType.StoredProcedure)
               .FirstOrDefault();
            
        }
      
        /// <summary>
        /// Save new password for user
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="newPassword">New password</param>
        /// <returns>True or false (success or not)</returns>
        public bool UpdatePassword(int customerId, string newPassword)
        {
            bool isSamePassword = Query<bool>("ChangeCustomerPassword",
                                            new { CustomerID = customerId, pass = newPassword },
                                            commandType: CommandType.StoredProcedure).FirstOrDefault();
            return isSamePassword;
        }

        /// <summary>
        /// Save user datails
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <param name="question"></param>
        /// <param name="answer"></param>
        public void UpdateConfidentialInfo(string userId, string password, string question, string answer)
        {
            Execute("Update_UserConfidentialInfo",
                                        new
                                        {
                                            Password = password,
                                            Question = question,
                                            Answer = answer,
                                            UserId = userId
                                        }, commandType: CommandType.StoredProcedure);

        }

           public bool AddNewCustomerAccount(CustomerAccount model)
        {
            Execute("InsertNewCustomerAccount", new
            {
                ClientCustRef= model.ClientCustRef, 
                Email = model.Email, 
                Password = model.Password,
                ClientID = model.ClientId,
                EnroleCode = model.EnrolmentCode
              

            }, CommandType.StoredProcedure);
            return true;
        }

        public string GetPostcode(int customerId)
        {

            string query = string.Format("SELECT [postcode] FROM [dbo].[Customer] where CUSTOMERID={0} ", customerId);

            var result = Query<string>(query, CommandType.Text).FirstOrDefault();
            //  var firstOrDefault = list.FirstOrDefault(x => x.Value == title);
            return result;
        }
        /// <summary>
        /// Execute store procedure to get users list
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="pageNum">Number of showed page</param>
        /// <param name="pageSize">Count of rows in list</param>
        /// <returns>List of users</returns>
        public List<AccountDetailsModel> FindUsers(int storeId, int pageNum, int pageSize)
        {
            return Query<AccountDetailsModel>("Retrieve_Users", new
            {
                StoreId = storeId,
                ReturnLines = pageSize,
                PageNumber = pageNum
            }, CommandType.StoredProcedure).ToList();
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns></returns>
        public bool DeleteAccount(string userId)
        {
            Execute("Delete_User", new { id = userId }, commandType: CommandType.StoredProcedure);
            return true;
        }

        /// <summary>
        /// Add or update user
        /// </summary>
        /// <param name="model">Model </param>
        /// <param name="userId">user id</param>
        /// <returns></returns>
        public bool AddOrUpdateUser(AccountDetailsModel model, string newUserId)
        {
            Execute("UpdateNewUserWithEnroleID", new
            {
                UserId = model.UserId,
                Name = model.UserName,
                IsEnabled = true,
                DateOfBirth = model.DateOfBirth,
                UserLevel =1,
                Password = model.UserPassword ?? "",
                NewPassword = model.UserPassword ?? string.Empty,
                NewUserId = newUserId,
                ClientId = model.UserStoreId,
                EnroleID = model.EnroleId

            }, CommandType.StoredProcedure);
            return true;
        }

        public EntrolmentViewModel GetEnrolmentDetails(string entrolCode)
        {
            return Query<EntrolmentViewModel>("Retrieve_Enrolment", new { EnrolmentCode = entrolCode }, commandType: CommandType.StoredProcedure)
               .FirstOrDefault();
        }
        public EntrolmentViewModel GetActiveEnrolmentByCustomerId(int customerId,int linkType)
        {
            return Query<EntrolmentViewModel>("Retrieve_ActiveEnrolmentByCustomerId", new { CustomerId = customerId, LinkType=linkType }, commandType: CommandType.StoredProcedure)
               .FirstOrDefault();
        }

        public int GetEnrolmentID(int customerId)
        {
            string query = string.Format("SELECT [EnroleId] FROM [dbo].[CustomerEnrolment] where CUSTOMERID={0} ", customerId);

            var result = Query<int>(query, CommandType.Text).FirstOrDefault();
            //  var firstOrDefault = list.FirstOrDefault(x => x.Value == title);
            return result;
        }

        public bool UpdateCustomerEnrolment(int EnroleId)
        {
            Execute("UpdateCustomerEnrolment", new { EnroleId = EnroleId }, commandType: CommandType.StoredProcedure);
            return true;
        }
        public void UpdateAttempts(int enroleID)
        {
            Execute("updateAttempts", new
            {
                @EnroleId = enroleID

            }, CommandType.StoredProcedure);
        }

        public EntrolmentViewModel GetEnrolmentDetailsById(int entrolmentId)
        {
            return Query<EntrolmentViewModel>("Retrieve_EnrolmentById", new { EnrolmentId = entrolmentId }, commandType: CommandType.StoredProcedure)
                .FirstOrDefault();
        }

        public int GetCustomerId(string email)
        {
            string query = string.Format("SELECT [CustomerId] FROM [dbo].[Customer] where Email='{0}' ", email);

            var result = Query<int>(query, CommandType.Text).FirstOrDefault();
            //  var firstOrDefault = list.FirstOrDefault(x => x.Value == title);
            return result;
        }


        public bool InsertCustomerEnrolment(int customerID, int linkType)
        {
            Execute("InsertCustomerEnrolment", new { @customerId = customerID, @linkType =linkType}, commandType: CommandType.StoredProcedure);
            return true;
            
           
        }

        public CustomerAccount GetAccountDetailsByCustomerId(int customerId)
        {
            return Query<CustomerAccount>("Retrieve_CustomerAccount", new { customerId = customerId }, commandType: CommandType.StoredProcedure)
               .FirstOrDefault();
           
        }
        //public AccountDetailsModel GetCustomerAccountByCustomerId(int customerId)
        //{
        //    return Query<AccountDetailsModel>("Retrieve_UserDetailsByCustomerID", new { customerId = customerId }, commandType: CommandType.StoredProcedure)
        //       .FirstOrDefault();

        //}
        public void DisableEnrolementLink(int customerId, int enroleId)
        {
            string query = string.Format("update customerenrolment set validflag=0 where CustomerId={0} and EnroleId={1} ", customerId,enroleId);

            var result = Query<int>(query, CommandType.Text).FirstOrDefault();
            //  var firstOrDefault = list.FirstOrDefault(x => x.Value == title);
           
            
        }

        public void ClearTryCount(string UserId)
        {
            string query = string.Format("Update UserTryCount set trycount = 0 where UserId ='{0}'", UserId);
            var result = Query<int>(query, CommandType.Text).FirstOrDefault();
        }

        public void UpdateTryCount(string UserId)
        {
            string query = string.Format("Update UserTryCount set trycount = trycount+1 where UserId ='{0}'", UserId);
            var result = Query<int>(query, CommandType.Text).FirstOrDefault();
        }

        public void InsertUserTryCount(string UserId)
        {
            Execute("InsertUserTryCount", new { @UserId = UserId }, commandType: CommandType.StoredProcedure);
          
            
        }

        public int GetTryCount(string UserId)
        {
            string query = string.Format("SELECT [TryCount] FROM [dbo].[UserTryCount] where UserId='{0}' ", UserId);

            var result = Query<int>(query, CommandType.Text).FirstOrDefault();
            //  var firstOrDefault = list.FirstOrDefault(x => x.Value == title);
            return result;
           
        }

        //public CustomerAccount GetCustomerDataByEmailCustURN(string Email, string ClientCustRef)
        //{
        //    //var result = Query<CustomerAccount>(Execute("GetCustomerDataByEmailCustURN", new { @Email = Email, @ClientCustRef = ClientCustRef }, commandType: CommandType.StoredProcedure).FirstOrDefault();
        //    //return result;

        //    return Query<CustomerAccount>("GetCustomerDataByEmailCustURN", new { Email = Email, ClientCustRef = ClientCustRef }, commandType: CommandType.StoredProcedure).FirstOrDefault();
        //}

        public AccountDetailsModel GetAccountDetailsForCustomer(int customerId)
        {
            return Query<AccountDetailsModel>("Retrieve_CustomerUserDetails", new { CustomerId = customerId }, commandType: CommandType.StoredProcedure)
               .FirstOrDefault();

        }

        public void UpdateLastLogin(int customerId)
        {
            string query = string.Format("Update CustomerAccount set LastLoginDate = '{0}' where CustomerID ='{1}'", DateTime.Now.ToString("yyyy-MM-dd"), customerId);
            var result = Query<int>(query, CommandType.Text).FirstOrDefault();
        }

        public  List<CustomerAccount> GetCustomerByEmail(string Email)
        {
            return Query<CustomerAccount>("Retrieve_CustomerDetailsByEmail", new { Email = Email }, commandType: CommandType.StoredProcedure)
              .ToList();
            
           
        }
    }
}