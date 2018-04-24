using System;
using System.Data;
using System.Data.SqlClient;
using EmployeeDomain;
using EmployeeDomain.BLL;
using MyCommon;
using MyCommon.MyEnum;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Project
{
    public class ProjectInsert
    {
        private int _jobID;
        private string _jobNumber = "";
        private int _sa1ID = (int) EmployeeEN.NEmployeeIDDefault.NullSales110;
        private int _sales = (int) EmployeeEN.NEmployeeIDDefault.NullSales110;

        private int _validationErrorID;

        public ProjectInsert(int salesID)
        {
            _sales = salesID;
        }

        public int ValidationErrorID
        {
            get
            {
                _validationErrorID = GetValidation();
                return _validationErrorID;
            }
            set { _validationErrorID = value; }
        }

        public int JobID
        {
            get { return _jobID; }
        }

        public string JobNumber
        {
            get { return _jobNumber; }
        }

        private void SetAeAndOP()
        {
            var employee = new FsEmployee(_sales);
            if (employee.Role == (int) EmployeeEN.NEmployeeRole.OP)
            {
                _sa1ID = _sales;
                _sales = employee.GetMyAeID(2);
            }
        }


        public int GetValidation()
        {
            int val = 12;
            var employee = new FsEmployee(_sales);

            if (employee.Role == Convert.ToInt32((object) EmployeeEN.NEmployeeRole.AE) |
                employee.Role == Convert.ToInt32((object) EmployeeEN.NEmployeeRole.OP))
            {
                val = GetExtraValidation();
            }

            return val;
        }

        public virtual int GetExtraValidation()
        {
            return 0;
        }


        public void Insert()
        {
            if (ValidationErrorID == 0)
            {
                SetAeAndOP();

                int projectSubNo = GetProjectSubNo();
                //Project.Sub
                string jTitle = GetJobTitle();
                InsertNewProject(projectSubNo, jTitle);

                _jobID = GetNewlyInsertedJobID();

                _jobNumber = GetNewJobNumber();
                UpdateJobNumber(_jobID, _jobNumber);

                var ps = new ProjectStatus(_jobID);
                ps.ChnageTo((int) NJobStatus.ProjectNew, _sales);


                OnProjectInserted();

            }
        }

        public virtual void OnProjectInserted()
        {
       }
         
        public virtual int GetProjectSubNo()
        {
            return 0;
        }

        public virtual string GetJobTitle()
        {
            return "New Project";
        }


        public virtual void InsertNewProject(int subProjectNumber, string jTitle)
        {
            //Sales
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            using (ConnectionSQL)
            {
                // As New System.Data.SqlClient.SqlConnection(Configuration.ConnectionString)
                //                Dim InsertString As String = "INSERT INTO [Sales_JobMasterList] ([jobNumber], [sales], [sa1ID], [targetDate], [startTimeStamp],[jobStatus],[subProject],[yearNumber],[jobTitle]) VALUES (@jobNumber, @sales, @sa1ID, @targetDate, @startTimeStamp,@jobStatus,@subProject,@yearNumber,@jobTitle)"
                string InsertString ="INSERT INTO [Sales_JobMasterList] ([jobNumber], [sales], [sa1ID], [startTimeStamp],[jobStatus],[subProject],[yearNumber],[jobTitle]) VALUES (@jobNumber, @sales, @sa1ID,  @startTimeStamp,@jobStatus,@subProject,@yearNumber,@jobTitle)";
                var InsertCommand = new SqlCommand(InsertString, ConnectionSQL);
                InsertCommand.Parameters.Add("@jobNumber", SqlDbType.NVarChar, 15).Value = "ToBeAssigned";
                InsertCommand.Parameters.Add("@sales", SqlDbType.Int).Value = _sales;
                InsertCommand.Parameters.Add("@sa1ID", SqlDbType.Int).Value = _sa1ID;
                //InsertCommand.Parameters.Add("@targetDate", SqlDbType.SmallDateTime).Value = Date.Today.AddDays(31)
                InsertCommand.Parameters.Add("@startTimeStamp", SqlDbType.SmallDateTime).Value = DateTime.Now;
                InsertCommand.Parameters.Add("@jobStatus", SqlDbType.Int).Value = NJobStatus.ProjectNew;
                //101
                InsertCommand.Parameters.Add("@subProject", SqlDbType.SmallInt).Value = subProjectNumber;
                InsertCommand.Parameters.Add("@yearNumber", SqlDbType.SmallInt).Value = GetYearNumber();
                InsertCommand.Parameters.Add("@jobTitle", SqlDbType.NVarChar, 150).Value = jTitle;
                try
                {
                    ConnectionSQL.Open();
                    InsertCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    string errorLog = ex.Message;
                }
                finally
                {
                    ConnectionSQL.Close();
                }
            }
        }

        private int GetNewlyInsertedJobID()
        {
            int TicketID = 0;
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string SelectString = "SELECT IDENT_CURRENT('Sales_JobMasterList')";
                var SelectCommand = new SqlCommand(SelectString, Connection);
                try
                {
                    Connection.Open();
                    object tID = SelectCommand.ExecuteScalar();
                    TicketID = Convert.ToInt32(tID);
                }
                catch (SqlException ex)
                {
                    TicketID = 0;
                }
                finally
                {
                    Connection.Close();
                }
            }
            return TicketID;
        }

        public virtual string GetNewJobNumber()
        {
            //yy(year)+P+D5

            string jNumber = "ToBeAssigned";

            int yearnow = GetYearNumber();

            jNumber = yearnow.ToString() + "P";

            int n1 = GetRecordCountOfProjectForThisYear(yearnow);
            if (n1 > 0)
            {
                n1 = n1 + SalesCenterConstants.BEGIN_PROJECT_STARTER_SEED;
            }

            jNumber += n1.ToString("D5");

            //if error, xxP0000 is returned
            return jNumber;
        }

        private int GetYearNumber()
        {
            int yearNow = DateTime.Today.Year;
            return (yearNow - 2000);
        }

        private int GetRecordCountOfProjectForThisYear(int yearNumber)
        {
            //Same Year Number
            //SubProject=0
            int i = 0;

            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string SelectString =
                    "SELECT COUNT(jobID) AS JobNumberCount FROM Sales_JobMasterList WHERE (subProject = 0) AND (yearNumber = @YearNumber)";
                var SelectCommand = new SqlCommand(SelectString, Connection);
                SelectCommand.Parameters.Add("@yearNumber", SqlDbType.Int).Value = yearNumber;

                try
                {
                    Connection.Open();
                    SqlDataReader reader = SelectCommand.ExecuteReader();

                    if (reader != null)
                    {
                        //at the beginning of new year, the first result is 1
                        while (reader.Read())
                        {
                            i = Convert.ToInt32(reader[0]);
                        }
                    }
                    reader.Close();
                }
                catch (SqlException ex)
                {
                    string msg = ex.Message;
                }
                finally
                {
                    // Always call Close when done reading.
                    Connection.Close();
                }
            }

            return i;
        }

        private void UpdateJobNumber(int jID, string jobNum)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString = "UPDATE [Sales_JobMasterList] SET [jobNumber] = @jobNumber WHERE [jobID] = @jobID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = jID;
                UPdateCommand.Parameters.Add("@jobNumber", SqlDbType.VarChar, 15).Value = jobNum;
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
    }

    public class ProjectInsertSubproject : ProjectInsert
    {
        private readonly string _jobNumber = "";

        private readonly ProjectDetails _pd;
        private readonly int _sales;

        private string _idCode = "";

        private string _locationCode = "";


        public ProjectInsertSubproject(int salesID, int parentID)
            : base(salesID)
        {
            if (parentID == 0) return;
            _pd = new ProjectDetails(parentID);

            _jobNumber = _pd.JobNumber;
            _sales = salesID;
        }

        public string IdCode
        {
            set { _idCode = value; }
        }

        public string LocationCode
        {
            set { _locationCode = value; }
        }

        public override int GetProjectSubNo()
        {
            return GetSubRecordCountOfThisProject(_jobNumber) + 1;
            //sub project number
        }

        public override string GetJobTitle()
        {
            return "Sub Project of " + _jobNumber;
        }


        public override int GetExtraValidation()
        {
            int v = 0;

            //sub project can not have subproject
            if (_pd != null)
            {
                if (_pd.IsSubProject)
                {
                    v = 3;
                }
            }
            else
            {
                v = 1;
            }

            return v;
        }


        private int GetSubRecordCountOfThisProject(string jobNumber)
        {
            //Same JobNumber
            //SubProject<>0

            int i = 0;

            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string SelectString =
                    "SELECT COUNT(jobID) AS JobNumberCount FROM Sales_JobMasterList WHERE (subProject > 0) AND (jobNumber = @jobNumber)";
                var SelectCommand = new SqlCommand(SelectString, Connection);
                SelectCommand.Parameters.Add("@jobNumber", SqlDbType.NVarChar, 15).Value = jobNumber;

                try
                {
                    Connection.Open();
                    SqlDataReader reader = SelectCommand.ExecuteReader();

                    if (reader != null)
                    {
                        //at the beginning of new year, the first result is 1
                        while (reader.Read())
                        {
                            i = Convert.ToInt32(reader[0]);
                        }
                    }
                    reader.Close();
                }
                finally
                {
                    // Always call Close when done reading.
                    Connection.Close();
                }
            }

            return i;
        }

        public override string GetNewJobNumber()
        {
            return _jobNumber;
        }


        public override void InsertNewProject(int subProjectNumber, string jTitle)
        {
            //Sales
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            using (ConnectionSQL)
            {
                // As New System.Data.SqlClient.SqlConnection(Configuration.ConnectionString)
                string InsertString =
                    "INSERT INTO [Sales_JobMasterList] ([jobNumber], [sales], [sa1ID], [targetDate], [startTimeStamp],[jobStatus],[subProject],[yearNumber],[jobTitle]) VALUES (@jobNumber, @sales, @sa1ID, @targetDate, @startTimeStamp,@jobStatus,@subProject,@yearNumber,@jobTitle)";
                var InsertCommand = new SqlCommand(InsertString, ConnectionSQL);
                InsertCommand.Parameters.Add("@jobNumber", SqlDbType.NVarChar, 15).Value = "ToBeAssigned";
                InsertCommand.Parameters.Add("@sales", SqlDbType.Int).Value = _sales;
                InsertCommand.Parameters.Add("@sa1ID", SqlDbType.Int).Value = EmployeeEN.NEmployeeIDDefault.NullSales110;
                InsertCommand.Parameters.Add("@targetDate", SqlDbType.SmallDateTime).Value = DateTime.Today.AddDays(31);
                InsertCommand.Parameters.Add("@startTimeStamp", SqlDbType.SmallDateTime).Value = DateTime.Now;
                InsertCommand.Parameters.Add("@jobStatus", SqlDbType.Int).Value = NJobStatus.ProjectNew;
                //101
                InsertCommand.Parameters.Add("@subProject", SqlDbType.SmallInt).Value = subProjectNumber;
                InsertCommand.Parameters.Add("@yearNumber", SqlDbType.SmallInt).Value = MyDateTime.GetYearNumber();
                InsertCommand.Parameters.Add("@jobTitle", SqlDbType.NVarChar, 150).Value = jTitle;

                InsertCommand.Parameters.Add("@idCode", SqlDbType.NVarChar, 50).Value = _idCode;
                InsertCommand.Parameters.Add("@locationCode", SqlDbType.NVarChar, 50).Value = _locationCode;


                try
                {
                    ConnectionSQL.Open();
                    InsertCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    string errorLog = ex.Message;
                }
                finally
                {
                    ConnectionSQL.Close();
                }
            }
        }
    }

    public class ProjectInsertBidding : ProjectInsert
    {
        public string JobTitle { get; set; }
        private readonly int _salesID;
        public DateTime TargetDate { get; set; }
        public int CustomerID { get; set; }
       
        public ProjectInsertBidding(int salesID): base(salesID)
        {
            _salesID = salesID;
        }

        public override void OnProjectInserted()
        {
            UpdateJobNumber();
        }

        private void UpdateJobNumber()
        {
            using (var connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                const string updateString = "UPDATE [Sales_JobMasterList] SET [jobTitle] = @jobTitle, [sales]=@sales, [targetDate] = @targetDate, [Customer] = @Customer, [isBidToProject] = @isBidToProject, [SalesType] = @SalesType WHERE [jobID] = @jobID";
                var updateCommand = new SqlCommand(updateString, connection);

                updateCommand.Parameters.Add("@jobID", SqlDbType.Int).Value = JobID;
                updateCommand.Parameters.Add("@jobTitle", SqlDbType.VarChar, 150).Value = JobTitle;
                updateCommand.Parameters.Add("@isBidToProject", SqlDbType.Bit).Value = true;
                updateCommand.Parameters.Add("@SalesType", SqlDbType.Int).Value = NCommissionProjectCategoryID.Bid;
                updateCommand.Parameters.Add("@sales", SqlDbType.Int).Value =_salesID;
                if (MyCommon.MyConvert .IsDate(TargetDate))
                {
                    updateCommand.Parameters.Add("@targetDate", SqlDbType.SmallDateTime).Value = TargetDate;    
                }
                else
                {
                    updateCommand.Parameters.Add("@targetDate", SqlDbType.SmallDateTime).Value = System.DBNull.Value  ;
                }
                
                updateCommand.Parameters.Add("@Customer", SqlDbType.Int).Value = CustomerID;
                 
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


}