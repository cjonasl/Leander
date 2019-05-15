        protected void BtnBookCourier_Click(object sender, CommandEventArgs e)
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