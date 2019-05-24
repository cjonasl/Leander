using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MobilePortal
{
    public partial class ShipMate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblTitle.Text = Request.QueryString["ShipMateTitle"];
            lblConsignmentReference.Text = Request.QueryString["ShipMateConsignmentReference"];
            lblParcelReference.Text = Request.QueryString["ShipMateParcelReference"];
            lblCarrier.Text = Request.QueryString["ShipMateCarrier"];
            lblServiceName.Text = Request.QueryString["ShipMateServiceName"];
            lblTrackingReference.Text = Request.QueryString["ShipMateTrackingReference"];
            lblCreatedBy.Text = Request.QueryString["ShipMateCreatedBy"];
            lblCreatedWith.Text = Request.QueryString["ShipMateCreatedWith"];
            lblCreatedAt.Text = Request.QueryString["ShipMateCreatedAt"];
            lblDeliveryName.Text = Request.QueryString["ShipMateDeliveryName"];
            lblLine.Text = Request.QueryString["ShipMateLine"];
            lblCity.Text = Request.QueryString["ShipMateCity"];
            lblPostcode.Text = Request.QueryString["ShipMatePostcode"];
            lblCountry.Text = Request.QueryString["ShipMateCountry"];
        }
    }
}