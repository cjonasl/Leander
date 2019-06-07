using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mobile.Portal.BLL.Shipmate;
using Mobile.Portal.BLL;

namespace MobilePortal
{
    public partial class ShipmatePage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                return;

            Shipmate shipmate;
            ConsignmentResponse consignmentResponse = null;
            string title = Request.QueryString["Title"];
            lblTitle.Text = title;

            if (title == "Create Shipmate consignment")
            {
                txtServiceID.Text = Request.QueryString["ServiceID"];
                txtRemittanceID.Text = Request.QueryString["RemittanceID"];
                txtConsignmentReference.Text = Request.QueryString["ConsignmentReference"];
                txtServiceKey.Text = Request.QueryString["ServiceKey"];
                txtName.Text = Request.QueryString["Name"];
                txtLine1.Text = Request.QueryString["Line1"];
                txtCity.Text = Request.QueryString["City"];
                txtPostcode.Text = Request.QueryString["Postcode"];
                txtCountry.Text = Request.QueryString["Country"];
                txtReference.Text = Request.QueryString["Reference"];
                txtWeight.Text = "3000";
                txtWidth.Text = "20";
                txtLength.Text = "10";
                txtDepth.Text = "15";
                SaediFromId.Value = Request.QueryString["SaediFromId"]; //Hidden field
                UpdateVisibility(true, false, false, false);
            }
            else if (title == "Consignment details")
            {
                try
                {
                    string trackingReference = Request.QueryString["TrackingReference"]; ;
                    shipmate = new Shipmate();
                    consignmentResponse = shipmate.GetLabel(trackingReference);
                    lblTitle.Text = title;
                    SetLblText(consignmentResponse);
                    UpdateVisibility(false, false, true, false);
                }
                catch (Exception ex)
                {
                    divError.InnerText = string.Format("An error occurred:\r\n{0}", ex.Message);
                    UpdateVisibility(false, false, false, true);
                }
            }
        }

        private void UpdateVisibility(bool showDivCreateConsignmenmt, bool showDivNonPositiveIntegerError, bool showDivConsignmentDetails, bool showDivError)
        {
            divCreateConsignmenmt.Visible = showDivCreateConsignmenmt;
            divNonPositiveIntegerError.Visible = showDivNonPositiveIntegerError;
            divConsignmentDetails.Visible = showDivConsignmentDetails;
            divError.Visible = showDivError;
        }

        private void SetLblText(ConsignmentResponse consignmentResponse)
        {
            lblConsignmentReference.Text = consignmentResponse.data[0].consignment_reference;
            lblParcelReference.Text = consignmentResponse.data[0].parcel_reference;
            lblCarrier.Text = consignmentResponse.data[0].carrier;
            lblServiceName.Text = consignmentResponse.data[0].service_name;
            lblTrackingReference.Text = consignmentResponse.data[0].tracking_reference;
            lblCreatedBy.Text = consignmentResponse.data[0].created_by;
            lblCreatedWith.Text = consignmentResponse.data[0].created_with;
            lblCreatedAt.Text = consignmentResponse.data[0].created_at.ToString();
            lblDeliveryName.Text = consignmentResponse.data[0].to_address.delivery_name;
            lblLine.Text = consignmentResponse.data[0].to_address.line_1;
            lblCity.Text = consignmentResponse.data[0].to_address.city;
            lblPostcode.Text = consignmentResponse.data[0].to_address.postcode;
            lblCountry.Text = consignmentResponse.data[0].to_address.country;
        }

        protected void btnCreateConsignment_Click(object sender, EventArgs e)
        {
            int shipmateConsignmentCreationId;
            Shipmate shipMate;
            ConsignmentRequest consignmentRequest;
            ConsignmentResponse consignmentResponse;
            ToAddressRequest toAddress;
            List<Parcel> parcels;
            Parcel parcel;
            PartsBLL SAEDIParts = new PartsBLL();
            RMARefBLL rmaRefBLL = new RMARefBLL();

            int n;
            string[] v = new string[] { txtWeight.Text, txtWidth.Text, txtLength.Text, txtDepth.Text };

            if (v.Any(x => !int.TryParse(x, out n) || (int.TryParse(x, out n) && n < 1)))
            {
                UpdateVisibility(true, true, false, false);
            }
            else
            {
                try
                {
                    toAddress = new ToAddressRequest(txtName.Text, txtLine1.Text, txtCity.Text, txtPostcode.Text, txtCountry.Text);
                    parcel = new Parcel(txtReference.Text, int.Parse(txtWeight.Text), int.Parse(txtWidth.Text), int.Parse(txtLength.Text), int.Parse(txtDepth.Text));
                    parcels = new List<Parcel>();
                    parcels.Add(parcel);
                    shipMate = new Shipmate();
                    consignmentRequest = new ConsignmentRequest(int.Parse(txtServiceID.Text), int.Parse(txtRemittanceID.Text), txtConsignmentReference.Text, shipMate.Token, txtServiceKey.Text, toAddress, parcels);
                    consignmentResponse = shipMate.CreateConsignment(SaediFromId.Value, consignmentRequest, out shipmateConsignmentCreationId);
                    rmaRefBLL.UpdateCollectionjob(txtConsignmentReference.Text, consignmentResponse.data[0].tracking_reference, null, false, shipmateConsignmentCreationId);
                    lblTitle.Text = "Consignment was created successfully";
                    SetLblText(consignmentResponse);
                    UpdateVisibility(false, false, true, false);
                }
                catch (Exception ex)
                {
                    spanError.InnerText = string.Format("An error occurred: {0}", ex.Message);
                    UpdateVisibility(false, false, false, true);
                }
            }
        }
    }
}