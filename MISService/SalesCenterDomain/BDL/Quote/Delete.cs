using System.Data;
using System.Data.SqlClient;
using SalesCenterDomain.BDL.Service;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Quote
{
    public class QuoteTitleDelete : BaseObjDelete
    {
        private readonly int _myID; //QuoteRevID
        private string _recordType = "Q";

        public QuoteTitleDelete(int myID)
        {
            _myID = myID;
        }

        public string RecordType
        {
            set { _recordType = value; }
        }

        public override int GetDeleteValidationResult()
        {
            //?workorder, invoice generated
            int result = 0;
            var qtp = new QuoteTitleProperty(_myID);
            bool b = qtp.IsContracted;
            if (b)
            {
                result = (int) NValidationErrorValue.Delete;
            }
            else
            {
                result = (int) NValidationErrorValue.OK;
            }
            return result;
        }


        public override void DeleteItems()
        {
            //Define
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString =
                "SELECT * FROM [Quote_Item] WHERE ([recordType]=@recordType and [QuoteRevID] = @quoteRevID)";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand);
            var ds1 = new DataSet();
            ds1.Tables.Clear();

            adapter1.SelectCommand.Parameters.Add("@quoteRevID", SqlDbType.Int).Value = _myID;
            adapter1.SelectCommand.Parameters.Add("@recordType", SqlDbType.Char).Value = _recordType;

            try
            {
                ConnectionSQL.Open();
                int rowsAffected = adapter1.Fill(ds1, "t1");

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


        public override void DeleteServices()
        {
            var svc = new FsService(_myID, "Quote");
            svc.DeleteServices();
        }


        public override void DeleteNotes()
        {
            int rowsAffected = 0;
            //Define
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString = "SELECT * FROM [FW_Quote_note] WHERE ([estRevID] = @estRevID)";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand);
            var ds1 = new DataSet();
            ds1.Tables.Clear();
            adapter1.SelectCommand.Parameters.Add("@estRevID", SqlDbType.Int).Value = _myID;
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


        public override void DeleteTitle()
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string delString = "DELETE FROM  [Sales_JobMasterList_QuoteRev] WHERE ([quoteRevID] = @quoteRevID)";
                var delCommand = new SqlCommand(delString, Connection);
                delCommand.Parameters.Add("@quoteRevID", SqlDbType.Int).Value = _myID;
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