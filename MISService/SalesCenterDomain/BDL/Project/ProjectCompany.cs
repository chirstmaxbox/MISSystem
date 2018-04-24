using System;
using System.Data;
using System.Data.SqlClient;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Project
{
    public class ProjectCompany
    {
        private readonly DataTable _t1;

        #region "Properties"

        //
        private int _billToCompanyID;
        private int _billToContactID;
        //Total company count for this project, check if this is the last record of this project, at least one required

        private int _count;
        private int _installToCompanyID;
        private int _installToContactID;
        private bool _isThereABilltoCompany;
        private bool _isThereAInstallOrShipToCompany;
        private bool _isThereAQuotetoCompany;
        private int _jobID;
        private int _quoteToCompanyID;
        private int _quoteToContactID;
        public int NewJobID { get; set; }


        public bool isThereAInstallOrShipToCompany
        {
            get { return _isThereAInstallOrShipToCompany; }
        }

        public bool isThereAQuotetoCompany
        {
            get { return _isThereAQuotetoCompany; }
        }


        public bool isThereABilltoCompany
        {
            get { return _isThereABilltoCompany; }
        }


        public int count
        {
            get { return _count; }
        }

        public int InstallToCompanyID
        {
            get { return _installToCompanyID; }
        }

        public int InstallToContactID
        {
            get { return _installToContactID; }
        }

        public int QuoteToCompanyID
        {
            get { return _quoteToCompanyID; }
        }

        public int QuoteToContactID
        {
            get { return _quoteToContactID; }
        }


        public int BillToCompanyID
        {
            get { return _billToCompanyID; }
        }


        public int BillToContactID
        {
            get { return _billToContactID; }
        }

        #endregion

        #region "Construction"

      //  public bool IsAllowToAddNewRow { get; private set; }


        public ProjectCompany(int jobID)
        {
            _jobID = jobID;

            _t1 = getProjectCustomersInfo(jobID);

            Initialization(_t1);
            
     //       IsAllowToAddNewRow = GetIsAllowToAddNewRow(_t1);

        }

        //Select
        private DataTable getProjectCustomersInfo(int jobID)
        {
            //CompanyType=Client

            //
            //1. Read the informations needed

            int affectedRows = 0;


            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString = "";
            SqlSelectString = "SELECT * FROM View_jobCustomer_AllRoles WHERE (jobID = @jobID)";

            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand);
            var ds1 = new DataSet();
            adapter1.SelectCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = jobID;

            try
            {
                ConnectionSQL.Open();
                ds1.Tables.Clear();
                affectedRows = adapter1.Fill(ds1, "t1");
            }
            catch (SqlException ex)
            {
                string errorLog = ex.Message;
            }
            finally
            {
                ConnectionSQL.Close();
            }

            if (affectedRows == 0)
            {
                return null;
            }
            else
            {
                return ds1.Tables["t1"];
            }
        }


        private void Initialization(DataTable t1)
        {
            _count = 0;
            if (t1 != null)
            {
                _count = t1.Rows.Count;

                _isThereAInstallOrShipToCompany = false;
                _isThereAQuotetoCompany = false;
                _isThereABilltoCompany = false;

                foreach (DataRow row in t1.Rows)
                {
                    if (Convert.IsDBNull(row["cID"])) continue;
                    int cID = Convert.ToInt32(row["cID"]);
                    int contactID = Convert.ToInt32(row["contactName"]);

                    if (Convert.ToBoolean(row["isInstallOrShipTo"]))
                    {
                        _isThereAInstallOrShipToCompany = true;
                        _installToCompanyID = cID;
                        _installToContactID = contactID;
                    }

                    if (Convert.ToBoolean(row["isQuoteTo"]))
                    {
                        _isThereAQuotetoCompany = true;
                        _quoteToCompanyID = cID;
                        _quoteToContactID = contactID;
                    }

                    if (Convert.ToBoolean(row["isBillTo"]))
                    {
                        _isThereABilltoCompany = true;
                        _billToCompanyID = cID;
                        _billToContactID = contactID;
                    }
                }
            }
        }


        private bool GetIsAllowToAddNewRow(DataTable t1)
        {
            if (t1 == null) return true;

                foreach (DataRow row in t1.Rows)
                {
                    var b1 = Convert.ToBoolean(row["isInstallOrShipTo"]);
                    var b2 = Convert.ToBoolean(row["isQuoteTo"]);
                    var b3 = Convert.ToBoolean(row["isBillTo"]);
                    var b = b1 | b2 | b3;
                    //at least one was checked
                    if (b == false) return false;
                }
            return true;
        }


        #endregion

        #region "Replace Install/Qutoe/Bill To"

        //InstallTo
        public void ReplaceInstallTo(int customerID)
        {
            foreach (DataRow row in _t1.Rows)
            {
                int tempID = Convert.ToInt32(row["cID"]);
                if (Convert.ToInt32(row["cID"]) != customerID)
                {
                    int jcID = Convert.ToInt32(row["jcID"]);
                    UpdateFields_IsInstallTo(jcID, false);
                }
            }
        }

        private void UpdateFields_IsInstallTo(int jcID, bool value)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Sales_JobMasterList_Customer] SET [isInstallOrShipTo] = @isInstallTo WHERE [jcID] = @jcID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@jcID", SqlDbType.Int).Value = jcID;
                UPdateCommand.Parameters.Add("@isInstallTo", SqlDbType.Bit).Value = value;
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

        //Qutoe To

        public void ReplaceQuoteTo(int customerID)
        {
            foreach (DataRow row in _t1.Rows)
            {
                int cID = Convert.ToInt32(row["cID"]);

                if (cID != customerID)
                {
                    int jcID = Convert.ToInt32(row["jcID"]);
                    UpdateFields_IsQuoteTo(jcID, false);
                }
            }
        }

        private void UpdateFields_IsQuoteTo(int jcID, bool value)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Sales_JobMasterList_Customer] SET [isQuoteTo] = @isQuoteTo WHERE [jcID] = @jcID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@jcID", SqlDbType.Int).Value = jcID;
                UPdateCommand.Parameters.Add("@isQuoteTo", SqlDbType.Bit).Value = value;
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

        //Bill To

        public void ReplaceBillTo(int customerID)
        {
            foreach (DataRow row in _t1.Rows)
            {
                int cID = Convert.ToInt32(row["cID"]);
                if (cID != customerID)
                {
                    int jcID = Convert.ToInt32(row["jcID"]);
                    UpdateFields_IsBillTo(jcID, false);
                }
            }
        }

        private void UpdateFields_IsBillTo(int jcID, bool value)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Sales_JobMasterList_Customer] SET [isBillTo] = @isBillTo WHERE [jcID] = @jcID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@jcID", SqlDbType.Int).Value = jcID;
                UPdateCommand.Parameters.Add("@isBillTo", SqlDbType.Bit).Value = value;
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

        public void Insert(int jobID, int CustomerID, bool isInstallTo, bool isQuoteTo, bool isBillTo)
        {
            //        
            string errorLog = "OK";
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string InsertString =
                    "INSERT INTO [Sales_JobMasterList_Customer] ([jobID], [cID], [isInstallOrShipTo],[isQuoteTo],[isBillTo]) VALUES (@jobID, @cID, @isInstallOrShipTo, @isQuoteTo, @isBillTo)";
                // Create the command and set its properties.
                var InsertCommand = new SqlCommand(InsertString, Connection);
                try
                {
                    Connection.Open();
                    InsertCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = jobID;
                    InsertCommand.Parameters.Add("@cID", SqlDbType.Int).Value = CustomerID;
                    InsertCommand.Parameters.Add("@isInstallOrShipTo", SqlDbType.Bit).Value = isInstallTo;
                    InsertCommand.Parameters.Add("@isQuoteTo", SqlDbType.Bit).Value = isQuoteTo;
                    InsertCommand.Parameters.Add("@isBillTo", SqlDbType.Bit).Value = isBillTo;

                    InsertCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    errorLog = ex.Message;
                }
                finally
                {
                    Connection.Close();
                }
            }
        }

        public void ReplaceAddNew(int customerID)
        {
            if (_t1 != null)
            {
                foreach (DataRow row in _t1.Rows)
                {
                    if (Convert.ToInt32(row["cID"]) < 1000)
                    {
                        int jcID = Convert.ToInt32(row["jcID"]);
                        UpdateCustomerID(jcID, customerID);
                    }
                }
            }
        }

        public void UpdateCustomerID(int jcID, int customerID)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString = "UPDATE [Sales_JobMasterList_Customer] SET [cID] = @cID WHERE [jcID] = @jcID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@jcID", SqlDbType.Int).Value = jcID;
                UPdateCommand.Parameters.Add("@cID", SqlDbType.Int).Value = customerID;
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

        #endregion
    }
}