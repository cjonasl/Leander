using System;
namespace Mobile.Portal.DAL
{
    public interface IInspectionDataProvider
    {
        Mobile.Portal.Classes.Inspection GetByReference(string fromId, string toId, string clientRef, int reference);
        System.Collections.Generic.List<Mobile.Portal.Classes.Inspection> GetForCall(string fromId, string toId, string clientRef);
        void Update(Mobile.Portal.Classes.Inspection inspection);
        bool Update(int uid, int status);

        System.Collections.Generic.List<string> GetInspectionDDContent(string table, string columnName,string filter);
    }
}
