using System;
using System.Data;
using System.Data.SqlClient;
using MyCommon.MyEnum;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Project
{
    public class ProjectField
    {
        private readonly int _jobID;

        public ProjectField(int jobID)
        {
            _jobID = jobID;
        }


        public DataRow GetProjectDataRow()
        {
            //tbl Sales_JobMasterList
            DataRow row = null;

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString = "SELECT * from Sales_JobMasterList WHERE (jobID= @jobID)";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand);
            var ds1 = new DataSet();
            adapter1.SelectCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = _jobID;

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


        public void UpdateIsBidToProject(bool isBidToProject)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString ="UPDATE [Sales_JobMasterList] SET [isBidToProject] = @isBidToProject, [SalesType]=@SalesType WHERE [jobID] = @jobID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = _jobID;
                UPdateCommand.Parameters.Add("@isBidToProject", SqlDbType.Bit).Value = isBidToProject;
                var salesType = NCommissionProjectCategoryID.House;
                if (isBidToProject )
                {
                    salesType = NCommissionProjectCategoryID.Bid;
                }
                UPdateCommand.Parameters.Add("@SalesType", SqlDbType.Int).Value =salesType;

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


        public void UpdateReasonOfNoContract(string value)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Sales_JobMasterList] SET [ReasonOfNoContract] = @Value WHERE [jobID] = @jobID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = _jobID;
                UPdateCommand.Parameters.Add("@Value", SqlDbType.NVarChar, 500).Value = value;

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


        public void UpdateReasonOfNoWorkorder(string value)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Sales_JobMasterList] SET [ReasonOfNoWorkorder] = @Value WHERE [jobID] = @jobID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = _jobID;
                UPdateCommand.Parameters.Add("@Value", SqlDbType.NVarChar, 500).Value = value;

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


        public void UpdateCustomerID(int customerID)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString = "UPDATE [Sales_JobMasterList] SET [Customer] = @Customer WHERE [jobID] = @jobID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = _jobID;
                UPdateCommand.Parameters.Add("@Customer", SqlDbType.Int).Value = customerID;
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


        public void UpdateRush(bool value)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                try
                {
                    Connection.Open();
                    string UpdateString = "UPDATE [Sales_JobMasterList] SET [Rush]=@Rush WHERE [jobID] = @jobID";
                    var UPdateCommand = new SqlCommand(UpdateString, Connection);

                    UPdateCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = _jobID;
                    UPdateCommand.Parameters.Add("@Rush", SqlDbType.Bit).Value = value;

                    UPdateCommand.ExecuteNonQuery();
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


        public void UpdateTargetDate(DateTime targetDate)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                try
                {
                    Connection.Open();
                    string UpdateString =
                        "UPDATE [Sales_JobMasterList] SET [TargetDate]=@TargetDate, [OriginalTargetDate]=@OriginalTargetDate  WHERE [jobID] = @jobID";
                    var UPdateCommand = new SqlCommand(UpdateString, Connection);

                    UPdateCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = _jobID;
                    UPdateCommand.Parameters.Add("@OriginalTargetDate", SqlDbType.SmallDateTime).Value = targetDate;
                    UPdateCommand.Parameters.Add("@TargetDate", SqlDbType.SmallDateTime).Value = targetDate;

                    UPdateCommand.ExecuteNonQuery();
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

        public void UpdateReasonOfLost(string value)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                const string updateString = "UPDATE [Sales_JobMasterList] SET [JobStatus] = @JobStatus, [reasonOfLoss] = @reasonOfLoss, [isLost] = @isLost WHERE ([jobID] = @jobID)";
                
                var updateCommand = new SqlCommand(updateString, Connection);

                updateCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = _jobID;
                updateCommand.Parameters.Add("@JobStatus", SqlDbType.Int).Value = NJobStatus.Loss;
                updateCommand.Parameters.Add("@reasonOfLoss", SqlDbType.NVarChar, 500).Value = value;
                updateCommand.Parameters.Add("@isLost", SqlDbType.Bit).Value = true;

                try
                {
                    Connection.Open();
                    updateCommand.ExecuteNonQuery();
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


        //public void UpdateQupteFinalRev(int quoteRevID)
        // {
        //     SqlConnection ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
        //     string SqlSelectString = "SELECT jobID,QuoteRevID FROM [Sales_JobMasterList] WHERE ([jobID] = @jobID)";
        //     SqlCommand SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
        //     SqlDataAdapter adapter1 = new SqlDataAdapter(SelectCommand);
        //     DataSet ds1 = new DataSet();
        //     ds1.Tables.Clear();
        //     adapter1.SelectCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = _jobID;
        //     try
        //     {
        //         ConnectionSQL.Open();
        //         int rowsAffected = adapter1.Fill(ds1, "t1");
        //         if (rowsAffected > 0)
        //         {
        //             DataRow row = ds1.Tables["t1"].Rows[0];
        //             row["quoteRevID"] = quoteRevID;
        //             //. Write  back to DB
        //             SqlCommandBuilder cb = new SqlCommandBuilder(adapter1);
        //             adapter1 = cb.DataAdapter;
        //             rowsAffected += adapter1.Update(ds1, "t1");
        //         }

        //     }
        //     catch (SqlException ex)
        //     {
        //         string errLog = ex.Message;

        //     }
        //     finally
        //     {
        //         ConnectionSQL.Close();
        //     }
        // }


        //public void UpdateEstimationFinalRev(int estRevID)
        // {
        //     SqlConnection ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
        //     string SqlSelectString = "SELECT jobID,estimationRevID FROM [Sales_JobMasterList] WHERE ([jobID] = @jobID)";
        //     SqlCommand SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
        //     SqlDataAdapter adapter1 = new SqlDataAdapter(SelectCommand);
        //     DataSet ds1 = new DataSet();
        //     ds1.Tables.Clear();
        //     adapter1.SelectCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = _jobID;
        //     try
        //     {
        //         ConnectionSQL.Open();
        //         int rowsAffected = adapter1.Fill(ds1, "t1");
        //         if (rowsAffected > 0)
        //         {
        //             DataRow row = ds1.Tables["t1"].Rows[0];
        //             row["estimationRevID"] = estRevID;
        //             //. Write  back to DB
        //             SqlCommandBuilder cb = new SqlCommandBuilder(adapter1);
        //             adapter1 = cb.DataAdapter;
        //             rowsAffected += adapter1.Update(ds1, "t1");
        //         }

        //     }
        //     catch (SqlException ex)
        //     {
        //         string errLog = ex.Message;

        //     }
        //     finally
        //     {
        //         ConnectionSQL.Close();
        //     }
        // }
    }
}