using System;
using System.Data;
using System.Data.SqlClient;
using MyCommon;

namespace EmployeeDomain.BLL
{
    public class FsEmployeeProfileSearchInitialization
    {
        #region "Properties"
        public int EmployeeNumber { get { return _eNumber; } }
        private readonly int _eNumber;

        private readonly int _pageID;
        private readonly DataRow _row;

        private readonly int _searchprofileID;

        private int _d1;


        private int _d2;

        public bool b01;
        public bool b02;
        public bool b03;
        public bool b04;
        public bool b05;
        public bool b06;
        public bool b07;
        public bool b08;
        public bool b09;
        public bool b10;
        public bool b11;

        public bool b12;
        public bool bAll;

        //Public eDate As Date = Date.Today

        public int int1;
        public int int2;
        public int int3;
        public int int4;

        public int int5;
        public bool isThereSearchProfileDataRow;


        public string CustomString1;
        public string CustomString2;
        public string EmployeeName;


        public DateTime bDate
        {
            get { return DateTime.Today.AddDays(_d1); }
            set { _d1 = (value - DateTime.Today).Days; }
        }

        public DateTime eDate
        {
            get { return DateTime.Today.AddDays(_d2); }
            set { _d2 = (value - DateTime.Today).Days; }
        }

        #endregion

        //UPDATE [FW_EmployeesProfilePageInitialization] SET [EmployeeNumber] = @EmployeeNumber, [PageID] = @PageID, [B00] = @B00, [B01] = @B01, [B02] = @B02, [B03] = @B03, [B04] = @B04, [B05] = @B05, [B06] = @B06, [B07] = @B07, [B08] = @B08, [B09] = @B09, [B10] = @B10, [B11] = @B11, [B12] = @B12, [BeginDate] = @BeginDate, [EndDate] = @EndDate, [Int1] = @Int1, [Int2] = @Int2, [Int3] = @Int3, [Int4] = @Int4, [Int5] = @Int5 WHERE [ID] = @ID
        //DELETE FROM [FW_EmployeesProfilePageInitialization] WHERE [ID] = @ID


        public FsEmployeeProfileSearchInitialization(int eNumber, int pageID)
        {
            _eNumber = eNumber;
            _pageID = pageID;

            _row = GetDataRow(_eNumber, _pageID);

            if (_row != null)
            {
                isThereSearchProfileDataRow = true;
            }
            else
            {
                isThereSearchProfileDataRow = false;
            }

            if (isThereSearchProfileDataRow)
            {
                _searchprofileID = Convert.ToInt32(_row["ID"]);
                InitializeProperties(_row);
            }
        }


