using System;
using System.Data;
using System.Data.SqlClient;
using MyCommon;
using MyCommon.MyEnum;
using SalesCenterDomain.BDL.Invoice;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.NumberControl
{
    public class NumberControlYearEndMainTableInilization
    {
        //develop and implementation, run one time


        public void Manipulate()
        {
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 = "SELECT * FROM [Sales_JobMasterList_NumberControl]";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                int NumRowsAffected = adapter1.Fill(ds1, "t1");

                if (NumRowsAffected > 0)
                {
                    //3. Copy
                    foreach (DataRow row in ds1.Tables["t1"].Rows)
                    {
                        row["ID"] = row["numControlID"];
                    }

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


        //Yearly

        public void Insert()
        {
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 = "SELECT * FROM [Sales_JobMasterList_NumberControl] WHERE ([year] = @year)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@year", SqlDbType.Int).Value = DateTime.Today.Year;
            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                int NumRowsAffected = adapter1.Fill(ds1, "t1");
                int numControlIDMax = 0;

                if (NumRowsAffected > 0)
                {
                    //3. Copy

                    foreach (DataRow row in ds1.Tables["t1"].Rows)
                    {
                        int temp = Convert.ToInt32(row["numControlID"]);
                        if (temp > numControlIDMax)
                        {
                            numControlIDMax = temp;
                        }
                    }
                }

                for (int i = numControlIDMax + 1; i <= SalesCenterConstants.TOTAL_RECORD_COUNT_PER_YEAR; i++)
                {
                    DataRow rowNew = ds1.Tables["t1"].NewRow();
                    rowNew["numControlID"] = i;
                    rowNew["year"] = DateTime.Today.Year;
                    ds1.Tables["t1"].Rows.Add(rowNew);
                }

                //4. Write ds2,  back to DB

                var cb = new SqlCommandBuilder(adapter1);
                adapter1 = cb.DataAdapter;
                NumRowsAffected = adapter1.Update(ds1, "t1");
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

    //?  
    public class InvoiceNumberController
    {
        private readonly int _invType = (int) NInvoiceType.Regular;

        private readonly int _myID;

        public InvoiceNumberController(int myID, int invType)
        {
            _myID = myID;
            _invType = invType;
        }

        private int getNewFreeInvoiceNumberWithoutPrefix()
        {
            string SqlSelectString2 = "";
            switch (_invType)
            {
                case (int) NInvoiceType.Regular:
                    SqlSelectString2 =
                        "SELECT MIN(DISTINCT numControlID) AS numControlID, num FROM View_NumberControl_Invoice GROUP BY num HAVING (num IS NULL)";
                    break;
                case (int) NInvoiceType.Proforma:
                    SqlSelectString2 =
                        "SELECT MIN(DISTINCT numControlID) AS numControlID, num FROM View_NumberControl_ProformaInvoice GROUP BY num HAVING (num IS NULL)";
                    break;
            }

            int fn = 0;

            int NumRowsAffected = 0;
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);

            var SelectCommand2 = new SqlCommand(SqlSelectString2, ConnectionSQL);
            var adapter2 = new SqlDataAdapter(SelectCommand2);
            var ds2 = new DataSet();
            ds2.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                NumRowsAffected = adapter2.Fill(ds2, "t2");
                if (NumRowsAffected > 0)
                {
                    DataRow row = null;
                    row = ds2.Tables["t2"].Rows[0];
                    fn = Convert.ToInt32(row["numControlID"]);
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
            return fn;
        }

        private int getNewFreeInvoiceNumberWithoutPrefix(int invoiceID)
        {
            string SqlSelectString2 = "";
            switch (_invType)
            {
                case (int) NInvoiceType.Regular:
                    SqlSelectString2 = "SELECT num FROM Sales_JobMasterList_InvoiceNumber where (invoiceID=@invoiceID)";
                    break;
                case (int) NInvoiceType.Proforma:
                    SqlSelectString2 =
                        "SELECT num FROM Sales_JobMasterList_ProformaInvoiceNumber where (invoiceID=@invoiceID)";
                    break;
            }

            int fn = 0;

            int NumRowsAffected = 0;
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);

            var SelectCommand2 = new SqlCommand(SqlSelectString2, ConnectionSQL);
            var adapter2 = new SqlDataAdapter(SelectCommand2);

            adapter2.SelectCommand.Parameters.Add("@invoiceID", SqlDbType.Int).Value = invoiceID;

            var ds2 = new DataSet();
            ds2.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                NumRowsAffected = adapter2.Fill(ds2, "t2");
                if (NumRowsAffected > 0)
                {
                    DataRow row = ds2.Tables["t2"].Rows[0];
                    fn = Convert.ToInt32(row["num"]);
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

            return fn;
        }


        private string getCalculatedInvocieNumber(int freeNum)
        {
            //Last one or 2 digit + Prefix + Number

            string Num = "";
            //year, 9, 10,11..
            Num += MyDateTime.GetYearNumber();

            //Prefix, 
            string prefix = "";
            switch (_invType)
            {
                case (int) NInvoiceType.Regular:
                    prefix = SalesCenterConstants.PrefixInvoice;

                    break;
                case (int) NInvoiceType.Proforma:
                    prefix = SalesCenterConstants.PrefixProformaInvoice;

                    break;
            }

            Num += prefix;

            Num += (freeNum + SalesCenterConstants.BEGIN_INVOICE_NUMBER).ToString("D4");

            return Num;
        }

        public void Register()
        {
            int freeNum = getNewFreeInvoiceNumberWithoutPrefix();
            RegisterInvoiceID(freeNum);
            string invoiceNumber = getCalculatedInvocieNumber(freeNum);

            var ip = new InvoiceProperty(_myID);
            ip.InvoiceNumber = invoiceNumber;
        }

        public string GetPureInvoiceNumberWithoutSurfix()
        {
            int freeNum = getNewFreeInvoiceNumberWithoutPrefix(_myID);
            string invoiceNumber = getCalculatedInvocieNumber(freeNum);
            return invoiceNumber;
        }

        //Register

        private void RegisterInvoiceID(int freeNum)
        {
            string InsertString = "";
            switch (_invType)
            {
                case (int) NInvoiceType.Regular:
                    InsertString =
                        "INSERT INTO [Sales_JobMasterList_InvoiceNumber] ([Prefix], [invoiceID], [num], [year]) VALUES (@Prefix, @invoiceID, @num, @year)";
                    break;
                case (int) NInvoiceType.Proforma:
                    InsertString =
                        "INSERT INTO [Sales_JobMasterList_ProformaInvoiceNumber] ([Prefix], [invoiceID], [num],[year]) VALUES (@Prefix, @invoiceID, @num, @year)";
                    break;
            }

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            using (ConnectionSQL)
            {
                // As New System.Data.SqlClient.SqlConnection(Configuration.ConnectionString.ConnectionString)

                var InsertCommand = new SqlCommand(InsertString, ConnectionSQL);

                string prefix = "";
                switch (_invType)
                {
                    case (int) NInvoiceType.Regular:
                        prefix = SalesCenterConstants.PrefixInvoice;

                        break;
                    case (int) NInvoiceType.Proforma:
                        prefix = SalesCenterConstants.PrefixProformaInvoice;

                        break;
                }

                InsertCommand.Parameters.Add("@PreFix", SqlDbType.NVarChar, 3).Value = prefix;
                InsertCommand.Parameters.Add("@invoiceID", SqlDbType.Int).Value = _myID;
                InsertCommand.Parameters.Add("@num", SqlDbType.Int).Value = freeNum;
                InsertCommand.Parameters.Add("@year", SqlDbType.Int).Value = DateTime.Today.Year;

                try
                {
                    ConnectionSQL.Open();
                    InsertCommand.ExecuteNonQuery();
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

        //Release
        public void ReleaseInvoiceNumber()
        {
            //No existing
            string SqlSelectString2 = "SELECT * FROM [Sales_JobMasterList_InvoiceNumber] WHERE (@InvoiceID=-1)";

            switch (_invType)
            {
                case (int) NInvoiceType.Regular:
                    SqlSelectString2 =
                        "SELECT * FROM [Sales_JobMasterList_InvoiceNumber] WHERE ([invoiceID] = @InvoiceID)";
                    break;
                case (int) NInvoiceType.Proforma:
                    SqlSelectString2 =
                        "SELECT * FROM [Sales_JobMasterList_ProformaInvoiceNumber] WHERE ([invoiceID] = @InvoiceID)";
                    break;
            }

            string errorLog = "Nothing";
            int NumRowsAffected = 0;

            //2. Define the destination: InvoiceItem
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);

            var SelectCommand2 = new SqlCommand(SqlSelectString2, ConnectionSQL);
            var adapter2 = new SqlDataAdapter(SelectCommand2);
            adapter2.SelectCommand.Parameters.Add("@InvoiceID", SqlDbType.Int).Value = _myID;
            var ds2 = new DataSet();
            ds2.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                NumRowsAffected = adapter2.Fill(ds2, "t2");

                if (NumRowsAffected > 0)
                {
                    DataRow row = ds2.Tables["t2"].Rows[0];
                    row.Delete();

                    //4. Write ds2,  back to DB
                    var cb = new SqlCommandBuilder(adapter2);
                    adapter2 = cb.DataAdapter;

                    NumRowsAffected = adapter2.Update(ds2, "t2");
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


    public class WorkorderNumberRegisterToNew
    {
        //Case NMenuCommand.WorkorderNew, NMenuCommand.CopyToAnother, NMenuCommand.WorkorderCopyToRedo

        private readonly int _newWoID;

        public WorkorderNumberRegisterToNew(int newWoID)
        {
            _newWoID = newWoID;
        }

        public void Register()
        {
            //2 Workorder Number control

            int freeNum = 0;


            freeNum = GetNewFreeWorkorderNumber();

            RegisterWorkorderNumber(freeNum);

            UpdateWorkOrderNumber(freeNum);
        }


        public virtual int GetNewFreeWorkorderNumber()
        {
            int fn = 0;
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);

            string SqlSelectString2 =
                "SELECT MIN(DISTINCT numControlID) AS numControlID, num FROM View_NumberControl_workorder GROUP BY num HAVING (num IS NULL)";
            var SelectCommand2 = new SqlCommand(SqlSelectString2, ConnectionSQL);
            var adapter2 = new SqlDataAdapter(SelectCommand2);
            var ds2 = new DataSet();
            ds2.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                int NumRowsAffected = adapter2.Fill(ds2, "t2");
                if (NumRowsAffected > 0)
                {
                    DataRow row = null;
                    row = ds2.Tables["t2"].Rows[0];
                    fn = Convert.ToInt32(row["numControlID"]);
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
            return fn;
        }


        private string getWorkorderNumber(int freeNum)
        {
            //Last one or 2 digit + Prefix + Number

            string Num = "";
            //year, 9, 10,11..
            Num += MyDateTime.GetYearNumber();

            //Prefix, "J" 
            Num += SalesCenterConstants.PrefixWorkorder;

            // min wnI

            Num += (freeNum + SalesCenterConstants.BEGIN_WORKORDER_NUMBER).ToString("D4");

            return Num;
        }


        private void RegisterWorkorderNumber(int num)
        {
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            using (ConnectionSQL)
            {
                // As New System.Data.SqlClient.SqlConnection(Configuration.ConnectionString.ConnectionString)

                string InsertString =
                    "INSERT INTO [Sales_JobMasterList_WO_Number] ([Prefix], [woID], [num],[year]) VALUES (@Prefix, @woID, @num,@year)";
                var InsertCommand = new SqlCommand(InsertString, ConnectionSQL);
                InsertCommand.Parameters.Add("@PreFix", SqlDbType.NVarChar, 3).Value =
                    SalesCenterConstants.PrefixWorkorder;
                InsertCommand.Parameters.Add("@woID", SqlDbType.Int).Value = _newWoID;
                InsertCommand.Parameters.Add("@num", SqlDbType.Int).Value = num;
                InsertCommand.Parameters.Add("@year", SqlDbType.Int).Value = DateTime.Today.Year;
                try
                {
                    ConnectionSQL.Open();
                    InsertCommand.ExecuteNonQuery();
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

        public virtual void UpdateWorkOrderNumber(int freeNum)
        {
            string woNumber = getWorkorderNumber(freeNum);

            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Sales_JobMasterList_WO] SET [WorkorderNumber] = @WorkOrderNumber WHERE [woID] = @woID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@woID", SqlDbType.Int).Value = _newWoID;
                UPdateCommand.Parameters.Add("@WorkorderNumber", SqlDbType.VarChar, 15).Value = woNumber;
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


    //NMenuCommand.CopyToNewRevision, NMenuCommand.WorkorderCopyToRevise
    public class WorkorderNumberRegisterToAnotherVersion : WorkorderNumberRegisterToNew
    {
        //
        private readonly int _newWoID;

        public WorkorderNumberRegisterToAnotherVersion(int newWoID)
            : base(newWoID)
        {
            _newWoID = newWoID;
        }

        public override int GetNewFreeWorkorderNumber()
        {
            //for this WoID

            int fn = 0;

            int NumRowsAffected = 0;
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);

            string SqlSelectString2 = "SELECT * FROM Sales_JobMasterList_Wo_Number WHERE (woID = @woID)";
            var SelectCommand2 = new SqlCommand(SqlSelectString2, ConnectionSQL);

            var adapter2 = new SqlDataAdapter(SelectCommand2);
            adapter2.SelectCommand.Parameters.Add("@woID", SqlDbType.Int).Value = _newWoID;

            var ds2 = new DataSet();
            ds2.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                NumRowsAffected = adapter2.Fill(ds2, "t2");
                if (NumRowsAffected > 0)
                {
                    DataRow row = null;
                    row = ds2.Tables["t2"].Rows[0];
                    fn = Convert.ToInt32(row["num"]);
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
            return fn;
        }

        public override void UpdateWorkOrderNumber(int freeNum)
        {
            //keep the number
        }
    }
}