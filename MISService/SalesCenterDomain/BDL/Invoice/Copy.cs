using System;
using System.Data;
using System.Data.SqlClient;
using MyCommon.MyEnum;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Invoice
{
    //Copy to new job: parentid, invoiceNumber
    //COpy to another new invoiceNumber
    //Copy to another new invoiceVersion


    public class InvoiceTitleCopy : BaseObjCopy
    {
        #region "Construction"

        private string _recordType = "I";

        private string _recordTypeService = "Invoice";

        public InvoiceTitleCopy(int myParentID, int newParentID, int myID)
            : base(myParentID, newParentID, myID)
        {
        }

        public override int CopyTitle(int myParentID, int newParentID, int myID)
        {
            var cit = new CopyInvoiceTitle(myID);
            cit.Copy(myParentID, myID);

            string tblName = "Sales_JobMasterList_Invoice";
            int nID = SqlCommon.GetNewlyInsertedRecordID(tblName);
            return nID;
        }

        #endregion

        #region "Copy"

        public override void CopyItems(int myParentID, int newParentID, int myID, int newID)
        {
            string errorLog = "Nothing";
            int NumRowsAffected = 0;

            //2. Define the destination: InvoiceItem
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 =
                "SELECT * FROM [Invoice_Item] WHERE ([quoteRevID] = @quoteRevID and [recordType]=@recordType)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@quoteRevID", SqlDbType.Int).Value = myID;
            adapter1.SelectCommand.Parameters.Add("@recordType", SqlDbType.Char).Value = _recordType;

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
            adapter1.SelectCommand.Parameters.Add("@recordType", SqlDbType.NChar, 7).Value = _recordTypeService;
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
            //Build Note to customer

            var iField = new InvoiceField(myID);
            DataRow row = iField.GetInvoiceTitleDataRow();
            string note = "This Invoice supercedes invoice No. ";

            string invNo = Convert.ToString(row["invoiceNo"]);
            note += invNo;


            int rev = Convert.ToInt32(row["Revision"]) - 1;
            if (rev > 0)
            {
                string revStr = " Rev " + rev.ToString();
                note += revStr;
            }

            //Write the note to db

            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Sales_JobMasterList_Invoice] SET [Note] = @Note WHERE [invoiceID] = @InvoiceID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@InvoiceID", SqlDbType.Int).Value = newID;
                UPdateCommand.Parameters.Add("@Note", SqlDbType.NVarChar, 500).Value = note;

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

        #endregion
    }


    public class CopyInvoiceTitle : CopyRecords
    {
        public CopyInvoiceTitle(int myID)
        {
            adapter1 = GetSqlDataAdapter(myID);
        }

        private SqlDataAdapter GetSqlDataAdapter(int myID)
        {
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 = "SELECT * FROM [Sales_JobMasterList_Invoice] WHERE ([invoiceID] = @invoiceID)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@invoiceID", SqlDbType.Int).Value = myID;

            return adapter1;
        }

        public override void UpdateContentFields(object myParentID, object myID, DataRow rowNew)
        {
            rowNew["jobID"] = myParentID;
            string invoiceNo = Convert.ToString(rowNew["invoiceNo"]);
            var vc = new VersionControl(invoiceNo);
            rowNew["Revision"] = vc.NewestInvoiceRevision;
            rowNew["iStatus"] = (int) NJobStatus.invNew;
        }
    }
}