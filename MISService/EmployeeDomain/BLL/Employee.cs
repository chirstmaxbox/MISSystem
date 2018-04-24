using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using EmployeeDomain.Models;
using MyCommon;
using System.Configuration;


namespace EmployeeDomain.BLL
{

    public class LabourTicketEmployee
    {
        private readonly DataRow _row;

        public int EmployeeNumber { get; private set; }
        public int DepartmentID { get; private set; }

        public LabourTicketEmployee(string un)
        {
            _row = GetUserDataRow(un);
            if (_row != null)
            {
                EmployeeNumber = Convert.ToInt32(_row["EmployeeNumber"]);
                DepartmentID = Convert.ToInt32(_row["dID"]);
            }
            else
            {
                EmployeeNumber = 0;
                DepartmentID = 0;
            }
        }

   
        private DataRow GetUserDataRow(string un)
        {
            DataRow row = null;
            var ConnectionSQL = new SqlConnection(EmployeeConfiguration.ConnectionString);
            string SqlSelectString = "SELECT * FROM FW_EMPLOYEES WHERE NickName=@UserName";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter = new SqlDataAdapter(SelectCommand);
            var ds2 = new DataSet();
            ds2.Tables.Clear();
            adapter.SelectCommand.Parameters.Add("@UserName", SqlDbType.NVarChar, 50).Value = un;
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

       

    }


    public class FsEmployee
    {
        #region "Properties"

        private readonly int _EmployeeNumber;
        private readonly string _NickName = "WhoAreYou";
        private readonly int _dID;
        private readonly int _role = (int) EmployeeEN.NEmployeeRole.General;

        private readonly DataRow _row;
        private readonly int _teamID;
        private string _UserName = "NotExist";


        public string UserName
        {
            //UserName = _UserName
            get { return _UserName; }
            set { _UserName = value; }
        }

        public int EmployeeNumber
        {
            get { return _EmployeeNumber; }
        }


        public string NickName
        {
            get { return _NickName; }
        }


        public int dID
        {
            get { return _dID; }
        }


        public int Role
        {
            get { return _role; }
        }

        public int TeamID
        {
            get { return _teamID; }
        }

        public string Email
        {
            get { return GetEmail(); }
        }

        public int GetMyAeID(int selectType)
        {
            var AeID = (int) EmployeeEN.NEmployeeIDDefault.NullSales110;
            var salesTeam = new FsEmployeeTeam(_teamID);
            DataTable tbl = salesTeam.GetAeList(selectType);
            if (tbl != null)
            {
                DataRow row = tbl.Rows[0];
                AeID = Convert.ToInt32(row["EmployeeNumber"]);
            }
            return AeID;
        }



		public string GetCompanyPhoneExtension()
		{
			string ext = "";
			if (_row != null)
			{
				if (!MyConvert.IsNullString(_row["Extension"]))

					ext = Convert.ToString(_row["Extension"]);
			}
			return ext;
		}

        private string GetEmail()
        {
            string email = "";
            if (_row != null)
            {
                if (!MyConvert.IsNullString(_row["CompanyEmail"]))

                    email = Convert.ToString(_row["CompanyEmail"]);
            }
            return email;
        }

        public int GetPayrollNumber()
        {
            return MyConvert.ConvertToInteger(_row["PayrollNumber"]);
        }

        #endregion

        #region "Construction"

        //Init User Name
        //username
        
        public FsEmployee(string un)
        {
            //Remove DomainName
            string userName = GetUserName(un);
            _row = GetUserDataRow(userName);
            
            _UserName =un;

            if (_row != null)
            {
                _EmployeeNumber = Convert.ToInt32(_row["EmployeeNumber"]);
                _NickName = Convert.ToString(_row["NickName"]);
                _role = Convert.ToInt32(_row["Role"]);
                _dID = Convert.ToInt32(_row["dID"]);
                _teamID = Convert.ToInt32(_row["Team"]);
            }
            else
            {
                _EmployeeNumber = 0;
                _NickName = "Who";
            }
        }

