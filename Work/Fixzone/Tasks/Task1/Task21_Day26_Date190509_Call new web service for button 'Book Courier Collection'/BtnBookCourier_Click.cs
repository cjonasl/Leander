            //---------- New code for this eventhandler May 2019 ------------
            FzShipMate fzShipMate = new FzShipMate();

            /*ToAddressRequest ToAddress = new ToAddressRequest("David Xu", "35 Ford Street", "Derby", "DE1 1EE", "GB");
            List<Parcel> Parcels = new List<Parcel>();
            Parcels.Add(new Parcel("80000884-1", 3000, 20, 10, 15));
            ConsignmentRequestData ConsignmentRequestData = new Mobile.Portal.BLL.FzShipMate.ConsignmentRequestData("80000884", fzShipMate.Token, "DPDNEXT", ToAddress, Parcels);
            ConsignmentResponseData ConsignmentResponseData = fzShipMate.CreateConsignment(ConsignmentRequestData);*/

            CancelConsignmentResponse cancelConsignmentResponse = fzShipMate.CancelConsignments("80000883");
            Mobile.Portal.BLL.FzShipMate.Utility.Print("C:\\tmp\\CancelConsignmentResponse.txt", cancelConsignmentResponse.ToString());

            //ServicesResponse servicesResponse = fzShipMate.GetServices();
            //Mobile.Portal.BLL.FzShipMate.Utility.Print("C:\\tmp\\ServicesResponse.txt", servicesResponse.ToString());
            return;
            //---------------------------------------------------------------


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

