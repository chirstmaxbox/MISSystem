using System.Data;
using System.Data.SqlClient;
using MyCommon.MyEnum;
using SalesCenterDomain.BDL.NumberControl;
using SalesCenterDomain.BDL.Service;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Invoice
{
    public class InvoiceTitleDelete
    {
        //? might inherits from QuoteTitleDelete

        private readonly int _myID;

        public InvoiceTitleDelete(int myID)
        {
            _myID = myID;
        }

        public bool IsDeletable
        {
            get { return GetIsDeletable(); }
        }

        public void Delete()
        {
            //Release Number
            int invoiceType = GetInvoiceType();
            var nc = new InvoiceNumberController(_myID, invoiceType);
            nc.ReleaseInvoiceNumber();
            DeleteItems();
            DeleteServices();

            DeleteWorkorders();
            DeleteTitle();
        }


        private bool GetIsDeletable()
        {
            var ip = new InvoiceProperty(_myID);
            return ip.InvoiceStatus == (int) NJobStatus.invNew ? true : false;
        }


        private int GetInvoiceType()
        {
            var ip = new InvoiceProperty(_myID);
            return ip.InvoiceType;
        }


        private void DeleteItems()
        {
            int rowsAffected = 0;
            //Define
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString =
                "SELECT * FROM [Invoice_Item] WHERE ([recordType]=@recordType and [QuoteRevID] = @quoteRevID)";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand);
            var ds1 = new DataSet();
            ds1.Tables.Clear();

            adapter1.SelectCommand.Parameters.Add("@quoteRevID", SqlDbType.Int).Value = _myID;
            adapter1.SelectCommand.Parameters.Add("@recordType", SqlDbType.Char).Value = "I";

            try
            {
                ConnectionSQL.Open();
                rowsAffected = adapter1.Fill(ds1, "t1");

                if (rowsAffected > 0)
                {
                    DataRow row = null;

                    foreach (DataRow row_loopVariable in ds1.Tables["t1"].Rows)
                    {
                        row = row_loopVariable;
                        row.Delete();
                    }

                    //. Write  back to DB
                    var cb = new SqlCommandBuilder(adapter1);
                    adapter1 = cb.DataAdapter;
                    rowsAffected += adapter1.Update(ds1, "t1");
                }
            }
            catch (SqlException ex)
            {
                string errLog = ex.Message;
            }
            finally
            {
                ConnectionSQL.Close();
            }
        }

        private void DeleteServices()
        {
            var svc = new FsService(_myID, "Invoice");
            svc.DeleteServices();
        }

        private void DeleteWorkorders()
        {
            int rowsAffected = 0;
            //Define
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString =
                "SELECT * FROM [Sales_JobMasterList_Invoice_Workorder] WHERE ([InvoiceID]=@InvoiceID)";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand);
            var ds1 = new DataSet();
            ds1.Tables.Clear();

            adapter1.SelectCommand.Parameters.Add("@InvoiceID", SqlDbType.Int).Value = _myID;


            try
            {
                ConnectionSQL.Open();
                rowsAffected = adapter1.Fill(ds1, "t1");

                if (rowsAffected > 0)
                {
                    DataRow row = null;

                    foreach (DataRow row_loopVariable in ds1.Tables["t1"].Rows)
                    {
                        row = row_loopVariable;
                        row.Delete();
                    }

                    //. Write  back to DB
                    var cb = new SqlCommandBuilder(adapter1);
                    adapter1 = cb.DataAdapter;
                    rowsAffected += adapter1.Update(ds1, "t1");
                }
            }
            catch (SqlException ex)
            {
                string errLog = ex.Message;
            }
            finally
            {
                ConnectionSQL.Close();
            }
        }

        private void DeleteTitle()
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string delString = "DELETE FROM  [Sales_JobMasterList_Invoice] WHERE ([invoiceID] = @InvoiceID)";
                var delCommand = new SqlCommand(delString, Connection);
                delCommand.Parameters.Add("@invoiceID", SqlDbType.Int).Value = _myID;
                try
                {
                    Connection.Open();
                    delCommand.ExecuteNonQuery();
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