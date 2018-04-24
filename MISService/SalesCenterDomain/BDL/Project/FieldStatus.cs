using System;
using System.Data;
using System.Data.SqlClient;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Project
{
    public class FieldStatus
    {
        private readonly int _jobID;

        public FieldStatus(int jobID)
        {
            _jobID = jobID;
        }

        public void InsertNewStatus(int jobStatus, int performer)
        {
            var connection = new SqlConnection(SalesCenterConfiguration.ConnectionString);

            using (connection)
            {
                const string insertString =
                    "INSERT INTO [Sales_JobStatusTable] ([jobID], [jobStatus], [accomplishDate], [Performer]) VALUES (@jobID, @jobStatus, @accomplishDate, @Performer)";
                // Create the command and set its properties.
                var insertCommand = new SqlCommand(insertString, connection);
                try
                {
                    connection.Open();
                    insertCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = _jobID;
                    insertCommand.Parameters.Add("@jobStatus", SqlDbType.Int).Value = jobStatus;
                    insertCommand.Parameters.Add("@Performer", SqlDbType.SmallInt).Value = performer;
                    insertCommand.Parameters.Add("@accomplishDate", SqlDbType.SmallDateTime).Value = DateTime.Now;
                    insertCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    string errorLog = ex.Message;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public void UpdateJobStatusChanged(bool boolValue)
        {
            var connection = new SqlConnection(SalesCenterConfiguration.ConnectionString);

            using (connection)
            {
                const string updateString =
                    "UPDATE [Sales_JobMasterList] SET [StatusChanged]=@StatusChanged WHERE [jobID] = @jobID";

                try
                {
                    connection.Open();
                    var uPdateCommand = new SqlCommand(updateString, connection);

                    uPdateCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = _jobID;
                    uPdateCommand.Parameters.Add("@StatusChanged", SqlDbType.Bit).Value = boolValue;

                    uPdateCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    string errorLog = ex.Message;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}