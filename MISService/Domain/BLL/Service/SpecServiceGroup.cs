using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using MyCommon;
using SpecDomain.BO;

namespace SpecDomain.BLL.Service
{
 
    public class SpecServiceAllGroup
    {
        public List<MyKeyValuePair> Values { get; private set; }

        public SpecServiceAllGroup()
        {
            Values = new List<MyKeyValuePair>();
            var tbl = GetAvailableServiceGroup();
            if (tbl == null) return;

            foreach (DataRow row in tbl.Rows)
            {
                var kvp = new MyKeyValuePair();
                kvp.Key =MyConvert.ConvertToInteger(row["Value"]);
                kvp.Value =Convert .ToString(row["Text"]);
                Values.Add(kvp);
            }
        }

        private DataTable GetAvailableServiceGroup()
        {
            DataTable tbl = null;
            var connectionSQL = new SqlConnection(SpecConfiguration.ConnectionString);
            const string sqlSelectString1 = "SELECT PS_NAME as Text, SERVICE_GROUP as Value FROM FW_QUOTE_PC WHERE (ACTIVE = 1) AND (CATID = 2) and (PC_ID>1000) GROUP BY PS_NAME, SERVICE_GROUP ORDER BY PS_NAME";
            var selectCommand1 = new SqlCommand(sqlSelectString1, connectionSQL);
            var adapter1 = new SqlDataAdapter(selectCommand1);
            var ds1 = new DataSet();
            ds1.Tables.Clear();
            try
            {
                connectionSQL.Open();
                var numRowsAffected = adapter1.Fill(ds1, "t1");
                if (numRowsAffected != 0)
                {
                    tbl = ds1.Tables["t1"];
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
            return tbl;
        }
    }

    public class SpecServiceSingleGroup
    {
        public DataTable Values { get; private set; }

        public SpecServiceSingleGroup(int groupID)
        {
            Values = GetServiceList(groupID);
        }

        private DataTable GetServiceList(int groupID)
        {
            DataTable tbl;
            var connectionSQL = new SqlConnection(SpecConfiguration.ConnectionString);;
            const string sqlSelectString1 = "SELECT PS_NAME, CONTENTS, CHARGE, PC_ID, SERVICE_GROUP FROM FW_QUOTE_PC WHERE (CAT_NAME = 'S') AND (SERVICE_GROUP = @sgID)";
            var selectCommand1 = new SqlCommand(sqlSelectString1, connectionSQL);
            var adapter1 = new SqlDataAdapter(selectCommand1);
            adapter1.SelectCommand.Parameters.Add("@sgID", SqlDbType.Int).Value = groupID;
            var ds1 = new DataSet();
            ds1.Tables.Clear();
            try
            {
                connectionSQL.Open();
                int numRowsAffected = adapter1.Fill(ds1, "t1");
                if (numRowsAffected > 0)
                {
                    tbl = ds1.Tables["t1"];
                }
                else
                {
                    tbl = null;
                }
            }
            catch (SqlException ex)
            {
                tbl = null;
                string errorLog = ex.Message;
            }
            finally
            {
                connectionSQL.Close();
            }

            return tbl;
        }


    }

}