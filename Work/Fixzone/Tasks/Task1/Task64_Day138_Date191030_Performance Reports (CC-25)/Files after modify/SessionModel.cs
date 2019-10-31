using System;

namespace ClientConnect.Models.Account
{
    [Serializable]
    public class SessionModel
    {
        public string UserId { get; set; }
        public int ClientId { get; set; }
        public bool IsAdm { get; set; }
        public bool IsSuperAdm { get; set; }
        public bool ShowReports { get; set; }
    }
}