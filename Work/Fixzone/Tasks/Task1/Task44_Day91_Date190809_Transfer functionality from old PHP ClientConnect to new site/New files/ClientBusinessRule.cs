using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
    }
}