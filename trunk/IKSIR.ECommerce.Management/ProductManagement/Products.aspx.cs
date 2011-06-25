﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IKSIR.ECommerce.Model.ProductModel;
using IKSIR.ECommerce.Infrastructure.DataLayer.ProductDataLayer;
using IKSIR.ECommerce.Infrastructure.DataLayer.DataBlock;
using IKSIR.ECommerce.Infrastructure.DataLayer.CommonDataLayer;
using IKSIR.ECommerce.Management.Common;
using IKSIR.ECommerce.Model.CommonModel;
//using IKSIR.ECommerce.Toolkit;

namespace IKSIR.ECommerce.Management.ProductManagement
{
    public partial class Products : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindValues();
                GetList();
            }
        }

        protected void lbtnNew_Click(object sender, EventArgs e)
        {
            ClearForm();
            lblProductId.Text = "Yeni Kayıt";
            pnlForm.Visible = true;
            txtProductName.Focus();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (btnSave.CommandArgument != "") //Kayıt güncelleme.
            {
                if (UpdateItem(Convert.ToInt32(btnSave.CommandArgument)))
                {
                    lblError.Visible = true;
                    lblError.ForeColor = System.Drawing.Color.Green;
                    lblError.Text = "Item başarıyla güncellendi.";
                    ClearForm();
                    pnlForm.Visible = false;
                    int count = 0;
                    GetList();
                }
                else
                {
                    lblError.Visible = true;
                    lblError.ForeColor = System.Drawing.Color.Red;
                    lblError.Text = "Item güncellenirken bir hata oluştu.";
                }
            }
            else //Yeni kayıt
            {
                if (InsertItem())
                {
                    lblError.Visible = true;
                    lblError.ForeColor = System.Drawing.Color.Green;
                    lblError.Text = "Item başarıyla kaydedildi.";
                    ClearForm();
                    pnlForm.Visible = false;
                    GetList();
                }
                else
                {
                    lblError.Visible = true;
                    lblError.ForeColor = System.Drawing.Color.Red;
                    lblError.Text = "Item kaydedilirken bir hata oluştu.";
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            pnlForm.Visible = false;
            pnlList.Visible = true;
            pnlFilter.Visible = true;
        }

        protected void lbtnEdit_Click(object sender, EventArgs e)
        {
            ClearForm();
            //var index = ((sender as LinkButton).Parent.Parent as GridViewRow).RowIndex;
            //gvList.SelectedIndex = index;
            //var itemId = (sender as LinkButton).CommandArgument == ""
            //                 ? 0
            //                 : Convert.ToInt32((sender as LinkButton).CommandArgument);

            //if (!GetItem(itemId))
            //{
            //    lblError.Visible = true;
            //    lblError.ForeColor = System.Drawing.Color.Red;
            //    lblError.Text = "Item güncelleme işleminde bir hata oluştu.";
            //}
        }

        protected void lbtnDelete_Click(object sender, EventArgs e)
        {
            var itemId = (sender as LinkButton).CommandArgument == "" ? 0 : Convert.ToInt32((sender as LinkButton).CommandArgument);

            //if (DeleteItem(itemId))
            //{
            //    lblError.Visible = true;
            //    lblError.ForeColor = System.Drawing.Color.Green;
            //    lblError.Text = "Item başarıyla silindi.";
            //    GetList();
            //}
            //else
            //{
            //    lblError.Visible = true;
            //    lblError.ForeColor = System.Drawing.Color.Red;
            //    lblError.Text = "Item silerken bir hata oluştu.";
            //}
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            GetList();
        }

        protected void btnAddDocument_Click(object sender, EventArgs e)
        {
            if ((fuSelectedDocument.PostedFile != null) && (fuSelectedDocument.PostedFile.ContentLength > 0))
            {
                try
                {
                    string fileExt = fuSelectedDocument.PostedFile.ContentType;
                    if (fileExt == ".doc" || fileExt == ".pdf" || fileExt == ".jpg")
                    {
                        string fn = System.IO.Path.GetFileName(fuSelectedDocument.PostedFile.FileName);
                        //var items = IKSIR.ECommerce.Toolkit.
                        var SaveLocation = "";
                        try
                        {
                            //System.IO.MemoryStream _MemoryStream = new MemoryStream(.CreateNewImage(SaveLocation, 25,25,fileExt));
                            //System.Drawing.Image item = System.Drawing.Image.FromStream(_MemoryStream);

                            fuSelectedDocument.PostedFile.SaveAs(SaveLocation);
                            lblDocumentAlert.Text = "Dosya Yüklendi";
                            lblDocumentAlert.Visible = true;
                            lblDocumentAlert.ForeColor = System.Drawing.Color.Green;
                        }
                        catch (Exception ex)
                        {
                            Response.Write("Error: " + ex.Message);
                        }
                    }
                    else
                    {
                        lblDocumentAlert.Text = "Yüklemek istediğiniz dosya biçimi desteklenmiyor";
                        lblDocumentAlert.Visible = true;
                        lblDocumentAlert.ForeColor = System.Drawing.Color.Red;
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
            else
            {
                lblDocumentAlert.Text = "Yüklemek için dosya seçininiz";
                lblDocumentAlert.Visible = true;
                lblDocumentAlert.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void btnAddProperty_Click(object sender, EventArgs e)
        {

        }

        private void GetItem(int itemId)
        {
            pnlForm.Visible = true;
        }

        private void BindValues()
        {
            //Buralarda tüm kategoriler gelecek istediği kategorinin altına tanımlama yapabilecek.
            List<ProductCategory> itemList = ProductCategoryData.GetProductCategoryList();
            //Utility.BindDropDownList(ddlCategories, itemList, "Title", "Id");
            //Utility.BindDropDownList(ddlFilterParentCategories, itemList, "Title", "Id");

            //List<Property> ProductPropertyList = PropertyData.GetList();
            //Utility.BindDropDownList(ddlProperties, ProductPropertyList, "Title", "Id");

            //List<EnumValue> enumValueList = EnumValueData.GetEnumValues(1);
            //Utility.BindDropDownList(ddlDocumentTypes, enumValueList, "Value", "Id");
        }

        private void GetList()
        {
            List<Product> itemList = ProductData.GetList();
            if (ddlFilterParentCategories.SelectedValue != "-1" && ddlFilterParentCategories.SelectedValue != "")
            {
                int parentCategoryId = DBHelper.IntValue(ddlFilterParentCategories.SelectedValue);
                itemList = itemList.Where(x => x.ProductCategory.Id == parentCategoryId).ToList();
            }
            if (txtFilterProductCode.Text != "")
            {
                string productCode = txtFilterProductCode.Text;
                itemList = itemList.Where(x => x.ProductCode == productCode).ToList();
            }

            gvList.DataSource = itemList;
            gvList.DataBind();
        }

        private bool InsertItem()
        {
            bool retValue = false;
            if (InsertPruductMain())
                retValue = true;
            return retValue;
        }

        private bool UpdateItem(int itemId)
        {
            bool retValue = false;
            if (UpdatePruductMain(itemId))
                retValue = true;
            return retValue;
        }

        private void ClearForm()
        {
            ddlCategories.SelectedIndex = -1;
            txtPropertyValue.Text = string.Empty;
            txtDescription.Text = string.Empty;
            btnSave.CommandArgument = string.Empty;
        }

        #region ProductMain
        private bool GetProductMain(int productId)
        {
            bool retValue = false;
            return retValue;
        }

        private bool SaveProductMain()
        {
            bool retValue = false;
            try
            {

                if (btnSave.CommandArgument != "")
                {
                    //güncelle
                    int productId = DBHelper.IntValue(btnSave.CommandArgument);
                    if (UpdatePruductMain(productId))
                    {
                        lblError.Visible = true;
                        lblError.ForeColor = System.Drawing.Color.Green;
                        lblError.Text = "Ürün güncelleme başarılı.";
                        retValue = false;
                    }
                    else
                    {
                        lblError.Visible = true;
                        lblError.ForeColor = System.Drawing.Color.Red;
                        lblError.Text = "Ürün güncellenirken hata oluştu.";
                        retValue = false;
                    }
                }
                else
                {
                    //yeni kayıt
                    if (InsertPruductMain())
                    {
                        lblError.Visible = true;
                        lblError.ForeColor = System.Drawing.Color.Green;
                        lblError.Text = "Yeni ürün kaydı başarılı.";
                        retValue = false;
                    }
                    else
                    {
                        lblError.Visible = true;
                        lblError.ForeColor = System.Drawing.Color.Red;
                        lblError.Text = "Yeni Ürün kaydedilirken hata oluştu.";
                        retValue = false;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            { }
            return retValue;
        }

        private bool InsertPruductMain()
        {
            bool retValue = false;
            string productCode = txtProductCode.Text;
            var itemProduct = ProductData.GetList().Where(x => x.ProductCode == productCode).FirstOrDefault();

            //item kaydedilmeden dbde olup olmadığına dair kontroller yapıyoruz.
            if (itemProduct != null)
            {
                lblError.Visible = true;
                lblError.ForeColor = System.Drawing.Color.Red;
                lblError.Text = "Bu ürün koduna sahip item zaten kayıtlıdır. Filtreleryerek kayda erişebilirsiniz.";
            }
            else
            {
                itemProduct.AlertDate = Convert.ToDateTime(txtAlertDate.Text);
                itemProduct.CreateAdminId = 1;
                itemProduct.CreateDate = DateTime.Now;
                itemProduct.Description = txtDescription.Text;
                itemProduct.MinStock = DBHelper.IntValue(txtMinStock.Text);
                itemProduct.ProductCategory = new ProductCategory() { Id = DBHelper.IntValue(ddlCategories.SelectedValue) };
                itemProduct.ProductCode = txtProductCode.Text;
                itemProduct.Title = txtProductName.Text;
                int result = ProductData.Insert(itemProduct);
                if (result > 0)
                {
                    retValue = true;
                    if (Session["PRODUCT_DOCUMENT_LIST"] != null)
                    {
                        //ayhant => yeni kayıt olduğundan önce product'tı sonra product'ın dökümanlarını kaydediyoruz.
                        List<Multimedia> productDocumentList = (List<Multimedia>)Session["PRODUCT_DOCUMENT_LIST"];
                        foreach (var item in productDocumentList)
                        {
                            if (MultimediasData.Insert(item) > 0)
                            {
                                retValue = true;
                            }
                        }
                    }
                    if (Session["PRODUCT_PROPERTY_LIST"] != null)
                    {
                        //ayhant => yeni kayıt olduğundan önce product'tı sonra product'ın propertylerini kaydediyoruz.
                        List<ProductProperty> productDocumentList = (List<ProductProperty>)Session["PRODUCT_PROPERTY_LIST"];
                        foreach (var item in productDocumentList)
                        {
                            if (ProductPropertyData.Insert(item) > 0)
                            {
                                retValue = true;
                            }
                        }
                    }
                }
            }
            return retValue;
        }

        private bool UpdatePruductMain(int productId)
        {
            bool retValue = false;
            var itemProduct = ProductData.GetList().Where(x => x.Id == productId).FirstOrDefault();
            if (itemProduct != null)
            {
                itemProduct.AlertDate = Convert.ToDateTime(txtAlertDate.Text);
                itemProduct.CreateAdminId = 1;
                itemProduct.CreateDate = DateTime.Now;
                itemProduct.Description = txtDescription.Text;
                itemProduct.MinStock = DBHelper.IntValue(txtMinStock.Text);
                itemProduct.ProductCategory = new ProductCategory() { Id = DBHelper.IntValue(ddlCategories.SelectedValue) };
                itemProduct.ProductCode = txtProductCode.Text;
                itemProduct.Title = txtProductName.Text;
                int result = ProductData.Update(itemProduct);
                if (result > 0)
                    retValue = true;
            }
            return retValue;
        }
        #endregion

        #region Document
        private bool GetProductDocuments(int productId)
        {
            bool retValue = false;
            try
            {
                var productDocumentList = MultimediasData.GetItemMultimedias(3, productId); //3 Product EnumValueId ayhant
                gvDocumentList.DataSource = productDocumentList;
                gvDocumentList.DataBind();
                retValue = true;
            }
            catch (Exception exception)
            {

            }
            return retValue;
        }

        private bool GetProductDocument(int documentId)
        {
            bool retValue = false;
            var itemProductDocument = MultimediasData.Get(documentId);
            if (itemProductDocument != null)
            {
                txtDocumentName.Text = itemProductDocument.Title;
                txtDocumentDescription.Text = itemProductDocument.Description;
                ddlDocumentTypes.SelectedValue = itemProductDocument.Type.Id.ToString();
                retValue = true;
            }
            return retValue;
        }

        private bool SaveDocument()
        {
            //Documents diye bir table olması lazım cache'te.
            bool retValue = false;
            try
            {
                if (btnAddDocument.CommandArgument != "")
                {
                    //güncelle
                    int documentId = DBHelper.IntValue(btnAddDocument.CommandArgument);
                    if (UpdateDocument(documentId))
                    {
                        lblDocumentAlert.Visible = true;
                        lblDocumentAlert.ForeColor = System.Drawing.Color.Green;
                        lblDocumentAlert.Text = "Döküman güncelleme başarılı.";
                        retValue = false;
                    }
                    else
                    {
                        lblDocumentAlert.Visible = true;
                        lblDocumentAlert.ForeColor = System.Drawing.Color.Red;
                        lblDocumentAlert.Text = "Döküman güncellenirken hata oluştu.";
                        retValue = false;
                    }
                }
                else
                {
                    //yeni kayıt
                    if (InsertDocument())
                    {
                        lblDocumentAlert.Visible = true;
                        lblDocumentAlert.ForeColor = System.Drawing.Color.Green;
                        lblDocumentAlert.Text = "Yeni ürün kaydı başarılı.";
                        retValue = false;
                    }
                    else
                    {
                        lblDocumentAlert.Visible = true;
                        lblDocumentAlert.ForeColor = System.Drawing.Color.Red;
                        lblDocumentAlert.Text = "Yeni Ürün kaydedilirken hata oluştu.";
                        retValue = false;
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {

            }
            return retValue;
        }

        private bool InsertDocument()
        {
            bool retValue = false;
            try
            {
                List<Multimedia> productDocumentList;
                if (Session["PRODUCT_DOCUMENT_LIST"] != null)
                {
                    productDocumentList = (List<Multimedia>)Session["PRODUCT_DOCUMENT_LIST"];
                }
                else
                {
                    productDocumentList = new List<Multimedia>();
                    Session.Add("PRODUCT_DOCUMENT_LIST", productDocumentList);
                }
                productDocumentList = (List<Multimedia>)Session["PRODUCT_DOCUMENT_LIST"];
                var item = new Multimedia();
                item.Description = txtDocumentDescription.Text;
                item.FilePath = fuSelectedDocument.FileName;
                item.Title = txtDocumentName.Text;
                item.Type = new EnumValue() { Id = DBHelper.IntValue(ddlDocumentTypes.SelectedValue) };
                productDocumentList.Add(item);
                Session.Add("PRODUCT_DOCUMENT_LIST", productDocumentList);
                retValue = true;
            }
            catch (Exception)
            {
                throw;
            }
            return retValue;
        }

        private bool UpdateDocument(int documentId)
        {
            bool retValue = false;
            try
            {
                List<Multimedia> productDocumentList;
                if (Session["PRODUCT_DOCUMENT_LIST"] != null)
                {
                    productDocumentList = (List<Multimedia>)Session["PRODUCT_DOCUMENT_LIST"];
                    var item = productDocumentList.Where(x => x.Id == documentId).SingleOrDefault();
                    //ayhant=> Kaydedip tekrar item'ı listeye ekleyeceğimizden listedekini önce siliyoruz.
                    productDocumentList.Remove(item);
                    item.Id = documentId;
                    item.Description = txtDocumentDescription.Text;
                    item.FilePath = fuSelectedDocument.FileName;
                    item.Title = txtDocumentName.Text;
                    item.Type = new EnumValue() { Id = DBHelper.IntValue(ddlDocumentTypes.SelectedValue) };
                    productDocumentList.Add(item);
                    Session.Add("PRODUCT_DOCUMENT_LIST", productDocumentList);
                    retValue = true;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return retValue;
        }
        #endregion

        #region Property
        private bool GetProductProperties(int productId)
        {
            bool retValue = false;
            var itemProductPropertyList = ProductPropertyData.GetProductProperties(productId);
            gvProductProperties.DataSource = itemProductPropertyList;
            gvProductProperties.DataBind();
            return retValue;
        }
        private bool GetProductProperty(int propertyId)
        {
            bool retValue = false;
            var itemProductProperty = ProductPropertyData.Get(propertyId);
            txtPropertyValue.Text = itemProductProperty.Property.PropertyValue.ToString();
            ddlProperties.SelectedValue = itemProductProperty.Property.Id.ToString();
            return retValue;
        }
        private bool SaveProductProperty(int productId)
        {
            bool retValue = false;
            try
            {
                if (btnAddProperty.CommandArgument != "")
                {
                    //güncelle
                    int propertyId = DBHelper.IntValue(btnAddProperty.CommandArgument);
                    if (UpdateProperty(propertyId))
                    {
                        lblPropertyAlert.Visible = true;
                        lblPropertyAlert.ForeColor = System.Drawing.Color.Green;
                        lblPropertyAlert.Text = "Özellik güncelleme başarılı.";
                        retValue = false;
                    }
                    else
                    {
                        lblPropertyAlert.Visible = true;
                        lblPropertyAlert.ForeColor = System.Drawing.Color.Red;
                        lblPropertyAlert.Text = "Özellik güncellenirken hata oluştu.";
                        retValue = false;
                    }
                }
                else
                {
                    //yeni kayıt
                    if (InsertProperty(productId))
                    {
                        lblPropertyAlert.Visible = true;
                        lblPropertyAlert.ForeColor = System.Drawing.Color.Green;
                        lblPropertyAlert.Text = "Yeni özellik kaydı başarılı.";
                        retValue = false;
                    }
                    else
                    {
                        lblPropertyAlert.Visible = true;
                        lblPropertyAlert.ForeColor = System.Drawing.Color.Red;
                        lblPropertyAlert.Text = "Yeni özellik kaydedilirken hata oluştu.";
                        retValue = false;
                    }
                }
            }
            catch (Exception)
            {

            }
            finally
            {

            }
            return retValue;
        }
        private bool InsertProperty(int productId)
        {
            bool retValue = false;
            try
            {
                List<ProductProperty> productPropertyList;
                if (Session["PRODUCT_PROPERTY_LIST"] != null)
                {
                    productPropertyList = (List<ProductProperty>)Session["PRODUCT_PROPERTY_LIST"];
                }
                else
                {
                    productPropertyList = new List<ProductProperty>();
                    Session.Add("PRODUCT_PROPERTY_LIST", productPropertyList);
                }
                productPropertyList = (List<ProductProperty>)Session["PRODUCT_PROPERTY_LIST"];
                var item = new ProductProperty();
                item.ProductId = productId;
                item.Property = new Property() { Id = DBHelper.IntValue(ddlProperties.SelectedValue) };
                productPropertyList.Add(item);
                Session.Add("PRODUCT_PROPERTY_LIST", productPropertyList);
                retValue = true;
            }
            catch (Exception)
            {
                throw;
            }
            return retValue;
        }
        private bool UpdateProperty(int propertyId)
        {
            bool retValue = false;
            try
            {
                List<ProductProperty> productPropertyList;
                if (Session["PRODUCT_PROPERTY_LIST"] != null)
                {
                    productPropertyList = (List<ProductProperty>)Session["PRODUCT_PROPERTY_LIST"];
                }
                else
                {
                    productPropertyList = new List<ProductProperty>();
                    Session.Add("PRODUCT_PROPERTY_LIST", productPropertyList);
                }
                productPropertyList = (List<ProductProperty>)Session["PRODUCT_PROPERTY_LIST"];
                var item = productPropertyList.Where(x => x.Id == propertyId).SingleOrDefault();
                productPropertyList.Remove(item);
                //Sessiondaki item'ı güncellemek için listeden silip tekrar ekliyoruz.
                item.Id = propertyId;
                item.Property = new Property() { Id = DBHelper.IntValue(ddlProperties.SelectedValue) };
                productPropertyList.Add(item);
                Session.Add("PRODUCT_PROPERTY_LIST", productPropertyList);
                retValue = true;
            }
            catch (Exception)
            {
                throw;
            }
            return retValue;
        }
        #endregion
    }
}