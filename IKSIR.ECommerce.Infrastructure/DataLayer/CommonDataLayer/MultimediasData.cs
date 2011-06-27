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
    public class MultimediasData
    {
        public static Multimedia Get(int id)
        {
            var returnValue = new Multimedia();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@Id", id));
            SqlDataReader dr = SQLDataBlock.ExecuteReader(StaticData.Idevit.ConnectionString, CommandType.StoredProcedure, "GetMultimedia", parameters);
            dr.Read();
            //TODO => tayfun
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

            dr.Close();
            return returnValue;
        }

        public static int Insert(Multimedia itemMultimedia)
        {
            var returnValue = 0;
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@TypeId", DBHelper.StringValue(itemMultimedia.Type.Id)));
            parameters.Add(new SqlParameter("@Value", DBHelper.StringValue(itemMultimedia.Value)));
            parameters.Add(new SqlParameter("@CreateAdminId", DBHelper.IntValue(itemMultimedia.CreateAdminId)));

            returnValue = Convert.ToInt32(SQLDataBlock.ExecuteScalar(StaticData.Idevit.ConnectionString, CommandType.StoredProcedure, "InsertMultimedia", parameters));
            return returnValue;
        }

        public static int Update(Multimedia itemMultimedia)
        {
            var returnValue = 1;
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@Id", itemMultimedia.Id));
            parameters.Add(new SqlParameter("@TypeId", DBHelper.StringValue(itemMultimedia.Type.Id)));
            parameters.Add(new SqlParameter("@Value", DBHelper.StringValue(itemMultimedia.Value)));
            parameters.Add(new SqlParameter("@EditAdminId", DBHelper.IntValue(itemMultimedia.EditAdminId)));
            parameters.Add(new SqlParameter("@ErrorCode", ParameterDirection.Output));
            returnValue = SQLDataBlock.ExecuteNonQuery(StaticData.Idevit.ConnectionString, CommandType.StoredProcedure, "UpdateMultimedia", parameters);
            return returnValue;
        }

        public static int Delete(Multimedia itemMultimedia)
        {
            var returnValue = 0;

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@Id", itemMultimedia.Id));
            parameters.Add(new SqlParameter("@ErrorCode", ParameterDirection.Output));
            returnValue = SQLDataBlock.ExecuteNonQuery(StaticData.Idevit.ConnectionString, CommandType.StoredProcedure, "DeleteMultimedia", parameters);
            return returnValue;
        }

        public static List<Multimedia> GetItemMultimedias(int itemTypeId, int itemId)
        {
            List<Multimedia> itemMultimediaList = null;

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@TypeId", itemTypeId));
            parameters.Add(new SqlParameter("@ItemId", itemId));
            IDataReader dr = SQLDataBlock.ExecuteReader(StaticData.Idevit.ConnectionString, CommandType.StoredProcedure, "GetMultimedias", parameters);
            itemMultimediaList = new List<Multimedia>();

            while (dr.Read())
            {
                var item = new Multimedia();
                //TODO => tayfun
                item.CreateDate = DBHelper.DateValue(dr["CreateDate"].ToString());
                item.CreateAdminId = DBHelper.IntValue(dr["CreateAdminId"].ToString());
                item.Value = DBHelper.StringValue(dr["Value"].ToString());
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
