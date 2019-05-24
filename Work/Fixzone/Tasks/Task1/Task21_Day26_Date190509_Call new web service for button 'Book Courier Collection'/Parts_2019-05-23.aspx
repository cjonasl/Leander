<%@ Page Title="Parts" Language="C#" MasterPageFile="~/mobile.Master" AutoEventWireup="true"
    CodeBehind="Parts.aspx.cs" Inherits="MobilePortal.PartsPage" Debug="true" %>

<%--<%@ Register Src="~/WebControls/AppointmentRequest.ascx" TagPrefix="uc1" TagName="AppointmentRequest" %>--%>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <script type="text/javascript">
            function DisableAllocate() {
                // Must be timer otherwise the postback is not triggered!
                window.setTimeout(function () {
                    document.getElementById('<% = usePartLinkButton.ClientID %>').disabled = true;
                }, 1);
            }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
 <style type="text/css">
   
     .ModalPopupBG
{
	background-color: #666699;
	filter: alpha(opacity=50);
	opacity: 0.7;
}</style>  

    <div>
        <div style="float: left">
            <asp:Label ID="callLabel" runat="server" Style="text-align: right"></asp:Label>
        </div>
        <div style="float: right">
            <asp:Label ID="readOnlyLabel" runat="server" Text="Assigned to Mobile" Font-Bold="True"
                Font-Size="Small" ForeColor="Red"></asp:Label>
            &nbsp;
             <asp:Button ID="WebEdit" runat="server" CssClass="btn btn-inverse" OnClick="WebEdit_Click"
                        Text="Access from Web" />
                    &nbsp;
            <asp:ImageButton ID="refreshCallImageButton" runat="server" ImageUrl="~/image/smallRefresh.png"
                OnClick="refreshCallImageButton_Click" Height="16px" Width="16px" />
        </div>
        <div style="clear: both" />
        <div>
            <!-- Phones -->
            <div class="btn-group visible-phone">
                <a class="btn dropdown-toggle" data-toggle="dropdown" href="#">
                    <img src="image/chargesButton.png" alt="" />
                    Parts <span style="float: right" class="caret"></span></a>
                <ul class="dropdown-menu">
                    <li><a href="Appointment.aspx">
                        <img src="image/callButton.png" alt="" />Appointment</a> </li>
                    <li><a href="Instructions.aspx">
                        <img src="image/instructionsButton.png" border="none" alt="" />Instructions</a></li>
                    <li class="active"><a href="Parts.aspx">
                        <img src="image/partsButton.png" border="none" alt="" />Parts</a></li>
                    <li><a href="Notes.aspx">
                        <img src="image/notesButton.png" border="none" alt="" />Notes</a></li>
                    <li><a href="Reports.aspx">
                        <img src="image/reportButton.png" border="none" alt="" />Reports</a></li>
                    <li><a href="Charges.aspx">
                        <img src="image/chargesButton.png" border="none" alt="" />Charges</a></li>
                    <li><a href="History.aspx">
                        <img src="image/historyButton.png" border="none" alt="" />History</a></li>
                    <li><a href="Media.aspx">
                        <img src="image/mediaButton.png" border="none" alt="" />Media</a></li>
                    <% if (HasInspections)
                       { %>
                    <li><a class="" href="Inspections.aspx">
                        <img src="image/inpectionsButton.png" border="none" alt="" />Inspections</a></li>
                    <% } %>
                </ul>
            </div>

            <!-- Tablets and Desktop -->
            <ul class="nav nav-tabs hidden-phone">
                <li><a href="Appointment.aspx">
                    <img src="image/callButton.png" alt="" />Appointment</a> </li>
                <li><a href="Instructions.aspx">
                    <img src="image/instructionsButton.png" border="none" alt="" />Instructions</a></li>
                <li class="active"><a href="Parts.aspx">
                    <img src="image/partsButton.png" border="none" alt="" />Parts</a></li>
                <li><a href="Notes.aspx">
                    <img src="image/notesButton.png" border="none" alt="" />Notes</a></li>
                <li><a href="Reports.aspx">
                    <img src="image/reportButton.png" border="none" alt="" />Reports</a></li>
                <li><a href="Charges.aspx">
                    <img src="image/chargesButton.png" border="none" alt="" />Charges</a></li>
                <li><a href="History.aspx">
                    <img src="image/historyButton.png" border="none" alt="" />History</a></li>
                <li><a href="Media.aspx">
                        <img src="image/mediaButton.png" border="none" alt="" />Media</a></li>

                <% if (HasInspections)
                   { %>
                <li><a class="" href="Inspections.aspx">
                    <img src="image/inpectionsButton.png" border="none" alt="" />Inspections</a></li>
                <% } %>
                 <% if (call.SaediToId.ToUpper() == "SONY3C") %>
                    <% { %>

                    <li><a class="" href="SonyInfo.aspx">
                       <img src="image/SONYcC.png" border="none" alt="" /></a></li>
                    <% } %>
            </ul>
        </div>
        <div>
            <% System.Collections.Generic.List<Mobile.Portal.Session.ErrorEntry> errors = ((MobilePortal.mobile)Master).ErrorList(); %>
            <% if (errors.Count > 0) %>
            <% { %>
            <div class="alert alert-error">
                <a class="close" data-dismiss="alert" href="#">×</a>
                <h4 class="alert-heading">
                    Warning!
                    <%= (((MobilePortal.mobile)Master).ErrorTitle()) %></h4>
                <p>
                    <ul>
                        <% foreach (Mobile.Portal.Session.ErrorEntry err in errors) %>
                        <% { %>
                        <li><a href='<%= err.Redirect.Replace("~/","") %>'>
                            <%= (err.Message) %></a></li>
                        <% } %>
                    </ul>
                </p>
            </div>
            <% } %>
            <div id="PartsList" runat ="server">
            <div id="allocatedPartsDiv">
                <asp:GridView ID="partsUsedGridView" runat="server" AutoGenerateColumns="False" CellPadding="3"
                    CssClass="table table-condensed" DataSourceID="partsUsedDataSource" ShowFooter="True" 
                    OnRowCommand="partsUsedGridView_RowCommand" OnRowDataBound  ="partsUsedGridView_DataBound"
                    DataKeyNames="PartReference" OnSelectedIndexChanged="partsUsedGridView_SelectedIndexChanged"
                    GridLines="None">
                    <EmptyDataTemplate>
                        <div style="text-align: center">
                            No Parts Have Been Allocated
                        </div>
                    </EmptyDataTemplate>
                    <SelectedRowStyle CssClass="grid-row-color-selected" />
                    <HeaderStyle CssClass="grid-header-color" />
                    <AlternatingRowStyle CssClass="grid-alt-row-color" />
                    <EmptyDataRowStyle Width="100%" CssClass="grid-header-color" />
                    <Columns>
                        <asp:TemplateField HeaderText="Allocated" SortExpression="Code">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Code") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <HeaderTemplate>
                                <div>Allocated</div>                                    
                            </HeaderTemplate>
                            <ItemTemplate>
                            <div>
                                <div class="pull-left">
                                    <asp:LinkButton ID="LinkButton8" runat="server" Text='<%# Eval("Code") == null || Eval("Code") == "" ? "<span style=\"color:red\">(blank)</span>" : Eval("Code") %>'
                                        CommandArgument='<%# Eval("PartReference") %>' CommandName="Select"></asp:LinkButton>
                                    <br />
                                    <asp:Label ID="Label1" runat="server" Text='<%# Eval("Description") == null || Eval("Description") == "" ? "<span style=\"color:red\">(blank)</span>" : Eval("Description") %>'>

                                    </asp:Label>
                                    <br />
                                    Order Number&nbsp;<asp:Label ID="Label16" runat="server" Font-Bold="true" Text='<%# Eval("PartReference") %>'></asp:Label>                                   
                                    <div id="Div1" runat="server" visible='<%# (bool)Eval("IsSony") == true %>'>
                                     
                                        Order Reference (SON)&nbsp;<asp:Label ID="Label18" runat="server" /><%# ((bool)Eval("IsSony") == true &&(bool)(Eval("StatusID").ToString() == "V")?Eval("InputSon"):Eval("OrderReference")) %><br />                                        
                                            <asp:Label ID="Label17" Font-Bold="true" runat="server" Text='<%# Eval("SonyPartStatus") %>'></asp:Label><br />
                                        Primary&nbsp;<asp:Label ID="Label22" runat="server"><%# ((bool)Eval("IsPrimary") == true || (bool)Eval("IsBulletin") == true ||                                                                          
                                                                          Eval("Code").ToString() == "000000010") ? "<strong>Yes</strong>" : "No" %> </asp:Label>                                              
                                    </div>
                                </div> 
                                
                                <div id="Div2" class="pull-right" runat="server" visible='<%# ((bool)Eval("IsSony") == true) %>'  style="width :65%">
                                  <div id="Div3"  class="pull-left" runat="server"  style="vertical-align:central">
                                  <br />                                      
                                       <asp:LinkButton ID="BtnBookCourier"      runat="server" Text="Book Courier Collection" 
                                       CommandName="BookCourier"  CommandArgument='<%# Eval("ReturnReference")%>'    OnCommand="BtnBookCourier_Click"
                                      CssClass="btn btn-warning"   />   <%-- Visible='<%#((bool)this.SonyStatusComplete== true)&& ((bool)Eval("NeedCollection") ) %>'--%>                                                                                    
                                      <br />
                                   
                                      <%# ((bool)(Eval("IsSony")))? ( String.IsNullOrEmpty(Eval("Collectionref").ToString().Trim())?"":"Collection ref:"+Eval("Collectionref").ToString()):"" %>
                                     <br />
                                      <asp:LinkButton ID="linkBtnShowConsignmentDetails" runat="server" Text="Show consignment details" Font-Size="Smaller" OnCommand="linkBtnShowConsignmentDetails_Click" CommandArgument='<%#Eval("Collectionref")%>'
                                                 Visible='<%# Eval("Collectionref") != null && Eval("Collectionref") != "" %>'></asp:LinkButton>
                                      <br />
                                       <%# ((bool)(Eval("IsSony")))?  (String.IsNullOrEmpty(Eval("CollectionDate").ToString().Trim())?"":"Collection booked for:"+Eval("CollectionDate","{0:d}")):"" %>
                                    
                                         <%--<%{ %>--%>
                                 <%-- <asp:Label ID="LblCollectRef"  Visible='<%# (bool)(Eval("IsSony")) %>'     runat="server" Text='<%# String.IsNullOrEmpty(Eval("Collectionref").ToString())?"":"Collection ref:"+Eval("Collectionref").ToString() %>'
								 
                                         />      --%>      <%--  Visible='<%# ((bool)this.SonyStatusComplete== true)? ((bool)Eval("NeedCollection") ):false %>' --%>
<%--<asp:Label ID="LblCollectionDate"      runat="server"  Text='<%# String.IsNullOrEmpty(Eval("CollectionDate").ToString())?"":"Collection booked for:"+Eval("CollectionDate","{0:d}") %>'--%>
                                   
                                              
									<%--	<%} %>--%>
										 
                                </div>  
                                  <div id="Div4" class="pull-right" runat="server" style="width:270px"> <asp:Label ID="Label11" Font-Size="Smaller" runat="server" Visible='<%# ((bool)(Eval("ReturnRequired")) == true) %>'
                                        Text='RMA Required.'></asp:Label>
                                    <asp:Label ID="Label91" Font-Size="Smaller" runat="server" Text='<%# Eval("ReturnDescription") %>'></asp:Label>
