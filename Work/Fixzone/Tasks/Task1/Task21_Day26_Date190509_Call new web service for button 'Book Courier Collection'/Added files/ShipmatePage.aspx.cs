using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mobile.Portal.BLL.Shipmate;
using Mobile.Portal.BLL;
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
            lblTitle.Text = title;

            if (title == "Create Shipmate consignment")
            {
                txtServiceID.Text = txtServiceID.ToolTip = Request.QueryString["ServiceID"];
                txtRemittanceID.Text = txtRemittanceID.ToolTip = Request.QueryString["RemittanceID"];
                txtConsignmentReference.Text = txtConsignmentReference.ToolTip = Request.QueryString["ConsignmentReference"];
                txtServiceKey.Text = txtServiceKey.ToolTip = Request.QueryString["ServiceKey"];
                txtName.Text = txtName.ToolTip = Request.QueryString["Name"];
                txtLine1.Text = txtLine1.ToolTip = Request.QueryString["Line1"];
                txtCity.Text = txtCity.ToolTip = Request.QueryString["City"];
                txtPostcode.Text = txtPostcode.ToolTip = Request.QueryString["Postcode"];
                txtCountry.Text = txtCountry.ToolTip = Request.QueryString["Country"];
                txtReference.Text = txtReference.ToolTip = Request.QueryString["Reference"];
                txtWeight.Text = txtWeight.ToolTip = "3000";
                txtWidth.Text = txtWidth.ToolTip = "20";
                txtLength.Text = txtLength.ToolTip = "10";
                txtDepth.Text = txtDepth.ToolTip = "15";
                SaediFromId.Value = Request.QueryString["SaediFromId"]; //Hidden field
                UpdateVisibility(true, false, false, false);
            }
            else if (title == "Consignment details")
            {
                try
                {
                    string trackingReference = Request.QueryString["TrackingReference"];
                    SiteSession session = SiteSessionFactory.LoadSession(this.Page);
                    Shipmate shipmate = new Shipmate(session.Login.CreatedBy);
                    ConsignmentResponse consignmentResponse = shipmate.GetLabel(trackingReference);
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
            else if (title == "SetConfig") //ShipmatePage.aspx?Title=SetConfig&AdminPsw=Ping68pong&ClientId=??????????&UserName=??????????&Password=??????????&Token=??????????&ServiceKey=??????????&BaseUrl=??????????
            {
                string errorMessage, clientId = "", userName = "", password = "", token = "", serviceKey = "", baseUrl = "";

                errorMessage = CheckAdminPsw();

                if (errorMessage == "")
                {
                    clientId = Request.QueryString["ClientId"];
                    userName = Request.QueryString["UserName"];
                    password = Request.QueryString["Password"];
                    token = Request.QueryString["Token"];
                    serviceKey = Request.QueryString["ServiceKey"];
                    baseUrl = Request.QueryString["BaseUrl"];

                    if (string.IsNullOrEmpty(clientId))
                        errorMessage = "ClientId is not given in the query string!";
                    else if (string.IsNullOrEmpty(userName))
                        errorMessage = "UserName is not given in the query string!";
                    else if (string.IsNullOrEmpty(password))
                        errorMessage = "Password is not given in the query string!";
                    else if (string.IsNullOrEmpty(token))
                        errorMessage = "Token is not given in the query string!";
                    else if (string.IsNullOrEmpty(serviceKey))
                        errorMessage = "ServiceKey is not given in the query string!";
                    else if (string.IsNullOrEmpty(baseUrl))
                        errorMessage = "BaseUrl is not given in the query string!";
                }

                if (errorMessage != "")
                {
                    ReturnHtml("Error", string.Format("<span style='color: red; font-weight: bold;'>{0}</span>", errorMessage));
                }
                else
                {
                    Shipmate shipmate = new Shipmate();
                    string result = shipmate.SetConfig(clientId, userName, password, token, serviceKey, baseUrl);

                    if (result.StartsWith("Error"))
                    {
                        ReturnHtml("Error", string.Format("<span style='color: red; font-weight: bold;'>{0}</span>", errorMessage));
                    }
                    else
                    {
                        ReturnHtml("SetConfig", string.Format("<span style='color: green; font-weight: bold;'>Shipmate configuration was successfully updated for {0}</span>", clientId));
                    }
                }
            }
            else if (title == "GetConfig") //ShipmatePage.aspx?Title=GetConfig&AdminPsw=Ping68pong&ClientId=??????????
            {
                string clientId = "";
                string errorMessage = CheckAdminPsw();
                ShipmateConfig shipmateConfig = null;

                if (errorMessage == "")
                {
                    clientId = Request.QueryString["ClientId"];

                    if (string.IsNullOrEmpty(clientId))
                        errorMessage = "ClientId is not given in the query string!";
                    else
                    {
                        Shipmate shipmate = new Shipmate();
                        shipmateConfig = shipmate.GetConfig(clientId, out errorMessage);
                    }
                }

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    ReturnHtml("Error", string.Format("<span style='color: red; font-weight: bold;'>{0}</span>", errorMessage));
                }
                else
                {
                    StringBuilder sb = new StringBuilder(string.Format("<div style='margin-top: 15px; margin-left: 15px;'><h2>Shipmate configuration for {0}</h2><table style='border-collapse: collapse;'>", clientId));

                    sb.Append("<tr><td style='border: 1px solid black'><strong>UserName</strong><td style='border: 1px solid black'>" + shipmateConfig.UserName + "</td></tr>");
                    sb.Append("<tr><td style='border: 1px solid black'><strong>Password</strong><td style='border: 1px solid black'>" + shipmateConfig.Password + "</td></tr>");
                    sb.Append("<tr><td style='border: 1px solid black'><strong>Token</strong><td style='border: 1px solid black'>" + shipmateConfig.Token + "</td></tr>");
                    sb.Append("<tr><td style='border: 1px solid black'><strong>ServiceKey</strong><td style='border: 1px solid black'>" + shipmateConfig.ServiceKey + "</td></tr>");
                    sb.Append("<tr><td style='border: 1px solid black'><strong>BaseUrl</strong><td style='border: 1px solid black'>" + shipmateConfig.BaseUrl + "</td></tr></table></div>");

                    ReturnHtml("GetConfig", sb.ToString());
                }
            }
            else
            {
                ReturnHtml("Error", "<span style='color: red; font-weight: bold;'>Incorrect Title in the query string!</span>");
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

        private void ReturnHtml(string title, string body)
        {
            string template = "<!DOCTYPE html> <html lang='en' xmlns='http://www.w3.org/1999/xhtml'><head><title>{0}</title><body>{1}</body></html>";
            string html = string.Format(template, title, body);
            Response.ContentType = "text/html";
            Response.BinaryWrite(Encoding.UTF8.GetBytes(html.ToCharArray()));
            Response.Flush();
            Response.Close();
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
            Shipmate shipmate;
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
                    SiteSession session = SiteSessionFactory.LoadSession(this.Page);
                    shipmate = new Shipmate(session.Login.CreatedBy);
                    consignmentRequest = new ConsignmentRequest(int.Parse(txtServiceID.Text), int.Parse(txtRemittanceID.Text), txtConsignmentReference.Text, shipmate.Token, txtServiceKey.Text, toAddress, parcels);
                    consignmentResponse = shipmate.CreateConsignment(SaediFromId.Value, consignmentRequest, out shipmateConsignmentCreationId);
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