        //Employee Number
        public FsEmployee(int eNo)
        {
            _row = GetUserDataRow(eNo);
            _EmployeeNumber = eNo;

            if (_row != null)
            {
                if (Convert.IsDBNull(_row["UserName"]))
                {
                    _UserName = "";
                }
                else
                {
                    _UserName = Convert.ToString(_row["UserName"]);
                }

                if (Convert.IsDBNull(_row["NickName"]))
                {
                    _NickName = "";
                }
                else
                {
                    _NickName = Convert.ToString(_row["NickName"]);
                }

                if (!Convert.IsDBNull(_row["Team"]))
                {
                    _teamID = Convert.ToInt32(_row["Team"]);
                }
                _role = Convert.ToInt32(_row["Role"]);
                _dID = Convert.ToInt32(_row["dID"]);
            }
            else
            {
                _UserName = "NoExist";
                _NickName = "Who";
            }
        }


        public string GetUserName(string un)
        {

            char[] splitter = {','};
            
            string[] serverPrefix = EmployeeConfiguration.ServerPrefix.Split(splitter);

            string userName = un;
            int i = userName.IndexOf("\\");
            if (i > 0)
            {
                userName = userName.Remove(0, i + 1);
            }
            else
            {
                userName = userName.ToLower();
                for (i = 0; i <= serverPrefix.Length - 1; i++)
                {
                    string s = serverPrefix[i];
                    s = s.ToLower();
                    bool b = userName.Contains(s);
                    if (b)
                    {
                        userName = userName.Replace(s, "");
                    }
                }
            }

            return userName;
        }

        #endregion

        #region "Datarow"

