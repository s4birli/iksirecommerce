﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IKSIR.ECommerce.Infrastructure.DataLayer.ProductDataLayer;
using IKSIR.ECommerce.Model.Order;
using IKSIR.ECommerce.Infrastructure.DataLayer.OrderDataLayer;
using IKSIR.ECommerce.Model.MembershipModel;
using IKSIR.ECommerce.Model.Bank;
using IKSIR.ECommerce.Infrastructure.DataLayer.BankDataLayer;
using IKSIR.ECommerce.Toolkit;
using IKSIR.ECommerce.Model.CommonModel;
using IKSIR.ECommerce.Infrastructure.DataLayer.CommonDataLayer;
using System.IO;
using _PosnetDotNetModule;
//using _PosnetDotNetTDSOOSModule;
using System.Net;
using System.Xml;
using System.Text;
using System.Security.Cryptography;

namespace IKSIR.ECommerce.UI.Pages
{
    public partial class OrderPayment : System.Web.UI.Page
    {
        public User loginUser = null;
        public Basket basket = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LOGIN_USER"] != null && Session["USER_BASKET"] != null)
            {
                if (!Page.IsPostBack)
                {
                    loginUser = (User)Session["LOGIN_USER"];
                    basket = (Basket)HttpContext.Current.Session["USER_BASKET"];
                    GetOrderBasket();
                }
            }
            else
            {
                Response.Redirect("../SecuredPages/Login.aspx?returl=../Pages/OrderPayment.aspx");
            }

            if (!Page.IsPostBack)
            {
                BindForm();
            }
        }

        private void BindForm()
        {
            List<TransferAccount> itemList = TransferAccountData.GetTransferAccountList();
            rblTransferAccount.DataTextField = "Detail";
            rblTransferAccount.DataValueField = "Id";
            rblTransferAccount.DataSource = itemList;
            rblTransferAccount.DataBind();

            ddlMonth.Items.Clear();
            for (int i = 1; i < 13; i++)
            {
                ddlMonth.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }

            ddlYear.Items.Clear();
            for (int i = 2011; i < 2030; i++)
            {
                ddlYear.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }

            List<EnumValue> ListPaymentType = EnumValueData.GetEnumValues(21);//Ödeme Tipleri
            Utility.BindDropDownList(ddlPaymentType, ListPaymentType, "Value", "Id");

            List<CreditCard> ListCreditCard = CreditCardData.GetCreditCardList();
            Utility.BindDropDownList(ddlCreditCard, ListCreditCard, "Name", "Id");

            ListItem ls = new ListItem("Diğer", "99");
            ddlCreditCard.Items.Add(ls);
        }

        private void GetOrderBasket()
        {
            loginUser = (User)Session["LOGIN_USER"];
            basket = (Basket)HttpContext.Current.Session["USER_BASKET"];
            rptBasketProducts.DataSource = basket.BasketItems;
            rptBasketProducts.DataBind();

            if (basket.TotalRatedPrice >= 100)
            {
                lblBasketTotal.Text = Toolkit.Utility.CurrencyFormat(basket.TotalRatedPrice);
                trShippingPrice.Visible = false;
            }
            else
            {
                trShippingPrice.Visible = true;
                decimal totaldesi = 0;

                foreach (var item in basket.BasketItems)
                {
                    if (item.Product.Desi != null && item.Product.Desi != "")
                        totaldesi += item.Count * Convert.ToDecimal(item.Product.Desi);
                }

                lblShippingPrice.Text = Utility.CurrencyFormat(OrderData.CalculateShippingPrice(totaldesi));
                lblBasketTotal.Text = Toolkit.Utility.CurrencyFormat(basket.TotalRatedPrice + OrderData.CalculateShippingPrice(totaldesi));
            }

            lblTotalTax.Text = Toolkit.Utility.CurrencyFormat(basket.TotalRatedPrice - basket.TotalPrice);
            lblTotalPrice.Text = Toolkit.Utility.CurrencyFormat(basket.TotalPrice);
        }

        protected void rptBasketProducts_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                loginUser = (User)Session["LOGIN_USER"];
                basket = (Basket)HttpContext.Current.Session["USER_BASKET"];
                Image imgProduct = e.Item.FindControl("imgProduct") as Image;
                HiddenField hdnProductId = e.Item.FindControl("hdnProductId") as HiddenField;
                DropDownList ddlItemCount = e.Item.FindControl("ddlItemCount") as DropDownList;

