using System.Data;
using System.Data.SqlClient;
using MyCommon.MyEnum;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Invoice
{
    public class InvoiceType
    {
        //

        private readonly int _invoiceID;
        private readonly int _oldInvoiceType;

        public InvoiceType(int invoiceID)
        {
            _invoiceID = invoiceID;
            var ip = new InvoiceProperty(invoiceID);
            _oldInvoiceType = ip.InvoiceType;
        }


        public void ChangeTo(int newInvoiceType)
        {
            UpdateInvoiceTypeRecord(newInvoiceType);

            //Delete associated workorder
            if (_oldInvoiceType != (int) NInvoiceType.Proforma & newInvoiceType == (int) NInvoiceType.Proforma)
            {
                var iw = new InvoiceWorkorder(_invoiceID);
                iw.DeleteAllWorkorders();
            }
        }

        private void UpdateInvoiceTypeRecord(int newInvoiceType)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Sales_JobMasterList_Invoice] SET [invoiceType] = @InvoiceType WHERE [invoiceID] = @InvoiceID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@InvoiceID", SqlDbType.Int).Value = _invoiceID;
                UPdateCommand.Parameters.Add("@InvoiceType", SqlDbType.Int).Value = newInvoiceType;
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