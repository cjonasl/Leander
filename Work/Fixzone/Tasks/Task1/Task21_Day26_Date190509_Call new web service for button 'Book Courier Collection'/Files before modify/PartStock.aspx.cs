using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mobile.Portal.BLL;
using Mobile.Portal.Session;
using Mobile.Portal.Utilities;
using Mobile.Portal.Classes;
using System.Text;

namespace MobilePortal
{
    public partial class PartStock : System.Web.UI.Page
    {
        PartsBLL orderPartsBLL = new PartsBLL();
        int maxParts;

        protected void Page_Load(object sender, EventArgs e)
        {
            SiteSession session = SiteSessionFactory.LoadSession(this.Page);           

            if (!session.LoginAccepted)
            {
                Response.Redirect("~/Denied.aspx");
            }
            else
            {               
                //if (!Page.IsPostBack)
                //{
                    Call call = new Call();

                    maxParts = int.Parse(ConfigUtils.GetAppSetting("maxParts", 10));

                    if (session.Data.ContainsKey("call"))
                    {
                        session.Data["call"] = call;
                    }
                    else
                    {
                        session.Data.Add("call", call);
                    }
                    SiteSessionFactory.SaveSession(this.Page, session);                                       
                //}

                    orderPartsBLL.List = BindOrderParts(session.Login.SaediId); 
                    orderPartsBLL.List.Reverse();
                    partsOrderGridView.DataBind();
                // SetButton();                
            }
        }

        private List<CallPart> BindOrderParts(string SaediId)
        {
         List <CallPart> OrderedParts=   orderPartsBLL.GetPartsByClientId(SaediId);
         if (TxtStockCode.Text == String.Empty && TxtSonNumber.Text == string.Empty && TxtStockDesc.Text == string.Empty)
             return OrderedParts;
         else
         {
             List<CallPart> FilteredOrderedParts = OrderedParts;
             if (TxtStockCode.Text != string.Empty)
                 FilteredOrderedParts = FilteredOrderedParts.Where(x => x.Code.ToUpper().Contains(TxtStockCode.Text.ToUpper())).ToList();
             if (TxtSonNumber.Text != string.Empty)
                 FilteredOrderedParts = FilteredOrderedParts.Where(x => x.OrderReference.ToUpper().Contains(TxtSonNumber.Text.ToUpper())).ToList();
             if (TxtStockDesc.Text != string.Empty)
                 FilteredOrderedParts = FilteredOrderedParts.Where(x => x.Description.ToUpper().Contains(TxtStockDesc.Text.ToUpper())).ToList();
             return FilteredOrderedParts;
         }
        }

        protected void partReturnButton_Click(object sender, EventArgs e)
        {
            CallPartView(((Button)sender).CommandArgument, orderPartsBLL.List, PartAction.Return);
        }

        private void CallPartView(string commandArgument, List<CallPart> basket, PartAction editMode)
        {
            SiteSession session = SiteSessionFactory.LoadSession(this.Page);
            CallPart part = basket.Find(f => f.PartReference.ToString() == commandArgument);
            if (part != null)
            {
                if (session.Data.ContainsKey("part"))
                { session.Data["part"] = orderPartsBLL.Clone(part); }
                else
                { session.Data.Add("part", orderPartsBLL.Clone(part)); }
                SiteSessionFactory.SaveSession(this.Page, session);
                Response.Redirect("~/PartReturn.aspx?from=stock");                
            }
        }

        //private void SetButton()
        //{
        //    CallPart part = orderPartsBLL.List.Find(delegate(CallPart cp) { return cp.PartReference.ToString() == partReturnButton.CommandArgument.ToString(); });

        //    string partDescription;
        //    try
        //    {
        //        partDescription = part.ReturnDescription;
        //    }
        //    catch
        //    {
        //        partDescription = string.Empty;
        //    }
        //    partReturnButton.Enabled = (part != null && part.ReturnRequired) || (partDescription != string.Empty); // added peter 15.04.2013
        //}

