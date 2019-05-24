<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShipMate.aspx.cs" Inherits="MobilePortal.ShipMate" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Shipmate</title>
     <link rel="stylesheet" href="css/default.css"/>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            //Attach click event to image button using class selector
            $('.imgbutton').on("click", function () {
                //  alert('edit called');
                ShowProgress();
                return true;
            });
        });

        function ShowProgress() {
            setTimeout(function () {
                var modal = $('<div />');
                modal.addClass("modal");
                $('body').append(modal);
                var loading = $(".loading");
                loading.show();
                var top = Math.max($(window).height() / 2 - loading[0].offsetHeight / 2, 0);
                var left = Math.max($(window).width() / 2 - loading[0].offsetWidth / 2, 0);
                loading.css({ top: top, left: left });
            }, 200);
        }
        function okay() {

            window.parent.document.getElementById('btnOkay').click();
            window.location.reload();

        }
        function cancel() {

            window.parent.document.getElementById('btnbookingCancelWindow').click();
            //  window.location.reload();

        }
    </script>
</head>
   
<body>
    
     <style type="text/css">
     .modal
    {
        position: fixed;
        top: 0;
        left: 0;
        background-color: black;
        z-index: 99;
        opacity: 0.8;
        filter: alpha(opacity=80);
        -moz-opacity: 0.8;
        min-height: 100%;
        width: 100%;
    }
    .loading
    {
        font-family: Arial;
        font-size: 10pt;
        border: 5px solid #67CFF5;
        width: 200px;
        height: 100px;
        display: none;
        position: fixed;
        background-color: White;
        z-index: 999;
    }
   .gridview
{
    
    border-style:none !important;
    
    }
</style>
    <form id="form1" runat="server">
        <div  class="popup_Container">
             <div class="popup_Titlebar" id="PopupHeader">
            <div class="TitlebarLeft">
                <asp:Label ID="lblTitle" runat="server"></asp:Label>
            </div>
            <div class="TitlebarRight" onclick="cancel();">
            </div>
        </div>
        <div class="popup_Body">
     <div>

<div>
    <table style="width: 40%;">
        <tr><td class="auto-style2" style="font-family: Arial; font-weight: bolder;">Consignment reference:</td><td class="auto-style2"><asp:Label ID="lblConsignmentReference" runat="server"></asp:Label></td></tr>
        <tr><td class="auto-style2" style="font-family: Arial; font-weight: bolder;">Parcel reference:</td><td class="auto-style2"><asp:Label ID="lblParcelReference" runat="server"></asp:Label></td></tr>
        <tr><td class="auto-style2" style="font-family: Arial; font-weight: bolder;">Carrier:</td><td class="auto-style2"><asp:Label ID="lblCarrier" runat="server"></asp:Label></td></tr>
        <tr><td class="auto-style2" style="font-family: Arial; font-weight: bolder;">Service name:</td><td class="auto-style2"><asp:Label ID="lblServiceName" runat="server"></asp:Label></td></tr>
        <tr><td class="auto-style2" style="font-family: Arial; font-weight: bolder;">Tracking reference:</td><td class="auto-style2"><asp:Label ID="lblTrackingReference" runat="server"></asp:Label></td></tr>
        <tr><td class="auto-style2" style="font-family: Arial; font-weight: bolder;">Created by:</td><td class="auto-style2"><asp:Label ID="lblCreatedBy" runat="server"></asp:Label></td></tr>
        <tr><td class="auto-style2" style="font-family: Arial; font-weight: bolder;">Created with:</td><td class="auto-style2"><asp:Label ID="lblCreatedWith" runat="server"></asp:Label></td></tr>
        <tr><td class="auto-style2" style="font-family: Arial; font-weight: bolder;">Created at</td><td class="auto-style2"><asp:Label ID="lblCreatedAt" runat="server"></asp:Label></td></tr>
        <tr><td class="auto-style2" style="font-family: Arial; font-weight: bolder;">Delivery name:</td><td class="auto-style2"><asp:Label ID="lblDeliveryName" runat="server"></asp:Label></td></tr>
        <tr><td class="auto-style2" style="font-family: Arial; font-weight: bolder;">Line:</td><td class="auto-style2"><asp:Label ID="lblLine" runat="server"></asp:Label></td></tr>
        <tr><td class="auto-style2" style="font-family: Arial; font-weight: bolder;">City:</td><td class="auto-style2"><asp:Label ID="lblCity" runat="server"></asp:Label></td></tr>
        <tr><td class="auto-style2" style="font-family: Arial; font-weight: bolder;">Postcode:</td><td class="auto-style2"><asp:Label ID="lblPostcode" runat="server"></asp:Label></td></tr>
        <tr><td class="auto-style2" style="font-family: Arial; font-weight: bolder;">Contry:</td><td class="auto-style2"><asp:Label ID="lblCountry" runat="server"></asp:Label></td></tr>
    </table>
</div>

</div>
        </div>   <div class="popup_Buttons">
            <input id="btnOkay" value="Next" type="button" runat="server" onclick="okay();"  style="display:none" />
            <input id="btnCancel" value="Cancel" type="button" onclick="cancel();"   style="display:none" />
        </div>
            </div>
    </form>

    <div class="loading" align="center">
    Loading. Please wait.<br />
    <br />
    <img  src="image/loading_animated.gif" alt="No Loading image" />
</div>
</body>
</html>