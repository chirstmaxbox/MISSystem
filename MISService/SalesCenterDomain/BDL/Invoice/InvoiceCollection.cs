using System.Data;
using System.Data.SqlClient;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Invoice
{
    public class InvoiceCollection
    {
        public DataTable GetNewInvoiceRevision(string invoiceNo)
        {
            DataTable tbl = null;
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);

            string SqlSelectString =
                "SELECT [invoiceNo], [Revision] FROM [Sales_JobMasterList_Invoice] WHERE ([invoiceNo] = @invoiceNo)";

            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);

            var adapter2 = new SqlDataAdapter(SelectCommand);
            adapter2.SelectCommand.Parameters.Add("@invoiceNo", SqlDbType.NVarChar).Value = invoiceNo;

            var ds2 = new DataSet();
            ds2.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                int rowsAffected = adapter2.Fill(ds2, "t2");
                if (rowsAffected > 0)
                {
                    tbl = ds2.Tables["t2"];
                }
            }
            catch (SqlException ex)
            {
                string errorlog = ex.Message;
            }
            finally
            {
                ConnectionSQL.Close();
            }

            return tbl;
        }
    }
}