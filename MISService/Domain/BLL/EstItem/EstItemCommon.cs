using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using MyCommon;
using SpecDomain.BLL.EstTitle;
using SpecDomain.BO;
using SpecDomain.Model;

namespace SpecDomain.BLL.EstItem
{
    public class EstItemCommon
    {
        public static int GetNewNumber(int estRevID)
        {
            int num = 1;

            using (var connection = new System.Data.SqlClient.SqlConnection(SpecConfiguration.ConnectionString))
            {
                const string selectString = "SELECT MAX(estItemNo) AS estItemNo FROM EST_Item WHERE (estRevID = @estRevID)";
                var selectCommand = new System.Data.SqlClient.SqlCommand(selectString, connection);
                selectCommand.Parameters.Add("@estRevID", SqlDbType.Int).Value = estRevID;
                connection.Open();
                var reader = selectCommand.ExecuteReader();
                try
                {
                    reader.Read();
                    if (!Convert.IsDBNull(reader[0]))
                    {
                        num = Convert.ToInt32(reader[0]) + 1;
                    }
                }
                catch (SqlException ex)
                {
                    string errorlog = ex.Message;
                }
                finally
                {
                    reader.Close();
                    connection.Close();
                }
            }
            return num;
        }
        
        public static int GetNewOption(int estRevID, int estItemNo)
        {
            int opt = 1;

            using (var connection = new System.Data.SqlClient.SqlConnection(SpecConfiguration.ConnectionString))
            {
                const string selectString = "SELECT MAX(itemOption) AS itemOption FROM EST_Item WHERE (estRevID = @estRevID) AND (estItemNo = @estItemNO)";
                var selectCommand = new System.Data.SqlClient.SqlCommand(selectString, connection);
                selectCommand.Parameters.Add("@estRevID", SqlDbType.Int).Value = estRevID;
                selectCommand.Parameters.Add("@estItemNo", SqlDbType.SmallInt).Value = estItemNo;
                connection.Open();
                var reader = selectCommand.ExecuteReader();
                try
                {
                    reader.Read();
                    if (!Convert.IsDBNull(reader[0]))
                    {
                        opt = Convert.ToInt32(reader[0]) + 1;
                    }
                }
                catch (SqlException ex)
                {
                    string errorlog = ex.Message;
                }
                finally
                {
                    reader.Close();
                    connection.Close();
                }
            }
            return opt;
        }

        public static int GetNewSerialID(int estRevID)
        {
            var serialID = 1;

            using (var connection = new System.Data.SqlClient.SqlConnection(SpecConfiguration.ConnectionString))
            {
                const string selectString = "SELECT MAX(SerialID) AS SerialID FROM EST_Item WHERE (estRevID = @EstRevID)";
                var selectCommand = new System.Data.SqlClient.SqlCommand(selectString, connection);
                selectCommand.Parameters.Add("@EstRevID", SqlDbType.BigInt).Value = estRevID;
                connection.Open();
                var reader = selectCommand.ExecuteReader();
                try
                {
                    reader.Read();
                    if (!Convert.IsDBNull(reader[0]))
                    {
                        serialID  = Convert.ToInt32(reader[0]);
                    }
                }
                catch (SqlException ex)
                {
                    string errorlog = ex.Message;
                }
                finally
                {
                    reader.Close();
                    connection.Close();
                }
            }

            //if (serialID == 1) return serialID;

            //var row = GetEstimationDataRow(estRevID);
            //var  jobID = Convert.ToInt32(row["jobID"]);
            //var p = new SpecProjectDetail(jobID);
            //serialID = p.LastEstItemID;
            //p.LastEstItemID = serialID + 1;

            return serialID + 1;
        }

