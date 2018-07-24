using System;
using System.Data;
using System.Data.SqlClient;
using MyCommon;
using MyCommon.MyEnum;
using SalesCenterDomain.BDL.Invoice;
using SalesCenterDomain.BLL;
using SalesCenterDomain.BO;
using SpecDomain.BLL.EstItem;

namespace SalesCenterDomain.BDL.Quote
{
    public class QuoteItemBlank
    {
        private readonly int _myParentID;
        public int NewID { set; get; }

        public QuoteItemBlank(int myParentID)
        {
            _myParentID = myParentID;
        }

        public void CreateNew()
        {
            InsertRecord();
            NewID = SqlCommon.GetNewlyInsertedRecordID("Quote_Item");
            UpdateSerialID(NewID);
        }

        private void InsertRecord()
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string InsertString =
                    "INSERT INTO [Quote_Item] ([estItemID], [quoteRevID], [qiPrintOrder], [quoteOption], [supplyType], [qiItemTitle], [qiDescription], [qiAmount], [qiUnitPrice], [qiQty], [recordType], [qiQtyPc],[Requirement],[Position], [quoteOptionText], [qiAmountText]) VALUES (@estItemID, @quoteRevID, @qiPrintOrder, @quoteOption, @supplyType, @qiItemTitle, @qiDescription, @qiAmount, @qiUnitPrice, @qiQty, @recordType,@qiQtyPc, @Requirement, @Position, @quoteOptionText, @qiAmountText)";

