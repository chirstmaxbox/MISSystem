
using System.Data;
using System.Data.SqlClient;
using System.Security.Principal;

namespace EmployeeDomain.BLL
{
    public class FsMembershipMgt
    {
        private readonly int _eNumber;

        public FsMembershipMgt(string un)
        {
            var fe = new FsEmployee(un);
            _eNumber = fe.EmployeeNumber;
        }

        public FsMembershipMgt(IPrincipal u)
        {
            var fe = new FsEmployee(u.Identity.Name);
            _eNumber = fe.EmployeeNumber;
        }


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
                    InsertCommand.Parameters.Add("@EmployeeNumber", SqlDbType.Int).Value = _eNumber;
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
    }
}

