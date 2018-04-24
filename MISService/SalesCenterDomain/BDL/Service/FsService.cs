using System;
using System.Data;
using System.Data.SqlClient;
using MyCommon;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Service
{
    public class FsService
    {
        //     Case "Quote"
        //         _recordType = "Q"
        //     Case "Invoice"
        //         _recordType = "I"
        //     Case "Est"
        //         _recordType = "E"


        private readonly int _serviceParentID;
        private readonly string _serviceType;


        public FsService(int parentID, string serviceType)
        {
            _serviceParentID = parentID;
            _serviceType = serviceType;
        }

        #region "Insert"

        public void InsertServices()
        {
            var sd = new ServiceDefinition(ServiceGroup, 0);
            DataTable t1 = sd.Tbl;
            if (t1 == null) return;
            int printOrder = GetQsMaxPrintOrder() + 1;

            try
            {
                foreach (DataRow row in t1.Rows)
                {
                    InsertRecord(Convert.ToInt32(row["PC_ID"]),
                                 MyConvert.ConvertToString(row["Charge"]),
                                 1,
                                 MyConvert.ConvertToString(row["CONTENTS"]),
                                 MyConvert.ConvertToString(row["PS_NAME"]),
                                 MyConvert.ConvertToString(row["Charge"]),
                                 printOrder
                        );
                }
                t1.Clear();
            }
            catch (SqlException ex)
            {
                string errorLog = ex.Message;
            }
            finally
            {
            }
        }

        //Insert New
        private void InsertRecord(int qsServiceID, string qsAmount, int qsQty, string qsDescription, string qsTitle,
                                  string qsAmountText, int printNumber)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string InsertString =
                    "INSERT INTO [FW_QUOTE_SERVICE] ([estRevID], [qsPrintOrder], [qsServiceID],[qsAmount], [qsQty],[recordType],[qsDescription],[qsTitle],[qsAmountText]) VALUES (@estRevID, @qsPrintOrder, @qsServiceID, @qsAmount, @qsQty, @recordType,@qsDescription,@qsTitle, @qsAmountText)";
                // Create the command and set its properties.
                var InsertCommand = new SqlCommand(InsertString, Connection);
                try
                {
                    //@estRevID, @qsPrintOrder, @qsServiceID, @qsAmount, @qsQty, @recordType)"
                    InsertCommand.Parameters.Add("@estRevID", SqlDbType.Int).Value = _serviceParentID;
                    InsertCommand.Parameters.Add("@qsPrintOrder", SqlDbType.SmallInt).Value = printNumber;
                    InsertCommand.Parameters.Add("@qsServiceID", SqlDbType.SmallInt).Value = qsServiceID;
                    InsertCommand.Parameters.Add("@qsAmount", SqlDbType.NVarChar, 100).Value = qsAmount;
                    InsertCommand.Parameters.Add("@qsQty", SqlDbType.SmallInt).Value = qsQty;
                    InsertCommand.Parameters.Add("@recordType", SqlDbType.NChar, 7).Value = _serviceType;
                    InsertCommand.Parameters.Add("@qsDescription", SqlDbType.NVarChar, 300).Value = qsDescription;
                    InsertCommand.Parameters.Add("@qsTitle", SqlDbType.NVarChar, 300).Value = qsTitle;

                    InsertCommand.Parameters.Add("@qsAmountText", SqlDbType.NVarChar, 300).Value = qsAmountText;


                    Connection.Open();
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


        private int GetQsMaxPrintOrder()
        {
            int max = 0;
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 =
                "SELECT MAX(qsPrintOrder) AS qsPrintOrder FROM FW_QUOTE_SERVICE WHERE (estRevID = @estRevID) AND (recordType = @recordType)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@estRevID", SqlDbType.Int).Value = _serviceParentID;
            adapter1.SelectCommand.Parameters.Add("@recordType", SqlDbType.NChar, 7).Value = _serviceType;
            var ds1 = new DataSet();

            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                int numRowsAffected = adapter1.Fill(ds1, "t1");
                if (numRowsAffected != 0)
                {
                    //Destination
                    DataRow row = ds1.Tables["t1"].Rows[0];
                    if (!Convert.IsDBNull(row["qsPrintOrder"]))
                    {
                        max = Convert.ToInt32(row["qsPrintOrder"]);
                    }
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
            return max;
        }

        #endregion

        #region "Delete"

        public void DeleteServices()
        {
            string errorLog = "Nothing";
            int NumRowsAffected = 0;

            //2. Define the destination: InvoiceItem
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 =
                "SELECT * FROM [FW_QUOTE_SERVICE] WHERE ([estRevID] = @estRevID and [recordType]=@recordType)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@estRevID", SqlDbType.Int).Value = _serviceParentID;
            adapter1.SelectCommand.Parameters.Add("@recordType", SqlDbType.NChar, 7).Value = _serviceType;
            var ds1 = new DataSet();

            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                NumRowsAffected = adapter1.Fill(ds1, "t1");

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

        #endregion

        #region "Copy"

        public void CopyServices(int NewParentID)
        {
            if (NewParentID > 0)
            {
                //Copy to a new revision
            }
            else
            {
                //Copy to itself, reserve
            }

            string errorLog = "Nothing";
            int NumRowsAffected = 0;

            //2. Define the destination: InvoiceItem
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 =
                "SELECT * FROM [FW_QUOTE_SERVICE] WHERE ([estRevID] = @estRevID and [recordType]=@recordType)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@estRevID", SqlDbType.Int).Value = _serviceParentID;
            adapter1.SelectCommand.Parameters.Add("@recordType", SqlDbType.NChar, 7).Value = _serviceType;
            var ds1 = new DataSet();

            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                NumRowsAffected = adapter1.Fill(ds1, "t1");
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

                        if (NewParentID > 0)
                        {
                            rowNew["estRevID"] = NewParentID;
                        }
                        else
                        {
                            rowNew["estRevID"] = _serviceParentID;
                        }

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

        #endregion

        public int ServiceGroup { get; set; }
    }
}