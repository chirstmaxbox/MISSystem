using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace EmployeeDomain.BLL
{
 //Quota Original Data according to Year
    public class SalesQuotaOriginalData

    {
        public DataTable QuataDatatable
        {
            get { return GetQuotaOriginalDataTable(); }
        }

        private readonly int _year = 2011;

        public SalesQuotaOriginalData(int year)
        {
            _year = year;
        }


        private DataTable GetQuotaOriginalDataTable()
        {
            SqlConnection ConnectionSQL = new SqlConnection(EmployeeConfiguration.ConnectionString);
            DataTable tbl = null;
            string SelectString = "SELECT * FROM Sales_Quota WHERE Year=@Year";
            SqlCommand SelectCommand = new SqlCommand(SelectString, ConnectionSQL);
            SqlDataAdapter da = new SqlDataAdapter(SelectCommand);
            DataSet ds1 = new DataSet();
            ds1.Tables.Clear();
            da.SelectCommand.Parameters.Add("@Year", SqlDbType.Int).Value = _year;
            try
            {
                //Get Existing Data
                int rowsAffected = da.Fill(ds1, "t1");

                //Add DataRows for each employee
                if (rowsAffected > 0)
                {
                    tbl = ds1.Tables["t1"];
                }

            }
            catch
            {
                string errorlog = "";
            }
            finally
            {
            }

            return tbl;

        }

    }
}