        public static List<MyLongKeyValuePair> GetEstimationItemList(int estRevID)
        {
            var list = new List<MyLongKeyValuePair>();
            DataTable tbl = EstItemCommon.GetEstimationItems(estRevID);
            if (tbl == null) return list;
            foreach (DataRow row in tbl.Rows)
            {
                var vp = new MyLongKeyValuePair();
                vp.Key = Convert.ToInt64(row["estItemID"]);
                vp.Value = "Item " + row["estItemNo"] + " - Option " + row["ItemOption"] + " - " + row["ProductName"];
                list.Add(vp);
            }

            return list;
        }

        public static DataTable GetEstimationItems(int estRevID)
        {
            //Input: Parent ID
            DataTable t1 = null;
            var connectionSQL = new SqlConnection(SpecConfiguration.ConnectionString);
            const string sqlSelectString = "SELECT * FROM [EST_Item] WHERE ([estRevID] = @estRevID AND [ItemPurposeID]=0) order by [EstItemNo]";
            var selectCommand = new SqlCommand(sqlSelectString, connectionSQL);
            var adapter1 = new SqlDataAdapter(selectCommand);
            var ds1 = new DataSet();
            ds1.Tables.Clear();
            adapter1.SelectCommand.Parameters.Add("@estRevID", SqlDbType.Int).Value = estRevID;

            try
            {
                connectionSQL.Open();
                int rowsAffected = adapter1.Fill(ds1, "t1");
                if (rowsAffected > 0)
                {
                    t1 = ds1.Tables["t1"];
                }
            }
            catch (SqlException ex)
            {
                string errLog = ex.Message;

            }
            finally
            {
                connectionSQL.Close();
            }

            return t1;
        }

        private static DataRow GetEstimationDataRow(int estRevID)
        {
            DataRow row = null;
            var connectionSQL = new SqlConnection(SpecConfiguration.ConnectionString);
            const string sqlSelectString = "SELECT * FROM [Sales_JobMasterList_EstRev] WHERE ([estRevID] = @estRevID)";
            var selectCommand = new SqlCommand(sqlSelectString, connectionSQL);
            var adapter1 = new SqlDataAdapter(selectCommand);
            adapter1.SelectCommand.Parameters.Add("@estRevID", SqlDbType.Int).Value = estRevID;

            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                connectionSQL.Open();
                int rowsAffected = adapter1.Fill(ds1, "t1");
                if (rowsAffected > 0)
                {
                    row = ds1.Tables["t1"].Rows[0];
                }

            }
            catch (SqlException ex)
            {
                string errorLog = ex.Message;

            }
            finally
            {
                connectionSQL.Close();
            }
            return row;
        }

        public static int GetNewPartNumber(int estRevID, int serialID )
        {
            int num = 1;

            using (var connection = new System.Data.SqlClient.SqlConnection(SpecConfiguration.ConnectionString))
            {
                const string selectString = "SELECT MAX(EstPart) AS EstPart FROM EST_Item WHERE (estRevID = @EstRevID AND SerialID=@SerialID)";
                var selectCommand = new System.Data.SqlClient.SqlCommand(selectString, connection);
                selectCommand.Parameters.Add("@EstRevID", SqlDbType.BigInt).Value = estRevID;
                selectCommand.Parameters.Add("@SerialID", SqlDbType.Int).Value = serialID;
                connection.Open();
                var reader = selectCommand.ExecuteReader();
                try
                {
                    reader.Read();
                    if (!Convert.IsDBNull(reader[0]))
                    {
                        num = Convert.ToInt32(reader[0]) + 1;
                    }
                }
                catch (SqlException ex)
                {
                    string errorlog = ex.Message;
                }
                finally
                {
                    reader.Close();
                    connection.Close();
                }
            }
            return num;
        }

        public static string GetFullItemName (EST_Item item)
        {
            var s = "Item " + string.Format("{0:D0}", item.EstItemNo);
            s += " -- Ver " + string.Format("{0:D0}", item.Version);
            s += " -- E" + string.Format("{0:D2}", item.SerialID );
            s += "." + string.Format("{0:D2}", item.EstPart);
            s += " -- Opt " + string.Format("{0:D0}", item.ItemOption);
            s += " -- "     + item.ProductName;

            return s;
        }
    }


}