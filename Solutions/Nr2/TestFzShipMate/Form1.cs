using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mobile.Portal.BLL.FzShipMate;

namespace TestFzShipMate
{
    public enum FzShipMateAction
    {
        Login,
        Services,
        CreateConsignment,
        TrackingByConsignment,
        TrackingByParcels,
        CancelConsignment,
        Label,
        PrintLabel
    }


    public partial class Form1 : Form
    {
        private FzShipMateAction _fzShipMateAction;

        public Form1()
        {
            InitializeComponent();
        }

        private void HandleTextBoxes(
            string ServiceID,
            string RemittanceID,
            string consignment_reference,
            string Token,
            string service_key,
            string name,
            string line_1,
            string city,
            string postcode,
            string country,
            string reference,
            string weight,
            string width,
            string length,
            string depth,
            string Tracking_reference
            )
        {
            if (string.IsNullOrEmpty(ServiceID))
            {
                this.txt_ServiceID.Clear();
                this.txt_ServiceID.Enabled = false;
            }
            else
            {
                this.txt_ServiceID.Text = ServiceID;
                this.txt_ServiceID.Enabled = true;
            }

            if (string.IsNullOrEmpty(RemittanceID))
            {
                this.txt_RemittanceID.Clear();
                this.txt_RemittanceID.Enabled = false;
            }
            else
            {
                this.txt_RemittanceID.Text = RemittanceID;
                this.txt_RemittanceID.Enabled = true;
            }

            if (string.IsNullOrEmpty(consignment_reference))
            {
                this.txt_consignment_reference.Clear();
                this.txt_consignment_reference.Enabled = false;
            }
            else
            {
                this.txt_consignment_reference.Text = consignment_reference;
                this.txt_consignment_reference.Enabled = true;
            }

            if (string.IsNullOrEmpty(Token))
            {
                this.txt_Token.Clear();
                this.txt_Token.Enabled = false;
            }
            else
            {
                this.txt_Token.Text = Token;
                this.txt_Token.Enabled = true;
            }

            if (string.IsNullOrEmpty(service_key))
            {
                this.txt_service_key.Clear();
                this.txt_service_key.Enabled = false;
            }
            else
            {
                this.txt_service_key.Text = service_key;
                this.txt_service_key.Enabled = true;
            }

            if (string.IsNullOrEmpty(name))
            {
                this.txt_name.Clear();
                this.txt_name.Enabled = false;
            }
            else
            {
                this.txt_name.Text = name;
                this.txt_name.Enabled = true;
            }

            if (string.IsNullOrEmpty(line_1))
            {
                this.txt_line_1.Clear();
                this.txt_line_1.Enabled = false;
            }
            else
            {
                this.txt_line_1.Text = line_1;
                this.txt_line_1.Enabled = true;
            }

            if (string.IsNullOrEmpty(city))
            {
                this.txt_city.Clear();
                this.txt_city.Enabled = false;
            }
            else
            {
                this.txt_city.Text = city;
                this.txt_city.Enabled = true;
            }

            if (string.IsNullOrEmpty(postcode))
            {
                this.txt_postcode.Clear();
                this.txt_postcode.Enabled = false;
            }
            else
            {
                this.txt_postcode.Text = postcode;
                this.txt_postcode.Enabled = true;
            }

            if (string.IsNullOrEmpty(country))
            {
                this.txt_country.Clear();
                this.txt_country.Enabled = false;
            }
            else
            {
                this.txt_country.Text = country;
                this.txt_country.Enabled = true;
            }

            if (string.IsNullOrEmpty(reference))
            {
                this.txt_reference.Clear();
                this.txt_reference.Enabled = false;
            }
            else
            {
                this.txt_reference.Text = reference;
                this.txt_reference.Enabled = true;
            }

            if (string.IsNullOrEmpty(weight))
            {
                this.txt_weight.Clear();
                this.txt_weight.Enabled = false;
            }
            else
            {
                this.txt_weight.Text = weight;
                this.txt_weight.Enabled = true;
            }

            if (string.IsNullOrEmpty(width))
            {
                this.txt_width.Clear();
                this.txt_width.Enabled = false;
            }
            else
            {
                this.txt_width.Text = width;
                this.txt_width.Enabled = true;
            }

            if (string.IsNullOrEmpty(length))
            {
                this.txt_length.Clear();
                this.txt_length.Enabled = false;
            }
            else
            {
                this.txt_length.Text = length;
                this.txt_length.Enabled = true;
            }

            if (string.IsNullOrEmpty(depth))
            {
                this.txt_depth.Clear();
                this.txt_depth.Enabled = false;
            }
            else
            {
                this.txt_depth.Text = depth;
                this.txt_depth.Enabled = true;
            }

            if (string.IsNullOrEmpty(Tracking_reference))
            {
                this.txt_Tracking_reference.Clear();
                this.txt_Tracking_reference.Enabled = false;
            }
            else
            {
                this.txt_Tracking_reference.Text = Tracking_reference;
                this.txt_Tracking_reference.Enabled = true;
            }

        }

