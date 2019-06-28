using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mobile.Portal.DAL;
using Mobile.Portal.Classes;

namespace Mobile.Portal.BLL
{
    public class InspectionBLL : BaseBLL<Inspection>, Mobile.Portal.BLL.IInspectionBLL
    {
        IInspectionDataProvider _dal;
        
        public InspectionBLL()
        {
            _dal = new InspectionDataProvider();
        }

        public InspectionBLL(IInspectionDataProvider dal)
        {
            _dal = dal;
        }

        public List<Inspection> GetForCall(string fromId, string toId, string clientRef)
        {
            return _dal.GetForCall(fromId, toId, clientRef);
        }

        public Inspection GetByReference(string fromId, string toId, string clientRef, int reference)
        {
            return _dal.GetByReference(fromId, toId, clientRef, reference);
        }

        public void Update(Inspection inspection)
        {
            _dal.Update(inspection);
        }
        public List<string> GetInspectionDDContent(string table ,string filter)
        {
            List<string> result = new List<string>();
            string columnName = string.Empty;
         
            switch (table)
            {
                case "MODELSOFTWAREUPDATES":
                    columnName = "UpdateText";
                    break;
                default:
                    columnName = "";
                    break;

            }
            if (columnName != string.Empty)
                result = _dal.GetInspectionDDContent(table, columnName,filter);
            return result;
        }
    }
}
