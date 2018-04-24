using System;
using System.Data;
using System.Data.SqlClient;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Service
{
    public class ServiceDefinition
    {
        private readonly int _ServiceGroup;

        private readonly int _ServiceID;

        private readonly DataRow _row;

        private readonly DataTable _tbl;

        public ServiceDefinition(int ServiceGroup, int justForOverload)
        {
            _ServiceGroup = ServiceGroup;
            _tbl = GetServiceList();
        }

        //for job title
        public ServiceDefinition(int ServiceID)
        {
            _ServiceID = ServiceID;
            _row = GetServiceTitle();
        }

        public string ServiceTitle
        {
            get
            {
                string s = "";
                if (_row != null)
                {
                    s = Convert.ToString(_row["PS_NAME"]);
                }
                return s;
            }
        }

        public DataTable Tbl
        {
            get { return _tbl; }
        }

        public DataRow Row
        {
            get { return _row; }
        }

        private DataTable GetServiceList()
        {
            var tbl = new DataTable();
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 =
                "SELECT PS_NAME, CONTENTS, CHARGE, PC_ID, SERVICE_GROUP FROM FW_QUOTE_PC WHERE (CAT_NAME = 'S') AND (SERVICE_GROUP = @sgID)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@sgID", SqlDbType.Int).Value = _ServiceGroup;
            var ds1 = new DataSet();
            ds1.Tables.Clear();
            try
            {
                ConnectionSQL.Open();
                int NumRowsAffected = adapter1.Fill(ds1, "t1");
                if (NumRowsAffected > 0)
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
                ConnectionSQL.Close();
            }

            return tbl;
        }

        private DataRow GetServiceTitle()
        {
            DataRow row = null;

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 = "SELECT *  FROM FW_QUOTE_PC WHERE (PC_ID= @PC_ID)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@PC_ID", SqlDbType.Int).Value = _ServiceID;
            var ds1 = new DataSet();
            ds1.Tables.Clear();
            try
            {
                ConnectionSQL.Open();
                int NumRowsAffected = adapter1.Fill(ds1, "t1");
                if (NumRowsAffected > 0)
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
                ConnectionSQL.Close();
            }

            return row;
        }
    }
}