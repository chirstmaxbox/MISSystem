using System.Data;
using System.Data.SqlClient;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Project
{
    public class SubcontractChildren
    {
        private readonly int _jobID;

        public SubcontractChildren(int jobID)
        {
            _jobID = jobID;
        }

        public DataTable GetItems()
        {
            DataTable tbl = null;
            var connectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            const string sqlSelectString = "SELECT * FROM [View_SideMenu_Subcontract] WHERE ([jobID] = @jobID)";
            var selectCommand = new SqlCommand(sqlSelectString, connectionSQL);
            var adapter1 = new SqlDataAdapter(selectCommand);
            var ds1 = new DataSet();
            ds1.Tables.Clear();
            adapter1.SelectCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = _jobID;

            try
            {
                connectionSQL.Open();
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
                connectionSQL.Close();
            }

            return tbl;
        }
    }

    public class PermitChildren
    {
        private readonly int _jobID;

        public PermitChildren(int jobID)
        {
            _jobID = jobID;
        }

        public DataTable GetItems()
        {
            DataTable tbl = null;
            var connectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            const string sqlSelectString = "SELECT * FROM [View_SideMenu_Permit] WHERE ([jobID] = @jobID)";
            var selectCommand = new SqlCommand(sqlSelectString, connectionSQL);
            var adapter1 = new SqlDataAdapter(selectCommand);
            var ds1 = new DataSet();
            ds1.Tables.Clear();
            adapter1.SelectCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = _jobID;

            try
            {
                connectionSQL.Open();
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
                connectionSQL.Close();
            }

            return tbl;
        }
    }
}