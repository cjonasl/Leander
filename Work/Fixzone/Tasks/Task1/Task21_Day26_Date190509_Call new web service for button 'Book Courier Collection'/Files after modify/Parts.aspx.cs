using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mobile.Portal.Session;
using Mobile.Portal.Classes;
using Mobile.Portal.BLL;
using System.IO;
using Mobile.Portal.Utilities;
using System.Xml;

// FOR TEMP FUNCTIONS: ---------
using System.Configuration;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;
// ----------------------------

// TEST:
using DATALib;

namespace MobilePortal
{
    public partial class PartsPage : System.Web.UI.Page
    {
        //bool UseOldPartSearch = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseOldPartSearch"]);
        PartsBLL actionPartsBLL = new PartsBLL();
        PartsBLL orderPartsBLL = new PartsBLL();
        PartsBLL cancelPartsBLL = new PartsBLL();
        PartsBLL usedPartsBLL = new PartsBLL();
        PartsBLL usedAllocatedPartsBLL = new PartsBLL();
        //  public static string SaediFromId { get; set; }
        public string ClientRef { get; set; }
        public string SAEDIFromId { get; set; }
        public bool IsSwap2Credit { get; set; }
        public Call call { get; set; }
        public bool AepJob { get; set; }
        public bool PDAUSer { get; set; }
        int maxParts = 10;
        public bool HasInspections = false;
        public bool SonyAcknowledgement;
        public bool IsSonyStatusInCompleted = false; // work in progress
        public bool SonyStatusComplete = false;// find completed status for sony jobs.
        public PartsPage()
        {
            //SiteSession session = SiteSessionFactory.LoadSession(this.Page);
            // call = new Call();
            //call = (Call)session.Data["call"];
            //if (!session.LoginAccepted)
            //{
            //    Response.Redirect("~/Denied.aspx");
            //}
            //else
            //{
            //    SAEDIFromId = call.SaediFromId;
            //    ClientRef = call.ClientRef;
            //}
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            SiteSession session = SiteSessionFactory.LoadSession(this.Page);
            Call call = new Call();
            call = (Call)session.Data["call"];
            if (PDAUSer)
            {
                FindAllButtons(this.Page);
            }
            else
            {
                WebEdit.Visible = false;

                // stopping part ordering afetr completing the job

                List<int> sonyCompletedStatuses;
                if (call.VisitReasonCode == "097" || call.VisitReasonCode == "098" || call.VisitReasonCode == "099")
                    sonyCompletedStatuses = new List<int>() { 40, 43, 44, 45, 50, 51, 54, 55, 56, 58, 59, 60, };
                else
                    sonyCompletedStatuses = new List<int>() { 22, 36, 40, 43, 50, 54, 58, 62 };
                // disabling all the button s in parts page except 'Cretae Rma ' button after completing the job.
                // condition applies  only to completed / closed sony jobs
                if (call.IsSony && (!call.IsStatusWIP || sonyCompletedStatuses.Contains(call.StatusId)))
                    FindAllButtons(this.Page, false);

            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            SiteSession session = SiteSessionFactory.LoadSession(this.Page);
            if (!session.LoginAccepted)
            {
                Response.Redirect("~/Denied.aspx");
            }
            else
            {
                Session.Remove("UseAdvancedPartsearchCall");
                // write to databse
                call = new Call();
                call = (Call)session.Data["call"];
                SAEDIFromId = call.SaediFromId;
                ClientRef = call.ClientRef;
                PDAUSer = call.callReadOnly;
                btnDeletePartFromSony.Visible = ((ProgramVersion.IsPilotVersion() || ProgramVersion.IsTesting || ProgramVersion.IsDevelopment) && call.IsSony);
                //  courierBookingButton.Visible = call.IsSony;// (ProgramVersion.IsTesting || ProgramVersion.IsDevelopment || ProgramVersion.IsPilotVersion()) && //
                partReturnButton.Visible = call.IsSony;
                IsSwap2Credit = Swap2CreditApproved(SAEDIFromId, ClientRef);
                // ==================================================
                // REFRESH CALL: 
                CallsBLL refreshCallBLL = new CallsBLL();
                Call nCall = refreshCallBLL.GetById(call.Id);
                session.Data["call"] = nCall;
                SiteSessionFactory.SaveSession(this.Page, session);

                IsSonyStatusInCompleted = nCall.IsStatusWIP;
                if (nCall.IsSony)
                    SonyStatusComplete = IsStatusInCompleted(nCall.StatusId, nCall);
                //finding whether the call is eligible to use new part search
                OSPRefBLL ospBLL = new OSPRefBLL();
                List<OSPRefs> ospRefsList = ospBLL.GetOSPRefByCallID(call.Id.ToString());

                bool usenewCallPartserach = false;
                if (ospRefsList.Count > 0)
                    usenewCallPartserach = ospRefsList[0].UseNewPartSearch; // client level setting for the job

                bool UseAdvancedPartsearchCall = usenewCallPartserach && Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseNewPartSearch"]);
                Session["UseAdvancedPartsearchCall"] = false;// UseAdvancedPartsearchCall;
                call = nCall;
                // ==================================================

                // Max Parts
                try
                {
                    maxParts = int.Parse(ConfigUtils.GetAppSetting("maxParts", 10));
                }
                catch
                {
                    maxParts = 10;
                }

                CallsBLL callsBLL = new CallsBLL();
                readOnlyLabel.Visible = call.callReadOnly;
                WebEdit.Visible = call.callReadOnly;

                HasInspections = call.hasInspections;
                callLabel.Text = call.CallReference.Replace("<br/>", " - ");

                //if (Page.IsPostBack == false)
                //{

                session.Data.Remove("part");
                divRMAOutput.Visible = false;
                SiteSessionFactory.SaveSession(this.Page, session);

                //===============================================
                // SYNCHRONIZE SAEDIPARTS WITH PARTS WEB SERVICE:
                //===============================================
                if (call.IsSony)
                {
                    PartsBLL partsAll = new PartsBLL();
                    PartsBLL SAEDIParts = new PartsBLL();
                    CourierBooking partCourierdetails = new CourierBooking();
                    // 1.) GET ALL SAEDIPARTS FOR CALL:
                    try
                    {
                        string SaediID;
                        if (call.SaediFromId != "SONY3C")
                            SaediID = call.SaediFromId;
                        else
                            SaediID = call.SaediToId;
                        SAEDIParts.List = SAEDIParts.GetSAEDIPartsByCall(SaediID, call.ClientRef).ToList();



                        if (call.VisitReasonCode == "097" || call.VisitReasonCode == "098" || call.VisitReasonCode == "099")
                        {
                            AepJob = true;

                            //// checking the job status change is submitted to sony
                            //UpdateServiceEventStatusBLL ServiceEventStatusBLL =new UpdateServiceEventStatusBLL ();
                            //SonyAcknowledgement = ServiceEventStatusBLL.GetServiceEventStatusRecord(call.SaediFromId, call.SaediToId, call.ClientRef).Count > 0;
                        }
                        if (call.VisitReasonCode == "000" || call.VisitReasonCode == "097" || call.VisitReasonCode == "099")
                        {
                            UpdateServiceEventStatusBLL ServiceEventStatusBLL = new UpdateServiceEventStatusBLL();
                            SonyAcknowledgement = ServiceEventStatusBLL.GetServiceEventStatusRecordForConfirmed(call.SaediFromId, call.SaediToId, call.ClientRef).Count > 0;
                        }

                    }
                    catch (Exception er)
                    {
                        ErrorHandler.ShowError(this.Page, "GET SAEDI PARTS", er.Message);
                    }

                    // 2.) GET ALL STOCK / ALLOCATED PARTS FOR CALL FROM WEB SERVICE:
                    try
                    {
                        partsAll.List = partsAll.GetPartsByCallId(call.Id);
                    }
                    catch (Exception er)
                    {
                        ErrorHandler.ShowError(this.Page, "GET ALLOCATED PARTS", er.Message);
                    }

                    // 3.) ADD/UPDATE SAEDIPARTS:
                    string duplicatedPartReference = string.Empty;
                    bool IsDuplicated = false;
                    foreach (CallPart part in partsAll.List.ToList())
                    {
                        part.SAEDIFromID = call.SaediFromId;
                        part.SAEDICallRef = call.ClientRef;
                        // CourierBooking Courierresult = partCourierdetail.FetchCourierBookingDetails(call.SaediFromId, call.ClientRef, part.OrderReference, part.PartReference);

                        // TEST:
                        // MSSQLDATA.WriteToTEST_TABLE("fromID:"+part.SAEDIFromID, "ClientRef:"+part.SAEDICallRef, "Code:"+part.Code, 0, DateTime.Now);

                        if (duplicatedPartReference != part.PartReference.ToString())
                        {
                            duplicatedPartReference = part.PartReference.ToString();
                            IsDuplicated = false;
                        }
                        else
                        {
                            IsDuplicated = true;
                        }

                        CallPart saediPart = SAEDIParts.List.Find(p => p.PartReference == part.PartReference); //  && p.StatusID == part.StatusID
                        if (saediPart == null)
                        {
                            // - ADD NEW PART TO SAEDIPARTS:
                            if (IsDuplicated == false)
                                SAEDIParts.InsertSAEDIPart(part);
                        }
                        else
                        {
                            // - UPDATE EXISTING PART IN SAEDIPARTS:
                            // SAEDIParts.DeleteSAEDIPart(part);
                            part.IsAllocated = saediPart.IsAllocated;
                            part.IsPartConsumptionDone = saediPart.IsPartConsumptionDone;
                            part.IsPrimary = saediPart.IsPrimary;
                            SAEDIParts.UpdateSAEDIPart(part);
                        }
                    }

                    // 4.) DELETE SAEDIPARTS WHICH NOT EXISTS IN WEB SERVICE:
                    foreach (CallPart part in SAEDIParts.List.ToList())
                    {
                        CallPart webServicePart = partsAll.List.Find(p => p.PartReference == part.PartReference);
                        if (webServicePart == null)
                            SAEDIParts.DeleteSAEDIPart(part);
                    }
                }

                // }
                ShowParts(call);
            }
        }
        protected bool IsStatusInCompleted(int selectedValue, Call call)
        {
            List<int> sonyCompletedStatuses;
            if (call.VisitReasonCode == "097" || call.VisitReasonCode == "098" || call.VisitReasonCode == "099")
                sonyCompletedStatuses = new List<int>() { 40, 43, 44, 45, 50, 51, 54, 55, 56, 58, 59, 60, };
            else
                sonyCompletedStatuses = new List<int>() { 22, 36, 40, 43, 50, 54, 58, 62 };

            if (sonyCompletedStatuses.Contains(selectedValue))
                return true;
            else
                return false;
        }
        protected void GenerateSwap()
        {
            Swap2CreditBLL _bll = new Swap2CreditBLL();

            SiteSession session = SiteSessionFactory.LoadSession(this.Page);
            Call call = (Call)session.Data["call"];
            var SPresult = _bll.GetSwap2CreditResults(call.SaediFromId, call.ClientRef);
            var result = GenerateSWAP(SPresult[0].referenceId);
        }
        private SwapforCredit GenerateSWAP(string GPSWAPRMAToolid)
        {
            SiteSession session = SiteSessionFactory.LoadSession(this.Page);
            Call call = (Call)session.Data["call"];
            SwapforCredit swapforCredit = new SwapforCredit();
            Swap2CreditBLL s2cBLL = new Swap2CreditBLL();
            ClientBLL clientBLL = new ClientBLL();
            Client client = clientBLL.GetBySaediId(session.Login.SaediId, programVersion: "2");

            //      // -----------------------------
            //    // CREATE S2C AND SHOW RESPONSE:
            //    // -----------------------------
            List<string> userPassword =
                Mobile.Portal.Helpers.SonyServicesHelper.GetSONYWebServiceUserPassword(client.Currency);

            string claimType = "SERVICEPRODUCTRETURN";
            swapforCredit.referenceId = GPSWAPRMAToolid;
            try
            {
                s2cBLL.List = s2cBLL.Swap2CreditRequest(saediFromId: call.SaediFromId,
                                clientRef: call.ClientRef,
                                userID: userPassword[0],      // "GBEPACIF",                        
                                password: userPassword[1],    // "HUL3L49", 
                                aepBookInRef: string.Empty,
                                externalMatId: string.Empty, // record.INPUTascMaterialId
                                claimType: claimType,
                                clientReference: call.ImportedTechnicianCD,
                                faultCode: call.Iris.SymptomCode,
                                gpToolRmaId: GPSWAPRMAToolid,
                                repairNumber: call.ClientRef,
                                modelID: call.Appliance.Model.Code,
                                remark: string.Empty,
                                returnQuantity: 1,
                                serialNumber: call.Appliance.SerialNumber);

                swapforCredit = s2cBLL.List[0];
            }
            catch (Exception er)
            {
                swapforCredit = new SwapforCredit();
                swapforCredit.success = false;
                swapforCredit.errorMessage = er.Message;
                swapforCredit.ClientRef = call.ClientRef;
                swapforCredit.referenceId = GPSWAPRMAToolid;
            }

            //    // CUSTOMIZE RESPONSE MESSAGES:


            s2cBLL.CreateS2C(swapforCredit);

            session.Data.Remove("S2C");
            session.Data.Add("S2C", swapforCredit);
            SiteSessionFactory.SaveSession(this.Page, session);
            if (swapforCredit.success)
            {
                string queryString = string.Format("Collectionjob.aspx?SAEDIID={0}&ClientRef={1}&RMAREF={2}&SWAP2Credit={3}", call.SaediFromId, call.ClientRef, swapforCredit.rmaId, true);


                Iframe.Attributes.Add("src", queryString);

                ModalPopupExtender1.Show();

            }
            else
            {
                SetResponseMessage(swapforCredit);
                btnref.Visible = true;
                btnref.Enabled = true;
            }

            return swapforCredit;
        }

        private void SetResponseMessage(SwapforCredit swapforCredit)
        {
            PartsList.Visible = false;
            divSWAP2CREDITOutput.Visible = true;
            LblTextSwap2Credit.Text = swapforCredit.referenceId;
            txtLblSwap2CreditClientref.Text = swapforCredit.ClientRef;
            //  txtSwap2CreditDocumenturl.Text=swapforCredit.rmaDocumentUrl;
            //  txtSwap2CreditRMARef.Text= swapforCredit.rmaId;
            txtSwap2Creditresult.Text = swapforCredit.success ? "Cliam raised" : swapforCredit.errorMessage + ":" + swapforCredit.validationErrorList;
        }

        private void ShowParts(Call call)
        {
            SiteSession session = SiteSessionFactory.LoadSession(this.Page);

            List<CallPart> parts = new List<CallPart>();
            PartsBLL usedSONYPartsBLL = new PartsBLL();

            // =====================================
            // VAN (STOCK) PARTS AND ALLOCATED PARTS
            // =====================================
            btnNoPartsUsed.Visible = false;
            btnTechnicalBulletin.Visible = false;

            if (call.IsSony)                       // If it is SONY
            {
                editAllocatedPartLinkButton.Visible = false;
                try
                {
                    usedSONYPartsBLL.List = usedSONYPartsBLL.GetSAEDIPartsByCall(call.SaediFromId, call.ClientRef);
                    foreach (CallPart p in usedSONYPartsBLL.List.ToList())
                    {
                        p.IsSony = true;
                        //if (p.Code == "000000010")
                        if ((p.IsAllocated == true) || (p.StatusID == "V")) // Van or Allocated parts for SONY 
                            usedPartsBLL.List.Add(p);
                        else
                            parts.Add(p);
                    }
                }
                catch (Exception er)
                {
                    ErrorHandler.ShowError(this.Page, " GET SAEDI PARTS FOR CALL ", er.Message);
                }

                if (usedPartsBLL.List.Count == 0)
                {
                    btnNoPartsUsed.Visible = !(call.StatusId == 3 || call.StatusId == 71);//&& SonyAcknowledgement); 
                    btnTechnicalBulletin.Visible = !(call.StatusId == 3 || call.StatusId == 71);//&& SonyAcknowledgement);

                    //todo:
                    //if (call.VisitReasonCode == "097" || call.VisitReasonCode == "000")
                    //{
                    //    btnNoPartsUsed.Visible =  SonyAcknowledgement ; // job status can differ from sony  as we keep all status change locally
                    //    btnTechnicalBulletin.Visible = SonyAcknowledgement;

                    //}
                }
                else
                {
                    btnNoPartsUsed.Visible = false;
                }

                // If "No Parts Used" was added then you can't add or allocate parts:
                foreach (CallPart p in usedPartsBLL.List.ToList())
                {
                    if (p.Code == "000000010")
                    {
                        btnNoPartsUsed.Visible = false;
                        btnTechnicalBulletin.Visible = false;
                        usePartLinkButton.Enabled = false;
                        addAllocatedPartLinkButton.Enabled = false;
                    }
                    //else if(p.Code.StartsWith("FX"))// "99"
                    //{
                    //    usePartLinkButton.Enabled=false;
                    //}
                }
            }
            else
            {
                // partReturnButtonTop.Visible = false;
                usedPartsBLL.List = call.UsedParts.Items;
            }


            // ==============
            // ORDERED PARTS
            // ==============    
            PartsBLL partsBLL = new PartsBLL();
            //try
            //{
            if (partsBLL.UsesPartOrdering(call.Id) == true)
            {
                actionPartsDiv.Visible = false;
                cancelledPartsDiv.Visible = false;

                if (call.IsSony)               // If it is SONY
                {
                    // parts = usedSONYPartsBLL.List.ToList();
                }
                else
                {
                    parts = partsBLL.GetPartsByCallId(call.Id);
                }

                //try
                //{
                foreach (CallPart line in parts)
                {
                    if (line.Status == "On Hold")
                    {
                        actionPartsDiv.Visible = true;
                        actionPartsBLL.List.Add(line);
                    }
                    else if (line.Status == "7")
                    {
                        actionPartsDiv.Visible = true;
                        line.Status = "On Hold";
                        actionPartsBLL.List.Add(line);
                    }
                    else if (line.Status == "Cancelled")
                    {
                        cancelledPartsDiv.Visible = true;
                        cancelPartsBLL.List.Add(line);
                    }
                    else
                    {
                        if (line.StatusID.ToUpper().Trim() != "V") // Add only records with statusid different then "V"   
                        {
                            if (call.IsSony)   // If SONY
                            {
                                // CallPart callPart = call.UsedParts.Items.Find(p => p.Code == line.Code);
                                if (line.IsAllocated == false && line.IsStock == false)
                                {
                                    orderPartsBLL.List.Add(line);
                                }
                            }
                            else
                                orderPartsBLL.List.Add(line);
                        }
                    }

                    if (line.TransactionCode != "")
                    {
                        CallPart usedPart = usedPartsBLL.List.Find(delegate(CallPart cp) { return cp.Code == line.Code && cp.TransactionCode == line.TransactionCode; });
                        // CallPart usedPart = usedPartsBLL.List.Find(delegate(CallPart cp) { return cp.Code == line.Code && cp.PartReference == line.PartReference; });

                        if (usedPart != null)
                        {
                            usedPart.Status = line.Status;
                        }
                    }
                }
                //}
                //catch (Exception er)
                //{
                //    ErrorHandler.ShowError(this.Page, "LOOP ORDERED PARTS", er.Message);
                //}

                foreach (CallPart item in orderPartsBLL.List)
                {
                    if (item.DispatchDate == DateTime.Parse("01/01/0001"))
                    { item.DispatchDate = null; }
                    if (item.OrderDate == DateTime.Parse("01/01/0001"))
                    { item.OrderDate = null; }
                    if (item.TransactionCode == null || item.TransactionCode == "")
                    {
                        item.TransactionCode = "Call Spares Dept.";
                    }
                }
            }
            // }
            //catch (Exception er)
            //{
            //    ErrorHandler.ShowError(this.Page, "ORDERING PARTS", er.Message);
            //}

            try
            {
                ClientBLL clientBLL = new ClientBLL();

                // Client client = clientBLL.GetBySaediId(call.SaediToId);
                Client client = clientBLL.GetBySaediId(call.SaediFromId, programVersion: "2");
                if (client.HasAEPScheme && call.VisitReasonCode == "099")
                {
                    Panel1.Visible = true;
                }
                else
                {
                    Panel1.Visible = false;
                }

                SetOrderButtons(call);
                SetActionButtons(call);
                SetUsedButtons(call);
                SetCancelButtons(call);

                try
                {
                    if (partsBLL.UsesPartOrdering(call.Id) == false)
                    {
                        orderedPartsDiv.Visible = false;
                        actionPartsDiv.Visible = false;
                        cancelledPartsDiv.Visible = false;
                    }
                }
                catch (Exception er)
                {
                    ErrorHandler.ShowError(this.Page, "GET CLIENT", er.Message);
                }
            }
            catch (Exception er)
            {
                ErrorHandler.ShowError(this.Page, "SHOW PARTS FOR CALL", er.Message);
            }
        }

        protected void partsOrderDataSource_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            e.ObjectInstance = orderPartsBLL;
        }

