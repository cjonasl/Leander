using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mobile.Portal.Session;
using Mobile.Portal.BLL;
using System.Drawing;
using Mobile.Portal.Classes;
using System.IO;

namespace MobilePortal
{
    public partial class CallListPage : System.Web.UI.Page
    {
        CallsBLL callsBLL;
        
        protected void Page_Load(object sender, EventArgs e)
        {

            SiteSession session = SiteSessionFactory.LoadSession(this.Page);
            session.Errors.Clear();
            SiteSessionFactory.SaveSession(this.Page, session);
            if (!session.LoginAccepted)
            {
                Response.Redirect("~/Denied.aspx");
            }
            else
            {
                if (session.Data.ContainsKey("call"))
                { session.Data.Remove("call"); }

                if (session.Data.ContainsKey("part"))
                { session.Data.Remove("part"); }

                if (session.Data.ContainsKey("search"))
                { session.Data.Remove("search"); }

                if (session.Data.ContainsKey("basket"))
                { session.Data.Remove("basket"); }

                callsBLL = new CallsBLL();

                try
                {
                    if (!Page.IsCallback)
                    {
                        GridView1.Columns[4].Visible = session.Device == "DESKTOP";
                        GridView1.Columns[2].Visible = session.Device == "DESKTOP";

                        GridView1.Columns[4].Visible = session.Login.OSPRef == "SONY3C";
                        GridView1.Columns[5].Visible = session.Login.OSPRef == "SONY3C";

                        // Test - Peter 
                        // if (session.Params["type"] == "closed" ||
                        //     session.Params["type"] == "wip" ||
                        //     session.Params["type"] == "nodate" ||
                        //     session.Params["type"] == "new" ||
                        //     session.Params["type"] == "incomplete" ||
                        //     session.Params["type"] == "parts")
                        //     session.Params["type"] == "search") 
                        // {
                        // Peter Test ----------------------------

                        CallDataSource.EnablePaging = true;
                        CallDataSource.SelectParameters.Clear();
                        CallDataSource.StartRowIndexParameterName = "startRowIndex";
                        CallDataSource.MaximumRowsParameterName = "maxNumRows";
                        CallDataSource.SelectParameters.Add(new Parameter("saediId"));

                        if (Request.QueryString["ID"] != null)
                        {
                            if (Request.QueryString["ID"].Length > 3)
                                Response.RedirectPermanent("~/Home.aspx");

                            StatusBLL statusBLL = new StatusBLL();
                            try
                            {
                                int statusID = int.Parse(Request.QueryString["ID"].ToString());
                                string statusName = statusBLL.GetStatusName(statusID);
                                titleLabel.Text = " - " + statusName;
                            }
                            catch
                            {
                                Response.RedirectPermanent("~/Home.aspx");
                            }  

                            dateLabel.Visible = false;
                            previousButton.Visible = false;
                            nextButton.Visible = false;
                            todayButton.Visible = false;
                            calendarButton.Visible = false;

                            CallDataSource.SelectParameters.Add(new Parameter("statusId"));
                            session.Params.Remove("Chart-StatusID");

                            CallDataSource.SelectMethod = "GetCallsByStatus";
                            CallDataSource.SelectCountMethod = "GetCallsByStatusCount";
                     
                            return;
                        }

                        if (Request.QueryString["callId"] != null)
                        {
                            CallDataSource.SelectParameters.Add(new Parameter("callId"));

                            titleLabel.Text = " - Search Results";
                            // Test Peter: ---------------------------------
                            CallDataSource.SelectMethod = "GetModelsBySearch";
                            CallDataSource.SelectCountMethod = "GetModelsBySearchCount";
                            // ---------------------------------------------
                            dateLabel.Visible = false;
                            previousButton.Visible = false;
                            nextButton.Visible = false;
                            todayButton.Visible = false;
                            calendarButton.Visible = false;
                            return;
                        }
                        
                        if (session.Params["type"] == "search")
                        {
                            CallDataSource.SelectParameters.Add(new Parameter("callId"));
                            CallDataSource.SelectParameters.Add(new Parameter("postCode"));
                            CallDataSource.SelectParameters.Add(new Parameter("surname"));
                            CallDataSource.SelectParameters.Add(new Parameter("status"));
                            CallDataSource.SelectParameters.Add(new Parameter("importedTechnicianCD"));
                            CallDataSource.SelectParameters.Add(new Parameter("dateFrom"));
                            CallDataSource.SelectParameters.Add(new Parameter("dateTo"));
                          
                            CallDataSource.SelectParameters.Add(new Parameter("CaseId"));

                        }
                        if (session.Params["type"] == "Reservationsearch")
                        {
                            CallDataSource.SelectParameters.Add(new Parameter("ReservationId"));

                        }
                        // }
                        // ----------------------------------------

                        if (session.Params["type"] == "date")
                        {
                            DateTime saved = DateTime.Parse(session.Params["date"]);
                            titleLabel.Text = " - Date";
                            dateLabel.Text = saved.ToString("d");
                            dateLabel.Visible = true;
                            previousButton.Visible = true;
                            nextButton.Visible = true;
                            todayButton.Visible = true;
                            calendarButton.Visible = true;

                            CallDataSource.SelectParameters.Add(new Parameter("thedate"));
                            // Test Peter: ---------------------------------
                            CallDataSource.SelectMethod = "GetModelsByDate";
                            CallDataSource.SelectCountMethod = "GetModelsByDateCount";
                            // ---------------------------------------------
                            // callsBLL.List = callsBLL.GetModelsByDate(session.Login.SaediId, DateTime.Parse(session.Params["date"]));
                        }
                        else
                        {
                            dateLabel.Visible = false;
                            previousButton.Visible = false;
                            nextButton.Visible = false;
                            todayButton.Visible = false;
                            calendarButton.Visible = false;

                            if (session.Params["type"] == "new")
                            {
                                titleLabel.Text = " - New Calls";
                                GridView1.Columns[4].Visible = false;

                                // Test Peter: ---------------------------------
                                CallDataSource.SelectMethod = "GetModelsNewCalls";
                                CallDataSource.SelectCountMethod = "GetModelsNewCallsCount";
                                // ---------------------------------------------

                                // callsBLL.List = callsBLL.GetModelsNewCalls(session.Login.SaediId);
                            }
                            else if (session.Params["type"] == "nodate")
                            {
                                titleLabel.Text = " - No Date";

                                // Test Peter: ---------------------------------
                                CallDataSource.SelectMethod = "GetModelsNoDate";
                                CallDataSource.SelectCountMethod = "GetModelsNoDateCount";
                                // ---------------------------------------------

                                // callsBLL.List = callsBLL.GetModelsNoDate(session.Login.SaediId);
                            }
                            else if (session.Params["type"] == "incomplete")
                            {
                                titleLabel.Text = " - Visit Booked";

                                // Test Peter: ---------------------------------
                                CallDataSource.SelectMethod = "GetModelsIncomplete";
                                CallDataSource.SelectCountMethod = "GetModelsIncompleteCount";
                                // ---------------------------------------------

                                // callsBLL.List = callsBLL.GetModelsIncomplete(session.Login.SaediId);
                            }
                            else if (session.Params["type"] == "parts")
                            {
                                titleLabel.Text = " - Parts Required";

                                // Test Peter: ---------------------------------
                                CallDataSource.SelectMethod = "GetModelsRequiresOrder";
                                CallDataSource.SelectCountMethod = "GetModelsRequiresOrderCount";
                                // ---------------------------------------------

                                // callsBLL.List = callsBLL.GetModelsRequiresOrder(session.Login.SaediId);
                            }
                            else if (session.Params["type"] == "wip")
                            {
                                titleLabel.Text = " - Work in Progress";

                                // Test Peter: ---------------------------------
                                CallDataSource.SelectMethod = "GetModelsAllWIP";
                                CallDataSource.SelectCountMethod = "GetModelsAllWIPCount";
                                // ---------------------------------------------

                                // callsBLL.List = callsBLL.GetModelsAllWIP(session.Login.SaediId);
                            }
                                 else if (session.Params["type"] == "RMAcollect")
                            {
                                titleLabel.Text = " - RMA collection";

                                // Test Peter: ---------------------------------
                                CallDataSource.SelectMethod = "GetModelsRMACollection";
                                CallDataSource.SelectCountMethod = "GetModelsRMACollectionCount";
                                // ---------------------------------------------

                                // Test Peter: ---------------------------------
                                // callsBLL.List = callsBLL.GetModelsAllClosed(session.Login.SaediId);
                            }
                            else if (session.Params["type"] == "S2CList")
                            {
                                titleLabel.Text = " - Swap to credit";

                                // Test Peter: ---------------------------------
                                CallDataSource.SelectMethod = "GetModelsS2C";
                                CallDataSource.SelectCountMethod = "GetModelsS2Ccount";
                                // ---------------------------------------------

                                // Test Peter: ---------------------------------
                                // callsBLL.List = callsBLL.GetModelsAllClosed(session.Login.SaediId);
                            }
                            else if (session.Params["type"] == "closed")
                            {
                                titleLabel.Text = " - Closed";

                                // Test Peter: ---------------------------------
                                CallDataSource.SelectMethod = "GetModelsAllClosed";
                                CallDataSource.SelectCountMethod = "GetModelsAllClosedCount";
                                // ---------------------------------------------

                                // Test Peter: ---------------------------------
                                // callsBLL.List = callsBLL.GetModelsAllClosed(session.Login.SaediId);
                            }
                            else if (session.Params["type"] == "search" && session.Params["status"] == "1")
                            {
                                titleLabel.Text = " - Search for New";

                                // Test Peter: ---------------------------------
                                CallDataSource.SelectMethod = "GetModelsBySearch";
                                CallDataSource.SelectCountMethod = "GetModelsBySearchCount";
                                // ---------------------------------------------

                                // callsBLL.List = callsBLL.GetModelsBySearch(session.Login.SaediId, session.Params["callId"], session.Params["postCode"], session.Params["surname"], session.Params["status"]);
                            }
                            else  if( session.Params["type"] == "Reservationsearch")
                        {
                          // CallDataSource.SelectParameters.Add(new Parameter("ReservationId"));
                            CallDataSource.SelectMethod = "GetModelsByReservationsearch";
                            CallDataSource.SelectCountMethod = "GetModelsByReservationsearchCount";
                        }
                            else if (session.Params["type"] == "search")
                            {
                                titleLabel.Text = " - Search Results";

                                // Test Peter: ---------------------------------
                                CallDataSource.SelectMethod = "GetModelsBySearch";
                                CallDataSource.SelectCountMethod = "GetModelsBySearchCount";
                                // ---------------------------------------------

                                // callsBLL.List = callsBLL.GetModelsBySearch(session.Login.SaediId, session.Params["callId"], session.Params["postCode"], session.Params["surname"], session.Params["status"]);
                            }
                            else
                            {
                                titleLabel.Text = "Service Calls";

                                // Test Peter: ---------------------------------
                                CallDataSource.SelectMethod = "GetModelsAllWIP";
                                CallDataSource.SelectCountMethod = "GetModelsAllWIPCount";
                                // ---------------------------------------------

                                // callsBLL.List = callsBLL.GetModelsAllWIP(session.Login.SaediId);
                            }
                        }
                    }
                }
                catch (Exception ex)
                { ex.ToString(); }
            }
        }

