using System;
using System.Data;
using System.Data.SqlClient;

using System.Web.UI.WebControls;
using MyCommon;

namespace EmployeeDomain.BLL
{

    public abstract class EmployeeDropdownlistExceptionHandler
    {
        //1)select employeelist for this dropdown
        //2)Read ExistingEmployeeID in the designed module Record
        //3) fillout the dropdownlist and set the existingEmployeeID as selected if it's fit
        //

        //private readonly DataTable _tbl;

        //private readonly int _existingEmployeeID = (int) EmployeeEN.NEmployeeIDDefault.NullSales110;
        //private int _parameterToSelectEmployeeList;
        //private int _recordIdToSelectExistingEmployee;

        //public EmployeeDropdownlistExceptionHandler(int parameterToSelectEmployeeList, int recordIdToSelectExistingEmployee)
        //{
        //    _parameterToSelectEmployeeList = parameterToSelectEmployeeList;
        //    _recordIdToSelectExistingEmployee = recordIdToSelectExistingEmployee;
        //}

      
        public virtual DataTable GetEmployeeList()
        {
            //Did not user ParameterToSelectEmployeeList, which should be Role=1(AE) or Role=2 (OP)
            //Mix all AE and OP
            DataTable tbl = null;
            var connectionSQL = new SqlConnection(EmployeeConfiguration.ConnectionString);
            const string sqlSelectString = "SELECT EmployeeNumber AS Value, NickName AS Text FROM FW_Employees WHERE ((Role = 2 or Role=1) AND (Active = 1) AND (dID = 63) ) OR (EmployeeNumber = 110 OR EmployeeNumber = 0) ORDER BY Text";
            var selectCommand = new SqlCommand(sqlSelectString, connectionSQL);
            var adapter = new SqlDataAdapter(selectCommand);
            var ds2 = new DataSet();
            ds2.Tables.Clear();
            try
            {
                connectionSQL.Open();
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
                connectionSQL.Close();
            }
            return tbl;

        }

        public abstract int GetExistingEmployeeID();

        public void SetDropdownlistSelectValue(DropDownList ddl)
        {
            var tbl = GetEmployeeList();
           var existingEmployeeID = GetExistingEmployeeID();            
            
            ddl.SelectedValue = EmployeeEN.NEmployeeIDDefault.Null.ToString();
            var existing = false;
            foreach (DataRow row in tbl.Rows)
            {
                //Fill out
                var wildcard = new ListItem
                                   {
                                       Text =Convert.ToString( row["Text"]), 
                                       Value =Convert .ToString(  row["Value"])
                                   };
                ddl.Items.Insert(ddl.Items.Count, wildcard);

                //setup selectedvalue
                if (existingEmployeeID ==Convert .ToInt32(  row["Value"]))
                {
                    ddl.SelectedValue =Convert .ToString( row["Value"]);
                    existing = true;
                }
            }

            if (existing) return;
            //handling except
            var selectedEmployee = new FsEmployee(existingEmployeeID);
            if (!MyConvert.IsNullString(selectedEmployee.NickName))
            {
                var wildcard = new ListItem {
                    Text = selectedEmployee.NickName,
                    Value =Convert.ToString(existingEmployeeID)
                };
                ddl.Items.Insert(ddl.Items.Count, wildcard);
                ddl.SelectedValue =Convert.ToString( existingEmployeeID);
            }
        }
    }

 
}
