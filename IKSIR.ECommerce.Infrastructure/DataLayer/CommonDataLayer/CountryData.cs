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
    public class CountryData
    {
        public static Country Get(Country itemCountry)
        {
            var returnValue = new Country();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@Id", itemCountry.Id));

            SqlDataReader dr = SQLDataBlock.ExecuteReader(StaticData.Idevit.ConnectionString, CommandType.StoredProcedure, "GetCountry", parameters);
            while (dr.Read())
            {
                returnValue.CreateDate = DBHelper.DateValue(dr["CreateDate"].ToString());
                returnValue.CreateAdminId = DBHelper.IntValue(dr["CreateAdminId"].ToString());
                returnValue.Name = DBHelper.StringValue(dr["Name"].ToString());
                returnValue.EditDate = DBHelper.DateValue(dr["EditDate"].ToString());
                returnValue.EditAdminId = DBHelper.IntValue(dr["EditAdminId"].ToString());
                returnValue.Id = DBHelper.IntValue(dr["Id"].ToString());
            }
            dr.Close();
            return returnValue;
        }

        public static int Insert(Country itemCountry)
        {
            var returnValue = 0;
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@Name", DBHelper.StringValue(itemCountry.Name)));
            parameters.Add(new SqlParameter("@CreateAdminId", DBHelper.IntValue(itemCountry.CreateAdminId)));

            returnValue = Convert.ToInt32(SQLDataBlock.ExecuteScalar(StaticData.Idevit.ConnectionString, CommandType.StoredProcedure, "InsertCountry", parameters));
            return returnValue;
        }

        public static int Update(Country itemCountry)
        {
            var returnValue = 1;
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@Id", itemCountry.Id));
            parameters.Add(new SqlParameter("@EditAdminId", DBHelper.IntValue(itemCountry.EditAdminId)));
            parameters.Add(new SqlParameter("@Name", DBHelper.StringValue(itemCountry.Name)));
            parameters.Add(new SqlParameter("@ErrorCode", ParameterDirection.Output));
            returnValue = SQLDataBlock.ExecuteNonQuery(StaticData.Idevit.ConnectionString, CommandType.StoredProcedure, "UpdateCountry", parameters);
            return returnValue;
        }

        public static int Save(Country itemCountry)
        {
            var returnValue = 0;
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@Id", DBHelper.IntValue(itemCountry.Id)));
            parameters.Add(new SqlParameter("@AdminId", DBHelper.IntValue(itemCountry.CreateAdminId)));
            parameters.Add(new SqlParameter("@Name", DBHelper.StringValue(itemCountry.Name)));
            returnValue = Convert.ToInt32(SQLDataBlock.ExecuteScalar(StaticData.Idevit.ConnectionString, CommandType.StoredProcedure, "SaveCountry", parameters));
            return returnValue;
        }

        public static int Delete(Country itemCountry)
        {
            var returnValue = 0;

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@Id", itemCountry.Id));
            parameters.Add(new SqlParameter("@ErrorCode", ParameterDirection.Output));
            returnValue = SQLDataBlock.ExecuteNonQuery(StaticData.Idevit.ConnectionString, CommandType.StoredProcedure, "DeleteCountry", parameters);
            return returnValue;
        }

        public static List<Country> GetCountryList(Country itemCountry = null)
        {
            List<Country> itemCountryList = null;

            List<SqlParameter> parameters = new List<SqlParameter>();
            //if (itemProductCategory != null)
            //    parameters.Add(new SqlParameter("@Id", itemProductCategory.Id));
            //if (itemProductCategory.ParentCategory != null)
            //    parameters.Add(new SqlParameter("@ProductCategoryId", itemProductCategory.ParentCategory.Id));
            IDataReader dr = SQLDataBlock.ExecuteReader(StaticData.Idevit.ConnectionString, CommandType.StoredProcedure, "GetCountry", parameters);
            itemCountryList = new List<Country>();

            while (dr.Read())
            {
                var item = new Country();
                
                item.CreateDate = DBHelper.DateValue(dr["CreateDate"].ToString());
                item.CreateAdminId = DBHelper.IntValue(dr["CreateAdminId"].ToString());
                item.Name = DBHelper.StringValue(dr["Name"].ToString());
                item.EditDate = DBHelper.DateValue(dr["EditDate"].ToString());
                item.EditAdminId = DBHelper.IntValue(dr["EditAdminId"].ToString());
                item.Id = DBHelper.IntValue(dr["Id"].ToString());

                itemCountryList.Add(item);
            }

            dr.Close();
            return itemCountryList;
        }
    }
}