        private void RadioButtonCheckedChanged(object sender, EventArgs e)
        {
            this.txtResponse.Clear();

            if (((RadioButton)sender).Checked)
            {
                string name = ((RadioButton)sender).Name;

                switch (name)
                {
                    case "Login":
                        HandleTextBoxes("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
                        _fzShipMateAction = FzShipMateAction.Login;
                        break;
                    case "Services":
                        HandleTextBoxes("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
                        _fzShipMateAction = FzShipMateAction.Services;
                        break;
                    case "CreateConsignment":
                        HandleTextBoxes("123456", "23456", "80000883", "628af98fb045bdb636452204fde483d6", "DPDNEXT", "David Xu", "35 Ford Street", "Derby", "DE1 1EE", "GB", "80000883-1", "3000", "20", "10", "15", "");
                        _fzShipMateAction = FzShipMateAction.CreateConsignment;
                        break;
                    case "TrackingByConsignment":
                        HandleTextBoxes("", "", "80000885", "628af98fb045bdb636452204fde483d6", "", "", "", "", "", "", "", "", "", "", "", "");
                        _fzShipMateAction = FzShipMateAction.TrackingByConsignment;
                        break;
                    case "TrackingByParcels":
                        HandleTextBoxes("", "", "", "628af98fb045bdb636452204fde483d6", "", "", "", "", "", "", "", "", "", "", "", "15553245200014");
                        _fzShipMateAction = FzShipMateAction.TrackingByParcels;
                        break;
                    case "CancelConsignment":
                        HandleTextBoxes("", "", "", "628af98fb045bdb636452204fde483d6", "", "", "", "", "", "", "80000883", "", "", "", "", "");
                        _fzShipMateAction = FzShipMateAction.CancelConsignment;
                        break;
                    case "Label":
                        HandleTextBoxes("", "", "", "628af98fb045bdb636452204fde483d6", "", "", "", "", "", "", "", "", "", "", "", "15553245200011");
                        _fzShipMateAction = FzShipMateAction.Label;
                        break;
                    case "PrintLabel":
                        HandleTextBoxes("", "", "", "628af98fb045bdb636452204fde483d6", "", "", "", "", "", "", "", "", "", "", "", "15553245200014");
                        _fzShipMateAction = FzShipMateAction.PrintLabel;
                        break;
                    default:
                        throw new Exception("Can not find " + name);
                }
            }
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            try
            {
                ConsignmentResponse consignmentResponse;

                FzShipMate fzShipMate = new FzShipMate();

                switch (_fzShipMateAction)
                {
                    case FzShipMateAction.Login:
                        LoginResponse loginResponse = fzShipMate.Login(new LoginRequest(fzShipMate.UserName, fzShipMate.Password));
                        this.txtResponse.Text = loginResponse.ToString();
                        break;
                    case FzShipMateAction.Services:
                        ServicesResponse servicesResponse = fzShipMate.GetServices();
                        this.txtResponse.Text = servicesResponse.ToString();
                        break;
                    case FzShipMateAction.CreateConsignment:
                        List <Parcel> list = new List<Parcel>();
                        Parcel parcel = new Parcel(this.txt_reference.Text, int.Parse(this.txt_weight.Text), int.Parse(this.txt_width.Text), int.Parse(this.txt_length.Text), int.Parse(this.txt_depth.Text));
                        list.Add(parcel);
                        consignmentResponse = fzShipMate.CreateConsignment(new ConsignmentRequest(int.Parse(this.txt_ServiceID.Text), int.Parse(this.txt_RemittanceID.Text), this.txt_consignment_reference.Text, this.txt_Token.Text, this.txt_service_key.Text, new ToAddressRequest(this.txt_name.Text, this.txt_line_1.Text, this.txt_city.Text, this.txt_postcode.Text, this.txt_country.Text), list));
                        this.txtResponse.Text = consignmentResponse.ToString();
                        break;
                    case FzShipMateAction.TrackingByConsignment:
                        TrackingConsignmentResponse trackingConsignmentResponse = fzShipMate.TrackingByConsignment(this.txt_consignment_reference.Text);
                        this.txtResponse.Text = trackingConsignmentResponse.ToString();
                        break;
                    case FzShipMateAction.TrackingByParcels:
                        TrackingByParcelsResponse trackingByParcelsResponse = fzShipMate.TrackingByParcels(this.txt_Tracking_reference.Text);
                        this.txtResponse.Text = trackingByParcelsResponse.ToString();
                        break;
                    case FzShipMateAction.CancelConsignment:
                        CancelConsignmentResponse cancelConsignmentResponse = fzShipMate.CancelConsignments(this.txt_consignment_reference.Text);
                        this.txtResponse.Text = cancelConsignmentResponse.ToString();
                        break;
                    case FzShipMateAction.Label:
                        consignmentResponse = fzShipMate.GetLabel(this.txt_Tracking_reference.Text);
                        this.txtResponse.Text = consignmentResponse.ToString();
                        break;
                    case FzShipMateAction.PrintLabel:
                        consignmentResponse = fzShipMate.PrintLabel(this.txt_Tracking_reference.Text);
                        this.txtResponse.Text = consignmentResponse.ToString();
                        break;
                }
            }
            catch(Exception ex)
            {
                this.txtResponse.Text = "An error happened: " + ex.Message;
            }
        }
    }
}
