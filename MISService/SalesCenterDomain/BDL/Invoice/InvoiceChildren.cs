using System.Data;
using System.Data.SqlClient;
using MyCommon;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Invoice
{
    public class InvoiceChildren
    {
        public InvoiceChildren(int invoiceID)
        {
            ProductItem = GetProductItem(invoiceID);
            ServiceItem = GetServiceItem(invoiceID);
        }

        public DataTable ProductItem { get; private set; }
        public DataTable ServiceItem { get; private set; }


        private DataTable GetProductItem(int invoiceID)
        {
            DataTable dt = null;
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 =
                "SELECT * FROM [QUOTE_Item] WHERE ([quoteRevID] = @quoteRevID and [recordType]=@recordType)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@quoteRevID", SqlDbType.Int).Value = invoiceID;
            adapter1.SelectCommand.Parameters.Add("@recordType", SqlDbType.Char).Value = "I";

            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                int numRowsAffected = adapter1.Fill(ds1, "t1");
                if (numRowsAffected != 0)
                {
                    dt = ds1.Tables["t1"];
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
            return dt;
        }

        private DataTable GetServiceItem(int invoiceID)
        {
            DataTable dt = null;

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 ="SELECT * FROM [FW_QUOTE_SERVICE] WHERE ([estRevID] = @estRevID and [recordType]=@recordType)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@estRevID", SqlDbType.Int).Value = invoiceID;
            adapter1.SelectCommand.Parameters.Add("@recordType", SqlDbType.NChar, 7).Value = "Invoice";
            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                int numRowsAffected = adapter1.Fill(ds1, "t1");
                if (numRowsAffected != 0)
                {
                    //Destination
                    dt = ds1.Tables["t1"];
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
            return dt;
        }
    }

    public class InvoicePrintValidation
    {
        //    public List<int> Errors { get; set; }

        public InvoicePrintValidation(int invoiceID)
        {
            Result = 0;
            var children = new InvoiceChildren(invoiceID);

            if (!IsProductionItemAllDigit(children.ProductItem))
            {
                Result = (int) NValidationErrorValue.InvoiceProductAmount;
            }

            if (!IsServiceItemAllDigit(children.ServiceItem))
            {
                Result = (int) NValidationErrorValue.InvoiceServiceAmount;
            }
        }

        public int Result { get; set; }

        private bool IsProductionItemAllDigit(DataTable dt)
        {
            bool b = true;
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    object amount = row["qiAmount"];
                    string s = MyConvert.RemoveAccountingFormatCharacter(amount);
                    if (!MyConvert.IsNumeric(s))
                    {
                        b = false;
                    }
                }
            }

            return b;
        }


        private bool IsServiceItemAllDigit(DataTable dt)
        {
            bool b = true;
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    object amount = row["qsAmount"];
                    string s = MyConvert.RemoveAccountingFormatCharacter(amount);
                    if (!MyConvert.IsNumeric(s))
                    {
                        b = false;
                    }
                }
            }
            return b;
        }
    }
}