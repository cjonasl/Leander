using CAST.fzFMIPActivation;
using CAST.Models;
using System;
using CAST.Repositories;
using CAST.Infrastructure;
using CAST.Properties;
using CAST.Logging;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Web;
using System.Reflection;
using System.Runtime.Serialization;
using System.Collections;

namespace CAST.Services
{
    [DataContract]
    public class FmiResponse : IExtensibleDataObject
    {
        private ExtensionDataObject extensionDataObject_value;
        public ExtensionDataObject ExtensionData
        {
            get
            {
                return extensionDataObject_value;
            }
            set
            {
                extensionDataObject_value = value;
            }
        }

        [DataMember]
        public string Success;

        [DataMember]
        public string Error;
    }
    
    
    public class FMIPActivationService
    {
        private FmipServiceClient fmipActivationService;
        private readonly FMIPRepository _reporsitory;
        private readonly UserService _userService;


        private string _fzFMIPActivation_Request = string.Empty;
        private string _fzFMIPActivation_Response = string.Empty;

        private static readonly string userName = Settings.Default.fzFMIPCredUserName;
        private static readonly string passWord = Settings.Default.fzFMIPCredPassWord;

        JavaScriptSerializer jSerializer = new JavaScriptSerializer();
        ResponseInformation responseInformation = new ResponseInformation();
        public FMIPActivationService(DataContext data)
        {
            fmipActivationService = new FmipServiceClient();
            _reporsitory = new FMIPRepository(data);
            _userService = new UserService(data);
        }

        public FMIPServiceResponse FindMyIPadNow(string SessionID, string SerialNumber)
        {
            fmipServiceResponse fmipStatus = new fmipServiceResponse();
            FMIPServiceResponse response = new FMIPServiceResponse();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            AditionalData aditionalData = new AditionalData();
            PropertyInfo nameProperty, valueProperty, innerValueProperty = null;
            string status = "", message = "", name, statusValue = null;
            object val, obj;

            try
            {
                fmipStatus = fmipActivationService.getFMIPStatus(SessionID, SerialNumber);

                PropertyInfo membersProperty = fmipStatus.ExtensionData.GetType().GetProperty("Members", BindingFlags.NonPublic | BindingFlags.Instance);
                IList members = (IList)membersProperty.GetValue(fmipStatus.ExtensionData, null);

                foreach (var member in members)
                {
                    nameProperty = member.GetType().GetProperty("Name");
                    name = (string)nameProperty.GetValue(member, null);
                    valueProperty = member.GetType().GetProperty("Value");
                    val = valueProperty.GetValue(member, null);

                    if (name != "responseData")
                    {
                        innerValueProperty = val.GetType().GetProperty("Value");
                    }

                    switch (name)
                    {
                        case "Status":
                            status = (string)innerValueProperty.GetValue(val, null);
                            break;
                        case "Message":
                            message = (string)innerValueProperty.GetValue(val, null);
                            break;
                        case "responseData":
                            innerValueProperty = val.GetType().GetProperty("Items", BindingFlags.NonPublic | BindingFlags.Instance);
                            obj = innerValueProperty.GetValue(val, null);
                            valueProperty = obj.GetType().GetProperty("Item");
                            val = valueProperty.GetValue(obj, new object[] {0});
                            membersProperty = val.GetType().GetProperty("Members", BindingFlags.NonPublic | BindingFlags.Instance);
                            members = (IList)membersProperty.GetValue(val, null);

                            foreach (var m in members)
                            {
                                name = (string)nameProperty.GetValue(m, null);

                                if (name == "Value")
                                {
                                    valueProperty = m.GetType().GetProperty("Value");
                                    val = valueProperty.GetValue(m, null);
                                    innerValueProperty = val.GetType().GetProperty("Value");
                                    statusValue = (string)innerValueProperty.GetValue(val, null);
                                }
                            }

                            break;
                    }
                }

                if (status == "SUCCESS")
                {
                    response.Message = null;
                    response.Status = statusValue;
                }
                else
                {
                    response.Message = "Request failed";
                    response.Status = null;
                }
            }
            catch (Exception ex)
            {
                Log.File.ErrorFormat("Exception at FindMyIPadNow() : " + ex.Message);

                response.Message = "Request failed";
                response.Status = null;
            }
            return response;
        }

        public void LogServiceInfo(ResponseInformation Response)
        {
            _reporsitory.InsertFMIPServiceLog(Response);
        }
    }
}