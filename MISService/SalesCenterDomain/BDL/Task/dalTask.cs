using System;
using System.Data;
using System.Data.SqlClient;
using SalesCenterDomain.BO;
using SalesCenterDomain.SalesCenter;

namespace SalesCenterDomain.BDL.Task
{
    /// <summary>
    /// To add a new type of request:
    /// 1.  RequestType GetRequestResponse()
    /// 2.  Reject.FinishStatus
    /// </summary>
    public class DispatchingTaskInsertDAL
    {
        private readonly DispatchingTask _cp;

        public DispatchingTaskInsertDAL(DispatchingTask createParameter)
        {
            _cp = createParameter;
        }

        public void InsertRecord(int version)
        {
            using (var connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string InsertString =
                    "INSERT INTO [Sales_Dispatching] ([taskType], [jobID], [estRevID], [woID], [submitBy], [Responsible], [Subject], [Description], [Status], [submitTime], [lastUpdateTime], [requiredTime],[requestType], [iiOkTime],[Rush],[Importance]) VALUES (@taskType, @jobID, @estRevID, @woID, @submitBy, @Responsible, @Subject, @Description, @Status, @submitTime, @lastUpdateTime, @requiredTime,@requestType, @iiOkTime,@Rush, @Importance)";
                // Create the command and set its properties.
                var insertCommand = new SqlCommand(InsertString, connection);
                try
                {
                    connection.Open();
                    insertCommand.Parameters.Add("@taskType", SqlDbType.SmallInt).Value = _cp.TaskCategory;
                    insertCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = _cp.JobId;
                    insertCommand.Parameters.Add("@estRevID", SqlDbType.Int).Value = _cp.EstRevId;
                    insertCommand.Parameters.Add("@woID", SqlDbType.Int).Value = _cp.WoId;
                    insertCommand.Parameters.Add("@submitBy", SqlDbType.SmallInt).Value = _cp.SubmitBy;
                    insertCommand.Parameters.Add("@Responsible", SqlDbType.SmallInt).Value = _cp.Responsible;
                    insertCommand.Parameters.Add("@Subject", SqlDbType.NVarChar, 200).Value = _cp.Subject;
                    insertCommand.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value = _cp.Description;
                    insertCommand.Parameters.Add("@Status", SqlDbType.SmallInt).Value = _cp.Status;
                    insertCommand.Parameters.Add("@submitTime", SqlDbType.SmallDateTime).Value = DateTime.Now;
                    insertCommand.Parameters.Add("@Importance", SqlDbType.SmallInt).Value = version;

                    insertCommand.Parameters.Add("@lastUpdateTime", SqlDbType.SmallDateTime).Value = DateTime.Now;
                    insertCommand.Parameters.Add("@requiredTime", SqlDbType.SmallDateTime).Value = _cp.RequiredTime;
                    insertCommand.Parameters.Add("@requestType", SqlDbType.Char, 3).Value = _cp.RequestType;

                    insertCommand.Parameters.Add("@iiOkTime", SqlDbType.SmallDateTime).Value = DBNull.Value;
                    insertCommand.Parameters.Add("@Rush", SqlDbType.Bit).Value = _cp.Rush;
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
    }

    public class DispatchingTaskFinishDAL
    {
        //Normal Finish, Input Labour Hour
        private readonly DispatchingTask _cp;

        public DispatchingTaskFinishDAL(DispatchingTask createParameter)
        {
            _cp = createParameter;
        }

        public void UpdateFinishTime()
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString ="UPDATE [Sales_Dispatching] SET [FinishedDate]=@FinishedDate  WHERE [taskID] = @taskID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);
                UPdateCommand.Parameters.Add("@taskID", SqlDbType.Int).Value = _cp.TaskId;
                UPdateCommand.Parameters.Add("@FinishedDate", SqlDbType.SmallDateTime).Value = _cp.FinishedDate;

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

        public void InputLabourHour()
        {
            using (var connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string updateString =
                    "UPDATE [Sales_Dispatching] SET [workedHour] = @workedHour, [NumberOfDrawing]=@NumberOfDrawing, [Note]=@Note WHERE [taskID] = @taskID";
                var updateCommand = new SqlCommand(updateString, connection);

                updateCommand.Parameters.Add("@taskID", SqlDbType.Int).Value = _cp.TaskId;
                updateCommand.Parameters.Add("@workedHour", SqlDbType.Float).Value = _cp.WorkedHour;
                updateCommand.Parameters.Add("@NumberOfDrawing", SqlDbType.SmallInt).Value = _cp.NumberOfDrawing;
                updateCommand.Parameters.Add("@Note", SqlDbType.VarChar, 998).Value = _cp.Note;

                try
                {
                    connection.Open();
                    updateCommand.ExecuteNonQuery();
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
    }

    public class DispatchingTaskRejectDAL
    {
        //Reject, Input Reason
        private readonly DispatchingTask _cp;

        public DispatchingTaskRejectDAL(DispatchingTask createParameter)
        {
            _cp = createParameter;
        }

        public void Reject()
        {
            using (var connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Sales_Dispatching] SET [IncompleteInfo] = @IncompleteInfo, [iiMarkedTime]=@iiMarkedTime,[iiReason]=@iiReason, [Status] = @Status,[FinishedDate]=@FinishedDate  WHERE [taskID] = @taskID";

                var uPdateCommand = new SqlCommand(UpdateString, connection);

                uPdateCommand.Parameters.Add("@taskID", SqlDbType.Int).Value = _cp.TaskId;
                uPdateCommand.Parameters.Add("@IncompleteInfo", SqlDbType.Bit).Value = _cp.InCompleteInfo;
                uPdateCommand.Parameters.Add("@iiMarkedTime", SqlDbType.SmallDateTime).Value = DateTime.Now;
                uPdateCommand.Parameters.Add("@iiReason", SqlDbType.NVarChar, 500).Value = _cp.IiReason;
                uPdateCommand.Parameters.Add("@FinishedDate", SqlDbType.SmallDateTime).Value = _cp.FinishedDate;
                uPdateCommand.Parameters.Add("@Status", SqlDbType.Int).Value = _cp.Status;

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
    }

    public class DispatchingTaskSelectionDAL
    {
        private readonly int _taskId;

        public DispatchingTaskSelectionDAL(int taskId)
        {
            _taskId = taskId;
        }

        public DataRow Row
        {
            get { return GetDispatchingDataRow(); }
        }

        private DataRow GetDispatchingDataRow()
        {
            DataRow row = null;
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);

            string SqlSelectString1 = "SELECT * FROM Sales_Dispatching WHERE (taskID = @taskID)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            //Following 2 differ from job request
            adapter1.SelectCommand.Parameters.Add("@taskID", SqlDbType.Int).Value = _taskId;
            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                int numRowsAffected = adapter1.Fill(ds1, "t1");
                if (numRowsAffected > 0)
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

    public class DispatchingTaskValidationDAL
    {
        private readonly DispatchingTask _cp;

        public DispatchingTaskValidationDAL(DispatchingTask createParameter)
        {
            _cp = createParameter;
        }

        public bool IsJobRequestExisting()
        {
            //EstRevID
            //To whom, (dp.taskType /dtRequestRadioButtonList.SelectedValue)
            //Purpose  (dp.Subject = RadioButtonList1.SelectedItem.Text)
            //response
            //taskType

            bool b = true;

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            int NumRowsAffected = 0;

            string SqlSelectString1 =
                "SELECT taskID, taskType, requestType, jobID, estRevID, quoteRevID, woID, invoiceID, submitBy, Responsible, Subject, Description, submitTime, lastUpdateTime, requiredTime FROM Sales_Dispatching WHERE (estRevID = @estRevID) AND (taskType = @TaskType) AND (requestType = @requestType)  AND (Subject = @subject) AND (jobID = @jobID)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            //Following 2 differ from work order request
            adapter1.SelectCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = _cp.JobId;
            adapter1.SelectCommand.Parameters.Add("@estRevID", SqlDbType.Int).Value = _cp.EstRevId;
            adapter1.SelectCommand.Parameters.Add("@requestType", SqlDbType.NVarChar, 3).Value = "job";

            adapter1.SelectCommand.Parameters.Add("@TaskType", SqlDbType.Int).Value = _cp.TaskCategory;
            adapter1.SelectCommand.Parameters.Add("@subject", SqlDbType.NVarChar, 200).Value = _cp.Subject.Trim();
            //   adapter1.SelectCommand.Parameters.Add("@Responsible", SqlDbType.Int).Value = _cp.Responsible;

            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                NumRowsAffected = adapter1.Fill(ds1, "t1");
                if (NumRowsAffected > 0)
                {
                    //'updating
                    //Dim row As DataRow = ds1.Tables["t1"].Rows[0]
                    //row["submitBy") = submitBy
                    //row["lastUpdateTime") = submitTime
                    //row["requiredTime") = requiredTime
                    //row["Description") = Description

                    //Dim cb As New SqlCommandBuilder(adapter1)
                    //adapter1 = cb.DataAdapter
                    //NumRowsAffected = adapter1.Update(ds1, "t1")
                }
                else
                {
                    b = false;
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

            return b;
        }

        public bool IsWorkorderRequestExisting()
        {
            //woID
            //To whom, (dp.taskType /dtRequestRadioButtonList.SelectedValue)
            //Purpose  (dp.Subject = RadioButtonList1.SelectedItem.Text)
            //response
            //taskType


            bool b = true;

            //Dim ws As New WorkOrderSelect(woID)
            //Dim woStatus As Integer = ws.WoStatus()

            //If woStatus = (int)Common.FsENums.NJobStatus.woNew Or woStatus = (int)Common.FsENums.NJobStatus.woNew Then
            //    b = False
            //Else

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            int NumRowsAffected = 0;

            string SqlSelectString1 =
                "SELECT taskID, taskType, requestType, jobID, estRevID, quoteRevID, woID, invoiceID, submitBy, Responsible, Subject, Description, submitTime, lastUpdateTime, requiredTime FROM Sales_Dispatching WHERE (woID = @woID) AND (taskType = @TaskType) AND (requestType = @requestType)  AND (Subject = @subject) ";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            //Following 2 differ from job request
            adapter1.SelectCommand.Parameters.Add("@woID", SqlDbType.Int).Value = _cp.WoId;
            adapter1.SelectCommand.Parameters.Add("@requestType", SqlDbType.NVarChar, 3).Value = "wip";

            adapter1.SelectCommand.Parameters.Add("@TaskType", SqlDbType.Int).Value = _cp.TaskCategory;
            adapter1.SelectCommand.Parameters.Add("@subject", SqlDbType.NVarChar, 200).Value = _cp.Subject.Trim();
            //?
            //   adapter1.SelectCommand.Parameters.Add("@Responsible", SqlDbType.Int).Value = _cp.Responsible;

            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                NumRowsAffected = adapter1.Fill(ds1, "t1");
                if (NumRowsAffected > 0)
                {
                    //updating
                    //Dim row As DataRow = ds1.Tables["t1"].Rows[0]
                    //row["submitBy") = submitBy
                    //row["lastUpdateTime") = submitTime
                    //row["requiredTime") = requiredTime
                    //row["Description") = Description

                    //Dim cb As New SqlCommandBuilder(adapter1)
                    //adapter1 = cb.DataAdapter
                    //NumRowsAffected = adapter1.Update(ds1, "t1")
                }
                else
                {
                    b = false;
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
            //  End If

            return b;
        }
    }
}