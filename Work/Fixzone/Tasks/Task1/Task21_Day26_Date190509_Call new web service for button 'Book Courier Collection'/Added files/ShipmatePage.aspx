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

    <style>
        .ShipmateConfigFullWidth {
            width: 97% !important;
        }

       .ShipmateConsignmentFullWidth {
            width: 95% !important;
        }

        .ShipmateConfigMargin {
            margin-bottom: 10px !important;
        }

        tr.grid-row-color, tr.grid-alt-row-color, tr.grid-header-color {
            border-right: 1px solid black !important;
            border-left: 1px solid black !important;
        }
    </style>
</head>
<body style="padding: 10px !important; max-height: 2000px !important;">
    <div id="divConfig" runat="server">
        <form id="formConfig" runat="server">
            <asp:HiddenField ID="configState" runat="server" />
            <table style="margin-left: 50px !important; margin-top: 10px !important; margin-right: 50px !important; width: 40% !important;">
                <tr class="grid-row-color" style="border-right: 1px solid white !important; border-left: 1px solid white !important;">
                    <td class="grid-column-label" style="width: 25% !important;">&nbsp;Admin password</td>
                    <td class="grid-column-data" style="text-align: left !important; width: 75% !important;">
                        <asp:TextBox ID="txtConfigAdminPassword" TextMode="Password" runat="server"></asp:TextBox>&nbsp;&nbsp;
                        <asp:Label ID="lblMsgAdminPassword" Font-Bold="true" ForeColor="Red" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">&nbsp;&nbsp;&nbsp;</td>
                </tr>
                <tr>
                    <td colspan="2">&nbsp;&nbsp;&nbsp;</td>
                </tr>
                <tr class="grid-row-color" style="border-right: 1px solid white !important; border-left: 1px solid white !important;">
                    <td class="grid-column-label" style="width: 25% !important;">&nbsp;ClientId</td>
                    <td class="grid-column-data" style="width: 75% !important;">
                        <asp:TextBox ID="txtConfigClientId" runat="server"></asp:TextBox>&nbsp;&nbsp;
                        <asp:Button ID="btnSearchClient" runat="server" Text="Search" Font-Bold="true" CssClass="ShipmateConfigMargin" OnClick="btnSearchClient_Click"/>&nbsp;&nbsp;
                        <asp:Button ID="btnNewSearchClient" runat="server" Text="New" Font-Bold="true" CssClass="ShipmateConfigMargin" OnClick="btnNewSearchClient_Click"/>&nbsp;&nbsp;
                        <asp:Label ID="lblErrorMsgSearchClient" Font-Bold="true" ForeColor="Red" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">&nbsp;&nbsp;&nbsp;</td>
                </tr>
                <tr class="grid-alt-row-color" style="border-top: 1px solid black !important;">
                    <td class="grid-column-label" style="width: 25% !important;">&nbsp;Shipmate username</td>
                    <td class="grid-column-data" style="width: 75% !important;">
                        <asp:TextBox ID="txtConfigShipmateUsername" CssClass="ShipmateConfigFullWidth" runat="server"></asp:TextBox></td>
                </tr>
                <tr class="grid-row-color">
                    <td class="grid-column-label" style="width: 25% !important;">&nbsp;Shipmate password</td>
                    <td class="grid-column-data" style="width: 75% !important;">
                        <asp:TextBox ID="txtConfigShipmatePassword" CssClass="ShipmateConfigFullWidth" runat="server"></asp:TextBox></td>
                </tr>
                <tr class="grid-alt-row-color">
                    <td class="grid-column-label" style="width: 25% !important;">&nbsp;Shipmate token</td>
                    <td class="grid-column-data" style="width: 75% !important;">
                        <asp:TextBox ID="txtConfigShipmateToken" CssClass="ShipmateConfigFullWidth" runat="server"></asp:TextBox></td>
                    </tr>
                <tr class="grid-row-color">
                    <td class="grid-column-label" style="width: 25% !important;">&nbsp;Shipmate service key</td>
                    <td class="grid-column-data" style="width: 75% !important;">
                        <asp:TextBox ID="txtConfigShipmateServiceKey" CssClass="ShipmateConfigFullWidth" runat="server"></asp:TextBox></td>
                </tr>
                <tr class="grid-alt-row-color">
                    <td class="grid-column-label" style="width: 25% !important;">&nbsp;Shipmate base url</td>
                    <td class="grid-column-data" style="width: 75% !important;">
                        <asp:TextBox ID="txtConfigShipmateBaseUrl" CssClass="ShipmateConfigFullWidth" runat="server"></asp:TextBox></td>
                </tr>
                <tr class="grid-row-color">
                    <td class="grid-column-label" style="width: 25% !important;">&nbsp;Carrier track and trace url</td>
                    <td class="grid-column-data" style="width: 75% !important;">
                        <asp:TextBox ID="txtConfigCarrierTrackAndTraceUrl" CssClass="ShipmateConfigFullWidth" runat="server"></asp:TextBox></td>
                </tr>
                <tr class="grid-alt-row-color">
                    <td class="grid-column-label" style="width: 25% !important;">&nbsp;Delivery to name</td>
                    <td class="grid-column-data" style="width: 75% !important;">
                        <asp:TextBox ID="txtConfigDeliveryToName" CssClass="ShipmateConfigFullWidth" runat="server"></asp:TextBox></td>
                </tr>
                <tr class="grid-row-color">
                    <td class="grid-column-label" style="width: 25% !important;">&nbsp;Delivery to line1</td>
                    <td class="grid-column-data" style="width: 75% !important;">
                        <asp:TextBox ID="txtConfigDeliveryToLine1" CssClass="ShipmateConfigFullWidth" runat="server"></asp:TextBox></td>
                </tr>
                <tr class="grid-alt-row-color">
                    <td class="grid-column-label" style="width: 25% !important;">&nbsp;Delivery to line2</td>
                    <td class="grid-column-data" style="width: 75% !important;">
                        <asp:TextBox ID="txtConfigDeliveryToLine2" CssClass="ShipmateConfigFullWidth" runat="server"></asp:TextBox></td>
                </tr>
                <tr class="grid-row-color">
                    <td class="grid-column-label" style="width: 25% !important;">&nbsp;Delivery to line3</td>
                    <td class="grid-column-data" style="width: 75% !important;">
                        <asp:TextBox ID="txtConfigDeliveryToLine3" CssClass="ShipmateConfigFullWidth" runat="server"></asp:TextBox></td>
                </tr>
                <tr class="grid-alt-row-color">
                    <td class="grid-column-label" style="width: 25% !important;">&nbsp;Delivery to company name</td>
                    <td class="grid-column-data" style="width: 75% !important;">
                        <asp:TextBox ID="txtConfigDeliveryToCompanyName" CssClass="ShipmateConfigFullWidth" runat="server"></asp:TextBox></td>
                </tr>
                <tr class="grid-row-color">
                    <td class="grid-column-label" style="width: 25% !important;">&nbsp;Delivery to telephone</td>
                    <td class="grid-column-data" style="width: 75% !important;">
                        <asp:TextBox ID="txtConfigDeliveryToTelephone" CssClass="ShipmateConfigFullWidth" runat="server"></asp:TextBox></td>
                </tr>
                <tr class="grid-alt-row-color">
                    <td class="grid-column-label" style="width: 25% !important;">&nbsp;Delivery to email</td>
                    <td class="grid-column-data" style="width: 75% !important;">
                        <asp:TextBox ID="txtConfigDeliveryToEmail" CssClass="ShipmateConfigFullWidth" runat="server"></asp:TextBox></td>
                </tr>
                <tr class="grid-row-color">
                    <td class="grid-column-label" style="width: 25% !important;">&nbsp;Delivery to city</td>
                    <td class="grid-column-data" style="width: 75% !important;">
                        <asp:TextBox ID="txtConfigDeliveryToCity" CssClass="ShipmateConfigFullWidth" runat="server"></asp:TextBox></td>
                </tr>
                <tr class="grid-alt-row-color">
                    <td class="grid-column-label" style="width: 25% !important;">&nbsp;Delivery to postcode</td>
                    <td class="grid-column-data" style="width: 75% !important;">
                        <asp:TextBox ID="txtConfigDeliveryToPostcode" CssClass="ShipmateConfigFullWidth" runat="server"></asp:TextBox></td>
                </tr>
                <tr class="grid-row-color" style="border-bottom: 1px solid black !important;">
                    <td class="grid-column-label" style="width: 25% !important;">&nbsp;Delivery to country</td>
                    <td class="grid-column-data" style="width: 75% !important;">
                        <asp:TextBox ID="txtConfigDeliveryToCountry" CssClass="ShipmateConfigFullWidth" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td colspan="2">&nbsp;&nbsp;&nbsp;</td>
                </tr>
                <tr class="grid-row-color" style="border-right: 1px solid white !important; border-left: 1px solid white !important;">
                    <td class="grid-column-label" colspan="2">
                        <asp:Button ID="btnSaveConfig" runat="server" Text="Save" Font-Bold="true" OnClick="btnSaveConfig_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btnEditConfig" runat="server" Text="Edit" Font-Bold="true" OnClick="btnEditConfig_Click" />
                    </td>
                </tr>
            </table>
        </form>
    </div>

    <div id="divModalHeader" class="modal-header grid-header-color" runat="server">
        <asp:Label ID="lblTitle" runat="server" Font-Bold="true"></asp:Label>
    </div>

    <div id="divModalBody" class="modal-body" style="padding: 10px !important; max-height: 2000px !important;" runat="server">
        <div id="divConsignment" runat="server">
            <form id="formConsignment" runat="server">
                <asp:HiddenField ID="consignmentState" runat="server" />
                <div id="divNonPositiveIntegerError" style="color: red !important; margin-top: 5px !important; margin-bottom: 10px !important;" runat="server">
                    <strong>Error! Weight, width, length and depth should all be positive integers!</strong>
                </div>
                <table id="tableParcelData" style="margin-top: 10px !important; width: 100% !important; border-top: 1px solid black !important;">
                    <tr class="grid-header-color">
                        <td class="grid-column-label" colspan="2">&nbsp;Please adjust weight, width, length and depth</td>
                    </tr>
                    <tr class="grid-row-color">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Weight (in grammes)</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:TextBox ID="txtParcelWeight" runat="server" ReadOnly="false" CssClass="ShipmateConsignmentFullWidth"></asp:TextBox></td>
                    </tr>
                    <tr class="grid-alt-row-color">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Width (in centimetres)</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:TextBox ID="txtParcelWidth" runat="server" ReadOnly="false" CssClass="ShipmateConsignmentFullWidth"></asp:TextBox></td>
                    </tr>
                    <tr class="grid-row-color">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Length (in centimetres)</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:TextBox ID="txtParcelLength" runat="server" ReadOnly="false" CssClass="ShipmateConsignmentFullWidth"></asp:TextBox></td>
                    </tr>
                    <tr class="grid-alt-row-color">
                        <td class="grid-column-label" style="width: 40% !important; border-bottom: 1px solid black !important;">&nbsp;Depth (in centimetres)</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:TextBox ID="txtParcelDepth" runat="server" ReadOnly="false" CssClass="ShipmateConsignmentFullWidth"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td colspan="2">&nbsp;&nbsp;&nbsp;</td>
                    </tr>
                    <tr class="grid-row-color">
                        <td style="text-align: right !important; border-right: 1px solid white !important; border-left: 1px solid white !important;" colspan="2">
                            <asp:Button ID="Button1" runat="server" Text="Create" OnClick="btnCreateConsignment_Click" Font-Bold="true" />&nbsp;&nbsp;&nbsp;
                          <button onclick="window.parent.document.getElementById('btnbookingCancelWindow').click();"><strong>Cancel</strong></button>
                        </td>
                    </tr>
                </table>

                <table style="margin-top: 10px !important;">
                    <tr id="trConsignSuccess">
                        <td colspan="2" style="color: green !important; font-weight:bolder !important;">Consignment was created successfully</td>
                    </tr>
                    <tr id="trBlank1">
                        <td colspan="2">&nbsp;&nbsp;&nbsp;</td>
                    </tr>
                    <tr id="trOk">
                        <td colspan="2"><button onclick="window.parent.document.getElementById('btnbookingCancelWindow').click();"><strong>Ok</strong></button></td>
                    </tr>
                    <tr id="trBlank2">
                        <td colspan="2">&nbsp;&nbsp;&nbsp;</td>
                    </tr>
                     <tr id="trToggle">
                        <td colspan="2"><a id="aNonEditableData" href="javascript: toggleVisibility()"></a></td>
                    </tr>
                </table>

                <table id="tableConsignmentDetails" style="margin-top: 15px; width: 100% !important; border-top: 1px solid black !important;">
                    <tr id="trHeaderNonEditableData" class="grid-header-color NonEditableData">
                        <td class="grid-column-label" colspan="2">&nbsp;Non editable data</td>
                    </tr>
                    <tr class="grid-row-color NonEditableData">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Consignment reference </td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:TextBox ID="txtConsignmentReference" runat="server" ReadOnly="true" CssClass="ShipmateConsignmentFullWidth"></asp:TextBox>
                            <asp:Label ID="lblConsignmentReference" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="grid-alt-row-color NonEditableData">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Parcel reference</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:TextBox ID="txtParcelReference" runat="server" ReadOnly="true" CssClass="ShipmateConsignmentFullWidth"></asp:TextBox>
                            <asp:Label ID="lblParcelReference" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="grid-row-color NonEditableData">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;ServiceID</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:TextBox ID="txtServiceID" runat="server" ReadOnly="true" CssClass="ShipmateConsignmentFullWidth"></asp:TextBox>
                            <asp:Label ID="lblServiceID" runat="server"></asp:Label>
                        </td>
                    </tr>
                   <tr class="grid-alt-row-color NonEditableData">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Service key</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:TextBox ID="txtServiceKey" runat="server" ReadOnly="true" CssClass="ShipmateConsignmentFullWidth"></asp:TextBox>
                            <asp:Label ID="lblServiceKey" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="grid-row-color ResponseFromShipmate">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Tracking reference</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:Label ID="lblTrackingReference" runat="server"></asp:Label></td>
                    </tr>
                    <tr class="grid-alt-row-color ResponseFromShipmate">
                        <td class="grid-column-label" title="Generated on Parcel creation" style="width: 40% !important;">&nbsp;Label created date</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:Label ID="lblLabelCreated" runat="server"></asp:Label></td>
                    </tr>
                    <tr class="grid-row-color ResponseFromShipmate">
                        <td class="grid-column-label" title="Triggered when a Parcel has been manifested and sent to a Carrier" style="width: 40% !important;">&nbsp;Manifested date</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:Label ID="lblManifested" runat="server"></asp:Label></td>
                    </tr>
                    <tr class="grid-alt-row-color ResponseFromShipmate">
                        <td class="grid-column-label" title="Triggered when the Consignment has been collected by the Carrier" style="width: 40% !important;">&nbsp;Collected date</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:Label ID="lblCollected" runat="server"></asp:Label></td>
                    </tr>
                    <tr class="grid-row-color ResponseFromShipmate">
                        <td class="grid-column-label" title="Triggered when any Events happen between collection and delivery" style="width: 40% !important;">&nbsp;In transit date</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:Label ID="lblInTransit" runat="server"></asp:Label></td>
                    </tr>
                    <tr class="grid-alt-row-color ResponseFromShipmate">
                        <td class="grid-column-label" title="Triggered when a Parcel has been successfully delivered" style="width: 40% !important;">&nbsp;Delivered date</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:Label ID="lblDelivered" runat="server"></asp:Label></td>
                    </tr>
                    <tr class="grid-row-color ResponseFromShipmate">
                        <td class="grid-column-label" title="Triggered when a Parcel has had a failed delivery attempt" style="width: 40% !important;">&nbsp;Delivery failed date</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:Label ID="lblDeliverFailed" runat="server"></asp:Label></td>
                    </tr>
                    <tr class="grid-alt-row-color ResponseFromShipmate">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Carrier</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:Label ID="lblCarrier" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="grid-row-color ResponseFromShipmate">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Service name</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:Label ID="lblServiceName" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="grid-alt-row-color ResponseFromShipmate">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Created by</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:Label ID="lblCreatedBy" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="grid-row-color ResponseFromShipmate">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Created with</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:Label ID="lblCreatedWith" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="grid-alt-row-color ResponseFromShipmate">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Created at</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:Label ID="lblCreatedAt" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="grid-row-color ParcelData">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Parcel weight</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:Label ID="lblParcelWeight" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="grid-alt-row-color ParcelData">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Parcel width</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:Label ID="lblParcelWidth" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="grid-row-color ParcelData">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Parcel length</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:Label ID="lblParcelLength" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="grid-alt-row-color ParcelData">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Parcel depth</td>
                        <td class="grid-column-data" style="width: 60% !important;" >
                            <asp:Label ID="lblParcelDepth" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="grid-row-color NonEditableData">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Collection from name</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:TextBox ID="txtCollectionFromName" runat="server" ReadOnly="true" CssClass="ShipmateConsignmentFullWidth"></asp:TextBox>
                            <asp:Label ID="lblCollectionFromName" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="grid-alt-row-color NonEditableData">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Collection from line 1</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:TextBox ID="txtCollectionFromLine1" runat="server" ReadOnly="true" CssClass="ShipmateConsignmentFullWidth"></asp:TextBox>
                            <asp:Label ID="lblCollectionFromLine1" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="grid-row-color NonEditableData">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Collection from line 2</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:TextBox ID="txtCollectionFromLine2" runat="server" ReadOnly="true" CssClass="ShipmateConsignmentFullWidth"></asp:TextBox>
                            <asp:Label ID="lblCollectionFromLine2" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="grid-alt-row-color NonEditableData">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Collection from line 3</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:TextBox ID="txtCollectionFromLine3" runat="server" ReadOnly="true" CssClass="ShipmateConsignmentFullWidth"></asp:TextBox>
                            <asp:Label ID="lblCollectionFromLine3" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="grid-row-color NonEditableData">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Collection from company name</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:TextBox ID="txtCollectionFromCompanyName" runat="server" ReadOnly="true" CssClass="ShipmateConsignmentFullWidth"></asp:TextBox>
                            <asp:Label ID="lblCollectionFromCompanyName" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="grid-alt-row-color NonEditableData">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Collection from telephone</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:TextBox ID="txtCollectionFromTelephone" runat="server" ReadOnly="true" CssClass="ShipmateConsignmentFullWidth"></asp:TextBox>
                            <asp:Label ID="lblCollectionFromTelephone" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="grid-row-color NonEditableData">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Collection from email address</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:TextBox ID="txtCollectionFromEmailAddress" runat="server" ReadOnly="true" CssClass="ShipmateConsignmentFullWidth"></asp:TextBox>
                            <asp:Label ID="lblCollectionFromEmailAddress" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="grid-alt-row-color NonEditableData">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Collection from city</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:TextBox ID="txtCollectionFromCity" runat="server" ReadOnly="true" CssClass="ShipmateConsignmentFullWidth"></asp:TextBox>
                            <asp:Label ID="lblCollectionFromCity" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="grid-row-color NonEditableData">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Collection from postcode</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:TextBox ID="txtCollectionFromPostcode" runat="server" ReadOnly="true" CssClass="ShipmateConsignmentFullWidth"></asp:TextBox>
                            <asp:Label ID="lblCollectionFromPostcode" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="grid-alt-row-color NonEditableData">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Collection from country</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:TextBox ID="txtCollectionFromCountry" runat="server" ReadOnly="true" CssClass="ShipmateConsignmentFullWidth"></asp:TextBox>
                            <asp:Label ID="lblCollectionFromCountry" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="grid-row-color NonEditableData">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Delivery to name</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:TextBox ID="txtDeliveryToName" runat="server" ReadOnly="true" CssClass="ShipmateConsignmentFullWidth"></asp:TextBox>
                            <asp:Label ID="lblDeliveryToName" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="grid-alt-row-color NonEditableData">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Delivery to line 1</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:TextBox ID="txtDeliveryToLine1" runat="server" ReadOnly="true" CssClass="ShipmateConsignmentFullWidth"></asp:TextBox>
                            <asp:Label ID="lblDeliveryToLine1" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="grid-row-color NonEditableData">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Delivery to line 2</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:TextBox ID="txtDeliveryToLine2" runat="server" ReadOnly="true" CssClass="ShipmateConsignmentFullWidth"></asp:TextBox>
                            <asp:Label ID="lblDeliveryToLine2" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="grid-alt-row-color NonEditableData">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Delivery to line 3</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:TextBox ID="txtDeliveryToLine3" runat="server" ReadOnly="true" CssClass="ShipmateConsignmentFullWidth"></asp:TextBox>
                            <asp:Label ID="lblDeliveryToLine3" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="grid-row-color NonEditableData">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Delivery to company name</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:TextBox ID="txtDeliveryToCompanyName" runat="server" ReadOnly="true" CssClass="ShipmateConsignmentFullWidth"></asp:TextBox>
                            <asp:Label ID="lblDeliveryToCompanyName" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="grid-alt-row-color NonEditableData">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Delivery to telephone</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:TextBox ID="txtDeliveryToTelephone" runat="server" ReadOnly="true" CssClass="ShipmateConsignmentFullWidth"></asp:TextBox>
                            <asp:Label ID="lblDeliveryToTelephone" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="grid-row-color NonEditableData">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Delivery to email address</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:TextBox ID="txtDeliveryToEmailAddress" runat="server" ReadOnly="true" CssClass="ShipmateConsignmentFullWidth"></asp:TextBox>
                            <asp:Label ID="lblDeliveryToEmailAddress" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="grid-alt-row-color NonEditableData">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Delivery to city</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:TextBox ID="txtDeliveryToCity" runat="server" ReadOnly="true" CssClass="ShipmateConsignmentFullWidth"></asp:TextBox>
                            <asp:Label ID="lblDeliveryToCity" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="grid-row-color NonEditableData">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Delivery to postcode</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:TextBox ID="txtDeliveryToPostcode" runat="server" ReadOnly="true" CssClass="ShipmateConsignmentFullWidth"></asp:TextBox>
                            <asp:Label ID="lblDeliveryToPostcode" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr class="grid-alt-row-color NonEditableData" style="border-bottom: 1px solid black !important;">
                        <td class="grid-column-label" style="width: 40% !important;">&nbsp;Delivery to country</td>
                        <td class="grid-column-data" style="width: 60% !important;">
                            <asp:TextBox ID="txtDeliveryToCountry" runat="server" ReadOnly="true" CssClass="ShipmateConsignmentFullWidth"></asp:TextBox>
                            <asp:Label ID="lblDeliveryToCountry" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="SaediFromId" runat="server" />
                <asp:HiddenField ID="ClientRef" runat="server" />
                <asp:HiddenField ID="OnlineBookingURL" runat="server" />
            </form>
        </div>
        <div id="divError" runat="server">
            <span id="spanError" style="color: red !important; font-weight: bolder !important; margin-top: 10px !important;" runat="server"></span>
            <br />
            <br />
            <button onclick="window.parent.document.getElementById('btnbookingCancelWindow').click();">Ok</button>
        </div>
    </div>

    <script>
        function getModalObject() {
            var iframes, modalObject;

            iframes = window.parent.document.getElementsByTagName("iframe");

            for (var i = 0; i < iframes.length; i++) {
                if (iframes[i].id.indexOf("ModalPopupExtender") >= 0) {
                    modalObject = iframes[i];
                    break;
                }
            }

            return modalObject;
        }

        function toggleVisibility() {
            var consignmentState, modalObject;

            modalObject = getModalObject();
            consignmentState = $("#consignmentState").val();

            if ($("#tableConsignmentDetails").is(":visible")) {
                $("#tableConsignmentDetails").hide();

                if ($("input", "#tableConsignmentDetails").length > 0)
                    modalObject.height = 370;
                else
                    modalObject.height = 200;

                if (consignmentState === "Create")
                    $("#aNonEditableData").text("Show non editable booking data");
                else
                    $("#aNonEditableData").text("Show consigment details");
            }
            else {
                $("#tableConsignmentDetails").show();
                modalObject.height = 600;

                if (consignmentState === "Create")
                    $("#aNonEditableData").text("Hide non editable booking data");
                else
                    $("#aNonEditableData").text("Hide consigment details");
            }
        }

        $(document).ready(function () {
            var configState, consignmentState, errorMsg, modalObject;
            
            configState = $("#configState").val();
            consignmentState = $("#consignmentState").val();
            errorMsg = $("#spanError").text();

            if (configState) {
                if (configState === "ConfigSearch") {
                    $("#txtConfigClientId").val("");
                    $(".ShipmateConfigFullWidth").val("");
                    $(".ShipmateConfigFullWidth").attr("readonly", true);
                    $("#btnSearchClient").attr("disabled", false);
                    $("#btnNewSearchClient").attr("disabled", true);
                    $("#btnSaveConfig").attr("disabled", true);
                    $("#btnEditConfig").attr("disabled", true);
                    $("#txtConfigClientId").attr("readonly", false);
                }
                else if (configState === "ConfigEditAvailable") {
                    $(".ShipmateConfigFullWidth").attr("readonly", true);
                    $("#btnSearchClient").attr("disabled", true);
                    $("#btnNewSearchClient").attr("disabled", false);
                    $("#btnSaveConfig").attr("disabled", true);
                    $("#btnEditConfig").attr("disabled", false);
                    $("#txtConfigClientId").attr("readonly", true);
                }
                else if (configState === "ConfigEdit") {
                    $(".ShipmateConfigFullWidth").attr("readonly", false);
                    $("#btnSearchClient").attr("disabled", true);
                    $("#btnNewSearchClient").attr("disabled", false);
                    $("#btnSaveConfig").attr("disabled", false);
                    $("#btnEditConfig").attr("disabled", true);
                    $("#txtConfigClientId").attr("readonly", true);
                }
            }
            else if (consignmentState) {
                modalObject = getModalObject();
                modalObject.width = 600;

                if (consignmentState === "Create") { //To create a consignment. The user can adjust parcel weight, width, length and depth. A Create and Cancel button in the modal window.
                    $(".ResponseFromShipmate, .ParcelData").remove();
                    $("span", "#tableConsignmentDetails").remove();
                    $("#tableConsignmentDetails").hide();
                    $("#aNonEditableData").text("Show non editable booking data");
                    $("#trConsignSuccess, #trBlank1, #trOk, #trBlank2").remove();
                    modalObject.height = 370;
                }
                else if (consignmentState === "Create success") { //The consignment was successfully created. Show a message "Consignment was created successfully". Only an Ok button in the modal window.
                    $("#tableParcelData").remove();
                    $("input", "#tableConsignmentDetails").remove();
                    $("#trHeaderNonEditableData").remove();
                    $("#tableConsignmentDetails").hide();
                    $("#aNonEditableData").text("Show consigment details");
                    $("td").css("padding", "4px");
                    modalObject.height = 200;
                }
                else { //Show consignment details. Only an Ok button in the modal window.
                    $("#tableParcelData").remove();
                    $("input", "#tableConsignmentDetails").remove();
                    $("#trHeaderNonEditableData, #trConsignSuccess, #trBlank1, #trBlank2").remove();
                    $("td").css("padding", "4px");
                    $("#tableMsgOk").css("margin-bottom", "2px");
                    modalObject.height = 600;
                }
            }
            else if (errorMsg) {
                modalObject = getModalObject();
                modalObject.width = 500;
                modalObject.height = 200;
            }
        });
    </script>
</body>
</html>
