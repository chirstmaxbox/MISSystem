using System.Data;
using System.Data.SqlClient;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Invoice
{
    public class InvoiceField
    {
        private readonly int _myID;

        public InvoiceField(int myID)
        {
            _myID = myID;
        }

        public DataRow GetInvoiceTitleDataRow()
        {
            //by ID

            DataRow row = null;

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 = "SELECT * FROM [Sales_JobMasterList_Invoice] WHERE ([invoiceID] = @invoiceID)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@invoiceID", SqlDbType.Int).Value = _myID;
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

        public void UpdateNumber(string invoiceNumber)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Sales_JobMasterList_Invoice] SET [InvoiceNo] = @InvoiceNo WHERE [invoiceID] = @InvoiceID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@InvoiceID", SqlDbType.Int).Value = _myID;
                UPdateCommand.Parameters.Add("@InvoiceNo", SqlDbType.NVarChar, 20).Value = invoiceNumber;
                try
                {
                    Connection.Open();
                    UPdateCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    string errorlog = ex.Message;
                }
                finally
                {
                    Connection.Close();
                }
            }
        }

        public void UpdateCustomerID(int customerID)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Sales_JobMasterList_Invoice] SET [CustomerID] = @CustomerID WHERE [invoiceID] = @InvoiceID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@InvoiceID", SqlDbType.Int).Value = _myID;
                UPdateCommand.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerID;
                try
                {
                    Connection.Open();
                    UPdateCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    string errorlog = ex.Message;
                }
                finally
                {
                    Connection.Close();
                }
            }
        }

        public void UpdateContactID(int contactID)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Sales_JobMasterList_Invoice] SET [ContactID] = @ContactID WHERE [invoiceID] = @InvoiceID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@InvoiceID", SqlDbType.Int).Value = _myID;
                UPdateCommand.Parameters.Add("@ContactID", SqlDbType.Int).Value = contactID;
                try
                {
                    Connection.Open();
                    UPdateCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    string errorlog = ex.Message;
                }
                finally
                {
                    Connection.Close();
                }
            }
        }
    }
}