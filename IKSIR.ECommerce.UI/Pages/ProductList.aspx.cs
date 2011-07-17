﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IKSIR.ECommerce.Infrastructure.DataLayer.ProductDataLayer;
using IKSIR.ECommerce.Model.ProductModel;

namespace IKSIR.ECommerce.UI.Pages
{
    public partial class ProductList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            GetProducts();
            if (!Page.IsPostBack)
            {
                int categoryId = 0;
                int moduleId = 0;

                if (Page.Request.QueryString["catid"] != null && Page.Request.QueryString["catid"].ToString() != "" && int.TryParse(Page.Request.QueryString["catid"].ToString(), out categoryId))
                {
                    GetProductsForCategory(categoryId);
                }
                else if (Page.Request.QueryString["modid"] != null && Page.Request.QueryString["modid"].ToString() != "" && int.TryParse(Page.Request.QueryString["modid"].ToString(), out moduleId))
                {
                    GetProductsForModul(moduleId);
                }
                else
                {
                    Response.Redirect("/Default.aspx");
                }
            }
        }

        private void GetProducts()
        {
            int activePage = 0;
            if (Request.QueryString["p"] != null)
            {
                activePage = Int32.Parse(Request.QueryString["p"].ToString());
                activePage -= 1;
            }

            List<Product> productList = ProductData.GetList();

            var pageCount = productList.Count / 5;
            if (productList.Count % 5 != 0)
                pageCount += 1;
            Dictionary<string, string> pages = new Dictionary<string, string>();

            for (int i = 1; i <= pageCount; i++)
            {
                pages.Add(i.ToString(), "/Pages/ProductList.aspx?catid=1&p=" + i.ToString());
            }

            dlPaging.DataSource = pages;
            dlPaging.DataBind();

            productList = productList.Skip(5 * activePage).Take(5).ToList();
            dlProductList.DataSource = productList;
            dlProductList.DataBind();
        }

        private void GetProductsForModul(int moduleId)
        {
            List<Product> list = ModuleProductData.GetModuleProductList(moduleId);
            BindGrid(list);
        }

        private void GetProductsForCategory(int categoryId)
        {
            List<Product> list = ModuleProductData.GetModuleProductList(categoryId);
            BindGrid(list);
        }

        private void BindGrid(List<Product> productList)
        {
            int activePage = 0;
            if (Request.QueryString["p"] != null)
            {
                activePage = Int32.Parse(Request.QueryString["p"].ToString());
                activePage -= 1;
            }

            var pageCount = productList.Count / 5;
            if (productList.Count % 5 != 0)
                pageCount += 1;
            Dictionary<string, string> pages = new Dictionary<string, string>();

            for (int i = 1; i <= pageCount; i++)
            {
                pages.Add(i.ToString(), "/Pages/ProductList.aspx?catid=1&p=" + i.ToString());
            }

            dlPaging.DataSource = pages;
            dlPaging.DataBind();

            productList = productList.Skip(5 * activePage).Take(5).ToList();
            dlProductList.DataSource = productList;
            dlProductList.DataBind();
        }

        protected void dlPaging_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                HyperLink hplPageNo = e.Item.FindControl("hplPageNo") as HyperLink;

                if (hplPageNo != null && Page.Request.QueryString["p"] != null && Page.Request.QueryString["p"].ToString() != "" && hplPageNo.Text == Page.Request.QueryString["p"].ToString())
                {
                    hplPageNo.CssClass = "selectedpage";
                }
            }
        }
    }
}