        // Peter Test ------------------------
        protected void CallDataSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            SiteSession session = SiteSessionFactory.LoadSession(this.Page);

            e.InputParameters["saediId"] = session.Login.SaediId;

            // ------------------
            // SEARCH FROM NOTES:
            // ------------------
            if (Request.QueryString["callId"] != null)
            {
                try
                {
                    e.InputParameters["callId"] = Request.QueryString["callId"].ToString();
                    e.InputParameters["postCode"] = string.Empty;
                    e.InputParameters["surname"] = string.Empty;
                    e.InputParameters["status"] = "0";
                    e.InputParameters["importedTechnicianCD"] = string.Empty;
                    e.InputParameters["dateFrom"] = DateTime.Now.AddYears(-5).ToString("dd/MM/yyyy");
                    e.InputParameters["dateTo"] = DateTime.Now.AddYears(100).ToString("dd/MM/yyyy"); 
               		e.InputParameters["CaseId"] = session.Params["CaseId"];
                }
                catch { }

                return;
            }

            if (Request.QueryString["ID"] != null)
            {
                try
                {
                    int statusID = int.Parse(Request.QueryString["ID"].ToString());
                    e.InputParameters["statusId"] = statusID;
                }
                catch
                {
                    Response.RedirectPermanent("~/Home.aspx");
                }
                               
                return;
            }

