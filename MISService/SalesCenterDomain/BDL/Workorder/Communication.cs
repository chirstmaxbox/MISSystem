using System;
using System.Data;
using System.Data.SqlClient;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Workorder
{
    //Communication Between Sales and Schedule Meeting
    //To Change Job Deadline
    //Not actually Implemented

    #region "Communication"

    public class WorkorderResponse
    {
        private bool _closed;
        private int _descendent;
        private string _description = "";
        private DateTime _finishedDate;
        private DateTime _lastVisitTime = DateTime.Now;
        private int _parent;
        private DateTime _requiredTime = DateTime.Now.AddHours(8);
        private int _responsible = 110;
        private int _root;
        private int _status;
        private string _subject = "";
        private int _submitBy = 28;
        private DateTime _submitTime = DateTime.Now;
        private int _taskID;
        private int _woID;
        public string isInsertSuccessfully = "Yes";

        public WorkorderResponse()
        {
            //For create
        }

        public WorkorderResponse(int taskID)
        {
            //read task details
            _taskID = taskID;
        }

        public int TaskID
        {
            get { return _taskID; }

            set { _taskID = value; }
        }

        //Status

        public bool Closed
        {
            get { return _closed; }

            set { _closed = value; }
        }

        public int Status
        {
            get { return _status; }

            set { _status = value; }
        }

        //Who

        public int SubmitBy
        {
            get { return _submitBy; }

            set { _submitBy = value; }
        }

        public int Responsible
        {
            get { return _responsible; }

            set { _responsible = value; }
        }

        //When

        public DateTime SubmitTime
        {
            get { return _submitTime; }

            set { _submitTime = value; }
        }


        public DateTime RequiredTime
        {
            get { return _requiredTime; }

            set { _requiredTime = value; }
        }

        public DateTime FinishedDate
        {
            get { return _finishedDate; }

            set { _finishedDate = value; }
        }

        //Which

        public int WoID
        {
            get { return _woID; }

            set { _woID = value; }
        }

        //What

        public string Subject
        {
            get { return _subject; }

            set { _subject = value; }
        }

        public string Description
        {
            get { return _description; }

            set { _description = value; }
        }


        //Replay
        //0=root, >=1, Children

        public int Descendent
        {
            get { return _descendent; }

            set { _descendent = value; }
        }

        public int Root
        {
            get { return _root; }

            set { _root = value; }
        }

        public int Parent
        {
            get { return _parent; }

            set { _parent = value; }
        }

        //Display

        public DateTime LastVisitTime
        {
            get { return _lastVisitTime; }

            set { _lastVisitTime = value; }
        }


        //Variables

        public void CreateNew()
        {
            //The first time
            //There are 2 directions
            //insertRecord()
            //'
        }

        public void Reply()
        {
            //Difference with CreateNew: descendent=parent.descendent+1
        }

        public void Finish()
        {
            //close=true
            //finishedDate=now
            //All sub closed
        }


        public void Delete()
        {
            //Check deletable: No Child
        }


        private string FullinsertRecord()
        {
            string errorLog = "OK";
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string InsertString =
                    "INSERT INTO [WO_Message_list] ([Closed], [Status], [SubmitBy], [Responsible], [SubmitTime], [RequiredTime], [FinishedDate], [woID], [Subject], [Description], [Descendent], [Root], [Parent], [LastVisitTime]) VALUES (@Closed,@Status, @SubmitBy, @Responsible, @SubmitTime, @RequiredTime, @FinishedDate, @woID, @Subject, @Description, @Descendent, @Root, @Parent, @LastVisitTime)";
                // Create the command and set its properties.
                var InsertCommand = new SqlCommand(InsertString, Connection);
                try
                {
                    Connection.Open();
                    InsertCommand.Parameters.Add("@Closed", SqlDbType.Bit).Value = _closed;
                    InsertCommand.Parameters.Add("@@Status", SqlDbType.Int).Value = _status;

                    InsertCommand.Parameters.Add("@SubmitBy", SqlDbType.Int).Value = _submitBy;
                    InsertCommand.Parameters.Add("@Responsible", SqlDbType.Int).Value = _responsible;
                    InsertCommand.Parameters.Add("@SubmitTime", SqlDbType.SmallDateTime).Value = _submitTime;
                    InsertCommand.Parameters.Add("@RequiredTime", SqlDbType.SmallDateTime).Value = _requiredTime;
                    InsertCommand.Parameters.Add("@FinishedDate", SqlDbType.SmallDateTime).Value = _finishedDate;
                    InsertCommand.Parameters.Add("@woID", SqlDbType.Int).Value = _woID;
                    InsertCommand.Parameters.Add("@Subject", SqlDbType.NVarChar, 500).Value = _subject;
                    InsertCommand.Parameters.Add("@Description", SqlDbType.NVarChar, 2000).Value = _description;

                    InsertCommand.Parameters.Add("@Descendent", SqlDbType.Int).Value = _descendent;
                    InsertCommand.Parameters.Add("@Root", SqlDbType.Int).Value = _root;
                    InsertCommand.Parameters.Add("@Parent", SqlDbType.Int).Value = _parent;

                    InsertCommand.Parameters.Add("@LastVisitTime", SqlDbType.SmallDateTime).Value = _lastVisitTime;
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

            return errorLog;
        }

        private void Update()
        {
            //?Status
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [WO_Message_list] SET [Closed] = @Closed, [SubmitBy] = @SubmitBy, [Responsible] = @Responsible, [SubmitTime] = @SubmitTime, [RequiredTime] = @RequiredTime, [FinishedDate] = @FinishedDate, [woID] = @woID, [Subject] = @Subject, [Description] = @Description, [Descendent] = @Descendent, [Root] = @Root, [Parent] = @Parent, [LastVisitTime] = @LastVisitTime WHERE [taskID] = @taskID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@taskID", SqlDbType.Int).Value = _taskID;
                UPdateCommand.Parameters.Add("@Closed", SqlDbType.Bit).Value = _closed;
                UPdateCommand.Parameters.Add("@SubmitBy", SqlDbType.Int).Value = _submitBy;
                UPdateCommand.Parameters.Add("@Responsible", SqlDbType.Int).Value = _responsible;
                UPdateCommand.Parameters.Add("@SubmitTime", SqlDbType.SmallDateTime).Value = _submitTime;
                UPdateCommand.Parameters.Add("@RequiredTime", SqlDbType.SmallDateTime).Value = _requiredTime;
                UPdateCommand.Parameters.Add("@FinishedDate", SqlDbType.SmallDateTime).Value = _finishedDate;
                UPdateCommand.Parameters.Add("@woID", SqlDbType.Int).Value = _woID;
                UPdateCommand.Parameters.Add("@Subject", SqlDbType.NVarChar, 500).Value = _subject;
                UPdateCommand.Parameters.Add("@Description", SqlDbType.NVarChar, 2000).Value = _description;

                UPdateCommand.Parameters.Add("@Descendent", SqlDbType.Int).Value = _descendent;
                UPdateCommand.Parameters.Add("@Root", SqlDbType.Int).Value = _root;
                UPdateCommand.Parameters.Add("@Parent", SqlDbType.Int).Value = _parent;

                UPdateCommand.Parameters.Add("@LastVisitTime", SqlDbType.SmallDateTime).Value = _lastVisitTime;

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

        private void DeleteTask()
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string delString = "DELETE FROM [WO_Message_list] WHERE [taskID] = @taskID";
                var delCommand = new SqlCommand(delString, Connection);
                delCommand.Parameters.Add("@taskID", SqlDbType.Int).Value = _taskID;
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

        private DataRow GetDataRow()
        {
            DataRow row = null;
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            int NumRowsAffected = 0;

            string SqlSelectString1 = "SELECT * FROM WO_Message_list WHERE (taskID = @taskID)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@taskID", SqlDbType.Int).Value = _taskID;

            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                NumRowsAffected = adapter1.Fill(ds1, "t1");
                if (NumRowsAffected > 0)
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

    public class WorkorderResponseList
    {
        private DataTable _tbl;

        public DataTable list
        {
            get
            {
                _tbl = GetList();
                return _tbl;
            }
        }


        //BeginDate
        //EndDate
        //Active
        private DataTable GetList()
        {
            DataTable tbl = null;
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            int NumRowsAffected = 0;

            string SqlSelectString1 = "SELECT * FROM WO_Message_list WHERE (Closed = 0)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            //adapter1.SelectCommand.Parameters.Add("@taskID", SqlDbType.Int).Value = _taskID

            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                NumRowsAffected = adapter1.Fill(ds1, "t1");
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
    }

    #endregion
}