<%--                                <br />                         
                                   &&  Eval("Code").ToString() != "000000010" <asp:Label ID="Label13" runat="server" Visible='<%# (Eval("StatusID").ToString() != "V") %>' > Dispatch Date <%# Eval("DispatchDate", "{0:dd/M/yyyy}") %></asp:Label> --%>
                                    <br />
                            <asp:Button ID="BtnRMA"      runat="server" Text="Create RMA"  OnClick="BtnRMA_Click" 
                                      CssClass="btn btn-warning"   
                                />
                             <%--  Visible='<%# ((bool)this.SonyStatusComplete== true)?( (( (bool)Eval("IsAllocated")  || Eval("StatusID").ToString() == "V")  && (!(bool) Eval("IsBulletin")  &&  Eval("Code") .ToString()!= "TECHNICALBULLETIN")
                             &&  Eval("INPUTson").ToString().ToUpper() != "FOC" && ! (bool)Eval("IsRmaDone") && (bool)Eval("IsSony") == true) &&  this.IsSonyStatusInCompleted):false %>' --%>   
                                         <asp:Label ID="Label15" runat="server" Text='<%# ((bool)Eval("IsRmaDone") == true) ? "RMA Done" : "RMA Not Done!"  %>'
                                        Font-Size="Smaller" Visible='<%# Eval("Code").ToString() != "000000010" %>'></asp:Label>
                                    <br />
                                    <asp:Label ID="Label92" runat="server" Visible='<%# Eval("ReturnReference") != null && Eval("ReturnReference") != "" %>'
                                        Font-Size="Smaller">Return Ref.: </asp:Label>
                                    <asp:Label ID="Label90" runat="server" Text='<%# Eval("ReturnReference") %>' Visible='<%# Eval("ReturnReference") != null && Eval("ReturnReference") != "" %>'
                                        Font-Size="Smaller"></asp:Label>
                                    <br />
                                    <asp:Label ID="Label20" runat="server" Visible='<%# Eval("ReturnReference") != null && Eval("ReturnReference").ToString() != "" %>'
                                        Font-Size="Smaller">RMA Status: </asp:Label> 
                                    <asp:Label ID="Label21" runat="server" Text='<%# Eval("ValidationStatus") + " / " + Eval("ShipmentStatus")  %>' Visible='<%# Eval("ReturnReference") != null && Eval("ReturnReference") != "" %>'
                                        Font-Size="Smaller"></asp:Label>
                                    <br />
                                    <asp:HyperLink ID="HyperLink1" runat="server" Text="Show RMA PDF document" NavigateUrl='<%# Eval("RmaDocumentUrl") %>' Target="_blank" 
                                                   Visible='<%# Eval("RmaDocumentUrl") != null && Eval("RmaDocumentUrl") != "" %>' Font-Size="Smaller"></asp:HyperLink>
                                     <%-- Visible='<%#( (bool)Eval("IsAllocated")  || Eval("StatusID").ToString() == "V")  &&--%>
                               
                                             <asp:LinkButton ID="LinkButton1" runat="server" Text="Courier Label"  Font-Size="Smaller"   CommandArgument='<%#Eval("ConsignmentNo")+ ";" +Eval("Input_courierid")+";"+Eval("BookingUniqueNumber")%>'
                                                    Visible='<%# Eval("LabelUrl") != null && Eval("LabelUrl") != "" %>'></asp:LinkButton><%--OnCommand="LnkLabel_Click" --%>
                                                 
                                    <asp:LinkButton ID="LnkConNote" runat="server" Text="Courier Consignment note"  Font-Size="Smaller"   OnCommand="LnkConNote_Click" CommandArgument='<%#Eval("ConsignmentNo")+ ";" +Eval("Input_courierid")+";"+Eval("BookingUniqueNumber")%>'
                                                    Visible='<%# Eval("ConNoteUrl") != null && Eval("ConNoteUrl") != "" %>'></asp:LinkButton>
                                  <%--+ ";" +Eval("Input_courierid")--%>
                                    <asp:HyperLink ID="CourierTracking" runat="server" Text="Track" NavigateUrl='<%# ConfigurationSettings.AppSettings["TNTTrackUrl"]+Eval("ConsignmentNo") %>' Target="_blank" 
                                                    Font-Size="Smaller"  Visible='<%# Eval("ConNoteUrl") != null && Eval("ConNoteUrl") != "" %>'></asp:HyperLink>



                                    <br> 
                                </div>
                                    </div>
                            </div>
                            </ItemTemplate>
                            <FooterStyle CssClass="grid-footer-color-slim" />
                            <HeaderStyle CssClass="grid-header-color" HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qty" SortExpression="Quantity">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Quantity") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <HeaderTemplate>
                                <div class="right">
                                    Qty</div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div class="right">
                                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("Quantity") %>'></asp:Label>
                                </div>
                            </ItemTemplate>
                            <FooterStyle CssClass="grid-footer-color-slim" />
                            <HeaderStyle CssClass="grid-header-color" HorizontalAlign="Right" />
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Price" SortExpression="UnitPrice">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("UnitPrice") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <HeaderTemplate>
                                <div class="right">
                                    Price</div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div class="right">
                                    <asp:Label ID="Label3" runat="server" Text='<%# Bind("UnitPrice", "{0:f2}") %>'></asp:Label></div>
                            </ItemTemplate>
                            <FooterStyle CssClass="grid-footer-color-slim" />
                            <HeaderStyle CssClass="grid-header-color" HorizontalAlign="Right" />
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Status" SortExpression="Status">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("Status") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <HeaderTemplate>
                                <div class="right">
                                    Status</div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div class="right">
                                    <asp:Label ID="Label12" runat="server" Text='<%# "(" + Eval("StatusID") + ") " + Eval("Status") %>'></asp:Label>
                                </div>
                            </ItemTemplate>
                            <FooterStyle CssClass="grid-footer-color-slim" />
                            <HeaderStyle CssClass="grid-header-color" HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                    </Columns>
                    <RowStyle CssClass="grid-row-color" />
                </asp:GridView>
                <div id="allocatedPartsButtonsDiv" align="left">
                    <asp:Button ID="editAllocatedPartLinkButton" runat="server" OnClick="editAllocatedPartLinkButton_Click"
                        Text="Edit" CssClass="btn btn-primary" />
                    &nbsp;
                    <asp:Button ID="addAllocatedPartLinkButton" runat="server" OnClick="addAllocatedPartLinkButton_Click"
                        Text="Add Stock" CssClass="btn"  Visible='<%# !AepJob %>' />
                    &nbsp;
                    <asp:Button ID="removeAllocatedPartLinkButton" runat="server" OnClick="removeAllocatedPartLinkButton_Click"
                        Text="Remove" CssClass="btn btn-danger" />
                                        &nbsp;
                    <asp:Button ID="btnNoPartsUsed" runat="server" CssClass="btn btn-warning" OnClick="btnNoPartsUsed_Click"
                        Text="No Parts Used"  />
                                        &nbsp;
                    <asp:Button ID="btnTechnicalBulletin" runat="server" CssClass="btn btn-warning" OnClick="btnTechnicalBulletin_Click"
                        Text="Technical Bulletin Code"  />
                                        &nbsp;
                    <asp:Button ID="courierBookingButton" runat="server"  OnClick="courierBookingButton_Click"
                        Text="Courier Booking" Enabled="false" Visible="false" />
                       &nbsp;                    
                 <%--                                        &nbsp;
                    <asp:Button ID="partReturnTop" runat="server" CssClass="btn btn-warning" OnClick="partReturnTopButton_Click"
                        Text="Return Part" />--%>
                    <br />
                    <br />
                </div>
            </div>
            <asp:ObjectDataSource ID="partsUsedDataSource" runat="server" OnObjectCreating="partsUsedDataSource_ObjectCreating"
                SelectMethod="Select" TypeName="Mobile.Portal.BLL.PartsBLL"></asp:ObjectDataSource>
            <asp:Panel ID="Panel1" runat="server">
                <div class="grid-header-color" style="width: 100%">
                    &nbsp;</div>
                <div class="modal modal-static">
                    <div class="modal-header grid-header-color">
                        Instructions
                    </div>
                    <div class="modal-body">
                        <iframe id="aepFrame" runat="server" frameborder="0" name="newsFrame" src="aep.htm"
                            style="background-color: White; height: 300px; width: 100%; font-family: Arial, Helvetica, sans-serif;">
                        </iframe>
                    </div>
                </div>
            </asp:Panel>
            <div id="orderedPartsDiv" runat="server">
                <asp:GridView ID="partsOrderGridView" runat="server" AutoGenerateColumns="False" OnRowDataBound="partsOrderGridView_RowDataBound"
                    CellPadding="3" CssClass="table table-condensed" DataSourceID="partsOrderDataSource" 
                    ShowFooter="True" DataKeyNames="PartReference" OnSelectedIndexChanged="partsOrderGridView_SelectedIndexChanged"
                    OnRowCommand="partsOrderGridView_RowCommand" GridLines="None">
                    <EmptyDataTemplate>
                        <div style="text-align: center">
                            No Parts Have Been Ordered
                        </div>
                    </EmptyDataTemplate>
                    <SelectedRowStyle CssClass="grid-row-color-selected" />
                    <HeaderStyle CssClass="grid-header-color" />
                    <AlternatingRowStyle CssClass="grid-alt-row-color" />
                    <EmptyDataRowStyle CssClass="grid-header-color" Width="100%" />
                    <Columns>
                        <asp:TemplateField HeaderText="Ordered" SortExpression="Code">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Code") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <HeaderTemplate>
                                <div>
                                    Ordered</div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div>
                                    <div class="pull-left">
                                        <asp:LinkButton ID="LinkButton9" runat="server" CommandArgument='<%# Eval("PartReference") %>'
                                            CommandName="Select" Text='<%# Eval("Code") %>'></asp:LinkButton><br />
                                        <asp:Label ID="Label2" runat="server" Text='<%# Eval("Description") %>'></asp:Label>
                                        <br />
                                        Order Number <asp:Label ID="Label16" Font-Bold="true" runat="server" Text='<%# Eval("PartReference") %>'></asp:Label> 
                                    </div>
                                         
                                </div> 
                                    <div class="pull-right"  style="width :65%">
                                         <div id="Div1"  class="pull-left" runat="server" visible='<%# ((bool)Eval("IsSony") == true) %>' style="vertical-align:central">
                                  <br /> <asp:LinkButton ID="BtnBookCourier"      runat="server" Text="Book Courier Collection"  OnCommand="BtnBookCourier_Click" 
                                      CssClass="btn btn-warning" Visible='<%# ((bool)this.SonyStatusComplete== true)&& ((bool)Eval("NeedCollection") ) %>'    CommandArgument='<%# Eval("ReturnReference")%>'/>       
                                      <br />
                                 <%--<%# ((bool)(Eval("IsSony")))%>
                                             <%{ %>   --%> 
                              <%# ((bool)(Eval("IsSony")))? ( String.IsNullOrEmpty(Eval("Collectionref").ToString())?"":"Collection ref:"+Eval("Collectionref").ToString()):"" %>
                                     <br />
                                       <%# ((bool)(Eval("IsSony")))?  (String.IsNullOrEmpty(Eval("CollectionDate").ToString())?"":"Collection booked for:"+Eval("CollectionDate","{0:d}")):"" %>
                                     
                                         <%--    <%} %>--%>
                                         </div>  <div  class="pull-right" style="width:270px">
                                        <asp:Label ID="Label11" Font-Size="Smaller" runat="server" Visible='<%# Eval("ReturnRequired").ToString() == "True" %>'
                                            Text='RMA Required.'></asp:Label>
                                        <asp:Label ID="Label91" Font-Size="Smaller" runat="server" Text='<%# Eval("ReturnDescription") %>'></asp:Label>
                                        <br />                         
                                        <asp:Label ID="Label13" runat="server" Text='<%# Eval("OrderReference") %>' Visible='<%# Eval("OrderReference") != null && Eval("OrderReference").ToString() != "" %>'
                                            Font-Size="Smaller"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label92" runat="server" Visible='<%# Eval("ReturnReference") != null && Eval("ReturnReference").ToString() != "" %>'
                                            Font-Size="Smaller">Return Ref.:</asp:Label>
                                        <asp:Label ID="Label90" runat="server" Text='<%# Eval("ReturnReference") %>' Visible='<%# Eval("ReturnReference") != null && Eval("ReturnReference").ToString() != "" %>'
                                            Font-Size="Smaller"></asp:Label>
                                                                            <br />
                                        <asp:HyperLink ID="Label14" runat="server" Text="Show RMA PDF document" NavigateUrl='<%# Eval("RmaDocumentUrl") %>' Target="_blank" 
                                                        Visible='<%# Eval("RmaDocumentUrl") != null && Eval("RmaDocumentUrl").ToString() != "" %>'>
                                       </asp:HyperLink>
                                         </div>    
                                    </div>
                                </div>
                               <%-- <div class="right">
                                    <asp:Label ID="Label8" runat="server"  text="hi"
                                        Font-Bold="True" Font-Size="Smaller">Courier Ref.:</asp:Label>
                                   
                                </div>--%>
                            </ItemTemplate>
                            <FooterStyle CssClass="grid-footer-color-slim" />
                            <HeaderStyle CssClass="grid-header-color" HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qty" SortExpression="Quantity">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Quantity") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <HeaderTemplate>
                                <div class="right">
                                    Qty</div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div class="right">
                                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("Quantity") %>'></asp:Label></div>
                            </ItemTemplate>
                            <FooterStyle CssClass="grid-footer-color-slim" />
                            <HeaderStyle CssClass="grid-header-color" HorizontalAlign="Right" />
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Price" SortExpression="UnitPrice">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("UnitPrice") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <HeaderTemplate>
                                <div class="right">
                                    Price</div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div class="right">
                                    <asp:Label ID="Label3" runat="server" Text='<%# Bind("UnitPrice", "{0:f2}") %>'></asp:Label></div>
                            </ItemTemplate>
                            <FooterStyle CssClass="grid-footer-color-slim" />
                            <HeaderStyle CssClass="grid-header-color" HorizontalAlign="Right" />
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Status" SortExpression="Status">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("Status") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <HeaderTemplate>
                                <div class="right">
                                    Status</div>
                            </HeaderTemplate>
                            <ItemTemplate>

                                <div class="right">
                                    <asp:Label ID="Label4" runat="server" Text='<%# Bind("Status") %>'></asp:Label>
                                </div>
                                <div class="right">
                                    <asp:Label ID="Label9" runat="server" Visible='<%# ((DateTime?)Eval("DispatchDate")).HasValue %>'
                                        Font-Bold="True" Font-Size="Smaller">Dispatched:</asp:Label>
                                    <asp:Label ID="Label10" runat="server" Text='<%# Eval("DispatchDate", "{0:d}") %>'
                                        Visible='<%# ((DateTime?)Eval("DispatchDate")).HasValue %>' Font-Size="Smaller"></asp:Label>
                                </div>
                                <div class="right">
                                    <asp:Label ID="Label7" runat="server" Text="Delivery No.:" Visible='<%# Eval("DeliveryNumber").ToString().Trim() != "" %>'
                                        Font-Bold="True" Font-Size="Smaller"></asp:Label>
                                    <asp:Label ID="Label5" runat="server" Text='<%# Bind("DeliveryNumber") %>' Visible='<%# Eval("DeliveryNumber").ToString().Trim() != "" %>'
                                        Font-Size="Smaller"></asp:Label>
                                </div>
                                <div class="right">
                                    <asp:Label ID="Label8" runat="server" Visible='<%# Eval("CourierReference").ToString().Trim() != "" %>'
                                        Font-Bold="True" Font-Size="Smaller">Courier Ref.:</asp:Label>
                                    <asp:Label ID="Label6" runat="server" Text='<%# Bind("CourierReference") %>' Visible='<%# Eval("CourierReference").ToString().Trim() != "" %>'
                                        Font-Size="Smaller"></asp:Label>
                                </div>
                            </ItemTemplate>
                            <FooterStyle CssClass="grid-footer-color-slim" />
                            <HeaderStyle CssClass="grid-header-color" HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                    </Columns>
                    <RowStyle CssClass="grid-row-color" />
                </asp:GridView>
                <div align="left">
                    <asp:Button ID="usePartLinkButton" runat="server" OnClick="usePartLinkButton_Click" OnClientClick="DisableAllocate()" 
                        Text="Allocate" CssClass="btn btn-primary" />
                    &nbsp;
                    <asp:Button ID="cancelOrderLinkButton" runat="server" OnClick="cancelOrderLinkButton_Click"
                        Text="Cancel" CssClass="btn btn-danger" Visible="False" />
                    &nbsp;
                    <asp:Button ID="orderLinkButton" runat="server" CssClass="btn btn-inverse" OnClick="orderLinkButton_Click"
                        Text="Order" />
                    &nbsp;
                    <asp:Button ID="partEnquiryButton" runat="server" CssClass="btn" OnClick="partEnquiryButton_Click"
                        Text="Enquiry" />
                    &nbsp;
                    <asp:Button ID="partReturnButton" runat="server" CssClass="btn btn-warning" OnClick="partReturnButton_Click"
                        Text="Return Part" Visible='<%# ((bool)Eval("IsSony") == true) %>' /> &nbsp; 
                 
                    <asp:Button ID="btnDeletePartFromSony" runat="server" CssClass="btn btn-warning"  
                        Text="DELETE/RESUBMIT PART IN SONY" OnClick="btnDeletePartFromSony_Click" Visible='<%# ((bool)Eval("IsSony") == true) %>'/> 
                     &nbsp;   
               </div>
               <br />
            </div>
            <asp:ObjectDataSource ID="partsOrderDataSource" runat="server" OnObjectCreating="partsOrderDataSource_ObjectCreating"
                SelectMethod="Select" TypeName="Mobile.Portal.BLL.PartsBLL"></asp:ObjectDataSource>
            <div id="actionPartsDiv" runat="server">
                <asp:GridView ID="partsActionGridView" runat="server" AutoGenerateColumns="False"
                    CellPadding="3" CssClass="table table-condensed" DataSourceID="partsActionDataSource"
                    ShowFooter="True" DataKeyNames="PartReference" OnSelectedIndexChanged="partsActionGridView_SelectedIndexChanged"
                    OnRowCommand="partsActionGridView_RowCommand" GridLines="None">
                    <SelectedRowStyle CssClass="grid-row-color-selected" />
                    <HeaderStyle CssClass="grid-header-color" />
                    <AlternatingRowStyle CssClass="grid-alt-row-color" />
                    <EmptyDataRowStyle Width="100%" CssClass="grid-header-color" />
                    <Columns>
                        <asp:TemplateField HeaderText="Require Action" SortExpression="Code">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Code") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <HeaderTemplate>
                                <div>
                                    Require Action</div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div>
                                    <asp:LinkButton ID="LinkButton10" runat="server" CommandArgument='<%# Eval("PartReference") %>'
                                        CommandName="Select" Text='<%# Eval("Code") %>'></asp:LinkButton>
                                    <br />
                                    <asp:Label ID="Label3" runat="server" Text='<%# Eval("Description") %>'></asp:Label>
                                    <br />
                                    Order Number <asp:Label ID="Label16" Font-Bold="true" runat="server" Text='<%# Eval("PartReference") %>'></asp:Label> 
                                </div>
                            </ItemTemplate>
                            <FooterStyle CssClass="grid-footer-color-slim" />
                            <HeaderStyle CssClass="grid-header-color" HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qty" SortExpression="Quantity">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Quantity") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <HeaderTemplate>
                                <div class="right">
                                    Qty</div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div class="right">
                                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("Quantity") %>'></asp:Label></div>
                            </ItemTemplate>
                            <FooterStyle CssClass="grid-footer-color-slim" />
                            <HeaderStyle CssClass="grid-header-color" HorizontalAlign="Right" />
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Price" SortExpression="UnitPrice">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("UnitPrice") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <HeaderTemplate>
                                <div class="right">
                                    Price</div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div class="right">
                                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("UnitPrice", "{0:f2}") %>'></asp:Label></div>
                            </ItemTemplate>
                            <FooterStyle CssClass="grid-footer-color-slim" />
                            <HeaderStyle CssClass="grid-header-color" HorizontalAlign="Right" />
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Status" SortExpression="Status">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("Status") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <HeaderTemplate>
                                <div class="right">
                                    Status</div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div class="right">
                                    <asp:Label ID="Label4" runat="server" Text='<%# Bind("Status") %>'></asp:Label></div>
                            </ItemTemplate>
                            <FooterStyle CssClass="grid-footer-color-slim" />
                            <HeaderStyle CssClass="grid-header-color" HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                    </Columns>
                    <RowStyle CssClass="grid-row-color" />
                </asp:GridView>
                <div align="left">
                    <asp:Button ID="acceptPartLinkButton" runat="server" OnClick="acceptPartLinkButton_Click"
                        Text="Accept" CssClass="btn btn-success" />
                    &nbsp;
                    <asp:Button ID="rejectPartLinkButton" runat="server" OnClick="rejectPartLinkButton_Click"
                        Text="Reject" CssClass="btn btn-danger" />
                </div>
                <br />
            </div>
            <asp:ObjectDataSource ID="partsActionDataSource" runat="server" OnObjectCreating="partsActionDataSource_ObjectCreating"
                SelectMethod="Select" TypeName="Mobile.Portal.BLL.PartsBLL"></asp:ObjectDataSource>
            <div id="cancelledPartsDiv" runat="server">
                <asp:GridView ID="partsCancelGridView" runat="server" AutoGenerateColumns="False"
                    CellPadding="3" CssClass="table grid-footer-color-slim" DataSourceID="partsCancelDataSource"
                    ShowFooter="True" DataKeyNames="PartReference" OnSelectedIndexChanged="partsCancelGridView_SelectedIndexChanged"
                    OnRowCommand="partsCancelGridView_RowCommand" GridLines="None">
                    <FooterStyle CssClass="grid-footer-color-slim" />
                    <SelectedRowStyle CssClass="grid-row-color-selected" />
                    <HeaderStyle CssClass="grid-header-color" />
                    <AlternatingRowStyle CssClass="grid-alt-row-color" Font-Overline="False" />
                    <EmptyDataRowStyle Width="100%" CssClass="grid-header-color" />
                    <Columns>
                        <asp:TemplateField HeaderText="Cancelled" SortExpression="Code">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Code") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <HeaderTemplate>
                                <div>
                                    Cancelled</div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div>
                                    <asp:LinkButton ID="LinkButton11" runat="server" CommandArgument='<%# Eval("PartReference") %>'
                                        CommandName="Select" Text='<%# Eval("Code") %>'></asp:LinkButton>
                                    <br />
                                    <asp:Label ID="Label4" runat="server" Text='<%# Eval("Description") %>'></asp:Label>
                                    <br />
                                    Order Number <asp:Label ID="Label16" runat="server" Text='<%# Eval("PartReference") %>'></asp:Label> 
                                </div>
                            </ItemTemplate>
                            <FooterStyle CssClass="grid-footer-color-slim" />
                            <HeaderStyle CssClass="grid-header-color" HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qty" SortExpression="Quantity">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Quantity") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <HeaderTemplate>
                                <div class="right">
                                    Qty</div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div class="right">
                                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("Quantity") %>'></asp:Label></div>
                            </ItemTemplate>
                            <FooterStyle CssClass="grid-footer-color-slim" />
                            <HeaderStyle CssClass="grid-header-color" HorizontalAlign="Right" />
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Price" SortExpression="UnitPrice">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("UnitPrice") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <HeaderTemplate>
                                <div class="right">
                                    Price</div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div class="right">
                                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("UnitPrice", "{0:f2}") %>'></asp:Label></div>
                            </ItemTemplate>
                            <FooterStyle CssClass="grid-footer-color-slim" />
                            <HeaderStyle CssClass="grid-header-color" HorizontalAlign="Right" />
                            <ItemStyle HorizontalAlign="Right" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Status" SortExpression="Status">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("Status") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <HeaderTemplate>
                                <div class="right">
                                    Status</div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div class="right">
                                    <asp:Label ID="Label3" runat="server" Text='<%# Bind("Status") %>'></asp:Label></div>
                            </ItemTemplate>
                            <FooterStyle CssClass="grid-footer-color-slim" />
                            <HeaderStyle CssClass="grid-header-color" HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                    </Columns>
                    <RowStyle CssClass="grid-row-color" />
                </asp:GridView>
                <div align="left">
                    <asp:Button ID="viewCancelledLinkButton" runat="server" OnClick="viewCancelledLinkButton_Click"
                        Text="View" CssClass="btn" />
                    <br />
                </div>
            </div>
            <asp:ObjectDataSource ID="partsCancelDataSource" runat="server" OnObjectCreating="partsCancelDataSource_ObjectCreating"
                SelectMethod="Select" TypeName="Mobile.Portal.BLL.PartsBLL"></asp:ObjectDataSource>
          </div>
              <div runat="server" id="divRMAOutput" style="text-align:center; padding:8px; align-content:center"  >
                        <p runat="server" id="titleRMA" style="font-weight:bold;" ></p>                                           
                        <asp:Repeater runat="server" ID="repeaterRMA" EnableTheming="true"  >
                            <ItemTemplate><div style="float:inherit">
                                <table style="text-align:left;width:50%;margin: auto;" >     
                                    <tr class="grid-header-color">
                                        <td class="grid-column-label" width="50%">
                                            <asp:Label ID="Label2" runat="server">Response - Part Code: <%# Eval("StockCode") %></asp:Label>
                                        </td>
                                        <td class="grid-column-data">
                                            &nbsp;
                                        </td>
                                    </tr>                                                                            
                                    <%--<tr class="grid-row-color"><td class="grid-column-label"><strong>Stock Code</strong></td><td class="grid-column-data"> <%# Eval("StockCode") %> </td></tr>  --%>          
                                    <tr class="grid-alt-row-color"><td class="grid-column-label"><strong>Success</strong></td><td class="grid-column-data"> <%# Eval("Success") %> </td></tr>
                                    <tr class="grid-row-color" style='<%# bool.Parse(Eval("IsError").ToString()) ? "" : "display:none;" %>'><td class="grid-column-label"><strong>Error Message</strong></td><td class="grid-column-data" style="color:red;"><%# Eval("ErrorMessage") %> </td></tr> 
                                    <tr class="grid-alt-row-color" style='<%# bool.Parse(Eval("IsError").ToString()) ? "" : "display:none;" %>'><td class="grid-column-label"><strong>Validation Errors</strong></td><td class="grid-column-data" style="color:red;"><%# Eval("ErrorList") %> </td></tr>
                                            
                                    <tr class="grid-row-color" style='<%# !bool.Parse(Eval("IsError").ToString()) ? "" : "display:none;" %>'><td class="grid-column-label"><strong>Validation Status</strong></td><td class="grid-column-data""><%# Eval("ValidationStatus") %> </td></tr>  
                                    <tr class="grid-alt-row-color" style='<%# !bool.Parse(Eval("IsError").ToString()) ? "" : "display:none;" %>'><td class="grid-column-label"><strong>Shipment Status</strong></td><td class="grid-column-data"> <%# Eval("ShipmentStatus") %> </td></tr>      
                                    <tr class="grid-row-color" style='<%# !bool.Parse(Eval("IsError").ToString()) ? "" : "display:none;" %>'><td class="grid-column-label"><strong>Document URL</strong></td><td class="grid-column-data""><%# Eval("DocumentURL") %> </td></tr>  
                                    <tr class="grid-alt-row-color" style='<%# !bool.Parse(Eval("IsError").ToString()) ? "" : "display:none;" %>'><td class="grid-column-label"><strong>RMA</strong></td><td class="grid-column-data"> <%# Eval("Rma") %> </td></tr>         
                                                                                   
                               </table></div>
                                <br />
                            </ItemTemplate> 
                        </asp:Repeater>  
                  <div class="modal-footer">
                        <asp:Button ID="okLinkButton" runat="server" CssClass="btn" OnClick="okLinkButton_Click" CausesValidation="false" 
                            Text="OK"/><%--   --%>
                    </div>                          
              </div>
               <div runat="server" id="divSWAP2CREDITOutput" style="text-align:center; padding:0px; align-content:center"  visible="false">
                        <p runat="server" id="P1" title="Result of swap to credit" style="font-weight:bold;" ></p> 
                   <table>
                       <tr>
                 <td class="grid-column-label" width="40%">  <asp:Label ID="LblSwap2Credittoolid"   text="Solution Request Reference" runat="server" ></asp:Label></td><td><asp:Label ID="LblTextSwap2Credit"     runat="server" ></asp:Label>
              </td></tr><tr> <td class="grid-column-label" width="30%" >    <asp:Label ID="LblSwap2CreditClientref" text="Call reference"  runat="server" ></asp:Label></td><td><asp:Label ID="txtLblSwap2CreditClientref"     runat="server" ></asp:Label><br />
               </td></tr><tr><td class="grid-column-label" width="30%" >    <asp:Label ID="LblSwap2Creditresult" text="Response"  runat="server" ></asp:Label></td><td><asp:Label Font-Bold="true" ForeColor="Red" ID="txtSwap2Creditresult"     runat="server" ></asp:Label><br />
                </td></tr><%--<tr> <td class="grid-column-label" width="50%" >  <asp:Label ID="LblSwap2CreditRMARef" text="RMA ref"  runat="server" ></asp:Label></td><td><asp:Label ID="txtSwap2CreditRMARef"     runat="server" ></asp:Label><br />
                 </td></tr><tr><td class="grid-column-label" width="50%">  <asp:Label ID="LblSwap2CreditDocumenturl"  text="Return Document"  runat="server" ></asp:Label></td><td><asp:Label ID="txtSwap2CreditDocumenturl"     runat="server" ></asp:Label><br />
     </td>
                      </tr>--%></table> 
                     <asp:Button ID="btnref" runat="server" CssClass="btn" OnClick="okLinkButton_Click" CausesValidation="false" 
                            Text="OK"/>     
        </div>
    </div>

    <div >
        <asp:Panel runat="server" ID="pnl1" class="popupConfirmation">
            <iframe id="Iframe" runat="server"  width="900" height="600"></iframe><%-- src="Collectionjob.aspx"--%>
        </asp:Panel>
      <asp:LinkButton ID="LinkButton1" Text="Book Courier" runat="server"  OnClientClick="JavaScript: return false;" Style="display: none;"></asp:LinkButton><%----%>
        <cc1:ModalPopupExtender BackgroundCssClass="ModalPopupBG" ID="ModalPopupExtender1" OkControlID="btnOkay" CancelControlID="btnbookingCancelWindow"  runat="server"
            PopupControlID="pnl1" TargetControlID="LinkButton1">
        </cc1:ModalPopupExtender>
         <div class="popup_Buttons" style="display: none">
                <input id="btnOkay" value="Done" type="button" />
                <input id="btnbookingCancelWindow" value="Cancel" type="button" onclick="window.open('parts.aspx', '_self');"  />
            </div>
    </div>
 
    </div>
 
    </div>
 
</asp:Content>