            if (session.Params["type"] == "search")
            {
                e.InputParameters["callId"] = session.Params["callId"];
                e.InputParameters["postCode"] = session.Params["postCode"];
                e.InputParameters["surname"] = session.Params["surname"];
                e.InputParameters["status"] = session.Params["status"];
                try
                {
                    e.InputParameters["importedTechnicianCD"] = session.Params["importedTechnicianCD"];
                }
                catch { }
                e.InputParameters["dateFrom"] = session.Params["dateFrom"];
                e.InputParameters["dateTo"] = session.Params["dateTo"];
           
                e.InputParameters["CaseId"] = session.Params["CaseId"];
                return;
            }
            if (session.Params["type"] == "Reservationsearch")
            {
                e.InputParameters["ReservationId"] = session.Params["ReservationId"];
               
                return;
            }

            if (session.Params["type"] == "date")
            {
                e.InputParameters["thedate"] = DateTime.Parse(session.Params["date"]);
            }
        }
        // ----------------------------------

        protected void CallDataSource_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
           
        }

        protected void rejectImageButton_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Page")
            {
                switch (e.CommandArgument.ToString())
                {
                    case ("Last"): GridView1.PageIndex = 1000;
                        break;
                    case ("First"): GridView1.PageIndex = 1;
                        break;
                    case ("Next"): GridView1.PageIndex = GridView1.PageIndex + 1;
                        break;
                    case ("Prev"): GridView1.PageIndex = GridView1.PageIndex - 1;
                        break;
                    default:
                        int pageNo;
                        if (int.TryParse(e.CommandArgument.ToString(), out pageNo))
                            GridView1.PageIndex = pageNo;
                        break;
                }

            }
            else
            {
                int id = int.Parse(e.CommandArgument.ToString());
                Call rowCall = callsBLL.GetById(id);
                SiteSession session = SiteSessionFactory.LoadSession(this.Page);

                if (rowCall.Instruction != 1 && !rowCall.IsSony)
                {
                    InspectionBLL inspectionsBLL = new InspectionBLL();
                    List<Inspection> inspections = inspectionsBLL.GetForCall(rowCall.SaediToId, rowCall.SaediFromId, rowCall.ClientRef);
                    rowCall.hasInspections = inspections.Count != 0;
                }

                if (session.Data.ContainsKey("call"))
                { session.Data["call"] = rowCall; }
                else
                { session.Data.Add("call", rowCall); }


                SiteSessionFactory.SaveSession(this.Page, session);
                if (rowCall.Instruction == 1)
                {
                    Response.Redirect("~/Accept.aspx");
                }
                else
                {
                    /////////////////////////// CHECK STOCK PARTS IN SAEDICALLS ////////////////////////////
                    // This should be added on add stock parts or Allocate part. 
                    // Sometimes the proper part wasn't added (web service return different part!!??)
                    // We check the part here again:
                    ///////////////////////////////////////////////////////////////////////////////////////
                    //OSPRefBLL ospBLL = new OSPRefBLL();
                    //List<OSPRefs> ospRefsList = ospBLL.GetOSPRefByCallID(rowCall.Id.ToString());
                    //OSPRefs ospRefs = new OSPRefs();
                    //if (ospRefsList.Count > 0)
                    //    ospRefs = ospRefsList[0];
                    //else
                    //    ospRefs.StockAddSearch = "False";

                    //if (ospRefs.StockAddSearch == "True") // If it is SONY
                    //{
                    //    PartsBLL partsBLL = new PartsBLL();
                    //    PartsBLL stockPartsBLL = new PartsBLL();
                    //    PartsBLL allocatedPartsBLL = new PartsBLL();
                    //    PartsBLL partsAll = new PartsBLL();
                    //    PartsBLL partsOrdered = new PartsBLL();
                    //    PartsBLL SAEDIParts = new PartsBLL();

                    //    SAEDIParts.List = SAEDIParts.GetSAEDIPartsByCall(rowCall.SaediFromId, rowCall.ClientRef).ToList();
                    //    stockPartsBLL.List = partsBLL.GetPartsByClientIdForSONY(rowCall.Id.ToString(), rowCall.SaediFromId, rowCall.ClientRef).ToList();
                        
                    //    try
                    //    { partsAll.List = partsBLL.GetPartsByCallId(rowCall.Id); }
                    //    catch { }
                    //    foreach (CallPart line in partsAll.List.ToList())
                    //    {
                    //        CallPart callPartAllocated = rowCall.UsedParts.Items.Find(p => p.Code == line.Code);
                    //        if (callPartAllocated != null && line.StatusID.ToUpper().Trim() != "V" && line.IsStock == false)
                    //        {
                    //            allocatedPartsBLL.List.Add(line);
                    //        }
                    //    }

                    //    try
                    //    {
                    //        // ---------------------------
                    //        // 1.) SYNCHRONISE SAEDICalls
                    //        // ---------------------------
                    //        rowCall.UsedParts.Items.Clear();
                    //        rowCall.UsedParts.Items.AddRange(stockPartsBLL.List.ToList());
                    //        rowCall.UsedParts.Items.AddRange(allocatedPartsBLL.List.ToList());
                    //        callsBLL.UpdateCall(rowCall);
                    //        session.Data["call"] = rowCall;
                    //        SiteSessionFactory.SaveSession(this.Page, session);

                    //        // --------------------------
                    //        // 2.) SYNCHRONISE SAEDIParts
                    //        // --------------------------
                    //        foreach (CallPart part in partsAll.List.ToList())
                    //        {
                    //            CallPart saediPart = SAEDIParts.List.ToList().Find(p => p.PartReference == part.PartReference && p.StatusID == part.StatusID);
                    //            bool IsSAEDIAllocated = false;
                    //            if (saediPart != null)
                    //                IsSAEDIAllocated = saediPart.IsAllocated;

                    //            if (IsSAEDIAllocated == false)
                    //            {
                    //                CallPart allocatedPart = allocatedPartsBLL.List.ToList().Find(p => p.PartReference == part.PartReference && p.StatusID == part.StatusID);
                    //                if (allocatedPart != null)
                    //                    part.IsAllocated = true;

                    //                part.SAEDIFromID = rowCall.SaediFromId;
                    //                part.SAEDICallRef = rowCall.ClientRef;

                    //                SAEDIParts.DeleteSAEDIPart(part);
                    //                SAEDIParts.InsertSAEDIPart(part);
                    //            }
                    //        }

                    //        foreach (CallPart part in SAEDIParts.List.ToList())
                    //        {
                    //            CallPart partExist = partsAll.List.ToList().Find(p => p.PartReference == part.PartReference && p.StatusID == part.StatusID);
                    //            if (partExist == null)
                    //                SAEDIParts.DeleteSAEDIPart(part);
                    //        }
                    //    }
                    //    catch { }
                    //}
                    //////////////////////////
                                                                                                                                                                                                                         
                    Response.Redirect("~/Appointment.aspx");
                }
            }
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DateTime? dt = ((Call)e.Row.DataItem).ChaseDate;
                Boolean bl = dt.HasValue;
                if (bl)
                {
                    if (dt.Value.Date <= DateTime.Today.Date)
                    {
                        e.Row.Cells[4].ForeColor = Color.Red;
                    }
                    else if (dt.Value.Date == DateTime.Today.AddDays(1).Date)
                    {
                        e.Row.Cells[4].ForeColor = Color.Orange;
                    }
                }

                if (((Call)e.Row.DataItem).IsStatusWIP == true)
                {
                    int iNoOfDay = ((Call)e.Row.DataItem).NoOfDays;

                    if (iNoOfDay >= 30)
                    {
                        e.Row.Cells[2].ForeColor = Color.Red;
                    }
                }
            }
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected string ShowVisitType(object SaediSourceID, object code)
        {
            VisitTypeBLL visitTypeBLL = new VisitTypeBLL();
            List<VisitType> visitList = visitTypeBLL.GetVisitTypeBySAEDISourceID(SaediSourceID.ToString(), code.ToString());

            try
            {
                VisitType visit = visitList[0];
                // if (visit.Description != string.Empty)
                    return visit.Description;
                //else
                //    return call.VisitReasonText;
            }
            catch
            {
                return string.Empty; // call.VisitReasonText;
            }
        }

        protected void previousButton_Click(object sender, ImageClickEventArgs e)
        {
            SiteSession session = SiteSessionFactory.LoadSession(this.Page);
            DateTime saved = DateTime.Parse(session.Params["date"]);
            session.Params.Clear();
            session.Params.Add("type", "date");
            session.Params.Add("date", saved.AddDays(-1).ToString());
            SiteSessionFactory.SaveSession(this.Page, session);
            Response.Redirect("~/CallList.aspx");
        }

        protected void calendarButton_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/CallCalendar.aspx");
        }

        protected void nextButton_Click(object sender, ImageClickEventArgs e)
        {
            SiteSession session = SiteSessionFactory.LoadSession(this.Page);
            DateTime saved = DateTime.Parse(session.Params["date"]);
            session.Params.Clear();
            session.Params.Add("type", "date");
            session.Params.Add("date", saved.AddDays(+1).ToString());
            SiteSessionFactory.SaveSession(this.Page, session);
            Response.Redirect("~/CallList.aspx");
        }

        protected void todayButton_Click(object sender, ImageClickEventArgs e)
        {
            SiteSession session = SiteSessionFactory.LoadSession(this.Page);
            session.Params.Clear();
            session.Params.Add("type", "date");
            session.Params.Add("date", DateTime.Today.ToString());
            SiteSessionFactory.SaveSession(this.Page, session);
            Response.Redirect("~/CallList.aspx");
        }

    

    }
}
