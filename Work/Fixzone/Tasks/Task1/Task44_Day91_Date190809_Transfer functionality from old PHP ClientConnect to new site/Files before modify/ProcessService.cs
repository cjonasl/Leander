using System.Collections.Generic;
using System.Linq;
using ClientConnect.Configuration;
using ClientConnect.Models.Process;
using ClientConnect.Process;
using ClientConnect.Repositories;
using ClientConnect.Validation;
using Omu.ValueInjecter;

namespace ClientConnect.Services
{
    public class ProcessService : Service
    {
        private ProcessValidation validation = new ProcessValidation();

        /// <summary>
        /// repository
        /// </summary>
        private ProcessRepository Repository { get; set; }

        /// <summary>
        /// session model
        /// </summary>
        private List<ProcessDetailsModel> CacheModel
        {
            get { return Cache.Load(new List<ProcessDetailsModel>()); }
            set
            {
                if (value != null)
                {
                    Cache.UpdateFrom(value);
                }
            }
        }
        
        private GeneralService GeneralService { get; set; }
        public ProcessService(GeneralService generalService)
        {
            GeneralService = generalService;
            Repository = (ProcessRepository) Ioc.Get<ProcessRepository>();
        }

        /// <summary>
        /// Stack of processes
        /// </summary>
        private ProcessesStack ProcessStack
        {
            get
            {
                return Session.Load(new ProcessesStack());
            }
        }

        /// <summary>
        /// Retrieves process data
        /// </summary>
        private IEnumerable<ProcessDetailsModel> ProcessList
        {
            get
            {
                if ((CacheModel == null) || (CacheModel.Count == 0))
                {
                    //CacheModel = Repository.GetProcessesFromDB(StoreId);
                    CacheModel = Repository.GetALLProcessesFromDB();
                }
                if (CacheModel.Where(x => x.Clientid == StoreId).ToList().Count() > 0)
                    return CacheModel.Where(x => x.Clientid == StoreId).ToList();
                else
                    return CacheModel.Where(x => x.Clientid == 0).ToList();
            }
        }
        
        /// <summary>
        /// Starts a new process
        /// </summary>
        /// <param name="processId">Process identifier</param>
        /// <returns>Process navigation result</returns>
        public ProcessDetailsModel StartProcess(int processId)
        {
            var navigationProcess = GetNavigationDataFromStep(FindProcessStartingStep(processId));
            if (validation.Validate(navigationProcess, Authenticated))
            {
                ProcessStack.MoveTo(navigationProcess);
                return navigationProcess;
            }
            return navigationProcess;
        }

        /// <summary>
        /// Moves to the next step of the current process
        /// </summary>
        /// <returns>Navigation result</returns>
        public ProcessDetailsModel NextStep()
        {
            var currentProcess = ProcessStack.CurrentProcess;
            if (!validation.Validate(currentProcess, Authenticated))
            {
                return currentProcess;
            }
            
            var navigationData = GetNavigationDataFromStep(FindNextStep(currentProcess.ProcessId, currentProcess.Step ?? 1));

            if (validation.Validate(navigationData, Authenticated))
            {
                ProcessStack.MoveTo(navigationData);
                return navigationData;
            }
            return currentProcess;
        }

        /// <summary>
        /// Moves to the previous step of the current process 
        /// If there's no previous, returns to the previous process
        /// </summary>
        /// <returns>Navigation result</returns>
        public ProcessDetailsModel PreviousStep()
        {
            var result = ProcessStack.CurrentProcess;
            if (!ProcessStack.IsProcessActive)
            {
                result.PageURL = "/";
                return result;
            }

            var previousStep = GetNavigationDataFromStep(FindPreviousStep(result.ProcessId, result.Step ?? 1));

            if (validation.Validate(previousStep, Authenticated))
            {
                ProcessStack.MoveTo(previousStep);
            }
            else
            {
                if (previousStep.NoProcessOrStep)
                {
                    // no previous step - return to the previous process
                    bool previousProcessExist = ProcessStack.Return();

                    if (previousProcessExist)
                    {

                        var previousProcess = GetNavigationDataFromStep(ProcessStack.CurrentProcess);
                        return previousProcess;
                    }
                    // no previous process - return to home screen
                    result = new ProcessDetailsModel { PageURL = "/" };
                    return result;
                }
            }
            return result;
        }

