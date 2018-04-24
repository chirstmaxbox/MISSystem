using System;
using System.Data;
using System.Data.SqlClient;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Item
{
    public class QuoteNoteCreateNew
    {
        private readonly int _quoteRevID;

        public QuoteNoteCreateNew(int quoteRevID)
        {
            _quoteRevID = quoteRevID;
        }

        public void Insert()
        {
            //Insert documents for items
            using (var connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                const string insertString =
                    "INSERT INTO FW_QUOTE_NOTE(estRevID, qnPrintOrder, qnDescription, qnTitle) VALUES (@estRevID, @qnPrintOrder, @qnDescription, @qnTitle)";

                var insertCommand = new SqlCommand(insertString, connection);

                try
                {
                    connection.Open();

                    int printOrder = GetMaxQsPrintOrder() + 1;

                    insertCommand.Parameters.Add("@estRevID", SqlDbType.Int).Value = _quoteRevID;
                    insertCommand.Parameters.Add("@qnPrintOrder", SqlDbType.Int).Value = printOrder;

                    insertCommand.Parameters.Add("@qnDescription", SqlDbType.NVarChar, 100).Value = "";


                    insertCommand.Parameters.Add("@qnTitle", SqlDbType.NVarChar, 500).Value = "";

                    insertCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    string errorLog = ex.Message;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private int GetMaxQsPrintOrder()
        {
            var qns = new QuoteNoteSelection(_quoteRevID);
            return qns.ChildrenCount;
        }
    }

    public class QuoteNoteSelection
    {
        private readonly int _quoteRevID;

        public QuoteNoteSelection(int quoteRevID)
        {
            _quoteRevID = quoteRevID;
            ChildrenCount = GetChildrenCount();
        }

        public int ChildrenCount { get; private set; }

        private int GetChildrenCount()
        {
            int count = 0;
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString = "SELECT COUNT(qnID) AS Count FROM FW_QUOTE_NOTE WHERE (estRevID = @estRevID)";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand);
            var ds1 = new DataSet();
            ds1.Tables.Clear();
            adapter1.SelectCommand.Parameters.Add("@estRevID", SqlDbType.Int).Value = _quoteRevID;

            try
            {
                ConnectionSQL.Open();
                int rowsAffected = adapter1.Fill(ds1, "t1");
                if (rowsAffected > 0)
                {
                    DataRow row = ds1.Tables["t1"].Rows[0];
                    count = Convert.ToInt32(row["Count"]);
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

            return count;
        }
    }
}