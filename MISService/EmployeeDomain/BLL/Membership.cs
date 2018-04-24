using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Principal;

namespace EmployeeDomain.BLL
{
    // Windows Authentication
    // Custom user Role Provider

    public class FsMembership
    {
        #region "Properties"

        //All authroles should be here
        public enum AuthenticationType
        {
            MultipleSales = 1,
            Admin = 10,

            Accounting = 30,
            Production = 40,
            Installation = 50,
            Estimation = 60,
            ArtDrawing = 70,
            StructuralDrawing = 80,
            HR = 90
        }


        private readonly int _EmployeeNumber;

        private readonly string _NickName = "WhoAreYou";
        private string _UserName = "NotExist";

        public string UserName
        {
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

        #endregion

        #region "Construction"

        //Init User Name
        //username
        public FsMembership(string un)
        {
            DataRow row = GetUserDataRow(un);
            _UserName = un;
            if (row != null)
            {
                _EmployeeNumber = Convert.ToInt32(row["EmployeeNumber"]);
                _NickName = Convert.ToString(row["NickName"]);
            }
            else
            {
                _EmployeeNumber = 0;
                _NickName = "Who";
            }
        }

        public FsMembership(IPrincipal u)
        {
            string un = GetUserName(u);
            DataRow row = GetUserDataRow(un);
            _UserName = un;
            if (row != null)
            {
                _EmployeeNumber = Convert.ToInt32(row["EmployeeNumber"]);
                _NickName = Convert.ToString(row["NickName"]);
            }
            else
            {
                _EmployeeNumber = 0;
                _NickName = "Who";
            }
        }

        #endregion

        #region "Select"

        private DataRow GetUserDataRow(string un)
        {
            DataRow row = null;
            var ConnectionSQL = new SqlConnection(EmployeeConfiguration.ConnectionString);
            string SqlSelectString ="SELECT EmployeeNumber, NickName,UserName FROM FW_EMPLOYEES WHERE UserName=@UserName";
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
                ConnectionSQL.Close();
            }
            return row;
        }


        private DataTable GetRoleDataTable()
        {
            //employeenumber
            DataTable t1 = null;
            var ConnectionSQL = new SqlConnection(EmployeeConfiguration.ConnectionString);
            string SqlSelectString =
                "SELECT FW_EmployeesInRole.RoleID, FW_Roles.RoleName FROM FW_EmployeesInRole INNER JOIN FW_Roles ON FW_EmployeesInRole.RoleID = FW_Roles.RoleID WHERE (FW_EmployeesInRole.EmployeeNumber = @EmployeeNumber)";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter = new SqlDataAdapter(SelectCommand);
            var ds2 = new DataSet();
            ds2.Tables.Clear();
            adapter.SelectCommand.Parameters.Add("@EmployeeNumber", SqlDbType.Int).Value = EmployeeNumber;
            try
            {
                ConnectionSQL.Open();
                int affectedRows = adapter.Fill(ds2, "t2");
                if (affectedRows != 0)
                {
                    t1 = ds2.Tables["t2"];
                }
            }
            catch (SqlException ex)
            {
                ConnectionSQL.Close();
            }

            return t1;
        }


        public string GetUserName(IPrincipal u)
        {
            char[] splitter = {','};
            string[] serverPrefix = EmployeeConfiguration.ServerPrefix.Split(splitter);
            string userName = u.Identity.Name;
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

             //'is this user in Roles which argument is a Array ("SalesManage"....)
        public bool IsInRoles(Array roleName)
        {
            bool inRole = false;
            DataTable t1 = GetRoleDataTable();
            if (t1 != null)
            {
                foreach (DataRow row in t1.Rows)
                {
                    string employeeRole = Convert.ToString(row["RoleName"]);
                    int i = Array.IndexOf(roleName, employeeRole);
                    if (i >= 0)
                    {
                        inRole = true;
                        break;
                    }
                }
            }
            return inRole;
        }

        #endregion

        #region "Insert "

        //Maintenance
        public void InsertEmployeesInRole(int roleID)
        {
            //Insert documents for items
            using (var Connection = new SqlConnection(EmployeeConfiguration.ConnectionString))
            {
                string InsertString =
                    "INSERT INTO [FW_EmployeesInRole] ([RoleID], [EmployeeNumber]) VALUES (@RoleID, @EmployeeNumber)";
                var InsertCommand = new SqlCommand(InsertString, Connection);
                try
                {
                    Connection.Open();
                    InsertCommand.Parameters.Add("@RoleID", SqlDbType.Int).Value = roleID;
                    InsertCommand.Parameters.Add("@EmployeeNumber", SqlDbType.Int).Value = EmployeeNumber;
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

        #endregion
    }
}