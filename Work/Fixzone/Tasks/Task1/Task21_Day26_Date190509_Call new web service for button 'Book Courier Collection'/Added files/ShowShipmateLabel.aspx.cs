﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mobile.Portal.BLL.Shipmate;
using Mobile.Portal.Session;

namespace MobilePortal
{
    public partial class ShowShipMateLabel : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string trackingReference = Request.QueryString["TrackingReference"];

            Response.ContentType = "application/pdf";
            SiteSession session = SiteSessionFactory.LoadSession(this.Page);
            Shipmate shipmate = new Shipmate(session.Login.CreatedBy);
            ConsignmentResponse consignmentResponse = shipmate.GetLabel(trackingReference);
            Session["ShipMateMediaFile"] = consignmentResponse.data[0].pdf;
            Response.BinaryWrite(Convert.FromBase64String(consignmentResponse.data[0].pdf));
            Response.Flush();
            Response.Close();
        }
    }
}