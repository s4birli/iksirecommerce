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
using IKSIR.ECommerce.Toolkit;

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
            lblPropertyId.Text = "Yeni Kayıt";
            pnlForm.Visible = true;
            ddlProductCategories.Focus();
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            int productId = 0;
            if (btnSave.CommandArgument != "") //Kayıt güncelleme.
            {
                productId = Convert.ToInt32(btnSave.CommandArgument);
                SaveProductMain(productId);
            }
            else
            {
                productId = InsertPruductMain();
            }
            SaveDocuments(productId);
            SaveProductProperties(productId);
            GetList();
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
            var index = ((sender as LinkButton).Parent.Parent as GridViewRow).RowIndex;
            gvList.SelectedIndex = index;
            var itemId = (sender as LinkButton).CommandArgument == ""
                             ? 0
                             : Convert.ToInt32((sender as LinkButton).CommandArgument);

            btnSave.CommandArgument = itemId.ToString();
            GetItem(itemId);
        }
        protected void lbtnDocumentEdit_Click(object sender, EventArgs e)
        {
            ruProductDocuments.Visible = false;
            ClearForm();
            var index = ((sender as LinkButton).Parent.Parent as GridViewRow).RowIndex;
            gvList.SelectedIndex = index;
            var documentId = (sender as LinkButton).CommandArgument == ""
                             ? 0
                             : Convert.ToInt32((sender as LinkButton).CommandArgument);
            if (!GetProductDocument(documentId))
            {
                divAlert.InnerHtml += "<span style=\"color:Red\">Dosya bilgilerini getirirken hata oluştu!</span><br />";
            }
        }

        protected void lbtnPropertyEdit_Click(object sender, EventArgs e)
        {
            ClearForm();
            var index = ((sender as LinkButton).Parent.Parent as GridViewRow).RowIndex;
            gvList.SelectedIndex = index;
            var PropertyId = (sender as LinkButton).CommandArgument == ""
                             ? 0
                             : Convert.ToInt32((sender as LinkButton).CommandArgument);

            btnSave.CommandArgument = lblProductId.Text.ToString();
            if (!GetProductProperty(PropertyId))
            {
                divAlert.InnerHtml += "<span style=\"color:Red\">Dosya bilgilerini getirirken hata oluştu!</span><br />";
            }
        }
        protected void lbtnDelete_Click(object sender, EventArgs e)
        {
            var itemId = (sender as LinkButton).CommandArgument == "" ? 0 : Convert.ToInt32((sender as LinkButton).CommandArgument);
            if (DeleteProductMain(itemId))
            {
                divAlert.InnerHtml += "<span style=\"color:Green\">Ürün başarıyla silindi <i>Ürün Id: " + itemId.ToString() + "</i></span><br />";
                GetList();
            }
            else
            {
                divAlert.InnerHtml += "<span style=\"color:Red\">Ürün silinirken hata oluştu! <i>Ürün Id: " + itemId.ToString() + "</i></span><br />";
            }
        }
        protected void lbtnDocumentDelete_Click(object sender, EventArgs e)
        {
            var itemId = (sender as LinkButton).CommandArgument == "" ? 0 : Convert.ToInt32((sender as LinkButton).CommandArgument);
            if (DeleteDocument(itemId))
            {
                divAlert.InnerHtml += "<span style=\"color:Green\">Dosya başarıyla silindi</span><br />";
                GetItem(Convert.ToInt32(lblProductId.Text));
            }
            else
            {
                divAlert.InnerHtml += "<span style=\"color:Red\">Dosya silinirken hata oluştu!</span><br />";
            }
        }
        protected void lbtnPropertyDelete_Click(object sender, EventArgs e)
        {
            var itemId = (sender as LinkButton).CommandArgument == "" ? 0 : Convert.ToInt32((sender as LinkButton).CommandArgument);
            if (DeletePorperty(itemId))
            {
                divAlert.InnerHtml += "<span style=\"color:Green\">Dosya başarıyla silindi</span><br />";
                GetItem(Convert.ToInt32(lblProductId.Text));
            }
            else
            {
                divAlert.InnerHtml += "<span style=\"color:Red\">Dosya silinirken hata oluştu!</span><br />";
            }
        }
        protected void btnFilter_Click(object sender, EventArgs e)
        {
            GetList();
        }

        //protected void btnAddDocument_Click(object sender, EventArgs e)
        //{
        //    UploadFiles(3);
        //    if ((fuSelectedDocument.PostedFile != null) && (fuSelectedDocument.PostedFile.ContentLength > 0))
        //    {
        //        try
        //        {
        //            //string fileExt = fuSelectedDocument.PostedFile.ContentType;
        //            string fileName = fuSelectedDocument.PostedFile.FileName;
        //            char[] tripChars = new char[] { '.' };
        //            int count = fileName.Split(tripChars).Length;
        //            string fileExt = fileName.Split(tripChars)[count - 1];

        //            if (fileExt == "doc" || fileExt == "pdf" || fileExt == "jpg" || fileExt == "png")
        //            {
        //                string fn = System.IO.Path.GetFileName(fuSelectedDocument.PostedFile.FileName);
        //                //var items = IKSIR.ECommerce.Toolkit.                        
        //                var SaveLocation = Server.MapPath("..\\Images\\ProductImages\\OrginalImage");
        //                try
        //                {
        //                    //System.IO.MemoryStream _MemoryStream = new MemoryStream(.CreateNewImage(SaveLocation, 25,25,fileExt));
        //                    //System.Drawing.Image item = System.Drawing.Image.FromStream(_MemoryStream);

        //                    fuSelectedDocument.PostedFile.SaveAs(SaveLocation);
        //                    lblDocumentAlert.Text = "Dosya Yüklendi";
        //                    lblDocumentAlert.Visible = true;
        //                    lblDocumentAlert.ForeColor = System.Drawing.Color.Green;

        //                    if (fileExt == "jpg" || fileExt == "png")
        //                    { 
        //                        //Resize image
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    //Response.Write("Error: " + ex.Message);
        //                    lblDocumentAlert.Text = "Dosya yüklenemedi yazma yetkisi yok";
        //                    lblDocumentAlert.Visible = true;
        //                    lblDocumentAlert.ForeColor = System.Drawing.Color.Red;
        //                }
        //            }
        //            else
        //            {
        //                lblDocumentAlert.Text = "Yüklemek istediğiniz dosya biçimi desteklenmiyor";
        //                lblDocumentAlert.Visible = true;
        //                lblDocumentAlert.ForeColor = System.Drawing.Color.Red;
        //            }
        //        }
        //        catch (Exception)
        //        {

        //            throw;
        //        }
        //    }
        //    else
        //    {
        //        lblDocumentAlert.Text = "Yüklemek için dosya seçininiz";
        //        lblDocumentAlert.Visible = true;
        //        lblDocumentAlert.ForeColor = System.Drawing.Color.Red;
        //    }
        //}

        protected void btnAddProperty_Click(object sender, EventArgs e)
        {
            SaveProductPropertyToList();
            ddlProperties.SelectedIndex = -1;
            txtPropertyValue.Text = string.Empty;
            btnAddProperty.CommandArgument = "";
            lblPropertyId.Text = "Yeni Kayıt";
        }

        private bool GetItem(int itemId)
        {
            bool retValue = false;

            pnlForm.Visible = true;

            if (GetProductMain(itemId))
            {
                divAlert.InnerHtml += "<span style=\"color:Green\">Ürün Genel bilgileri başarıyla yüklendi.</span><br />";
                retValue = true;
            }
            else
            {
                divAlert.InnerHtml += "<span style=\"color:Red\">Ürün Genel bilgileri yüklenirken hata oluştu.</span><br />";
            }

            if (GetProductDocuments(itemId))
            {
                divAlert.InnerHtml += "<span style=\"color:Green\">Ürün dökümanları başarıyla yüklendi.</span><br />";
                retValue = true;
            }
            else
            {
                divAlert.InnerHtml += "<span style=\"color:Red\">Ürün dökümanları yüklenirken hata oluştu.</span><br />";
            }

            if (GetProductProperties(itemId))
            {
                divAlert.InnerHtml += "<span style=\"color:Green\">Ürün özellikleri başarıyla yüklendi.</span><br />";
                retValue = true;
            }
            else
            {
                divAlert.InnerHtml += "<span style=\"color:Red\">Ürün özellikleri yüklenirken hata oluştu.</span><br />";
            }
            return retValue;
        }

        private void BindValues()
        {
            //Buralarda tüm kategoriler gelecek istediği kategorinin altına tanımlama yapabilecek.
            List<ProductCategory> itemList = ProductCategoryData.GetProductCategoryList();
            Utility.BindDropDownList(ddlProductCategories, itemList, "Title", "Id");
            Utility.BindDropDownList(ddlFilterParentCategories, itemList, "Title", "Id");

            List<Property> ProductPropertyList = PropertyData.GetList();
            Utility.BindDropDownList(ddlProperties, ProductPropertyList, "Title", "Id");

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
            int productId = InsertPruductMain();
            if (productId > 0)
            {
                divAlert.InnerHtml += "<span style=\"color:Green\">Ürün başarıyla kaydedildi.</span><br />";
                if (SaveDocuments(productId))
                {
                    divAlert.InnerHtml += "<span style=\"color:Green\">Dokümanlar başarıyla kaydedildi.</span><br />";
                }
                else
                {
                    divAlert.InnerHtml += "<span style=\"color:Red\">Dokümanlar kaydedilirken hatalar oluştu lütfen daha sonra tekrar deneyiniz.</span><br />";
                }
                if (SaveProductProperties(productId))
                {
                    divAlert.InnerHtml += "<span style=\"color:Green\">Özellikler başarıyla kaydedildi.</span><br />";
                }
                else
                {
                    divAlert.InnerHtml += "<span style=\"color:Red\">Özellikler kaydedilirken hatalar oluştu lütfen daha sonra tekrar deneyiniz.</span><br />";
                }
                retValue = true;
            }

            return retValue;
        }

        private bool UpdateItem(int productId)
        {
            bool retValue = false;
            if (UpdatePruductMain(productId))
                retValue = true;
            return retValue;
        }

        private void ClearForm()
        {
            ddlProductCategories.SelectedIndex = -1;
            txtProductCode.Text = string.Empty;
            Session["PRODUCT_PROPERTY_LIST"] = null;
            txtProductName.Text = string.Empty;
            txtProductDescription.Text = string.Empty;
            txtMinStock.Text = string.Empty;
            txtAlertDate.Text = string.Empty;
            ddlProperties.SelectedIndex = -1;
            txtPropertyValue.Text = string.Empty;
            btnSave.CommandArgument = string.Empty;
            gvDocumentList.DataSource = null;
            gvDocumentList.DataBind();
            gvProductProperties.DataSource = null;
            gvProductProperties.DataBind();
            //RadTabStrip1.Tabs[0].Selected = true;
            //RadTabStrip1.Tabs[1].Selected = false;
            //RadTabStrip1.Tabs[2].Selected = false;
            //RadMultiPage1.PageViews[0].Selected = true;
            //RadMultiPage1.PageViews[1].Selected = false;
            //RadMultiPage1.PageViews[2].Selected = false;
            //RadPageView1.Selected = true;
            //RadPageView2.Selected = false;
            //RadPageView3.Selected = false;
        }

        #region ProductMain
        private bool GetProductMain(int productId)
        {
            bool retValue = false;
            try
            {
                var item = ProductData.Get(productId);
                lblProductId.Text = item.Id.ToString();
                ddlProductCategories.SelectedValue = item.ProductCategory.Id.ToString();
                txtProductCode.Text = item.ProductCode;
                txtProductName.Text = item.Title;
                txtProductDescription.Text = item.Description;
                txtMinStock.Text = item.MinStock.ToString();
                txtAlertDate.Text = item.AlertDate.ToShortDateString();
                retValue = true;
            }
            catch (Exception exception)
            {
                SystemLog itemSystemLog = new SystemLog();
                itemSystemLog.Title = "GetProductMain";
                itemSystemLog.Content = "Id=" + productId.ToString() + " ile alanlar doldurulamadı. Hata: " + exception.ToString();
                itemSystemLog.Type = new EnumValue() { Id = 0 };
                SystemLogData.Insert(itemSystemLog);
            }

            return retValue;
        }

        private bool SaveProductMain(int productId)
        {
            bool retValue = false;
            try
            {
                if (btnSave.CommandArgument != "")
                {
                    //güncelle                    
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
                    if (InsertPruductMain() > 0)
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

        private int InsertPruductMain()
        {
            int retValue = 0;
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
                itemProduct = new Product();
                itemProduct.AlertDate = Convert.ToDateTime(txtAlertDate.Text);
                itemProduct.CreateAdminId = 1;
                itemProduct.CreateDate = DateTime.Now;
                itemProduct.Description = txtProductDescription.Text;
                itemProduct.MinStock = DBHelper.IntValue(txtMinStock.Text);
                itemProduct.ProductCategory = new ProductCategory() { Id = DBHelper.IntValue(ddlProductCategories.SelectedValue) };
                itemProduct.ProductCode = txtProductCode.Text;
                itemProduct.Title = txtProductName.Text;
                int result = ProductData.Insert(itemProduct);
                if (result > 0)
                {
                    retValue = result;

                    //if (Session["PRODUCT_PROPERTY_LIST"] != null)
                    //{
                    //    //ayhant => yeni kayıt olduğundan önce product'tı sonra product'ın propertylerini kaydediyoruz.
                    //    List<Property> productDocumentList = (List<Property>)Session["PRODUCT_PROPERTY_LIST"];
                    //    foreach (var item in productDocumentList)
                    //    {
                    //        if (ProductPropertyData.Insert(item) > 0)
                    //        {
                    //            retValue = true;
                    //        }
                    //    }
                    //}
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
                itemProduct.Description = txtProductDescription.Text;
                itemProduct.MinStock = DBHelper.IntValue(txtMinStock.Text);
                itemProduct.ProductCategory = new ProductCategory() { Id = DBHelper.IntValue(ddlProductCategories.SelectedValue) };
                itemProduct.ProductCode = txtProductCode.Text;
                itemProduct.Title = txtProductName.Text;
                int result = ProductData.Update(itemProduct);
                if (result != 1)
                    retValue = true;
            }
            return retValue;
        }
        private bool DeleteProductMain(int productId)
        {
            bool retValue = false;
            if (ProductData.Delete(productId) > 0)
            {
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
                SystemLog itemSystemLog = new SystemLog();
                itemSystemLog.Title = "Get Document List";
                itemSystemLog.Content = productId.ToString() + " Ürün numarası ile döküman listesi getirilirken hata oluştu. Hata: " + exception.ToString();
                itemSystemLog.Type = new EnumValue() { Id = 0 };
                SystemLogData.Insert(itemSystemLog);
            }
            return retValue;
        }

        private bool GetProductDocument(int documentId)
        {
            bool retValue = false;
            var item = MultimediasData.Get(documentId);
            try
            {
                if (item != null)
                {
                    divAlert.InnerHtml = "";
                    //txtDocumentDescription.Text = item.Description;
                    //txtDocumentName.Text = item.Title;
                    divDocuments.InnerHtml = "";
                    string fileExtension = item.Title;
                    if (fileExtension == ".png" || fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".gif")
                    {
                        divDocuments.InnerHtml += "Orjinal Boyut: <a taget=\"_blank\" href=\"../ProductDocuments/Orginal/Images/" + item.FilePath + "\">Orjinal Boyutlardaki resmi görmek için tıklayınız.</a><br />";
                        divDocuments.InnerHtml += "Büyük: <a taget=\"_blank\" href=\"../ProductDocuments/Images/Big/big_" + item.FilePath + "\">Büyük boyutlardaki resmi görmek için tıklayınız.</a><br />";
                        divDocuments.InnerHtml += "Küçük: <a taget=\"_blank\" href=\"../ProductDocuments/Images/Small/small_" + item.FilePath + "\">Küçük Boyutlardaki resmi görmek için tıklayınız.</a><br />";
                        divDocuments.InnerHtml += "İkon: <a taget=\"_blank\" href=\"../ProductDocuments/Images/Icon/icon_" + item.FilePath + "\">İkon Boyutlarında resmi görmek için tıklayınız.</a><br />";
                    }
                    else
                    {
                        divDocuments.InnerHtml += "Doküman: <a taget=\"_blank\" href=\"../ProductDocuments/Orginal/Others/" + item.FilePath + "\">" + item.FilePath + "</a>";
                    }
                    retValue = true;
                }
            }
            catch (Exception exception)
            {
                SystemLog itemSystemLog = new SystemLog();
                itemSystemLog.Title = "Edit Document";
                itemSystemLog.Content = "Id=" + documentId.ToString() + " ile Doküman güncellenemedi.";
                itemSystemLog.Type = new EnumValue() { Id = 0 };
                SystemLogData.Insert(itemSystemLog);
            }
            return retValue;
        }

        private bool SaveDocuments(int productId)
        {
            bool retValue = true;
            bool isOK = false;
            foreach (Telerik.Web.UI.UploadedFile uploadedFile in ruProductDocuments.UploadedFiles)
            {
                string fileName = DateTime.Now.ToString().Replace(".", "").Replace(":", "").Replace("/", "").Replace("-", "").Replace(" ", "");
                fileName += "_" + productId.ToString();
                //Dökümanı kaydet.
                string targetFolderImage = Server.MapPath("~/ProductDocuments/Orginal/Images");
                string targetFolderOther = Server.MapPath("~/ProductDocuments/Orginal/Others");

                string targetFileNameImage = System.IO.Path.Combine(targetFolderImage, fileName + uploadedFile.GetExtension());

                //Eğer resim ise 3 farklı boyutta resize et.
                string fileExtension = uploadedFile.GetExtension();
                if (fileExtension == ".png" || fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".gif")
                {
                    try
                    {
                        uploadedFile.SaveAs(targetFileNameImage, isOK);
                        Utility.ResizeImage(targetFileNameImage, Server.MapPath("~/ProductDocuments/Images/Big/big_" + fileName + fileExtension), 500, 500, true);
                        Utility.ResizeImage(targetFileNameImage, Server.MapPath("~/ProductDocuments/Images/Small/small_" + fileName + fileExtension), 250, 250, true);
                        Utility.ResizeImage(targetFileNameImage, Server.MapPath("~/ProductDocuments/Images/Icon/icon_" + fileName + fileExtension), 50, 50, true);
                        divAlert.InnerHtml += "<span style=\"color:Green\">Dosya başarıyla yüklendi. Dosya Adı: <i>" + uploadedFile.FileName + "</i></span><br />";
                    }
                    catch (Exception exception)
                    {
                        divAlert.InnerHtml += "<span style=\"color:Red\">Dosya yüklenirken hata oluştu! Dosya Adı: <i>" + uploadedFile.FileName + "</i> Hata:" + exception.ToString() + "</span><br />";
                        SystemLog itemSystemLog = new SystemLog();
                        itemSystemLog.Title = "SaveDocuments";
                        itemSystemLog.Content = productId.ToString() + " Döküman eklerken hata oluştu. Hata: " + exception.ToString();
                        itemSystemLog.Type = new EnumValue() { Id = 0 };
                        SystemLogData.Insert(itemSystemLog);
                        retValue = false;
                    }
                }
                else
                {
                    uploadedFile.SaveAs(targetFolderOther, isOK);
                }
                if (InsertDocument(productId, fileName + fileExtension, fileExtension))
                {
                    divAlert.InnerHtml += "<span style=\"color:Green\">Dosya veritabanına başarıyla kaydedildi. Dosya Adı: <i>" + uploadedFile.FileName + "</i></span><br />";
                }
                else
                {
                    divAlert.InnerHtml += "<span style=\"color:Red\">Dosya veritabanına kaydedilerken hata oluştu! Dosya Adı: <i>" + uploadedFile.FileName + "</i></span><br />";
                }
            }
            return retValue;
        }

        private bool InsertDocument(int productId, string fileName, string fileExtension)
        {
            bool retValue = false;
            try
            {
                var item = new Multimedia();
                //item.Description = txtDocumentDescription.Text;                
                item.FilePath = fileName;
                item.Title = fileExtension;
                item.ProductId = productId;
                if (MultimediasData.Insert(item) > 0)
                {
                    retValue = true;
                }
            }
            catch (Exception exception)
            {
                SystemLog itemSystemLog = new SystemLog();
                itemSystemLog.Title = "Delete Enum";
                itemSystemLog.Content = "Doküman kaydedilemedi. Hata: " + exception.ToString();
                itemSystemLog.Type = new EnumValue() { Id = 0 };//olumsu sonuc 1 olumsuz 0
                SystemLogData.Insert(itemSystemLog);
            }
            return retValue;
        }

        //private bool UpdateDocument(int documentId)
        //{
        //    bool retValue = false;
        //    try
        //    {
        //        var item = MultimediasData.Get(documentId);
        //        item.Title = txtDocumentName.Text;
        //        item.Description = txtDocumentDescription.Text;
        //        item.EditDate = DateTime.Now;
        //        item.EditAdminId = 100;
        //        if (MultimediasData.Update(item) >= 0)
        //        {
        //            retValue = true;
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        SystemLog itemSystemLog = new SystemLog();
        //        itemSystemLog.Title = "Update Document";
        //        itemSystemLog.Content = "Id=" + documentId.ToString() + " ile Doküman güncellenemedi. Hata: " exception.ToString();
        //        itemSystemLog.Type = new EnumValue() { Id = 0 };//olumsu sonuc 1 olumsuz 0
        //        SystemLogData.Insert(itemSystemLog);
        //    }
        //    return retValue;
        //}

        private bool DeleteDocument(int documentId)
        {
            bool retValue = false;
            try
            {
                if (MultimediasData.Delete(documentId) < 0)
                {
                    retValue = true;
                }
            }
            catch (Exception exception)
            {
                SystemLog itemSystemLog = new SystemLog();
                itemSystemLog.Title = "Delete Document";
                itemSystemLog.Content = "Id=" + documentId.ToString() + " ile Doküman silinemedi. Hata: " + exception.ToString();
                itemSystemLog.Type = new EnumValue() { Id = 0 };
                SystemLogData.Insert(itemSystemLog);
            }
            return retValue;
        }
        #endregion

        #region Property
        private List<ProductProperty> GetProductPropertyList()
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

            return productPropertyList;
        }
        
        private bool GetProductProperties(int productId)
        {
            bool retValue = false;
            
            try
            {
                var productPropertyList = ProductPropertyData.GetProductProperties(productId);

                Session.Add("PRODUCT_PROPERTY_LIST", productPropertyList);
                gvProductProperties.DataSource = productPropertyList;
                gvProductProperties.DataBind();
                retValue = true;
            }
            catch (Exception exception)
            {
                SystemLog itemSystemLog = new SystemLog();
                itemSystemLog.Title = "SaveProductProperty";
                itemSystemLog.Content = "Ürün özelliği kaydedilirken hata oluştu. Hata: " + exception.ToString();
                itemSystemLog.Type = new EnumValue() { Id = 0 };
                SystemLogData.Insert(itemSystemLog);
                retValue = false;
            }
            return retValue;
        }
        private bool GetProductProperties()
        {
            bool retValue = false;
            var productPropertyList = GetProductPropertyList();
            gvProductProperties.DataSource = productPropertyList;
            gvProductProperties.DataBind();
            return retValue;
        }

        private bool GetProductProperty(int propertyId)
        {
            bool retValue = false;
            var itemProductProperty = ProductPropertyData.Get(propertyId);
            lblPropertyId.Text = itemProductProperty.Id.ToString();
            txtPropertyValue.Text = itemProductProperty.Value.ToString();
            ddlProperties.SelectedValue = itemProductProperty.Property.Id.ToString();
            btnAddProperty.CommandArgument = lblPropertyId.Text.ToString();
            GetItem(Convert.ToInt32(lblProductId.Text));
            return retValue;
        }
        private bool SaveProductPropertyToList()
        {
            bool retValue = false;
            try
            {
                var productPropertyList = GetProductPropertyList();
                if (btnAddProperty.CommandArgument != "")
                {
                    //güncelle
                    int propertyId = DBHelper.IntValue(btnAddProperty.CommandArgument);
                    if (propertyId != 0)
                    {
                        ProductProperty item = productPropertyList.Where(x => x.Id == propertyId).SingleOrDefault();
                        productPropertyList.Remove(item);
                        var newItem = new ProductProperty();
                        newItem.Id = item.Id;
                        newItem.Property = new Property()
                        {
                            Id = Convert.ToInt32(ddlProperties.SelectedValue),
                            Title = ddlProperties.SelectedItem.Text,
                        };
                        newItem.ProductId = item.ProductId;
                        newItem.Value = txtPropertyValue.Text;
                        productPropertyList.Add(newItem);
                        divAlert.InnerHtml += "<span style=\"color:Green\">Özellik güncelleme başarılı.</span><br />";
                        retValue = true;
                    }
                    else
                    {
                        divAlert.InnerHtml += "<span style=\"color:Red\">Özellik güncellenirken hata oluştu!</span><br />";
                        retValue = false;
                    }
                }
                else
                {
                    try
                    {
                        var item = new ProductProperty();
                        item.Property = new Property()
                        {
                            Id = Convert.ToInt32(ddlProperties.SelectedValue),
                            Title = ddlProperties.SelectedItem.Text,
                        };
                        item.ProductId = item.ProductId;
                        item.Value = txtPropertyValue.Text;
                        item.ProductId = 0;
                        item.Value = txtPropertyValue.Text;
                        productPropertyList.Add(item);
                        divAlert.InnerHtml += "<span style=\"color:Green\">Yeni özellik kaydı başarılı.</span><br />";
                        retValue = true;
                    }
                    catch (Exception)
                    {
                        divAlert.InnerHtml += "<span style=\"color:Red\">Yeni özellik kaydedilirken hata oluştu.</span><br />";
                        retValue = false;
                    }
                }
                GetProductProperties();
            }
            catch (Exception exception)
            {
                SystemLog itemSystemLog = new SystemLog();
                itemSystemLog.Title = "SaveProductProperty";
                itemSystemLog.Content = "Ürün özelliği kaydedilirken hata oluştu. Hata: " + exception.ToString();
                itemSystemLog.Type = new EnumValue() { Id = 0 };
                SystemLogData.Insert(itemSystemLog);
            }
            finally
            {

            }
            return retValue;
        }

        private bool SaveProductProperties(int productId)
        {
            bool retValue = false;
            try
            {
                var productPropertyList = GetProductPropertyList();

                foreach (ProductProperty itemProductProperty in productPropertyList)
                {
                    if (itemProductProperty.ProductId != 0)
                    {
                        //güncelle
                        int propertyId = DBHelper.IntValue(btnAddProperty.CommandArgument);
                        
                        if (ProductPropertyData.Update(itemProductProperty) < 0)
                        {
                            divAlert.InnerHtml += "<span style=\"color:Green\">Özellik güncelleme başarılı.</span><br />";
                            retValue = true;
                        }
                        else
                        {
                            divAlert.InnerHtml += "<span style=\"color:Red\">Özellik güncellenirken hata oluştu.</span><br />";
                            retValue = false;
                        }
                    }
                    else
                    {
                        itemProductProperty.ProductId = productId;
                        //yeni kayıt
                        if (ProductPropertyData.Insert(itemProductProperty) > 0)
                        {
                            divAlert.InnerHtml += "<span style=\"color:Green\">Yeni özellik kaydı başarılı.</span><br />";
                            retValue = true;
                        }
                        else
                        {
                            divAlert.InnerHtml += "<span style=\"color:Red\">Yeni özellik kaydedilirken hata oluştu.</span><br />";
                            retValue = false;
                        }
                    }
                }
                GetProductProperties();
            }
            catch (Exception exception)
            {
                SystemLog itemSystemLog = new SystemLog();
                itemSystemLog.Title = "SaveProductProperties";
                itemSystemLog.Content = productId.ToString() + " Hata: " + exception.ToString();
                itemSystemLog.Type = new EnumValue() { Id = 0 };
                SystemLogData.Insert(itemSystemLog);
            }
            finally
            {

            }
            return retValue;
        }
        private bool InsertPropertyToList(ProductProperty itemProperty)
        {
            bool retValue = false;
            try
            {
                var productPropertyList = GetProductPropertyList();
                productPropertyList = (List<ProductProperty>)Session["PRODUCT_PROPERTY_LIST"];
                ProductProperty item = new ProductProperty();
                item.Id = 0;//Yeni kayıt.
                item.ProductId = itemProperty.ProductId;
                item.CreateAdminId = itemProperty.CreateAdminId;
                item.Property = new Property()
                {
                    Id = Convert.ToInt32(ddlProperties.SelectedValue),
                    Title = ddlProperties.SelectedItem.Text,
                };
                item.Value = txtPropertyValue.Text;
                productPropertyList.Add(item);
                retValue = true;
            }
            catch (Exception exception)
            {
                SystemLog itemSystemLog = new SystemLog();
                itemSystemLog.Title = "InsertProperty";
                itemSystemLog.Content = "Hata: " + exception.ToString();
                itemSystemLog.Type = new EnumValue() { Id = 0 };
                SystemLogData.Insert(itemSystemLog);
            }
            return retValue;
        }
        private bool DeletePorperty(int PropertyId)
        {
            bool retValue = false;
            try
            {
                if (ProductPropertyData.Delete(PropertyId) < 0)
                {
                    retValue = true;
                }
            }
            catch (Exception exception)
            {
                SystemLog itemSystemLog = new SystemLog();
                itemSystemLog.Title = "Delete Document";
                itemSystemLog.Content = "Id=" + PropertyId.ToString() + " ile Doküman silinemedi. Hata: " + exception.ToString();
                itemSystemLog.Type = new EnumValue() { Id = 0 };
                SystemLogData.Insert(itemSystemLog);
            }
            return retValue;
        }
        #endregion
    }
}