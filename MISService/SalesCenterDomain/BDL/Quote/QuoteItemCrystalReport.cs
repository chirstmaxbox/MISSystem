using System;
using System.Data;
using System.Data.SqlClient;
using MyCommon;
using MyCommonWeb;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Quote
{
    public class QuoteItemCrystalReport
    {
        //define Dataset
        //Add items:
        //   Qutoe
        //   Service
        //   Note
        // ?? should use dataset to Crystal Report
        //Write back to database

        //?PrintOption


        private const int PRINT_ORDER_NOTE = 200;
        private readonly int _eid;

        private readonly int _quoteRevID;

        public QuoteItemCrystalReport(int employeeID, int quoteRevID)
        {
            _eid = employeeID;
            _quoteRevID = quoteRevID;

            DeleteItemDetails();
            ConfigureQuoteItems();

            DeleteItemDetails_Service();
            ConfigureQuoteItems_Service();
        }

        //(ByVal eID As Integer)
        private void DeleteItemDetails_Service()
        {
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString = "SELECT * FROM CR_QuoteItem_Service WHERE (eID = @eID)";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter2 = new SqlDataAdapter(SelectCommand);
            var ds2 = new DataSet();
            adapter2.SelectCommand.Parameters.Add("@eID", SqlDbType.Int).Value = _eid;
            try
            {
                ConnectionSQL.Open();
                int NumRowsAffected = adapter2.Fill(ds2, "t2");
                if (NumRowsAffected > 0)
                {
                    //1.2 ? ds2.Tables("t2").Clear()
                    DataRow row = null;
                    foreach (DataRow row_loopVariable in ds2.Tables["t2"].Rows)
                    {
                        row = row_loopVariable;
                        row.Delete();
                    }

                    //1.3. Write   back to DB
                    var cb = new SqlCommandBuilder(adapter2);
                    adapter2 = cb.DataAdapter;
                    NumRowsAffected = adapter2.Update(ds2, "t2");
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

        //(ByVal eID As Integer)
        private void DeleteItemDetails()
        {
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString = "SELECT * FROM CR_QuoteItem WHERE (eID = @eID)";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter2 = new SqlDataAdapter(SelectCommand);
            var ds2 = new DataSet();
            adapter2.SelectCommand.Parameters.Add("@eID", SqlDbType.Int).Value = _eid;
            try
            {
                ConnectionSQL.Open();
                int NumRowsAffected = adapter2.Fill(ds2, "t2");
                if (NumRowsAffected > 0)
                {
                    //1.2 ? ds2.Tables("t2").Clear()
                    DataRow row = null;
                    foreach (DataRow row_loopVariable in ds2.Tables["t2"].Rows)
                    {
                        row = row_loopVariable;
                        row.Delete();
                    }

                    //1.3. Write   back to DB
                    var cb = new SqlCommandBuilder(adapter2);
                    adapter2 = cb.DataAdapter;
                    NumRowsAffected = adapter2.Update(ds2, "t2");
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


        private void ConfigureQuoteItems()
        {
            //define dataset
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString = "SELECT * FROM [CR_QuoteItem] WHERE ([qiID] = 0)";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter2 = new SqlDataAdapter(SelectCommand);
            var ds2 = new DataSet();
            ds2.Tables.Clear();
            try
            {
                ConnectionSQL.Open();
                int rowsAffected = adapter2.Fill(ds2, "t2");

                //format the items
                DataTable t2 = ds2.Tables["t2"];

                var qp = new QuoteItemReportAdd_Product(t2, _quoteRevID);
                t2 = qp.ItemTable;

                //t2 = GetFormatedQuoteNotes(t2)

                t2 = AddEiDtoQuoteItemTable(t2);

                t2 = FormatHtmlStringForCrystalReport(t2);


                //. Write  back to DB
                var cb = new SqlCommandBuilder(adapter2);
                adapter2 = cb.DataAdapter;
                int s1 = adapter2.Update(ds2, "t2");
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

        private void ConfigureQuoteItems_Service()
        {
            //define dataset
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString = "SELECT * FROM [CR_QuoteItem_Service] WHERE ([qiID] = 0)";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter2 = new SqlDataAdapter(SelectCommand);
            var ds2 = new DataSet();
            ds2.Tables.Clear();
            try
            {
                ConnectionSQL.Open();
                int rowsAffected = adapter2.Fill(ds2, "t2");

                //format the items
                DataTable t2 = ds2.Tables["t2"];

                var qs = new QuoteItemReportAdd_Service(t2, _quoteRevID);
                t2 = qs.ItemTable;

                //t2 = GetFormatedQuoteNotes(t2)

                t2 = AddEiDtoQuoteItemTable(t2);
                t2 = FormatHtmlStringForCrystalReport(t2);

                //. Write  back to DB
                var cb = new SqlCommandBuilder(adapter2);
                adapter2 = cb.DataAdapter;
                int s1 = adapter2.Update(ds2, "t2");
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


        private DataTable AddEiDtoQuoteItemTable(DataTable tbl)
        {
            foreach (DataRow row in tbl.Rows)
            {
                row["eID"] = _eid;
            }
            return tbl;
        }

        private DataTable FormatHtmlStringForCrystalReport(DataTable tbl)
        {
            foreach (DataRow rowNew in tbl.Rows)
            {
                rowNew["Title"] = GetCrystalReportHtmlFormat(rowNew["Title"]);
                rowNew["QuoteOption"] = GetCrystalReportHtmlFormat(rowNew["QuoteOption"]);
                rowNew["Description"] = GetCrystalReportHtmlFormat(rowNew["Description"]);
                rowNew["HtmlAmount"] = GetCrystalReportHtmlFormat(rowNew["HtmlAmount"]);
                rowNew["HtmlAmountOption"] = GetCrystalReportHtmlFormat(rowNew["HtmlAmountOption"]);
            }

            return tbl;
        }

        private string GetCrystalReportHtmlFormat(object htmlString)
        {
            //Remove the space after ":"

            string s1 = "font-style: ";
            string s2 = "font-family: ";
            string s3 = "font-weight: ";
            string s4 = "<br />";

            string s1R = "font-style:";
            string s2R = "font-family:";
            string s3R = "font-weight:";
            string s4R = " <br> ";

            string s = Convert.ToString(htmlString);
            if (s.Contains(s1))
            {
                s = s.Replace(s1, s1R);
            }

            if (s.Contains(s2))
            {
                s = s.Replace(s2, s2R);
            }

            if (s.Contains(s3))
            {
                s = s.Replace(s3, s3R);
            }

            if (s.Contains(s4))
            {
                s = s.Replace(s4, s4R);
            }
            
            return s;
        }
    }


    public class QuoteItemReportAdd_Product
    {
        private readonly DataTable _tbl;

        public QuoteItemReportAdd_Product(DataTable tbl, int quoteRevID)
        {
            try
            {
                DataTable tbl2 = GetItemsList(quoteRevID);

                DataTable tbl3 = GetItemsGroupByPrintOrder(quoteRevID);


                if (tbl2 != null & tbl3 != null)
                {
                    //row Data 
                    foreach (DataRow row in tbl2.Rows)
                    {
                        int printOrder = Convert.ToInt32(row["qiPrintOrder"]);
                        int optionCount = GetOptionCount(printOrder, tbl3);

                        DataRow rowNew = tbl.NewRow();
                        //row New

                        rowNew["PrintOrder"] = AddPrintOrderBaseNumber(printOrder);
                        row["qiAmountText"] = ConvertToF2Format(row["qiAmountText"]);

                        QuoteItemPrintFormat qiTO = GetQuoteItemPrintFormat(row, optionCount);

                        rowNew["Title"] = qiTO.qiTitle;
                        if (!Convert.IsDBNull(row["SerialID"]))
                        {
                            string str = Convert.ToString(row["SerialID"]).Trim();
                            if (str.Length == 3)
                            {
                                rowNew["Title"] = qiTO.qiTitle + " (ID# " + str + ")";
                            }
                            else
                            {
                                var id1 = MyConvert.ConvertToInteger(row["SerialID"]);
                                if (id1>0)
                                {
                                    rowNew["Title"] = qiTO.qiTitle + " (ID# E" + id1.ToString("D2" ) + ")";
                                }
                            }
                        }
                        rowNew["QuoteOption"] = qiTO.qiOption;
                        rowNew["HtmlAmount"] = qiTO.HtmlTitleAmount;
                        rowNew["HtmlAmountOption"] = qiTO.HtmlOptionAmount;

                        rowNew["Description"] = qiTO.Description;
                        if (!MyConvert.IsNullString(row["qiAmount"]))
                        {
                            string sq1 = MyConvert.ConvertToString(row["qiAmount"]);
                            if (MyConvert.IsAccountingFormatNumberic(sq1))
                            {
                                rowNew["Amount"] = MyConvert.ConvertAccountingFormatStringToDouble(sq1);
                            }
                        }

                        tbl.Rows.Add(rowNew);
                    }
                }
            }
            catch (SqlException ex)
            {
                string errorLog = ex.Message;
            }
            finally
            {
            }

            _tbl = tbl;
        }

        public DataTable ItemTable
        {
            get { return _tbl; }
        }

        public QuoteItemPrintFormat GetQuoteItemPrintFormat(DataRow row, int optionCount)
        {
            QuoteItemPrintFormat qiTO = null;
            if (optionCount == 1)
            {
                qiTO = new QuoteItem_OneOption_Product(row);
            }
            else
            {
                qiTO = new QuoteItem_MoreThanOneOptions_Product(row);
            }
            return qiTO;
        }


        public int AddPrintOrderBaseNumber(int printOrder)
        {
            const int PRINT_ORDER_BASE_NUMBER = 0;
            return printOrder + PRINT_ORDER_BASE_NUMBER;
        }

        public DataTable GetItemsList(int quoteRevID)
        {
            //Input: parentItem, such as quoteRevID, 
            //      recordType=Q

            DataTable tbl = null;

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString =
                "SELECT * FROM [Quote_Item] WHERE ([quoteRevID] = @quoteRevID and [recordType]='Q') ORDER BY [qiPrintOrder], [quoteOption]";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand);
            adapter1.SelectCommand.Parameters.Add("@quoteRevID", SqlDbType.Int).Value = quoteRevID;

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

        public DataTable GetItemsGroupByPrintOrder(int quoteRevID)
        {
            //Input: parentItem, such as quoteRevID, 
            //      qiPrintOrder, OptionCount

            DataTable tbl = null;

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);

            string SqlSelectString =
                "SELECT qiPrintOrder AS PrintOrder, COUNT(quoteItemID) AS OptionCount FROM Quote_Item WHERE (quoteRevID = @QuoteRevID) GROUP BY qiPrintOrder ORDER BY PrintOrder";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);

            var adapter1 = new SqlDataAdapter(SelectCommand);

            adapter1.SelectCommand.Parameters.Add("@quoteRevID", SqlDbType.Int).Value = quoteRevID;

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

        private int GetOptionCount(int printOrder, DataTable tblServiceGroup)
        {
            int optionCount = 1;
            //row Group
            foreach (DataRow rowG in tblServiceGroup.Rows)
            {
                if (printOrder == Convert.ToInt32(rowG["PrintOrder"]))
                {
                    optionCount = Convert.ToInt32(rowG["OptionCount"]);
                    break;
                }
            }
            return optionCount;
        }


        private string ConvertToF2Format(object htmlFormatString)
        {
            string s1 = "";

            if (!MyConvert.IsNullString(htmlFormatString))
            {
                s1 = VbHtml.MyHtmlDecode( htmlFormatString);

                if (MyConvert.IsNumeric(s1))
                {
                    if (!s1.Contains("$"))
                    {
                        string s2 = "$" + s1;
                        s1 = Convert.ToString(htmlFormatString);
                        s1 = s1.Replace(s1, s2);
                    }
                }
            }
            return s1;
        }
    }


    public class QuoteItemReportAdd_Service
    {
        private readonly DataTable _tbl;

        public QuoteItemReportAdd_Service(DataTable tbl, int quoteRevID)
        {
            try
            {
                DataTable tbl2 = GetItemsList(quoteRevID);

                DataTable tbl3 = GetItemsGroupByPrintOrder(quoteRevID);


                if (tbl2 != null & tbl3 != null)
                {
                    //row Data 
                    foreach (DataRow row in tbl2.Rows)
                    {
                        int printOrder = Convert.ToInt32(row["qiPrintOrder"]);
                        int optionCount = GetOptionCount(printOrder, tbl3);

                        DataRow rowNew = tbl.NewRow();
                        //row New

                        rowNew["PrintOrder"] = AddPrintOrderBaseNumber(printOrder);
                        row["qiAmountText"] = ConvertToF2Format(row["qiAmountText"]);

                        QuoteItemPrintFormat qiTO = GetQuoteItemPrintFormat(row, optionCount);

                        rowNew["Title"] = qiTO.qiTitle;

                        rowNew["QuoteOption"] = qiTO.qiOption;
                        rowNew["HtmlAmount"] = qiTO.HtmlTitleAmount;
                        rowNew["HtmlAmountOption"] = qiTO.HtmlOptionAmount;

                        rowNew["Description"] = qiTO.Description;
                        if (!MyConvert.IsNullString(row["qiAmount"]))
                        {
                            string sq1 = MyConvert.ConvertToString(row["qiAmount"]);
                            if (MyConvert.IsAccountingFormatNumberic(sq1))
                            {
                                rowNew["Amount"] = MyConvert.ConvertAccountingFormatStringToDouble(sq1);
                            }
                        }

                        tbl.Rows.Add(rowNew);
                    }
                }
            }
            catch (SqlException ex)
            {
                string errorLog = ex.Message;
            }
            finally
            {
            }

            _tbl = tbl;
        }

        public DataTable ItemTable
        {
            get { return _tbl; }
        }


        public int AddPrintOrderBaseNumber(int printOrder)
        {
            const int PRINT_ORDER_BASE_NUMBER = 100;
            return printOrder + PRINT_ORDER_BASE_NUMBER;
        }

        private int GetOptionCount(int printOrder, DataTable tblServiceGroup)
        {
            int optionCount = 1;
            //row Group
            foreach (DataRow rowG in tblServiceGroup.Rows)
            {
                if (printOrder == Convert.ToInt32(rowG["PrintOrder"]))
                {
                    optionCount = Convert.ToInt32(rowG["OptionCount"]);
                    break; // TODO: might not be correct. Was : Exit For
                }
            }
            return optionCount;
        }


        private string ConvertToF2Format(object htmlFormatString)
        {
            string s1 = "";

            if (!MyConvert.IsNullString(htmlFormatString))
            {
                s1 = VbHtml.MyHtmlDecode( htmlFormatString);

                if (MyConvert.IsNumeric(s1))
                {
                    if (!s1.Contains("$"))
                    {
                        string s2 = "$" + s1;
                        s1 = Convert.ToString(htmlFormatString);
                        s1 = s1.Replace(s1, s2);
                    }
                }
            }
            return s1;
        }


        public DataTable GetItemsList(int quoteRevID)
        {
            DataTable tbl = null;
            string errorLog = "Nothing";
            int NumRowsAffected = 0;

            //2. Define the destination: InvoiceItem
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 =
                "SELECT estRevID, qsPrintOrder AS qiPrintOrder, qsTitle AS qiItemTitle, qsDescription AS qiDescription, qsAmount AS qiAmount, qsAmountText AS qiAmountText, recordType FROM dbo.FW_QUOTE_SERVICE WHERE (recordType = N'Quote') and (estRevID=@quoteRevID) ORDER BY qsPrintOrder,qsDescription ";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@quoteRevID", SqlDbType.Int).Value = quoteRevID;
            adapter1.SelectCommand.Parameters.Add("@recordType", SqlDbType.NChar, 7).Value = "Quote";
            var ds1 = new DataSet();

            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                NumRowsAffected = adapter1.Fill(ds1, "t1");
                if (NumRowsAffected > 0)
                {
                    //Destination
                    tbl = ds1.Tables["t1"];
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

            return tbl;
        }

        public DataTable GetItemsGroupByPrintOrder(int quoteRevID)
        {
            DataTable tbl = null;
            string errorLog = "Nothing";
            int NumRowsAffected = 0;

            //2. Define the destination: InvoiceItem
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 =
                "SELECT qsPrintOrder as PrintOrder, COUNT(qsID) AS OptionCount FROM FW_QUOTE_SERVICE WHERE (estRevID = @quoteRevID) AND (recordType = N'Quote') GROUP BY qsPrintOrder";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@quoteRevID", SqlDbType.Int).Value = quoteRevID;
            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                NumRowsAffected = adapter1.Fill(ds1, "t1");
                if (NumRowsAffected > 0)
                {
                    //Destination
                    tbl = ds1.Tables["t1"];
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

            return tbl;
        }

        public QuoteItemPrintFormat GetQuoteItemPrintFormat(DataRow row, int optionCount)
        {
            QuoteItemPrintFormat qiTO = null;
            if (optionCount == 1)
            {
                qiTO = new QuoteItem_OneOption_Service(row);
            }
            else
            {
                qiTO = new QuoteItem_MoreThanOneOptions_Service(row);
            }
            return qiTO;
        }
    }

    #region "Product Item : Title, Option, Amount"

    public abstract class QuoteItemPrintFormat
    {
        private readonly string _description = "";
        private readonly string _htmlOptionAmount = "";
        private readonly string _htmlTitleAmount = "";
        private readonly string _qiOption = "";
        private readonly string _qiTitle = "";

        public QuoteItemPrintFormat(DataRow row)
        {
            string requirement = GetRequirement(row);
            _qiTitle = GetQiTitle(requirement, row);
            _qiOption = GetQiOption(requirement, row);
            _htmlTitleAmount = GetQiTitleAmount(row);
            _htmlOptionAmount = GetQiOptionAmount(row);
            _description = GetQiDescription(row);
        }

        public string qiTitle
        {
            get { return _qiTitle; }
        }

        public string qiOption
        {
            get { return _qiOption; }
        }

        public string HtmlTitleAmount
        {
            get { return _htmlTitleAmount; }
        }

        public string HtmlOptionAmount
        {
            get { return _htmlOptionAmount; }
        }

        public string Description
        {
            get { return _description; }
        }

        public abstract string GetQiTitle(string requirement, DataRow row);
        public abstract string GetQiOption(string requirement, DataRow row);
        public abstract string GetQiDescription(DataRow row);

        public abstract string GetQiTitleAmount(DataRow row);
        public abstract string GetQiOptionAmount(DataRow row);

        public virtual string GetRequirement(DataRow row)
        {
            int qiID = Convert.ToInt32(row["quoteItemID"]);
            var qiSelect = new QuoteItemSelect(qiID);
            string st = qiSelect.QiSupplyType;
            return st;
        }
    }


    public class QuoteItem_OneOption_Product : QuoteItemPrintFormat
    {
        public QuoteItem_OneOption_Product(DataRow row)
            : base(row)
        {
        }

        public override string GetQiTitle(string requirement, DataRow row)
        {
            //Level 1: Title (PrintOrder +Title --WithFormat + rquirement); Amout
            //Level 2:       Description
            string newTitle = MyConvert.ConvertToString(row["qiItemTitle"]) + ", " + requirement;
            return newTitle;
        }

        public override string GetQiOption(string requirement, DataRow row)
        {
            string newOption = "";
            if (!MyConvert.IsNullString(row["qiDescription"]))
            {
                newOption = Convert.ToString(row["qiDescription"]);
            }
            return newOption;
        }

        public override string GetQiDescription(DataRow row)
        {
            string s = "";
            return s;
        }


        public override string GetQiTitleAmount(DataRow row)
        {
            string Titleamount = Convert.ToString(row["qiAmountText"]);
            return Titleamount;
        }

        public override string GetQiOptionAmount(DataRow row)
        {
            return "";
        }
    }

    public class QuoteItem_MoreThanOneOptions_Product : QuoteItemPrintFormat
    {
        public QuoteItem_MoreThanOneOptions_Product(DataRow row)
            : base(row)
        {
        }

        public override string GetQiTitle(string requirement, DataRow row)
        {
            //Level 1: Title (PrintOrder +Title --WithFormat + rquirement); Amout
            //Level 2:       Description
            string newTitle = MyConvert.ConvertToString(row["qiItemTitle"]);

            return newTitle;
        }

        public override string GetQiOption(string requirement, DataRow row)
        {
            //>1,only the first option title is usefull
            //Level 1: Item ( PrintOrder +Title )
            //Level 2:   Option +requirement ; Amount
            //Level 3:   Description

            //Dim OptionOriginal As String = row("QuoteOptionText")
            //Dim optionValue As String = RemoveUnValidChars(OptionOriginal)
            //Dim newOption As String = ">Option " + optionValue + ", " + requirement + "<"
            //optionValue = ">" + optionValue + "<"

            //If OptionOriginal.Length >= 10 Then
            //    newOption = OptionOriginal.Replace(optionValue, newOption)
            //End If

            string OptionOriginal = Convert.ToString(row["QuoteOptionText"]);
            string optionValue = VbHtml.MyHtmlDecode( OptionOriginal);
            string newOption = "Option " + optionValue + ", " + requirement;
            if (OptionOriginal.Length > 10)
            {
                newOption = ">" + newOption + "<";
                optionValue = ">" + optionValue + "<";
                newOption = OptionOriginal.Replace(optionValue, newOption);
            }

            return newOption;
        }

        public override string GetQiTitleAmount(DataRow row)
        {
            return "";
        }

        public override string GetQiOptionAmount(DataRow row)
        {
            string Titleamount = Convert.ToString(row["qiAmountText"]);

            return Titleamount;
        }

        public override string GetQiDescription(DataRow row)
        {
            string newOption = "";
            if (!MyConvert.IsNullString(row["qiDescription"]))
            {
                newOption = Convert.ToString(row["qiDescription"]);
            }
            return newOption;
        }
    }


    public class QuoteItem_OneOption_Service : QuoteItem_OneOption_Product
    {
        public QuoteItem_OneOption_Service(DataRow row)
            : base(row)
        {
        }

        public override string GetQiTitle(string requirement, DataRow row)
        {
            //Level 1: Title (PrintOrder +Title --WithFormat + rquirement); Amout
            //Level 2:       Description
            string newTitle = MyConvert.ConvertToString(row["qiItemTitle"]);
            return newTitle;
        }

        public override string GetRequirement(DataRow row)
        {
            return "";
        }
    }


    public class QuoteItem_MoreThanOneOptions_Service : QuoteItem_MoreThanOneOptions_Product
    {
        public QuoteItem_MoreThanOneOptions_Service(DataRow row)
            : base(row)
        {
        }

        public override string GetQiTitle(string requirement, DataRow row)
        {
            //Level 1: Title (PrintOrder +Title --WithFormat + rquirement); Amout
            //Level 2:       Description
            string newTitle = MyConvert.ConvertToString(row["qiItemTitle"]);
            return newTitle;
        }

        public override string GetQiOption(string requirement, DataRow row)
        {
            string newOption = "";
            if (!MyConvert.IsNullString(row["qiDescription"]))
            {
                newOption = Convert.ToString(row["qiDescription"]);
            }
            return newOption;
        }

        //Public Overrides Function GetQiTitleAmount(ByVal row As System.Data.DataRow) As String
        //    Return ""
        //End Function

        //Public Overrides Function GetQiOptionAmount(ByVal row As System.Data.DataRow) As String
        //    Dim Titleamount As String = row("qiAmountText")
        //    Return Titleamount

        //End Function

        //Public Overrides Function GetQiDescription(ByVal row As DataRow) As String
        //    Dim s As String = ""
        //    Return s
        //End Function
        public override string GetRequirement(DataRow row)
        {
            return "";
        }
    }

    #endregion
}