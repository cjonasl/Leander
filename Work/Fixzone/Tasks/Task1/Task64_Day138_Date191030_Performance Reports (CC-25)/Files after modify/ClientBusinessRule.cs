using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using ClientConnect.Services;
using ClientConnect.Configuration;
using ClientConnect.Home;

namespace ClientConnect.Infrastructure
{
    public static class ClientBusinessRule
    {
        public static void AddBusinessRulesInSession(int clientId, HttpSessionStateBase session)
        {
            HomeService homeService = (HomeService)Ioc.Get<HomeService>();

            List<BusinessRule> businessRules = homeService.GetBusinessRuleList(clientId);

            foreach(BusinessRule businessRule in businessRules)
            {
                session[businessRule.Key] = businessRule.Checked.ToString().ToLower();
            }
        }

        private static bool IsVisible(HttpSessionState session, BusinessRuleKey businessRuleKey)
        {
            if (session[businessRuleKey.ToString()] == null || session[businessRuleKey.ToString()].ToString() == "true")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void GetVisibility(HttpSessionState session, out bool productSearchIsVisible, out bool jobSearchIsVisible, out bool customerSearchIsVisible, out bool jobStatusesIsVisible, out int clientId, out bool isAdm, out bool isSuperAdm, out string cols, out bool showReports)
        {
            int n;

            //Set default values
            n = 0;
            productSearchIsVisible = false;
            jobSearchIsVisible = false;
            customerSearchIsVisible = false;
            jobStatusesIsVisible = false;

            AccountService accService = (AccountService)Ioc.Get<AccountService>();
            clientId = accService.SessionInfo.ClientId;
            isAdm = accService.SessionInfo.IsAdm;
            isSuperAdm = accService.SessionInfo.IsSuperAdm;
            showReports = accService.SessionInfo.ShowReports;

            //if (IsVisible(session, BusinessRuleKey.ProductSearchIsVisible))
            //{
            //    n++;
            //    productSearchIsVisible = true;
            //}

            if (IsVisible(session, BusinessRuleKey.JobSearchIsVisible))
            {
                n++;
                jobSearchIsVisible = true;
            }

            if (IsVisible(session, BusinessRuleKey.CustomerSearchIsVisible))
            {
                n++;
                customerSearchIsVisible = true;
            }

            if (IsVisible(session, BusinessRuleKey.JobStatusesIsVisible))
            {
                jobStatusesIsVisible = true;
            }

            cols = "cols" + n.ToString();
        }
    }
}