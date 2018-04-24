using System;
using System.Data;
using System.Data.SqlClient;
using SpecDomain.BO;

namespace SpecDomain.BLL.EstTitle
{

    public class SpecProjectCompany
    {

        //Quote, workorder, invoice check to these items
        private bool _isThereAInstallOrShipToCompany;
        private bool _isThereAQuotetoCompany;
        private bool _isThereABilltoCompany;


        public bool IsThereAnInstallToCompany
        {
            get { return _isThereAInstallOrShipToCompany; }
        }

        public bool IsThereAQuotetoCompany
        {
            get { return _isThereAQuotetoCompany; }
        }


        public bool IsThereABilltoCompany
        {
            get { return _isThereABilltoCompany; }
        }

        
        public SpecProjectCompany(int jobID)
        {
        
            var t1 = GetProjectCustomersInfo(jobID);
            Initialization(t1);

        }


        private DataTable GetProjectCustomersInfo(int jobID)
        {
            int affectedRows = 0;

            var ConnectionSQL = new SqlConnection(SpecConfiguration.ConnectionString);
            string SqlSelectString = "";
            SqlSelectString = "SELECT * FROM View_jobCustomer_AllRoles WHERE (jobID = @jobID)";

            SqlCommand SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            SqlDataAdapter adapter1 = new SqlDataAdapter(SelectCommand);
            DataSet ds1 = new DataSet();
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
            _isThereAInstallOrShipToCompany = false;
            _isThereAQuotetoCompany = false;
            _isThereABilltoCompany = false;
            if (t1 == null) return;
            foreach (DataRow row in t1.Rows)
            {
                if (Convert.IsDBNull(row["cID"])) continue;

                if (Convert.ToBoolean( row["isInstallOrShipTo"]))
                {
                    _isThereAInstallOrShipToCompany = true;
                }

                if( Convert.ToBoolean( row["isQuoteTo"]))
                {
                    _isThereAQuotetoCompany = true;
                }

                if (Convert.ToBoolean( row["isBillTo"]))
                {
                    _isThereABilltoCompany = true;
                }
            }
        }
 


       
    }


}