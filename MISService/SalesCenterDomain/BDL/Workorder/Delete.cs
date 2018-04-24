using System;
using System.Data;
using System.Data.SqlClient;
using MyCommon.MyEnum;
using SalesCenterDomain.BDL.Item;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Workorder
{

    #region "Delete "

    public class WorkorderDelete
    {
        private readonly int _woID;

        public WorkorderDelete(int woID)
        {
            _woID = woID;
        }

        public int WorkorderID
        {
            get { return _woID; }
        }

        public bool isDeletable()
        {
            bool b = false;
            DataRow woTitleInfo = WorkorderShared.GetWorkorderInfo(_woID);
            if (woTitleInfo != null)
            {
                int woStatus = Convert.ToInt32(woTitleInfo["woStatus"]);
                if (woStatus == (int) NJobStatus.woNew | woStatus == (int) NJobStatus.woInvalid |
                    woStatus == (int) NJobStatus.woObsolete)
                {
                    b = true;
                }
            }
            return b;
        }


        public void delete()
        {
            DataTable tbl = WorkorderShared.getExistingWorkorderItems(_woID);
            if (tbl != null)
            {
                foreach (DataRow row in tbl.Rows)
                {
                    var wi = new WorkorderItemDelete(Convert.ToInt32(row["woItemID"]));
                    wi.Delete();
                }
            }

            ReleaseRegisteredWorkorderNumber();

            var wda = new WorkorderDeleteAccessary(_woID);
            wda.Delete();

            DeleteWorkorderTitle();
        }

        private void ReleaseRegisteredWorkorderNumber()
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string delString = "DELETE FROM [Sales_JobMasterList_WO_Number] WHERE [woID] = @woID";
                var delCommand = new SqlCommand(delString, Connection);
                delCommand.Parameters.Add("@woID", SqlDbType.Int).Value = _woID;
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


        private void DeleteWorkorderTitle()
        {
            //
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string delString = "DELETE FROM [Sales_JobMasterList_WO] WHERE [woID] = @woID";
                var delCommand = new SqlCommand(delString, Connection);
                delCommand.Parameters.Add("@woID", SqlDbType.Int).Value = _woID;
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

    public class WorkorderItemDelete
    {
        private readonly int _woItemID;

        public WorkorderItemDelete(int woItemID)
        {
            _woItemID = woItemID;
        }

        public void Delete()
        {
            //        Delete Workorder Item Drawing
            var drawing = new WorkorderItemDrawingsDelete( _woItemID);
            drawing.DeleteByItemID();

            DeleteWorkorderItemTo();
            DeleteSpecialProcedures();

            DeleteSingleWorkorderItem();
        }


        private void DeleteWorkorderItemTo()
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
                    DataRow row = null;
                    foreach (DataRow row_loopVariable in ds1.Tables["t1"].Rows)
                    {
                        row = row_loopVariable;
                        row.Delete();
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

        private void DeleteSingleWorkorderItem()
        {
            //
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string delString = "DELETE FROM [WO_Item] WHERE [woItemID] = @woItemID";
                var delCommand = new SqlCommand(delString, Connection);
                delCommand.Parameters.Add("@woItemID", SqlDbType.Int).Value = _woItemID;
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


        public void DeleteSpecialProcedures()
        {
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString = "Select * From [WO_Item_SpecialProcedure] Where ([woItemID]=@woItemID)";

            var SelectCommand1 = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@woItemID", SqlDbType.Int).Value = _woItemID;

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


    public class WorkorderDeleteAccessary
    {
        private readonly int _woID;

        public WorkorderDeleteAccessary(int woID)
        {
            _woID = woID;
        }


        private void DeleteRecords(string SqlSelectString)
        {
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            var SelectCommand1 = new SqlCommand(SqlSelectString, ConnectionSQL);
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

        private void DeleteInstructions()
        {
            string SqlSelectString = "SELECT * FROM [WO_Instruction_Datatable] WHERE ([woID] = @woID)";
            DeleteRecords(SqlSelectString);
        }

        private void DeleteDocuments()
        {
            string SqlSelectString = "SELECT * FROM [WO_WORKORDER_CHECKLIST_Datatable] WHERE ([woID] = @woID)";
            DeleteRecords(SqlSelectString);
        }


        private void DeleteSignProblem()
        {
            string SqlSelectString = "SELECT * FROM [WO_SignProblem_Datatable] WHERE ([woID] = @woID)";
            DeleteRecords(SqlSelectString);
        }


        public void Delete()
        {
            DeleteInstructions();
            DeleteDocuments();
            DeleteSignProblem();
        }
    }

    #endregion
}