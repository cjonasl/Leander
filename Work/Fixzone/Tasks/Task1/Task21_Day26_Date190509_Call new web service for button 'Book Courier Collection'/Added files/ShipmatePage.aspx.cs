using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mobile.Portal.BLL.Shipmate;
using Mobile.Portal.BLL;
using Mobile.Portal.Classes;
using Mobile.Portal.Session;

namespace MobilePortal
{
    public partial class ShipmatePage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                return;

            string title = Request.QueryString["Title"];

            if (title != null)
                lblTitle.Text = title;

            if (title == null) //Configuration (the default)
            {
                UpdateVisibility(true, false, false, false);
                divModalHeader.Visible = false;
                divModalBody.Visible = false;
                configState.Value = "ConfigSearch";
            }
            else if (title == "Book Courier Collection")
            {
                string saediFromId = Request.QueryString["SaediFromId"];
                string rmaId = Request.QueryString["RmaId"];
                string clientRef = Request.QueryString["ClientRef"];
                string onlineBookingURL;

                SiteSession session = SiteSessionFactory.LoadSession(this.Page);
                Shipmate shipmate = new Shipmate(session.Login.CreatedBy);

                CreateConsignmentRequest createConsignmentRequest = shipmate.GetCreateConsignmentRequest(saediFromId, rmaId, clientRef, out onlineBookingURL);

                txtParcelWeight.Text = "3000";
                txtParcelWidth.Text = "20";
                txtParcelLength.Text = "10";
                txtParcelDepth.Text = "15";
                txtConsignmentReference.Text = createConsignmentRequest.consignment_reference;
                txtParcelReference.Text = createConsignmentRequest.consignment_reference + "-1";
                txtServiceID.Text = createConsignmentRequest.ServiceID.ToString();
                txtServiceKey.Text = createConsignmentRequest.service_key;
                txtCollectionFromName.Text = createConsignmentRequest.collection_address.name;
                txtCollectionFromLine1.Text = createConsignmentRequest.collection_address.line_1;
                txtCollectionFromLine2.Text = createConsignmentRequest.collection_address.line_2;
                txtCollectionFromLine3.Text = createConsignmentRequest.collection_address.line_3;
                txtCollectionFromCompanyName.Text = createConsignmentRequest.collection_address.company_name;
                txtCollectionFromTelephone.Text = createConsignmentRequest.collection_address.telephone;
                txtCollectionFromEmailAddress.Text = createConsignmentRequest.collection_address.email_address;
                txtCollectionFromCity.Text = createConsignmentRequest.collection_address.city;
                txtCollectionFromPostcode.Text = createConsignmentRequest.collection_address.postcode;
                txtCollectionFromCountry.Text = createConsignmentRequest.collection_address.country;
                txtDeliveryToName.Text = createConsignmentRequest.to_address.name;
                txtDeliveryToLine1.Text = createConsignmentRequest.to_address.line_1;
                txtDeliveryToLine2.Text = createConsignmentRequest.to_address.line_2;
                txtDeliveryToLine3.Text = createConsignmentRequest.to_address.line_3;
                txtDeliveryToCompanyName.Text = createConsignmentRequest.to_address.company_name;
                txtDeliveryToTelephone.Text = createConsignmentRequest.to_address.telephone;
                txtDeliveryToEmailAddress.Text = createConsignmentRequest.to_address.email_address;
                txtDeliveryToCity.Text = createConsignmentRequest.to_address.city;
                txtDeliveryToPostcode.Text = createConsignmentRequest.to_address.postcode;
                txtDeliveryToCountry.Text = createConsignmentRequest.to_address.country;
                consignmentState.Value = "Create";
                SaediFromId.Value = saediFromId; //Hidden field
                ClientRef.Value = clientRef; //Hidden field
                OnlineBookingURL.Value = onlineBookingURL; //Hidden field

                UpdateVisibility(false, true, false, false);
            }
            else if (title == "Consignment details")
            {
                try
                {
                    lblTitle.Text = title;
                    SiteSession session = SiteSessionFactory.LoadSession(this.Page);
                    Shipmate shipmate = new Shipmate(session.Login.CreatedBy);
                    string trackingReference = Request.QueryString["TrackingReference"];
                    object shipmateConsignmentDetails = shipmate.GetShipmateConsignmentDetails(trackingReference);
                    ShipmateConsignmentRequestRepsonseDetails shipmateConsignmentRequestRepsonseDetails = shipmate.GetShipmateConsignmentRequestRepsonseDetails(shipmateConsignmentDetails);
                    Mobile.Portal.BLL.Shipmate.Address collectionFromAddress = shipmate.GetCollectionFromAddress(shipmateConsignmentDetails);
                    Mobile.Portal.BLL.Shipmate.Address deliveryToAddress = shipmate.GetDeliveryToAddress(shipmateConsignmentDetails);
                    SetLblText(shipmateConsignmentRequestRepsonseDetails, collectionFromAddress, deliveryToAddress);
                    consignmentState.Value = "Show details";
                    UpdateVisibility(false, true, false, false);
                }
                catch (Exception ex)
                {
                    spanError.InnerText = string.Format("An error occurred: {0}", ex.Message);
                    UpdateVisibility(false, false, false, true);
                }
            }
            else
            {
                spanError.InnerText = "Incorrect Title in the query string!";
                UpdateVisibility(false, false, false, true);
            }
        }

        private string CheckAdminPsw()
        {
            string adminPsw = Request.QueryString["AdminPsw"];

            if (string.IsNullOrEmpty(adminPsw))
            {
                return "AdminPsw is not given in the query string!";
            }
            else if (adminPsw != "Ping68pong")
            {
                return "AdminPsw is incorrect!";
            }
            else
            {
                return "";
            }
        }

        private void UpdateVisibility(bool showConfig, bool showDivConsignment, bool showDivNonPositiveIntegerError, bool showDivError)
        {
            divConfig.Visible = showConfig;
            divConsignment.Visible = showDivConsignment;
            divNonPositiveIntegerError.Visible = showDivNonPositiveIntegerError;
            divError.Visible = showDivError;
        }

        private void SetLblText(ShipmateConsignmentRequestRepsonseDetails details, Mobile.Portal.BLL.Shipmate.Address collectionFromAddress, Mobile.Portal.BLL.Shipmate.Address deliveryToAddress)
        {
            lblConsignmentReference.Text = details.ConsignmentReference;
            lblParcelReference.Text = details.ParcelReference;
            lblServiceID.Text = details.ServiceID;
            lblServiceKey.Text = details.ServiceKey;
            lblTrackingReference.Text = details.TrackingReference;
            lblLabelCreated.Text = details.LabelCreated;
            lblManifested.Text = details.Manifested;
            lblCollected.Text = details.Collected;
            lblInTransit.Text = details.InTransit;
            lblDelivered.Text = details.Delivered;
            lblDeliverFailed.Text = details.DeliveryFailed;
            lblCarrier.Text = details.Carrier;
            lblServiceName.Text = details.ServiceName;
            lblCreatedBy.Text = details.CreatedBy;
            lblCreatedWith.Text = details.CreatedWith;
            lblCreatedAt.Text = details.CreatedAt;
            lblParcelWeight.Text = details.ParcelWeight;
            lblParcelWidth.Text = details.ParcelWidth;
            lblParcelLength.Text = details.ParcelLength;
            lblParcelDepth.Text = details.ParcelDepth;
            lblCollectionFromName.Text = collectionFromAddress.name;
            lblCollectionFromLine1.Text = collectionFromAddress.line_1;
            lblCollectionFromLine2.Text = collectionFromAddress.line_2;
            lblCollectionFromLine3.Text = collectionFromAddress.line_3;
            lblCollectionFromCompanyName.Text = collectionFromAddress.company_name;
            lblCollectionFromTelephone.Text = collectionFromAddress.telephone;
            lblCollectionFromEmailAddress.Text = collectionFromAddress.email_address;
            lblCollectionFromCity.Text = collectionFromAddress.city;
            lblCollectionFromPostcode.Text = collectionFromAddress.postcode;
            lblCollectionFromCountry.Text = collectionFromAddress.country;
            lblDeliveryToName.Text = deliveryToAddress.name;
            lblDeliveryToLine1.Text = deliveryToAddress.line_1;
            lblDeliveryToLine2.Text = deliveryToAddress.line_2;
            lblDeliveryToLine3.Text = deliveryToAddress.line_3;
            lblDeliveryToCompanyName.Text = deliveryToAddress.company_name;
            lblDeliveryToTelephone.Text = deliveryToAddress.telephone;
            lblDeliveryToEmailAddress.Text = deliveryToAddress.email_address;
            lblDeliveryToCity.Text = deliveryToAddress.city;
            lblDeliveryToPostcode.Text = deliveryToAddress.postcode;
            lblDeliveryToCountry.Text = deliveryToAddress.country;
        }

        protected void btnCreateConsignment_Click(object sender, EventArgs e)
        {
            int n;
            string[] v = new string[] { txtParcelWeight.Text, txtParcelWidth.Text, txtParcelLength.Text, txtParcelDepth.Text };

            if (v.Any(x => !int.TryParse(x, out n) || (int.TryParse(x, out n) && n < 1)))
            {
                UpdateVisibility(false, true, true, false);
            }
            else
            {
                try
                {
                    SiteSession session;
                    int shipmateConsignmentCreationId;
                    Shipmate shipmate;
                    RMARefBLL rmaRefBLL = new RMARefBLL();
                    CourierRMABLL onlinemediaBLL = new CourierRMABLL();
                    CreateConsignmentRequest createConsignmentRequest;
                    int serviceID;
                    string consignmentReference, serviceKey;
                    Mobile.Portal.BLL.Shipmate.Address collectionAddress, toAddress;
                    List<Parcel> parcels;

                    session = SiteSessionFactory.LoadSession(this.Page);
                    shipmate = new Shipmate(session.Login.CreatedBy);
                    serviceID = int.Parse(txtServiceID.Text);
                    consignmentReference = txtConsignmentReference.Text;
                    serviceKey = txtServiceKey.Text;

                    collectionAddress = new Mobile.Portal.BLL.Shipmate.Address(
                        txtCollectionFromName.Text,
                        txtCollectionFromLine1.Text,
                        txtCollectionFromLine2.Text,
                        txtCollectionFromLine3.Text,
                        txtCollectionFromCompanyName.Text,
                        txtCollectionFromTelephone.Text,
                        txtCollectionFromEmailAddress.Text,
                        txtCollectionFromCity.Text,
                        txtCollectionFromPostcode.Text,
                        txtCollectionFromCountry.Text);

                    toAddress = new Mobile.Portal.BLL.Shipmate.Address(
                        txtDeliveryToName.Text,
                        txtDeliveryToLine1.Text,
                        txtDeliveryToLine2.Text,
                        txtDeliveryToLine3.Text,
                        txtDeliveryToCompanyName.Text,
                        txtDeliveryToTelephone.Text,
                        txtDeliveryToEmailAddress.Text,
                        txtDeliveryToCity.Text,
                        txtDeliveryToPostcode.Text,
                        txtDeliveryToCountry.Text);

                    parcels = new List<Parcel>();
                    parcels.Add(new Parcel(txtParcelReference.Text, int.Parse(txtParcelWeight.Text), int.Parse(txtParcelWidth.Text), int.Parse(txtParcelLength.Text), int.Parse(txtParcelDepth.Text)));

                    createConsignmentRequest = new CreateConsignmentRequest(serviceID, consignmentReference, null, serviceKey, collectionAddress, toAddress, parcels);

                    string trackingReference = shipmate.CreateConsignment(SaediFromId.Value, createConsignmentRequest, out shipmateConsignmentCreationId);

                    object shipmateConsignmentDetails = shipmate.GetShipmateConsignmentDetails(trackingReference);
                    ShipmateConsignmentRequestRepsonseDetails shipmateConsignmentRequestRepsonseDetails = shipmate.GetShipmateConsignmentRequestRepsonseDetails(shipmateConsignmentDetails);

                    Mobile.Portal.BLL.Shipmate.Address collectionFromAddress = shipmate.GetCollectionFromAddress(shipmateConsignmentDetails);
                    Mobile.Portal.BLL.Shipmate.Address deliveryToAddress = shipmate.GetDeliveryToAddress(shipmateConsignmentDetails);

                    rmaRefBLL.UpdateCollectionjob(txtConsignmentReference.Text, trackingReference, null, false, shipmateConsignmentCreationId);

                    int linkId = int.Parse(ClientRef.Value);

                    if (linkId != 0) //When linkId == 0 it is a PartStock and for those we don't need to call AddMediaMapping according to Paul/Vijay 19/06/2019
                    {
                        onlinemediaBLL.AddMediaMapping(OnlineBookingURL.Value, shipmateConsignmentRequestRepsonseDetails.MediaGUID, true, "pdf", "Courier Collection Label", linkId, (int)MediaTypeId.ServiceCall, (int)MediaContextId.General, SaediFromId.Value);
                    }

                    SetLblText(shipmateConsignmentRequestRepsonseDetails, collectionFromAddress, deliveryToAddress);
                    consignmentState.Value = "Create success";
                    UpdateVisibility(false, true, false, false);
                }
                catch (Exception ex)
                {
                    spanError.InnerText = string.Format("An error occurred: {0}", ex.Message);
                    UpdateVisibility(false, false, false, true);
                }
            }
        }

        private void ResetLabels()
        {
            lblMsgAdminPassword.Text = "";
            lblErrorMsgSearchClient.Text = "";
        }

        protected void btnSearchClient_Click(object sender, EventArgs e)
        {
            ResetLabels();

            if (!txtConfigAdminPassword.ReadOnly)
            {
                if (string.IsNullOrEmpty(txtConfigAdminPassword.Text))
                    lblMsgAdminPassword.Text = "Admin password is not given!";
                else if (txtConfigAdminPassword.Text != "Ping68pong")
                    lblMsgAdminPassword.Text = "Admin password is incorrect!";
                else
                {
                    txtConfigAdminPassword.TextMode = TextBoxMode.SingleLine;
                    txtConfigAdminPassword.Text = "..........";
                    txtConfigAdminPassword.ReadOnly = true;
                    lblMsgAdminPassword.Text = "Correct password";
                    lblMsgAdminPassword.ForeColor = System.Drawing.Color.Green;
                }
            }
            else
            {
                lblMsgAdminPassword.Text = "Correct password";
            }

            if (string.IsNullOrEmpty(txtConfigClientId.Text))
                lblErrorMsgSearchClient.Text = "ClientId is not given!";

            if (lblMsgAdminPassword.Text == "Correct password" && string.IsNullOrEmpty(lblErrorMsgSearchClient.Text))
            {
                string message;
                Shipmate shipmate = new Shipmate();
                ShipmateConfig shipmateConfig = shipmate.GetConfig(txtConfigClientId.Text, out message);

                if (message == "ClientId does not exist!")
                    lblErrorMsgSearchClient.Text = "ClientId does not exist!";
                else
                {
                    configState.Value = "ConfigEditAvailable";

                    if (message != "Config is empty")
                    {
                        txtConfigShipmateUsername.Text = shipmateConfig.ShipmateUsername;
                        txtConfigShipmatePassword.Text = shipmateConfig.ShipmatePassword;
                        txtConfigShipmateToken.Text = shipmateConfig.ShipmateToken;
                        txtConfigShipmateServiceKey.Text = shipmateConfig.ShipmateServiceKey;
                        txtConfigShipmateBaseUrl.Text = shipmateConfig.ShipmateBaseUrl;
                        txtConfigCarrierTrackAndTraceUrl.Text = shipmateConfig.CarrierTrackAndTraceUrl;
                        txtConfigDeliveryToName.Text = shipmateConfig.DeliveryToName;
                        txtConfigDeliveryToLine1.Text = shipmateConfig.DeliveryToLine1;
                        txtConfigDeliveryToLine2.Text = shipmateConfig.DeliveryToLine2;
                        txtConfigDeliveryToLine3.Text = shipmateConfig.DeliveryToLine3;
                        txtConfigDeliveryToCompanyName.Text = shipmateConfig.DeliveryToCompanyName;
                        txtConfigDeliveryToTelephone.Text = shipmateConfig.DeliveryToTelephone;
                        txtConfigDeliveryToEmail.Text = shipmateConfig.DeliveryToEmail;
                        txtConfigDeliveryToCity.Text = shipmateConfig.DeliveryToCity;
                        txtConfigDeliveryToPostcode.Text = shipmateConfig.DeliveryToPostcode;
                        txtConfigDeliveryToCountry.Text = shipmateConfig.DeliveryToCountry;
                    }
                }
            }
        }

        protected void btnEditConfig_Click(object sender, EventArgs e)
        {
            configState.Value = "ConfigEdit";
        }

        protected void btnSaveConfig_Click(object sender, EventArgs e)
        {
            Shipmate shipmate = new Shipmate();

            shipmate.SetConfig(txtConfigClientId.Text,
                txtConfigShipmateUsername.Text, 
                txtConfigShipmatePassword.Text,
                txtConfigShipmateToken.Text,
                txtConfigShipmateServiceKey.Text,
                txtConfigShipmateBaseUrl.Text,
                txtConfigCarrierTrackAndTraceUrl.Text,
                txtConfigDeliveryToName.Text,
                txtConfigDeliveryToLine1.Text,
                txtConfigDeliveryToLine2.Text,
                txtConfigDeliveryToLine3.Text,
                txtConfigDeliveryToCompanyName.Text,
                txtConfigDeliveryToTelephone.Text,
                txtConfigDeliveryToEmail.Text,
                txtConfigDeliveryToCity.Text,
                txtConfigDeliveryToPostcode.Text,
                txtConfigDeliveryToCountry.Text);

            configState.Value = "ConfigEditAvailable";
        }

        protected void btnNewSearchClient_Click(object sender, EventArgs e)
        {
            configState.Value = "ConfigSearch";
        }
    }
}