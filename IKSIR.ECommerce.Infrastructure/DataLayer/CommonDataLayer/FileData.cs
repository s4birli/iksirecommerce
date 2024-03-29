﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using IKSIR.ECommerce.Infrastructure.DataLayer.DataBlock;
using IKSIR.ECommerce.Model.CommonModel;

namespace IKSIR.ECommerce.Infrastructure.DataLayer.CommonDataLayer
{
    public class FileData
    {
        public static Files Get(int id)
        {
            var returnValue = new Files();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@Id", id));
            SqlDataReader dr = SQLDataBlock.ExecuteReader(StaticData.Idevit.ConnectionString, CommandType.StoredProcedure, "GetFile", parameters);
            while (dr.Read())
            {
                
                returnValue.CreateDate = DBHelper.DateValue(dr["CreateDate"].ToString());
                returnValue.CreateAdminId = DBHelper.IntValue(dr["CreateAdminId"].ToString());
                returnValue.Value = DBHelper.StringValue(dr["Value"].ToString());
                returnValue.EditDate = DBHelper.DateValue(dr["EditDate"].ToString());
                returnValue.EditAdminId = DBHelper.IntValue(dr["EditAdminId"].ToString());
                returnValue.Id = DBHelper.IntValue(dr["Id"].ToString());
                returnValue.Type = EnumValueData.Get(new EnumValue() { Id = DBHelper.IntValue(dr["TypeId"].ToString()) });
                returnValue.Title = DBHelper.StringValue(dr["Title"].ToString());
                returnValue.Description = DBHelper.StringValue(dr["Description"].ToString());
                returnValue.FilePath = DBHelper.StringValue(dr["FilePath"].ToString());
            }
            dr.Close();
            return returnValue;
        }

        public static int Insert(Files itemMultimedia)
        {
            var returnValue = 0;
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@Id", DBHelper.IntValue(itemMultimedia.Id)));
            parameters.Add(new SqlParameter("@ProductId", DBHelper.IntValue(itemMultimedia.ProductId)));
            parameters.Add(new SqlParameter("@Title", DBHelper.StringValue(itemMultimedia.Title)));
            parameters.Add(new SqlParameter("@Description", DBHelper.StringValue(itemMultimedia.Description)));
            parameters.Add(new SqlParameter("@FilePath", DBHelper.StringValue(itemMultimedia.FilePath)));
            parameters.Add(new SqlParameter("@CreateAdminId", DBHelper.IntValue(itemMultimedia.CreateAdminId)));
            parameters[0].Direction = ParameterDirection.Output;

            returnValue = Convert.ToInt32(SQLDataBlock.ExecuteScalar(StaticData.Idevit.ConnectionString, CommandType.StoredProcedure, "InsertFile", parameters));
            returnValue = Convert.ToInt32(parameters[0].Value);
            return returnValue;
        }

        public static int Update(Files itemMultimedia)
        {
            var returnValue = 1;
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@Id", itemMultimedia.Id));
            parameters.Add(new SqlParameter("@TypeId", DBHelper.StringValue(itemMultimedia.Type.Id)));
            parameters.Add(new SqlParameter("@Value", DBHelper.StringValue(itemMultimedia.Value)));
            parameters.Add(new SqlParameter("@EditAdminId", DBHelper.IntValue(itemMultimedia.EditAdminId)));
            parameters.Add(new SqlParameter("@ErrorCode", ParameterDirection.Output));
            returnValue = SQLDataBlock.ExecuteNonQuery(StaticData.Idevit.ConnectionString, CommandType.StoredProcedure, "UpdateFile", parameters);
            return returnValue;
        }


        public static int Delete(int id)
        {
            var returnValue = 0;

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@Id", id));
            parameters.Add(new SqlParameter("@ErrorCode", ParameterDirection.Output));
            returnValue = SQLDataBlock.ExecuteNonQuery(StaticData.Idevit.ConnectionString, CommandType.StoredProcedure, "DeleteFile", parameters);
            return returnValue;
        }

        public static List<Files> GetItemFiles(int itemTypeId, int itemId)
        {
            List<Files> itemMultimediaList = null;

            List<SqlParameter> parameters = new List<SqlParameter>();
            //parameters.Add(new SqlParameter("@TypeId", itemTypeId));
            parameters.Add(new SqlParameter("@ProductId", itemId));
            IDataReader dr = SQLDataBlock.ExecuteReader(StaticData.Idevit.ConnectionString, CommandType.StoredProcedure, "GetProductFiles", parameters);
            itemMultimediaList = new List<Files>();

            while (dr.Read())
            {
                var item = new Files();
                
                item.CreateDate = DBHelper.DateValue(dr["CreateDate"].ToString());
                item.CreateAdminId = DBHelper.IntValue(dr["CreateAdminId"].ToString());
                item.Value = DBHelper.StringValue(dr["Value"].ToString());
                item.Title = DBHelper.StringValue(dr["Title"].ToString());
                item.Description = DBHelper.StringValue(dr["Description"].ToString());
                item.FilePath = DBHelper.StringValue(dr["FilePath"].ToString());
                item.ProductId = DBHelper.IntValue(dr["ProductId"].ToString());
                item.EditDate = DBHelper.DateValue(dr["EditDate"].ToString());
                item.EditAdminId = DBHelper.IntValue(dr["EditAdminId"].ToString());
                item.Id = DBHelper.IntValue(dr["Id"].ToString());
                item.Type = EnumValueData.Get(new EnumValue() { Id = DBHelper.IntValue(dr["TypeId"].ToString()) });
                itemMultimediaList.Add(item);
            }

            dr.Close();
            return itemMultimediaList;
        }
    }
}
