using System;
using System.Data;
using System.Data.SqlClient;
using CustomerDomain.BO;

namespace CustomerDomain.BDL
{
    public class CustomerProjectDetail
    {
        public string ProjectNumber { get { return GetProjectNumber(); } }
        public string JobTitle { get { return GetJobTitle(); } }

        private readonly DataRow _row;
        public CustomerProjectDetail (int jobID)
        {
            _row = GetProjectDataRow(jobID);
        }

        private string GetProjectNumber()
        {
            var jobNumber = "11P00000";
            if (_row != null)
            {
                if (!Convert.IsDBNull(_row["jobNumber"]))
                {
                    jobNumber = Convert.ToString(_row["jobNumber"]);
                }
            }
            return jobNumber;

        }

        private  string GetJobTitle()
        {

            var jobTitle = "DoesNotExist";
             
            if (_row != null)
            {
                if (!Convert.IsDBNull(_row["jobTitle"]))
                {
                    jobTitle = Convert.ToString(_row["jobTitle"]);
                }
            }
            return jobTitle;
            
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

    }
}