using System;
using System.Data;
using System.Data.SqlClient;
using MyCommon;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Quote
{
    public abstract class QuoteItemEdit
    {
        #region "Properties"

        private string _QuoteOptionText = "1";
        private string _qiAmountText = "";
        private string _qiDescription = "";
        private string _qiItemTitle = "";
        private int _quoteOption = 1;
        private int _supplyType = Convert.ToInt32(NQuoteSupplyType.SupplyAndInstallation);
        public int QuoteItemID { get; set; }


        public int QiPrintOrder { get; set; }


        public int QuoteOption
        {
            get { return _quoteOption; }

            set { _quoteOption = value; }
        }


        public string QuoteOptionText
        {
            get { return _QuoteOptionText; }
            set { _QuoteOptionText = value; }
        }


        public int SupplyType
        {
            get { return _supplyType; }
            set { _supplyType = value; }
        }


        public int QiQty { get; set; }

        public int QiQtyPc { get; set; }

        public bool IsFinal { get; set; }


        public double QiAmount { get; set; }

        public string QiAmountText
        {
            get { return _qiAmountText; }
            set { _qiAmountText = value; }
        }


        public string QiItemTitle
        {
            //Return GetString("", "qiItemTitle")
            get { return _qiItemTitle; }
            set { _qiItemTitle = value; }
        }

        public string QiDescription
        {
            // Return GetString("", "qiDescription")
            get { return _qiDescription; }
            set { _qiDescription = value; }
        }


        //???For service only
        public int QsServiceID { get; set; }

        #endregion

        //for Inilization
        public QuoteItemEdit(int quoteItemID)
        {
            QuoteItemID = quoteItemID;
            DataRow row = GetItemDataRow();
            InitializationProperties(row);
        }

        //for Update
        public QuoteItemEdit()
        {
        }

        public abstract void InitializationProperties(DataRow row);
        public abstract DataRow GetItemDataRow();
        public abstract void Update();

        public static string GetString(string defaultValue, DataRow row, string fieldsName)
        {
            string s = defaultValue;

            if (!MyConvert.IsNullString(row[fieldsName]))
            {
                s = Convert.ToString(row[fieldsName]);
            }

            return s;
        }

        public static int GetString(int defaultValue, DataRow row, string fieldsName)
        {
            int s = defaultValue;

            if (!MyConvert.IsNullString(row[fieldsName]))
            {
                s = Convert.ToInt32(row[fieldsName]);
            }

            return s;
        }

        public static bool GetString(bool defaultValue, DataRow row, string fieldsName)
        {
            bool s = defaultValue;

            if (!MyConvert.IsNullString(row[fieldsName]))
            {
                s = Convert.ToBoolean(row[fieldsName]);
            }

            return s;
        }
    }


    public class QuoteItemProduction : QuoteItemEdit
    {
        public QuoteItemProduction(int quoteItemID)
            : base(quoteItemID)
        {
        }

        public QuoteItemProduction()
        {
        }


        //string recordType = "Q";

        public override DataRow GetItemDataRow()
        {
            //Input: quoteRevID and recordType

            DataRow row = null;


            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString =
                "SELECT * FROM [Quote_Item] WHERE ([recordType]= 'Q' and [quoteItemID] = @quoteItemID)";
            //Q
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand);
            var ds1 = new DataSet();
            ds1.Tables.Clear();
            adapter1.SelectCommand.Parameters.Add("@quoteItemID", SqlDbType.Int).Value = QuoteItemID;

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

        public override void InitializationProperties(DataRow row)
        {
            if (row != null)
            {
                QiPrintOrder = GetString(1, row, "qiPrintOrder");

                QuoteOptionText = GetString("1", row, "quoteOptionText");
                QiAmountText = GetString("0", row, "qiAmountText");

                IsFinal = GetString(false, row, "isFinal");
                SupplyType = GetString(Convert.ToInt32(NQuoteSupplyType.SupplyAndInstallation), row, "supplyType");
                QiItemTitle = GetString("", row, "qiItemTitle");
                QiDescription = GetString("", row, "qiDescription");

                QiQty = GetString(0, row, "qiQty");
                QiQtyPc = GetString(0, row, "qiQtypc");
            }
        }


        public override void Update()
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Quote_Item] SET [qiPrintOrder] = @qiPrintOrder, [quoteOption] = @quoteOption, [isFinal] = @isFinal, [supplyType] = @supplyType, [qiItemTitle] = @qiItemTitle, [qiDescription] = @qiDescription, [qiAmount] = @qiAmount, [qiQty] = @qiQty, [qiQtypc] = @qiQtypc, [quoteOptionText] = @quoteOptionText, [qiAmountText] = @qiAmountText  WHERE [quoteItemID] = @quoteItemID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@quoteItemID", SqlDbType.Int).Value = QuoteItemID;

                UPdateCommand.Parameters.Add("@qiPrintOrder", SqlDbType.Int).Value = QiPrintOrder;
                UPdateCommand.Parameters.Add("@quoteOption", SqlDbType.Int).Value = QuoteOption;

                UPdateCommand.Parameters.Add("@isFinal", SqlDbType.Bit).Value = IsFinal;
                UPdateCommand.Parameters.Add("@supplyType", SqlDbType.Int).Value = SupplyType;

                UPdateCommand.Parameters.Add("@qiItemTitle", SqlDbType.NVarChar, -1).Value = QiItemTitle;
                UPdateCommand.Parameters.Add("@qiDescription", SqlDbType.NVarChar, -1).Value = QiDescription;

                UPdateCommand.Parameters.Add("@qiAmount", SqlDbType.Float).Value = QiAmount;

                UPdateCommand.Parameters.Add("@qiQty", SqlDbType.Int).Value = QiQty;
                UPdateCommand.Parameters.Add("@qiQtypc", SqlDbType.Int).Value = QiQtyPc;

                UPdateCommand.Parameters.Add("@quoteOptionText", SqlDbType.NVarChar, 300).Value = QuoteOptionText;
                UPdateCommand.Parameters.Add("@qiAmountText", SqlDbType.NVarChar, 300).Value = QiAmountText;

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


    public class QuoteItemService : QuoteItemEdit
    {
        private bool _IsTitleEditable = true;

        public QuoteItemService(int quoteItemID)
            : base(quoteItemID)
        {
        }

        public QuoteItemService()
        {
        }


        public bool IsTitleEditable
        {
            get { return _IsTitleEditable; }
            set { _IsTitleEditable = value; }
        }


        public override DataRow GetItemDataRow()
        {
            //Input: quoteRevID and recordType

            DataRow row = null;

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 = "SELECT * FROM [FW_QUOTE_SERVICE] WHERE ([qsID] = @qsID)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@qsID", SqlDbType.Int).Value = QuoteItemID;

            var ds1 = new DataSet();
            ds1.Tables.Clear();

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


        public override void InitializationProperties(DataRow row)
        {
            if (row != null)
            {
                QiPrintOrder = GetString(1, row, "qsPrintOrder");
                QiAmountText = GetString("0", row, "qsAmountText");

                QiItemTitle = GetString("", row, "qsTitle");
                //
                QiDescription = GetString("", row, "qsDescription");

                QsServiceID = GetString(0, row, "qsServiceID");
            }
        }


        private string GetServiceTitle(int serviceID)
        {
            string s = "";

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 = "SELECT *  FROM FW_QUOTE_PC WHERE (PC_ID= @PC_ID)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@PC_ID", SqlDbType.Int).Value = serviceID;
            var ds1 = new DataSet();
            ds1.Tables.Clear();
            try
            {
                ConnectionSQL.Open();
                int NumRowsAffected = adapter1.Fill(ds1, "t1");
                if (NumRowsAffected > 0)
                {
                    DataRow row = ds1.Tables["t1"].Rows[0];
                    s = Convert.ToString(row["PS_NAME"]);
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

            return s;
        }


        public override void Update()
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "Update FW_QUOTE_SERVICE SET qsPrintOrder = @qsPrintOrder, qsAmount = @qsAmount, qsDescription = @qsDescription, qsTitle = @qsTitle, qsAmountText = @qsAmountText WHERE qsID = @qsID";

                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@qsID", SqlDbType.Int).Value = QuoteItemID;

                UPdateCommand.Parameters.Add("@qsPrintOrder", SqlDbType.Int).Value = QiPrintOrder;

                UPdateCommand.Parameters.Add("@qsTitle", SqlDbType.NVarChar, -1).Value = QiItemTitle;
                UPdateCommand.Parameters.Add("@qsDescription", SqlDbType.NVarChar, -1).Value = QiDescription;

                UPdateCommand.Parameters.Add("@qsAmount", SqlDbType.Float).Value = QiAmount;
                UPdateCommand.Parameters.Add("@qsAmountText", SqlDbType.NVarChar, 300).Value = QiAmountText;

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


    public class QuoteItemNote : QuoteItemEdit
    {
        public QuoteItemNote(int quoteItemID)
            : base(quoteItemID)
        {
        }

        public QuoteItemNote()
        {
        }


        public override DataRow GetItemDataRow()
        {
            //Input: quoteRevID and recordType

            DataRow row = null;

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 = "SELECT * FROM FW_QUOTE_NOTE WHERE (qnID = @qnID)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@qnID", SqlDbType.Int).Value = QuoteItemID;

            var ds1 = new DataSet();
            ds1.Tables.Clear();

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


        public override void InitializationProperties(DataRow row)
        {
            if (row != null)
            {
                QiPrintOrder = GetString(1, row, "qnPrintOrder");

                QiItemTitle = GetString("", row, "qnTitle");
                //
                QiDescription = GetString("", row, "qnDescription");
            }
        }


        public override void Update()
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE FW_QUOTE_NOTE SET qnPrintOrder = @qnPrintOrder, qnDescription = @qnDescription, qnTitle = @qnTitle WHERE (qnID = @qnID)";

                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@qnID", SqlDbType.Int).Value = QuoteItemID;

                UPdateCommand.Parameters.Add("@qnPrintOrder", SqlDbType.Int).Value = QiPrintOrder;

                UPdateCommand.Parameters.Add("@qnTitle", SqlDbType.NVarChar, 500).Value = QiItemTitle;
                UPdateCommand.Parameters.Add("@qnDescription", SqlDbType.NVarChar, 3000).Value = QiDescription;

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