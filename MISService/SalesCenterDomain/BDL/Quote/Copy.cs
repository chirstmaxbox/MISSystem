using System;
using System.Data;
using System.Data.SqlClient;
using MyCommon.MyEnum;
using SalesCenterDomain.BDL.Project;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Quote
{
    public class QuoteTitleCopy : BaseObjCopy
    {
        #region "Construction"

        //'??
        public string recordType = "Q";

        public string recordTypeService = "Quote";

        public QuoteTitleCopy(int myParentID, int newParentID, int myID)
            : base(myParentID, newParentID, myID)
        {
        }

        #endregion

        #region "Copy"

        public override int CopyTitle(int myParentID, int newParentID, int myID)
        {
            var cqt = new CopyQuoteTitle(myID);
            cqt.Copy(myParentID, myID);
            string tblName = "Sales_JobMasterList_QuoteRev";
            int nID = SqlCommon.GetNewlyInsertedRecordID(tblName);
            return nID;
        }


        public override void CopyItems(int myParentID, int newParentID, int myID, int newID)
        {
            string errorLog = "Nothing";
            int NumRowsAffected = 0;

            //2. Define the destination: InvoiceItem
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 =
                "SELECT * FROM [QUOTE_Item] WHERE ([quoteRevID] = @quoteRevID and [recordType]=@recordType)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@quoteRevID", SqlDbType.Int).Value = myID;
            adapter1.SelectCommand.Parameters.Add("@recordType", SqlDbType.Char).Value = recordType;

            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                NumRowsAffected = adapter1.Fill(ds1, "t1");
                if (NumRowsAffected != 0)
                {
                    //Destination
                    DataTable t2 = ds1.Tables["t1"].Copy();
                    t2.TableName = "t2";
                    ds1.Tables.Add(t2);
                    ds1.Tables["t2"].Clear();

                    //3. Copy
                    int itemIndex = ds1.Tables["t1"].Columns.Count;
                    DataRow row = null;
                    DataRow rowNew = null;
                    int i = 0;
                    foreach (DataRow row_loopVariable in ds1.Tables["t1"].Rows)
                    {
                        row = row_loopVariable;
                        rowNew = ds1.Tables["t2"].NewRow();
                        for (i = 1; i <= itemIndex - 1; i++)
                        {
                            rowNew[i] = row[i];
                        }

                        //The rows need to change
                        rowNew["quoteRevID"] = newID;

                        ds1.Tables["t2"].Rows.Add(rowNew);
                    }

                    //4. Write ds2,  back to DB

                    var cb = new SqlCommandBuilder(adapter1);
                    adapter1 = cb.DataAdapter;
                    NumRowsAffected = adapter1.Update(ds1, "t2");
                }
            }
            catch (SqlException ex)
            {
                errorLog = ex.Message;
            }
            finally
            {
                ConnectionSQL.Close();
            }
        }

        public override void CopyServices(int myParentID, int newParentID, int myID, int newID)
        {
            string errorLog = "Nothing";
            int NumRowsAffected = 0;

            //2. Define the destination: InvoiceItem
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 =
                "SELECT * FROM [FW_QUOTE_SERVICE] WHERE ([estRevID] = @estRevID and [recordType]=@recordType)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@estRevID", SqlDbType.Int).Value = myID;
            adapter1.SelectCommand.Parameters.Add("@recordType", SqlDbType.NChar, 7).Value = recordTypeService;
            var ds1 = new DataSet();

            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                NumRowsAffected = adapter1.Fill(ds1, "t1");
                if (NumRowsAffected != 0)
                {
                    //Destination
                    DataTable t2 = ds1.Tables["t1"].Copy();
                    t2.TableName = "t2";
                    ds1.Tables.Add(t2);
                    ds1.Tables["t2"].Clear();

                    //3. Copy
                    int itemIndex = ds1.Tables["t1"].Columns.Count;
                    DataRow row = null;
                    DataRow rowNew = null;
                    int i = 0;
                    foreach (DataRow row_loopVariable in ds1.Tables["t1"].Rows)
                    {
                        row = row_loopVariable;
                        rowNew = ds1.Tables["t2"].NewRow();
                        for (i = 1; i <= itemIndex - 1; i++)
                        {
                            rowNew[i] = row[i];
                        }

                        rowNew["estRevID"] = newID;
                        ds1.Tables["t2"].Rows.Add(rowNew);
                    }

                    //4. Write ds2,  back to DB

                    var cb = new SqlCommandBuilder(adapter1);
                    adapter1 = cb.DataAdapter;
                    NumRowsAffected = adapter1.Update(ds1, "t2");
                }
            }
            catch (SqlException ex)
            {
                errorLog = ex.Message;
            }
            finally
            {
                ConnectionSQL.Close();
            }
        }


        public override void CopyNotes(int myParentID, int newParentID, int myID, int newID)
        {
            string errorLog = "Nothing";
            int NumRowsAffected = 0;

            //2. Define the destination: InvoiceItem
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 = "SELECT * FROM [FW_QUOTE_NOTE] WHERE ([estRevID] = @estRevID )";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@estRevID", SqlDbType.Int).Value = myID;

            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                NumRowsAffected = adapter1.Fill(ds1, "t1");
                if (NumRowsAffected != 0)
                {
                    //Destination
                    DataTable t2 = ds1.Tables["t1"].Copy();
                    t2.TableName = "t2";
                    ds1.Tables.Add(t2);
                    ds1.Tables["t2"].Clear();

                    //3. Copy
                    int itemIndex = ds1.Tables["t1"].Columns.Count;
                    DataRow row = null;
                    DataRow rowNew = null;
                    int i = 0;
                    foreach (DataRow row_loopVariable in ds1.Tables["t1"].Rows)
                    {
                        row = row_loopVariable;
                        rowNew = ds1.Tables["t2"].NewRow();
                        for (i = 1; i <= itemIndex - 1; i++)
                        {
                            rowNew[i] = row[i];
                        }

                        //The rows need to change
                        rowNew["estRevID"] = newID;

                        ds1.Tables["t2"].Rows.Add(rowNew);
                    }

                    //4. Write ds2,  back to DB

                    var cb = new SqlCommandBuilder(adapter1);
                    adapter1 = cb.DataAdapter;
                    NumRowsAffected = adapter1.Update(ds1, "t2");
                }
            }
            catch (SqlException ex)
            {
                errorLog = ex.Message;
            }
            finally
            {
                ConnectionSQL.Close();
            }
        }

        #endregion
    }


    public class CopyQuoteTitle : CopyRecords
    {
        public CopyQuoteTitle(int myID)
        {
            adapter1 = getSqlDataAdapter(myID);
        }

        private SqlDataAdapter getSqlDataAdapter(int myID)
        {
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString = "Select * FROM [Sales_JobMasterList_QuoteRev] WHERE ([quoteRevID] = @quoteRevID)";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand);
            adapter1.SelectCommand.Parameters.Add("@quoteRevID", SqlDbType.Int).Value = myID;
            return adapter1;
        }

        public override void UpdateContentFields(object jobID, object estRevID, DataRow rowNew)
        {
            //New Version
            var pcv = new ProjectChildrenVersion(Convert.ToInt32(jobID));
            rowNew["quoteRev"] = pcv.NewestQuoteRev;
            rowNew["isssueDate"] = DateTime.Today;
            if (Convert.ToInt32(rowNew["quoteStatus"]) == (int) NJobStatus.win)
            {
                rowNew["quoteStatus"] = (int) NJobStatus.qProcessing;
                rowNew["contractNumber"] = DBNull.Value;
                rowNew["contractAmount"] = DBNull.Value;
                rowNew["contractDate"] = DBNull.Value;
            }
        }
    }
}