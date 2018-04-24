using System;
using System.Data;
using System.Data.SqlClient;
using CustomerDomain.BO;

namespace CustomerDomain.BDL
{
    public class CustomerDataRow
    {

        public DataRow CustomerRow { get; private set; }
     
        public CustomerDataRow(int rowID)
        {

            CustomerRow = GetCustomerDataRow(rowID);
        }    

        private DataRow GetCustomerDataRow(int rowID)
        {
            //ByVal jobID As Integer
            //1.1 list all records in dataset
            var connectionSql = new SqlConnection(CustomerDomainConfiguration.ConnectionString);
            DataRow row = null;
            const string sqlSelectString = "SELECT * FROM [CUSTOMER] WHERE ([ROWID] = @ROWID)";
            var selectCommand = new SqlCommand(sqlSelectString, connectionSql);
            var adapter1 = new SqlDataAdapter(selectCommand);
            var ds1 = new DataSet();
            ds1.Tables.Clear();

            adapter1.SelectCommand.Parameters.Add("@ROWID", SqlDbType.Int).Value = rowID ;

            try
            {
                connectionSql.Open();
                var rowAffected = adapter1.Fill(ds1, "t1");
                if (rowAffected > 0)
                {
                    row = ds1.Tables["t1"].Rows[0];
                    if (Convert.ToInt32(row["rowID"]) == 0)
                    {
                        row = null;
                    }
                }

            }
            catch (SqlException ex)
            {
                string errorLog = ex.Message;

            }
            finally
            {
                connectionSql.Close();
            }

            return row;
        }


    }
}