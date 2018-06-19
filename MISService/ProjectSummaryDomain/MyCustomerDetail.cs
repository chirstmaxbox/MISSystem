using System;
using System.Data;
using System.Data.SqlClient;

namespace ProjectSummaryDomain
{
    public class MyCustomerDetail
    {
        private readonly DataRow _row;
        public MyCustomerDetail(int rowID)
        {
            _row = GetCustomerDataRow(rowID); 
        }

        private DataRow GetCustomerDataRow(int rowID)
        {

            var connectionSql = new SqlConnection(MyConfiguration.ConnectionString);
            DataRow row = null;
            const string sqlSelectString = "SELECT * FROM [CUSTOMER] WHERE ([ROWID] = @ROWID)";
            var selectCommand = new SqlCommand(sqlSelectString, connectionSql);
            var adapter1 = new SqlDataAdapter(selectCommand);
            var ds1 = new DataSet();
            ds1.Tables.Clear();

            adapter1.SelectCommand.Parameters.Add("@ROWID", SqlDbType.Int).Value = rowID;

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



        public string GetCompanyName()
        {
            string s = "";
            if (_row != null)
            {
                if (!Convert.IsDBNull(_row["NAME"]))
                {
                    s = Convert.ToString(_row["NAME"]);
                }
            }

            return s;
        }



        public string GetAddress()
        {
            if (_row == null) return "";

   
                var address =ConvertToString( _row["Addr_1"]);
             var temp =ConvertToString( _row["Addr_2"]);

            if (temp.Length >2)
                {
                    address += "  " + temp;
                }

                temp = ConvertToString(_row["CITY"]);
                if (temp.Length > 1)
                {
                    address += "  " + temp ;
                }

                temp = ConvertToString(_row["STATE"]);
                if (temp.Length > 1)
                {
                    address += ", " + temp;
                }

                temp = ConvertToString(_row["ZIPCODE"]);
                if (temp.Length > 1)
                {
                    address += ", " + temp;
                }

            return address;

        }




    private string ConvertToString(object obj)
    {
        return Convert.IsDBNull(obj)?"":Convert .ToString( obj);
    }

        
    }
    }