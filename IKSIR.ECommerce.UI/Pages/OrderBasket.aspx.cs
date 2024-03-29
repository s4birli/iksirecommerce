﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IKSIR.ECommerce.Model.MembershipModel;
using IKSIR.ECommerce.Model.Order;
using IKSIR.ECommerce.Infrastructure.DataLayer.ProductDataLayer;
using IKSIR.ECommerce.UI.ClassLibrary;
using IKSIR.ECommerce.Infrastructure.DataLayer.CommonDataLayer;

namespace IKSIR.ECommerce.UI.Pages
{
    public partial class OrderBasket : System.Web.UI.Page
    {
        public User loginUser = null;
        public Basket basket = null;

        public decimal BasketTotal = 0;
        public decimal TotalTax = 0;
        public decimal TotalPrice = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Kullanıcı login değilse sayfaya giremez.
            if (Session["LOGIN_USER"] != null)
            {
                if (!Page.IsPostBack)
                {
                    loginUser = (User)Session["LOGIN_USER"];
                    basket = (Basket)HttpContext.Current.Session["USER_BASKET"];
                    if (basket != null && basket.BasketItems != null && basket.BasketItems.Count > 0)
                    {
                        cbxComfirmation.Visible = true;
                        divConfirmation.Visible = true;
                        rptBasketProducts.Visible = true;
                        hplNoItem.Visible = false;
                        divBasketTotal.Visible = true;
                        divRules.Visible = true;
                        divBasketButtons.Visible = true;
                        GetOrderBasket();
                    }
                    else
                    {
                        cbxComfirmation.Visible = false;
                        divConfirmation.Visible = false;
                        rptBasketProducts.Visible = false;
                        hplNoItem.Visible = true;
                        divBasketTotal.Visible = false;
                        divRules.Visible = false;
                        divBasketButtons.Visible = false;
                    }
                    var item = StaticPageData.Get(10);
                    if (item != null)
                        divRules.InnerHtml = item.PageContent;
                }
            }
            else
            {
                Response.Redirect("../SecuredPages/Login.aspx?returl=../Pages/OrderBasket.aspx");
            }
        }

        private void GetOrderBasket()
        {
            basket = (Basket)HttpContext.Current.Session["USER_BASKET"];
            rptBasketProducts.DataSource = basket.BasketItems;
            rptBasketProducts.DataBind();

            lblBasketTotal.Text = Toolkit.Utility.CurrencyFormat(BasketTotal);
            lblTotalTax.Text = Toolkit.Utility.CurrencyFormat(TotalTax);
            lblTotalPrice.Text = Toolkit.Utility.CurrencyFormat(TotalPrice);
        }

        protected void rptBasketProducts_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Label lblUnitPrice = e.Item.FindControl("lblUnitPrice") as Label;
                Label lblBasketItemPrice = e.Item.FindControl("lblBasketItemPrice") as Label;

                decimal unitePrice = 0;
                decimal.TryParse(lblUnitPrice.Text, out unitePrice);
                lblUnitPrice.Text = Toolkit.Utility.CurrencyFormat(unitePrice);

                decimal basketItemPrice = 0;
                decimal.TryParse(lblBasketItemPrice.Text, out basketItemPrice);
                lblBasketItemPrice.Text = Toolkit.Utility.CurrencyFormat(basketItemPrice);

                basket = (Basket)HttpContext.Current.Session["USER_BASKET"];
                Image imgProduct = e.Item.FindControl("imgProduct") as Image;
                HiddenField hdnProductId = e.Item.FindControl("hdnProductId") as HiddenField;
                DropDownList ddlItemCount = e.Item.FindControl("ddlItemCount") as DropDownList;


                Repeater rptProductProperties = e.Item.FindControl("rptProductProperties") as Repeater;
                int productId;

                if (hdnProductId != null && rptProductProperties != null && hdnProductId.Value != "" && int.TryParse(hdnProductId.Value, out productId))
                {
                    var itemProduct = ProductData.Get(productId);

                    if (itemProduct != null)
                    {
                        for (int i = 1; i <= itemProduct.MaxQuantity; i++)
                        {
                            ddlItemCount.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        }
                    }

                    var product = basket.BasketItems.Where(x => x.Product.Id == productId).First();

                    if (product != null)
                    {
                        ddlItemCount.SelectedValue = product.Count.ToString();
                    }

                    rptProductProperties.DataSource = ProductPropertyData.GetProductProperties(productId);
                    rptProductProperties.DataBind();

                    var productPriceData = ProductPriceData.GetByProduct(productId);
                    if (productPriceData != null)
                    {
                        BasketTotal += product.Count * productPriceData.Price;
                        TotalTax += product.Count * productPriceData.UnitPrice * productPriceData.Tax / 100;
                        TotalPrice += product.Count * productPriceData.UnitPrice;
                     
                    }
                }
            }
        }

        protected void rptBasketProducts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                int productId;
                ImageButton imgbtnDeleteItem = (ImageButton)e.Item.FindControl("imgbtnDeleteItem");
                if (imgbtnDeleteItem != null && imgbtnDeleteItem.CommandArgument != null && int.TryParse(imgbtnDeleteItem.CommandArgument, out productId))
                {
                    Shopping.RemoveBasketItem(productId);
                }
            }
            GetOrderBasket();
        }

        protected void imgbtnContinue_Click(object sender, ImageClickEventArgs e)
        {
            basket = (Basket)HttpContext.Current.Session["USER_BASKET"];
            if (basket.BasketItems.Count == 0)
            {
                string textForMessage = @"<script language='javascript'> alert('Sepetinizde hiç ürün bulunmamaktadır. Devam etmek için sepetinize ürün ekleyiniz.');</script>";
                ClientScript.RegisterClientScriptBlock(this.GetType(), "UserPopup", textForMessage);
                return;
            }

            if (cbxComfirmation.Checked)
            {
                basket.TotalPrice = Convert.ToDecimal(lblTotalPrice.Text);
                basket.TotalRatedPrice = Convert.ToDecimal(lblBasketTotal.Text);
                Session.Add("USER_BASKET", basket);
                Response.Redirect("OrderAddress.aspx");
            }
            else
            {
                dvAlert.Visible = true;
                dvAlert.InnerHtml = "<span style=\"color:Red\">Genel kurallar ve koşulları kabul ediniz!</span><br />";

                //this.ClientScript.RegisterStartupScript(this.GetType(), "Koşulları okuyup onaylayınız!", "<script language=\"javaScript\">" + "alert('Genel kurallar ve koşulları kabul ediniz!');" + "window.location.href='OrderBasket.aspx';" + "<" + "/script>");
            }
        }

        protected void ddlItemCount_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlItemCount = (DropDownList)sender;
            int productId = 0;
            int itemCount = 0;
            int.TryParse(ddlItemCount.ToolTip, out productId);
            int.TryParse(ddlItemCount.SelectedValue, out itemCount);
            if (productId != 0 && itemCount != 0)
            {
                Shopping.AddToBasket(productId, itemCount);
                GetOrderBasket();
            }
        }
    }
}