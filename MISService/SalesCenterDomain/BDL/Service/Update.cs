using System;
using System.Data;
using System.Data.SqlClient;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Service
{
    public class ServiceUpdate
    {
        private readonly int _qiPrintOrder;

        private readonly int _quoteItemID;
        private readonly int _quoteRevID;

        public ServiceUpdate(int quoteRevID, int qiPrintOrder, int quoteItemID)
        {
            _quoteRevID = quoteRevID;
            _qiPrintOrder = qiPrintOrder;
            _quoteItemID = quoteItemID;
        }


        public void UpdateServiceTitleOfSameGroup()
        {
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 =
                "SELECT * FROM [FW_QUOTE_SERVICE] WHERE ([estRevID] = @estRevID and [recordType]='Quote' AND [qsPrintOrder]=@qsPrintOrder)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@estRevID", SqlDbType.Int).Value = _quoteRevID;
            adapter1.SelectCommand.Parameters.Add("@qsPrintOrder", SqlDbType.Int).Value = _qiPrintOrder;
            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                int NumRowsAffected = adapter1.Fill(ds1, "t1");
                if (NumRowsAffected != 0)
                {
                    //find original row
                    DataRow rowO = null;
                    foreach (DataRow row in ds1.Tables["t1"].Rows)
                    {
                        if (Convert.ToInt32(row["qsID"]) == _quoteItemID)
                        {
                            rowO = row;
                        }
                        break; // TODO: might not be correct. Was : Exit For
                    }

                    if (rowO != null)
                    {
                        string s = Convert.ToString(rowO["qsTitle"]);
                        foreach (DataRow row in ds1.Tables["t1"].Rows)
                        {
                            if (Convert.ToInt32(row["qsID"]) != _quoteItemID)
                            {
                                row["qsTitle"] = s;
                            }
                        }
                    }

                    //4. Write ds2,  back to DB

                    var cb = new SqlCommandBuilder(adapter1);
                    adapter1 = cb.DataAdapter;
                    NumRowsAffected = adapter1.Update(ds1, "t1");
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


        public void UpdateServicePrintOrder(int printOrderOldValue)
        {
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 =
                "SELECT * FROM [FW_QUOTE_SERVICE] WHERE ([estRevID] = @estRevID and [recordType]='Quote' AND [qsPrintOrder]=@qsPrintOrder)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@estRevID", SqlDbType.Int).Value = _quoteRevID;
            adapter1.SelectCommand.Parameters.Add("@qsPrintOrder", SqlDbType.Int).Value = printOrderOldValue;
            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                int NumRowsAffected = adapter1.Fill(ds1, "t1");
                if (NumRowsAffected != 0)
                {
                    foreach (DataRow row in ds1.Tables["t1"].Rows)
                    {
                        row["qsPrintOrder"] = _qiPrintOrder;
                    }

                    //4. Write ds2,  back to DB
                    var cb = new SqlCommandBuilder(adapter1);
                    adapter1 = cb.DataAdapter;
                    NumRowsAffected = adapter1.Update(ds1, "t1");
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
    }
}