                Label lblUnitPrice = e.Item.FindControl("lblUnitPrice") as Label;
                Label lblBasketItemPrice = e.Item.FindControl("lblBasketItemPrice") as Label;

                decimal unitePrice = 0;
                decimal.TryParse(lblUnitPrice.Text, out unitePrice);
                lblUnitPrice.Text = Toolkit.Utility.CurrencyFormat(unitePrice);

                decimal basketItemPrice = 0;
                decimal.TryParse(lblBasketItemPrice.Text, out basketItemPrice);
                lblBasketItemPrice.Text = Toolkit.Utility.CurrencyFormat(basketItemPrice);



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
                }
            }
        }

        private void SaveOrder(PaymetInfo paymetInfo)
        {
            if (Session["LOGIN_USER"] != null && Session["USER_BASKET"] != null)
            {
                User loginUser = (User)Session["LOGIN_USER"];
                Basket basket = (Basket)HttpContext.Current.Session["USER_BASKET"];
                basket.Status = new Model.CommonModel.EnumValue() { Id = 29 };
                int retValue = 0;
                int basketId = 0;

                int billingAddressId = BasketAddressData.Insert(basket.BillingAddress, basketId, 0);
                int shippingAddressId = BasketAddressData.Insert(basket.ShippingAddress, basketId, 0);

                basket.BillingAddress.Id = billingAddressId;
                basket.ShippingAddress.Id = shippingAddressId;
                basket.ShippingCompany = new Model.ProductModel.Shipment() { Id = basket.ShippingCompany.Id };
                basketId = BasketData.Insert(basket);
                BasketAddressData.UpdateBasketId(billingAddressId, basketId, shippingAddressId);
                if (basketId > 0)
                    foreach (BasketItem basketItem in basket.BasketItems)
                    {
                        basketItem.Basket = new Basket() { Id = basketId };
                        basketItem.Status = new Model.CommonModel.EnumValue() { Id = 39 };
                        retValue = BasketItemData.Insert(basketItem);
                        if (retValue <= 0)
                        {
                            break;
                        }
                    }
                basket.Id = basketId;



                if (retValue > 0) //itemlar başarıyla kaydedildiyese
                {
                    paymetInfo.Cvc = "";
                    paymetInfo.CreditCardNumber = "";
                    retValue = PaymetInfoData.Insert(paymetInfo);
                }

                if (retValue > 0) //itemlar başarıyla kaydedildiyese
                {
                    Order order = new Order();
                    order.User = loginUser;
                    order.Basket = basket;
                    order.TotalPrice = basket.TotalPrice;
                    order.TotalRatedPrice = basket.TotalRatedPrice;
                    if (lblShippingPrice.Text != "")
                        order.ShippingPrice = Convert.ToDecimal(lblShippingPrice.Text);
                    else
                        order.ShippingPrice = 0;
                    if (ddlPaymentType.SelectedValue == "36")
                        order.Status = new Model.CommonModel.EnumValue() { Id = 29 };
                    else
                        order.Status = new Model.CommonModel.EnumValue() { Id = 32 };
                    order.PaymetInfo = new PaymetInfo() { Id = retValue };
                    retValue = OrderData.Insert(order);
                    order.Id = retValue;
                    Session.Add("USER_ORDER", order);

                }

                if (retValue > 0)
                {
                    string MailBody = File.ReadAllText(HttpContext.Current.Request.MapPath("~") + "/MailTemplates/OrderResultMail.htm");
                    MailBody = MailBody.Replace("%OrderID%", retValue.ToString());

                    MailBody = MailBody.Replace("%Taxamount%", lblTotalTax.Text);
                    MailBody = MailBody.Replace("%TotalAmount%", lblTotalPrice.Text);
                    MailBody = MailBody.Replace("%ShippingAmount%", lblShippingPrice.Text);
                    MailBody = MailBody.Replace("%TotalOrderAmount%", lblBasketTotal.Text);

                    string cityName = basket.BillingAddress.City != null ? basket.BillingAddress.City.Name : basket.BillingAddress.CityName;

                    string districtName = basket.BillingAddress.District != null ? basket.BillingAddress.District.Name : basket.BillingAddress.DistrictName;

                    MailBody = MailBody.Replace("%BillingAddress%", "İl : " + cityName + " </br>İlçe : " + districtName +
                        "</br> Adres : " + basket.BillingAddress.AddressDetail.ToString() + "</br>Posta Kodu : " + basket.BillingAddress.PostalCode.ToString());

                    cityName = basket.ShippingAddress.City != null ? basket.ShippingAddress.City.Name : basket.ShippingAddress.CityName;

                    districtName = basket.ShippingAddress.District != null ? basket.ShippingAddress.District.Name : basket.ShippingAddress.DistrictName;

                    MailBody = MailBody.Replace("%DeliveryAddress%", "İl : " + cityName + " </br>İlçe : " + districtName +
                      "</br> Adres : " + basket.ShippingAddress.AddressDetail.ToString() + "</br>Posta Kodu : " + basket.ShippingAddress.PostalCode.ToString());

                    MailBody = MailBody.Replace("%NameSurname%", loginUser.FirstName.ToString() + " " + loginUser.LastName.ToString());
                    string HtmlProducts = "<table><tr><td>Ürün Adı</td><td>Sayısı</td><td>Fiyatı</td></tr>";
                    foreach (BasketItem basketItem in basket.BasketItems)
                    {
                        HtmlProducts += "<tr>";
                        HtmlProducts += "<td>" + basketItem.Product.Title.ToString() + "</td>";
                        HtmlProducts += "<td>" + basketItem.Count.ToString() + "</td>";
                        HtmlProducts += "<td>" + basketItem.ItemPrice.ToString() + "</td>";
                        HtmlProducts += "</tr>";

                    }
                    HtmlProducts += "</table>";

                    MailBody = MailBody.Replace("%Products%", HtmlProducts);
                    bool retValueSendMail = Mail.sendMail(loginUser.Email.ToString(), "musterihizmetleri@banyom.com.tr", "m171007", "Banyom.com.tr | Şipariş Bilgileriniz", MailBody);

                    if (retValueSendMail)
                    {
                        Response.Redirect("Order.aspx");
                    }
                    else
                    {
                        Session.Add("IsSend", 0);
                        Response.Redirect("Order.aspx");

                    }
                }
                Session.Remove("USER_BASKET");
            }
            else
            {
                Response.Redirect("../SecuredPages/Login.aspx?returl=Default.aspx");
            }
        }


        protected void ddlCreditCard_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCreditCard.SelectedValue == "99")
            {
                ListItem ls = new ListItem("1", "1");
                ddlCreditCardMonth.Items.Clear();
                ddlCreditCardMonth.Items.Add(ls);
            }
            else
            {
                List<PaymetTermRate> ListRates = PaymetTermRateData.GetPaymetTermRateList(Convert.ToInt32(ddlCreditCard.SelectedValue));
                Utility.BindDropDownList(ddlCreditCardMonth, ListRates, "Month", "Rate");
            }
        }

        protected void ddlCreditCardMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            basket = (Basket)HttpContext.Current.Session["USER_BASKET"];
            lblMonth.Text = ddlCreditCardMonth.SelectedItem.Text;
            lblBasketTotal.Text = Toolkit.Utility.CurrencyFormat(basket.TotalRatedPrice * Convert.ToDecimal(ddlCreditCardMonth.SelectedValue));
        }

        protected void ddlPaymentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlPaymentType.SelectedValue == "36") // havale
            {
                DvTransferAccount.Visible = true;
                DvCreditCard.Visible = false;
            }
            else
            {
                DvTransferAccount.Visible = false;
                DvCreditCard.Visible = true;
            }

            if (ddlPaymentType.SelectedValue == "-1") // seçiniz
            {
                DvTransferAccount.Visible = false;
                Session.Remove("USER_BASKET");
                DvCreditCard.Visible = false;
            }
        }

        protected void btnApprove_Click(object sender, ImageClickEventArgs e)
        {
            PaymetInfo paymetInfo = null;
            //TEST AMACLI KALDIRILACAK
            if (Session["LOGIN_USER"] != null && Session["USER_BASKET"] != null)
            {
                loginUser = (User)Session["LOGIN_USER"];
            }

            bool isOk = true;
            divAlert.InnerHtml = " ";
            if (FormControl())
            {
                if (ddlPaymentType.SelectedValue == "37") // kredikartı
                {
                    if (ddlCreditCard.SelectedValue != "99")
                    {
                        paymetInfo = new Model.Bank.PaymetInfo() { CreditCard = CreditCardData.Get(Convert.ToInt32(ddlCreditCard.SelectedValue)), Name = txtCustomerName.Text, CreditCardNumber = txtCreditCardNumber.Text, Cvc = txtCvv2.Text, Month = Convert.ToInt32(ddlMonth.SelectedValue), SelectedTerm = Convert.ToInt32(ddlCreditCardMonth.SelectedItem.Text), Rate = Convert.ToDecimal(ddlCreditCardMonth.SelectedValue), Year = Convert.ToInt32(ddlYear.SelectedValue), PaymentType = new Model.CommonModel.EnumValue() { Id = 37 } };
                        isOk = BinControl(paymetInfo);
                    }
                    else
                    {
                        paymetInfo = new Model.Bank.PaymetInfo() { CreditCard = CreditCardData.Get(Convert.ToInt32(ddlCreditCard.SelectedValue)), Name = txtCustomerName.Text, CreditCardNumber = txtCreditCardNumber.Text, Cvc = txtCvv2.Text, Month = Convert.ToInt32(ddlMonth.SelectedValue), SelectedTerm = Convert.ToInt32(ddlCreditCardMonth.SelectedItem.Text), Rate = Convert.ToDecimal(ddlCreditCardMonth.SelectedValue), Year = Convert.ToInt32(ddlYear.SelectedValue), PaymentType = new Model.CommonModel.EnumValue() { Id = 37 } };
                        isOk = true;
                    }
                }
                else
                {
                    paymetInfo = new Model.Bank.PaymetInfo() { CreditCard = new CreditCard(), Name = txtCustomerName.Text, CreditCardNumber = "", Cvc = "", Month = 0, Year = 0, TransferAccount = TransferAccountData.Get(Convert.ToInt32(rblTransferAccount.SelectedValue)), PaymentType = new Model.CommonModel.EnumValue() { Id = 36 } };
                }
                if (isOk)
                {

                    if (Payment(paymetInfo))
                    {
                        SaveOrder(paymetInfo);
                    }

                }
            }
        }

        private bool BinControl(PaymetInfo paymetInfo)
        {
            bool isOk = false;
            List<BinNumber> binNumberList = BinNumberData.GetBinNumberList(paymetInfo.CreditCard.Bank.Id);
            foreach (BinNumber binNumber in binNumberList)
            {
                if (binNumber.Number == paymetInfo.CreditCardNumber.Substring(0, 6).ToString())
                {
                    isOk = true;
                }
            }
            if (!isOk)
            {
                divAlert.InnerHtml += "<span style=\"color:Red\">Girmiş olduğunuz Kredi kartı numarası Seçmiş olduğunuz Kredi kartı ile uyuşmamaktadır</span><br />";
            }
            return isOk;

        }

        private bool FormControl()
        {
            bool isOk = true;
            if (ddlPaymentType.SelectedValue == "37") // kredikartı
            {
                if (txtCvv2.Text == "")
                {
                    divAlert.InnerHtml += "<span style=\"color:Red\">Lütfen cvv numarasını giriniz!</span><br />";
                    isOk = false;
                }
                if (txtCustomerName.Text == "")
                {
                    divAlert.InnerHtml += "<span style=\"color:Red\">Lütfen kart üzerindeki ismi giriniz!</span><br />";
                    isOk = false;
                }
                if (txtCreditCardNumber.Text == "")
                {
                    divAlert.InnerHtml += "<span style=\"color:Red\">Lütfen kart numarasını  giriniz!</span><br />";
                    isOk = false;
                }
                if (txtCreditCardNumber.Text.Length != 16)
                {
                    divAlert.InnerHtml += "<span style=\"color:Red\">Lütfen kart numarasını  giriniz!</span><br />";
                    isOk = false;
                }

                if (txtCvv2.Text.Length != 3)
                {
                    divAlert.InnerHtml += "<span style=\"color:Red\">Lütfen cvv numarasını  giriniz!</span><br />";
                    isOk = false;
                }
                if (ddlCreditCard.SelectedValue == "-1")
                {
                    divAlert.InnerHtml += "<span style=\"color:Red\">Lütfen kredi kartı tipini  şeçiniz!</span><br />";
                    isOk = false;
                }
                if (ddlCreditCardMonth.SelectedValue == "-1")
                {
                    divAlert.InnerHtml += "<span style=\"color:Red\">Lütfen taksit oranını  şeçiniz!</span><br />";
                    isOk = false;
                }
            }
            else
            {
                if (rblTransferAccount.SelectedValue == "")
                {
                    divAlert.InnerHtml += "<span style=\"color:Red\">Lütfen havale yapacağınız bankayı  şeçiniz!</span><br />";
                    isOk = false;
                }
            }
            return isOk;

        }
        private string NoTurkishChar(string str)
        {
            str = str.Replace("ı", "i");
            str = str.Replace("ç", "c");
            str = str.Replace("ö", "o");
            str = str.Replace("ş", "s");
            str = str.Replace("ğ", "g");
            str = str.Replace("ü", "u");
            str = str.Replace("Ç", "c");
            str = str.Replace("Ş", "s");
            str = str.Replace("İ", "i");
            str = str.Replace("Ö", "o");
            str = str.Replace("Ğ", "g");
            str = str.Replace("Ü", "u");
            return str;
        }
        private bool Payment(PaymetInfo paymetInfo)
        {
            loginUser = (User)Session["LOGIN_USER"];
            bool isOk = false;
            basket = (Basket)HttpContext.Current.Session["USER_BASKET"];
            if (paymetInfo.PaymentType.Id == 36)//havale
            {
                return true;
            }
            else
            {

                if (ddlCreditCard.SelectedValue == "99")
                {
                    isOk = DefaultCard(paymetInfo);
                }
                else
                {
                    switch (paymetInfo.CreditCard.Id)
                    {
                        case 1:
                            isOk = CreditCard(paymetInfo);
                            break;
                        case 2:
                            isOk = YapiKredi(paymetInfo);
                            break;
                    }
                }



            }
            return isOk;
        }

        private bool DefaultCard(PaymetInfo paymetInfo)
        {
            bool isOk = false;
            string term = "";

            //_PosnetDotNetTDSOOSModule.C_PosnetOOSTDS myYK = new C_PosnetOOSTDS();
            string Month = "";
            if (paymetInfo.Month.ToString().Length == 1)
                Month = "0" + paymetInfo.Month.ToString();
            else
                Month = paymetInfo.Month.ToString();

            string pccno = paymetInfo.CreditCardNumber.ToString();
            string pexpdate = paymetInfo.Year.ToString().Replace("20", "") + Month;
            string pamount = lblBasketTotal.Text.Replace(".", "").Replace(",", "");

            //test için sonra silinecek

            Session["PaymentInfo"] = paymetInfo;
            Session["pamount"] = pamount;
            Session["Shipping"] = lblShippingPrice.Text;
            string pcurrencycode = "YT";

            string pcvc = paymetInfo.Cvc.ToString();

            term = "1";

            string ptaknum = "00";

            string yil = "";
            string ay = "";
            string gun = "";
            string sa = "";
            string dk = "";
            string sn = "";

            yil = DateTime.Now.Year.ToString().Replace("20", "");
            ay = DateTime.Now.Month.ToString();
            gun = DateTime.Now.Day.ToString();
            sa = DateTime.Now.Hour.ToString();
            dk = DateTime.Now.Minute.ToString();
            sn = DateTime.Now.Second.ToString();

            if (ay.Length == 1) ay = "0" + ay;
            if (gun.Length == 1) gun = "0" + gun;
            if (sa.Length == 1) sa = "0" + sa;
            if (dk.Length == 1) dk = "0" + dk;
            if (sn.Length == 1) sn = "0" + sn;
            string porderid = "YKB_0000" + yil + ay + gun + sa + dk + sn;
            Session["ptaknum"] = ptaknum;
            Session["pamount"] = pamount;
            Session["porderid"] = porderid;
            Session["3ds"] = 0;
            Response.Redirect("posnettds.aspx");
            //myYK.SetMid("6734273367");
            //myYK.SetTid("67932822");
            //myYK.SetKey("10,10,10,10,10,10,10,10");
            //myYK.SetPosnetID("3261");
            //myYK.SetURL("http://setmpos.ykb.com/PosnetWebService/XML");

            bool outtran = false;
            //outtran=  myYK.CreateTranRequestDatas(txtCustomerName.Text, pamount, pcurrencycode, ptaknum, porderid, "Sale",
            //                                   pccno, pexpdate, pcvc);

            if (outtran == true)
            {
                divAlert.InnerHtml = "baglantı kuruldu<br>";

                btnApprove.OnClientClick = "submitFormEx(formName, 0, 'YKBWindow');";
                //if (myYK.GetApprovedCode() == "1")
                //{
                //    divAlert.InnerHtml += "Para çekildi.(YapiKredi)";

                //    return true;
                //}
                //else // (myYK.GetApprovedCode == "0")
                //{
                //    divAlert.InnerHtml += myYK.GetApprovedCode();
                //    divAlert.InnerHtml += myYK.GetResponseText();
                //    divAlert.InnerHtml += "<span style=\"color:Red\"> Kart onaylanmadı </span><br />";
                //    return false;
                //}
            }
            else
            {
                divAlert.InnerHtml = outtran.ToString();
            }

            return false;
        }

        private bool YapiKredi(PaymetInfo paymetInfo)
        {
            bool isOk = false;
            string term = "";

            //_PosnetDotNetTDSOOSModule.C_PosnetOOSTDS myYK = new C_PosnetOOSTDS();
            string Month = "";
            if (paymetInfo.Month.ToString().Length == 1)
                Month = "0" + paymetInfo.Month.ToString();
            else
                Month = paymetInfo.Month.ToString();
            string pccno = paymetInfo.CreditCardNumber.ToString();
            string pexpdate = paymetInfo.Year.ToString().Replace("20", "") + Month;
            string pamount = lblBasketTotal.Text.Replace(".", "").Replace(",", "");

            //test için sonra silinecek
            Session["PaymentInfo"] = paymetInfo;
            Session["pamount"] = pamount;
            Session["Shipping"] = lblShippingPrice.Text;
            string pcurrencycode = "YT";

            string pcvc = paymetInfo.Cvc.ToString();

            if (paymetInfo.SelectedTerm.ToString() == "1")
                term = "1";
            else
                term = paymetInfo.SelectedTerm.ToString();

            string ptaknum = term;
            switch (term)
            {
                case "1":
                    ptaknum = "00";
                    break;
                case "2":
                    ptaknum = "02";
                    break;
                case "3":
                    ptaknum = "03";
                    break;
                case "4":
                    ptaknum = "04";
                    break;
                case "5":
                    ptaknum = "05";
                    break;
                case "6":
                    ptaknum = "06";
                    break;
                case "7":
                    ptaknum = "07";
                    break;
                case "8":
                    ptaknum = "08";
                    break;
                case "9":
                    ptaknum = "09";
                    break;
                case "10":
                    ptaknum = "10";
                    break;
                case "11":
                    ptaknum = "11";
                    break;
                case "12":
                    ptaknum = "12";
                    break;
                default:
                    ptaknum = "00";
                    break;
            }

            string yil = "";
            string ay = "";
            string gun = "";
            string sa = "";
            string dk = "";
            string sn = "";

            yil = DateTime.Now.Year.ToString().Replace("20", "");
            ay = DateTime.Now.Month.ToString();
            gun = DateTime.Now.Day.ToString();
            sa = DateTime.Now.Hour.ToString();
            dk = DateTime.Now.Minute.ToString();
            sn = DateTime.Now.Second.ToString();

            if (ay.Length == 1) ay = "0" + ay;
            if (gun.Length == 1) gun = "0" + gun;
            if (sa.Length == 1) sa = "0" + sa;
            if (dk.Length == 1) dk = "0" + dk;
            if (sn.Length == 1) sn = "0" + sn;
            string porderid = "YKB_0000" + yil + ay + gun + sa + dk + sn;
            Session["ptaknum"] = ptaknum;
            Session["pamount"] = pamount;
            Session["porderid"] = porderid;
            Session["3ds"] = 0;
            Response.Redirect("posnettds.aspx");

            //if (porderid.Length < 24)
            //{
            //    string nullvalue = "";
            //    int m = 24 - porderid.Length;
            //    for (int i = 0; i < m; i++)
            //    {
            //        nullvalue += "0";
            //    }
            //    porderid = nullvalue + porderid;
            //}

            //myYK.SetMid(paymetInfo.CreditCard.VposId.ToString());
            //myYK.SetTid(paymetInfo.CreditCard.VposPassword.ToString());
            //myYK.SetURL(paymetInfo.CreditCard.VposHost.ToString());

            //bool outtran = false;
            //outtran = myYK.DoSaleTran(pccno, pexpdate, pcvc, porderid, pamount, pcurrencycode, ptaknum, "00", "000000");

            //if (outtran == true)
            //{
            //    divAlert.InnerHtml = "baglantı kuruldu<br>";
            //    if (myYK.GetApprovedCode() == "1")
            //    {
            //        divAlert.InnerHtml += "Para çekildi.(YapiKredi)";

            //        return true;
            //    }
            //    else // (myYK.GetApprovedCode == "0")
            //    {
            //        divAlert.InnerHtml += myYK.GetApprovedCode();
            //        divAlert.InnerHtml += myYK.GetResponseText();
            //        divAlert.InnerHtml += "<span style=\"color:Red\"> Kart onaylanmadı </span><br />";
            //        return false;
            //    }
            //}
            //else
            //{
            //    divAlert.InnerHtml = outtran.ToString();
            //}

            return false;

        }


        public string GetSHA1(string SHA1Data)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            string HashedPassword = SHA1Data;
            byte[] hashbytes = Encoding.GetEncoding("ISO-8859-9").GetBytes(HashedPassword);
            byte[] inputbytes = sha.ComputeHash(hashbytes);
            return GetHexaDecimal(inputbytes);
        }
        public string GetHexaDecimal(byte[] bytes)
        {
            StringBuilder s = new StringBuilder();
            int length = bytes.Length;
            for (int n = 0; n <= length - 1; n++)
            {
                s.Append(String.Format("{0,2:x}", bytes[n]).Replace(" ", "0"));
            }
            return s.ToString();
        }
        private bool CreditCard(PaymetInfo paymetInfo)
        {
            bool isOk = false;
            string strMode = "TEST";
            string strVersion = "v0.01";
            string strTerminalID = "10012855"; //8 Haneli TerminalID yazılmalı.
            string _strTerminalID = "0" + strTerminalID;
            string strProvUserID = "PROVAUT";
            string strProvisionPassword = "Snr7793748"; //TerminalProvUserID şifresi
            string strUserID = "PROVAUT";
            string strMerchantID = "9271569"; //Üye İşyeri Numarası
            string strIPAddress = Request.UserHostAddress; //Kullanıcının IP adresini alır
            string strEmailAddress = "info@tradesis.com";
            string strOrderID = BasketData.GetMaxBasket();
            string strNumber = paymetInfo.CreditCardNumber.ToString();
            string Month = "";
            if (paymetInfo.Month.ToString().Length == 1)
                Month = "0" + paymetInfo.Month.ToString();
            else
                Month = paymetInfo.Month.ToString();
            string pexpdate = paymetInfo.Year.ToString().Replace("20", "") + Month;

            string strExpireDate = pexpdate;
            string strCVV2 = "";
            string strAmount = "100"; //İşlem Tutarı 1.00 TL için 100 gönderilmeli
            string strType = "sales";
            string strCurrencyCode = "949";
            string strCardholderPresentCode = "0";
            string strMotoInd = "N";
            string strInstallmentCount = "";
            string strHostAddress = "https://sanalposprov.garanti.com.tr/VPServlet";

            string SecurityData = GetSHA1(strProvisionPassword + _strTerminalID).ToUpper();
            string HashData = GetSHA1(strOrderID + strTerminalID + strNumber + strAmount + SecurityData).ToUpper();

            string strXML = null;
            strXML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + "<GVPSRequest>" + "<Mode>" + strMode + "</Mode>" + "<Version>" + strVersion + "</Version>" + "<Terminal><ProvUserID>" + strProvUserID + "</ProvUserID><HashData>" + HashData + "</HashData><UserID>" + strUserID + "</UserID><ID>" + strTerminalID + "</ID><MerchantID>" + strMerchantID + "</MerchantID></Terminal>" + "<Customer><IPAddress>" + strIPAddress + "</IPAddress><EmailAddress>" + strEmailAddress + "</EmailAddress></Customer>" + "<Card><Number>" + strNumber + "</Number><ExpireDate>" + strExpireDate + "</ExpireDate><CVV2>" + strCVV2 + "</CVV2></Card>" + "<Order><OrderID>" + strOrderID + "</OrderID><GroupID></GroupID><AddressList><Address><Type>S</Type><Name></Name><LastName></LastName><Company></Company><Text></Text><District></District><City></City><PostalCode></PostalCode><Country></Country><PhoneNumber></PhoneNumber></Address></AddressList></Order>" + "<Transaction>" + "<Type>" + strType + "</Type><InstallmentCnt>" + strInstallmentCount + "</InstallmentCnt><Amount>" + strAmount + "</Amount><CurrencyCode>" + strCurrencyCode + "</CurrencyCode><CardholderPresentCode>" + strCardholderPresentCode + "</CardholderPresentCode><MotoInd>" + strMotoInd + "</MotoInd>" + "</Transaction>" + "</GVPSRequest>";

            try
            {
                string data = "data=" + strXML;

                WebRequest _WebRequest = WebRequest.Create(strHostAddress);
                _WebRequest.Method = "POST";

                byte[] byteArray = Encoding.UTF8.GetBytes(data);
                _WebRequest.ContentType = "application/x-www-form-urlencoded";
                _WebRequest.ContentLength = byteArray.Length;

                Stream dataStream = _WebRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                WebResponse _WebResponse = _WebRequest.GetResponse();
                Console.WriteLine(((HttpWebResponse)_WebResponse).StatusDescription);
                dataStream = _WebResponse.GetResponseStream();

                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();

                Console.WriteLine(responseFromServer);

                //Müşteriye gösterilebilir ama Fraud riski açısından bu değerleri göstermeyelim.
                //responseFromServer

                //GVPSResponse XML'in değerlerini okuyoruz. İstediğiniz geri dönüş değerlerini gösterebilirsiniz.
                string XML = responseFromServer;
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(XML);

                //ReasonCode
                XmlElement xElement1 = xDoc.SelectSingleNode("//GVPSResponse/Transaction/Response/ReasonCode") as XmlElement;
                //lblResult2.Text = xElement1.InnerText;

                //Message
                //XmlElement xElement2 = xDoc.SelectSingleNode("//GVPSResponse/Transaction/Response/Message") as XmlElement;
                //lblResult2.Text = xElement2.InnerText;

                //ErrorMsg
                XmlElement xElement3 = xDoc.SelectSingleNode("//GVPSResponse/Transaction/Response/ErrorMsg") as XmlElement;
                divAlert.InnerHtml += xElement3.InnerText;
                divAlert.InnerHtml += xElement1.InnerText;
                //00 ReasonCode döndüğünde işlem başarılıdır. Müşteriye başarılı veya başarısız şeklinde göstermeniz tavsiye edilir. (Fraud riski)
                if (xElement1.InnerText == "00")
                {
                    divAlert.InnerHtml += "İşlem Başarılı";
                }
                else
                {
                    divAlert.InnerHtml += "İşlem Başarısız";
                }

            }
            catch (Exception ex)
            {
                divAlert.InnerHtml += ex.Message;
            }
            //string term = "";
            //ePayment.cc5payment mycc5pay = new ePayment.cc5payment();
            //mycc5pay.clientid = paymetInfo.CreditCard.VposId.ToString();
            //mycc5pay.name = paymetInfo.CreditCard.VposName.ToString();
            //mycc5pay.password = paymetInfo.CreditCard.VposPassword.ToString();
            //mycc5pay.oid = BasketData.GetMaxBasket();//Id gerekiyordu loginId yi aldım normalde OrderId olması lazım
            //mycc5pay.host = paymetInfo.CreditCard.VposHost.ToString();
            //mycc5pay.ip = HttpContext.Current.Request.ServerVariables["Remote_Addr"];//"127.0.0.7";// Request.UserHostAddress;

            //mycc5pay.bname = loginUser.Id.ToString();//Id gerekiyordu loginId yi aldım normalde OrderId olması lazım
            //divAlert.InnerHtml = NoTurkishChar(paymetInfo.Name.ToString()).ToLower() + "<br>";
            //mycc5pay.orderresult = 0;
            //mycc5pay.chargetype = "Auth";
            //mycc5pay.cardnumber = paymetInfo.CreditCardNumber.ToString();
            //mycc5pay.expmonth = paymetInfo.Month.ToString();
            //mycc5pay.expyear = paymetInfo.Year.ToString();
            //mycc5pay.cv2 = paymetInfo.Cvc.ToString();
            //mycc5pay.subtotal = lblBasketTotal.Text;
            //mycc5pay.userid = paymetInfo.CreditCard.VposUser.ToString();
            //mycc5pay.currency = "949";//TL
            //if (paymetInfo.SelectedTerm.ToString() == "1")
            //    term = "";
            //else
            //    term = paymetInfo.SelectedTerm.ToString();

            //mycc5pay.taksit = term;
            //divAlert.InnerHtml = mycc5pay.processorder();

            //if (mycc5pay.appr == "Approved")
            //{
            //    divAlert.InnerHtml += "Para çekildi.";
            //    Session.Remove("USER_BASKET");
            //    isOk = true;
            //}

            //else
            //{
            //    divAlert.InnerHtml += "<span style=\"color:Red\">";
            //    divAlert.InnerHtml += "<br>HataMesaji:" + mycc5pay.errmsg;
            //    divAlert.InnerHtml += "<br>OrderId:" + mycc5pay.oid;
            //    divAlert.InnerHtml += "<br>ApprovalKodu:" + mycc5pay.appr;
            //    divAlert.InnerHtml += "</span><br />";
            //    isOk = false;
            //}
            return isOk;
        }

        protected void imgbtnBack_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/Pages/OrderShipping.aspx");
        }
    }
}