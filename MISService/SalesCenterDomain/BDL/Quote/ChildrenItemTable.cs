using System.Data;
using System.Data.SqlClient;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Quote
{
    public class QuoteChildrenItemTable
    {
        private readonly int _myID;

        public QuoteChildrenItemTable(int quoteRevID)
        {
            _myID = quoteRevID;
        }

        public DataTable GetItems(string recordType, bool isFinalOnly)
        {
            //Input: parentItem, such as quoteRevID, 
            //      recordType=Q

            DataTable tbl = null;

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString =
                "SELECT * FROM [Quote_Item] WHERE [quoteRevID] = @quoteRevID and [recordType]=@recordType and ([isFinal]=@isFinalOnly or @isFinalOnly=0)";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand);
            adapter1.SelectCommand.Parameters.Add("@quoteRevID", SqlDbType.Int).Value = _myID;
            adapter1.SelectCommand.Parameters.Add("@recordType", SqlDbType.Char).Value = recordType;
            adapter1.SelectCommand.Parameters.Add("@isFinalOnly", SqlDbType.Bit).Value = isFinalOnly;

            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                int rowsAffected = adapter1.Fill(ds1, "t1");
                if (rowsAffected > 0)
                {
                    tbl = ds1.Tables["t1"];
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

            return tbl;
        }
    }
}