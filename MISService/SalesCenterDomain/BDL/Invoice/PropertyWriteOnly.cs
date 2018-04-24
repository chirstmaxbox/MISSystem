using System.Data;
using System.Data.SqlClient;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Invoice
{
    public class InvoiceTitleUpdate
    {
        private int _myID;

        public InvoiceTitleUpdate(int myID)
        {
            _myID = myID;
        }

        public int MyID
        {
            set { _myID = value; }
        }


        public void UpdateAmount(double newAmount)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Sales_JobMasterList_Invoice] SET [InvoiceAmount] = @InvoiceAmount WHERE [invoiceID] = @InvoiceID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@InvoiceID", SqlDbType.Int).Value = _myID;
                UPdateCommand.Parameters.Add("@InvoiceAmount", SqlDbType.Decimal, 8).Value = newAmount;
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


        public void UpdateTerm(int term)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Sales_JobMasterList_Invoice] SET [Term] = @Term WHERE [invoiceID] = @InvoiceID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@InvoiceID", SqlDbType.Int).Value = _myID;
                UPdateCommand.Parameters.Add("@Term", SqlDbType.Int).Value = term;
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


        public void UpdateRev(int rev)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString ="UPDATE [Sales_JobMasterList_Invoice] SET [Revision] = @Revision WHERE [invoiceID] = @InvoiceID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@InvoiceID", SqlDbType.Int).Value = _myID;
                UPdateCommand.Parameters.Add("@Revision", SqlDbType.Int).Value = rev;

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


        public void UpdateMultiplLegalName(bool isMLN, string mlnText)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Sales_JobMasterList_Invoice] SET [isMultipleLegalName] = @isMLN, [MultipleLegalNameText]=@mlnText WHERE [invoiceID] = @InvoiceID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@InvoiceID", SqlDbType.Int).Value = _myID;
                UPdateCommand.Parameters.Add("@isMLN", SqlDbType.Bit).Value = isMLN;
                UPdateCommand.Parameters.Add("@mlnText", SqlDbType.NVarChar, 300).Value = mlnText;
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

     public void UpdateNote(string myNote)
     {
         using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
         {
             string UpdateString ="UPDATE [Sales_JobMasterList_Invoice] SET [Note] = @Note WHERE [invoiceID] = @InvoiceID";
             var UPdateCommand = new SqlCommand(UpdateString, Connection);

             UPdateCommand.Parameters.Add("@InvoiceID", SqlDbType.Int).Value = _myID;
             UPdateCommand.Parameters.Add("@Note", SqlDbType.NVarChar, 500).Value = myNote;

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