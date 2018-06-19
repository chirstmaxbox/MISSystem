using System;
using System.Data;
using System.Data.SqlClient;

namespace ProjectSummaryDomain
{

    public class MyProjectDetail
    {
        
        private readonly DataRow _row;
        private readonly int _jobID;

        public MyProjectDetail(int jobID)
        {
            _jobID = jobID;
            _row = GetProjectDataRow(jobID);

        }

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
                    }
                }
                return jobNumber;
            }
        }


        public string Description
        {
            get
            {
                var str = "";
                if (_row != null)
                {
                    if (!Convert.IsDBNull(_row["Description"]))
                    {
                        str = Convert.ToString(_row["Description"]);
                    }
                }
                return str;
            }
        }

        public int EstimationRevID
        {
            get
            {
                var i = 0;
                if (_row!=null)
                {
                    i=ConvertToInteger( _row["EstimationRevID"]);
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
                    sales =ConvertToInteger(_row["sales"]);
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
                    sa1ID = ConvertToInteger(_row["sa1ID"]);
                }
                return sa1ID;
            }
        }

        private DataRow GetProjectDataRow(int jobID)
        {
            //tbl Sales_JobMasterList
            DataRow row = null;

            var connectionSQL = new SqlConnection(MyConfiguration.ConnectionString);
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

        private int ConvertToInteger(object str)
        {
            int i = 0;
            if (str != null)
            {
                var s = Convert.ToString(str);
                if (!Convert.IsDBNull(str))
                {
                    int number;
                    var b = int.TryParse(s, out number);
                    if (b)
                    {
                        i = number;
                    }
                }
            }
            return i;
        }

    }

}
