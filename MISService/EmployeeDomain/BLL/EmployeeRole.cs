using System;
using System.Data;
using System.Data.SqlClient;

namespace EmployeeDomain.BLL
{
    /// <summary>
    /// Is In Roles   
    /// </summary>

    public class EmployeeRole
    {
        private readonly int _employeeNumber;

        public EmployeeRole(int employeeNumber)
        {
            _employeeNumber = employeeNumber;
        }

        public bool IsInRoles(string[] enabledRoleArray)
        {
            return GetIsInRoles(enabledRoleArray);
        }

        //is this user in Roles which argument is a Array ("SalesManage"....)
        private bool GetIsInRoles(string[] roleArray)
        {
            DataTable dt = GetRoleDataTable();

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string employeeRole = Convert.ToString(row["RoleName"]);
                    int i = Array.IndexOf(roleArray, employeeRole);
                    if (i >= 0) return true;
                }
            }
            return false;
        }


        private DataTable GetRoleDataTable()
        {
            //employeenumber
            DataTable t1 = null;
            var connectionSQL = new SqlConnection(EmployeeConfiguration.ConnectionString);
            const string sqlSelectString = "SELECT FW_EmployeesInRole.RoleID, FW_Roles.RoleName FROM FW_EmployeesInRole INNER JOIN FW_Roles ON FW_EmployeesInRole.RoleID = FW_Roles.RoleID WHERE (FW_EmployeesInRole.EmployeeNumber = @EmployeeNumber)";
            var selectCommand = new SqlCommand(sqlSelectString, connectionSQL);
            var adapter = new SqlDataAdapter(selectCommand);
            var ds2 = new DataSet();
            ds2.Tables.Clear();
            adapter.SelectCommand.Parameters.Add("@EmployeeNumber", SqlDbType.Int).Value = _employeeNumber;
            try
            {
                connectionSQL.Open();
                int affectedRows = adapter.Fill(ds2, "t2");
                if (affectedRows != 0)
                {
                    t1 = ds2.Tables["t2"];
                }
            }
            catch (SqlException ex)
            {
                connectionSQL.Close();
            }

            return t1;
        }
    }
}