using System;
using System.Data;
using System.Data.SqlClient;
using MyCommon;
using MyCommon.MyEnum;
using SalesCenterDomain.BDL.Item;
using SalesCenterDomain.BDL.NumberControl;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Workorder
{

    #region ------------------Abstract  -------------------------------

    public abstract class WorkorderCopy
    {
        //Level 1:  WoID--->WoIdNew
        //Level 2:                  woItemID-->woItemIdNew
        //Level 3:                           ---->woDrawingID, copy itself
        //                                   ----->toID       , copy itself  

        private readonly int _woID;
        private int _newWoID;

        public WorkorderCopy(int woID)
        {
            _woID = woID;
        }

        public abstract int ValidationErrorID { get; }

        public int NewWoID
        {
            get { return _newWoID; }
        }

        public string ExistingWorkorderNumber { get; set; }

        public void Copy()
        {
            if (ValidationErrorID == 0)
            {
                ExistingWorkorderNumber = CopyWorkorderTitle();
                _newWoID = GetNewlyInsertedWorkOrderID();
                CopyItems();
                UpdateField();
            }
        }

        public abstract void UpdateField();

        private string CopyWorkorderTitle()
        {
            string woNumber = "";
            string errorLog = "Nothing";
            int NumRowsAffected = 0;

            //2. Define the destination: InvoiceItem
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 = "SELECT * FROM [Sales_JobMasterList_WO] WHERE ([woID] = @woID)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@woID", SqlDbType.Int).Value = _woID;

            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                NumRowsAffected = adapter1.Fill(ds1, "t1");

                //copy

                if (NumRowsAffected != 0)
                {
                    DataRow row = ds1.Tables["t1"].Rows[0];

                    //3. Copy
                    int itemIndex = ds1.Tables["t1"].Columns.Count;
                    DataRow rowNew = null;
                    int i = 0;

                    rowNew = ds1.Tables["t1"].NewRow();
                    for (i = 1; i <= itemIndex - 1; i++)
                    {
                        rowNew[i] = row[i];
                    }
                    //Issued Date=Today
                    rowNew["issuedDate"] = DateTime.Today;
                    //New Status
                    rowNew["woStatus"] = NJobStatus.woNew;
                    //Assign workorder number
                    woNumber = Convert.ToString(row["WorkorderNumber"]);
                    rowNew["RedoOfWoNumbers"] = woNumber;

                    ds1.Tables["t1"].Rows.Add(rowNew);

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
            //ErrorExit:
            return woNumber;
        }

        private void CopyItems()
        {
            DataTable tbl = WorkorderShared.getExistingWorkorderItems(_woID);
            if (tbl == null) return;
            foreach (DataRow row in tbl.Rows)
            {
                int woItemID = Convert.ToInt32(row["woItemID"]);
                var wi = new WorkorderItemCopy(_woID, _newWoID, woItemID);
                wi.Copy();
            }
        }

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

    #endregion

    #region  --------  Copy Title --------------------

    public class WorkorderCopyFactory
    {
        public WorkorderCopyFactory(int copyType, int woID)
        {
            switch (copyType)
            {
                case (int) NMenuCommand.CopyToAnother:
                    ConcreteWorkorderCopy = new WorkorderCopyToAnother(woID);
                    break;

                case (int) NMenuCommand.WorkorderCopyToRedo:
                    ConcreteWorkorderCopy = new WorkorderCopyToRedo(woID);
                    break;
                case (int) NMenuCommand.CopyToNewRevision:
                    ConcreteWorkorderCopy = new WorkorderCopyToNewRevision(woID);
                    break;

                default:
                    ConcreteWorkorderCopy = new WorkorderCopyToRevise(woID);
                    break;
            }
        }

        public WorkorderCopy ConcreteWorkorderCopy { get; private set; }
    }

    public class WorkorderCopyToAnother : WorkorderCopy
    {
        private readonly int _woID;

        public WorkorderCopyToAnother(int woID) : base(woID)
        {
            _woID = woID;
        }

        public override int ValidationErrorID
        {
            get { return GetValidationErrorID(); }
        }

        public override void UpdateField()
        {
            var wr1 = new WorkorderNumberRegisterToNew(NewWoID);
            wr1.Register();

            UpdateWorkOrderCopytoAnother();
        }


        private void UpdateWorkOrderCopytoAnother()
        {
            int rowsAffected = 0;
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString = "Select * FROM Sales_JobMasterList_WO WHERE (woID= @woID)";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand);
            var ds1 = new DataSet();
            ds1.Tables.Clear();

            adapter1.SelectCommand.Parameters.Add("@woID", SqlDbType.Int).Value = NewWoID;

            try
            {
                ConnectionSQL.Open();
                rowsAffected = adapter1.Fill(ds1, "t1");
                if (rowsAffected > 0)
                {
                    DataRow row = ds1.Tables["t1"].Rows[0];

                    //Revision =1
                    row["woRev"] = 1;

                    row["revise"] = false;
                    row["revisedReason"] = "";
                    row["reviseVer"] = 0;

                    row["redo"] = false;
                    row["redoReason"] = "";
                    row["redoVer"] = 0;

                    row["rush"] = false;
                    row["rushReason"] = "";


                    //1.3. Write  back to DB
                    var cb = new SqlCommandBuilder(adapter1);
                    adapter1 = cb.DataAdapter;
                    rowsAffected += adapter1.Update(ds1, "t1");
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

        private int GetValidationErrorID()
        {
            return 0;
        }
    }

    public class WorkorderCopyToRedo : WorkorderCopy
    {
        private readonly int _woID;

        public WorkorderCopyToRedo(int woID) : base(woID)
        {
            _woID = woID;
        }

        public override int ValidationErrorID
        {
            get { return GetValidationErrorID(); }
        }

        public override void UpdateField()
        {
            var wr1 = new WorkorderNumberRegisterToNew(NewWoID);
            wr1.Register();

            UpdateWorkOrderCopyToRedo();
        }

        private void UpdateWorkOrderCopyToRedo()
        {
            int rowsAffected = 0;
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString = "Select woID, redo,redoVer,woRev, WorkorderAmount, PayMethods FROM Sales_JobMasterList_WO WHERE (woID= @woID)";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand);
            var ds1 = new DataSet();
            ds1.Tables.Clear();

            adapter1.SelectCommand.Parameters.Add("@woID", SqlDbType.Int).Value = NewWoID;

            try
            {
                ConnectionSQL.Open();
                rowsAffected = adapter1.Fill(ds1, "t1");
                if (rowsAffected > 0)
                {
                    DataRow row = ds1.Tables["t1"].Rows[0];

                    //Revision=1
                    row["woRev"] = 1;

                    row["redo"] = true;
                    row["redoVer"] = MyConvert.ConvertToInteger(row["redoVer"]) + 1;
                    row["WorkorderAmount"] = 0;
                    row["PayMethods"] =(int)NPaymentMethods.NoChargeOther;  
                 
                    //1.3. Write  back to DB
                    var cb = new SqlCommandBuilder(adapter1);
                    adapter1 = cb.DataAdapter;
                    rowsAffected += adapter1.Update(ds1, "t1");
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

        private int GetValidationErrorID()
        {
            var ws = new WorkOrderSelect(_woID);
            int woStatus = ws.WoStatus();
            return woStatus == (int) NJobStatus.woApproved ? 0 : 1503;
        }
    }

    public class WorkorderCopyToNewRevision : WorkorderCopy
    {
        private readonly int _woID;

        public WorkorderCopyToNewRevision(int woID) : base(woID)
        {
            _woID = woID;
        }

        public override int ValidationErrorID
        {
            get { return GetValidationErrorID(); }
        }

        public override void UpdateField()
        {
            var wr2 = new WorkorderNumberRegisterToAnotherVersion(NewWoID);
            wr2.Register();

            int woRevNew2 = GetNewWorkorderRevision(ExistingWorkorderNumber) + 1;
            UpdateNewWorkorderRevision(NewWoID, woRevNew2);

            UpdateMisc();
        }


        private int GetNewWorkorderRevision(string woNumber)
        {
            string errorLog = "Nothing";
            int NumRowsAffected = 0;
            int woMaxRev = 0;

            //2. Define the destination: InvoiceItem
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 =
                "SELECT WorkorderNumber, MAX(woRev) AS ExistingMaxRev FROM Sales_JobMasterList_WO GROUP BY WorkorderNumber HAVING (WorkorderNumber = @WorkorderNumber)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@workorderNumber", SqlDbType.NVarChar, 15).Value = woNumber;
            var ds1 = new DataSet();
            ds1.Tables.Clear();
            try
            {
                ConnectionSQL.Open();
                NumRowsAffected = adapter1.Fill(ds1, "t1");
                //copy
                if (NumRowsAffected != 0)
                {
                    DataRow row = ds1.Tables["t1"].Rows[0];
                    woMaxRev = Convert.ToInt32(row["ExistingMaxRev"]);
                }
            }
            catch (SqlException ex)
            {
                errorLog = ex.Message;
            }

            return woMaxRev;
        }


        private void UpdateNewWorkorderRevision(int workorderID, int woRevNew)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString = "UPDATE [Sales_JobMasterList_WO] SET [woRev] = @woRev WHERE [woID] = @woID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@woID", SqlDbType.Int).Value = workorderID;
                UPdateCommand.Parameters.Add("@woRev", SqlDbType.Int).Value = woRevNew;
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


        public virtual int GetValidationErrorID()
        {
            var ws = new WorkOrderSelect(_woID);
            return !ws.IsLocked() ? 0 : 1501;
        }

        public virtual void UpdateMisc()
        {
        }
    }

    public class WorkorderCopyToRevise : WorkorderCopyToNewRevision
    {
        private readonly int _woID;

        public WorkorderCopyToRevise(int woID) : base(woID)
        {
            _woID = woID;
        }

        public override void UpdateMisc()
        {
            int rowsAffected = 0;
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString = "Select woID,revise,reviseVer FROM Sales_JobMasterList_WO WHERE (woID= @woID)";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand);
            var ds1 = new DataSet();
            ds1.Tables.Clear();

            adapter1.SelectCommand.Parameters.Add("@woID", SqlDbType.Int).Value = NewWoID;

            try
            {
                ConnectionSQL.Open();
                rowsAffected = adapter1.Fill(ds1, "t1");
                if (rowsAffected > 0)
                {
                    DataRow row = ds1.Tables["t1"].Rows[0];
                    row["revise"] = true;
                    row["reviseVer"] = MyConvert.ConvertToInteger(row["reviseVer"]) + 1;
                    //1.3. Write  back to DB
                    var cb = new SqlCommandBuilder(adapter1);
                    adapter1 = cb.DataAdapter;
                    rowsAffected += adapter1.Update(ds1, "t1");
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


        public override int GetValidationErrorID()
        {
            var ws = new WorkOrderSelect(_woID);
            return ws.IsLocked() ? 0 : 1502;
        }
    }

    #endregion

    #region  -------------  Copy Item -----------------------------

    public class WorkorderItemCopy
    {
        private readonly int _woIDNew;

        private readonly int _woItemID;
        private int _woID;

        public WorkorderItemCopy(int woID, int woIDNew, int woItemID)
        {
            _woID = woID;
            _woIDNew = woIDNew;
            _woItemID = woItemID;
        }


        public void Copy()
        {
            CopySingleWorkorderItem();

            int woItemIdNew = getNewlyInsertedWoItemID();

            //CopyWorkorderItemDrawing
            var drawing = new WorkorderItemDrawingCopy(_woItemID);
            drawing.CopyItemDocuments(woItemIdNew);
            //Copy Note
            CopyWorkorderItemTo(woItemIdNew);
        }

        private void CopySingleWorkorderItem()
        {
            string errorLog = "Nothing";
            int NumRowsAffected = 0;

            //2. Define the destination: InvoiceItem
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 = "SELECT * FROM [WO_Item] WHERE ([woItemID] = @woItemID)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@woItemID", SqlDbType.Int).Value = _woItemID;
            var ds1 = new DataSet();

            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                NumRowsAffected = adapter1.Fill(ds1, "t1");

                //copy
                if (NumRowsAffected != 0)
                {
                    //Destination

                    //3. Copy
                    int itemIndex = ds1.Tables["t1"].Columns.Count;
                    DataRow row = ds1.Tables["t1"].Rows[0];
                    DataRow rowNew = null;
                    int i = 0;

                    rowNew = ds1.Tables["t1"].NewRow();
                    for (i = 1; i <= itemIndex - 1; i++)
                    {
                        rowNew[i] = row[i];
                    }

                    rowNew["woID"] = _woIDNew;

                    ds1.Tables["t1"].Rows.Add(rowNew);


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

        //Note

        private void CopyWorkorderItemTo(int woItemIDNew)
        {
            string errorLog = "Nothing";
            int NumRowsAffected = 0;

            //2. Define the destination: InvoiceItem
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 = "SELECT * FROM [WO_Item_To] WHERE ([woItemID] = @woItemID)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@woItemID", SqlDbType.Int).Value = _woItemID;
            var ds1 = new DataSet();

            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                NumRowsAffected = adapter1.Fill(ds1, "t1");

                //copy

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
                        //-------This is the only fields changed
                        rowNew["woItemID"] = woItemIDNew;

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

        private int getNewlyInsertedWoItemID()
        {
            int ID = 0;
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string SelectString = "SELECT IDENT_CURRENT('WO_Item')";
                var SelectCommand = new SqlCommand(SelectString, Connection);
                try
                {
                    Connection.Open();
                    object tID = SelectCommand.ExecuteScalar();
                    ID = Convert.ToInt32(tID);
                }
                catch (SqlException ex)
                {
                    ID = 0;
                }
                finally
                {
                    Connection.Close();
                }
            }
            return ID;
        }
    }

    #endregion
}