<%@ Page Title="" Language="C#" MasterPageFile="~/mobile.Master" AutoEventWireup="true"
    CodeBehind="PartStock.aspx.cs" Inherits="MobilePortal.PartStock" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="OrderedPartSearch" runat="server"> <%--Stock Part Search--%>
        <table title="Stock Part Search"  width="100%" frame="hsides">
            <caption align="Top" style="font-weight:bold;height:25px">Search</caption>
          <tr><td><asp:Label runat="server" ID="LblStockCode" Text ="Stock Code"/></td>
              <td><asp:Label runat="server" ID="LblDescription" Text ="Stock Desc."/></td>
              <td><asp:Label runat="server" ID="LblSonNo" Text="SON Number"/></td>
          </tr>
            <tr >
                <td><asp:TextBox runat="server" ID="TxtStockCode"  /></td>
                <td><asp:TextBox runat="server" ID="TxtStockDesc"  /></td>
                 <td><asp:TextBox runat="server" ID="TxtSonNumber"  /></td>
            </tr>
            <tr></tr>
            <tr style="height:50px">               
                <td colspan="3" align="center"><asp:Button ID="BtnSearch" runat="server"   Text ="Search" />&nbsp;&nbsp;&nbsp;<asp:Button ID="BtnCancel" runat="server" Text ="Reset"  OnClick="BtnCancel_Click"/></td>
            </tr>  
           
        </table>

    </div>
    <div>&nbsp;</div>
    <div id="orderedPartsDiv" runat="server">
        <asp:GridView ID="partsOrderGridView" runat="server" AutoGenerateColumns="False"
            CellPadding="3" CssClass="table table-condensed" DataSourceID="partsOrderDataSource"
            ShowFooter="True" DataKeyNames="PartReference" OnSelectedIndexChanged="partsOrderGridView_SelectedIndexChanged" OnRowDataBound="partsOrderGridView_RowDataBound"
            OnRowCommand="partsOrderGridView_RowCommand" AllowPaging="True" GridLines="None">

             <PagerSettings FirstPageImageUrl="~/image/firstGridButton.png" LastPageImageUrl="~/image/lastGridButton.png"
                NextPageImageUrl="~/image/nextGridButton.png" Position="TopAndBottom" PreviousPageImageUrl="~/image/prevGridButton.png"
                PageButtonCount="5" />

            <RowStyle CssClass="grid-row-color" VerticalAlign="Middle" />

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
                        <div >
                            <div class="pull-left">
<%--                                <asp:LinkButton ID="LinkButton9" runat="server" CommandArgument='<%# Eval("PartReference") %>'  
                                    CommandName="Select" Text='<%# Eval("Code") %>'></asp:LinkButton>--%>
                                    
                                <asp:LinkButton ID="LinkButton9" runat="server" Text='<%# Eval("Code") == null || Eval("Code").ToString() == "" ? "<span style=\"color:red\">(blank)</span>" : Eval("Code").ToString() %>'
                                    CommandArgument='<%# Eval("PartReference") %>' CommandName="Select"></asp:LinkButton>   

                                <asp:Label ID="LabelPartNote" runat="server" Text='<%# Eval("PartNote") %>'></asp:Label>
                                    
                                <br />
                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("Description") %>'></asp:Label>
                            </div>
                            <div  class="pull-right" style="width:55%" >  <div id="Div1"  class="pull-left" runat="server"  style="vertical-align:central">
                                  <br /> 
                                <asp:LinkButton ID="BtnBookCourier"      runat="server" Text="Book Courier Collection"     CommandName="BookCourier"   CausesValidation="false" 
                                      CssClass="btn btn-warning"   Visible="false" />    <%--Visible='<%# ((bool)Eval("NeedCollection") ) %>'CommandArgument='<%# Eval("ReturnReference")%>'--%>   
                                      <br />
                                   <asp:Label runat="server" ID="CourierDetails" ></asp:Label>                                 
                         <%-- 
                                  <asp:Label ID="LblCollectRef"      runat="server" Text='<%# Eval("Collectionref") %>'
                                         /> <br />          
                                         <asp:Label ID="LblCollectionDate"      runat="server"   Text='<%# Eval("CollectionDate","{0:dd/M/yyyy}") %>'
                                         />    --%> 
                                </div> 
                            <div class="pull-right">
                                <asp:Label runat="server" ID="ReturnDetails" ></asp:Label>
                        <%--     <% foreach(Mobile.Portal.Classes.RMARef rmaRef in (List<Mobile.Portal.Classes.RMARef>)Eval("RmaDetails"))
                                {%>
                                   <%-- <asp:Label ID="Label11" runat="server" Font-Size="Smaller"  Visible='<% rmaRef.shipmentStatus.ToString() == "True" %>'
                                    Text='RMA Required.'></asp:Label>
                                 <asp:Label ID="Label11" runat="server" Font-Size="Smaller" 
                                    Text='<% rmaRef.shipmentStatus.ToString() '></asp:Label>
                                <asp:Label ID="Label91" Font-Size="Smaller" runat="server" Text='<%# Eval("rmaDocumentUrl") %>'></asp:Label>      
                                <br />                         
                               
                                <br />
                             
                                <asp:Label ID="Label90" runat="server" Text='<% rmaRef.rmaId %>' 
                                    Font-Size="Smaller"></asp:Label>
                              <%   }
                                     %>
                                --%>  <asp:Label ID="Label13" runat="server" Text='<%# Eval("OrderReference") %>' Visible='<%# Eval("OrderReference") != null && Eval("OrderReference").ToString() != "" %>'
                                    Font-Size="Smaller"></asp:Label>
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
            <PagerStyle CssClass="grid-pager" />
        </asp:GridView>
        <div align="left">
            &nbsp;
            &nbsp;
            <asp:Button ID="orderLinkButton" runat="server" CssClass="btn btn-inverse" OnClick="orderLinkButton_Click"
                Text="Order" />
            &nbsp;
            <asp:Button ID="cancelOrderLinkButton" runat="server" OnClick="cancelOrderLinkButton_Click"
                Text="Cancel" CssClass="btn btn-danger" Visible="False" />
            &nbsp;
            <asp:Button ID="partReturnButton" runat="server" CssClass="btn btn-warning" OnClick="partReturnButton_Click"
                Text="Return Part" />
        </div>
        <br />
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
                <input id="btnbookingCancelWindow" value="Cancel" type="button" onclick="window.open('PartStock.aspx', '_self');"/>
            </div>
    </div>
    <asp:ObjectDataSource ID="partsOrderDataSource" runat="server" OnObjectCreating="partsOrderDataSource_ObjectCreating"
        SelectMethod="Select" TypeName="Mobile.Portal.BLL.PartsBLL"></asp:ObjectDataSource>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
</asp:Content>
