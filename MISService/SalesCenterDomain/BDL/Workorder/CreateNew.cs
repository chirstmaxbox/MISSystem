using System;
using System.Data;
using System.Data.SqlClient;
using MyCommon;
using MyCommon.MyEnum;
using MyCommonWeb;
using SalesCenterDomain.BDL.NumberControl;
using SalesCenterDomain.BDL.Project;
using SalesCenterDomain.BDL.Quote;
using SalesCenterDomain.BLL;
using SalesCenterDomain.BO;
using SpecDomain.BLL;
using SpecDomain.BLL.EstItem;

namespace SalesCenterDomain.BDL.Workorder
{
    public abstract class WorkorderValidationGeneration
    {
        // private string _woNumber = "";
        private readonly int _jobID;


        public WorkorderValidationGeneration(int jobID)
        {
            _jobID = jobID;
        }

        public abstract int ChildrenCount { get; }

        public int ValidationErrorID
        {
            get { return GetValidationErrorID(); }
        }

        private int GetValidationErrorID()
        {
            var cp = new ProjectCompany(_jobID);
            //Install to Company
            if (!cp.isThereAInstallOrShipToCompany) return 10;

            //Bill to company if not subproject
            var p = new ProjectDetails(_jobID);
            if (!(p.IsSubProject | cp.isThereABilltoCompany)) return 11;

            if (ChildrenCount == 0) return 2;
            return 0;
        }
    }

    public class WorkorderValidationGenerateFromEstimation : WorkorderValidationGeneration
    {
        private readonly int _estRevID;

        public WorkorderValidationGenerateFromEstimation(int jobID, int estRevID) : base(jobID)
        {
            _estRevID = estRevID;
        }

        public override int ChildrenCount
        {
            get { return GetChildrenCount(); }
        }

        private int GetChildrenCount()
        {
            int cc = 0;
            DataTable tbl = EstItemCommon.GetEstimationItems(_estRevID);
            if (tbl != null)
            {
                cc = tbl.Rows.Count;
            }
            return cc;
        }
    }

    public class WorkorderValidationGenerateFromQuote : WorkorderValidationGeneration
    {
        private readonly int _quoteRevID;

        public WorkorderValidationGenerateFromQuote(int jobID, int quoteRevID) : base(jobID)
        {
            _quoteRevID = quoteRevID;
        }

        public override int ChildrenCount
        {
            get { return GetChildrenCount(); }
        }

        private int GetChildrenCount()
        {
            int cc = 0;
            var quote = new QuoteChildren(_quoteRevID);
            DataTable tbl = quote.ItemList("Q", true);

            if (tbl != null)
            {
                cc = tbl.Rows.Count;
            }
            return cc;
        }
    }


    public abstract class WorkorderGeneration
    {
        private readonly ProjectCompany _cp;
        private readonly int _jobID;
        private readonly ProjectDetails _p;

        public WorkorderGeneration(int jobID)
        {
            WoID = 0;
            _jobID = jobID;
            _cp = new ProjectCompany(_jobID);
            _p = new ProjectDetails(_jobID);
        }

        public abstract int ValidationErrorID { get; }
        public abstract int EstRevID { get; }
        public abstract int FromWhere { get; }

        public int WoID { get; private set; }


        public void CreateNew()
        {
            if (ValidationErrorID != 0) return;

            InsertNewWorkOrderHeader();

            WoID = GetNewlyInsertedWorkOrderID();

            var wr = new WorkorderNumberRegisterToNew(WoID);
            wr.Register();

            //3. Generate Workorder Itme
            //CreateNewWorkorderItems();
        }


