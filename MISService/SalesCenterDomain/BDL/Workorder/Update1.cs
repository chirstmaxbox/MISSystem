using System;
using System.Data;
using System.Data.SqlClient;
using MyCommon.MyEnum;
using SalesCenterDomain.BDL.Item;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Workorder
{
    public abstract class WorkorderUpdateReasons
    {
        private readonly int _woID;

        public WorkorderUpdateReasons(int woID)
        {
            _woID = woID;
        }

        public string Reason
        {
            set { Update(_woID, value); }
        }

        public abstract void Update(int woID, string value);
    }

    public class WorkorderUpdateReasonRush : WorkorderUpdateReasons
    {
        public WorkorderUpdateReasonRush(int woID)
            : base(woID)
        {
        }

        public override void Update(int woID, string value)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Sales_JobMasterList_WO] SET [RushReason] = @RushReason WHERE [woID] = @woID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@woID", SqlDbType.Int).Value = woID;
                UPdateCommand.Parameters.Add("@RushReason", SqlDbType.NVarChar, 500).Value = value;
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


    public class WorkorderUpdateReasonRedo : WorkorderUpdateReasons
    {
        public WorkorderUpdateReasonRedo(int woID)
            : base(woID)
        {
        }

        public override void Update(int woID, string value)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Sales_JobMasterList_WO] SET [RedoReason] = @RedoReason WHERE [woID] = @woID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@woID", SqlDbType.Int).Value = woID;
                UPdateCommand.Parameters.Add("@RedoReason", SqlDbType.NVarChar, 500).Value = value;
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


    public class WorkorderUpdateReasonRevise : WorkorderUpdateReasons
    {
        public WorkorderUpdateReasonRevise(int woID)
            : base(woID)
        {
        }

        public override void Update(int woID, string value)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Sales_JobMasterList_WO] SET [RevisedReason] = @RevisedReason WHERE [woID] = @woID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@woID", SqlDbType.Int).Value = woID;
                UPdateCommand.Parameters.Add("@RevisedReason", SqlDbType.NVarChar, 500).Value = value;
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


    public class WorkorderChangeType
    {
        private readonly DataTable _itemTbl;
        private readonly int _woID;
        private readonly int _woType;

        public WorkorderChangeType(int woID, int woType)
        {
            _woID = woID;
            _woType = woType;
            _itemTbl = WorkorderShared.getExistingWorkorderItems(woID);
        }

        public void Change()
        {
            //1 change type

            var wt = new WorkorderFieldWoType(_woID);
            dynamic oldWoType = wt.FieldValue;
            wt.Update(_woID, _woType);

            //2 accessary
            var wda = new WorkorderDeleteAccessary(_woID);
            wda.Delete();

            var wsc = new WorkorderSitecheckPurpose(_woID);
            if (_woType == (int) NWorkorderType.Sitecheck)
            {
                wsc.Insert();
            }
            else
            {
                wsc.Delete();
            }
            //3 Items

            ChangeItemsTitleToNewItem(oldWoType);

            //4. 
            DeleteSpecialProcedures();

            UpdateSpecialProcedure();
        }


        private void ChangeItemsTitleToNewItem(int oldWoType)
        {
            if (_itemTbl != null)
            {
                foreach (DataRow row in _itemTbl.Rows)
                {
                    int woItemID = Convert.ToInt32(row["woItemID"]);
                    var iw = new ObjItemWorkorder();
                    iw.ItemID = woItemID;


                    switch (_woType)
                    {
                        case (int) NWorkorderType.Production:
                            iw.ItemRequirement = (int) NWorkorderRequirement.Installation;
                            break;
                        case (int) NWorkorderType.Service:
                            //Change to Service
                            iw.ItemNameDetailsID = 0;
                            iw.ItemNameText = "New Item";
                            iw.Update();

                            iw.ItemRequirement = (int) NWorkorderRequirement.Service;
                            break;
                        case (int) NWorkorderType.Sitecheck:
                            iw.ItemRequirement = (int) NWorkorderRequirement.SiteCheck;
                            break;
                        default:
                            break;
                    }
                }
            }
        }


        //SELECT [woID], [SpecialProcedureApplicable] FROM [Sales_JobMasterList_WO] WHERE ([woID] = @woID)

        private void UpdateSpecialProcedure()
        {
            bool b = false;

            if (_woType == (int) NWorkorderType.Production)
            {
                b = true;
            }

            UpdateSpecialProcedureValue(_woID, b);
        }


        private void UpdateSpecialProcedureValue(int woID, bool value)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Sales_JobMasterList_WO] SET [SpecialProcedureApplicable] = @SpecialProcedureApplicable WHERE [woID] = @woID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@woID", SqlDbType.Int).Value = woID;
                UPdateCommand.Parameters.Add("@SpecialProcedureApplicable", SqlDbType.Bit).Value = value;
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

        private void DeleteSpecialProcedures()
        {
            if (_itemTbl != null)
            {
                foreach (DataRow row in _itemTbl.Rows)
                {
                    int woItemID = Convert.ToInt32(row["woItemID"]);
                    var wid = new WorkorderItemDelete(woItemID);
                    wid.DeleteSpecialProcedures();
                }
            }
        }
    }

    public class WorkorderSitecheckPurpose
    {
        private readonly int _woID;

        public WorkorderSitecheckPurpose(int woID)
        {
            _woID = woID;
        }


        public void Insert()
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string InsertString =
                    "INSERT INTO [WO_Sitecheck_Purpose] ([woID], [scPurpose1], [scPurpose2], [scPurpose3], [scPurpose4], [scPurposeOther]) VALUES (@woID, @scPurpose1, @scPurpose2, @scPurpose3, @scPurpose4, @scPurposeOther)";

                // Create the command and set its properties.
                var InsertCommand = new SqlCommand(InsertString, Connection);
                try
                {
                    Connection.Open();
                    InsertCommand.Parameters.Add("@woID", SqlDbType.Int).Value = _woID;
                    InsertCommand.Parameters.Add("@scPurpose1", SqlDbType.Bit).Value = false;
                    InsertCommand.Parameters.Add("@scPurpose2", SqlDbType.Bit).Value = false;
                    InsertCommand.Parameters.Add("@scPurpose3", SqlDbType.Bit).Value = false;
                    InsertCommand.Parameters.Add("@scPurpose4", SqlDbType.Bit).Value = false;
                    InsertCommand.Parameters.Add("@scPurposeOther", SqlDbType.NVarChar, 500).Value = DBNull.Value;

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


        public void Delete()
        {
            //
            string sqlSelectString = "SELECT * FROM [WO_Sitecheck_Purpose] WHERE ([woID] = @woID)";
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            var SelectCommand1 = new SqlCommand(sqlSelectString, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@woID", SqlDbType.Int).Value = _woID;

            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                int affectedRows = adapter1.Fill(ds1, "t1");
                if (affectedRows > 0)
                {
                    foreach (DataRow row in ds1.Tables["t1"].Rows)
                    {
                        row.Delete();
                    }

                    var cb = new SqlCommandBuilder(adapter1);
                    adapter1 = cb.DataAdapter;
                    adapter1.Update(ds1, "t1");
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
    }
}