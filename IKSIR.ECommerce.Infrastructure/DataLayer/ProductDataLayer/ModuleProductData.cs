﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using IKSIR.ECommerce.Infrastructure.DataLayer.DataBlock;
using IKSIR.ECommerce.Model.ProductModel;
using IKSIR.ECommerce.Model.SiteModel;
using IKSIR.ECommerce.Infrastructure.DataLayer.ProductDataLayer;
using IKSIR.ECommerce.Infrastructure.DataLayer.CommonDataLayer;
using IKSIR.ECommerce.Model.CommonModel;


namespace IKSIR.ECommerce.Infrastructure.DataLayer.ProductDataLayer
{
    public class ModuleProductData
    {
        public static ModuleProduct Get(ModuleProduct itemModuleProduct)
        {
            var returnValue = new ModuleProduct();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@ModuleId", itemModuleProduct.Module.Id));
            parameters.Add(new SqlParameter("@ProductId", itemModuleProduct.Product.Id));

            SqlDataReader dr = SQLDataBlock.ExecuteReader(StaticData.Idevit.ConnectionString, CommandType.StoredProcedure, "GetModuleProduct", parameters);
            while (dr.Read())
            {
                

                returnValue.Module = ModuleData.Get(new Module() { Id = DBHelper.IntValue(dr["ModuleId"].ToString()) });
                returnValue.Product = ProductData.Get(DBHelper.IntValue(dr["ProductId"].ToString()));
                returnValue.CreateAdminId = DBHelper.IntValue(dr["CreateAdminId"].ToString());
                returnValue.EditDate = DBHelper.DateValue(dr["EditDate"].ToString());
                returnValue.EditAdminId = DBHelper.IntValue(dr["EditAdminId"].ToString());
                returnValue.Id = DBHelper.IntValue(dr["Id"].ToString());
            }
            dr.Close();
            return returnValue;
        }

        public static ModuleProduct GetById(ModuleProduct itemModuleProduct)
        {
            var returnValue = new ModuleProduct();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@Id", itemModuleProduct.Id));

            SqlDataReader dr = SQLDataBlock.ExecuteReader(StaticData.Idevit.ConnectionString, CommandType.StoredProcedure, "GetModuleProduct", parameters);
            while (dr.Read())
            {
                

                returnValue.Module = ModuleData.Get(new Module() { Id = DBHelper.IntValue(dr["ModuleId"].ToString()) });
                returnValue.Product = ProductData.Get(DBHelper.IntValue(dr["ProductId"].ToString()));
                returnValue.CreateAdminId = DBHelper.IntValue(dr["CreateAdminId"].ToString());
                returnValue.EditDate = DBHelper.DateValue(dr["EditDate"].ToString());
                returnValue.EditAdminId = DBHelper.IntValue(dr["EditAdminId"].ToString());
                returnValue.Id = DBHelper.IntValue(dr["Id"].ToString());
            }
            dr.Close();
            return returnValue;
        }

        public static ModuleProduct Get(int ModuleId)
        {
            var returnValue = new ModuleProduct();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@Product", 0));
            parameters.Add(new SqlParameter("@ModuleId", ModuleId));

            SqlDataReader dr = SQLDataBlock.ExecuteReader(StaticData.Idevit.ConnectionString, CommandType.StoredProcedure, "GetModuleProduct", parameters);
            while (dr.Read())
            {
                

                returnValue.Module = ModuleData.Get(new Module() { Id = DBHelper.IntValue(dr["ModuleId"].ToString()) });
                returnValue.Product = ProductData.Get(DBHelper.IntValue(dr["ProductId"].ToString()));
                returnValue.CreateAdminId = DBHelper.IntValue(dr["CreateAdminId"].ToString());
                returnValue.EditDate = DBHelper.DateValue(dr["EditDate"].ToString());
                returnValue.EditAdminId = DBHelper.IntValue(dr["EditAdminId"].ToString());
                returnValue.Id = DBHelper.IntValue(dr["Id"].ToString());
            }
            dr.Close();
            return returnValue;
        }

