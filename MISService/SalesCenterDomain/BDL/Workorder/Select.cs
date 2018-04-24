using System;
using System.Data;
using System.Data.SqlClient;
using EmployeeDomain;
using MyCommon;
using MyCommon.MyEnum;
using SalesCenterDomain.BO;

namespace SalesCenterDomain.BDL.Workorder
{
    public abstract class WorkorderFieldBase
    {
        //for read
        private readonly int _woID;

        public WorkorderFieldBase(int woID)
        {
            _woID = woID;
        }

        public object FieldValue
        {
            get
            {
                DataRow row = GetWorkorderInfoationDataRow();
                return GetFieldValue(row);
            }
            set { Update(_woID, value); }
        }

        public abstract object GetFieldValue(DataRow row);

        private DataRow GetWorkorderInfoationDataRow()
        {
            int NumRowsAffected = 0;
            DataRow titleInfoRow = null;

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 = "SELECT * FROM [Sales_JobMasterList_WO] WHERE ([woID] = @woID)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@woID", SqlDbType.Int).Value = _woID;
            var ds1 = new DataSet();

            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                NumRowsAffected = adapter1.Fill(ds1, "t1");

                if (NumRowsAffected != 0)
                {
                    titleInfoRow = ds1.Tables["t1"].Rows[0];
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

            return titleInfoRow;
        }

        public virtual void Update(int woID, object value)
        {
        }
    }

    public class WorkorderFieldSales : WorkorderFieldBase
    {
        public WorkorderFieldSales(int woID)
            : base(woID)
        {
        }

        public override object GetFieldValue(DataRow row)
        {
            var i = (int) EmployeeEN.NEmployeeIDDefault.NullSales110;

            if (!Convert.IsDBNull(row["Sales"]))
            {
                i = MyConvert.ConvertToInteger(row["Sales"]);
            }

            return i;
        }
    }

    public class WorkorderFieldSa1ID : WorkorderFieldBase
    {
        public WorkorderFieldSa1ID(int woID)
            : base(woID)
        {
        }

        public override object GetFieldValue(DataRow row)
        {
            var i = (int) EmployeeEN.NEmployeeIDDefault.NullSales110;

            if (!Convert.IsDBNull(row["Sa1ID"]))
            {
                i = MyConvert.ConvertToInteger(row["Sa1ID"]);
            }

            return i;
        }
    }


    public class WorkorderFieldIssuedDate : WorkorderFieldBase
    {
        public WorkorderFieldIssuedDate(int woID)
            : base(woID)
        {
        }

        public override object GetFieldValue(DataRow row)
        {
            DateTime d = DateTime.Today;
            if (!Convert.IsDBNull(row["issuedDate"]))
            {
                if (MyConvert.IsDate(row["issuedDate"]))
                {
                    d = Convert.ToDateTime(row["issuedDate"]);
                }
            }
            return d;
        }

        public override void Update(int woID, object value)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString =
                    "UPDATE [Sales_JobMasterList_WO] SET [IssuedDate] = @IssuedDate WHERE [woID] = @woID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@woID", SqlDbType.Int).Value = woID;
                UPdateCommand.Parameters.Add("@IssuedDate", SqlDbType.SmallDateTime).Value = Convert.ToDateTime(value);
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


    public class WorkorderFieldWoType : WorkorderFieldBase
    {
        public WorkorderFieldWoType(int woID)
            : base(woID)
        {
        }

        public override object GetFieldValue(DataRow row)
        {
            int i = 0;

            if (!Convert.IsDBNull(row["WoType"]))
            {
                i = MyConvert.ConvertToInteger(row["WoType"]);
            }

            return i;
        }