                // Create the command and set its properties.
                var InsertCommand = new SqlCommand(InsertString, Connection);
                try
                {
                    Connection.Open();

                    //[Quote_Item] ([estItemID], [quoteRevID], [qiPrintOrder], [quoteOption], [supplyType], [qiItemTitle], [qiDescription], [qiAmount], [qiUnitPrice], [qiQty], [recordType]) 
                    //Values        (@estItemID, @quoteRevID,  @qiPrintOrder,  @quoteOption,  @supplyType,  @qiItemTitle,  @qiDescription,  @qiAmount,  @qiUnitPrice,  @qiQty,  @recordType)"

                    InsertCommand.Parameters.Add("@estItemID", SqlDbType.Int).Value = 0;
                    InsertCommand.Parameters.Add("@quoteRevID", SqlDbType.Int).Value = _myParentID;
                    InsertCommand.Parameters.Add("@qiPrintOrder", SqlDbType.SmallInt).Value =
                        QuoteItemShared.GetMaxItemPrintOrderNumber(_myParentID);
                    InsertCommand.Parameters.Add("@quoteOption", SqlDbType.SmallInt).Value = 1;
                    InsertCommand.Parameters.Add("@supplyType", SqlDbType.SmallInt).Value =
                        NQuoteSupplyType.SupplyAndInstallation;
                    InsertCommand.Parameters.Add("@qiItemTitle", SqlDbType.NVarChar, 500).Value = "My Title";
                    InsertCommand.Parameters.Add("@qiDescription", SqlDbType.NVarChar, 1500).Value = "My Description";
                    InsertCommand.Parameters.Add("@qiAmount", SqlDbType.Decimal, 8).Value = 0;
                    InsertCommand.Parameters.Add("@qiUnitPrice", SqlDbType.Decimal, 8).Value = 0;
                    InsertCommand.Parameters.Add("@QiQty", SqlDbType.SmallInt).Value = 1;
                    InsertCommand.Parameters.Add("@recordType", SqlDbType.Char).Value = "Q";

                    InsertCommand.Parameters.Add("@qiQtyPc", SqlDbType.Int).Value = 0;
                    InsertCommand.Parameters.Add("@Requirement", SqlDbType.Int).Value =NWorkorderRequirement.Installation;
                    InsertCommand.Parameters.Add("@Position", SqlDbType.NVarChar, 7).Value = "Outdoor";

                    InsertCommand.Parameters.Add("@quoteOptionText", SqlDbType.NVarChar, 300).Value = 1;
                    InsertCommand.Parameters.Add("@qiAmountText", SqlDbType.NVarChar, 300).Value = 0;

                    InsertCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    string errorLog = ex.Message;
                }
                finally
                {
                    Connection.Close();
                }
            }
        }

        private void UpdateSerialID(int myNewID)
        {
            var mqi = new MyQuoteItem(_myParentID, myNewID);
            mqi.GenerateMySelfSerialID();
        }
    }


    public class QuoteItemsGenerateFromEstimation
    {
        private readonly int _estRevID;
        private readonly int _quoteRevID;

        public QuoteItemsGenerateFromEstimation(int quoteRevID, int estRevID)
        {
            _quoteRevID = quoteRevID;
            _estRevID = estRevID;
        }

        public void Generate()
        {
            DataTable tbl = EstItemCommon.GetEstimationItems(_estRevID);
            if (tbl == null) return;

            foreach (DataRow row in tbl.Rows)
            {
                var qi = new QuoteItemGenerateFromEstimationSingleItem(_quoteRevID, row);
                qi.Generate();
            }
        }

        public int Generate(int estItemID)
        {
            int id = 0;
            DataTable tbl = EstItemCommon.GetEstimationItems(_estRevID, estItemID);
            if (tbl == null) return id;


            foreach (DataRow row in tbl.Rows)
            {
                var qi = new QuoteItemGenerateFromEstimationSingleItem(_quoteRevID, row);
                qi.Generate();
                id = qi.NewlyInsertedQuoteItemID;
            }

            return id;
        }
    }


    

    //Add a single Quote Item from estimation Item
    public class QuoteItemGenerateFromEstimationSingleItem
    {
        private readonly int _quoteRevID;
        private readonly DataRow _row;
        public int NewlyInsertedQuoteItemID { get; private set; }

        public QuoteItemGenerateFromEstimationSingleItem(int quoteRevID, int estItemID)
        {
            _quoteRevID = quoteRevID;
            _row = GetEstimationRow(estItemID);
        }

        public QuoteItemGenerateFromEstimationSingleItem(int quoteRevID, DataRow row)
        {
            _quoteRevID = quoteRevID;
            _row = row;
        }


        public void Generate()
        {
            var connectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);

            const string sqlSelectString2 = "SELECT *  FROM [Quote_Item] WHERE ([quoteItemID] = 0) ";
            var selectCommand2 = new SqlCommand(sqlSelectString2, connectionSQL);
            var adapter2 = new SqlDataAdapter(selectCommand2);

            var ds2 = new DataSet();
            ds2.Tables.Clear();

            try
            {
                connectionSQL.Open();
                adapter2.Fill(ds2, "t2");

                DataRow rowNew = ds2.Tables["t2"].NewRow();

                rowNew["quoteRevID"] = _quoteRevID;
                rowNew["estItemID"] = _row["estItemID"];
                rowNew["qiPrintOrder"] = _row["estItemNo"];
                rowNew["quoteOption"] = _row["ItemOption"];
                rowNew["isFinal"] = _row["isFinalOption"];

                int fwJobTypeRequirementID = Convert.ToInt32(_row["RequirementID"]);
                var sp = new SupplyType(fwJobTypeRequirementID);
                rowNew["supplyType"] = sp.QuoteSupplyTypeID;

                rowNew["qiItemTitle"] = _row["ProductName"];

                double qiAmount = MyConvert.ConvertToDouble(_row["PriceA"]);
                rowNew["qiAmount"] = qiAmount;

                rowNew["qiUnitPrice"] = 0;

                rowNew["qiDescription"] = "";

                rowNew["qiQty"] = _row["Qty"];
                rowNew["qiQtyPC"] = 0;

                rowNew["Requirement"] = _row["RequirementID"];
                rowNew["Position"] = "Indoor";

                rowNew["recordType"] = "Q";
                rowNew["quoteOptionText"] = _row["ItemOption"];
                rowNew["qiAmountText"] = qiAmount.ToString("F2");
                rowNew["nameDetailsID"] = _row["ProductID"];
                rowNew["BySubcontractor"] = _row["BySubcontractor"];
                ds2.Tables["t2"].Rows.Add(rowNew);

                //4. Write ds2,  back to DB

                var cb = new SqlCommandBuilder(adapter2);
                adapter2 = cb.DataAdapter;

                adapter2.Update(ds2, "t2");

                NewlyInsertedQuoteItemID  = SqlCommon.GetNewlyInsertedRecordID("Quote_Item");
            }
            catch (SqlException ex)
            {
                string errorLog = ex.Message;
            }
            finally
            {
                connectionSQL.Close();
            }
        }


        private DataRow GetEstimationRow(long estItemID)
        {
            DataRow row = null;
            //2. Define the destination: InvoiceItem
            var connectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            const string sqlSelectString1 = "SELECT * FROM [EST_Item] WHERE ([estItemID] = @estItemID) ";
            var selectCommand1 = new SqlCommand(sqlSelectString1, connectionSQL);
            var adapter1 = new SqlDataAdapter(selectCommand1);
            adapter1.SelectCommand.Parameters.Add("@estItemID", SqlDbType.Int).Value = estItemID;

            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                connectionSQL.Open();
                int numRowsAffected = adapter1.Fill(ds1, "t1");
                if (numRowsAffected != 0)
                {
                    row = ds1.Tables["t1"].Rows[0];
                }
            }
            catch (SqlException ex)
            {
                string errorLog = ex.Message;
            }

            finally
            {
                connectionSQL.Close();
            }

            return row;
        }
    }


    public class QuoteItemUpdate
    {
        private readonly int _myID;

        public QuoteItemUpdate(int myID)
        {
            _myID = myID;
        }

        public int EstItemID
        {
            set { UpdateEstItemID(value); }
        }


        private void UpdateEstItemID(int estItemID)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Quote_Item] SET [estItemID] = @estItemID WHERE [quoteItemID] = @quoteItemID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@quoteItemID", SqlDbType.Int).Value = _myID;
                UPdateCommand.Parameters.Add("@estItemID", SqlDbType.Int).Value = estItemID;
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

        public void UpdateQuoteItem()
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE Quote_Item SET qiPrintOrder = @qiPrintOrder, quoteOption = @quoteOption, supplyType = @supplyType, qiItemTitle = @qiItemTitle, qiDescription = @qiDescription, qiAmount = @qiAmount, isFinal = @isFinal, qiQty = @qiQty, qiQtypc = @qiQtypc WHERE (quoteItemID = @quoteItemID)";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                //qiPrintOrder = @qiPrintOrder, quoteOption = @quoteOption, supplyType = @supplyType, qiItemTitle = @qiItemTitle, qiDescription = @qiDescription, qiAmount = @qiAmount, isFinal = @isFinal, qiQty = @qiQty, qiQtypc = @qiQtypc WHERE (quoteItemID = @quoteItemID)

                UPdateCommand.Parameters.Add("@quoteItemID", SqlDbType.Int).Value = _myID;
                //               UPdateCommand.Parameters.Add("@estItemID", SqlDbType.Int).Value = estItemID
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


    public class QuoteItemDelete
    {
        private readonly int _myID;

        public QuoteItemDelete(int myID)
        {
            _myID = myID;
        }

        public void DeleteItem()
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string delString = "DELETE FROM  [Quote_Item] WHERE ([quoteItemID] = @quoteItemID)";
                var delCommand = new SqlCommand(delString, Connection);
                delCommand.Parameters.Add("@quoteItemID", SqlDbType.Int).Value = _myID;
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


    public class QuoteItemCopyToNewItem
    {
        // private string _recordType = "";
        private readonly int _myID;
        private readonly int _myParentID;
        //private int _newParentID = 0;
        //
        private int _newID;


        public QuoteItemCopyToNewItem(int myParentID, int myID)
        {
            _myParentID = myParentID;
            //_newParentID = newParentID;
            _myID = myID;
        }

        public int NewID
        {
            get { return _newID; }
        }


        public void Copy()
        {
            CopyRecord();
            //Update serialID
            var mqi = new MyQuoteItem(_myParentID, _newID);
            mqi.GenerateMySelfSerialID();
        }

        private void CopyRecord()
        {
            string errorLog = "Nothing";
            int NumRowsAffected = 0;

            //2. Define the destination: InvoiceItem
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 = "SELECT * FROM [QUOTE_Item] WHERE ([quoteItemID] = @quoteItemID) ";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@quoteItemID", SqlDbType.Int).Value = _myID;

            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                NumRowsAffected = adapter1.Fill(ds1, "t1");
                if (NumRowsAffected != 0)
                {
                    //Destination
                    //3. Copy
                    int itemIndex = ds1.Tables["t1"].Columns.Count;
                    DataRow row = ds1.Tables["t1"].Rows[0];
                    DataRow rowNew = ds1.Tables["t1"].NewRow();
                    int i = 0;

                    for (i = 1; i <= itemIndex - 1; i++)
                    {
                        rowNew[i] = row[i];
                    }

                    //qiPrintOrder =GetMaxItemPrintOrderNumber(QuoteRevID) + 1

                    rowNew["quoteRevID"] = _myParentID;
                    //newRevID
                    rowNew["qiPrintOrder"] = QuoteItemShared.GetMaxItemPrintOrderNumber(_myParentID);

                    rowNew["qiAmountText"] = rowNew["qiAmount"];
                    rowNew["quoteOptionText"] = rowNew["quoteOption"];

                    ds1.Tables["t1"].Rows.Add(rowNew);

                    //4. Write ds2,  back to DB

                    var cb = new SqlCommandBuilder(adapter1);
                    adapter1 = cb.DataAdapter;
                    NumRowsAffected = adapter1.Update(ds1, "t1");

                    _newID = SqlCommon.GetNewlyInsertedRecordID("Quote_Item");
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
    }


    //QuoteOption=GetMaxOption(QuoteRevID, qiPrintOrder)
    public class QuoteItemCopyToNewOption
    {
        // private string _recordType = "";
        private readonly int _myID;
        private readonly int _myParentID;
        //
        private int _newID;


        public QuoteItemCopyToNewOption(int myParentID, int myID)
        {
            _myParentID = myParentID;
            _myID = myID;
        }

        public int NewID
        {
            get { return _newID; }
        }


        public void Copy()
        {
            string errorLog = "Nothing";
            int NumRowsAffected = 0;

            //2. Define the destination: InvoiceItem
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 = "SELECT * FROM [QUOTE_Item] WHERE ([quoteItemID] = @quoteItemID) ";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@quoteItemID", SqlDbType.Int).Value = _myID;

            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                NumRowsAffected = adapter1.Fill(ds1, "t1");
                if (NumRowsAffected != 0)
                {
                    //Destination
                    //3. Copy
                    int itemIndex = ds1.Tables["t1"].Columns.Count;
                    DataRow row = ds1.Tables["t1"].Rows[0];
                    DataRow rowNew = ds1.Tables["t1"].NewRow();
                    int i = 0;

                    for (i = 1; i <= itemIndex - 1; i++)
                    {
                        rowNew[i] = row[i];
                    }

                    int pn = Convert.ToInt32(rowNew["qiPrintOrder"]);
                    rowNew["QuoteOption"] = QuoteItemShared.GetMaxOptionNumberOfOneItem(_myParentID, pn);
                    rowNew["isFinal"] = false;

                    rowNew["qiAmountText"] = rowNew["qiAmount"];
                    rowNew["quoteOptionText"] = rowNew["quoteOption"];

                    ds1.Tables["t1"].Rows.Add(rowNew);

                    //4. Write ds2,  back to DB

                    var cb = new SqlCommandBuilder(adapter1);
                    adapter1 = cb.DataAdapter;
                    NumRowsAffected = adapter1.Update(ds1, "t1");

                    _newID = SqlCommon.GetNewlyInsertedRecordID("Quote_Item");
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
    }


    public class QuoteItemShared
    {
        public static DataRow GetItemDataRow(int ItemID, string recordType)
        {
            //Input: quoteRevID and recordType

            DataRow row = null;

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString =
                "SELECT * FROM [Quote_Item] WHERE ([recordType]='Q' and [quoteItemID] = @quoteItemID)";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand);
            var ds1 = new DataSet();
            ds1.Tables.Clear();
            adapter1.SelectCommand.Parameters.Add("@quoteItemID", SqlDbType.Int).Value = ItemID;

            try
            {
                ConnectionSQL.Open();

                if (adapter1.Fill(ds1, "t1") > 0)
                {
                    row = ds1.Tables["t1"].Rows[0];
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

            return row;
        }


        public static int GetMaxItemPrintOrderNumber(int parentID)
        {
            //Input: quoteRevID, recordType, copyType

            int qr = 0;
            int rowsAffected = 0;

            //2. Define the destination: InvoiceItem

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 =
                "SELECT * FROM [QUOTE_Item] WHERE ([quoteRevID] = @quoteRevID and [recordType]=@recordType)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@quoteRevID", SqlDbType.Int).Value = parentID;
            adapter1.SelectCommand.Parameters.Add("@recordType", SqlDbType.Char).Value = "Q";

            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                rowsAffected = adapter1.Fill(ds1, "t1");

                //Destination
                if (rowsAffected > 0)
                {
                    DataRow row = null;

                    foreach (DataRow row_loopVariable in ds1.Tables["t1"].Rows)
                    {
                        row = row_loopVariable;
                        if (Convert.ToInt32(row["qiPrintOrder"]) > qr)
                        {
                            qr = Convert.ToInt32(row["qiPrintOrder"]);
                        }
                    }
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

            return qr + 1;
        }

        public static int GetMaxOptionNumberOfOneItem(int parentID, int printOrder)
        {
            //Input: quoteRevID, recordType, copyType

            int qr = 0;
            int rowsAffected = 0;

            //2. Define the destination: InvoiceItem

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 =
                "SELECT * FROM [QUOTE_Item] WHERE ([quoteRevID] = @quoteRevID and [recordType]=@recordType and [qiPrintOrder]=@qiPrintOrder)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@quoteRevID", SqlDbType.Int).Value = parentID;
            adapter1.SelectCommand.Parameters.Add("@recordType", SqlDbType.Char).Value = "Q";
            adapter1.SelectCommand.Parameters.Add("@qiPrintOrder", SqlDbType.Int).Value = printOrder;

            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                rowsAffected = adapter1.Fill(ds1, "t1");

                //Destination
                if (rowsAffected > 0)
                {
                    foreach (DataRow row in ds1.Tables["t1"].Rows)
                    {
                        if (Convert.ToInt32(row["quoteOption"]) > qr)
                        {
                            qr = Convert.ToInt32(row["quoteOption"]);
                        }
                    }
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

            return qr + 1;
        }
    }


    public class QuoteItemOption
    {
        private readonly int _qiPrintOrder;

        private readonly int _quoteItemID;
        private readonly int _quoteRevID;


        public QuoteItemOption(int quoteRevID, int qiPrintOrder, int quoteItemID)
        {
            _quoteRevID = quoteRevID;
            _qiPrintOrder = qiPrintOrder;
            _quoteItemID = quoteItemID;
        }


        public void ClearOtherOption()
        {
            string errorLog = "Nothing";
            int NumRowsAffected = 0;

            //2. Define the destination: InvoiceItem
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 =
                "SELECT * FROM [QUOTE_Item] WHERE ([quoteRevID] = @quoteRevID  and [qiPrintOrder]=@qiPrintOrder and [RecordType]='Q') ";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@quoteRevID", SqlDbType.Int).Value = _quoteRevID;
            adapter1.SelectCommand.Parameters.Add("@qiPrintOrder", SqlDbType.Int).Value = _qiPrintOrder;

            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                NumRowsAffected = adapter1.Fill(ds1, "t1");

                if (NumRowsAffected > 1)
                {
                    foreach (DataRow row in ds1.Tables["t1"].Rows)
                    {
                        if (Convert.ToInt32(row["quoteItemID"]) != _quoteItemID)
                        {
                            row["isFinal"] = false;
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
                errorLog = ex.Message;
            }
            finally
            {
                ConnectionSQL.Close();
            }
        }


        public void SyncTitle()
        {
            string errorLog = "Nothing";
            int NumRowsAffected = 0;

            //2. Define the destination: InvoiceItem
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 =
                "SELECT * FROM [QUOTE_Item] WHERE ([quoteRevID] = @quoteRevID  and [qiPrintOrder]=@qiPrintOrder  and [RecordType]='Q') ";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@quoteRevID", SqlDbType.Int).Value = _quoteRevID;
            adapter1.SelectCommand.Parameters.Add("@qiPrintOrder", SqlDbType.Int).Value = _qiPrintOrder;

            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                NumRowsAffected = adapter1.Fill(ds1, "t1");

                if (NumRowsAffected > 1)
                {
                    string title = "";
                    foreach (DataRow row in ds1.Tables["t1"].Rows)
                    {
                        if (Convert.ToInt32(row["quoteItemID"]) == _quoteItemID)
                        {
                            title = Convert.ToString(row["qiItemTitle"]);
                        }
                    }

                    foreach (DataRow row in ds1.Tables["t1"].Rows)
                    {
                        if (Convert.ToInt32(row["quoteItemID"]) != _quoteItemID)
                        {
                            row["qiItemTitle"] = title;
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
                errorLog = ex.Message;
            }
            finally
            {
                ConnectionSQL.Close();
            }
        }


        public void SyncPrintOrder(int printOrderOldValue)
        {
            //

            string errorLog = "Nothing";
            int NumRowsAffected = 0;

            //2. Define the destination: InvoiceItem
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 =
                "SELECT * FROM [QUOTE_Item] WHERE ([quoteRevID] = @quoteRevID  and [qiPrintOrder]=@qiPrintOrder  and [RecordType]='Q') ";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@quoteRevID", SqlDbType.Int).Value = _quoteRevID;
            //Check old value
            adapter1.SelectCommand.Parameters.Add("@qiPrintOrder", SqlDbType.Int).Value = printOrderOldValue;

            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                NumRowsAffected = adapter1.Fill(ds1, "t1");

                if (NumRowsAffected > 0)
                {
                    foreach (DataRow row in ds1.Tables["t1"].Rows)
                    {
                        row["qiPrintOrder"] = _qiPrintOrder;
                    }

                    //4. Write ds2,  back to DB
                    var cb = new SqlCommandBuilder(adapter1);
                    adapter1 = cb.DataAdapter;
                    NumRowsAffected = adapter1.Update(ds1, "t1");
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

        public virtual void UpdateContentFields(object p1, object p2, DataRow row)
        {
        }
    }

    //For Quote.Aspx
    public class QuoteItemSelect
    {
        private readonly DataRow _row;
        private readonly int _weiqItemID;

        public QuoteItemSelect(int weiqItemID)
        {
            _weiqItemID = weiqItemID;
            _row = GetItemDataRow();
        }


        public string QiSupplyType
        {
            get
            {
                string st = "";
                if (_row != null)
                {
                    if (!MyConvert.IsNullString(_row["qstName"]))
                    {
                        st = Convert.ToString(_row["qstName"]);
                    }
                }
                return st;
            }
        }


        private DataRow GetItemDataRow()
        {
            //Input: quoteRevID and recordType

            DataRow row = null;

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString =
                "SELECT Quote_Item.quoteItemID, FW_tblcQuoteSupplyType.qstName FROM Quote_Item INNER JOIN FW_tblcQuoteSupplyType ON Quote_Item.supplyType = FW_tblcQuoteSupplyType.qstID WHERE (Quote_Item.recordType = 'Q') AND (Quote_Item.quoteItemID = @quoteItemID)";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand);
            var ds1 = new DataSet();
            ds1.Tables.Clear();
            adapter1.SelectCommand.Parameters.Add("@quoteItemID", SqlDbType.Int).Value = _weiqItemID;

            try
            {
                ConnectionSQL.Open();

                if (adapter1.Fill(ds1, "t1") > 0)
                {
                    row = ds1.Tables["t1"].Rows[0];
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

            return row;
        }
    }
}