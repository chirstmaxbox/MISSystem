using System;
using System.Data;
using System.Data.SqlClient;


namespace EmployeeDomain.BLL
{
    public class FsEmployeeTeam
    {
        #region "Sales Team"

        //Add a new team
        public void AddNewTeam()
        {
            //Insert documents for items
            using (var Connection = new SqlConnection(EmployeeConfiguration.ConnectionString))
            {
                string InsertString = "INSERT INTO [FW_EmployeesTeam] ([teamName]) VALUES (@teamName)";
                var InsertCommand = new SqlCommand(InsertString, Connection);
                try
                {
                    Connection.Open();
                    InsertCommand.Parameters.Add("@teamName", SqlDbType.NVarChar, 100).Value = "";
                    InsertCommand.ExecuteNonQuery();
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

        //Delete a Team

        #endregion

        private readonly int _eID;
        private readonly DataRow _row;
        private readonly int _teamID;
        private int _teamType;

        public FsEmployeeTeam(int teamID)
        {
            _teamID = teamID;
            _row = GetTeamDataRow();
        }

        public FsEmployeeTeam(int doesnotMatter, int eID)
        {
            _eID = eID;
            _teamID = GetEmployeeTeamID();
            _row = GetTeamDataRow();
        }

        public int TeamType
        {
            get
            {
                _teamType = Convert.ToInt32(_row["teamType"]);
                return _teamType;
            }
        }


        public int TeamID
        {
            get { return _teamID; }
        }

        public string TeamName
        {
            get { return Convert.ToString(_row["teamName"]); }
        }

        public bool IsShowOnTv
        {
            get { return Convert.ToBoolean(_row["IsShowOnTV"]); }
        }

        public DataRow Datarow
        {
            get { return _row; }
        }




        private DataRow GetTeamDataRow()
        {
            DataRow row = null;
            var ConnectionSQL = new SqlConnection(EmployeeConfiguration.ConnectionString);
            string SqlSelectString = "SELECT * FROM FW_EMPLOYEESTEAM WHERE TEAMID=@TEAMID";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter = new SqlDataAdapter(SelectCommand);
            var ds2 = new DataSet();
            ds2.Tables.Clear();
            adapter.SelectCommand.Parameters.Add("@TEAMID", SqlDbType.Int).Value = _teamID;
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
            }
            finally
            {
                ConnectionSQL.Close();
            }
            return row;
        }


        private int GetEmployeeTeamID()
        {
            int tID = 0;

            var ConnectionSQL = new SqlConnection(EmployeeConfiguration.ConnectionString);
            string SqlSelectString = "SELECT EmployeeNumber,Team FROM FW_EMPLOYEES WHERE EmployeeNumber=@EmployeeNumber";

            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand);
            var ds1 = new DataSet();
            ds1.Tables.Clear();
            adapter1.SelectCommand.Parameters.Add("@EmployeeNumber", SqlDbType.Int).Value = _eID;

            try
            {
                ConnectionSQL.Open();
                int rowsAffected = adapter1.Fill(ds1, "t1");
                if (rowsAffected > 0)
                {
                    DataRow row = ds1.Tables["t1"].Rows[0];
                    if (!Convert.IsDBNull(row["Team"]))
                    {
                        tID = Convert.ToInt32(row["Team"]);
                    }
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

            return tID;
        }


        public int GetMyAeID(int selectType)
        {
            var aeID = (int) EmployeeEN.NEmployeeIDDefault.NullSales110;
            var salesTeam = new FsEmployeeTeam(_teamID);
            DataTable tbl = salesTeam.GetAeList(selectType);
            if (tbl != null)
            {
                DataRow row = tbl.Rows[0];
                aeID = Convert.ToInt32(row["EmployeeNumber"]);
            }
            return aeID;
        }

        public DataTable GetAeList(int teamType)
        {
            var ConnectionSQL = new SqlConnection(EmployeeConfiguration.ConnectionString);
            DataTable tbl = null;
            var SelectCommand = new SqlCommand("rptGetAeList", ConnectionSQL);
            //Stored Procedure
            SelectCommand.CommandType = CommandType.StoredProcedure;
            var adapter1 = new SqlDataAdapter(SelectCommand);

            var ds2 = new DataSet();
            ds2.Tables.Clear();

            adapter1.SelectCommand.Parameters.Add("@Team", SqlDbType.Int).Value = _teamID;
            adapter1.SelectCommand.Parameters.Add("@TeamType", SqlDbType.Int).Value = teamType;

            try
            {
                ConnectionSQL.Open();
                int affectedRows = adapter1.Fill(ds2, "t2");
                if (affectedRows != 0)
                {
                    tbl = ds2.Tables["t2"];
                }
            }
            catch (SqlException ex)
            {
            }
            finally
            {
                ConnectionSQL.Close();
            }
            return tbl;
        }
    }
}