        protected void partsOrderGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                partReturnButton.CommandArgument = e.CommandArgument.ToString();
                // SetButton();
            }

            else if (e.CommandName == "BookCourier")
            {
                SiteSession session = SiteSessionFactory.LoadSession(this.Page);
               
                string rmaId = e.CommandArgument.ToString();
                string queryString = string.Format("Collectionjob.aspx?SAEDIID={0}&RMAList={1}&loop=true", session.Login.SaediId, rmaId );


                Iframe.Attributes.Add("src", queryString);

                ModalPopupExtender1.Show();
            }
        }

        protected void partsOrderDataSource_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            e.ObjectInstance = orderPartsBLL;
        }

        protected void orderLinkButton_Click(object sender, EventArgs e)
        {
            SiteSession session = SiteSessionFactory.LoadSession(this.Page);
            session.Data.Remove("StockAddSearch");
            session.Data.Remove("PartStockNote");
            session.Data.Add("PartStockNote", true);
            SiteSessionFactory.SaveSession(this.Page, session);

            Response.Redirect("~/PartSearch.aspx");
        }

        protected void partsOrderGridView_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void cancelOrderLinkButton_Click(object sender, EventArgs e)
        {

        }
        
        protected void BtnCancel_Click(object sender, EventArgs e)
        {
            TxtSonNumber.Text = string.Empty;
            TxtStockCode.Text = string.Empty;
            TxtStockDesc.Text = string.Empty;

            SiteSession session = SiteSessionFactory.LoadSession(this.Page);
            orderPartsBLL.List = BindOrderParts(session.Login.SaediId);
            orderPartsBLL.List.Reverse();
            partsOrderGridView.DataBind();
        }

        protected void partsOrderGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            SiteSession session = SiteSessionFactory.LoadSession(this.Page);
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
              
              string son=(string)DataBinder.Eval(e.Row.DataItem, "OrderReference");
              List<RMARef> result = orderPartsBLL.GetPartsRMADetails(son, session.Login.SaediId);
                StringBuilder returnResult= new StringBuilder();
                  StringBuilder returnCourierResult= new StringBuilder();
                  LinkButton BtnBookCourier = (LinkButton)(e.Row.FindControl("BtnBookCourier"));
                foreach( RMARef item in result)
                {
                    if (!string.IsNullOrEmpty(item.rmaDocumentUrl) && string.IsNullOrEmpty(item.Collectionref))
                    {
                        BtnBookCourier.Visible = true;
                        BtnBookCourier.CommandArgument = string.IsNullOrEmpty(BtnBookCourier.CommandArgument) ? item.rmaId : BtnBookCourier.CommandArgument + "," + item.rmaId;
                    }
                    returnResult.AppendFormat("{3}. Return Ref: {1} ;{2}<br/> <a href='{0}' target='_blank' style='{4}'>Show RMA Document</a><br/>", item.rmaDocumentUrl, item.rmaId, item.shipmentStatus, (result.IndexOf(item)) + 1, string.IsNullOrEmpty(item.rmaDocumentUrl)? "visibility: hidden":"");
                    if(!string.IsNullOrEmpty(item.Collectionref))
                        returnCourierResult.AppendFormat(" Collection Ref : {0}  {1} <br/>",item.Collectionref,item.CollectionDate);
                }
              Label lbl = (Label)(e.Row.FindControl("ReturnDetails"));
              Label lblCourierDetails = (Label)(e.Row.FindControl("CourierDetails"));
              lbl.Text = returnResult.ToString();
              lblCourierDetails.Text = returnCourierResult.ToString();
                
            }  
        //    List<CallPart> partsResult= orderPartsBLL.GetPartsByClientId(_id);
        //    foreach(  CallPart part in  partsResult)
        //    {
        //        List<RMARef> result = _RMAdal.GetPartsRMADetails(part.OrderReference, _id);
        //        part.RmaDetails = result;
        //    }
        //ReturnDetails
        }
    }
}