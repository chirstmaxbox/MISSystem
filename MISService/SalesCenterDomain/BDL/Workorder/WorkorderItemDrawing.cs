using System.Data;
using System.Data.SqlClient;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Workorder
{
    public class WorkorderItemDrawing
    {
        public long DrawingID { get; set; }
        public int ParentID { get; set; }

        //   public int docType = (int) DispatchingTaskEN.NDocumentType.Project;
        public string DrawingType;
        public int DrawingPurpose;
        public string DrawingHyperlink;
        public string DrawingName;
        public bool IsFinalDrawing;
        public string Note;

        public void UpdateDrawing()
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString = "UPDATE [WO_Item_Drawing] SET [DrawingType] = @DrawingType, [DrawingPurpose] = @DrawingPurpose, [DrawingName] = @DrawingName, [DrawingHyperlink] = @DrawingHyperlink, [IsFinalDrawing] = @IsFinalDrawing, [Note] = @Note WHERE [DrawingID] = @DrawingID";
    
                var UPdateCommand = new SqlCommand(UpdateString, Connection);
                UPdateCommand.Parameters.Add("@DrawingID", SqlDbType.Int).Value = DrawingID;

                UPdateCommand.Parameters.Add("@DrawingType", SqlDbType.NVarChar, 100).Value = DrawingType;
                UPdateCommand.Parameters.Add("@DrawingPurpose", SqlDbType.Int).Value = DrawingPurpose;
                UPdateCommand.Parameters.Add("@DrawingName", SqlDbType.NVarChar, 200).Value = DrawingName;
                UPdateCommand.Parameters.Add("@DrawingHyperlink", SqlDbType.NVarChar, 500).Value = DrawingHyperlink;
                UPdateCommand.Parameters.Add("@IsFinalDrawing", SqlDbType.Bit).Value = IsFinalDrawing;
                UPdateCommand.Parameters.Add("@Note", SqlDbType.NVarChar, 500).Value = Note;
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

        public void UpdateDrawing(string updateType)
        {
            if (updateType == "FileDidNotChange")
            {
                using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
                {
                    string UpdateString = "UPDATE [WO_Item_Drawing] SET [DrawingType] = @DrawingType, [DrawingPurpose] = @DrawingPurpose,  [IsFinalDrawing] = @IsFinalDrawing, [Note] = @Note WHERE [DrawingID] = @DrawingID";

                    var UPdateCommand = new SqlCommand(UpdateString, Connection);
                    UPdateCommand.Parameters.Add("@DrawingID", SqlDbType.Int).Value = DrawingID;

                    UPdateCommand.Parameters.Add("@DrawingType", SqlDbType.NVarChar, 100).Value = DrawingType;
                    UPdateCommand.Parameters.Add("@DrawingPurpose", SqlDbType.Int).Value = DrawingPurpose;
                    
                    UPdateCommand.Parameters.Add("@IsFinalDrawing", SqlDbType.Bit).Value = IsFinalDrawing;
                    UPdateCommand.Parameters.Add("@Note", SqlDbType.NVarChar, 500).Value = Note;

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

        public void Insert()
        {
            //Insert documents for items
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string InsertString = "INSERT INTO [WO_Item_Drawing] ([ParentID], [DrawingType], [DrawingPurpose], [DrawingName], [DrawingHyperlink], [IsFinalDrawing], [Note]) VALUES (@ParentID, @DrawingType, @DrawingPurpose, @DrawingName, @DrawingHyperlink, @IsFinalDrawing, @Note)";

                var InsertCommand = new SqlCommand(InsertString, Connection);

                try
                {
                    Connection.Open();
                    InsertCommand.Parameters.Add("@ParentID", SqlDbType.Int).Value = ParentID;
                    InsertCommand.Parameters.Add("@DrawingType", SqlDbType.NVarChar, 100).Value = DrawingType;
                    InsertCommand.Parameters.Add("@DrawingPurpose", SqlDbType.Int).Value = DrawingPurpose;
                    InsertCommand.Parameters.Add("@DrawingName", SqlDbType.NVarChar, 200).Value = DrawingName;
                    InsertCommand.Parameters.Add("@DrawingHyperlink", SqlDbType.NVarChar, 500).Value = DrawingHyperlink;
                    InsertCommand.Parameters.Add("@IsFinalDrawing", SqlDbType.Bit).Value = IsFinalDrawing;
                    InsertCommand.Parameters.Add("@Note", SqlDbType.NVarChar, 500).Value = Note;

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
       
    }

    public class WorkorderItemDrawingsDelete
    {
        private readonly int _parentID;
            public WorkorderItemDrawingsDelete(int parentID)
            {
                _parentID = parentID;
            }

        public void DeleteByItemID()
        {
            int rowsAffected = 0;

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString = "SELECT * FROM [WO_Item_Drawing] WHERE ([ParentID] = @parentID)";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand);
            var ds1 = new DataSet();
            //table of newly copied items, copy the documents
            ds1.Tables.Clear();

            adapter1.SelectCommand.Parameters.Add("@parentID", SqlDbType.Int).Value =_parentID;

            try
            {
                ConnectionSQL.Open();
                rowsAffected = adapter1.Fill(ds1, "t1");
                if (rowsAffected > 0)
                {
                    foreach (DataRow row in ds1.Tables["t1"].Rows)
                    {
                        row.Delete();
                    }

                    //. Write  back to DB
                    var cb = new SqlCommandBuilder(adapter1);
                    adapter1 = cb.DataAdapter;
                    rowsAffected += adapter1.Update(ds1, "t1");
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
        }
    }

    public class WorkorderItemDrawingCopy
    {
        private readonly int _parentID;

        public WorkorderItemDrawingCopy(int parentID)
        {
            _parentID = parentID;
        }

        public void CopyItemDocuments(int newParentID)
        {
            int rowsAffected = 0;
            int i = 0;
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString = "SELECT * FROM [WO_Item_Drawing] WHERE ([ParentID] = @parentID)";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand);
            var ds1 = new DataSet();
            ds1.Tables.Clear();
            adapter1.SelectCommand.Parameters.Add("@parentID", SqlDbType.Int).Value = _parentID;

            try
            {
                ConnectionSQL.Open();
                rowsAffected = adapter1.Fill(ds1, "t1");
                if (rowsAffected > 0)
                {
                    DataTable t2 = ds1.Tables["t1"].Copy();
                    t2.TableName = "t2";
                    ds1.Tables.Add(t2);
                    ds1.Tables["t2"].Clear();
                    int itemIndex = ds1.Tables["t1"].Columns.Count;

                    //3. copy each and every Rev Record to Table 2
                    //   modify special fields


                    foreach (DataRow row in ds1.Tables["t1"].Rows)
                    {
                        DataRow rowNew = ds1.Tables["t2"].NewRow();
                        for (i = 0; i <= itemIndex - 1; i++)
                        {
                            rowNew[i] = row[i];
                        }
                        rowNew["ParentID"] = newParentID;
                        ds1.Tables["t2"].Rows.Add(rowNew);
                    }

                    //. Write  back to DB
                    var cb = new SqlCommandBuilder(adapter1);
                    adapter1 = cb.DataAdapter;
                    rowsAffected += adapter1.Update(ds1, "t2");
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
        }
    }

    public class WorkorderItemDrawings
    {
        public DataRow GetValue(int drawingID)
        {
            DataRow t1 = null;
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString = "SELECT * FROM [WO_Item_Drawing] WHERE ([DrawingID] = @DrawingID)";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand);
            var ds1 = new DataSet();
            ds1.Tables.Clear();
            adapter1.SelectCommand.Parameters.Add("@DrawingID", SqlDbType.Int).Value = drawingID ;

            try
            {
                ConnectionSQL.Open();
                var rowsAffected = adapter1.Fill(ds1, "t1");
                if (rowsAffected > 0)
                {
                    t1 = ds1.Tables["t1"].Rows[0];
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
            return t1;
        }
 
        public DataTable GetValues(int parentID)
        {
            DataTable t1 = null;

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString = "SELECT * FROM [WO_Item_Drawing] WHERE ([ParentID] = @parentID)";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand);
            var ds1 = new DataSet();
            ds1.Tables.Clear();
            adapter1.SelectCommand.Parameters.Add("@parentID", SqlDbType.Int).Value = parentID;

            try
            {
                ConnectionSQL.Open();
                var rowsAffected = adapter1.Fill(ds1, "t1");
                if (rowsAffected > 0)
                {
                    t1 = ds1.Tables["t1"];
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
            return t1;
        }
    }
}