        /// <summary>
        /// Remove all child processes and return last main process. 
        /// For example, authetication.
        /// </summary>
        /// <returns></returns>
        public ProcessDetailsModel LastMainStep()
        {
            var result = ProcessStack.CurrentProcess;
            while (ProcessStack.IsProcessActive &&!ProcessStack.CurrentProcess.IsMainStep)
            {
                ProcessStack.Return();
            }

            if (!ProcessStack.IsProcessActive)
            {
                result.PageURL = "/";
                return result;
            }

            result = ProcessStack.CurrentProcess;
            return result;
        }

        
        /// <summary>
        /// Finds starting step information for the process
        /// </summary>
        /// <param name="processId">Process identifier</param>
        /// <returns>Process step details</returns>
        private ProcessDetailsModel FindProcessStartingStep(int processId)
        {
            return ProcessList.Where(it => it.ProcessId == processId)
                .OrderBy(it => it.Step)
                .FirstOrDefault();
        }

        /// <summary>
        /// Creates navigation data from step details
        /// Walks through child processes if necessary
        /// </summary>
        /// <param name="stepData">Step information</param>
        /// <returns>Process navigation data</returns>
        private ProcessDetailsModel GetNavigationDataFromStep(ProcessDetailsModel stepData)
        {
            if (stepData == null) return new ProcessDetailsModel();

            var destProcess = new ProcessDetailsModel();
            destProcess.InjectFrom(stepData);

            // walk through child processes until we find step which has reference to the page and not another process
            while (destProcess.ChildProcessId.HasValue)
            {
                destProcess = FindProcessStartingStep(destProcess.ChildProcessId.Value);
            }

            // return corresponding navigation data
            if (destProcess != null)
            {
                destProcess.IsMainStep = IsMainProcess(destProcess.ProcessId);
                destProcess.OriginalProcessStep = stepData.Step;
                return destProcess;
            }
            return new ProcessDetailsModel();
        }

        /// <summary>
        /// Validate main process or not
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        private bool IsMainProcess(int processId)
        {
            switch (processId)
            {
                case (int)PredefinedProcess.SignIn:
                    return false;
                case (int)PredefinedProcess.UserEmptyPassword:
                    return false;
                case (int)PredefinedProcess.UserForgottenPassword:
                    return false;
                case (int)PredefinedProcess.ExpiredPassword:
                    return false;
                default:
                    return true;
            }
        }

        /// <summary>
        /// When process go back
        /// </summary>
        public bool Backward
        {
            set
            {
                GeneralService.SetBackButtonPressedFlag(value);
            }
        }

        /// <summary>
        /// Delegate returning true if user is authenticated and false otherwise
        /// </summary>
        public bool Authenticated { 
            get { return !string.IsNullOrEmpty(UserId); }
        }
        
        /// <summary>
        /// Finds next step information for the process
        /// </summary>
        /// <param name="processId">Process identifier</param>
        /// <param name="currentStep">Current process step</param>
        /// <returns>Process step details</returns>
        private ProcessDetailsModel FindNextStep(int processId, int currentStep)
        {
            return ProcessList.Where(it => it.ProcessId == processId && it.Step > currentStep)
                .OrderBy(it => it.Step)
                .FirstOrDefault();
        }

        /// <summary>
        /// Finds previous step information for the process
        /// </summary>
        /// <param name="processId">Process identifier</param>
        /// <param name="currentStep">Current process step</param>
        /// <returns>Process step details</returns>
        private ProcessDetailsModel FindPreviousStep(int processId, int currentStep)
        {
            return ProcessList.Where(it => it.ProcessId == processId && it.Step < currentStep)
                .OrderByDescending(it => it.Step)
                .FirstOrDefault();
        }
        
        /// <summary>
        /// Stops all running processes
        /// </summary>
        public void StopAll()
        {
            ProcessStack.Clear();
        }
        public void ClearCache()
        {
            CacheModel.Clear();
        }
        /// <summary>
        /// Remove current process from stack
        /// </summary>
        public void RemoveCurrentProcess()
        {
            ProcessStack.Return();
        }
    }
}