        private void InsertNewWorkOrderHeader()
        {
            var connectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            using (connectionSQL)
            {
                //No deadline, which will be NULL
                const string insertString =
                    "INSERT INTO [Sales_JobMasterList_WO] ([jobID], [estRevID], [woType], [woRev], [woCurrent], [issuedDate], [ExactDate], [Company1],[Company2], [Contact1], [Contact2], [PayMethods], [woStatus], [ParkingLocation],[sales],[sa1ID],[jobTitle],[GenerateFrom]) VALUES (@jobID, @estRevID, @woType, @woRev, @woCurrent,  @issuedDate, @ExactDate,@Company1, @Company2, @Contact1, @Contact2, @PayMethods, @woStatus, @ParkingLocation,@sales,@sa1ID,@jobTitle, @GenerateFrom)";
                var insertCommand = new SqlCommand(insertString, connectionSQL);
                insertCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = _jobID;

                insertCommand.Parameters.Add("@estRevID", SqlDbType.Int).Value = EstRevID;

                insertCommand.Parameters.Add("@woType", SqlDbType.SmallInt).Value = NWorkorderType.Production;
                insertCommand.Parameters.Add("@woRev", SqlDbType.SmallInt).Value = 1;
                //New Revision
                insertCommand.Parameters.Add("@woCurrent", SqlDbType.Bit).Value = true;

                insertCommand.Parameters.Add("@issuedDate", SqlDbType.SmallDateTime).Value = DateTime.Today;
                insertCommand.Parameters.Add("@exactDate", SqlDbType.Int).Value = false;
                //Obselete

                insertCommand.Parameters.Add("@Company1", SqlDbType.Int).Value = _cp.InstallToCompanyID;
                insertCommand.Parameters.Add("@Contact1", SqlDbType.Int).Value = _cp.InstallToContactID;
                insertCommand.Parameters.Add("@Company2", SqlDbType.Int).Value = _cp.BillToCompanyID;
                insertCommand.Parameters.Add("@Contact2", SqlDbType.Int).Value = _cp.BillToContactID;

                insertCommand.Parameters.Add("@PayMethods", SqlDbType.SmallInt).Value = NPaymentMethods.MailOutbyOffice;
                insertCommand.Parameters.Add("@woStatus", SqlDbType.SmallInt).Value = NJobStatus.woNew;
                insertCommand.Parameters.Add("@ParkingLocation", SqlDbType.Bit).Value = false;
                insertCommand.Parameters.Add("@sales", SqlDbType.Int).Value = _p.Sales;
                insertCommand.Parameters.Add("@sa1ID", SqlDbType.Int).Value = _p.Sa1ID;
                insertCommand.Parameters.Add("@jobTitle", SqlDbType.NVarChar, 200).Value = _p.JobTitle;
                insertCommand.Parameters.Add("@GenerateFrom", SqlDbType.SmallInt).Value = FromWhere;

                try
                {
                    connectionSQL.Open();
                    insertCommand.ExecuteNonQuery();
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

        public abstract void CreateNewWorkorderItems();

        private int GetNewlyInsertedWorkOrderID()
        {
            int id = 0;
            using (var connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                const string selectString = "SELECT IDENT_CURRENT('Sales_JobMasterList_WO')";
                var selectCommand = new SqlCommand(selectString, connection);
                try
                {
                    connection.Open();
                    object tID = selectCommand.ExecuteScalar();
                    id = Convert.ToInt32(tID);
                }
                catch (SqlException ex)
                {
                    id = 0;
                }
                finally
                {
                    connection.Close();
                }
            }
            return id;
        }
    }

    public class WorkorderGenerateFromEstimation : WorkorderGeneration
    {
        private readonly int _estRevID;
        private readonly int _jobID;

        public WorkorderGenerateFromEstimation(int jobID, int estRevID) : base(jobID)
        {
            _estRevID = estRevID;
            _jobID = jobID;
        }

        public override int ValidationErrorID
        {
            get { return GetValidationErrorID(); }
        }

        public override int EstRevID
        {
            get { return _estRevID; }
        }

        public override int FromWhere
        {
            get { return (int) NGenerateTitleFrom.Estimation; }
        }

        public override void CreateNewWorkorderItems()
        {
            var wi = new WorkorderItemGenerateFromEstimation(WoID, _estRevID);
            wi.GenerateItems();
        }

        private int GetValidationErrorID()
        {
            var v = new WorkorderValidationGenerateFromEstimation(_jobID, _estRevID);

            return v.ValidationErrorID;
        }
    }


    public class WorkorderGenerateFromQuote : WorkorderGeneration
    {
        private readonly int _jobID;
        private readonly int _quoteRevID;

        public WorkorderGenerateFromQuote(int jobID, int quoteRevID) : base(jobID)
        {
            _quoteRevID = quoteRevID;
            _jobID = jobID;
        }

        public override int ValidationErrorID
        {
            get { return GetValidationErrorID(); }
        }

        public override int EstRevID
        {
            get { return GetEstRevID(); }
        }

        public override int FromWhere
        {
            get { return (int) NGenerateTitleFrom.Quote; }
        }


        public override void CreateNewWorkorderItems()
        {
            var wi = new WorkorderItemGenerateFromQuote(WoID, _quoteRevID);
            wi.GenerateItems();
        }

        private int GetValidationErrorID()
        {
            var v = new WorkorderValidationGenerateFromQuote(_jobID, _quoteRevID);

            return v.ValidationErrorID;
        }

        private int GetEstRevID()
        {
            var qt1 = new QuoteTitleProperty(_quoteRevID);
            return qt1.EstRevID;
        }
    }


    public class WokrorderItemGenerateFromBlank
    {
        private readonly int _woID;
        public int NewWorkItemID { get; set; }

        public WokrorderItemGenerateFromBlank(int woID)
        {
            _woID = woID;
            SetValidationID();
        }

        public int ValidationID { get; set; }

        private void SetValidationID()
        {
            ValidationID = 0;

            //Lockec
            var wos = new WorkOrderSelect(_woID);
            if (wos.IsLocked())
            {
                ValidationID = (int) NValidationErrorValue.WorkorderLockedCanNotAddNewItem;
                return;
            }

            //IS service or Site check
            var wo = new MyWorkorder(_woID);

            if (wo.Value.woType == (int) NWorkorderType.Service |
                wo.Value.woType == (int) NWorkorderType.Sitecheck)
            {
                return;
            }

            //IS there a reason
            if (wo.Value.addNewItemReason != null)
            {
                string reason1 = Convert.ToString(wo.Value.addNewItemReason);
                if (reason1.Length >= 10)
                {
                    return;
                }
            }


            ValidationID = (int) NValidationErrorValue.WorkorderAddNewItem;
        }

        public void InsertItem()
        {
            InsertWorkorderItem();
            int newWoItemID = SqlCommon.GetNewlyInsertedRecordID("WO_Item");
            NewWorkItemID = newWoItemID;
            GenerateSerialID(newWoItemID);
        }

        private void InsertWorkorderItem()
        {
            using (var connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                const string insertString =
                    "INSERT INTO [WO_Item] ([woID], [estItemID],[woDescription],[woPrintOrder],[qty],[qtyPC],[heightFromGround],[Requirement],[Position],[estItemNameText],[fromWhere], [qtyUnit], [qtyPcUnit] ) VALUES (@woID,  @estItemID,@woDescription,@woPrintOrder,@qty,@qtyPC,@heightFromGround, @Requirement, @Position, @estItemNameText,@fromWhere, @qtyUnit, @qtyPcUnit)";

                // Create the command and set its properties.
                var insertCommand = new SqlCommand(insertString, connection);
                try
                {
                    connection.Open();
                    insertCommand.Parameters.Add("@woID", SqlDbType.Int).Value = _woID;
                    insertCommand.Parameters.Add("@estItemID", SqlDbType.Int).Value = 0;
                    insertCommand.Parameters.Add("@woDescription", SqlDbType.NVarChar, 2500).Value = "New Item";
                    insertCommand.Parameters.Add("@woPrintOrder", SqlDbType.Int).Value = 1 + GetMaxPrintOrderOfItems();
                    insertCommand.Parameters.Add("@qty", SqlDbType.Int).Value = 1;
                    insertCommand.Parameters.Add("@qtyPC", SqlDbType.Int).Value = 1;
                    insertCommand.Parameters.Add("@heightFromGround", SqlDbType.NVarChar, 30).Value = "";
                    insertCommand.Parameters.Add("@Requirement", SqlDbType.Int).Value = GetItemRequirement();
                    insertCommand.Parameters.Add("@Position", SqlDbType.NVarChar, 7).Value = "Outdoor";
                    insertCommand.Parameters.Add("@estItemNameText", SqlDbType.NVarChar, 200).Value = "New Item";
                    insertCommand.Parameters.Add("@fromWhere", SqlDbType.SmallInt).Value = NGenerateTitleFrom.Blank;
                    insertCommand.Parameters.Add("@qtyUnit", SqlDbType.NVarChar, 50).Value = "set";
                    insertCommand.Parameters.Add("@qtyPcUnit", SqlDbType.NVarChar, 50).Value = "pc";

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


        private int GetItemRequirement()
        {
            var wo = new MyWorkorder(_woID);

            if (wo.Value.woType == (int) NWorkorderType.Service)
                return (int) NWorkorderRequirement.Service;
            if (wo.Value.woType == (int) NWorkorderType.Sitecheck)
                return (int) NWorkorderRequirement.SiteCheck;

            return (int) NWorkorderRequirement.Installation;
        }


        private int GetMaxPrintOrderOfItems()
        {
            int maxValue = 0;
            DataTable children = WorkorderShared.getExistingWorkorderItems(_woID);
            if (children == null) return maxValue;

            foreach (DataRow row in children.Rows)
            {
                int i = Convert.ToInt32(row["woPrintOrder"]);
                if (i > maxValue)
                {
                    maxValue = i;
                }
            }
            return maxValue;
        }


        public void GenerateSerialID(int newWoItemID)
        {
            var mwi = new MyWorkorderItem(_woID, newWoItemID);
            mwi.GenerateMySelfSerialID();
        }
    }

    public class WorkorderItemGenerateFromEstimation
    {
        private readonly int _myParentID;

        private readonly int _sourceTitleID;

        //    private int _myID;
        public WorkorderItemGenerateFromEstimation(int myParentID, int sourceTitleID)
        {
            _myParentID = myParentID;
            _sourceTitleID = sourceTitleID;
        }


        public void GenerateItems()
        {
            DataTable tblItems = GetSourceItmes(_sourceTitleID);

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 = "SELECT * FROM [WO_Item] WHERE ([woItemID] = 0)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);

            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                adapter1.Fill(ds1, "t1");

                CopyItems(_myParentID, ds1, tblItems);

                //setUnit, pcUnit
                foreach (DataRow row in ds1.Tables["t1"].Rows)
                {
                    row["qtyUnit"] = "set";
                    row["qtyPcUnit"] = "pc";
                }

                var cb = new SqlCommandBuilder(adapter1);
                adapter1 = cb.DataAdapter;
                adapter1.Update(ds1, "t1");
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

        public virtual DataTable GetSourceItmes(int sourceTitleID)
        {
            DataTable woItems = null;
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            int NumRowsAffected = 0;
            string SqlSelectString1 =
                " SELECT dbo.EST_Item.EstItemID, dbo.EST_Item.EstItemNo AS PrintOrder, dbo.EST_Item.IsFinalOption AS isFinal, CASE WHEN Description IS NULL THEN ' ' ELSE Description END AS Description, dbo.EST_Item.Qty, dbo.EST_Item.QtyPc, '' AS HeightFromGround, dbo.EST_Item.RequirementID AS Requirement, dbo.RequiredItemPosition.Name AS Position, dbo.EST_Item.ProductName AS estItemNameText, dbo.EST_Item.EstimatorPrice,  dbo.EST_Item.ProductID AS nameDetailsID, dbo.EST_Item.SerialID, dbo.EST_Item.BySubcontractor FROM dbo.EST_Item INNER JOIN dbo.RequiredItemPosition ON dbo.EST_Item.PositionID = dbo.RequiredItemPosition.PositionID WHERE (dbo.EST_Item.EstRevID = @estRevID AND ItemPurposeID=0 )";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@estRevID", SqlDbType.Int).Value = sourceTitleID;

            var ds1 = new DataSet();
            try
            {
                ConnectionSQL.Open();
                NumRowsAffected = adapter1.Fill(ds1, "t1");
                if (NumRowsAffected != 0)
                {
                    woItems = ds1.Tables["t1"];
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
            return woItems;
        }

        public virtual void CopyItems(int myParentID, DataSet ds1, DataTable tblItems)
        {
            foreach (DataRow row in tblItems.Rows)
            {
                if (Convert.ToBoolean(row["isFinal"]))
                {
                    DataRow rowNew = ds1.Tables["t1"].NewRow();
                    rowNew["woID"] = _myParentID;
                    rowNew["estItemID"] = MyConvert.ConvertToInteger(row["estItemID"]);
                    rowNew["estItemNameText"] = MyConvert.ConvertToString(row["estItemNameText"]);

                    string copyDetails = SalesCenterConfiguration.CopyDetailsToWorkorder;
                    if (copyDetails.Trim().ToLower() == "yes")
                    {
                        rowNew["woDescription"] = MyConvert.ConvertToString(row["Description"]);
                    }
                    else
                    {
                        rowNew["woDescription"] = DBNull.Value;
                    }

                    rowNew["woPrintOrder"] = MyConvert.ConvertToInteger(row["PrintOrder"]);
                    rowNew["qty"] = MyConvert.ConvertToInteger(row["qty"]);
                    rowNew["qtyPC"] = MyConvert.ConvertToInteger(row["qtyPC"]);
                    rowNew["heightFromGround"] = MyConvert.ConvertToString(row["heightFromGround"]);


                    int requirement = MyConvert.ConvertToInteger(row["Requirement"]);
                    if (requirement < 10)
                    {
                        requirement = 10;
                    }
                    rowNew["Requirement"] = requirement;

                    rowNew["Position"] = "Outdoor";
                    if (!Convert.IsDBNull(row["Position"]))
                    {
                        rowNew["Position"] = row["Position"];
                    }

                    rowNew["fromWhere"] = NGenerateTitleFrom.Estimation;

                    rowNew["scHydroInspectionRequired"] = false;
                    rowNew["scPurpose"] = 0;
                    rowNew["scPurpose1"] = false;
                    rowNew["scPurpose2"] = false;
                    rowNew["scPurpose3"] = false;
                    rowNew["scPurpose4"] = false;

                    //Item ID
                    rowNew["nameDetailsID"] = row["nameDetailsID"];
                    rowNew["qiAmount"] = MyConvert.ConvertToDouble(row["estimatorPrice"]);
                    string s = "E" + string.Format("{0:D2}", row["SerialID"]);
                    rowNew["SerialID"] = s;
                    rowNew["BySubcontractor"] = row["BySubcontractor"];
                    ds1.Tables["t1"].Rows.Add(rowNew);
                }
            }
        }
    }

    public class WorkorderItemGenerateFromQuote : WorkorderItemGenerateFromEstimation
    {
        public WorkorderItemGenerateFromQuote(int myParentID, int sourceTitleID)
            : base(myParentID, sourceTitleID)
        {
        }


        public override DataTable GetSourceItmes(int sourceTitleID)
        {
            DataTable woItems = null;
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            int NumRowsAffected = 0;
            string SqlSelectString1 = "SELECT * FROM Quote_Item where (QuoteRevID=@QuoteRevID and recordType='Q')";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@quoteRevID", SqlDbType.Int).Value = sourceTitleID;

            var ds1 = new DataSet();
            try
            {
                ConnectionSQL.Open();
                NumRowsAffected = adapter1.Fill(ds1, "t1");
                if (NumRowsAffected != 0)
                {
                    woItems = ds1.Tables["t1"];
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
            return woItems;
        }

        public override void CopyItems(int myParentID, DataSet ds1, DataTable tblItems)
        {
            foreach (DataRow row in tblItems.Rows)
            {
                if (Convert.ToBoolean(row["isFinal"]))
                {
                    DataRow rowNew = ds1.Tables["t1"].NewRow();
                    rowNew["woID"] = myParentID;
                    rowNew["estItemID"] = MyConvert.ConvertToInteger(row["quoteItemID"]);
                    string s = VbHtml.MyHtmlDecode( row["qiItemTitle"]);
                    rowNew["estItemNameText"] = s;

                    string copyDetails = SalesCenterConfiguration.CopyDetailsToWorkorder;
                    if (copyDetails.Trim().ToLower() == "yes")
                    {
                        rowNew["woDescription"] = MyConvert.ConvertToString(row["qiDescription"]);
                    }
                    else
                    {
                        rowNew["woDescription"] = DBNull.Value;
                    }

                    rowNew["woPrintOrder"] = MyConvert.ConvertToInteger(row["qiPrintOrder"]);
                    rowNew["qty"] = MyConvert.ConvertToInteger(row["qiQty"]);
                    rowNew["qtyPC"] = MyConvert.ConvertToInteger(row["qiQtyPC"]);
                    rowNew["heightFromGround"] = MyConvert.ConvertToString(row["heightFromGround"]);

                    int requirement = MyConvert.ConvertToInteger(row["Requirement"]);
                    if (requirement < 10)
                    {
                        requirement = 10;
                    }

                    if (requirement == 40)
                    {
                        requirement = 41;
                    }

                    rowNew["Requirement"] = requirement;

                    rowNew["Position"] = "Outdoor";
                    if (!Convert.IsDBNull(row["Position"]))
                    {
                        rowNew["Position"] = row["Position"];
                    }

                    rowNew["fromWhere"] = NGenerateTitleFrom.Quote;

                    rowNew["scHydroInspectionRequired"] = false;
                    rowNew["scPurpose"] = 0;
                    rowNew["scPurpose1"] = false;
                    rowNew["scPurpose2"] = false;
                    rowNew["scPurpose3"] = false;
                    rowNew["scPurpose4"] = false;

                    //Item ID
                    rowNew["nameDetailsID"] = row["nameDetailsID"];
                    //Price
                    rowNew["qiAmount"] = MyConvert.ConvertToDouble(row["qiAmount"]);
                    rowNew["SerialID"] = row["SerialID"];
                    rowNew["BySubcontractor"] = row["BySubcontractor"];
                    ds1.Tables["t1"].Rows.Add(rowNew);
                }
            }
        }
    }
}