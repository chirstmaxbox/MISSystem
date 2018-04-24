using System;
using System.Data;
using System.Data.SqlClient;
using MyCommon.MyEnum;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Invoice
{
    /// <summary>
    /// Only One invoice allowed to be "Approved"
    /// </summary>
    /// <remarks></remarks>
    public class InvoiceStatus
    {
        private readonly int _invoiceID;

        public InvoiceStatus(int invoiceID)
        {
            _invoiceID = invoiceID;
        }


        public void Approve()
        {
            string invoiceNumber = "";
            var ip = new InvoiceProperty(_invoiceID);
            invoiceNumber = ip.InvoiceNumber;
            ApproveInvoice(invoiceNumber);
        }

        private void ApproveInvoice(String invoiceNumber)
        {
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString = "Select * FROM Sales_JobMasterList_Invoice WHERE ([InvoiceNo] = @InvoiceNumber )";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand);
            var ds1 = new DataSet();
            ds1.Tables.Clear();

            adapter1.SelectCommand.Parameters.Add("@InvoiceNumber", SqlDbType.NVarChar, 15).Value = invoiceNumber;

            try
            {
                ConnectionSQL.Open();
                dynamic rowsAffected = adapter1.Fill(ds1, "t1");

                if (rowsAffected > 0)
                {
                    foreach (DataRow row in ds1.Tables["t1"].Rows)
                    {
                        int rowInvoiceID = Convert.ToInt32(row["invoiceID"]);
                        if (rowInvoiceID == _invoiceID)
                        {
                            row["iStatus"] = (int) NJobStatus.InvApproved;
                        }
                        else
                        {
                            dynamic rowInvoiceStatus = row["iStatus"];
                            if (rowInvoiceStatus == (int) NJobStatus.InvApproved)
                            {
                                row["iStatus"] = (int) NJobStatus.woInvalid;
                            }
                        }
                    }

                    //1.3. Write  back to DB
                    var cb = new SqlCommandBuilder(adapter1);
                    adapter1 = cb.DataAdapter;
                    rowsAffected += adapter1.Update(ds1, "t1");
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
        }


        public void ChangeTo(int status)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Sales_JobMasterList_Invoice] SET [iStatus] = @iStatus WHERE [invoiceID] = @InvoiceID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@InvoiceID", SqlDbType.Int).Value = _invoiceID;
                UPdateCommand.Parameters.Add("@iStatus", SqlDbType.Int).Value = status;

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