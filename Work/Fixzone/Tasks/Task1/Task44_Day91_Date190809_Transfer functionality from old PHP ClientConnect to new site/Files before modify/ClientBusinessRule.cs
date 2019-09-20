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
                session[businessRule.Key] = businessRule.Value;
            }
        }

        public static void GetVisibility(HttpSessionState session, out bool productSearchIsVisible, out bool jobSearchIsVisible, out bool customerSearchIsVisible, out bool jobStatusesIsVisible, out string cols)
        {
            int n;

            //Set default values
            n = 0;
            productSearchIsVisible = false;
            jobSearchIsVisible = false;
            customerSearchIsVisible = false;
            jobStatusesIsVisible = false;

            if (session["ProductSearchIsVisible"] == null || session["ProductSearchIsVisible"].ToString() != "false")
            {
                n++;
                productSearchIsVisible = true;
            }

            if (session["JobSearchIsVisible"] == null || session["JobSearchIsVisible"].ToString() != "false")
            {
                n++;
                jobSearchIsVisible = true;
            }

            if (session["CustomerSearchIsVisible"] == null || session["CustomerSearchIsVisible"].ToString() != "false")
            {
                n++;
                customerSearchIsVisible = true;
            }

            if (session["JobStatusesIsVisible"] == null || session["JobStatusesIsVisible"].ToString() != "false")
            {
                jobStatusesIsVisible = true;
            }

            cols = "cols" + n.ToString();
        }
    }
}