        protected void partsActionDataSource_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            e.ObjectInstance = actionPartsBLL;
        }

        protected void partsCancelDataSource_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            e.ObjectInstance = cancelPartsBLL;
        }

        protected void partsUsedDataSource_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            e.ObjectInstance = usedPartsBLL;
        }
        private void FindAllButtons(Control parent, bool PDA = true)
        {
            foreach (Control c in parent.Controls)
            {
                if (c.GetType().ToString() == "System.Web.UI.WebControls.Button")
                {
                    Button button = c as Button;
                    if (button != null && button.ID != "WebEdit")
                    {
                        if (PDA) button.Enabled = false;
                        else if (button.ID == "okLinkButton")
                            button.Enabled = true;
                        else if (button.ID != "BtnRMA" && button.ID != "partReturnButton" && button.ID != "btnDeletePartFromSony" && button.ID != "BtnBookCourier")
                            button.Enabled = false;


                    }
                }
                if (c.Controls.Count > 0)
                {
                    FindAllButtons(c, PDA);
                }
            }
        }


        protected void partsUsedGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                editAllocatedPartLinkButton.CommandArgument = e.CommandArgument.ToString();
                removeAllocatedPartLinkButton.CommandArgument = e.CommandArgument.ToString();
                courierBookingButton.CommandArgument = e.CommandArgument.ToString();
                usePartLinkButton.CommandArgument = e.CommandArgument.ToString();
                SiteSession session = SiteSessionFactory.LoadSession(this.Page);
                Call call = new Call();
                try
                {
                    call = (Call)session.Data["call"];
                }
                catch (Exception er)
                {
                    ErrorHandler.ShowError(this.Page, "SESSION", er.Message);
                }
                if (call.IsSony)// 
                    setCourierButton(call);
            }
        }

        protected void partsUsedGridView_SelectedIndexChanged(object sender, EventArgs e)
        {
            SiteSession session = SiteSessionFactory.LoadSession(this.Page);
            Call call = (Call)session.Data["call"];
            SetUsedButtons(call);
        }

        private void SetUsedButtons(Call call)
        {
            CallPart part = usedPartsBLL.List.Find(delegate(CallPart cp) { return cp.PartReference.ToString() == removeAllocatedPartLinkButton.CommandArgument.ToString(); });
            addAllocatedPartLinkButton.Enabled = call.IsStatusWIP && call.UsedParts.Items.Count < maxParts && (!((call.StatusId == 3 || call.StatusId == 71) && call.IsSony) || !call.IsSony);
            if (AepJob)
            {
                addAllocatedPartLinkButton.Enabled = false;
                AWBBLL bll = new AWBBLL();
                var awbdetails = bll.SONYAWBDetails(call.SaediToId, call.AdditionalRef, call.ClientRef);
                DateTime result;
                if (DateTime.TryParse(awbdetails.awbDeliveryETA, out result))
                    addAllocatedPartLinkButton.Enabled = result <= DateTime.Now;
            }
            //todo:
            //if (!call.IsSony)
            //    addAllocatedPartLinkButton.Enabled = call.IsStatusWIP && call.UsedParts.Items.Count < maxParts;// (!((call.StatusId == 3 || call.StatusId == 71) && call.IsSony) || !call.IsSony);
            //else
            //{
            //    if ( call.IsStatusWIP && call.UsedParts.Items.Count < maxParts)
            //        addAllocatedPartLinkButton.Enabled =  (call.StatusId != 3 && call.StatusId != 71) && ((call.VisitReasonCode != "000" && call.VisitReasonCode != "097") && !SonyAcknowledgement);
            //    else
            //        addAllocatedPartLinkButton.Enabled = false;
            //}
            editAllocatedPartLinkButton.Enabled = usedPartsBLL.List.Count > 0 && partsUsedGridView.SelectedIndex > -1 && call.IsStatusWIP;
            removeAllocatedPartLinkButton.Enabled = usedPartsBLL.List.Count > 0 && partsUsedGridView.SelectedIndex > -1 && call.IsStatusWIP && !part.IsRmaDone;
            //  courierBookingButton.Enabled = usedPartsBLL.List.Count > 0 && partsUsedGridView.SelectedIndex > -1 && call.IsStatusWIP  ;            
            // courierBookingButton.Visible = false;
        }



        protected void partsActionGridView_SelectedIndexChanged(object sender, EventArgs e)
        {
            //  partsOrderGridView.SelectedIndex = -1;
            SiteSession session = SiteSessionFactory.LoadSession(this.Page);
            Call call = (Call)session.Data["call"];
            SetActionButtons(call);
        }

        private void SetActionButtons(Call call)
        {
            rejectPartLinkButton.Enabled = actionPartsBLL.List.Count > 0 && partsActionGridView.SelectedIndex > -1;
            acceptPartLinkButton.Enabled = actionPartsBLL.List.Count > 0 && partsActionGridView.SelectedIndex > -1 && call.IsStatusWIP;
        }

        protected void partsOrderGridView_SelectedIndexChanged(object sender, EventArgs e)
        {
            // partsActionGridView.SelectedIndex = -1;
            SiteSession session = SiteSessionFactory.LoadSession(this.Page);
            Call call = (Call)session.Data["call"];
            SetOrderButtons(call);
        }

        private void setCourierButton(Call call)
        {
            CallPart part = usedPartsBLL.List.Find(f => f.PartReference.ToString() == courierBookingButton.CommandArgument && f.SAEDICallRef == call.CallJobSeq.ToString() && f.SAEDIFromID == call.SaediFromId);//&&  f.ConNoteUrl.Length==0);

            // CallPart partCourier =parts.Find (x=>x.PartReference ==int.Parse( courierBookingButton.CommandArgument) && x.SAEDICallRef == call.CallJobSeq.ToString() && x.SAEDIFromID == call.SaediFromId); //courierBookingButton
            courierBookingButton.Enabled = (part.Equals(null)) ? false : (part.RmaDocumentUrl.Length > 0 && part.ConNoteUrl.Length <= 0 && call.IsStatusWIP);
        }
        private void SetOrderButtons(Call call)
        {
            CallPart part = orderPartsBLL.List.Find(delegate(CallPart cp) { return cp.PartReference.ToString() == partReturnButton.CommandArgument.ToString(); });
            string partDescription;
            try
            {
                partDescription = part.ReturnDescription;
            }
            catch
            {
                partDescription = string.Empty;
            }
            PartsBLL partsBLL = new PartsBLL();
            //   courierBookingButton.Enabled = (usedAllocatedPartsBLL.List.Count > 0) && part.Equals(null);
            string partCode = "";
            if (usePartLinkButton.CommandArgument != string.Empty)
            {
                List<CallPart> parts = partsBLL.GetPartsByCallId(call.Id);
                if (partsOrderGridView.SelectedIndex > -1)
                    partCode = parts.SingleOrDefault(x => x.PartReference == int.Parse(usePartLinkButton.CommandArgument)).Code;
            }

            //if( usePartLinkButton.CommandArgument !=string.Empty)
            //partCode = parts.Find(p => p.PartReference.ToString() == usePartLinkButton.CommandArgument.ToString()).Code.FirstOrDefault().ToString();
            // courierBookingButton.Visible = false;
            partReturnButton.Enabled = (orderPartsBLL.List.Count > 0 && partsOrderGridView.SelectedIndex > -1);
            usePartLinkButton.Enabled = true;
            if (orderPartsBLL.List.Count > 0 && partsOrderGridView.SelectedIndex > -1)
            {
                if (call.IsSony)
                {
                    if (AepJob)
                    {
                        usePartLinkButton.Enabled = false;
                        AWBBLL awb = new AWBBLL();
                        var awbResult = awb.SONYAWBDetails(call.SaediToId, call.AdditionalRef, call.ClientRef);

                        if ((awbResult != null) && !string.IsNullOrEmpty(awbResult.awbDeliveryETA) && DateTime.Parse(awbResult.awbDeliveryETA) <= DateTime.Now)
                        {
                            // usePartLinkButton.Text = awbResult.awbDeliveryETA;
                            usePartLinkButton.Enabled = true;
                        }
                    }
                    //string CheckAEPdelivery = System.Configuration.ConfigurationManager.AppSettings["CheckAEPdelivery"];
                    //if (bool.Parse(CheckAEPdelivery))
                    //    usePartLinkButton.Enabled = (!(partCode.StartsWith("99") && part.UnitPrice <= 0) && part.ReturnReference == "") && (part.DispatchDate != null);// Dispatched parts can only be allocated

                    else
                    {
                        // usePartLinkButton.Enabled = (!(partCode.StartsWith("99") && part.UnitPrice <= 0) && part.ReturnReference == "");// Dispatched parts can only be allocated
                        //   Dispatched parts can only be allocated except AEP part
                        if (part.DispatchDate == null && !part.Code.StartsWith("AEP"))
                            usePartLinkButton.Enabled = false;
                        // 99 code part can not be allocated
                        if (partCode.StartsWith("99") && part.UnitPrice <= 0)
                            usePartLinkButton.Enabled = false;
                        // if RMA done (ordered part RMA means- the part is damage , so it is returned by engineer, which created RMA) stop to allocate
                        if (part.ReturnReference != "")
                            usePartLinkButton.Enabled = false;
                    }
                }
                else usePartLinkButton.Enabled = true;
            }
            else usePartLinkButton.Enabled = false;
            cancelOrderLinkButton.Enabled = orderPartsBLL.List.Count > 0 && partsOrderGridView.SelectedIndex > -1;
            orderLinkButton.Enabled = call.IsStatusWIP && (!((call.StatusId == 3 || call.StatusId == 71) && call.IsSony) || !call.IsSony);
            //if (!call.IsSony)
            //orderLinkButton.Enabled = call.IsStatusWIP;// && (!((call.StatusId == 3 || call.StatusId == 71) && call.IsSony) || !call.IsSony);  
            //else
            //    orderLinkButton.Enabled = call.IsStatusWIP && (call.StatusId != 3 && call.StatusId != 71) && ((call.VisitReasonCode != "000" && call.VisitReasonCode != "097") && !SonyAcknowledgement);
            partEnquiryButton.Enabled = call.IsStatusWIP;
        }

        protected void partsCancelGridView_SelectedIndexChanged(object sender, EventArgs e)
        {
            SiteSession session = SiteSessionFactory.LoadSession(this.Page);
            Call call = (Call)session.Data["call"];
            SetCancelButtons(call);
        }

        private void SetCancelButtons(Call call)
        {
            viewCancelledLinkButton.Enabled = cancelPartsBLL.List.Count > 0 && partsCancelGridView.SelectedIndex > -1;
        }

        protected void editAllocatedPartLinkButton_Click(object sender, EventArgs e)
        {
            SiteSession session = SiteSessionFactory.LoadSession(this.Page);
            Call call = (Call)session.Data["call"];
            CallPart part = usedPartsBLL.List.Find(delegate(CallPart cp) { return cp.PartReference.ToString() == editAllocatedPartLinkButton.CommandArgument.ToString(); });

            if (part != null)
            {
                CallPart clonePart = usedPartsBLL.Clone(part);
                clonePart.Action = PartAction.Edit;
                if (session.Data.ContainsKey("part"))
                { session.Data["part"] = clonePart; }
                else
                { session.Data.Add("part", clonePart); }
                Response.Redirect("~/PartEdit.aspx");

                SiteSessionFactory.SaveSession(this.Page, session);
            }
        }

        protected void addAllocatedPartLinkButton_Click(object sender, EventArgs e)
        {
            SiteSession session = SiteSessionFactory.LoadSession(this.Page);
            session.Data.Remove("StockAddSearch");
            session.Data.Remove("PartStockNote");
            SiteSessionFactory.SaveSession(this.Page, session);

            Call call = new Call();
            try
            {
                call = (Call)session.Data["call"];
            }
            catch (Exception er)
            {
                ErrorHandler.ShowError(this.Page, "SESSION", er.Message);
            }

            CallPart part = new CallPart();
            part.Action = PartAction.Add;
            if (session.Data.ContainsKey("part"))
            { session.Data["part"] = part; }
            else
            { session.Data.Add("part", part); }
            SiteSessionFactory.SaveSession(this.Page, session);

            if (call.IsSony)  // If it is SONY
            {
                session.Data.Add("StockAddSearch", true);
                SiteSessionFactory.SaveSession(this.Page, session);

                if (Convert.ToBoolean(Session["UseAdvancedPartsearchCall"]))
                    Session["ReturnURL"] = "~/AdvancedPartSearch.aspx?from=stockpart";
                else
                    Session["ReturnURL"] = "~/PartSearch.aspx?from=stockpart";
                Response.Redirect("~/UpdateServiceEventStatus.aspx?action=ServiceUpdate&return=true", false);

            }
            else
                Response.Redirect("~/PartEdit.aspx");
        }

        protected void btnNoPartsUsed_Click(object sender, EventArgs e)
        {
            Session["ReturnURL"] = "~/UpdatePartConsumption.aspx?from=nopartsused";
            Response.Redirect("~/UpdateServiceEventStatus.aspx?action=ServiceUpdate&return=true", false);
        }

        protected void btnTechnicalBulletin_Click(object sender, EventArgs e)
        {
            Session["ReturnURL"] = "~/UpdatePartConsumption.aspx?from=technicalbulletin";
            Response.Redirect("~/UpdateServiceEventStatus.aspx?action=ServiceUpdate&return=true", false);
        }

        protected void partsOrderGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                usePartLinkButton.CommandArgument = e.CommandArgument.ToString();
                cancelOrderLinkButton.CommandArgument = e.CommandArgument.ToString();
                usePartLinkButton.CommandArgument = e.CommandArgument.ToString();
                partReturnButton.CommandArgument = e.CommandArgument.ToString();
            }

        }


        protected void partsActionGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                acceptPartLinkButton.CommandArgument = e.CommandArgument.ToString();
                rejectPartLinkButton.CommandArgument = e.CommandArgument.ToString();
            }
        }

        protected void partsCancelGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                viewCancelledLinkButton.CommandArgument = e.CommandArgument.ToString();
            }
        }

        protected void removeAllocatedPartLinkButton_Click(object sender, EventArgs e)
        {
            SiteSession session = SiteSessionFactory.LoadSession(this.Page);
            Call call = (Call)session.Data["call"];

            CallPart part = usedPartsBLL.List.Find(delegate(CallPart cp) { return cp.PartReference.ToString() == editAllocatedPartLinkButton.CommandArgument.ToString(); });

            if (part != null)
            {
                CallPart clonePart = usedPartsBLL.Clone(part);
                clonePart.Action = PartAction.Remove;
                if (session.Data.ContainsKey("part"))
                { session.Data["part"] = clonePart; }
                else
                { session.Data.Add("part", clonePart); }
                SiteSessionFactory.SaveSession(this.Page, session);

                if (call.IsSony)
                {
                    Response.Redirect("~/UpdatePartConsumption.aspx?from=removeallocated");
                }
                else
                    Response.Redirect("~/PartEdit.aspx");
            }
        }

        protected void usePartLinkButton_Click(object sender, EventArgs e)
        {
            SiteSession session = SiteSessionFactory.LoadSession(this.Page);
            Call call = (Call)session.Data["call"];

            CallPart part = orderPartsBLL.List.Find(f => f.PartReference.ToString() == usePartLinkButton.CommandArgument.ToString());

            if (part != null)
            {
                CallPart clonePart = usedPartsBLL.Clone(part);
                clonePart.Action = PartAction.Use;
                if (session.Data.ContainsKey("part"))
                { session.Data["part"] = clonePart; }
                else
                { session.Data.Add("part", clonePart); }
                SiteSessionFactory.SaveSession(this.Page, session);

                if (call.IsSony)
                    Response.Redirect("~/UpdatePartConsumption.aspx?from=allocate");
                else
                    Response.Redirect("~/PartView.aspx");
            }
        }

        protected void cancelOrderLinkButton_Click(object sender, EventArgs e)
        {
            CallPartView(((Button)sender).CommandArgument, orderPartsBLL.List, PartAction.Cancel);
        }

        protected void acceptPartLinkButton_Click(object sender, EventArgs e)
        {
            CallPartView(((Button)sender).CommandArgument, actionPartsBLL.List, PartAction.Accept);
        }

        protected void rejectPartLinkButton_Click(object sender, EventArgs e)
        {
            CallPartView(((Button)sender).CommandArgument, actionPartsBLL.List, PartAction.Reject);
        }

        protected void viewCancelledLinkButton_Click(object sender, EventArgs e)
        {
            CallPartView(((Button)sender).CommandArgument, cancelPartsBLL.List, PartAction.View);
        }

        private void CallPartView(string commandArgument, List<CallPart> basket, PartAction editMode)
        {
            SiteSession session = SiteSessionFactory.LoadSession(this.Page);
            Call call = (Call)session.Data["call"];
            CallPart part = basket.Find(f => f.PartReference.ToString().Trim() == commandArgument.Trim());

            if (part != null)
            {
                if (session.Data.ContainsKey("part"))
                { session.Data["part"] = usedPartsBLL.Clone(part); }
                else
                { session.Data.Add("part", usedPartsBLL.Clone(part)); }
                SiteSessionFactory.SaveSession(this.Page, session);
                ErrorHandler.LogToFile(session, part.Code + " " + part.Description);
                Response.Redirect("~/PartReturn.aspx");
            }
            else
                Response.Redirect("~/PartReturn.aspx?partCount=" + basket.Count.ToString() + "&commandArgument=" + commandArgument);
        }

        protected void refreshCallImageButton_Click(object sender, ImageClickEventArgs e)
        {
            SiteSession session = SiteSessionFactory.LoadSession(this.Page);
            CallsBLL refreshCallBLL = new CallsBLL();
            Call call = (Call)session.Data["call"];
            Call nCall = refreshCallBLL.GetById(call.Id);
            session.Data["call"] = nCall;
            SiteSessionFactory.SaveSession(this.Page, session);
            string url = Path.GetFileName(Request.Url.AbsolutePath);
            Response.Redirect(url);
        }

        protected void partReturnTopButton_Click(object sender, EventArgs e)
        {
            SiteSession session = SiteSessionFactory.LoadSession(this.Page);
            CallPart part = usedPartsBLL.List.Find(f => f.PartReference.ToString().Trim() == ((Button)sender).CommandArgument.Trim());

            if (part != null)
            {
                if (session.Data.ContainsKey("part"))
                { session.Data["part"] = usedPartsBLL.Clone(part); }
                else
                { session.Data.Add("part", usedPartsBLL.Clone(part)); }
                SiteSessionFactory.SaveSession(this.Page, session);
                Response.Redirect("~/PartReturn.aspx");
            }
        }

        protected void orderLinkButton_Click(object sender, EventArgs e)
        {
            SiteSession session = SiteSessionFactory.LoadSession(this.Page);
            session.Data.Remove("StockAddSearch");
            session.Data.Remove("PartStockNote");
            SiteSessionFactory.SaveSession(this.Page, session);
            Call call = (Call)session.Data["call"];
            if (call.IsSony)
            {
                if (Convert.ToBoolean(Session["UseAdvancedPartsearchCall"]))
                    Session["ReturnURL"] = "~/AdvancedPartSearch.aspx?from=order";
                else
                    Session["ReturnURL"] = "~/PartSearch.aspx?from=order";

                //Session["ReturnURL"] = "~/AEPResponse.aspx";
                Response.Redirect("~/UpdateServiceEventStatus.aspx?action=ServiceUpdate&return=true", false);
            }
            else
            {
                if (Convert.ToBoolean(Session["UseAdvancedPartsearchCall"]))
                    Response.Redirect("~/AdvancedPartSearch.aspx?from=order");
                else
                    Response.Redirect("~/PartSearch.aspx?from=order");

            }
        }

        protected void partEnquiryButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/NotePart.aspx");
        }

        protected void partReturnButton_Click(object sender, EventArgs e)
        {
            CallPartView(((Button)sender).CommandArgument, orderPartsBLL.List, PartAction.Return);
        }

        protected void WebEdit_Click(object sender, EventArgs e)
        {
            SiteSession session = SiteSessionFactory.LoadSession(this.Page);
            Call call = (Call)session.Data["call"];
            CallsBLL PDACAll = new CallsBLL();
            PDACAll.UpdatePDACallEditable(call.Id);
            Response.Redirect(Request.RawUrl);
        }



        protected void btnUpdateServiceEvent_Click(object sender, EventArgs e)
        {
            SiteSession session = SiteSessionFactory.LoadSession(this.Page);
            Call call = (Call)session.Data["call"];
            HttpContext.Current.Response.RedirectPermanent("~/UpdateServiceEvent.aspx");
        }

        protected void courierBookingButton_Click(object sender, EventArgs e)
        {
            SiteSession session = SiteSessionFactory.LoadSession(this.Page);
            Call call = (Call)session.Data["call"];

            CallPart part = usedPartsBLL.List.Find(f => f.PartReference.ToString() == ((Button)sender).CommandArgument && f.RmaDocumentUrl.Length > 0 && f.ConNoteUrl.Length == 0);//&&  f.ConNoteUrl.Length==0);

            if (part != null)
            {
                if (session.Data.ContainsKey("part"))
                { session.Data["part"] = usedPartsBLL.Clone(part); }
                else
                { session.Data.Add("part", usedPartsBLL.Clone(part)); }
                SiteSessionFactory.SaveSession(this.Page, session);
                Response.Redirect("~/CourierBooking.aspx");
            }
        }

        protected void btnDeletePartFromSony_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/DeletePartsFromSony.aspx");
        }

        protected void LnkConNote_Click(object sender, CommandEventArgs e)
        {
            string[] arg = new string[3];
            arg = e.CommandArgument.ToString().Split(';');
            string consignmentNumber = arg[0];
            string Courierid = arg[1];
            string bookinguniqueNumber = arg[2];

            Call call = new Call();

            SiteSession session = SiteSessionFactory.LoadSession(this.Page);
            call = (Call)session.Data["call"];
            SAEDIFromId = call.SaediFromId;
            string s = "window.open('" + GetFilename((CourierId)Enum.Parse(typeof(CourierId), Courierid), SAEDIFromId, bookinguniqueNumber, consignmentNumber, false) + ".html'";
            ClientScript.RegisterStartupScript(this.GetType(), "PopupWindow", "<script language='javascript'>" + s + ",'Title','width=1000,height=2000')</script>");
        }

        private string GetFilename(CourierId courierid, string SaediFromId, string BookingUniqNumber, string consignmentNumber, bool Label)
        {
            string filename = string.Empty;
            if (courierid == CourierId.TNT)
            {
                if (!Label)
                    filename = string.Concat("TNTOutputfiles/", SaediFromId, "_", "CONNOTE_Ref", BookingUniqNumber, "_", "CON", consignmentNumber);
                else
                    filename = string.Concat("TNTOutputfiles/", SaediFromId, "_", "Label_Ref", BookingUniqNumber, "_", "Lab", consignmentNumber);
            }
            return filename;
        }
        protected void LnkLabel_Click(object sender, CommandEventArgs e)
        {

            //    string[] arg = new string[3];
            //    arg = e.CommandArgument.ToString().Split(';');
            //    string consignmentNumber = arg[0];
            //    string Courierid = arg[1];
            //    string bookinguniqueNumber = arg[2];

            //    Call call = new Call();

            //    SiteSession session = SiteSessionFactory.LoadSession(this.Page);
            //    call = (Call)session.Data["call"]; SaediFromId = call.SaediFromId;
            //    string s = "window.open('" + GetFilename((CourierId)Enum.Parse(typeof(CourierId), Courierid), SaediFromId, bookinguniqueNumber, consignmentNumber, true) + ".html'";
            //    ClientScript.RegisterStartupScript(this.GetType(), "PopupWindow1", "<script language='javascript' >" + s + ",'Title','width=1000,height=2000')</script>");
        }
        private static void createFolder(FileResult filelist)
        {
            string file1 = HttpContext.Current.Server.MapPath("~/tntoutputfiles");
            string directoryName = file1 + filelist.foldername;
            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);
            foreach (var file in filelist.files)
            {
                if (File.Exists(directoryName + file.filename))
                {

                    File.Delete(directoryName + file.filename);
                }

                // Create the file. 
                using (FileStream fs = File.Create(directoryName + "\\" + file.filename))
                {
                    Byte[] info = file.fileBytes;
                    // Add some information to the file.
                    fs.Write(info, 0, info.Length);
                }
            }
        }
        //private static void AddFile(FileResult file)
        //{

        //    string file1 = HttpContext.Current.Server.MapPath("~/tntoutputfiles/");
        //    if (File.Exists(file1 + SaediFromId + file.filename))
        //    {

        //        File.Delete(file1 + SaediFromId + file.filename);
        //    }

        //    // Create the file. 
        //    using (FileStream fs = File.Create(file1 + SaediFromId + file.filename))
        //    {
        //        Byte[] info = file.fileBytes;
        //        // Add some information to the file.
        //        fs.Write(info, 0, info.Length);
        //    }
        //}
        private void copyfiles(List<FileResult> fileresult, int courierid)
        {
            string ResponseFilepath = string.Empty;
            if (courierid == (int)CourierId.TNT)
                ResponseFilepath = HttpContext.Current.Server.MapPath("~/tntoutputfiles/");
            foreach (var file in fileresult)
            {
                if (file.filename != null)
                {
                    //AddFile(file);
                }
                else
                {
                    createFolder(file);
                }
            }
        }

        protected void partsOrderGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                CallPart part = ((Mobile.Portal.Classes.CallPart)(e.Row.DataItem));
                e.Row.FindControl("BtnBookCourier").Visible = SonyStatusComplete && (part.NeedCollection || (NeedCollectionforSwapClaim(part)));
            }
        }
        private string GetSONYReservationIDFromInspection(string SaediFromId, string SaediToId, string ClientRef)
        {
            InspectionDataBLL inspection = new InspectionDataBLL();
            InspectionData id = new InspectionData();
            try
            {
                List<InspectionData> idList = inspection.GetAEPReservationIDFromInspectionData(SaediFromId, SaediToId, ClientRef);
                id = idList.ToList()[0];
            }
            catch
            {
                id = new InspectionData();
                id.Response = string.Empty;
            }
            return id.Response;
        }
        protected void BtnRMA_Click(object sender, EventArgs e)
        {

            Button btn = sender as Button;
            GridViewRow row = btn.NamingContainer as GridViewRow;
            string pk = partsUsedGridView.DataKeys[row.RowIndex].Values["PartReference"].ToString();
            string ErrorMessage = string.Empty;
            List<RMARef> responseList = new List<RMARef>();
            RMARef response = new RMARef();
            RMARefBLL rmaRefBLL = new RMARefBLL();
            PartsBLL SAEDIParts = new PartsBLL();
            SiteSession session = SiteSessionFactory.LoadSession(this.Page);
            //Call call = new Call();
            //   call = (Call)session.Data["call"];      
            string SaediID;
            if (call.SaediFromId != "SONY3C")
                SaediID = call.SaediFromId;
            else
                SaediID = call.SaediToId;
            SAEDIParts.List = SAEDIParts.GetSAEDIPartsByCall(SaediID, call.ClientRef).ToList();

            CallPart saediPart = SAEDIParts.List.Find(p => p.PartReference == int.Parse(pk));
            string errMessage = string.Empty;

            ClientBLL clientBLL = new ClientBLL();
            Client client = clientBLL.GetBySaediId(session.Login.SaediId, programVersion: "2");
            List<string> userPassword =
                Mobile.Portal.Helpers.SonyServicesHelper.GetSONYWebServiceUserPassword(client.Currency);
            if (saediPart.Code == "000000010")
            {
                GenerateSwap();
                //response = responseList.ToList()[0];
                //string gpToolRmaId =
                //    GetSONYReservationIDFromInspection(call.SaediFromId, call.SaediToId, call.ClientRef);
                //string claimType = "SERVICEPRODUCTRETURN";// saediPart.WarrantyStatus == "OOW" ? "OUTOFWARRANTY" : "WARRANTY";
                //if (saediPart.Code.ToUpper().StartsWith("AEP"))
                //    claimType = "AEP";
                //responseList = rmaRefBLL.RMAGenerateRequest(
                //    saediFromId: call.SaediFromId,
                //    clientRef: call.ClientRef,
                //    userID: userPassword[0],
                //    password: userPassword[1],
                //      aepBookInRef: string.Empty,
                //    externalMatId: string.Empty, 
                //    claimType: claimType,
                //    clientReference: call.ImportedTechnicianCD,
                //    faultCode: string.Empty,
                //    gpToolRmaId: gpToolRmaId,
                //    repairNumber: call.ClientRef,
                //    modelID: call.Appliance.Model.Code,
                //    partNumber: string.Empty,      // record.INPUTsonyPartNumber,
                //    remark: string.Empty,
                //    returnQuantity: 1,
                //    serialNumber: call.Appliance.SerialNumber,
                //    sonNumber: "",    // record.INPUTson,  // rmaPart.OrderReference,
                //    partNumberReceived: "");
            }
            else
            {
                string aepBookingRef = (string.IsNullOrEmpty(call.ExtendedCardNumber) || string.IsNullOrWhiteSpace(call.ExtendedCardNumber)) ?
                       GetSONYReservationIDFromInspection(call.SaediFromId, call.SaediToId, call.ClientRef) :
                       call.ExtendedCardNumber;
                string claimType = saediPart.WarrantyStatus == "OOW" ? "OUTOFWARRANTY" : "WARRANTY";
                if (saediPart.Code.ToUpper().StartsWith("AEP"))
                    claimType = "AEP";
                try
                {
                    responseList = rmaRefBLL.RMAGenerateRequest(
                        saediFromId: call.SaediFromId,
                        clientRef: call.ClientRef,
                        userID: userPassword[0],      // "GBEPACIF",                        
                        password: userPassword[1],    // "HUL3L49", 
                        aepBookInRef: aepBookingRef,
                        externalMatId: saediPart.INPUTascMaterialId, // record.INPUTascMaterialId
                        claimType: claimType,
                        clientReference: call.ImportedTechnicianCD,
                        faultCode: string.Empty,
                        gpToolRmaId: string.Empty,
                        repairNumber: call.ClientRef,
                        modelID: call.Appliance.Model.Code,
                        partNumber: saediPart.Code,      // record.INPUTsonyPartNumber,
                        remark: string.Empty,
                        returnQuantity: saediPart.Quantity,
                        serialNumber: call.Appliance.SerialNumber,
                        sonNumber: saediPart.INPUTson,    // record.INPUTson,  // rmaPart.OrderReference,
                        partNumberReceived: saediPart.Code);


                    response = responseList.ToList()[0];
                }
                catch (Exception ex)
                {
                    response = new RMARef();
                    response.success = false;
                    response.errorMessage = errMessage + " ( " + ex.Message + " ) ";
                }


                response.SaediFromId = call.SaediFromId;
                response.ClientRef = call.ClientRef;
                rmaRefBLL.InsertRMA(response);
                if (response.success)
                {
                    PartsBLL rmaPartsBLL = new PartsBLL();
                    if (response.rmaDocumentUrl != string.Empty) // UPDATE RMA in complete service only when url is returned
                    {
                        if (saediPart.StatusID == "V")
                        {
                            // 2.) UPDATE RMA FOR STOCK/VAN PART: 
                            try
                            {
                                rmaPartsBLL.ReturnByCallAndPartIdForSONY(call.Id, saediPart.PartReference, response.rmaId, response.rmaDocumentUrl);
                            }
                            catch (Exception er)
                            {
                                ErrorHandler.ShowError(this.Page, "RETURN BY CALL - VAN PART", er.Message);
                            }
                        }
                        else
                        {
                            // 3.) UPDATE RMA FOR ALLOCATED PART:
                            try
                            {
                                rmaPartsBLL.ReturnByCallAndPartId(call.Id, saediPart.PartReference, response.rmaId, response.rmaDocumentUrl);
                            }
                            catch (Exception er)
                            {
                                ErrorHandler.ShowError(this.Page, "RETURN BY CALL - ALLOCATE", er.Message);
                            }
                        }
                    }
                }
                RMARef.RmaResponseForView view = new RMARef.RmaResponseForView();
                response.statusID = saediPart.StatusID;
                view = rmaRefBLL.FormatRMAResponse(response);
                view.StockCode = saediPart.Code;
                List<RMARef.RmaResponseForView> listResponse = new List<RMARef.RmaResponseForView>();
                listResponse.Add(view);
                if (response.success)
                    titleRMA.InnerText = "RMA is done for the part:" + saediPart.Code;
                else
                    titleRMA.InnerText = "RMA is failed  for the part:" + saediPart.Code;

                repeaterRMA.DataSource = listResponse;
                repeaterRMA.DataBind();
                PartsList.Visible = false;
                divRMAOutput.Visible = true;
                if (!response.success)
                {
                    okLinkButton.Visible = true;
                    okLinkButton.Enabled = true;
                }

                else
                {



                    string queryString = string.Format("Collectionjob.aspx?SAEDIID={0}&ClientRef={1}&RMAREF={2}", call.SaediFromId, call.ClientRef, response.rmaId);


                    Iframe.Attributes.Add("src", queryString);

                    ModalPopupExtender1.Show();

                }
            }
        }

        protected void okLinkButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Parts.aspx");
        }

        protected void BtnBookCourier_Click_old(object sender, CommandEventArgs e)
        {
            //    Button btn = sender as Button;
            //    GridViewRow row = btn.NamingContainer as GridViewRow;
            //    string pk = partsUsedGridView.DataKeys[row.RowIndex].Values["PartReference"].ToString();
            //    PartsBLL SAEDIParts = new PartsBLL();
            //SiteSession session = SiteSessionFactory.LoadSession(this.Page);
            ////Call call = new Call();
            //call = (Call)session.Data["call"];
            //CallPart part = usedPartsBLL.List.Find(f => f.ReturnReference.ToString() == e.CommandArgument.ToString());


            //    string SaediID;
            //    if (call.SaediFromId != "SONY3C")
            //        SaediID = call.SaediFromId;
            //    else
            //        SaediID = call.SaediToId;
            //    SAEDIParts.List = SAEDIParts.GetSAEDIPartsByCall(SaediID, call.ClientRef).ToList();

            //    CallPart saediPart = SAEDIParts.List.Find(p => p.PartReference == int.Parse(pk));
            string queryString = string.Empty;
            //if (part != null && part.Code == "000000010")
            //{
            //    string rmaId = e.CommandArgument.ToString();
            //    queryString = string.Format("Collectionjob.aspx?SAEDIID={0}&ClientRef={1}&RMAREF={2}&SWAP2Credit={3}", call.SaediFromId, call.ClientRef, call.SonySwapCreditRMAid, true);

            //}
            //else
            //{
            string rmaId = e.CommandArgument.ToString();
            queryString = string.Format("Collectionjob.aspx?SAEDIID={0}&ClientRef={1}&RMAREF={2}", call.SaediFromId, call.ClientRef, rmaId);
            //}

            Iframe.Attributes.Add("src", queryString);

            ModalPopupExtender1.Show();


        }




        protected void partsUsedGridView_DataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                CallPart part = ((Mobile.Portal.Classes.CallPart)(e.Row.DataItem));
                // if (((Mobile.Portal.Classes.CallPart)(e.Row.DataItem)).Code == "000000010")
                e.Row.FindControl("BtnRMA").Visible = NeedRMA(part);
                //((bool)this.SonyStatusComplete == true) ? (((part.IsAllocated || part.StatusID.ToString() == "V") && (!part.IsBulletin && part.Code.ToString() != "TECHNICALBULLETIN")
                //&& (part.Code != "000000010" || IsSwap2Credit) && part.INPUTson.ToString().ToUpper() != "FOC" && !part.IsRmaDone && part.IsSony == true) && this.IsSonyStatusInCompleted) : false;
                e.Row.FindControl("BtnBookCourier").Visible = SonyStatusComplete && (part.NeedCollection || (NeedCollectionforSwapClaim(part)));

            }
        }

        private bool NeedRMA(CallPart part)
        {
            if ((bool)this.SonyStatusComplete != true)
                return false;
            else if (part.IsSony == true)
            {
                if (part.Code != "000000010")
                {
                    return ((part.IsAllocated || part.StatusID.ToString() == "V") && (!part.IsBulletin && part.Code.ToString() != "TECHNICALBULLETIN")
                 && part.INPUTson.ToString().ToUpper() != "FOC" && !part.IsRmaDone);
                }
                else
                {
                    return IsSwap2Credit;
                }
            }
            else
                return false;
        }
        private bool NeedCollectionforSwapClaim(CallPart part)
        {
            if (part.Code != "000000010")
                return false;
            else if (call.SonySwapCreditRMAid == string.Empty)
                return false;
            else
            {
                Swap2CreditBLL _bll = new Swap2CreditBLL();
                var result = _bll.GetSwap2CreditResults(SAEDIFromId, ClientRef);
                if (result.Count > 0)
                {
                    return result.Exists(x => x.statusCode == "YX" && !string.IsNullOrEmpty(x.rmaId) && !string.IsNullOrEmpty(x.rmaDocumentUrl));

                }
                else
                    return false;
            }

        }
        private bool Swap2CreditApproved(string SAEDIFromId, string ClientRef)
        {
            if (call.IsSony)
            {
                Swap2CreditBLL _bll = new Swap2CreditBLL();
                var result = _bll.GetSwap2CreditResults(SAEDIFromId, ClientRef);
                if (result.Count > 0)
                {
                    return result.Exists(x => x.statusCode == "YX" && string.IsNullOrEmpty(x.rmaId)) && call.Iris.RepairCode!="CS"; // CS- Item to be scrapped - no need for RMA

                }
                else
                    return false;
            }
            else
                return false;
        }

        protected void BtnBookCourier_Click(object sender, CommandEventArgs e)
        {
            string rmaId = e.CommandArgument.ToString();
            RMARefBLL rmaRefBLL = new RMARefBLL();
            PartsBLL SAEDIParts = new PartsBLL();

            SiteSession session = SiteSessionFactory.LoadSession(this.Page);
            Call call = new Call();
            call = (Call)session.Data["call"];

            ClientBLL clientBLL = new ClientBLL();
            Client client = clientBLL.GetBySaediId(call.SaediFromId, programVersion: "2");

            SAEDIParts.List = SAEDIParts.GetSAEDIPartsByCall(call.SaediFromId, call.ClientRef).ToList();

            CallPart saediPart = SAEDIParts.List.Find(p => p.ReturnReference == rmaId);

            string name = client.CompanyName != null ? client.CompanyName.Trim() : "";

            string[] address = new string[5];
            address[0] = string.IsNullOrEmpty(client.DeliveryAddress.Address1) ? "" : client.DeliveryAddress.Address1.Trim();
            address[1] = string.IsNullOrEmpty(client.DeliveryAddress.Address2) ? "" : client.DeliveryAddress.Address2.Trim();
            address[2] = string.IsNullOrEmpty(client.DeliveryAddress.Address3) ? "" : client.DeliveryAddress.Address3.Trim();
            address[3] = string.IsNullOrEmpty(client.DeliveryAddress.Address4) ? "" : client.DeliveryAddress.Address4.Trim();
            address[4] = string.IsNullOrEmpty(client.DeliveryAddress.Address5) ? "" : client.DeliveryAddress.Address5.Trim();

            string line1 = "";

            for (int i = 0; i < 5; i++)
            {
                if (line1 != "" && address[i] != "")
                    line1 += (" " + address[i]);
                else if (address[i] != "")
                    line1 = address[i];
            }

            string city = client.DeliveryAddress.City != null ? client.DeliveryAddress.City.Trim() : "";
            string postcode = client.DeliveryAddress.PostalCode != null ? client.DeliveryAddress.PostalCode.Trim() : "";
            string country = client.DeliveryAddress.Country != null ? client.DeliveryAddress.Country.Trim() : "";
            int serviceID = Convert.ToInt32(call.Id);
            int remittanceID = Convert.ToInt32(saediPart.Id);
            string serviceKey = ConfigurationManager.AppSettings["ShipmateServiceKey"];
            string reference = rmaId + "-1";
            string title = "Create Shipmate consignment";

            string queryString = string.Format("Shipmate.aspx?" +
                "Title={0}&" +
                "ServiceID={1}&" +
                "RemittanceID={2}&" +
                "ConsignmentReference={3}&" +
                "ServiceKey={4}&" +
                "Name={5}&" +
                "Line1={6}&" +
                "City={7}&" +
                "Postcode={8}&" +
                "Country={9}&" +
                "Reference={10}&" +
                "SaediFromId={11}",
                title,
                serviceID.ToString(),
                remittanceID.ToString(),
                rmaId,
                serviceKey,
                name,
                line1,
                city,
                postcode,
                country,
                reference,
                call.SaediFromId);

            Iframe.Attributes.Add("src", queryString);
            ModalPopupExtender1.Show();
        }

        protected void linkBtnShowConsignmentDetails_Click(object sender, CommandEventArgs e)
        {
            string title = "Consignment details";
            string trackingReference = e.CommandArgument.ToString();
            string queryString = string.Format("Shipmate.aspx?Title={0}&TrackingReference={1}", title, trackingReference);
            Iframe.Attributes.Add("src", queryString);
            ModalPopupExtender1.Show();
        }

        protected void linkBtnShowShipmateLabel_Click(object sender, CommandEventArgs e)
        {
            string title = "Shipmate label";
            string trackingReference = e.CommandArgument.ToString();
            string queryString = string.Format("Shipmate.aspx?Title={0}&TrackingReference={1}", title, trackingReference);
            Iframe.Attributes.Add("src", queryString);
            ModalPopupExtender1.Show();
        }
    }
}