        private DataRow GetUserDataRow(string un)
        {
            DataRow row = null;
            var ConnectionSQL = new SqlConnection(EmployeeConfiguration.ConnectionString);
             string SqlSelectString = "SELECT * FROM FW_EMPLOYEES WHERE UserName=@UserName";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter = new SqlDataAdapter(SelectCommand);
            var ds2 = new DataSet();
            ds2.Tables.Clear();
            adapter.SelectCommand.Parameters.Add("@UserName", SqlDbType.NVarChar, 50).Value = un;
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

        private DataRow GetUserDataRow(int eID)
        {
            DataRow row = null;
            var ConnectionSQL = new SqlConnection(EmployeeConfiguration.ConnectionString);
            string SqlSelectString = "SELECT * FROM FW_EMPLOYEES WHERE EmployeeNumber=@EmployeeNumber";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter = new SqlDataAdapter(SelectCommand);
            var ds2 = new DataSet();
            ds2.Tables.Clear();
            adapter.SelectCommand.Parameters.Add("@EmployeeNumber", SqlDbType.Int).Value = eID;
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

        #endregion



        #region "Department"

        public static void SetupDepartmentID()
        {
            int rowsAffected = 0;

            var ConnectionSQL = new SqlConnection(EmployeeConfiguration.ConnectionString);
            string SqlSelectString = "SELECT EmployeeNumber,Department, DepartmentID,dID FROM FW_EMPLOYEES";
            // WHERE UserName=@UserName"

            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand);
            var ds1 = new DataSet();
            ds1.Tables.Clear();
        
            try
            {
                ConnectionSQL.Open();
                rowsAffected = adapter1.Fill(ds1, "t1");

                DataRow row = null;
                foreach (DataRow row_loopVariable in ds1.Tables["t1"].Rows)
                {
                    row = row_loopVariable;
                    string dName = Convert.ToString(row["department"]);
                    switch (dName)
                    {
                        case "Art Room":
                            row["did"] = EmployeeEN.NDepartmentID.ArtRoom;
                            break;
                        case "Metal":
                            row["did"] = EmployeeEN.NDepartmentID.Metal;
                            break;
                        case "Painting":
                            row["did"] = EmployeeEN.NDepartmentID.Painting;
                            break;
                        case "Plastic A":
                            row["did"] = EmployeeEN.NDepartmentID.PlasticA;
                            break;
                        case "Plastic B":
                            row["did"] = EmployeeEN.NDepartmentID.PlasticB;
                            break;
                        case "RA3":
                            row["did"] = EmployeeEN.NDepartmentID.RA3;
                            break;
                        case "Installation":
                            row["did"] = EmployeeEN.NDepartmentID.Installation;

                            break;
                        case "Site Check":
                            row["did"] = EmployeeEN.NDepartmentID.SiteCheck;
                            break;
                        case "Installation B":
                            row["did"] = EmployeeEN.NDepartmentID.InstallationB;
                            break;
                        case "Workshop":
                            row["did"] = EmployeeEN.NDepartmentID.Workshop;
                            break;
                        case "Q & D":
                            row["did"] = EmployeeEN.NDepartmentID.QD;
                            break;
                        case "Sales & Marketing":
                            row["did"] = EmployeeEN.NDepartmentID.SalesMarketing;
                            break;
                        case "Purchasing":
                            row["did"] = EmployeeEN.NDepartmentID.Purchasing;
                            break;
                        case "ADMIN&HR":
                            row["did"] = EmployeeEN.NDepartmentID.AdminHr;
                            break;
                        case "ACCOUNTING":
                            row["did"] = EmployeeEN.NDepartmentID.Account;
                            break;
                    }
                }

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


        public static DataTable GetEmployeeDepartment()
        {
            DataTable t1 = null;

            int rowsAffected = 0;

            var ConnectionSQL = new SqlConnection(EmployeeConfiguration.ConnectionString);
            string SqlSelectString = "SELECT EmployeeNumber,Department, DepartmentID,dID FROM FW_EMPLOYEES";
            // WHERE UserName=@UserName"

            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter1 = new SqlDataAdapter(SelectCommand);
            var ds1 = new DataSet();
            ds1.Tables.Clear();
      
            try
            {
                ConnectionSQL.Open();
                rowsAffected = adapter1.Fill(ds1, "t1");
                foreach (DataRow row in ds1.Tables["t1"].Rows)
                {
                    string dName = Convert.ToString(row["department"]);
                    switch (dName)
                    {
                        case "ArtRoom":
                            row["did"] = EmployeeEN.NDepartmentID.ArtRoom;
                            break;
                        case "Metal":
                            row["did"] = EmployeeEN.NDepartmentID.Metal;
                            break;
                        case "Painting":
                            row["did"] = EmployeeEN.NDepartmentID.Painting;
                            break;

                        case "Plastic A":
                            row["did"] = EmployeeEN.NDepartmentID.PlasticA;
                            break;
                        case "Plastic B":
                            row["did"] = EmployeeEN.NDepartmentID.PlasticB;
                            break;
                        case "RA3":
                            row["did"] = EmployeeEN.NDepartmentID.RA3;
                            break;
                        case "Installation":
                            row["did"] = EmployeeEN.NDepartmentID.Installation;

                            break;
                        case "Site Check":
                            row["did"] = EmployeeEN.NDepartmentID.SiteCheck;
                            break;
                        case "Installation B":
                            row["did"] = EmployeeEN.NDepartmentID.InstallationB ;
                            break;
                        case "Workshop":
                            row["did"] = EmployeeEN.NDepartmentID.Workshop;
                            break;
                        case "Q&D":
                            row["did"] = EmployeeEN.NDepartmentID.QD;
                            break;
                        case "Sales & Marketing":
                            row["did"] = EmployeeEN.NDepartmentID.SalesMarketing;
                            break;
                        case "Purchasing":
                            row["did"] = EmployeeEN.NDepartmentID.Purchasing;
                            break;
                        case "ADMIN&HR":
                            row["did"] = EmployeeEN.NDepartmentID.AdminHr;
                            break;
                        case "ACCOUNTING":
                            row["did"] = EmployeeEN.NDepartmentID.Account;
                            break;
                    }
                }

                t1 = ds1.Tables["t1"];
            }
            catch (SqlException ex)
            {
                string errorLog = ex.Message;
            }
            finally
            {
                ConnectionSQL.Close();
            }

            return t1;
        }

        #endregion
    }



    public class FsEmployeeByNickName
    {
        public int EmployeeNumber { get; private set; }

        // input NickName and DepartmentID, to check is this employee Exist
        public FsEmployeeByNickName(string nickName)
        {
            EmployeeNumber = 0;
            var dc = new EmployeeDbModelEntities();
            var emps =dc.FW_Employees.Where(x => x.NickName == nickName.Trim()).OrderByDescending(x => x.dID).ToArray();
            if (emps.Count() > 0)
            {
                EmployeeNumber = emps[0].EmployeeNumber;
             }
        }

        
                public FsEmployeeByNickName(string nickName, int dID)
        {
            EmployeeNumber = 0;
            var dc = new EmployeeDbModelEntities();
            var emps =dc.FW_Employees.Where(x => x.NickName == nickName.Trim() && x.dID ==dID).OrderBy(x =>x.dID).ToArray();
            if (emps.Count() > 0)
            {
                EmployeeNumber = emps[0].EmployeeNumber;
             }
        }
    }
}