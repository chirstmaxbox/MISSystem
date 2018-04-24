using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using MyCommon;
using SpecDomain.BO;

namespace SpecDomain.BLL.EstTitle
{
    public class SpecProjectDetail
    {
        //
        public MyKeyValuePair Value { get { return GetValue(); } }

        public string JobTitle
        {
            get
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
        }

        public string JobNumber
        {
            get
            {
                var jobNumber = "11P00000";
                if (_row != null)
                {
                    if (!Convert.IsDBNull(_row["jobNumber"]))
                    {
                        jobNumber = Convert.ToString(_row["jobNumber"]);
                        var subProject = MyConvert.ConvertToInteger(_row["subProject"]);
                        if (subProject >0)
                        {
                            jobNumber += "-" + subProject;
                        }
                    }
                }
                return jobNumber;
            }
        }
  
        public int EstimationRevID
        {
            get
            {
                var i = 0;
                if (_row!=null)
                {
                    i=MyConvert.ConvertToInteger( _row["EstimationRevID"]);
                }
                return i;
            }
        }

        public int Sales
        {
            get
            {
                var sales = 0;
                if (_row != null)
                {
                    sales = MyConvert.ConvertToInteger(_row["sales"]);
                }

                return sales;
            }
        }

        public int Sa1ID
        {
            get
            {
                var sa1ID = 0;
                if (_row != null)
                {
                    sa1ID = MyConvert.ConvertToInteger(_row["sa1ID"]);
                }
                return sa1ID;
            }
        }

        public bool IsBidTo
        {
            get
            {
                return (bool)_row["IsBidToProject"];
            }
        }

        public string TargetDate
        {
            get
            {
                return MyConvert.ConvertNullableDateToString(_row["TargetDate"]);
            }
        }
        
        public int LastEstItemID
        {
            get
            {
                var i = 0;
                if (_row!=null)
                {
                    i=MyConvert.ConvertToInteger( _row["LastEstItemID"]);
                }
                return i;
            }

            set
            {
                UpdateLastEstItemID(value);
            }

        }


        private readonly DataRow _row;
        private readonly int _jobID;

        public SpecProjectDetail(int jobID)
        {
            _jobID = jobID;
            _row = GetProjectDataRow(jobID);
        }

       public SpecProjectDetail(string projectNumber)
        {
            _row = GetProjectDataRow(projectNumber);
            if (_row == null) return;
           _jobID = Convert.ToInt32(_row["jobID"]);
        }

        private MyKeyValuePair GetValue()
        {
           var  kv = new MyKeyValuePair()
            {
                Value = "Project Not Find",
                Key = 0
            };

            if (_row == null) return kv;

            var jobNumber = Convert.ToString(_row["jobNumber"]);
            var subProject = MyConvert.ConvertToInteger(_row["subProject"]);
            if (subProject > 0)
            {
                jobNumber += "-" + subProject;
            }
            
            kv.Value = jobNumber.ToUpper() + " - " + Convert.ToString(_row["jobTitle"]);
            kv.Key = Convert.ToInt32(_row["jobID"]);
            return kv;
        }

        private void UpdateLastEstItemID(int lastEstItemID)
        {
            using (var connection = new SqlConnection(SpecConfiguration.ConnectionString))
            {
                const string updateString = "UPDATE [Sales_JobMasterList] SET [LastEstItemID] = @lastEstItemID WHERE [jobID] = @jobID";
                var uPdateCommand = new SqlCommand(updateString, connection);

                uPdateCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = _jobID;
                uPdateCommand.Parameters.Add("@lastEstItemID", SqlDbType.Int).Value = lastEstItemID;
                try
                {
                    connection.Open();
                    uPdateCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    string errorlog = ex.Message;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private DataRow GetProjectDataRow(int jobID)
        {
            //tbl Sales_JobMasterList
            DataRow row = null;

            var connectionSQL = new SqlConnection(SpecConfiguration.ConnectionString);
            const string sqlSelectString = "SELECT * from Sales_JobMasterList WHERE (jobID= @jobID)";
            var selectCommand = new SqlCommand(sqlSelectString, connectionSQL);
            var adapter1 = new SqlDataAdapter(selectCommand);
            var ds1 = new DataSet();
            adapter1.SelectCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = jobID;

            try
            {
                connectionSQL.Open();
                ds1.Tables.Clear();
                var affectedRows = adapter1.Fill(ds1, "t1");
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
                connectionSQL.Close();
            }

            return row;

        }
   
        private DataRow GetProjectDataRow(string projectNumber)
        {
            //tbl Sales_JobMasterList
            DataRow row = null;
            var connectionSQL = new SqlConnection(SpecConfiguration.ConnectionString);
            const string sqlSelectString = "SELECT * from Sales_JobMasterList WHERE (JobNumber= @jobNumber and Subproject=0)";
            var selectCommand = new SqlCommand(sqlSelectString, connectionSQL);
            var adapter1 = new SqlDataAdapter(selectCommand);
            var ds1 = new DataSet();
            adapter1.SelectCommand.Parameters.Add("@jobNumber", SqlDbType.NChar, 20).Value = projectNumber;

            try
            {
                connectionSQL.Open();
                ds1.Tables.Clear();
                var affectedRows = adapter1.Fill(ds1, "t1");
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
                connectionSQL.Close();
            }

            return row;

        }

    
    }


}