        private static DataRow GetDataRow(int eNumber, int pageID)
        {
            DataRow row = null;
            var ConnectionSQL = new SqlConnection(EmployeeConfiguration.ConnectionString);
            string SqlSelectString ="SELECT ID, EmployeeNumber, PageID, B00, B01, B02, B03, B04, B05, B06, B07, B08, B09, B10, B11, B12, BeginDate, EndDate, Int1, Int2, Int3, Int4, Int5, CustomString1, CustomString2, EmployeeName FROM FW_EmployeesProfilePageInitialization WHERE (EmployeeNumber = @EmployeeNumber) AND (PageID = @PageID)";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter = new SqlDataAdapter(SelectCommand);
            var ds2 = new DataSet();
            ds2.Tables.Clear();
            adapter.SelectCommand.Parameters.Add("@EmployeeNumber", SqlDbType.Int).Value = eNumber;
            adapter.SelectCommand.Parameters.Add("@PageID", SqlDbType.Int).Value = pageID;
            try
            {
                ConnectionSQL.Open();
                int affectedRows = adapter.Fill(ds2, "t2");
                if (affectedRows != 0)
                {
                    row = ds2.Tables["t2"].Rows[0];
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

        private void InitializeProperties(DataRow row)
        {
            bAll = Convert.ToBoolean(row["B00"]);
            b01 = Convert.ToBoolean(row["B01"]);
            b02 = Convert.ToBoolean(row["B02"]);
            b03 = Convert.ToBoolean(row["B03"]);
            b04 = Convert.ToBoolean(row["B04"]);
            b05 = Convert.ToBoolean(row["B05"]);
            b06 = Convert.ToBoolean(row["B06"]);
            b07 = Convert.ToBoolean(row["B07"]);
            b08 = Convert.ToBoolean(row["B08"]);
            b09 = Convert.ToBoolean(row["B09"]);
            b10 = Convert.ToBoolean(row["B10"]);
            b11 = Convert.ToBoolean(row["B11"]);
            b12 = Convert.ToBoolean(row["B12"]);
            _d1 = Convert.ToInt32(row["BeginDate"]);
            _d2 = Convert.ToInt32(row["EndDate"]);

            bDate = DateTime.Today.AddDays(_d1);
            eDate = DateTime.Today.AddDays(_d2);

            int1 = Convert.ToInt32(row["Int1"]);
            int2 = Convert.ToInt32(row["Int2"]);
            int3 = Convert.ToInt32(row["Int3"]);
            int4 = Convert.ToInt32(row["Int4"]);
            int5 = Convert.ToInt32(row["Int5"]);

            CustomString1 = MyConvert.ConvertToString(row["CustomString1"]);
            CustomString2 = MyConvert.ConvertToString(row["CustomString2"]);
            EmployeeName = MyConvert.ConvertToString(row["EmployeeName"]);
            

        }

        public void Save()
        {
            if (isThereSearchProfileDataRow)
            {
                UpdateConfiguration();
            }
            else
            {
                InsertConfiguration();
            }
        }


        private void UpdateConfiguration()
        {
            int rowsAffected = 0;

            var ConnectionSQL = new SqlConnection(EmployeeConfiguration.ConnectionString);
            string SqlSelectString = "SELECT * FROM FW_EmployeesProfilePageInitialization WHERE (ID = @ID)";

            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand);
            var ds1 = new DataSet();
            ds1.Tables.Clear();
            adapter1.SelectCommand.Parameters.Add("@ID", SqlDbType.Int).Value = _searchprofileID;

            try
            {
                ConnectionSQL.Open();
                rowsAffected = adapter1.Fill(ds1, "t1");

                DataRow row = ds1.Tables["t1"].Rows[0];

                row["B00"] = bAll;
                row["B01"] = b01;
                row["B02"] = b02;
                row["B03"] = b03;
                row["B04"] = b04;
                row["B05"] = b05;
                row["B06"] = b06;
                row["B07"] = b07;
                row["B08"] = b08;
                row["B09"] = b09;
                row["B10"] = b10;
                row["B11"] = b11;
                row["B12"] = b12;

                row["BeginDate"] = _d1;
                row["EndDate"] = _d2;

                row["Int1"] = int1;
                row["Int2"] = int2;
                row["Int3"] = int3;
                row["Int4"] = int4;
                row["Int5"] = int5;

                row["CustomString1"] = CustomString1;
                row["CustomString2"] = CustomString2;
                row["EmployeeName"] = EmployeeName;

                //1.3. Write  back to DB
                var cb = new SqlCommandBuilder(adapter1);
                adapter1 = cb.DataAdapter;
                rowsAffected += adapter1.Update(ds1, "t1");
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

        
        private void InsertConfiguration()
        {
            var ConnectionSQL = new SqlConnection(EmployeeConfiguration.ConnectionString);
            using (ConnectionSQL)
            {
                string InsertString ="INSERT INTO [FW_EmployeesProfilePageInitialization] ([EmployeeNumber], [PageID], [B00], [B01], [B02], [B03], [B04], [B05], [B06], [B07], [B08], [B09], [B10], [B11], [B12], [BeginDate], [EndDate], [Int1], [Int2], [Int3], [Int4], [Int5], [CustomString1], [CustomString2], [EmployeeName] ) VALUES (@EmployeeNumber, @PageID, @B00, @B01, @B02, @B03, @B04, @B05, @B06, @B07, @B08, @B09, @B10, @B11, @B12, @BeginDate, @EndDate, @Int1, @Int2, @Int3, @Int4, @Int5, @CustomString1, @CustomString2, @EmployeeName)";
                var InsertCommand = new SqlCommand(InsertString, ConnectionSQL);

                InsertCommand.Parameters.Add("@EmployeeNumber", SqlDbType.Int).Value = _eNumber;
                InsertCommand.Parameters.Add("@PageID", SqlDbType.Int).Value = _pageID;

                InsertCommand.Parameters.Add("@B00", SqlDbType.Bit).Value = bAll;

                InsertCommand.Parameters.Add("@B01", SqlDbType.Bit).Value = b01;
                InsertCommand.Parameters.Add("@B02", SqlDbType.Bit).Value = b02;
                InsertCommand.Parameters.Add("@B03", SqlDbType.Bit).Value = b03;
                InsertCommand.Parameters.Add("@B04", SqlDbType.Bit).Value = b04;
                InsertCommand.Parameters.Add("@B05", SqlDbType.Bit).Value = b05;
                InsertCommand.Parameters.Add("@B06", SqlDbType.Bit).Value = b06;
                InsertCommand.Parameters.Add("@B07", SqlDbType.Bit).Value = b07;
                InsertCommand.Parameters.Add("@B08", SqlDbType.Bit).Value = b08;
                InsertCommand.Parameters.Add("@B09", SqlDbType.Bit).Value = b09;
                InsertCommand.Parameters.Add("@B10", SqlDbType.Bit).Value = b10;
                InsertCommand.Parameters.Add("@B11", SqlDbType.Bit).Value = b11;
                InsertCommand.Parameters.Add("@B12", SqlDbType.Bit).Value = b12;

                InsertCommand.Parameters.Add("@BeginDate", SqlDbType.Int).Value = _d1;
                InsertCommand.Parameters.Add("@EndDate", SqlDbType.Int).Value = _d2;

                InsertCommand.Parameters.Add("@Int1", SqlDbType.Int).Value = int1;
                InsertCommand.Parameters.Add("@Int2", SqlDbType.Int).Value = int2;
                InsertCommand.Parameters.Add("@Int3", SqlDbType.Int).Value = int3;
                InsertCommand.Parameters.Add("@Int4", SqlDbType.Int).Value = int4;
                InsertCommand.Parameters.Add("@Int5", SqlDbType.Int).Value = int5;

                InsertCommand.Parameters.Add("@CustomString1", SqlDbType.NVarChar, 50).Value = "";  // CustomString1;
                InsertCommand.Parameters.Add("@CustomString2", SqlDbType.NVarChar, 50).Value = "";  //CustomString2;
                InsertCommand.Parameters.Add("@EmployeeName", SqlDbType.NVarChar, 50).Value = "";

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
}