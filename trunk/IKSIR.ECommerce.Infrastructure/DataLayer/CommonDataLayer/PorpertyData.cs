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
    public class PorpertyData
    {
        public static Property Get(Property itemProperty)
        {
            var returnValue = new Property();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@Id", itemProperty.Id));
            SqlDataReader dr = SQLDataBlock.ExecuteReader(StaticData.Idevit.ConnectionString, CommandType.StoredProcedure, "GetProperty", parameters);
            dr.Read();
            //TODO => tayfun
            returnValue.CreateDate = DBHelper.DateValue(dr["CreateDate"].ToString());
            returnValue.CreateAdminId = DBHelper.IntValue(dr["CreateAdminId"].ToString());
            returnValue.Description = DBHelper.StringValue(dr["Description"].ToString());
            returnValue.Title = DBHelper.StringValue(dr["Title"].ToString());
            returnValue.Description = DBHelper.StringValue(dr["Description"].ToString());
            returnValue.EditDate = DBHelper.DateValue(dr["EditDate"].ToString());
            returnValue.EditAdminId = DBHelper.IntValue(dr["EditAdminId"].ToString());
            returnValue.Id = DBHelper.IntValue(dr["Id"].ToString());
            returnValue.PropertyValue = PropertyValueData.GetPropertyValueListById(DBHelper.IntValue(dr["PropertyId"].ToString()));

            dr.Close();
            return returnValue;
        }

        public static int Insert(Property itemProperty)
        {
            var returnValue = 0;
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@Title", DBHelper.StringValue(itemProperty.Title)));
            parameters.Add(new SqlParameter("@Description", DBHelper.StringValue(itemProperty.Description)));
            parameters.Add(new SqlParameter("@CreateUserId", DBHelper.IntValue(itemProperty.CreateAdminId)));

            returnValue = Convert.ToInt32(SQLDataBlock.ExecuteScalar(StaticData.Idevit.ConnectionString, CommandType.StoredProcedure, "InsertProperty", parameters));
            return returnValue;
        }

        public static int Update(Property itemProperty)
        {
            var returnValue = 1;
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@Id", itemProperty.Id));
            parameters.Add(new SqlParameter("@EditUserId", DBHelper.IntValue(itemProperty.EditAdminId)));
            parameters.Add(new SqlParameter("@Title", DBHelper.StringValue(itemProperty.Title)));
            parameters.Add(new SqlParameter("@Description", DBHelper.StringValue(itemProperty.Description)));
            parameters.Add(new SqlParameter("@ErrorCode", ParameterDirection.Output));
            returnValue = SQLDataBlock.ExecuteNonQuery(StaticData.Idevit.ConnectionString, CommandType.StoredProcedure, "UpdateProperty", parameters);
            return returnValue;
        }

        public static int Delete(Property itemProperty)
        {
            var returnValue = 0;

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@Id", itemProperty.Id));
            parameters.Add(new SqlParameter("@ErrorCode", ParameterDirection.Output));
            returnValue = SQLDataBlock.ExecuteNonQuery(StaticData.Idevit.ConnectionString, CommandType.StoredProcedure, "DeleteProperty", parameters);
            return returnValue;
        }

        public static List<Property> GetPropertyList(Property itemProperty = null)
        {
            List<Property> itemPropertyList = null;

            List<SqlParameter> parameters = new List<SqlParameter>();

            IDataReader dr = SQLDataBlock.ExecuteReader(StaticData.Idevit.ConnectionString, CommandType.StoredProcedure, "GetProperty", parameters);
            itemPropertyList = new List<Property>();

            while (dr.Read())
            {
                var item = new Property();
                //TODO => tayfun
                item.CreateDate = DBHelper.DateValue(dr["CreateDate"].ToString());
                item.CreateAdminId = DBHelper.IntValue(dr["CreateAdminId"].ToString());
                item.Title = DBHelper.StringValue(dr["Title"].ToString());
                item.Description = DBHelper.StringValue(dr["Description"].ToString());
                item.EditDate = DBHelper.DateValue(dr["EditDate"].ToString());
                item.EditAdminId = DBHelper.IntValue(dr["EditAdminId"].ToString());
                item.Id = DBHelper.IntValue(dr["Id"].ToString());
                item.PropertyValue = PropertyValueData.GetPropertyValueListById(DBHelper.IntValue(dr["PropertyId"].ToString()));
                itemPropertyList.Add(item);
            }

            dr.Close();
            return itemPropertyList;
        }
     
    }
}