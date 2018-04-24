using System;
using System.Data;
using System.Data.SqlClient;
using MyCommon;
using MyCommon.MyEnum;
using MyCommonWeb;
using SalesCenterDomain.BDL.Quote;
using SalesCenterDomain.BLL;
using SalesCenterDomain.BO;
using SpecDomain.BLL;
using SpecDomain.BLL.EstItem;

namespace SalesCenterDomain.BDL.Invoice
{
    public class InvoiceItemBlank
    {
        private readonly int _myParentID;

        public InvoiceItemBlank(int myParentID)
        {
            _myParentID = myParentID;
        }

        public void CreateNew()
        {
            InsertRecord();
            int myNewID = SqlCommon.GetNewlyInsertedRecordID("Invoice_Item");
            UpdateSerialID(myNewID);
        }

        private void InsertRecord()
        {
            using (var connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                const string insertString =
                    "INSERT INTO [Invoice_Item] ([estItemID], [quoteRevID], [qiPrintOrder], [quoteOption], [supplyType], [qiItemTitle], [qiDescription], [qiAmount], [qiUnitPrice], [qiQty], [recordType], [qiQtyPc],[Requirement],[Position], [quoteOptionText], [qiAmountText], [IsFinal], [NameDetailsID]) VALUES (@estItemID, @quoteRevID, @qiPrintOrder, @quoteOption, @supplyType, @qiItemTitle, @qiDescription, @qiAmount, @qiUnitPrice, @qiQty, @recordType,@qiQtyPc, @Requirement, @Position, @quoteOptionText, @qiAmountText, @IsFinal, @NameDetailsID)";

                // Create the command and set its properties.
                var insertCommand = new SqlCommand(insertString, connection);
                try
                {
                    connection.Open();

                    //[Quote_Item] ([estItemID], [quoteRevID], [qiPrintOrder], [quoteOption], [supplyType], [qiItemTitle], [qiDescription], [qiAmount], [qiUnitPrice], [qiQty], [recordType]) 
                    //Values        (@estItemID, @quoteRevID,  @qiPrintOrder,  @quoteOption,  @supplyType,  @qiItemTitle,  @qiDescription,  @qiAmount,  @qiUnitPrice,  @qiQty,  @recordType)"

                    insertCommand.Parameters.Add("@estItemID", SqlDbType.Int).Value = 0;
                    insertCommand.Parameters.Add("@quoteRevID", SqlDbType.Int).Value = _myParentID;
                    insertCommand.Parameters.Add("@qiPrintOrder", SqlDbType.SmallInt).Value =
                        GetMaxItemPrintOrderNumber();
                    insertCommand.Parameters.Add("@quoteOption", SqlDbType.SmallInt).Value = 1;
                    insertCommand.Parameters.Add("@supplyType", SqlDbType.SmallInt).Value =
                        NQuoteSupplyType.SupplyAndInstallation;
                    insertCommand.Parameters.Add("@qiItemTitle", SqlDbType.NVarChar, 500).Value = "My Title";
                    insertCommand.Parameters.Add("@qiDescription", SqlDbType.NVarChar, 1500).Value = "My Description";
                    insertCommand.Parameters.Add("@qiAmount", SqlDbType.Decimal, 8).Value = 0;
                    insertCommand.Parameters.Add("@qiUnitPrice", SqlDbType.Decimal, 8).Value = 0;
                    insertCommand.Parameters.Add("@QiQty", SqlDbType.SmallInt).Value = 1;
                    insertCommand.Parameters.Add("@recordType", SqlDbType.Char).Value = "I";

                    insertCommand.Parameters.Add("@qiQtyPc", SqlDbType.Int).Value = 0;
                    insertCommand.Parameters.Add("@Requirement", SqlDbType.Int).Value =
                        NWorkorderRequirement.Installation;
                    insertCommand.Parameters.Add("@Position", SqlDbType.NVarChar, 7).Value = "Outdoor";

                    insertCommand.Parameters.Add("@quoteOptionText", SqlDbType.NVarChar, 300).Value = 1;
                    insertCommand.Parameters.Add("@qiAmountText", SqlDbType.NVarChar, 300).Value = 0;
                    insertCommand.Parameters.Add("@IsFinal", SqlDbType.Bit).Value = true;
                    insertCommand.Parameters.Add("@NameDetailsID", SqlDbType.Int).Value = 0;

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

        private int GetMaxItemPrintOrderNumber()
        {
            //Input: quoteRevID, recordType, copyType

            int qr = 0;
            int rowsAffected = 0;

            //2. Define the destination: InvoiceItem

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 =
                "SELECT * FROM [Invoice_Item] WHERE ([quoteRevID] = @quoteRevID and [recordType]=@recordType)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@quoteRevID", SqlDbType.Int).Value = _myParentID;
            adapter1.SelectCommand.Parameters.Add("@recordType", SqlDbType.Char).Value = "I";

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


        private void UpdateSerialID(int myNewID)
        {
            var mii = new MyInvoiceItem(_myParentID, myNewID);
            mii.GenerateMySelfSerialID();
        }
    }


    public class InvoiceItemsGenerateFromEstimation
    {
        private readonly int _estRevID;
        private readonly int _invoiceID;

        public InvoiceItemsGenerateFromEstimation(int invoiceID, int estRevID)
        {
            _invoiceID = invoiceID;
            _estRevID = estRevID;
        }

        public void Generate()
        {
            DataTable tbl = EstItemCommon.GetEstimationItems(_estRevID);
            if (tbl != null)
            {
                //
                //2. Define the destination: InvoiceItem
                var connectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);

                const string sqlSelectString2 = "SELECT *  FROM [Invoice_Item] WHERE ([quoteItemID] = 0) ";
                var selectCommand2 = new SqlCommand(sqlSelectString2, connectionSQL);
                var adapter2 = new SqlDataAdapter(selectCommand2);

                var ds2 = new DataSet();
                ds2.Tables.Clear();

                try
                {
                    connectionSQL.Open();
                    //define row
                    adapter2.Fill(ds2, "t2");

                    foreach (DataRow row in tbl.Rows)
                    {
                        DataRow rowNew = ds2.Tables["t2"].NewRow();

                        rowNew["quoteRevID"] = _invoiceID;

                        int fwJobTypeRequriedmentID = Convert.ToInt32(row["RequirementID"]);
                        var sp = new SupplyType(fwJobTypeRequriedmentID);
                        rowNew["supplyType"] = sp.QuoteSupplyTypeID;

                        rowNew["qiPrintOrder"] = row["estItemNo"];
                        rowNew["quoteOption"] = row["ItemOption"];
                        rowNew["isFinal"] = row["isFinalOption"];
                        rowNew["estItemID"] = row["estItemID"];
                        rowNew["qiItemTitle"] = row["ProductName"];


                        string copyDetails = SalesCenterConfiguration.CopyDetailsToInvoice;
                        if (copyDetails.Trim().ToLower() == "yes")
                        {
                            rowNew["qiDescription"] = row["Description"];
                        }
                        else
                        {
                            rowNew["qiDescription"] = DBNull.Value;
                        }

                        rowNew["recordType"] = "I";

                        rowNew["qiQty"] = row["Qty"];
                        rowNew["qiQtyPC"] = row["QtyPc"];


                        double qiAmount = MyConvert.ConvertToDouble(row["estimatorPrice"]);
                        rowNew["qiAmount"] = qiAmount;
                        rowNew["qiUnitPrice"] = 0;
                        //to be defined in estimation

                        rowNew["estItemID"] = row["estItemID"];

                        rowNew["Requirement"] = row["RequirementID"];
                        //rowNew["Position"] = row["Position"];
                        rowNew["Position"] = "Indoor";

                        int s = MyConvert.ConvertToInteger(row["SerialID"]);
                        rowNew["SerialID"] = "E" + string.Format("{0:D2}", s);
                        rowNew["nameDetailsID"] = row["ProductID"];
                        ds2.Tables["t2"].Rows.Add(rowNew);
                    }

                    //4. Write ds2,  back to DB

                    var cb = new SqlCommandBuilder(adapter2);
                    adapter2 = cb.DataAdapter;


                    adapter2.Update(ds2, "t2");
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
        }
    }


    public class SupplyType
    {
        // Conver requirement of internal use to Quotation Supply Type
        //    ''' Get Datarow from tbl FW_JOB_TYPE 
        //    ''' Read column QUOTE_SUPPLY_TYPE

        public SupplyType(int fwJobTypeRequirementID)
        {
            int st = 10;
            DataRow row = GetFwJobTypeRow(fwJobTypeRequirementID);
            if (row != null)
            {
                st = Convert.ToInt32(row["QUOTE_SUPPLY_TYPE"]);
            }

            QuoteSupplyTypeID = st;
        }

        public int QuoteSupplyTypeID { get; private set; }


        private DataRow GetFwJobTypeRow(int requirement)
        {
            DataRow row = null;

            //2. Define the destination: InvoiceItem
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 = "SELECT * FROM [FW_JOB_TYPE] WHERE ([TYPE_ID] = @TYPE_ID) ";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@TYPE_ID", SqlDbType.Int).Value = requirement;

            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                int NumRowsAffected = adapter1.Fill(ds1, "t1");
                if (NumRowsAffected != 0)
                {
                    row = ds1.Tables["t1"].Rows[0];
                }
            }
            catch (SqlException ex)
            {
                string errlog = ex.Message;
            }
            finally
            {
                ConnectionSQL.Close();
            }

            return row;
        }
    }


    public class InvoiceItemsGenerateFromQuote
    {
        private readonly int _invoiceID;
        private readonly int _quoteRevID;

        public InvoiceItemsGenerateFromQuote(int invoiceID, int quoteRevID)
        {
            _invoiceID = invoiceID;
            _quoteRevID = quoteRevID;
        }


        public void Generate()
        {
            var qc = new QuoteChildren(_quoteRevID);
            DataTable tbl = qc.ItemList("Q", true);

            if (tbl == null) return;

            //
            //2. Define the destination: InvoiceItem
            var connectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);

            const string sqlSelectString2 = "SELECT *  FROM [Invoice_Item] WHERE ([quoteItemID] = 0) ";
            var selectCommand2 = new SqlCommand(sqlSelectString2, connectionSQL);
            var adapter2 = new SqlDataAdapter(selectCommand2);

            var ds2 = new DataSet();
            ds2.Tables.Clear();

            try
            {
                connectionSQL.Open();
                adapter2.Fill(ds2, "t2");

                foreach (DataRow row in tbl.Rows)
                {
                    DataRow rowNew = ds2.Tables["t2"].NewRow();
                    int itemIndex = ds2.Tables["t2"].Columns.Count;


                    int i = 0;
                    for (i = 1; i <= itemIndex - 1; i++)
                    {
                        rowNew[i] = row[i];
                    }

                    rowNew["qiItemTitle"] = VbHtml.MyHtmlDecode( rowNew["qiItemTitle"]);


                    string copyDetails = SalesCenterConfiguration.CopyDetailsToInvoice;
                    if (copyDetails.Trim().ToLower() == "yes")
                    {
                        object s1 = rowNew["qiDescription"];
                        rowNew["qiDescription"] = VbHtml.MyHtmlDecode( rowNew["qiDescription"]);
                        object s2 = rowNew["qiDescription"];
                        object s3 = s2;
                    }
                    else
                    {
                        rowNew["qiDescription"] = DBNull.Value;
                    }

                    rowNew["quoteRevID"] = _invoiceID;
                    rowNew["recordType"] = "I";

                    ds2.Tables["t2"].Rows.Add(rowNew);
                }

                //4. Write ds2,  back to DB

                var cb = new SqlCommandBuilder(adapter2);
                adapter2 = cb.DataAdapter;


                adapter2.Update(ds2, "t2");
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
    }
}