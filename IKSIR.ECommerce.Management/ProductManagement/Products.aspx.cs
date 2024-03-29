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
using IKSIR.ECommerce.Model.SiteModel;
using IKSIR.ECommerce.Infrastructure.DataLayer.SiteDataLayer;

namespace IKSIR.ECommerce.Management.ProductManagement
{
    public partial class Products : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindValues();
               // GetList();
            }
        }

        private bool GetItem(int itemId)
        {
            bool retValue = false;

            pnlForm.Visible = true;

            if (GetProductMain(itemId))
            {
                divAlert.InnerHtml = "<span style=\"color:Green\">Ürün Genel bilgileri başarıyla yüklendi.</span><br />";
                retValue = true;
            }
            else
            {
                divAlert.InnerHtml = "<span style=\"color:Red\">Ürün Genel bilgileri yüklenirken hata oluştu.</span><br />" + divAlert.InnerHtml;
            }

            if (GetProductDocuments(itemId))
            {
                divAlert.InnerHtml = "<span style=\"color:Green\">Ürün resimleri başarıyla yüklendi.</span><br />" + divAlert.InnerHtml;
                retValue = true;
            }
            else
            {
                divAlert.InnerHtml = "<span style=\"color:Red\">Ürün resimleri yüklenirken hata oluştu.</span><br />" + divAlert.InnerHtml;
            }

            if (GetProductFiles(itemId))
            {
                divAlert.InnerHtml = "<span style=\"color:Green\">Ürün dökümanları başarıyla yüklendi.</span><br />" + divAlert.InnerHtml;
                retValue = true;
            }
            else
            {
                divAlert.InnerHtml = "<span style=\"color:Red\">Ürün dökümanları yüklenirken hata oluştu.</span><br />" + divAlert.InnerHtml;
            }

            if (GetProductProperties(itemId))
            {
                divAlert.InnerHtml = "<span style=\"color:Green\">Ürün özellikleri başarıyla yüklendi.</span><br />" + divAlert.InnerHtml;
                retValue = true;
            }
            else
            {
                divAlert.InnerHtml = "<span style=\"color:Red\">Ürün özellikleri yüklenirken hata oluştu.</span><br />" + divAlert.InnerHtml;
            }

            if (GetProductRelated(itemId))
            {
                divAlert.InnerHtml = "<span style=\"color:Green\">Ürün ilişkili ürünleri başarıyla yüklendi.</span><br />" + divAlert.InnerHtml;
                retValue = true;
            }
            else
            {
                divAlert.InnerHtml = "<span style=\"color:Red\">Ürün ilişkili ürünleriyüklenirken hata oluştu.</span><br />" + divAlert.InnerHtml;
            }
            if (GetProductSimilar(itemId))
            {
                divAlert.InnerHtml = "<span style=\"color:Green\">Ürün Benzer ürünleri başarıyla yüklendi.</span><br />" + divAlert.InnerHtml;
                retValue = true;
            }
            else
            {
                divAlert.InnerHtml = "<span style=\"color:Red\">Ürün Benzer ürünleriyüklenirken hata oluştu.</span><br />" + divAlert.InnerHtml;
            }
            return retValue;
        }
        private void GetList()
        {
            List<Product> itemList = ProductData.GetList();
            if (ddlFilterParentCategories.SelectedValue != "-1" && ddlFilterParentCategories.SelectedValue != "")
            {
                int parentCategoryId = DBHelper.IntValue(ddlFilterParentCategories.SelectedValue);
                itemList = itemList.Where(x => x.ProductCategory.ParentCategory.Id == parentCategoryId).ToList();
            }

            if (ddlFilterCategories.SelectedValue != "-1" && ddlFilterCategories.SelectedValue != "")
            {
                int CategoryId = DBHelper.IntValue(ddlFilterCategories.SelectedValue);
                itemList = itemList.Where(x => x.ProductCategory.Id == CategoryId).ToList();
            }
            if (txtFilterProductCode.Text != "")
            {
                string productCode = txtFilterProductCode.Text;
                itemList = itemList.Where(x => x.ProductCode == productCode).ToList();
            }
            gvList.DataSource = itemList;
            gvList.DataBind();
            ClearForm();
            pnlForm.Visible = false;

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

        private void BindValues()
        {

            List<Site> itemListSite = SiteData.GetSiteList();
            Utility.BindDropDownList(ddlFilterSite, itemListSite, "Name", "Id");
            List<ProductCategory> itemList = ProductCategoryData.GetProductCategoryList();
            ddlFilterParentCategories.Enabled = false;
            ddlProductCategories.Enabled = false;
            ddlFilterCategories.Enabled = false;
            Utility.BindDropDownList(ddlFilterParentCategories, itemList, "Title", "Id");

            List<Property> ProductPropertyList = PropertyData.GetList();
            Utility.BindDropDownList(ddlProperties, ProductPropertyList, "Title", "Id");

            List<EnumValue> itemStokStatus = EnumValueData.GetEnumValues(12);
            Utility.BindDropDownList(ddlStokStatus, itemStokStatus, "Value", "Id");

            List<EnumValue> itemProductStatus = EnumValueData.GetEnumValues(13);//ürün durumu
            Utility.BindDropDownList(ddlProductStatus, itemProductStatus, "Value", "Id");

            List<ProductCategory> itemPList = ProductCategoryData.GetParentProductCategoryList();
            Utility.BindDropDownList(ddlParentProductCategories, itemPList, "Title", "Id");
        }

        protected void ddlFilterSites_SelectedIndexChanged(object sender, EventArgs e)
        {

            ddlFilterParentCategories.Items.Clear();
            ddlFilterParentCategories.Items.Clear();
            List<ProductCategory> itemList = ProductCategoryData.GetGetParentProductCategoryListBySiteId(Convert.ToInt32(ddlFilterSite.SelectedValue));

            Utility.BindDropDownList(ddlFilterParentCategories, itemList, "Title", "Id");
            ddlFilterParentCategories.Enabled = true;
        }
        protected void ddlParentProductCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlProductCategories.Items.Clear();
            List<ProductCategory> itemList = ProductCategoryData.GetProductCategoryById(Convert.ToInt32(ddlParentProductCategories.SelectedValue));

            Utility.BindDropDownList(ddlProductCategories, itemList, "Title", "Id");
            ddlProductCategories.Enabled = true;
        }

        protected void lbtnNew_Click(object sender, EventArgs e)
        {
            ClearForm();
            lblProductId.Text = "Yeni Kayıt";
            lblPropertyId.Text = "Yeni Kayıt";
            pnlForm.Visible = true;
            ddlProductCategories.Focus();
            List<EnumValue> itemStokStatus = EnumValueData.GetEnumValues(12);
            Utility.BindDropDownList(ddlStokStatus, itemStokStatus, "Value", "Id");

            List<EnumValue> itemProductStatus = EnumValueData.GetEnumValues(13);//ürün durumu
            Utility.BindDropDownList(ddlProductStatus, itemProductStatus, "Value", "Id");

            List<ProductCategory> itemPList = ProductCategoryData.GetParentProductCategoryList();
            Utility.BindDropDownList(ddlParentProductCategories, itemPList, "Title", "Id");
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
            SaveFiles(productId);
            SaveDocuments(productId);
            SaveProductProperties(productId);
            SaveProductRelated(productId);
            SaveProductSimilar(productId);
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

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            GetList();
        }

        #region ProductMain
        private bool GetProductMain(int productId)
        {
            bool retValue = false;
            try
            {
                ddlParentProductCategories.Enabled = true;
                ddlProductCategories.Enabled = true;
                var item = ProductData.Get(productId);
                lblProductId.Text = item.Id.ToString();
                List<ProductCategory> itemList = ProductCategoryData.GetParentProductCategoryList();
                Utility.BindDropDownList(ddlParentProductCategories, itemList, "Title", "Id");
                ddlParentProductCategories.SelectedValue = item.ProductCategory.ParentCategory.Id.ToString();
                List<ProductCategory> itemCategory = ProductCategoryData.GetProductCategoryById(Convert.ToInt32(ddlParentProductCategories.SelectedValue));
                Utility.BindDropDownList(ddlProductCategories, itemCategory, "Title", "Id");

                ddlProductCategories.SelectedValue = item.ProductCategory.Id.ToString();
                txtProductCode.Text = item.ProductCode;
                txtProductName.Text = item.Title;
                txtDesi.Text = item.Desi;
                txtVideo.Text = item.Video.ToString();
                List<EnumValue> itemStokStatus = EnumValueData.GetEnumValues(12);//stok durumu
                Utility.BindDropDownList(ddlStokStatus, itemStokStatus, "Value", "Id");

                List<EnumValue> itemProductStatus = EnumValueData.GetEnumValues(13);//ürün durumu
                Utility.BindDropDownList(ddlProductStatus, itemProductStatus, "Value", "Id");

                ddlProductStatus.SelectedValue = item.ProductStatus.Id.ToString();
                txtStok.Text = item.Stok.ToString();
                txtMaxQuantity.Text = item.MaxQuantity.ToString();
                ddlStokStatus.SelectedValue = item.StokStatus.Id.ToString();
                txtProductDescription.Text = item.Description;
                txtMinStock.Text = item.MinStock.ToString();
                txtguarantee.Text = item.Guarantee.ToString();
                txtAlertDate.SelectedDate = Convert.ToDateTime(item.AlertDate.ToShortDateString());
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
                itemProduct.AlertDate = Convert.ToDateTime(txtAlertDate.SelectedDate);
                itemProduct.CreateAdminId = 1;
                itemProduct.CreateDate = DateTime.Now;
                itemProduct.Description = txtProductDescription.Text;
                itemProduct.MinStock = DBHelper.IntValue(txtMinStock.Text);
                itemProduct.ProductCategory = new ProductCategory() { Id = DBHelper.IntValue(ddlProductCategories.SelectedValue) };
                itemProduct.ProductCode = txtProductCode.Text;
                itemProduct.Video = txtVideo.Text;
                itemProduct.Desi = txtDesi.Text;
                itemProduct.Stok = Convert.ToInt32(txtStok.Text);
                itemProduct.MaxQuantity = Convert.ToInt32(txtMaxQuantity.Text);
                itemProduct.StokStatus = new EnumValue() { Id = Convert.ToInt32(ddlStokStatus.SelectedValue) };
                itemProduct.ProductStatus = new EnumValue() { Id = Convert.ToInt32(ddlProductStatus.SelectedValue) };
                itemProduct.Guarantee = Convert.ToInt32(txtguarantee.Text);
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
                itemProduct.AlertDate = Convert.ToDateTime(txtAlertDate.SelectedDate);
                itemProduct.CreateAdminId = 1;
                itemProduct.CreateDate = DateTime.Now;
                itemProduct.Description = txtProductDescription.Text;
                itemProduct.MinStock = DBHelper.IntValue(txtMinStock.Text);
                itemProduct.ProductCategory = new ProductCategory() { Id = DBHelper.IntValue(ddlProductCategories.SelectedValue) };
                itemProduct.ProductCode = txtProductCode.Text;
                itemProduct.Video = txtVideo.Text;
                itemProduct.Title = txtProductName.Text;
                itemProduct.Desi = txtDesi.Text;
                itemProduct.Stok = Convert.ToInt32(txtStok.Text);
                itemProduct.MaxQuantity = Convert.ToInt32(txtMaxQuantity.Text);
                itemProduct.StokStatus = new EnumValue() { Id = Convert.ToInt32(ddlStokStatus.SelectedValue) };
                itemProduct.ProductStatus = new EnumValue() { Id = Convert.ToInt32(ddlProductStatus.SelectedValue) };

                itemProduct.Guarantee = Convert.ToInt32(txtguarantee.Text);
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

        protected void lbtnDocumentEdit_Click(object sender, EventArgs e)
        {
            ruProductDocuments.Visible = false;
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

        protected void lbtnDocumentUsed_Click(object sender, EventArgs e)
        {
            var itemId = (sender as LinkButton).CommandArgument == "" ? 0 : Convert.ToInt32((sender as LinkButton).CommandArgument);
            if (UsedDocument(itemId))
            {
                divAlert.InnerHtml += "<span style=\"color:Green\">Dosya başarıyla varsayılan yapıldı</span><br />";
                GetItem(Convert.ToInt32(lblProductId.Text));
            }
            else
            {
                divAlert.InnerHtml += "<span style=\"color:Red\">Dosya varsayılan yapılırken hata oluştu!</span><br />";
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

        private bool GetProductDocuments(int productId)
        {
            bool retValue = false;
            try
            {
                var productDocumentList = MultimediasData.GetItemMultimedias(3, productId); //3 Product EnumValueId ayhant
                gvDocumentList.DataSource = productDocumentList;
                gvDocumentList.DataBind();
                foreach (GridViewRow r in gvDocumentList.Rows)
                {
                    if (r.Cells[3].Text == "True")
                    {
                        r.Cells[3].Text = "Evet";
                        r.FindControl("lbtnUsed").Visible = false;
                    }
                    else
                    {
                        r.Cells[3].Text = "Hayır";
                    }

                }

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
                        divDocuments.InnerHtml += "Orjinal Boyut: <a target=\"_blank\" href=\"../ProductDocuments/Orginal/Images/" + item.FilePath + "\">Orjinal Boyutlardaki resmi görmek için tıklayınız.</a><br />";
                        divDocuments.InnerHtml += "Büyük: <a target=\"_blank\" href=\"../ProductDocuments/Images/Big/big_" + item.FilePath + "\">Büyük boyutlardaki resmi görmek için tıklayınız.</a><br />";
                        divDocuments.InnerHtml += "Küçük: <a target=\"_blank\" href=\"../ProductDocuments/Images/Small/small_" + item.FilePath + "\">Küçük Boyutlardaki resmi görmek için tıklayınız.</a><br />";
                        divDocuments.InnerHtml += "İkon: <a target=\"_blank\" href=\"../ProductDocuments/Images/Icon/icon_" + item.FilePath + "\">İkon Boyutlarında resmi görmek için tıklayınız.</a><br />";
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
            int i = 0;
            bool isDefault = false;
            foreach (Telerik.Web.UI.UploadedFile uploadedFile in ruProductDocuments.UploadedFiles)
            {
                System.Threading.Thread.Sleep(1000);
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
                        Utility.ResizeImage(targetFileNameImage, Server.MapPath("~/ProductDocuments/Images/Big/big_" + fileName + fileExtension), 350, 250, true);
                        Utility.ResizeImage(targetFileNameImage, Server.MapPath("~/ProductDocuments/Images/Small/small_" + fileName + fileExtension), 168, 120, true);
                        Utility.ResizeImage(targetFileNameImage, Server.MapPath("~/ProductDocuments/Images/Icon/icon_" + fileName + fileExtension), 75, 50, true);
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
                if (i < 1)
                    isDefault = true;
                else
                    isDefault = false;

                if (InsertDocument(productId, fileName + fileExtension, fileExtension, isDefault))
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

        private bool InsertDocument(int productId, string fileName, string fileExtension, bool isDefault)
        {
            bool retValue = false;
            try
            {
                var item = new Multimedia();
                //item.Description = txtDocumentDescription.Text;                
                item.FilePath = fileName;
                item.Title = fileExtension;
                item.ProductId = productId;
                item.IsDefault = isDefault;
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
            var productDocumentList = MultimediasData.Get(documentId);

            if (productDocumentList.IsDefault)
            {
                divAlert.InnerHtml += "<span style=\"color:Red\">Bu Resim Varsayılan Silemezsiniz</span><br />";
                retValue = false;
            }
            else
            {
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
            }
            return retValue;
        }

        private bool UsedDocument(int documentId)
        {
            bool retValue = false;
            try
            {
                if (MultimediasData.IsDefault(documentId) < 0)
                {
                    retValue = true;
                }
            }
            catch (Exception exception)
            {
                SystemLog itemSystemLog = new SystemLog();
                itemSystemLog.Title = "Used Document";
                itemSystemLog.Content = "Id=" + documentId.ToString() + " ile Doküman silinemedi. Hata: " + exception.ToString();
                itemSystemLog.Type = new EnumValue() { Id = 0 };
                SystemLogData.Insert(itemSystemLog);
            }
            return retValue;
        }

        #endregion

        #region Files

        protected void lbtnFileEdits_Click(object sender, EventArgs e)
        {
            var index = ((sender as LinkButton).Parent.Parent as GridViewRow).RowIndex;
            gvList.SelectedIndex = index;
            var fileId = (sender as LinkButton).CommandArgument == ""
                             ? 0
                             : Convert.ToInt32((sender as LinkButton).CommandArgument);
            if (!GetProductFile(fileId))
            {
                divAlert.InnerHtml += "<span style=\"color:Red\">Dosya bilgilerini getirirken hata oluştu!</span><br />";
            }
        }

        protected void lbtnFileDelete_Click(object sender, EventArgs e)
        {
            var itemId = (sender as LinkButton).CommandArgument == "" ? 0 : Convert.ToInt32((sender as LinkButton).CommandArgument);
            if (DeleteFile(itemId))
            {
                divAlert.InnerHtml += "<span style=\"color:Green\">Dosya başarıyla silindi</span><br />";
                GetItem(Convert.ToInt32(lblProductId.Text));
            }
            else
            {
                divAlert.InnerHtml += "<span style=\"color:Red\">Dosya silinirken hata oluştu!</span><br />";
            }
        }

        private bool GetProductFiles(int productId)
        {
            bool retValue = false;
            try
            {
                var productDocumentList = FileData.GetItemFiles(3, productId); //3 Product EnumValueId ayhant
                gvProductFiles.DataSource = productDocumentList;
                gvProductFiles.DataBind();
                retValue = true;
            }
            catch (Exception exception)
            {
                SystemLog itemSystemLog = new SystemLog();
                itemSystemLog.Title = "Get Document List";
                itemSystemLog.Content = productId.ToString() + " Ürün numarası ile dosya listesi getirilirken hata oluştu. Hata: " + exception.ToString();
                itemSystemLog.Type = new EnumValue() { Id = 0 };
                SystemLogData.Insert(itemSystemLog);
            }
            return retValue;
        }

        private bool GetProductFile(int fileId)
        {
            bool retValue = false;
            var item = FileData.Get(fileId);
            try
            {
                if (item != null)
                {
                    divAlert.InnerHtml = "";
                    lblFileId.Text = item.Id.ToString();
                    txtDocumentDescription.Text = item.Description;
                    txtDocumentName.Text = item.Title;
                    divFiles.InnerHtml = "";
                    string fileExtension = item.Title;

                    {
                        divFiles.InnerHtml += "Doküman: <a taget=\"_blank\" href=\"../ProductDocuments/Orginal/Others/" + item.FilePath + "\">" + item.FilePath + "</a>";
                    }
                    retValue = true;
                }
            }
            catch (Exception exception)
            {
                SystemLog itemSystemLog = new SystemLog();
                itemSystemLog.Title = "Edit File";
                itemSystemLog.Content = "Id=" + fileId.ToString() + " ile Dosya güncellenemedi.";
                itemSystemLog.Type = new EnumValue() { Id = 0 };
                SystemLogData.Insert(itemSystemLog);
            }
            return retValue;
        }

        private bool SaveFiles(int fileId)
        {
            bool retValue = true;
            bool isOK = false;
            int i = 0;
            bool isDefault = false;
            foreach (Telerik.Web.UI.UploadedFile uploadedFile in RadUploadDocument.UploadedFiles)
            {
                System.Threading.Thread.Sleep(1000);
                string fileName = "";// DateTime.Now.ToString().Replace(".", "").Replace(":", "").Replace("/", "").Replace("-", "").Replace(" ", "");
                if (txtDocumentName.Text == "")
                    fileName = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString() + "_" + fileId.ToString();
                else
                    fileName = txtDocumentName.Text;

                //Dökümanı kaydet.
                string targetFolderOther = Server.MapPath("~/ProductDocuments/Orginal/Others");

                //Eğer resim ise 3 farklı boyutta resize et.
                string fileExtension = uploadedFile.GetExtension();

                string targetFileNameOther = System.IO.Path.Combine(targetFolderOther, fileName + uploadedFile.GetExtension());
                uploadedFile.SaveAs(targetFileNameOther, isOK);
                if (i < 1)
                    isDefault = true;
                else
                    isDefault = false;

                if (InsertFile(fileId,fileName, fileName + fileExtension, fileExtension, isDefault))
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

        private bool InsertFile(int fileId,string fileName, string file, string fileExtension, bool isDefault)
        {
            bool retValue = false;
            try
            {
                var item = new Files();
                item.Description = txtDocumentDescription.Text;                
                item.FilePath = file;
                item.Title = fileName;
                item.ProductId = fileId;
                if (FileData.Insert(item) > 0)
                {
                    retValue = true;
                }

            }
            catch (Exception exception)
            {
                SystemLog itemSystemLog = new SystemLog();
                itemSystemLog.Title = "Insert File";
                itemSystemLog.Content = "File kaydedilemedi. Hata: " + exception.ToString();
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

        private bool DeleteFile(int fileId)
        {
            bool retValue = false;
            try
            {
                if (FileData.Delete(fileId) < 0)
                {
                    retValue = true;
                }
            }
            catch (Exception exception)
            {
                SystemLog itemSystemLog = new SystemLog();
                itemSystemLog.Title = "Delete File";
                itemSystemLog.Content = "Id=" + fileId.ToString() + " ile Dosya silinemedi. Hata: " + exception.ToString();
                itemSystemLog.Type = new EnumValue() { Id = 0 };
                SystemLogData.Insert(itemSystemLog);
            }
            return retValue;
        }

        #endregion

        #region Property

        protected void btnAddProperty_Click(object sender, EventArgs e)
        {
            SaveProductPropertyToList();
            ddlProperties.SelectedIndex = -1;
            txtPropertyValue.Text = string.Empty;
            btnAddProperty.CommandArgument = "";
            lblPropertyId.Text = "Yeni Kayıt";
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
            RadTabStrip1.SelectedIndex = 1;
            RadPageView3.Selected = true;
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
        protected void btnSearch_Click(object sender, EventArgs e)
        {

            if (txtRProductCode.Text != "")
            {
                var itemProduct = ProductData.Get(ProductData.FindProductId(txtRProductCode.Text));

                if (itemProduct.Title != null)
                {
                    txtRProductName.Text = itemProduct.Title.ToString();
                    lblhRelatedProductId.Text = itemProduct.Id.ToString();
                }

                
            }
        }
        protected void btnSSearch_Click(object sender, EventArgs e)
        {

            if (txtSProductCode.Text != "")
            {
                var itemProduct = ProductData.Get(ProductData.FindProductId(txtSProductCode.Text));

                if (itemProduct.Title != null)
                {
                    txtSProductName.Text = itemProduct.Title.ToString();
                    lblhSimilarProductId.Text = itemProduct.Id.ToString();
                }
            }
        }
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

        #region RelatedProduct

        protected void btnAddRelatedProduct_Click(object sender, EventArgs e)
        {
            SaveProductRelatedToList();

            txtPropertyValue.Text = string.Empty;
            btnAddRelatedProduct.CommandArgument = "";
            txtRProductCode.Text = "";
            lblRelatedProductId.Text = "Yeni Kayıt";
            txtRProductName.Text = "";
        }
        protected void lbtnRelatedProductDelete_Click(object sender, EventArgs e)
        {
            var itemId = (sender as LinkButton).CommandArgument == "" ? 0 : Convert.ToInt32((sender as LinkButton).CommandArgument);
            if (DeleteProductRelated(itemId))
            {
                divAlert.InnerHtml += "<span style=\"color:Green\">Dosya başarıyla silindi</span><br />";
                GetItem(Convert.ToInt32(lblProductId.Text));
            }
            else
            {
                divAlert.InnerHtml += "<span style=\"color:Red\">Dosya silinirken hata oluştu!</span><br />";
            }
        }

        private List<Product> GetProductRelatedList()
        {
            List<Product> productRelatedList;
            if (Session["PRODUCT_RELATED_LIST"] != null)
            {
                productRelatedList = (List<Product>)Session["PRODUCT_RELATED_LIST"];
            }
            else
            {
                productRelatedList = new List<Product>();
                Session.Add("PRODUCT_RELATED_LIST", productRelatedList);
            }
            productRelatedList = (List<Product>)Session["PRODUCT_RELATED_LIST"];

            return productRelatedList;
        }

        private bool GetProductRelated(int productId)
        {
            bool retValue = false;

            try
            {

                var productRelatedList = RelatedProductData.GetRelatedProductList(productId);

                Session.Add("PRODUCT_RELATED_LIST", productRelatedList);
                grvRelatedProduct.DataSource = productRelatedList;
                grvRelatedProduct.DataBind();
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
        private bool GetProductRelated()
        {
            bool retValue = false;
            var productRelatedList = GetProductRelatedList();
            grvRelatedProduct.DataSource = productRelatedList;
            grvRelatedProduct.DataBind();
            return retValue;
        }

        private bool SaveProductRelatedToList()
        {
            bool retValue = false;
            try
            {
                var productRelatedList = GetProductRelatedList();
                if (btnAddRelatedProduct.CommandArgument != "")
                {

                }
                else
                {
                    try
                    {
                        var item = new Product();
                        item.Id = Convert.ToInt32(lblhRelatedProductId.Text);
                        item.Title = txtRProductName.Text;
                        item.ProductCode = txtRProductCode.Text;
                        productRelatedList.Add(item);
                        divAlert.InnerHtml += "<span style=\"color:Green\">Yeni ilişkili ürün kaydı başarılı.</span><br />";
                        retValue = true;
                    }
                    catch (Exception)
                    {
                        divAlert.InnerHtml += "<span style=\"color:Red\">Yeni ilişkili ürün hata oluştu.</span><br />";
                        retValue = false;
                    }
                }
                GetProductRelated();
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

        private bool SaveProductRelated(int productId)
        {
            bool retValue = false;
            try
            {
                var productRelatedList = GetProductRelatedList();

                var productList = RelatedProductData.GetRelatedProductList(productId);

                foreach (Product itemProductRelated in productRelatedList)
                {
                    //yeni kayıt

                    if (RelatedProductData.Insert(productId, itemProductRelated.Id) > 0)
                    {
                        divAlert.InnerHtml += "<span style=\"color:Green\">Yeni ilişkili ürün kaydı başarılı.</span><br />";
                        retValue = true;
                    }
                    else
                    {
                        divAlert.InnerHtml += "<span style=\"color:Red\">Yeni ilişkili ürün kaydedilirken hata oluştu.</span><br />";
                        retValue = false;
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
        private bool InsertProductRelatedToList(Product itemRelateProduct)
        {
            bool retValue = false;
            try
            {
                var productRelatedList = GetProductRelatedList();
                productRelatedList = (List<Product>)Session["PRODUCT_RELATED_LIST"];
                Product item = new Product();
                item.Id = 0;//Yeni kayıt.
                item.Title = itemRelateProduct.Title;
                item.ProductCode = itemRelateProduct.ProductCode;

                productRelatedList.Add(item);
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
        private bool DeleteProductRelated(int ProductRelatedId)
        {
            bool retValue = false;
            try
            {
                if (lblProductId.Text != "")
                {
                    if (RelatedProductData.Delete(Convert.ToInt32(lblProductId.Text), ProductRelatedId) < 0)
                    {
                        retValue = true;
                    }
                }
            }
            catch (Exception exception)
            {
                SystemLog itemSystemLog = new SystemLog();
                itemSystemLog.Title = "Delete Document";
                itemSystemLog.Content = "Id=" + ProductRelatedId.ToString() + " ile Doküman silinemedi. Hata: " + exception.ToString();
                itemSystemLog.Type = new EnumValue() { Id = 0 };
                SystemLogData.Insert(itemSystemLog);
            }
            return retValue;
        }
        #endregion

        #region SimilarProduct

        protected void btnAddSimilarProduct_Click(object sender, EventArgs e)
        {
            SaveProductSimilarToList();

            txtPropertyValue.Text = string.Empty;
            btnAddRelatedProduct.CommandArgument = "";
            txtSProductCode.Text = "";
            lblSimilarProductId.Text = "Yeni Kayıt";
            txtSProductName.Text = "";
        }
        protected void lbtnSimilarProductDelete_Click(object sender, EventArgs e)
        {
            var itemId = (sender as LinkButton).CommandArgument == "" ? 0 : Convert.ToInt32((sender as LinkButton).CommandArgument);
            if (DeleteProductSimilar(itemId))
            {
                divAlert.InnerHtml += "<span style=\"color:Green\">Dosya başarıyla silindi</span><br />";
                GetItem(Convert.ToInt32(lblProductId.Text));
            }
            else
            {
                divAlert.InnerHtml += "<span style=\"color:Red\">Dosya silinirken hata oluştu!</span><br />";
            }
        }

        private List<Product> GetProductSimilarList()
        {
            List<Product> productSimilarList;
            if (Session["PRODUCT_SIMILAR_LIST"] != null)
            {
                productSimilarList = (List<Product>)Session["PRODUCT_SIMILAR_LIST"];
            }
            else
            {
                productSimilarList = new List<Product>();
                Session.Add("PRODUCT_SIMILAR_LIST", productSimilarList);
            }
            productSimilarList = (List<Product>)Session["PRODUCT_SIMILAR_LIST"];

            return productSimilarList;
        }

        private bool GetProductSimilar(int productId)
        {
            bool retValue = false;

            try
            {

                var productSimilarList = SimilarProductData.GetSimilarProductList(productId);

                Session.Add("PRODUCT_SIMILAR_LIST", productSimilarList);
                grvSimilarProduct.DataSource = productSimilarList;
                grvSimilarProduct.DataBind();
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
        private bool GetProductSimilars()
        {
            bool retValue = false;
            var productSimilarList = GetProductSimilarList();
            grvSimilarProduct.DataSource = productSimilarList;
            grvSimilarProduct.DataBind();
            return retValue;
        }

        private bool SaveProductSimilarToList()
        {
            bool retValue = false;
            try
            {
                var productSimilarList = GetProductSimilarList();
                if (btnAddSimilarProduct.CommandArgument != "")
                {

                }
                else
                {
                    try
                    {
                        var item = new Product();
                        item.Id = Convert.ToInt32(lblhSimilarProductId.Text);
                        item.Title = txtSProductName.Text;
                        item.ProductCode = txtSProductCode.Text;
                        productSimilarList.Add(item);
                        divAlert.InnerHtml += "<span style=\"color:Green\">Yeni ilişkili ürün kaydı başarılı.</span><br />";
                        retValue = true;
                    }
                    catch (Exception)
                    {
                        divAlert.InnerHtml += "<span style=\"color:Red\">Yeni ilişkili ürün hata oluştu.</span><br />";
                        retValue = false;
                    }
                }
                GetProductSimilars();
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

        private bool SaveProductSimilar(int productId)
        {
            bool retValue = false;
            try
            {
                var productSimilarList = GetProductSimilarList();

                var productList = SimilarProductData.GetSimilarProductList(productId);

                foreach (Product itemProductSimilar in productSimilarList)
                {
                    //yeni kayıt

                    if (SimilarProductData.Insert(productId, itemProductSimilar.Id) > 0)
                    {
                        divAlert.InnerHtml += "<span style=\"color:Green\">Yeni Benzer ürün kaydı başarılı.</span><br />";
                        retValue = true;
                    }
                    else
                    {
                        divAlert.InnerHtml += "<span style=\"color:Red\">Yeni Benzer ürün kaydedilirken hata oluştu.</span><br />";
                        retValue = false;
                    }
                }
                GetProductSimilars();
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
        private bool InsertProductSimilarToList(Product itemSimilarProduct)
        {
            bool retValue = false;
            try
            {
                var productSimilarList = GetProductSimilarList();
                productSimilarList = (List<Product>)Session["PRODUCT_SIMILAR_LIST"];
                Product item = new Product();
                item.Id = 0;//Yeni kayıt.
                item.Title = itemSimilarProduct.Title;
                item.ProductCode = itemSimilarProduct.ProductCode;

                productSimilarList.Add(item);
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
        private bool DeleteProductSimilar(int ProductSimilarId)
        {
            bool retValue = false;
            try
            {
                if (lblProductId.Text != "")
                {
                    if (SimilarProductData.Delete(Convert.ToInt32(lblProductId.Text), ProductSimilarId) < 0)
                    {
                        retValue = true;
                    }
                }
            }
            catch (Exception exception)
            {
                SystemLog itemSystemLog = new SystemLog();
                itemSystemLog.Title = "Delete Document";
                itemSystemLog.Content = "Id=" + ProductSimilarId.ToString() + " ile Doküman silinemedi. Hata: " + exception.ToString();
                itemSystemLog.Type = new EnumValue() { Id = 0 };
                SystemLogData.Insert(itemSystemLog);
            }
            return retValue;
        }
        #endregion

        protected void gvList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvList.PageIndex = e.NewPageIndex;
            GetList();
        }

        private void ClearForm()
        {
            ruProductDocuments.Visible = true;
            divDocuments.InnerHtml = "";
            ddlProductCategories.SelectedIndex = -1;
            ddlParentProductCategories.Items.Clear();
            ddlProductCategories.Items.Clear();
            ddlProductCategories.Enabled = false;
            ddlStokStatus.Items.Clear();
            ddlProductStatus.Items.Clear();
            txtProductCode.Text = string.Empty;
            Session["PRODUCT_PROPERTY_LIST"] = null;
            Session["PRODUCT_RELATED_LIST"] = null;
            Session["PRODUCT_SIMILAR_LIST"] = null;
            txtProductName.Text = string.Empty;
            txtProductDescription.Text = string.Empty;
            txtMinStock.Text = string.Empty;
            txtVideo.Text = string.Empty;
            txtRProductCode.Text = string.Empty;
            txtRProductName.Text = string.Empty;
            txtSProductCode.Text = string.Empty;
            txtSProductName.Text = string.Empty;
            txtguarantee.Text = string.Empty;
            txtAlertDate.DbSelectedDate = string.Empty;
            ddlProperties.SelectedIndex = -1;
            txtPropertyValue.Text = string.Empty;
            txtStok.Text = string.Empty;
            txtMaxQuantity.Text = string.Empty;
            btnSave.CommandArgument = string.Empty;
            grvRelatedProduct.DataSource = null;
            grvRelatedProduct.DataBind();
            gvDocumentList.DataSource = null;
            gvDocumentList.DataBind();
            gvProductProperties.DataSource = null;
            gvProductProperties.DataBind();
            RadTabStrip1.SelectedIndex = 0;
            RadPageView1.Selected = true;
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

        protected void ddlFilterParentCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            //ddlFilterCategories
            ddlFilterCategories.Items.Clear();
            List<ProductCategory> itemList = ProductCategoryData.GetProductCategoryById(Convert.ToInt32(ddlFilterParentCategories.SelectedValue));

            Utility.BindDropDownList(ddlFilterCategories, itemList, "Title", "Id");
            ddlFilterCategories.Enabled = true;
        }
    }
}