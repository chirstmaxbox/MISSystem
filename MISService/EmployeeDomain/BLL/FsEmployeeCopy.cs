
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace EmployeeDomain.BLL
{
    public class FsEmployeeCopy
    {
        public bool IsOperationSuccessfull { get; private set; }

        private readonly int _eID;

        public FsEmployeeCopy(int eID)
        {
            _eID = eID;
        }

        public void ToNewUser(MyNewEmployeeParameter myNewEmployeeParameter )
        {
            var connectionSQL = new SqlConnection(EmployeeConfiguration.ConnectionString);
            const string sqlSelectString = "SELECT * FROM FW_EMPLOYEES WHERE EmployeeNumber=@EmployeeNumber";
            var selectCommand = new SqlCommand(sqlSelectString, connectionSQL);
            var adapter = new SqlDataAdapter(selectCommand);
            var ds2 = new DataSet();
            ds2.Tables.Clear();
            adapter.SelectCommand.Parameters.Add("@EmployeeNumber", SqlDbType.Int).Value = _eID;
            //
            try
            {
                connectionSQL.Open();
                int affectedRows = adapter.Fill(ds2, "t2");

                if (affectedRows != 0)
                {
                    DataRow row = ds2.Tables["t2"].Rows[0];
                    DataRow rowNew = ds2.Tables["t2"].NewRow();

                    int itemIndex = ds2.Tables["t2"].Columns.Count;
                    //copy items of the row

                    for (int i = 1; i <= itemIndex - 1; i++)
                    {
                        rowNew[i] = row[i];
                    }

                    var newNumber = myNewEmployeeParameter.GetNextAvailableEmployeeID();
                    rowNew["EmployeeNumber"] = newNumber;
                    rowNew["NickName"] = myNewEmployeeParameter.EmployeeNickName ;
                    rowNew["UserName"] = "Logon UserName";
                    rowNew["Signature"] = null;
                    rowNew["NumberInfor"] = string.Format("{0:D4}", newNumber );
                    rowNew["Password"] = "p" + string.Format("{0:D4}", newNumber);
                    rowNew["PayrollNumber"] = myNewEmployeeParameter .GetNextAvailablePayrollID() ;
                    
                    ds2.Tables["t2"].Rows.Add(rowNew);

                    //1.3. Write  back to DB
                    var cb = new SqlCommandBuilder(adapter);
                    adapter = cb.DataAdapter;
                    affectedRows = adapter.Update(ds2, "t2");
                }
            }
            catch (SqlException ex)
            {
                string errorlog = ex.Message;
            }
            finally
            {
                connectionSQL.Close();
            }
        }
    }


    public abstract class MyNewEmployeeParameter
    {
        public string EmployeeNickName { get; set; }
        private const int StartEmployeeID = 501;
        public abstract int GetNextAvailablePayrollID();
        
        public  int GetNextAvailableEmployeeID()
        {
            int j = StartEmployeeID;
            var dc = new Models.EmployeeDbModelEntities( ) ;
            var eIDs =dc.FW_Employees.Where(c => c.EmployeeNumber >= StartEmployeeID).OrderBy(x => x.EmployeeNumber).Select(
                    x => x.EmployeeNumber).ToList();
            for (int i = StartEmployeeID; i <= 9900; i++)
            {
                if (!eIDs.Contains(i))
                {
                    j = i;
                    break;
                }
            }
            return j;
        }
    }

    public class MyNewEmployeeParameterWithPayrollParameterStartFrom4 : MyNewEmployeeParameter
    {
        public override int GetNextAvailablePayrollID()
        {
            int j = 4000;
            var dc = new Models.EmployeeDbModelEntities( ) ;
            var emps = dc.FW_Employees.Where(x => x.PayrollNumber >= 4000 && x.PayrollNumber < 5999).ToList();
            if (emps.Count > 0)
            {
                j = emps.Max(x => x.PayrollNumber);
            }

            return j + 1;
        }
    }


    public class MyNewEmployeeParameterWithPayrollParameterStartFrom2 : MyNewEmployeeParameter
    {
        public override int GetNextAvailablePayrollID()
        {
             int j = 2000;
             var dc = new Models.EmployeeDbModelEntities();
             var emps = dc.FW_Employees.Where(x => x.PayrollNumber >= 2000 && x.PayrollNumber < 3999).ToList();
             if (emps.Count > 0)
             {
                 j = emps.Max(x => x.PayrollNumber);
             }

            return j + 1;
        }
    }


    public class MyNewEmployeeParameterWithPayrollParameterStartFrom1 : MyNewEmployeeParameter
    {
        public override int GetNextAvailablePayrollID()
        {
            int j = 1000;
            var dc = new Models.EmployeeDbModelEntities();
            var emps = dc.FW_Employees.Where(x => x.PayrollNumber >= 1000 && x.PayrollNumber < 1999).ToList();
            if (emps.Count > 0)
            {
                j = emps.Max(x => x.PayrollNumber);
            }

            return j + 1;
        }
    }


}