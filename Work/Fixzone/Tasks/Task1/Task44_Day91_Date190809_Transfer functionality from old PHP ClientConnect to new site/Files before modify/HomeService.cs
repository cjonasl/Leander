using ClientConnect.Configuration;
using ClientConnect.Home;
using ClientConnect.Properties;
using ClientConnect.Repositories;
using ClientConnect.ViewModels.JobStatuses;
using System;
using System.Collections.Generic;
using System.Web.SessionState;

namespace ClientConnect.Services
{
    public class HomeService : Service
    {
        private HomeRepository Repository { get; set; }


        /// <summary>
        /// Controller
        /// </summary>
        public HomeService()
        {
            Repository = (HomeRepository)Ioc.Get<HomeRepository>();
        }
        

        /// <summary>
        /// Get summary statuses of jobs
        /// </summary>
        public JobStatus_Model SummaryJobsStatuses
        {
            get
            {
                return new JobStatus_Model();
            }
        }

        /// <summary>
        /// Get link from project settings
        /// </summary>
        public string FreeSparesEnquiry
        {
            get { return Settings.Default.FreeSparesEnquiryURL; }
        }
        
        /// <summary>
        /// Aclear sessions and stop processes
        /// </summary>
        public void ClearSessions()
        {
            //HttpSessionState test = new HttpSessionState();
            //test.Clear();
            // finish all running processes
            IService[] services =
                {
                    (BookNewServiceService) Ioc.Get<BookNewServiceService>(),
                    (JobService) Ioc.Get<JobService>(),
                    (ProductService) Ioc.Get<ProductService>(),
                    (ReportsService) Ioc.Get<ReportsService>(),
                    //(AuthenticationService) Ioc.Get<AuthenticationService>(),
                    (AdministrationService) Ioc.Get<AdministrationService>(),
                    (LogsService) Ioc.Get<LogsService>(),
                    (AccountService) Ioc.Get<AccountService>(),
                    (CustomerService) Ioc.Get<CustomerService>()
                };
            foreach (var service in services)
            {
                service.ClearFromSession();
            }
        }
        public List<DateTime> GetHolidaysList(int year)
        {
            return Repository.GetHolidaysList(year);
        }

        public List<BusinessRule> GetBusinessRuleList(int ClientId)
        {
            return Repository.GetBusinessRuleList(ClientId);
        }

        public List<SpecialJob> GetSpecialJobMappingList(int ClientId)
        {
            return Repository.GetSpecialJobMappingList(ClientId);
        }
    }
}