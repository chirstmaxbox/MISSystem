using System;
using System.Data;
using System.Data.SqlClient;
using MyCommon;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Invoice
{
    public class InvoiceWorkorder
    {
        private readonly int _invoiceID;
        private readonly DataTable _tblWo;
        private MyKeyValuePair[] _newWorkorders;
        private string _swo = "";
        private int _woID;


        public InvoiceWorkorder(int invoiceID)
        {
            _invoiceID = invoiceID;
            _tblWo = GetExistingWorkorders();
        }

        public InvoiceWorkorder(int invoiceID, string workorderNumber)
        {
            _invoiceID = invoiceID;
            _swo = workorderNumber;
        }

        public string[] ExistingWorkorders
        {
            get
            {
                int i = 0;
                if (_tblWo != null)
                {
                    int count = _tblWo.Rows.Count;
                    var woList = new string[count + 1];

                    foreach (DataRow row in _tblWo.Rows)
                    {
                        woList[i] = Convert.ToString(row["WorkorderNumber"]);
                        i += 1;
                    }
                    return woList;
                }
                else
                {
                    string[] woList = {"Nothing"};
                    return woList;
                }
            }
        }


        public MyKeyValuePair[] NewWorkorders
        {
            set { _newWorkorders = value; }
        }


        private DataTable GetExistingWorkorders()
        {
            DataTable tbl = null;

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 =
                "SELECT * FROM dbo.Sales_JobMasterList_Invoice_Workorder WHERE (invoiceID = @invoiceID)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@invoiceID", SqlDbType.Int).Value = _invoiceID;

            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                int rowsAffected = adapter1.Fill(ds1, "t1");

                //Destination

                if (rowsAffected > 0)
                {
                    tbl = ds1.Tables["t1"];
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

            return tbl;
        }


        private void Create()
        {
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            using (ConnectionSQL)
            {
                string InsertString =
                    "INSERT INTO [Sales_JobMasterList_Invoice_Workorder] ([InvoiceID], [WorkorderNumber], [woID]) VALUES (@InvoiceID, @WorkorderNumber,@woID)";
                var InsertCommand = new SqlCommand(InsertString, ConnectionSQL);
                InsertCommand.Parameters.Add("@invoiceID", SqlDbType.Int).Value = _invoiceID;
                InsertCommand.Parameters.Add("@WorkorderNumber", SqlDbType.NVarChar, 15).Value = _swo;
                InsertCommand.Parameters.Add("@woID", SqlDbType.Int).Value = _woID;

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


        public void SyncWorkorderList()
        {
            //1. if workorder.existing(tbl) does in workorder.New (array) then delete
            int i = 0;
            int count = _newWorkorders.Length;

            if (_tblWo != null)
            {
                foreach (DataRow row in _tblWo.Rows)
                {
                    string swo = Convert.ToString(row["WorkorderNumber"]);
                    bool toDelete = true;
                    for (i = 0; i <= count - 1; i++)
                    {
                        if (!MyConvert.IsNullString(_newWorkorders[i]))
                        {
                            if (_newWorkorders[i].Value == swo)
                            {
                                toDelete = false;
                            }
                        }
                    }

                    if (toDelete)
                    {
                        int id = GetID(swo);
                        DeleteRecord(id);
                    }
                }
            }


            //2. if New does not in existing then Create


            for (i = 0; i <= count - 1; i++)
            {
                bool toCreate = true;

                if (!MyConvert.IsNullString(_newWorkorders[i]))
                {
                    string swo = _newWorkorders[i].Value;
                    if (_tblWo != null)
                    {
                        foreach (DataRow row in _tblWo.Rows)
                        {
                            if (Convert.ToString(row["WorkorderNumber"]) == swo)
                            {
                                toCreate = false;
                            }
                        }
                    }

                    if (toCreate)
                    {
                        _swo = swo;
                        _woID = Convert.ToInt32(_newWorkorders[i].Key);
                        Create();
                    }
                }
            }
        }


        private int GetID(string workorderNumber)
        {
            int ID = 0;

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 =
                "SELECT * FROM dbo.Sales_JobMasterList_Invoice_Workorder WHERE (invoiceID = @invoiceID) AND (WorkorderNumber=@WorkorderNumber)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@invoiceID", SqlDbType.Int).Value = _invoiceID;
            adapter1.SelectCommand.Parameters.Add("@WorkorderNumber", SqlDbType.NChar, 15).Value = workorderNumber;
            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                int rowsAffected = adapter1.Fill(ds1, "t1");

                //Destination

                if (rowsAffected > 0)
                {
                    DataRow row = ds1.Tables["t1"].Rows[0];

                    ID = Convert.ToInt32(row["ID"]);
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

            return ID;
        }

        private void DeleteRecord(int myID)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string delString = "DELETE FROM  [Sales_JobMasterList_Invoice_Workorder] WHERE ([ID] = @ID)";
                var delCommand = new SqlCommand(delString, Connection);
                delCommand.Parameters.Add("@ID", SqlDbType.Int).Value = myID;
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

        public void DeleteAllWorkorders()
        {
            if (_tblWo != null)
            {
                foreach (DataRow row in _tblWo.Rows)
                {
                    int myID = Convert.ToInt32(row["ID"]);
                    DeleteRecord(myID);
                }
            }
        }
    }
}