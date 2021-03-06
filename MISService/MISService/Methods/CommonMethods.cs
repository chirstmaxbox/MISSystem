﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using enterprise = MISService.SfdcReference;
using System.ServiceModel;
using MISService.Models;
using MISService.Method;
using System.Data.SqlClient;

namespace MISService.Methods
{
    public class CommonMethods
    {
 
        public static void Delete(string tableName, string salesforceID, string salesforceParentID, string salesforceProjectID)
        {
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            try
            {
                Connection.Open();
                string SqlDelString = "DELETE FROM MISSalesForceMapping WHERE ([TableName] = @tableName) and ([SalesforceID] = @salesforceID) and ([SalesforceParentID] = @salesforceParentID) and ([SalesForceProjectID] = @salesForceProjectID)";
                var DelCommand = new SqlCommand(SqlDelString, Connection);
                DelCommand.Parameters.AddWithValue("@tableName", tableName);
                DelCommand.Parameters.AddWithValue("@salesforceID", salesforceID);
                DelCommand.Parameters.AddWithValue("@salesforceParentID", salesforceParentID);
                DelCommand.Parameters.AddWithValue("@salesForceProjectID", salesforceProjectID);
                DelCommand.ExecuteNonQuery();
                Connection.Close();
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("Delete:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }
        }

        public static void Delete(string tableName, string salesforceID, string salesforceProjectID)
        {
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            try
            {
                Connection.Open();
                string SqlDelString = "DELETE FROM MISSalesForceMapping WHERE ([TableName] = @tableName) and ([SalesforceID] = @salesforceID) and ([SalesForceProjectID] = @salesForceProjectID)";
                var DelCommand = new SqlCommand(SqlDelString, Connection);
                DelCommand.Parameters.AddWithValue("@tableName", tableName);
                DelCommand.Parameters.AddWithValue("@salesforceID", salesforceID);
                DelCommand.Parameters.AddWithValue("@salesForceProjectID", salesforceProjectID);
                DelCommand.ExecuteNonQuery();
                Connection.Close();
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("Delete:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }
        }

        public static List<string> GetAllSalesForceIDWithoutParent(string tableName, string salesforceProjectID)
        {
            List<string> ids = new List<string>();
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            try
            {
                string SqlSelectString = "SELECT SalesForceID FROM [MISSalesForceMapping] WHERE ([TableName] = @tableName) and ([SalesForceProjectID] = @salesForceProjectID)";
                var SelectCommand = new SqlCommand(SqlSelectString, Connection);
                SelectCommand.Parameters.AddWithValue("@tableName", tableName);
                SelectCommand.Parameters.AddWithValue("@salesForceProjectID", salesforceProjectID);
                Connection.Open();
                using (SqlDataReader dr = SelectCommand.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        ids.Add(dr[0].ToString());
                    }
                }
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("GetAllSalesForceIDWithoutParent:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return ids;
        }

        public static List<string> GetAllSalesForceID(string tableName, string salesforceParentID, string salesforceProjectID)
        {
            List<string> ids = new List<string>();
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            try
            {
                string SqlSelectString = "SELECT SalesForceID FROM [MISSalesForceMapping] WHERE ([TableName] = @tableName) and ([SalesforceParentID] = @salesforceParentID) and ([SalesForceProjectID] = @salesForceProjectID)";
                var SelectCommand = new SqlCommand(SqlSelectString, Connection);
                SelectCommand.Parameters.AddWithValue("@tableName", tableName);
                SelectCommand.Parameters.AddWithValue("@salesforceParentID", salesforceParentID);
                SelectCommand.Parameters.AddWithValue("@salesForceProjectID", salesforceProjectID);
                Connection.Open();
                using (SqlDataReader dr = SelectCommand.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        ids.Add(dr[0].ToString());
                    }
                }
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("GetAllSalesForceID:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return ids;
        }

        public static int GetCompanyMISID(string tableName, string salesforceID)
        {
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            int MISID = 0;
            try
            {
                string SqlSelectString = "SELECT MISID FROM [MISSalesForceMapping] WHERE ([TableName] = @tableName) and ([SalesForceID] = @salesforceID)";
                var SelectCommand = new SqlCommand(SqlSelectString, Connection);
                SelectCommand.Parameters.AddWithValue("@tableName", tableName);
                SelectCommand.Parameters.AddWithValue("@salesforceID", salesforceID);
                Connection.Open();
                using (SqlDataReader dr = SelectCommand.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        MISID = Convert.ToInt32(dr[0].ToString());
                    }
                }
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("GetMISID:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return MISID;
        }

        public static int GetContactCompanyMISID(string tableName, string salesforceID, string salesforceParentID)
        {
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            int MISID = 0;
            try
            {
                string SqlSelectString = "SELECT MISID FROM [MISSalesForceMapping] WHERE ([TableName] = @tableName) and ([SalesForceID] = @salesforceID) and ([SalesForceParentID] = @salesForceParentID)";
                var SelectCommand = new SqlCommand(SqlSelectString, Connection);
                SelectCommand.Parameters.AddWithValue("@tableName", tableName);
                SelectCommand.Parameters.AddWithValue("@salesforceID", salesforceID);
                SelectCommand.Parameters.AddWithValue("@salesforceParentID", salesforceParentID);
                Connection.Open();
                using (SqlDataReader dr = SelectCommand.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        MISID = Convert.ToInt32(dr[0].ToString());
                    }
                }
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("GetMISID:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return MISID;
        }

        public static int GetMISID(string tableName, string salesforceID, string salesforceProjectID)
        {
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            int MISID = 0;
            try
            {
                string SqlSelectString = "SELECT MISID FROM [MISSalesForceMapping] WHERE ([TableName] = @tableName) and ([SalesForceID] = @salesforceID) and ([SalesForceProjectID] = @salesForceProjectID)";
                var SelectCommand = new SqlCommand(SqlSelectString, Connection);
                SelectCommand.Parameters.AddWithValue("@tableName", tableName);
                SelectCommand.Parameters.AddWithValue("@salesforceID", salesforceID);
                SelectCommand.Parameters.AddWithValue("@salesForceProjectID", salesforceProjectID);
                Connection.Open();
                using (SqlDataReader dr = SelectCommand.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        MISID = Convert.ToInt32(dr[0].ToString());
                    }
                }
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("GetMISID:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return MISID;
        }

        public static int GetMISID(string tableName, string salesforceID, string salesforceParentID, string salesforceProjectID)
        {
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            int MISID = 0;
            try
            {
                string SqlSelectString = "SELECT MISID FROM [MISSalesForceMapping] WHERE ([TableName] = @tableName) and ([SalesForceID] = @salesforceID) and ([SalesForceParentID] = @salesForceParentID) and ([SalesForceProjectID] = @salesForceProjectID)";
                var SelectCommand = new SqlCommand(SqlSelectString, Connection);
                SelectCommand.Parameters.AddWithValue("@tableName", tableName);
                SelectCommand.Parameters.AddWithValue("@salesforceID", salesforceID);
                SelectCommand.Parameters.AddWithValue("@salesforceParentID", salesforceParentID);
                SelectCommand.Parameters.AddWithValue("@salesForceProjectID", salesforceProjectID);
                Connection.Open();
                using (SqlDataReader dr = SelectCommand.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        MISID = Convert.ToInt32(dr[0].ToString());
                    }
                }
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("GetMISID:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return MISID;
        }

        public static bool InsertToMISSalesForceMapping(string tableName, string salesforceID, string MISID, string salesforceProjectID)
        {
            bool ret = false;
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            try
            {
                string SqlSelectString = "INSERT INTO [MISSalesForceMapping](TableName, SalesForceID, MISID, SalesForceProjectID) VALUES (@tableName, @salesforceID, @MISID, @salesForceProjectID)";
                var SelectCommand = new SqlCommand(SqlSelectString, Connection);
                SelectCommand.Parameters.AddWithValue("@tableName", tableName);
                SelectCommand.Parameters.AddWithValue("@salesforceID", salesforceID);
                SelectCommand.Parameters.AddWithValue("@MISID", MISID);
                SelectCommand.Parameters.AddWithValue("@salesForceProjectID", salesforceProjectID);
                Connection.Open();
                SelectCommand.ExecuteNonQuery();
                ret = true;
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("InsertToMISSalesForceMapping:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return ret;
        }

        public static bool InsertToMISSalesForceMapping(string tableName, string salesforceID, string MISID, string salesforceParentID, string salesforceProjectID)
        {
            bool ret = false;
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            try
            {
                string SqlSelectString = "INSERT INTO [MISSalesForceMapping](TableName, SalesForceID, MISID, SalesForceParentID, SalesForceProjectID) VALUES (@tableName, @salesforceID, @MISID, @SalesForceParentID, @salesForceProjectID)";
                var SelectCommand = new SqlCommand(SqlSelectString, Connection);
                SelectCommand.Parameters.AddWithValue("@tableName", tableName);
                SelectCommand.Parameters.AddWithValue("@salesforceID", salesforceID);
                SelectCommand.Parameters.AddWithValue("@MISID", MISID);
                SelectCommand.Parameters.AddWithValue("@SalesForceParentID", salesforceParentID);
                SelectCommand.Parameters.AddWithValue("@salesForceProjectID", salesforceProjectID);
                Connection.Open();
                SelectCommand.ExecuteNonQuery();
                ret = true;
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("InsertToMISSalesForceMapping:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return ret;
        }

        public static int GetEstRevID(int jobID)
        {
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            int estRevID = 0;
            try
            {
                string SqlSelectString = "SELECT EstRevID FROM [Sales_JobMasterList_EstRev] WHERE ([jobID] = @jobID)";
                var SelectCommand = new SqlCommand(SqlSelectString, Connection);
                SelectCommand.Parameters.AddWithValue("@jobID", jobID);
                Connection.Open();
                using (SqlDataReader dr = SelectCommand.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        estRevID = Convert.ToInt32(dr[0].ToString());
                    }
                }
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("GetEstRevID:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return estRevID;
        }

        public static int GetEstimationItemID(int estRevID, string productName)
        {
            var Connection = new SqlConnection(MISServiceConfiguration.ConnectionString);
            int estItemID = 0;
            try
            {
                string SqlSelectString = "SELECT EstItemID FROM [EST_Item] WHERE ([EstRevID] = @estRevID) and ([ProductName] = @productName)";
                var SelectCommand = new SqlCommand(SqlSelectString, Connection);
                SelectCommand.Parameters.AddWithValue("@EstRevID", estRevID);
                SelectCommand.Parameters.AddWithValue("@productName", productName);
                Connection.Open();
                using (SqlDataReader dr = SelectCommand.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        estItemID = Convert.ToInt32(dr[0].ToString());
                    }
                }
            }
            catch (SqlException ex)
            {
                LogMethods.Log.Error("GetEstimationItemID:Error:" + ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return estItemID;
        }
    }
}
