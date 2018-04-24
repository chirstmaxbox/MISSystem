using System.Data;
using System.Data.SqlClient;
using CustomerDomain.BO;

namespace CustomerDomain.BDL
{
    public class CustomerChildren
    {
        private readonly int _rowID;
        public CustomerChildren(int customerID)
        {
            _rowID = customerID;
        }

        public DataTable GetProjects()
        {
            DataTable tbl = null;
            SqlConnection ConnectionSQL = new SqlConnection(CustomerDomainConfiguration.ConnectionString);
            string SqlSelectString = "SELECT * FROM Sales_JobMasterList_Customer WHERE (cID=@cID)";
            SqlCommand SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            SqlDataAdapter adapter1 = new SqlDataAdapter(SelectCommand);
            adapter1.SelectCommand.Parameters.Add("@cID", SqlDbType.Int).Value = _rowID;

            DataSet ds1 = new DataSet();
            ds1.Tables.Clear();
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

        public DataTable GetWorkorders()
        {
            DataTable tbl = null;
            SqlConnection ConnectionSQL = new SqlConnection(CustomerDomainConfiguration.ConnectionString);
            string SqlSelectString1 = "SELECT * FROM [Sales_JobMasterList_WO] WHERE ([Company1] = @CustomerID or [Company2] = @CustomerID)";
            SqlCommand SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            SqlDataAdapter adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@CustomerID", SqlDbType.Int).Value = _rowID;
            DataSet ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                int NumRowsAffected = adapter1.Fill(ds1, "t1");
                if (NumRowsAffected != 0)
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

        public DataTable GetInvoices()
        {

            DataTable tbl = null;

            SqlConnection ConnectionSQL = new SqlConnection(CustomerDomainConfiguration.ConnectionString);
            string SqlSelectString1 = "SELECT * FROM [Sales_JobMasterList_Invoice] WHERE ([CustomerID] = @CustomerID)";
            SqlCommand SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            SqlDataAdapter adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@CustomerID", SqlDbType.Int).Value = _rowID;
            DataSet ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                int NumRowsAffected = adapter1.Fill(ds1, "t1");
                if (NumRowsAffected > 0)
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
