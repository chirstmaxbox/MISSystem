
using System.Data;
using System.Data.SqlClient;

namespace EmployeeDomain.BLL
{
    public abstract class FsEmployeeList
    {
        //To solve the dropdownlist problem
        //Select value not in dropdownlist
        //

        public int SelectParamerter;
        private DataTable _tbl;

        public DataTable List
        {
            get
            {
                _tbl = GetEmployeeList();
                return _tbl;
            }
        }

        public abstract DataTable GetEmployeeList();
    }

    public class FsEmployeeListByRole : FsEmployeeList
    {
        public FsEmployeeListByRole(int roleID)
        {
            SelectParamerter = roleID;
        }

        public override DataTable GetEmployeeList()
        {
            DataTable tbl = null;
            var ConnectionSQL = new SqlConnection(EmployeeConfiguration.ConnectionString);
            string SqlSelectString ="SELECT EmployeeNumber as Value, NickName as Text,dID FROM FW_EMPLOYEES WHERE Role=@Role";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter = new SqlDataAdapter(SelectCommand);
            var ds2 = new DataSet();
            ds2.Tables.Clear();
            adapter.SelectCommand.Parameters.Add("@Role", SqlDbType.Int).Value = SelectParamerter;
            try
            {
                ConnectionSQL.Open();
                int affectedRows = adapter.Fill(ds2, "t2");
                if (affectedRows != 0)
                {
                    tbl = ds2.Tables["t2"];
                }
            }
            catch (SqlException ex)
            {
                string errormMsg = ex.Message;
            }
            finally
            {
                ConnectionSQL.Close();
            }
            return tbl;
        }
    }
}