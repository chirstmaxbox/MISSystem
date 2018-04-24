using System;
using System.Data;
using System.Data.SqlClient;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Quote
{
    public class QuoteDataRow
    {
        public static DataRow GetQuoteTitleDataRow(int quoteRevID)
        {
            //Case GenerateInvoiceFrom.Quote      'Quote
            DataRow row = null;
            int rowsAffected = 0;

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            //
            string SqlSelectString =
                "SELECT View_InvoiceTitleFromQuote.* FROM View_InvoiceTitleFromQuote WHERE (quoteRevID = @quoteRevID)";

            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter2 = new SqlDataAdapter(SelectCommand);
            var ds2 = new DataSet();

            adapter2.SelectCommand.Parameters.Add("@quoteRevID", SqlDbType.Int).Value = quoteRevID;

            try
            {
                ConnectionSQL.Open();
                rowsAffected = adapter2.Fill(ds2, "t2");
                if (rowsAffected > 0)
                {
                    row = ds2.Tables["t2"].Rows[0];
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

            return row;
        }


        public static DataRow GetQuoteTitleDataRowOriginal(int quoteRevID)
        {
            //Case GenerateInvoiceFrom.Quote      'Quote
            DataRow row = null;
            int rowsAffected = 0;

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);

            string SqlSelectString = "Select * FROM [Sales_JobMasterList_QuoteRev] WHERE ([quoteRevID] = @quoteRevID)";

            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter2 = new SqlDataAdapter(SelectCommand);
            var ds2 = new DataSet();

            adapter2.SelectCommand.Parameters.Add("@quoteRevID", SqlDbType.Int).Value = quoteRevID;

            try
            {
                ConnectionSQL.Open();
                rowsAffected = adapter2.Fill(ds2, "t2");
                if (rowsAffected > 0)
                {
                    row = ds2.Tables["t2"].Rows[0];
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

            return row;
        }
    }


    public class FieldUpdate
    {
        private readonly int _myID;

        public FieldUpdate(int quoteRevID)
        {
            _myID = quoteRevID;
        }


        public void UpdateQuoteAmount(double quoteAmount)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Sales_JobMasterList_QuoteRev] SET [quoteAmount] = @quoteAmount WHERE [QuoteRevID] = @QuoteRevID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@QuoteRevID", SqlDbType.Int).Value = _myID;
                UPdateCommand.Parameters.Add("@quoteAmount", SqlDbType.Real).Value = quoteAmount;
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


        public void UpdateContractDate(DateTime contractDate)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Sales_JobMasterList_QuoteRev] SET [contractDate] = @contractDate WHERE [QuoteRevID] = @QuoteRevID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@QuoteRevID", SqlDbType.Int).Value = _myID;
                UPdateCommand.Parameters.Add("@contractDate", SqlDbType.SmallDateTime).Value = contractDate;

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


        public void UpdateEstRevID(int estRevID)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Sales_JobMasterList_QuoteRev] SET [estRevID] = @estRevID WHERE [QuoteRevID] = @QuoteRevID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@QuoteRevID", SqlDbType.Int).Value = _myID;
                UPdateCommand.Parameters.Add("@estRevID", SqlDbType.Int).Value = estRevID;

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


        public void UpdateOrderConfirmationPrinted(int printTimes)
        {
            using (var connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                const string updateString =
                    "UPDATE [Sales_JobMasterList_QuoteRev] SET [OrderConfirmationPrinted] = @OrderConfirmationPrinted WHERE [QuoteRevID] = @QuoteRevID";
                var uPdateCommand = new SqlCommand(updateString, connection);

                uPdateCommand.Parameters.Add("@QuoteRevID", SqlDbType.Int).Value = _myID;
                uPdateCommand.Parameters.Add("@OrderConfirmationPrinted", SqlDbType.Int).Value = printTimes;

                try
                {
                    connection.Open();
                    uPdateCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    string errorlog = ex.Message;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public void UpdateIsLost(bool isLost, int jobID)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                try
                {
                    Connection.Open();
                    //QUOTE
                    string UpdateString ="UPDATE [Sales_JobMasterList_QuoteRev] SET [isLost] = @isLost WHERE [QuoteRevID] = @QuoteRevID";
                    var UPdateCommand = new SqlCommand(UpdateString, Connection);

                    UPdateCommand.Parameters.Add("@QuoteRevID", SqlDbType.Int).Value = _myID;
                    UPdateCommand.Parameters.Add("@isLost", SqlDbType.Bit).Value = isLost;


                    UPdateCommand.ExecuteNonQuery();

                    //JOBMASTER

                    UpdateString = "UPDATE [Sales_JobMasterList] SET [isLost] = @isLost WHERE [jobID] = @jobID";
                    UPdateCommand = new SqlCommand(UpdateString, Connection);


                    UPdateCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = jobID;
                    UPdateCommand.Parameters.Add("@isLost", SqlDbType.Bit).Value = isLost;

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

        public void UpdateIsFinished(bool isFininshed)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                try
                {
                    Connection.Open();
                    //QUOTE
                    string UpdateString =
                        "UPDATE [Sales_JobMasterList_QuoteRev] SET [isFinished]= @IsFinished, [FinishedDate]=@FinishedDate WHERE [QuoteRevID] = @QuoteRevID";
                    var UPdateCommand = new SqlCommand(UpdateString, Connection);

                    UPdateCommand.Parameters.Add("@QuoteRevID", SqlDbType.Int).Value = _myID;
                    UPdateCommand.Parameters.Add("@IsFinished", SqlDbType.Bit).Value = isFininshed;
                    UPdateCommand.Parameters.Add("@FinishedDate", SqlDbType.DateTime).Value = DateTime.Now;


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


        public void UpdateTaxOption(int taxOption)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Sales_JobMasterList_QuoteRev] SET [TaxOption] = @TaxOption WHERE [QuoteRevID] = @QuoteRevID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@QuoteRevID", SqlDbType.Int).Value = _myID;
                UPdateCommand.Parameters.Add("@estRevID", SqlDbType.Int).Value = taxOption;

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

        public void UpdateContractFileName(string contractFileName)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Sales_JobMasterList_QuoteRev] SET [ContractFileName] = @ContractFileName WHERE [QuoteRevID] = @QuoteRevID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@QuoteRevID", SqlDbType.Int).Value = _myID;
                UPdateCommand.Parameters.Add("@ContractFileName", SqlDbType.NVarChar, 50).Value = contractFileName;
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

        public void UpdateAddNewItemReason(string reason)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Sales_JobMasterList_QuoteRev] SET [ReasonOfAddProduct] = @ReasonOfAddProduct WHERE [QuoteRevID] = @QuoteRevID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@QuoteRevID", SqlDbType.Int).Value = _myID;
                UPdateCommand.Parameters.Add("@ReasonOfAddProduct", SqlDbType.NVarChar, 200).Value = reason;
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