        public static int Insert(int ProductId, int ModuleId)
        {
            var returnValue = 0;
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@ProductId", ProductId));
            parameters.Add(new SqlParameter("@ModuleId", ModuleId));
            parameters.Add(new SqlParameter("@CreateAdminId", DBHelper.IntValue(0)));


            returnValue = Convert.ToInt32(SQLDataBlock.ExecuteScalar(StaticData.Idevit.ConnectionString, CommandType.StoredProcedure, "InsertModuleProduct", parameters));
            return returnValue;
        }

        public static int Update(int ProductId, int ModuleId, int Id)
        {
            var returnValue = 1;
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@ProductId", ProductId));
            parameters.Add(new SqlParameter("@ModuleId", ModuleId));
            parameters.Add(new SqlParameter("@Id", Id));
            parameters.Add(new SqlParameter("@EditAdminId", 0));
            parameters.Add(new SqlParameter("@ErrorCode", ParameterDirection.Output));
            returnValue = SQLDataBlock.ExecuteNonQuery(StaticData.Idevit.ConnectionString, CommandType.StoredProcedure, "UpdateModuleProduct", parameters);
            return returnValue;
        }

        public static int Delete(ModuleProduct itemModuleProduct)
        {
            var returnValue = 0;

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@Id", itemModuleProduct.Id));
            parameters.Add(new SqlParameter("@ErrorCode", ParameterDirection.Output));
            returnValue = SQLDataBlock.ExecuteNonQuery(StaticData.Idevit.ConnectionString, CommandType.StoredProcedure, "DeleteModuleProduct", parameters);
            return returnValue;
        }

        public static List<ModuleProduct> GetModuleProductList(ModuleProduct itemModuleProduct = null)
        {
            List<ModuleProduct> itemModuleProductList = null;

            List<SqlParameter> parameters = new List<SqlParameter>();
            if (itemModuleProduct != null)
                parameters.Add(new SqlParameter("@ModuleId", itemModuleProduct.Module.Id));
            IDataReader dr = SQLDataBlock.ExecuteReader(StaticData.Idevit.ConnectionString, CommandType.StoredProcedure, "GetModuleProduct", parameters);
            itemModuleProductList = new List<ModuleProduct>();

            while (dr.Read())
            {
                var item = new ModuleProduct();
                item.Module = ModuleData.Get(new Module() { Id = DBHelper.IntValue(dr["ModuleId"].ToString()) });
                item.Product = ProductData.Get(DBHelper.IntValue(dr["ProductId"].ToString()));
                item.CreateAdminId = DBHelper.IntValue(dr["CreateAdminId"].ToString());
                item.EditDate = DBHelper.DateValue(dr["EditDate"].ToString());
                item.EditAdminId = DBHelper.IntValue(dr["EditAdminId"].ToString());
                item.Id = DBHelper.IntValue(dr["Id"].ToString());
                //item.ProductName = item.Product.Title;
                //item.ModuleName = item.Module.Name;

                itemModuleProductList.Add(item);
            }

            dr.Close();
            return itemModuleProductList;
        }

