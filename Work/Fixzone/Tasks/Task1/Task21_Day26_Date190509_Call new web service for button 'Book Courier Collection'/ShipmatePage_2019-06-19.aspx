<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShipmatePage.aspx.cs" Inherits="MobilePortal.ShipmatePage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Shipmate</title>
    <link rel="stylesheet" type="text/css" href="bootstrap/css/bootstrap.min.css" />
    <link rel="stylesheet" type="text/css" href="bootstrap/css/bootstrap-responsive.min.css" />
    <link rel="stylesheet" type="text/css" href="colors-engineer-connect.css" />
    <link rel="stylesheet" type="text/css" href="default-layout.css" id="defaultsheet" />
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
</head>
<body style="padding-bottom: 1px !important; max-height: 2000px !important;">
    <div class="modal-header grid-header-color">
        <asp:Label ID="lblTitle" runat="server" ></asp:Label>
    </div>

    <div class="modal-body" style="padding-bottom: 1px !important; max-height: 2000px !important;">
        <div id="divCreateConsignmenmt" runat="server">
            <form id="formCreateConsignmenmt" runat="server">
                <div id="divNonPositiveIntegerError" style="color: red !important; margin-top: 5px !important; margin-bottom: 10px !important;" runat="server">
                    <strong>Error! Weight, width, length and depth should all be positive integers!</strong>
                </div>
                <table style="width: 70%;">
                    <tr>
                        <td colspan="2"><strong>Data to be sent to Shipmate in the request.</strong></td>
                    </tr>
                     <tr>
                        <td><strong>Please adjust weight, width, length and depth.</strong></td>
                        <td style="text-align: right;">
                          <asp:Button ID="Button1" runat="server" Text="Create" OnClick="btnCreateConsignment_Click" />&nbsp;&nbsp;&nbsp;
                          <button onclick="window.parent.document.getElementById('btnbookingCancelWindow').click();">Cancel</button>
                        </td>
                    </tr>
                    <tr><td colspan="2">&nbsp;&nbsp;&nbsp;</td></tr>
                    <tr class="grid-header-color">
                        <td class="grid-column-label" style="padding: -7px;" colspan="2">Editable data</td>
                    </tr>
                    <tr class="grid-alt-row-color">
                        <td class="grid-column-label" style="padding: -7px;">Weight (in grammes)</td>
                        <td class="grid-column-data" style="padding: -7px; text-align: right;">
                            <asp:TextBox ID="txtWeight" runat="server" ReadOnly="false" CssClass=""></asp:TextBox></td>
                    </tr>
                    <tr class="grid-row-color">
                        <td class="grid-column-label" style="padding: -7px;">Width (in centimetres)</td>
                        <td class="grid-column-data" style="padding: -7px; text-align: right;">
                            <asp:TextBox ID="txtWidth" runat="server" ReadOnly="false" CssClass=""></asp:TextBox></td>
                    </tr>
                    <tr class="grid-alt-row-color">
                        <td class="grid-column-label" style="padding: -7px;">Length (in centimetres)</td>
                        <td class="grid-column-data" style="padding: -7px; text-align: right;">
                            <asp:TextBox ID="txtLength" runat="server" ReadOnly="false" CssClass=""></asp:TextBox></td>
                    </tr>
                    <tr class="grid-row-color">
                        <td class="grid-column-label" style="padding: -7px;">Depth (in centimetres)</td>
                        <td class="grid-column-data" style="padding: -7px; text-align: right;">
                            <asp:TextBox ID="txtDepth" runat="server" ReadOnly="false" CssClass=""></asp:TextBox></td>
                    </tr>
                    <tr class="grid-header-color">
                        <td class="grid-column-label" style="padding: -7px;" colspan="2">Non editable data</td>
                    </tr>
                    <tr class="grid-row-color">
                        <td class="grid-column-label" style="padding: -7px;">ServiceID</td>
                        <td class="grid-column-data" style="padding: -7px; text-align: right;">
                            <asp:TextBox ID="txtServiceID" runat="server" ReadOnly="true" CssClass=""></asp:TextBox></td>
                    </tr>
                    <tr class="grid-alt-row-color">
                        <td class="grid-column-label" style="padding: -7px;">RemittanceID</td>
                        <td class="grid-column-data" style="padding: -7px; text-align: right;">
                            <asp:TextBox ID="txtRemittanceID" runat="server" ReadOnly="true" CssClass=""></asp:TextBox></td>
                    </tr>
                    <tr class="grid-row-color">
                        <td class="grid-column-label" style="padding: -7px;">Consignment reference</td>
                        <td class="grid-column-data" style="padding: -7px; text-align: right;">
                            <asp:TextBox ID="txtConsignmentReference" runat="server" ReadOnly="true" CssClass=""></asp:TextBox></td>
                    </tr>
                    <tr class="grid-alt-row-color">
                        <td class="grid-column-label" style="padding: -7px;">Service key</td>
                        <td class="grid-column-data" style="padding: -7px; text-align: right;">
                            <asp:TextBox ID="txtServiceKey" runat="server" ReadOnly="true" CssClass=""></asp:TextBox></td>
                    </tr>
                    <tr class="grid-row-color">
                        <td class="grid-column-label" style="padding: -7px;">Name</td>
                        <td class="grid-column-data" style="padding: -7px; text-align: right;">
                            <asp:TextBox ID="txtName" runat="server" ReadOnly="true" CssClass=""></asp:TextBox></td>
                    </tr>
                    <tr class="grid-alt-row-color">
                        <td class="grid-column-label" style="padding: -7px;">Line 1</td>
                        <td class="grid-column-data" style="padding: -7px; text-align: right;">
                            <asp:TextBox ID="txtLine1" runat="server" ReadOnly="true" CssClass=""></asp:TextBox></td>
                    </tr>
                    <tr class="grid-row-color">
                        <td class="grid-column-label" style="padding: -7px;">City</td>
                        <td class="grid-column-data" style="padding: -7px; text-align: right;">
                            <asp:TextBox ID="txtCity" runat="server" ReadOnly="true" CssClass=""></asp:TextBox></td>
                    </tr>
                    <tr class="grid-alt-row-color">
                        <td class="grid-column-label" style="padding: -7px;">Postcode</td>
                        <td class="grid-column-data" style="padding: -7px; text-align: right;">
                            <asp:TextBox ID="txtPostcode" runat="server" ReadOnly="true" CssClass=""></asp:TextBox></td>
                    </tr>
                    <tr class="grid-row-color">
                        <td class="grid-column-label" style="padding: -7px;">Country</td>
                        <td class="grid-column-data" style="padding: -7px; text-align: right;">
                            <asp:TextBox ID="txtCountry" runat="server" ReadOnly="true" CssClass=""></asp:TextBox></td>
                    </tr>
                    <tr class="grid-alt-row-color">
                        <td class="grid-column-label" style="padding: -7px;">Reference</td>
                        <td class="grid-column-data" style="padding: -7px; text-align: right;">
                            <asp:TextBox ID="txtReference" runat="server" ReadOnly="true" CssClass=""></asp:TextBox></td>
                    </tr>
                </table>
                <asp:HiddenField ID="SaediFromId" runat="server" />
                <asp:HiddenField ID="ClientRefAsInt" runat="server" />
                <asp:HiddenField ID="OnlineBookingURL" runat="server" />
            </form>
        </div>
        <div id="divConsignmentDetails" runat="server">
            <table style="width: 65%;">
                <tr class="grid-alt-row-color">
                    <td class="grid-column-label" style="padding: 5px;">Consignment reference</td>
                    <td class="grid-column-data" style="padding: 5px;">
                        <asp:Label ID="lblConsignmentReference" runat="server"></asp:Label></td>
                </tr>
                <tr class="grid-row-color">
                    <td class="grid-column-label" style="padding: 5px;">Parcel reference</td>
                    <td class="grid-column-data" style="padding: 5px;">
                        <asp:Label ID="lblParcelReference" runat="server"></asp:Label></td>
                </tr>
                <tr class="grid-alt-row-color">
                    <td class="grid-column-label" style="padding: 5px;">Carrier</td>
                    <td class="grid-column-data" style="padding: 5px;">
                        <asp:Label ID="lblCarrier" runat="server"></asp:Label></td>
                </tr>
                <tr class="grid-row-color">
                    <td class="grid-column-label" style="padding: 5px;">Service name</td>
                    <td class="grid-column-data" style="padding: 5px;">
                        <asp:Label ID="lblServiceName" runat="server"></asp:Label></td>
                </tr>
                <tr class="grid-alt-row-color">
                    <td class="grid-column-label" style="padding: 5px;">Tracking reference</td>
                    <td class="grid-column-data" style="padding: 5px;">
                        <asp:Label ID="lblTrackingReference" runat="server"></asp:Label></td>
                </tr>
                <tr class="grid-row-color">
                    <td class="grid-column-label" style="padding: 5px;">Created by</td>
                    <td class="grid-column-data" style="padding: 5px;">
                        <asp:Label ID="lblCreatedBy" runat="server"></asp:Label></td>
                </tr>
                <tr class="grid-alt-row-color">
                    <td class="grid-column-label" style="padding: 5px;">Created with</td>
                    <td class="grid-column-data" style="padding: 5px;">
                        <asp:Label ID="lblCreatedWith" runat="server"></asp:Label></td>
                </tr>
                <tr class="grid-row-color">
                    <td class="grid-column-label" style="padding: 5px;">Created at</td>
                    <td class="grid-column-data" style="padding: 5px;">
                        <asp:Label ID="lblCreatedAt" runat="server"></asp:Label></td>
                </tr>
                <tr class="grid-alt-row-color">
                    <td class="grid-column-label" style="padding: 5px;">Delivery name</td>
                    <td class="grid-column-data" style="padding: 5px;">
                        <asp:Label ID="lblDeliveryName" runat="server"></asp:Label></td>
                </tr>
                <tr class="grid-row-color">
                    <td class="grid-column-label" style="padding: 5px;">Line</td>
                    <td class="grid-column-data" style="padding: 5px;">
                        <asp:Label ID="lblLine" runat="server"></asp:Label></td>
                </tr>
                <tr class="grid-alt-row-color">
                    <td class="grid-column-label" style="padding: 5px;">City</td>
                    <td class="grid-column-data" style="padding: 5px;">
                        <asp:Label ID="lblCity" runat="server"></asp:Label></td>
                </tr>
                <tr class="grid-row-color">
                    <td class="grid-column-label" style="padding: 5px;">Postcode</td>
                    <td class="grid-column-data" style="padding: 5px;">
                        <asp:Label ID="lblPostcode" runat="server"></asp:Label></td>
                </tr>
                <tr class="grid-alt-row-color">
                    <td class="grid-column-label" style="padding: 5px;">Country</td>
                    <td class="grid-column-data" style="padding: 5px;">
                        <asp:Label ID="lblCountry" runat="server"></asp:Label></td>
                </tr>
                <tr><td colspan="2">&nbsp;&nbsp;&nbsp;</td></tr>
                <tr><td colspan="2"><button onclick="window.parent.document.getElementById('btnbookingCancelWindow').click();">Ok</button></td></tr>
            </table>
        </div>
        <div id="divError" runat="server">
            <span id= "spanError" style="color: red !important; font-weight: bolder !important; margin-top: 10px !important;" runat="server"></span><br /><br />
            <button onclick="window.parent.document.getElementById('btnbookingCancelWindow').click();">Ok</button>
        </div>
    </div>
</body>
</html>
