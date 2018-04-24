using System;
using System.Data;
using System.Data.SqlClient;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Project
{
    public class ProjectChildrenItemTable
    {
        private readonly int _jobID;

        public ProjectChildrenItemTable(int jobID)
        {
            _jobID = jobID;
        }

        public DataTable Estimation()
        {
            DataTable tbl = null;
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString = "SELECT * FROM [Sales_JobMasterList_EstRev] WHERE ([jobID] = @jobID)";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand);
            var ds1 = new DataSet();
            ds1.Tables.Clear();
            adapter1.SelectCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = _jobID;

            try
            {
                ConnectionSQL.Open();
                int rowsAffected = adapter1.Fill(ds1, "t1");
                if (rowsAffected > 0)
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
                ConnectionSQL.Close();
            }

            return tbl;
        }


        public int GetNewEstimationVersion()
        {
            int num = 1;

            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string SelectString =
                    "SELECT MAX(erRev) AS erRev FROM Sales_JobMasterList_EstRev WHERE (jobID = @jobID)";
                var SelectCommand = new SqlCommand(SelectString, Connection);
                SelectCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = _jobID;
                Connection.Open();
                SqlDataReader reader = SelectCommand.ExecuteReader();
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
                    Connection.Close();
                }
            }
            return num;
        }


        public DataTable Quote()
        {
            DataTable tbl = null;
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString = "SELECT * FROM [Sales_JobMasterList_QuoteRev] WHERE ([jobID] = @jobID)";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand);
            var ds1 = new DataSet();
            ds1.Tables.Clear();
            adapter1.SelectCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = _jobID;

            try
            {
                ConnectionSQL.Open();
                int rowsAffected = adapter1.Fill(ds1, "t1");
                if (rowsAffected > 0)
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
                ConnectionSQL.Close();
            }

            return tbl;
        }


        public DataTable Workorder()
        {
            DataTable tbl = null;
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString = "SELECT * FROM [Sales_JobMasterList_WO] WHERE ([jobID] = @jobID)";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand);
            var ds1 = new DataSet();
            ds1.Tables.Clear();
            adapter1.SelectCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = _jobID;

            try
            {
                ConnectionSQL.Open();
                int rowsAffected = adapter1.Fill(ds1, "t1");
                if (rowsAffected > 0)
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
                ConnectionSQL.Close();
            }

            return tbl;
        }


        public DataTable Invoice()
        {
            DataTable tbl = null;
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString = "SELECT * FROM [Sales_JobMasterList_Invoice] WHERE ([jobID] = @jobID)";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand);
            var ds1 = new DataSet();
            ds1.Tables.Clear();
            adapter1.SelectCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = _jobID;

            try
            {
                ConnectionSQL.Open();
                int rowsAffected = adapter1.Fill(ds1, "t1");
                if (rowsAffected > 0)
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
                ConnectionSQL.Close();
            }

            return tbl;
        }
    }
}