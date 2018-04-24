using System;
using System.Data;
using System.Data.SqlClient;
using CustomerDomain.BO;

namespace CustomerDomain.BDL
{
    public class CustomerDelete
    {
        private int _errorCode = -1;
     
        public bool IsDeletable
        {
            get
            {
                _errorCode = GetErrorCode();
                if (_errorCode < 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private int GetErrorCode()
        {
            int ec = -1;
            DataTable tbl = GetProjectCustomers();
            if (tbl != null)
            {
                ec =(int) NErrorCode.Project;
                ErrorDefine[(int)NErrorCode.Project] = ErrorDefine[(int)NErrorCode.Project] + GetExistingProjectNumbers(tbl);

            }

            tbl = GetWorkorderCustomers();
            if (tbl != null)
            {
                ec =(int) NErrorCode.Workorder;
                ErrorDefine[(int)NErrorCode.Workorder] = ErrorDefine[(int)NErrorCode.Workorder] + GetExistingWorkorderNumbers(tbl);

            }

            tbl = GetInvoiceCustomer();
            if (tbl != null)
            {
                ec =(int) NErrorCode.Invoice;
                ErrorDefine[(int)NErrorCode.Invoice] = ErrorDefine[(int)NErrorCode.Invoice] + GetExistingInvoiceNumbers(tbl);

            }

            return ec;

        }

        public string ErrorText
        {
            get { return ErrorDefine[_errorCode]; }
        }

        string[] ErrorDefine = {
                                   "Delete failed, the customer is in Project: ",
                                   "Delete failed, the customer is in Work order: ",
                                   "Delete failed, the customer is in Invopice: "
                               };

        private enum NErrorCode : int
        {
            Project = 0,
            Workorder = 1,
            Invoice = 2
        }

        
        private int _rowID = 0;
        public CustomerDelete(int customerID)
        {
            _rowID = customerID;
        }


        private DataTable GetProjectCustomers()
        {
            DataTable tbl = null;
            SqlConnection ConnectionSQL = new SqlConnection(CustomerDomainConfiguration.ConnectionString);
            string SqlSelectString = "SELECT * FROM Sales_JobMasterList_Customer WHERE (cID=@cID)";
            SqlCommand SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            SqlDataAdapter adapter1 = new SqlDataAdapter(SelectCommand);
            adapter1.SelectCommand.Parameters.Add("@cID", SqlDbType.Int).Value = _rowID;

            DataSet ds1 = new DataSet();
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
                string errorLog = ex.Message;

            }
            finally
            {
                ConnectionSQL.Close();
            }
            return tbl;
        }

        private DataTable GetWorkorderCustomers()
        {
            DataTable tbl = null;
            SqlConnection ConnectionSQL = new SqlConnection(CustomerDomainConfiguration.ConnectionString);
            string SqlSelectString1 = "SELECT * FROM [Sales_JobMasterList_WO] WHERE ([Company1] = @CustomerID or [Company2] = @CustomerID)";
            SqlCommand SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            SqlDataAdapter adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@CustomerID", SqlDbType.Int).Value = _rowID;
            DataSet ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                int NumRowsAffected = adapter1.Fill(ds1, "t1");
                if (NumRowsAffected != 0)
                {
                    tbl = ds1.Tables["t1"];
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

            return tbl;

        }

        private DataTable GetInvoiceCustomer()
        {

            DataTable tbl = null;

            SqlConnection ConnectionSQL = new SqlConnection(CustomerDomainConfiguration.ConnectionString);
            string SqlSelectString1 = "SELECT * FROM [Sales_JobMasterList_Invoice] WHERE ([CustomerID] = @CustomerID)";
            SqlCommand SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            SqlDataAdapter adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@CustomerID", SqlDbType.Int).Value = _rowID;
            DataSet ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                int NumRowsAffected = adapter1.Fill(ds1, "t1");
                if (NumRowsAffected > 0)
                {
                    tbl = ds1.Tables["t1"];
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

            return tbl;

        }

        private string GetExistingProjectNumbers(DataTable tbl)
        {
            string s = "";
            foreach (DataRow row in tbl.Rows)
            {
                int jobID =Convert .ToInt32(  row["jobID"]);
                s += GetProjectNumber( jobID) + ", ";
            }

            return s;

        }

        private string GetProjectNumber(int jobID)
        {
            var jobNumber = "11P00000";
            var row = GetProjectDataRow(jobID);
            if (row != null)
            {
                if (!Convert.IsDBNull(row["jobNumber"]))
                {
                    jobNumber = Convert.ToString(row["jobNumber"]);
                }
            }
            return jobNumber;
  
        }

        private DataRow GetProjectDataRow(int jobID)
        {
            //tbl Sales_JobMasterList
            DataRow row = null;

            SqlConnection ConnectionSQL = new SqlConnection(CustomerDomainConfiguration.ConnectionString);
            string SqlSelectString = "SELECT * from Sales_JobMasterList WHERE (jobID= @jobID)";
            SqlCommand SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            SqlDataAdapter adapter1 = new SqlDataAdapter(SelectCommand);
            DataSet ds1 = new DataSet();
            adapter1.SelectCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = jobID;

            try
            {
                ConnectionSQL.Open();
                ds1.Tables.Clear();
                int affectedRows = adapter1.Fill(ds1, "t1");
                if (affectedRows > 0)
                {
                    row = ds1.Tables["t1"].Rows[0];
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

            return row;

        }


        private string GetExistingWorkorderNumbers(DataTable tbl)
        {
            string s = "";
            foreach (DataRow row in tbl.Rows)
            {
                s += row["WorkorderNumber"] + ", ";
            }
            return s;
        }


        private string GetExistingInvoiceNumbers(DataTable tbl)
        {
            string s = "";
            foreach (DataRow row in tbl.Rows)
            {
                s += row["InvoiceNo"] + ", ";
            }
            return s;
        }

     
    }
}