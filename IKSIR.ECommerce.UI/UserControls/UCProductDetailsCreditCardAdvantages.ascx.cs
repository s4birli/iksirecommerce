﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IKSIR.ECommerce.Infrastructure.DataLayer.BankDataLayer;
using IKSIR.ECommerce.Infrastructure.DataLayer.ProductDataLayer;
using IKSIR.ECommerce.Model.Bank;

namespace IKSIR.ECommerce.UI.UserControls
{
    public partial class UCProductDetailsCreditCardAdvantages : UCProductDetailsMaster
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack && productId != 0)
            {
                GetProductCreditCartAdvantages(productId);
            }
        }

        private void GetProductCreditCartAdvantages(int productId)
        {
            try
            {
                dlCreditCards.DataSource = CreditCardData.GetAktiveCreditCardList();
                dlCreditCards.DataBind();
            }
            catch (Exception exception)
            {
            }
        }

        protected void dlCreditCards_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                HiddenField hdnCardId = e.Item.FindControl("hdnCardId") as HiddenField;
                Repeater rptCreditCardAdvantages = e.Item.FindControl("rptCreditCardAdvantages") as Repeater;
                int cardId;

                if (rptCreditCardAdvantages != null && hdnCardId.Value != "" && int.TryParse(hdnCardId.Value, out cardId))
                {
                    //rptCreditCardAdvantages.DataSource = PaymetTermRateData.GetAktivePaymetTermRateList(cardId);
                    //rptCreditCardAdvantages.DataBind();

                    var item = ProductData.Get(productId);
                    if (item != null)
                    {
                        var items = ProductPriceData.GetCalculatedPriceList(item.ProductPrice.Price);
                        rptCreditCardAdvantages.DataSource = items.Where(x => x.Id == cardId).FirstOrDefault().Rates;
                        rptCreditCardAdvantages.DataBind();
                    }
                }
            }
        }
    }
}