        public static List<Product> GetModuleProductList(int ModuleId)
        {
            List<Product> itemModuleProductList = null;

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@ModuleId", ModuleId));
            IDataReader dr = SQLDataBlock.ExecuteReader(StaticData.Idevit.ConnectionString, CommandType.StoredProcedure, "GetModuleProduct", parameters);
            itemModuleProductList = new List<Product>();

            while (dr.Read())
            {
                var item = new Product();
                item.CreateDate = DBHelper.DateValue(dr["CreateDate"].ToString());
                item.CreateAdminId = DBHelper.IntValue(dr["CreateAdminId"].ToString());
                item.Description = DBHelper.StringValue(dr["Description"].ToString());
                item.EditDate = DBHelper.DateValue(dr["EditDate"].ToString());
                item.EditAdminId = DBHelper.IntValue(dr["EditAdminId"].ToString());
                item.Id = DBHelper.IntValue(dr["Id"].ToString());
                item.Video = DBHelper.StringValue(dr["Video"].ToString());
                item.Title = DBHelper.StringValue(dr["Title"].ToString());
                item.Description = DBHelper.StringValue(dr["Description"].ToString());
                item.ProductCode = DBHelper.StringValue(dr["ProductCode"].ToString());
                item.MinStock = DBHelper.IntValue(dr["MinStock"].ToString());
                item.AlertDate = DBHelper.DateValue(dr["AlertDate"].ToString());
                item.ProductStatus = EnumValueData.Get(new EnumValue() { Id = DBHelper.IntValue(dr["ProductStatus"].ToString()) });
                item.Guarantee = DBHelper.IntValue(dr["Guarantee"].ToString());
                item.Stok = DBHelper.IntValue(dr["Stok"].ToString());
                item.MaxQuantity = DBHelper.IntValue(dr["MaxQuantity"].ToString());
                item.StokStatus = EnumValueData.Get(new EnumValue() { Id = DBHelper.IntValue(dr["StokStatus"].ToString()) });
                item.ProductCategory = ProductCategoryData.Get(DBHelper.IntValue(dr["ProductCategoryId"].ToString()));
                item.ProductPrice = ProductPriceData.GetByProduct(DBHelper.IntValue(dr["Id"].ToString()));
                item.Multimedias = MultimediasData.GetItemMultimedias(3, DBHelper.IntValue(dr["Id"].ToString()));
                itemModuleProductList.Add(item);
            }

            dr.Close();
            return itemModuleProductList;
        }

        public static List<Product> GetSearchResult(string searchkey)
        {
            List<Product> itemModuleProductList = null;

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@SearchKey", searchkey));
            IDataReader dr = SQLDataBlock.ExecuteReader(StaticData.Idevit.ConnectionString, CommandType.StoredProcedure, "GetProductSearchResult", parameters);
            itemModuleProductList = new List<Product>();

            while (dr.Read())
            {
                var item = new Product();
                item.CreateDate = DBHelper.DateValue(dr["CreateDate"].ToString());
                item.CreateAdminId = DBHelper.IntValue(dr["CreateAdminId"].ToString());
                item.Description = DBHelper.StringValue(dr["Description"].ToString());
                item.EditDate = DBHelper.DateValue(dr["EditDate"].ToString());
                item.EditAdminId = DBHelper.IntValue(dr["EditAdminId"].ToString());
                item.Id = DBHelper.IntValue(dr["Id"].ToString());
                item.Video = DBHelper.StringValue(dr["Video"].ToString());
                item.Title = DBHelper.StringValue(dr["Title"].ToString());
                item.Description = DBHelper.StringValue(dr["Description"].ToString());
                item.ProductCode = DBHelper.StringValue(dr["ProductCode"].ToString());
                item.MinStock = DBHelper.IntValue(dr["MinStock"].ToString());
                item.AlertDate = DBHelper.DateValue(dr["AlertDate"].ToString());
                item.ProductStatus = EnumValueData.Get(new EnumValue() { Id = DBHelper.IntValue(dr["ProductStatus"].ToString()) });
                item.Guarantee = DBHelper.IntValue(dr["Guarantee"].ToString());
                item.Stok = DBHelper.IntValue(dr["Stok"].ToString());
                item.MaxQuantity = DBHelper.IntValue(dr["MaxQuantity"].ToString());
                item.StokStatus = EnumValueData.Get(new EnumValue() { Id = DBHelper.IntValue(dr["StokStatus"].ToString()) });
                item.ProductCategory = ProductCategoryData.Get(DBHelper.IntValue(dr["ProductCategoryId"].ToString()));
                item.ProductPrice = ProductPriceData.GetByProduct(DBHelper.IntValue(dr["Id"].ToString()));
                item.Multimedias = MultimediasData.GetItemMultimedias(3, DBHelper.IntValue(dr["Id"].ToString()));
                itemModuleProductList.Add(item);
            }

            dr.Close();
            return itemModuleProductList;
        }
    }
}