        public override void Update(int woID, object value)
        {
            using (var Connection = new SqlConnection(SalesCenterConfiguration.ConnectionString))
            {
                string UpdateString = "UPDATE [Sales_JobMasterList_WO] SET [WoType] = @WoType WHERE [woID] = @woID";
                var UPdateCommand = new SqlCommand(UpdateString, Connection);

                UPdateCommand.Parameters.Add("@woID", SqlDbType.Int).Value = woID;
                UPdateCommand.Parameters.Add("@WoType", SqlDbType.Int).Value = value;
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


    public class WorkOrderSelect
    {
        private readonly DataRow _row;
        private int _woID;

        public WorkOrderSelect(int woID)
        {
            _woID = woID;
            _row = WorkorderShared.GetWorkorderInfo(_woID);
        }

        public WorkOrderSelect(string woNumber)
        {
            _row = WorkorderShared.GetWorkorderInfo(woNumber);
            if (_row != null)
            {
                _woID = Convert.ToInt32(_row["woID"]);
            }
        }


        public int JobID
        {
            get { return GetJobID(); }
        }

        public int WoID
        {
            get { return _woID; }

            set { _woID = value; }
        }

        private int GetJobID()
        {
            int jID = 0;
            if (_row != null)
            {
                jID = Convert.ToInt32(_row["JobID"]);
            }
            return jID;
        }


        public int WoStatus()
        {
            var i = (int) NJobStatus.woNew;
            if (_row != null)
            {
                i = Convert.ToInt32(_row["woStatus"]);
            }
            return i;
        }

        public int WoEstRevID()
        {
            int i = 0;

            if (_row != null)
            {
                i = Convert.ToInt32(_row["estRevID"]);
            }

            return i;
        }


        public bool IsLocked()
        {
            int woStatus = WoStatus();
            bool b = true;
            switch (woStatus)
            {
                case (int) NJobStatus.woApproved:
                case (int) NJobStatus.woPrepared:
                case (int) NJobStatus.woObsolete:
                    b = true;
                    break;
                default:

                    //     Case NJobStatus.woNew, NJobStatus.woInvalid
                    b = false;
                    break;
            }

            return b;
        }

        public string WorkorderTitle()
        {
            string s = "";

            if (_row != null)
            {
                s = Convert.ToString(_row["jobTitle"]);
            }

            return s;
        }


        public int WoType()
        {
            var i = (int) NWorkorderType.Production;
            if (_row != null)
            {
                i = Convert.ToInt32(_row["woType"]);
            }
            return i;
        }
    }


    public class WorkorderShared
    {
        public static DataRow GetWorkorderInfo(string swo)
        {
            DataRow woInfo = null;

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            int NumRowsAffected = 0;
            string SqlSelectString1 = "SELECT * FROM Sales_JobMasterList_WO WHERE (WorkorderNumber = @WorkorderNumber)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@WorkorderNumber", SqlDbType.NVarChar, 15).Value = swo;
            var ds1 = new DataSet();

            try
            {
                ConnectionSQL.Open();
                NumRowsAffected = adapter1.Fill(ds1, "t1");
                if (NumRowsAffected != 0)
                {
                    woInfo = ds1.Tables["t1"].Rows[0];
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
            return woInfo;
        }

        public static DataRow GetWorkorderInfo(int woID)
        {
            int NumRowsAffected = 0;
            DataRow titleInfoRow = null;

            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 = "SELECT * FROM [Sales_JobMasterList_WO] WHERE ([woID] = @woID)";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@woID", SqlDbType.Int).Value = woID;
            var ds1 = new DataSet();

            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                NumRowsAffected = adapter1.Fill(ds1, "t1");

                if (NumRowsAffected != 0)
                {
                    titleInfoRow = ds1.Tables["t1"].Rows[0];
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

            return titleInfoRow;
        }


        public static DataTable getExistingWorkorderItems(int woID)
        {
            string errorLog = "Nothing";
            int NumRowsAffected = 0;
            DataTable tbl = null;

            //2. Define the destination: InvoiceItem
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString1 = "SELECT * FROM [WO_Item] WHERE ([woID] = @woID) order by woItemID";
            var SelectCommand1 = new SqlCommand(SqlSelectString1, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand1);
            adapter1.SelectCommand.Parameters.Add("@woID", SqlDbType.Int).Value = woID;
            var ds1 = new DataSet();

            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                NumRowsAffected = adapter1.Fill(ds1, "t1");
                if (NumRowsAffected != 0)
                {
                    tbl = ds1.Tables["t1"];
                }
            }
            catch (SqlException ex)
            {
                errorLog = ex.Message;
            }
            finally
            {
                ConnectionSQL.Close();
            }

            return tbl;
        }
    }


    //Active Work Order list
    public class WorkorderCollection
    {
        private readonly DataTable _tbl;

        public WorkorderCollection()
        {
            _tbl = GetWorkorderList();
        }

        public WorkorderCollection(DateTime beginDate, int eID)
        {
            _tbl = GetWorkorderList(beginDate, eID);
        }

        public DataTable List
        {
            get { return _tbl; }
        }

        private DataTable GetWorkorderList(DateTime BeginDate, int eID)
        {
            DataTable tbl = null;
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            string SqlSelectString =
                "SELECT * FROM View_WO_Message_List_Production WHERE (ScheduleDate >= @BeginDate) AND (Sales = @eID OR @eID = Sa1ID) ORDER BY Title";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand);
            var ds1 = new DataSet();
            ds1.Tables.Clear();
            adapter1.SelectCommand.Parameters.Add("@BeginDate", SqlDbType.SmallDateTime).Value = BeginDate;
            adapter1.SelectCommand.Parameters.Add("@eID", SqlDbType.Int).Value = eID;

            try
            {
                ConnectionSQL.Open();
                int rowsAffected = adapter1.Fill(ds1, "t1");
                if (rowsAffected > 0)
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

        //All Active 
        private DataTable GetWorkorderList()
        {
            DataTable tbl = null;
            var ConnectionSQL = new SqlConnection(SalesCenterConfiguration.ConnectionString);
            //Difference
            string SqlSelectString = "SELECT * FROM View_WO_Message_List_Production ORDER BY Title";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand);
            var ds1 = new DataSet();
            ds1.Tables.Clear();

            try
            {
                ConnectionSQL.Open();
                int rowsAffected = adapter1.Fill(ds1, "t1");
                if (rowsAffected > 0)
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
}